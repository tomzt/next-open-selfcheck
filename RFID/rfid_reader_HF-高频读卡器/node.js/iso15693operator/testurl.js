let http = require("http");
let url = require("url");

//load module
let querystring = require('querystring');
let refArray =require('ref-array-napi');
let ReaderDLL=require('./dll');


function parseConnStrFromUrl(query)
{
	var commtype='USB';
	var rdtype='RL8000';
	//var commtype='COM';
	//var rdtype='RD5200';
	var comname='COM6';
	var baudrate='38400';
	var frame='8E1';
	var busaddr='255';
	var remoteip='';
	var remoteport='';
	var localip='';
	var addrmode=0;
	var sernum='';
	var connt='';
	
	if(querystring.parse(query)["commtype"]!=undefined)
	{
			commtype=(querystring.parse(query)["commtype"]);
	}
	if(querystring.parse(query)["rdtype"]!=undefined)
	{
			 rdtype=(querystring.parse(query)["rdtype"]);
	}
	if(querystring.parse(query)["comname"]!=undefined)
	{
			 comname=(querystring.parse(query)["comname"]);
	}
	if(querystring.parse(query)["baudrate"]!=undefined)
	{
			 baudrate=(querystring.parse(query)["baudrate"]);
	}
	if(querystring.parse(query)["frame"]!=undefined)
	{
			 frame=(querystring.parse(query)["frame"]);
	}
	if(querystring.parse(query)["busaddr"]!=undefined)
	{
			 busaddr=(querystring.parse(query)["busaddr"]);
	}
		
	if(querystring.parse(query)["remoteip"]!=undefined)
	{
			 remoteip=(querystring.parse(query)["remoteip"]);
	}
	if(querystring.parse(query)["remoteport"]!=undefined)
	{
			 remoteport=(querystring.parse(query)["remoteport"]);
	}
	if(querystring.parse(query)["localip"]!=undefined)
	{
			 localip=(querystring.parse(query)["localip"]);
	}
		
	if(querystring.parse(query)["addrmode"]!=undefined)
	{
			 addrmode=(querystring.parse(query)["addrmode"]);
	}
	if(querystring.parse(query)["sernum"]!=undefined)
	{
			 sernum=(querystring.parse(query)["sernum"]);
	}
		
	if(commtype=='COM')
	{
		connt='RDType='+rdtype+';CommType='+commtype+';COMName='+comname+';BaudRate='+baudrate+';Frame='+frame+';BusAddr='+busaddr;//馆员工作站
	}else if(commtype=='NET')
	{
		connt='RDType='+rdtype+';CommType='+commtype+';RemoteIP='+remoteip+';RemotePort='+remoteport+';LocalIP='+localip;
	}
	else
	{
		connt='RDType='+rdtype+';CommType='+commtype+';AddrMode='+addrmode+';SerNum=';
	}
	console.log(connt);	
		
	return connt;
}


 
 
function start() {
	
	let res=ReaderDLL.InitLibrary();
	console.log(res);
	
  function onRequest(request, response) 
  {
	//console.log(request);
	
    var pathname = url.parse(request.url).pathname;
	var query=url.parse(request.url).query;
    console.log("Request for " + pathname + " received.");
    response.writeHead(200, {"Content-Type": "text/plain"});
   
	//console.log(pathname.indexOf('enumcom'));
	
	if(pathname.indexOf('enumcom')>=0)
	{
		console.log(ReaderDLL.getCommNameArr());
		response.write(ReaderDLL.getCommNameArr());
	}
	else if(pathname.indexOf('inventory')>=0)
	{
		console.log("inventory");
		
		var ant=1;
		var iso15693=1;
		var iso14443a=0;
		var commtype='COM';
		var rdtype='RD5200';
		var comname='';
		var baudrate='38400';
		var frame='8E1';
		var busaddr='255';
		var remoteip='';
		var remoteport='';
		var localip='';
		var addrmode='';
		var sernum='';
		var connt='';
		console.log(querystring.parse(query));
		
		if(querystring.parse(query)["ant"]!=undefined)
		{
			
			ant=(querystring.parse(query)["ant"]);
		}
		if(querystring.parse(query)["iso15693"]!=undefined)
		{
			iso15693=(querystring.parse(query)["iso15693"]);
		}
		if(querystring.parse(query)["iso14443a"]!=undefined)
		{
			iso14443a=(querystring.parse(query)["iso14443a"]);
		}
		
		var connt=parseConnStrFromUrl(query);
		
		var res=ReaderDLL.Inventory(connt,ant);
		
		response.write(JSON.stringify(res));
		
	}
	else if(pathname.indexOf('tagoperation')>=0)
	{
		var tagid;
		var uid=[];
		var op="iso15693readmultiblocks";
		var connectmode=1;
		
		var connt=parseConnStrFromUrl(query);
		
		var ant=1;
		
		if(querystring.parse(query)["op"]!=undefined)
		{
			op=(querystring.parse(query)["op"]);
			console.log("op="+op);
		}
		
		if(querystring.parse(query)["ant"]!=undefined)
		{
		 	ant= parseInt(querystring.parse(query)["ant"],10);
			console.log("ant="+ant);
		}
		
		
		if(querystring.parse(query)["tagid"]!=undefined)
		{
		 	tagid= parseInt(querystring.parse(query)["tagid"],10);
			console.log("tagid="+tagid);
		}
		
		if(querystring.parse(query)["connectmode"]!=undefined)
		{
			connectmode= parseInt(querystring.parse(query)["connectmode"],10);
			console.log("connectMode="+connectMode);
		}
		
		if(querystring.parse(query)["uid"]!=undefined)
		{
			uid=ReaderDLL.GetHexByte(querystring.parse(query)["uid"]);
			console.log(uid);
		}
		
		if(op.indexOf('iso15693readmultiblocks')>=0)
		{
			console.log('iso15693readmultiblocks ing');
			let startblock=0;
			let blocknum=1;
			if(querystring.parse(query)["startblock"]!=undefined)
			{
				startblock=parseInt(querystring.parse(query)["startblock"],10);
			}				
			
			if(querystring.parse(query)["blocknum"]!=undefined)
			{
				blocknum=parseInt(querystring.parse(query)["blocknum"],10);
			}		
				
			console.log('iso15693readmultiblocks');
				
			var res=ReaderDLL.ISO15693ReadMultiBlock(connt,uid,ant,tagid,connectmode,startblock,blocknum);
			
			response.write(res);
		}
		else if(op.indexOf('iso15693writemultiblocks')>=0)
		{
			console.log('iso15693writemultiblocks ing');
			
			let startblock=0;
			let blocknum=1;
			let writedata=[];
			if(querystring.parse(query)["startblock"]!=undefined)
			{
				startblock=parseInt(querystring.parse(query)["startblock"],10);
			}				
			
			if(querystring.parse(query)["blocknum"]!=undefined)
			{
				blocknum=parseInt(querystring.parse(query)["blocknum"],10);
			}		
			
			if(querystring.parse(query)['writedata']!=undefined)
			{
				writedata=ReaderDLL.GetHexByte(querystring.parse(query)["writedata"]);
			}
			
			var res=ReaderDLL.ISO15693WriteMultiBlock(connt,uid,ant,tagid,connectmode,startblock,blocknum,writedata);
			response.write(res);
		}
		else if(op.indexOf('iso15693writeafi')>=0)
		{
			console.log('iso15693writeafi ing');
			
			let writeafi=0;
			
			if(querystring.parse(query)['writeafi']!=undefined)
			{
				writeafi=parseInt(querystring.parse(query)["writeafi"],16);
			}
			
			var res=ReaderDLL.ISO15693WriteAFI(connt,uid,ant,tagid,connectmode,writeafi);
			
			response.write(res);
		}
		else if(op.indexOf('iso15693enableeas')>=0)
		{
			console.log('iso15693enableeas ing');

			
			var res=ReaderDLL.ISO15693ChangeEAS(connt,uid,ant,tagid,connectmode,true);
			response.write(res);

		}	
		else if(op.indexOf('iso15693disableeas')>=0)
		{
			console.log('iso15693disableeas ing');
			var res=ReaderDLL.ISO15693ChangeEAS(connt,uid,ant,tagid,connectmode,false);
			response.write(res);
		}
	}
	
    response.end();
  }
 
  http.createServer(onRequest).listen(8888);  
  console.log("Server has started.");
}
 

start();

