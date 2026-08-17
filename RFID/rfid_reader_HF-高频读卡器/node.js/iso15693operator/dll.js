let ffi =require('ffi-napi');
let ref =require('ref-napi');
let refArray =require('ref-array-napi');


let path='32dll';

var intPtr = ref.refType(ref.types.int64);
var bytePtr = ref.refType(ref.types.byte);
var aPtr = ref.refType(refArray('byte'));
  
var DLL = ffi.Library(path+'/rfidlib_reader.dll', {
	'RDR_LoadReaderDrivers':['int',['string']],//加载驱动
	'RDR_Open' : ['int', ['string',intPtr]],//连接驱动
	'RDR_Close' : ['int64', ['int64']],//关闭驱动
	'RDR_GetLoadedReaderDriverCount':['int',[]],
	'COMPort_Enum':['int',[]],
	'RDR_CreateInvenParamSpecList':['int64',[]],
	'RDR_TagInventory':['int64',['int64','byte','byte','byte','int64']],
	'RDR_GetTagDataReport':['int64',['int64','byte']],
	'RDR_TagDisconnect':['int64', ['int64','int64']],
	'DNODE_Destroy':['int64', ['int64']],
	'COMPort_GetEnumItem':['int',['int',aPtr,'int']],
	'RDR_SetAcessAntenna':['int64',['int64','byte']]
});

var ISO15693 = ffi.Library(path+'/rfidlib_aip_iso15693.dll', {
	'ISO15693_CreateInvenParam':['int64',['int64','byte','byte','byte','byte']],
	'ISO15693_ParseTagDataReport':['int64',['int64',intPtr,intPtr,intPtr,bytePtr,aPtr]],
	'ISO15693_Connect':['int64',['int64','int','byte',aPtr,intPtr]],//连接标签
	'ISO15693_WriteDSFID':['int',['int','int','byte']],//写dsfid 0在库 1借出
	'ISO15693_ReadMultiBlocks':['int64',['int64','int64','byte','int','int',intPtr,aPtr,'int',intPtr]],
	'ISO15693_WriteMultipleBlocks':['int',['int','int','int','int',aPtr,'int']],
	'ISO15693_WriteAFI':['int',['int','int','byte']],
	'NXPICODESLI_EableEAS':['int',['int','int']],  
	'NXPICODESLI_DisableEAS':['int',['int','int']]  
});



var ISO14443A=ffi.Library(path+'/rfidlib_aip_iso14443A.dll',{
'ISO14443A_ParseTagDataReport': ['int',['int',intPtr,intPtr,intPtr,bytePtr,aPtr]],
'ISO14443A_CreateInvenParam': ['int',['int','byte']],
'ISO14443A_Connect':['int',['int','byte',aPtr,'byte',intPtr]]

})




function fromCharCode(data){
    var str='';
	for(var i in data){
		if(data[i]>0){
			str+=String.fromCharCode(data[i]);
		}
    }
   return str;
}

function sleep(ms){
	var curTime = new Date().getTime();//当前时间
	var wakeTime = curTime + ms;//苏醒时间
	while (true) {
		curTime = new Date().getTime();
		if (curTime >= wakeTime) {
			return;
		}
	}
}


exports.fromCharCode=fromCharCode;


function getCommNameArr()
{
	
	let cnt=DLL.COMPort_Enum();
	var comArr=[];
	for(i=0;i<cnt;i++)
	{
		
			let comName = ref.alloc(refArray('byte',64));
			DLL.COMPort_GetEnumItem(i,comName,64);
			comArr.push(fromCharCode(comName));	
	}
	return JSON.stringify(comArr);
}

exports.getCommNameArr=getCommNameArr;

 
function GetHexByte(str) 
{
	var len=str.length;
	var hexbytes=[];
	for(i=0;i<len/2;i++)
	{
		var startpos=i*2;
		
		var endpos=0;
		if((i+1)*2<len)
		{
			endpos=(i+1)*2;
		}	
		else
		{
			endpos=len;
		}
		hexbytes.push(parseInt(str.slice(startpos,endpos),16));
	}
	return hexbytes;
}
exports.GetHexByte=GetHexByte;

function GetByteHexStr(arr)
{
	var str='';
	for(i=0;i<arr.length;i++)
	{
		var hex=arr[i].toString(16);

		if(hex.length==1)
			hex='0'+hex;
		str=str+hex;
	}
	
	return str;
}

exports.GetByteHexStr=GetByteHexStr;


function Inventory(connstr,ant)
{
	let handleRef = ref.alloc('int64',0) ;
	let iret = DLL.RDR_Open(connstr,handleRef);//打开连接串指定的设备驱动
	
	var operation={'error':0};
	
	console.log('RDR_Open iret:'+connstr+iret);
	
	
	if(iret==0)
	{
		console.log('链接设备成功'+ant);
		
	
	const header=handleRef.deref();
	console.log("句柄"+header);
	
	var dnInvenParamList=DLL.RDR_CreateInvenParamSpecList();//创建空中接口协议参数列表数据节点，不使用时需要调用DNODE_Destroy释放
	console.log(dnInvenParamList);

	ISO15693.ISO15693_CreateInvenParam(dnInvenParamList,0,0,0,0);//盘点指令
	console.log('链接设备成功11');

    let iret=DLL.RDR_TagInventory(header,1,0,null,dnInvenParamList);//寻找标签
	
	console.log('RDR_TagInventory iret'+iret);
	if(iret==0)
	{
	
		var uids=[];
		var uidStr=[];
		let flag=1;
		let dataReport=DLL.RDR_GetTagDataReport(header,flag);
		
		while(dataReport!=0)
		{
			console.log('RDR_GetTagDataReport');
			let aip_id = ref.alloc('int',0);
			let tag_id = ref.alloc('int',0);
			let ant_id = ref.alloc('int',0);
			let dsfid  = ref.alloc('byte',0);
			let uid    = ref.alloc(refArray('byte',8));//8限制数组长度
			iret=ISO15693.ISO15693_ParseTagDataReport(dataReport,aip_id, tag_id, ant_id, dsfid, uid);
			if(iret==0)
			{
				
				let data = JSON.parse(JSON.stringify(uid))['data'];
				
				uids.push(data);
				
			    console.log(data);
				
				uidStr.push(GetByteHexStr(data));
			}
			
			let	flag=2;
			dataReport=DLL.RDR_GetTagDataReport(header,flag);
		}
		
		operation['error']=0;

		if(uids.length>0)
		{
			operation['uids']=uids;
			operation['uidstrs']=uidStr;
		}
		
		console.log(operation);
		
	}
	else
	{
		operation['error']=iret;	
	}
	DLL.DNODE_Destroy(dnInvenParamList);//释放
	DLL.RDR_Close(header);
	
	}
	else
	{
		operation['error']=iret;
		
		return operation;
	}
	
	
	return operation;
}
exports.Inventory=Inventory;

function InitLibrary()
{
	let res=DLL.RDR_LoadReaderDrivers('./'+path+'/Drivers');
	
	return res==0?'success':'fail';
}

exports.InitLibrary=InitLibrary;


function ISO15693ReadMultiBlock(connstr,uid,ant,tagid,connectmode,startblock,blocknum)
{
	let handleRef = ref.alloc('int64',0) ;
	let iret = DLL.RDR_Open(connstr,handleRef);//打开连接串指定的设备驱动
	
	let header=handleRef.deref();
	
	var operation={'error':0};
	
	console.log('RDR_Open iret:'+iret);
	
	if(iret==0)
	{
		iret=DLL.RDR_SetAcessAntenna(header,ant);

		if(iret==0)
		{
		
		var uidbuff=Buffer.from(uid);
	
		const ht = ref.alloc('int64',0);
		console.log("ht"+ht);
		iret=ISO15693.ISO15693_Connect(header,tagid,connectmode,uidbuff,ht);//连接标签	
		
		if(iret==0)
		{
			
		const htHandle=ht.deref();
		console.log("htHandle"+htHandle);
		let blockreaded=ref.alloc('int64',0);
		let hData=ref.alloc(refArray('byte',4*blocknum));	
		let hDataReaded = ref.alloc('int64',0);
		
		iret=ISO15693.ISO15693_ReadMultiBlocks(header,htHandle,0,startblock,blocknum,blockreaded,hData,4*blocknum,hDataReaded);//读取标签数据
		
		if(iret==0)
		{
			console.log('ISO15693_ReadMultiBlocks :'+iret);
			operation['data']=JSON.parse(JSON.stringify(hData))['data'];
			operation['startblock']=startblock;
			operation['blocknum']=blocknum;
		}
		else
		{
			operation['error']=iret;

			console.log('ISO15693_ReadMultiBlocks iret:'+iret);	
		}
		DLL.RDR_TagDisconnect(header,htHandle);
		
		}
		
		}
		else
		{
			operation['error']=iret;
			console.log('RDR_SetAcessAntenna iret:'+iret);	

		}
		
		
		DLL.RDR_Close(header);
	}
	else
	{
		operation['error']=iret;
	}
	
	return JSON.stringify(operation);
	
}
exports.ISO15693ReadMultiBlock=ISO15693ReadMultiBlock;



function ISO15693WriteMultiBlock(connstr,uid,ant,tagid,connectmode,startblock,blocknum,writedata)
{
	let handleRef = ref.alloc('int64',0) ;
	let iret = DLL.RDR_Open(connstr,handleRef);//打开连接串指定的设备驱动
	
	let header=handleRef.deref();
	
	var operation={'error':0};
	
	console.log('RDR_Open iret:'+iret);
	
	if(iret==0)
	{
		iret=DLL.RDR_SetAcessAntenna(header,ant);

		if(iret==0)
		{
			console.log('uid:'+uid);
			console.log('data:'+writedata);
			
			var uidbuff=Buffer.from(uid);
			var writebuff=Buffer.from(writedata);
			
			
			let ht = ref.alloc('int64',0);
			iret=ISO15693.ISO15693_Connect(header,tagid,connectmode,uidbuff,ht);//连接标签	
			
			let htHandle=ht.deref();
			if(iret==0)	
			{
				iret=ISO15693.ISO15693_WriteMultipleBlocks(header,htHandle,startblock,blocknum,writebuff,writedata.length);
				
				if(iret==0)	
				{
					operation['error']=0;
				}
				else
				{
					operation['error']=iret;

					console.log('ISO15693_WriteMultipleBlocks iret:'+iret);	
				}
				
				DLL.RDR_TagDisconnect(htHandle);
			}
			else
			{
				operation['error']=iret;
			}
		}
		else
		{
			operation['error']=iret;
			console.log('RDR_SetAcessAntenna iret:'+iret);	
		}
		
		DLL.RDR_Close(header);
	}
	else
	{
		operation['error']=iret;
	}
	return JSON.stringify(operation);
}

exports.ISO15693WriteMultiBlock=ISO15693WriteMultiBlock;

function ISO15693WriteAFI(connstr,uid,ant,tagid,connectmode,writeafi)
{
	let handleRef = ref.alloc('int64',0) ;
	let iret = DLL.RDR_Open(connstr,handleRef);//打开连接串指定的设备驱动
	
	let header=handleRef.deref();
	
	var operation={'error':0};
	
	console.log('RDR_Open iret:'+iret);
	
	if(iret==0)
	{
		iret=DLL.RDR_SetAcessAntenna(header,ant);

		if(iret==0)
		{
			console.log('uid:'+uid);
		
			
			var uidbuff=Buffer.from(uid);
	
			let ht = ref.alloc('int64',0);
			iret=ISO15693.ISO15693_Connect(header,tagid,connectmode,uidbuff,ht);//连接标签	
			
			let htHandle=ht.deref();
			if(iret==0)	
			{
				iret=ISO15693.ISO15693_WriteAFI(header,htHandle,writeafi);
				
				if(iret==0)	
				{
					operation['error']=0;
				}
				else
				{
					operation['error']=iret;

					console.log('ISO15693_WriteAFI iret:'+iret);	
				}
				
				DLL.RDR_TagDisconnect(htHandle);
			}
			else
			{
				operation['error']=iret;
			}
		}
		else
		{
			operation['error']=iret;

			console.log('RDR_SetAcessAntenna iret:'+iret);	
		}
		
		DLL.RDR_Close(header);
	}
	else
	{
		operation['error']=iret;

		
	}
	return JSON.stringify(operation);
	
	
}
exports.ISO15693WriteAFI=ISO15693WriteAFI;	


function ISO15693ChangeEAS(connstr,uid,ant,tagid,connectmode,enable)
{

	let handleRef = ref.alloc('int64',0) ;
	let iret = DLL.RDR_Open(connstr,handleRef);//打开连接串指定的设备驱动
	
	let header=handleRef.deref();
	
	var operation={'error':0};
	
	console.log('RDR_Open iret:'+iret+'  '+connstr);
	
	if(iret==0)
	{
		iret=DLL.RDR_SetAcessAntenna(header,ant);

		if(iret==0)
		{
			console.log('uid:'+uid);
			var uidbuff=Buffer.from(uid);
	
			let ht = ref.alloc('int64',0);
			iret=ISO15693.ISO15693_Connect(header,tagid,connectmode,uidbuff,ht);//连接标签	
			
			let htHandle=ht.deref();
			if(iret==0)	
			{
				if(enable)
				{
					iret=ISO15693.NXPICODESLI_EableEAS(header,htHandle);

				
				}
				else
				{
					iret=ISO15693.NXPICODESLI_DisableEAS(header,htHandle);
				
				}

				if(iret==0)	
				{
					operation['error']=0;

				}
				else
				{
					operation['error']=iret;

					if(enable)
					console.log('NXPICODESLI_EableEAS iret:'+iret);	
					else
					console.log('NXPICODESLI_DisableEAS iret:'+iret);	

				}
				
				DLL.RDR_TagDisconnect(htHandle);
			}
			else
			{
				operation['error']=iret;
			}
		}
		else
		{
			operation['error']=iret;

			console.log('RDR_SetAcessAntenna iret:'+iret);	
		}
		
		DLL.RDR_Close(header);
	}
	else
	{
		operation['error']=iret;
	}
	return JSON.stringify(operation);
}

exports.ISO15693ChangeEAS=ISO15693ChangeEAS;	