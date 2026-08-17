package Main;

import java.awt.GridLayout;

import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.lang.reflect.InvocationTargetException;
import java.util.ArrayList;
import java.util.Dictionary;
import java.util.Hashtable;
import java.util.List;

import javax.swing.BorderFactory;
import javax.swing.GroupLayout;
import javax.swing.JButton;
import javax.swing.JCheckBox;
import javax.swing.JComboBox;
import javax.swing.JLabel;
import javax.swing.JList;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTextArea;
import javax.swing.JTextField;
import javax.swing.SwingUtilities;
import javax.swing.text.DefaultCaret;

import RFID.rfid_def;
import RFID.rfidlib_AIP_ISO15693;
import RFID.rfidlib_reader;

public class CommandTab extends JPanel implements ActionListener{

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;

	private static String EXCECUTE="Excecute";
	
	JList<String> listUIDSelection = new JList<>();
	JScrollPane spListUIDSelection = new JScrollPane();
	JPanel panelUIDSelection = new JPanel();
	
	JCheckBox ckReadUserBlock=new JCheckBox("Read user Block command");
	
	 JTextField textReadBlockPos=new JTextField();
	 JTextField textReadBlockNum=new JTextField();

	JCheckBox ckWriteUserBlock=new JCheckBox("Write user Block command");
	
	JTextField textWriteBlockPos=new JTextField();
	JTextField textWriteBlockNum=new JTextField();
	JTextField textBlockData=new JTextField(); 
	
	
	JCheckBox ckWriteAFIBlock=new JCheckBox("Write AFI command");
	JTextField textAFI=new JTextField(); 
	
	
	JCheckBox ckChangeEAS=new JCheckBox("EAS Enable/Disable Command");
	JComboBox<String> cbxEASOption=new JComboBox<String>();
	
	JButton btnExce;
	
	JTextArea textLog=new JTextArea();
	
	public static Dictionary<String,DataReport> tagReports=new Hashtable();
	
	public ArrayList<String > uids=new ArrayList<String>();
	
	JScrollPane textScroll = new JScrollPane(textLog); 
	
	
	public CommandTab()
	{
		initComponents();
		
	}
	
	public void initComponents()
	{
		btnExce=new JButton(EXCECUTE);
		btnExce.addActionListener(this);
		
		ckReadUserBlock.setEnabled(true);
		ckWriteUserBlock.setEnabled(true);
		ckWriteAFIBlock.setEnabled(true);
		ckChangeEAS.setEnabled(true);
		
		cbxEASOption.addItem("Disable");
		cbxEASOption.addItem("Enable");
		cbxEASOption.setSelectedIndex(0);
		
		textReadBlockPos.setText("0");
		textReadBlockNum.setText("1");
		
		textWriteBlockPos.setText("1");
		textWriteBlockNum.setText("1");
		textBlockData.setText("ffffffff");
		
		textAFI.setText("aa");
		
		JLabel jlblReadStartBlock=new JLabel("Start Block");
		JLabel jlblReadBlockNum=new JLabel("Num of blocks");
		
		JLabel jlblWriteStartBlock=new JLabel("Start Block");
		JLabel jlblWriteBlockNum=new JLabel("Num of blocks");
		JLabel jlblWriteData=new JLabel("Data");
		JLabel jlblafi=new JLabel("AFI:0x");
		
		JLabel jlbEas=new JLabel("EAS");
		
		 DefaultCaret caret = (DefaultCaret) textLog.getCaret();  
		 caret.setUpdatePolicy(DefaultCaret.ALWAYS_UPDATE);
		
		 
		
		panelUIDSelection.setBorder(BorderFactory.createTitledBorder("UID"));
		
		
		spListUIDSelection.setViewportView(listUIDSelection);
		
		GridLayout gridlay=new GridLayout();
		
		gridlay.setColumns(1);
		gridlay.setRows(1);
		panelUIDSelection.setLayout(gridlay);
		panelUIDSelection.add(spListUIDSelection);
		
		
		
		
		GroupLayout layGroup = new GroupLayout(this);
		layGroup = new GroupLayout(this);
		layGroup.setHorizontalGroup(layGroup.createSequentialGroup().addComponent(panelUIDSelection)
				.addGroup(layGroup.createParallelGroup().addComponent(ckReadUserBlock)
						.addGroup(layGroup.createSequentialGroup().addComponent(jlblReadStartBlock).addComponent(textReadBlockPos))
						.addGroup(layGroup.createSequentialGroup().addComponent(jlblReadBlockNum).addComponent(textReadBlockNum))
						.addComponent(ckWriteUserBlock)
						.addGroup(layGroup.createSequentialGroup().addComponent(jlblWriteStartBlock).addComponent(textWriteBlockPos))
						.addGroup(layGroup.createSequentialGroup().addComponent(jlblWriteBlockNum).addComponent(textWriteBlockNum))
						.addGroup(layGroup.createSequentialGroup().addComponent(jlblWriteData).addComponent(textBlockData))
						.addComponent(ckWriteAFIBlock)
						.addGroup(layGroup.createSequentialGroup().addComponent(jlblafi).addComponent(textAFI))
						.addComponent(ckChangeEAS)
						.addGroup(layGroup.createSequentialGroup().addComponent(jlbEas).addComponent(cbxEASOption))
						)
				.addGroup(layGroup.createParallelGroup().addComponent(btnExce).addComponent(textScroll))
				);
		layGroup.setVerticalGroup(layGroup.createParallelGroup().addComponent(panelUIDSelection)
				.addGroup(layGroup.createSequentialGroup().addGap(20).addComponent(ckReadUserBlock)
						.addGroup(layGroup.createParallelGroup().addComponent(jlblReadStartBlock).addComponent(textReadBlockPos,GroupLayout.PREFERRED_SIZE,20,GroupLayout.PREFERRED_SIZE))
						.addGroup(layGroup.createParallelGroup().addComponent(jlblReadBlockNum).addComponent(textReadBlockNum,GroupLayout.PREFERRED_SIZE,20,GroupLayout.PREFERRED_SIZE))
						.addGap(20)
						.addComponent(ckWriteUserBlock)
						.addGroup(layGroup.createParallelGroup().addComponent(jlblWriteStartBlock).addComponent(textWriteBlockPos,GroupLayout.PREFERRED_SIZE,20,GroupLayout.PREFERRED_SIZE))
						.addGroup(layGroup.createParallelGroup().addComponent(jlblWriteBlockNum).addComponent(textWriteBlockNum,GroupLayout.PREFERRED_SIZE,20,GroupLayout.PREFERRED_SIZE))
						.addGroup(layGroup.createParallelGroup().addComponent(jlblWriteData).addComponent(textBlockData,GroupLayout.PREFERRED_SIZE,20,GroupLayout.PREFERRED_SIZE))
						.addGap(20)
						.addComponent(ckWriteAFIBlock)
						.addGroup(layGroup.createParallelGroup().addComponent(jlblafi).addComponent(textAFI,GroupLayout.PREFERRED_SIZE,20,GroupLayout.PREFERRED_SIZE))
						.addGap(20)
						.addComponent(ckChangeEAS)
						.addGroup(layGroup.createParallelGroup().addComponent(jlbEas).addComponent(cbxEASOption,GroupLayout.PREFERRED_SIZE,20,GroupLayout.PREFERRED_SIZE))
						)
				.addGroup(layGroup.createSequentialGroup().addComponent(btnExce).addComponent(textScroll))
				);

		this.setLayout(layGroup);
	}

	boolean HasCmdReadData=false;
	boolean HasCmdWriteData=false;
	boolean HasCmdWriteAFI=false;
	boolean HasCmdEas=false;
	boolean CmdEnableEas = false;
	long  hTagSet=0;
	
	long tagCnt=0;
	
	@Override
	public void actionPerformed(ActionEvent e) {
		// TODO Auto-generated method stub
		
		if(e.getActionCommand().equals(EXCECUTE))
		{
			
			
			
			HasCmdReadData=ckReadUserBlock.isSelected();
			HasCmdWriteData=ckWriteUserBlock.isSelected();
			HasCmdWriteAFI=ckWriteAFIBlock.isSelected();
			HasCmdEas=ckChangeEAS.isSelected();
			
			if(cbxEASOption.getSelectedIndex()==0)
			{
				CmdEnableEas=false;
			}
			else
			{
				CmdEnableEas=true;
			}
			
			tagCnt=this.listUIDSelection.getSelectedValuesList().size();
			
			int readPos=	Integer.parseInt(textReadBlockPos.getText());
			int readNum=	Integer.parseInt(textReadBlockNum.getText());
			
			int writePos=Integer.parseInt(textWriteBlockPos.getText());
			int writeNum=Integer.parseInt(textWriteBlockNum.getText());
			
			byte[]databyt=gFunction.decodeHex(textBlockData.getText());
			
			byte[]writeByte=new byte[writeNum*4];
			
			System.arraycopy(databyt, 0, writeByte, 0, databyt.length>writeByte.length?writeByte.length:databyt.length );
			
			
			byte afival=gFunction.decodeHex(textAFI.getText())[0];
			
			
		
			
			  hTagSet=rfidlib_reader.CreateMultipleAccessTagSet((byte)0, (byte)0, (byte)0,100);
			
			if(hTagSet==0)
			{
				return;
			}
			

			List<String> selectuids=this.listUIDSelection.getSelectedValuesList();
			
			 boolean inheritTagType = false;
             boolean inheritAnt = false;
             boolean inheritCmd = false;
			
			
             int iret=0;
             
			for(int i=0;i<selectuids.size();i++)
			{
				String uidStr=selectuids.get(i);
				
				if(i==0)
				{
					inheritTagType=false;
					inheritAnt = false;
		            inheritCmd = false;
				}
				else
				{
					  String lastTagUidStr=selectuids.get(i-1);
					
					  inheritCmd = true;
					  
					  if(tagReports.get(uidStr).tag_id==tagReports.get(lastTagUidStr).tag_id)
					  {
						  inheritTagType=true;
					  }
					  else
					  {
						  inheritTagType=false;
					  }
					  
					  
					  if(tagReports.get(uidStr).ant_id==tagReports.get(lastTagUidStr).ant_id)
					  {
						  inheritAnt=true;
					  }
					  else
					  {
						  inheritAnt=false;
					  }
					  
				}
				
				byte[] uid=tagReports.get(uidStr).uid;
				
				iret=rfidlib_AIP_ISO15693.ISO15693_AddNewAccessTag( hTagSet,(byte) 1,uid,inheritTagType , inheritAnt, inheritCmd);
				
				if(iret!=0)
				{
					rfidlib_reader.DNODE_Destroy(hTagSet);
					
					return;
				}
				
				if(!inheritTagType)
				{
					iret = rfidlib_reader.RDR_SetLastATagTagType(hTagSet,tagReports.get(uidStr).tag_id);

                    if (iret != 0)
                    {
                       
                    	rfidlib_reader.DNODE_Destroy(hTagSet);
                    	
                    	
                       return;
                    }
				}
				
				if(!inheritAnt)
				{
					byte[]ant=new byte[1];
					ant[0]=(byte) tagReports.get(uidStr).ant_id;
					
					iret=rfidlib_reader.RDR_SetLastATagAntennas(hTagSet, ant, (byte) 1);
					
					 if (iret != 0)
	                 {  
						 rfidlib_reader.DNODE_Destroy(hTagSet);
	                    return;
	                 }
				}
				
				if(!inheritCmd)
				{

					
					if(HasCmdReadData)
					{
						long hCmd=rfidlib_AIP_ISO15693.ISO15693_CreateTAReadMultipleBlocks(0,false,readPos,readNum);	
						
						if(hCmd!=0)
						{
							rfidlib_reader.RDR_AddLastATagAccessCommand(hTagSet,hCmd);
						}else
						{
							rfidlib_reader.DNODE_Destroy(hTagSet);
							return;
						}
					}
					
					if(HasCmdWriteData)
					{
						long hCmd=	rfidlib_AIP_ISO15693.ISO15693_CreateTAWriteMultipleBlocks(0, writePos, writeNum, writeByte, writeByte.length);
						
						if(hCmd!=0)
						{
							rfidlib_reader.RDR_AddLastATagAccessCommand(hTagSet,hCmd);
						}else
						{
							rfidlib_reader.DNODE_Destroy(hTagSet);
							return;
						}
					}
					
					if(HasCmdWriteAFI)
					{
						long hCmd=	rfidlib_AIP_ISO15693.ISO15693_CreateTAWriteAFI(0, afival);
						
						if(hCmd!=0)
						{
							rfidlib_reader.RDR_AddLastATagAccessCommand(hTagSet,hCmd);
						}else
						{
							rfidlib_reader.DNODE_Destroy(hTagSet);
							return;
						}
						
					}
					
					if(HasCmdEas)
					{
						
						long hCmd=0;
						if(CmdEnableEas)
						{
							hCmd=rfidlib_AIP_ISO15693.NXPICODESLI_CreateTAEableEAS(0);
						}
						else
						{
							hCmd=rfidlib_AIP_ISO15693.NXPICODESLI_CreateTADisableEAS(0);
						}
						
						
						if(hCmd!=0)
						{
							rfidlib_reader.RDR_AddLastATagAccessCommand(hTagSet,hCmd);
						}else
						{
							rfidlib_reader.DNODE_Destroy(hTagSet);
							return;
						}
						
					}
					
					Thread thrd=new Thread(new CmdRunable());
					thrd.start();
					
					
				}
			}
			

			return;
		}
	}
	
	
	void PrintLog(String log)
	{
		 
    	try {
			SwingUtilities.invokeAndWait(new Runnable()
			{

				@Override
				public void run()
			        {
							
						textLog.append(log);
						textLog.append("\r\n");
					
						int length = textLog.getText().length();
						textLog.setCaretPosition(length);
					}
			});
		} catch (InvocationTargetException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	
	
	class CmdRunable implements Runnable 
	{

		@Override
		public void run() {
			// TODO Auto-generated method stub
			String str="";
			
			int iret=rfidlib_reader.RDR_AccessMultipleTags(MainFrm.m_hr, hTagSet);
			
			if(iret!=0)
			{
				//error
				rfidlib_reader.DNODE_Destroy(hTagSet); 
				
				str=String.format("RDR_AccessMultipleTags error:%d", iret);
				PrintLog(str);
				
				return;
			}
			else
			{
				int _iret = rfidlib_reader.RDR_SeekAccessTag(hTagSet,0);
	            if (iret != 0)
	            {
	            	rfidlib_reader.DNODE_Destroy(hTagSet); 
	            	
	            	str=String.format("RDR_SeekAccessTag error:%d", iret);
	            	
	            	PrintLog(str);
	            	
	            	return;
	            }

	          
	            for(int i=0;i<tagCnt;i++)
	            {
	            	StringBuilder uidSB=new StringBuilder();
	            	
	            	final int index=0; 
	            	 
	            	try {
						SwingUtilities.invokeAndWait(new Runnable()
						{
    
							@Override
							public void run()
						        {
									uidSB.append( listUIDSelection.getSelectedValuesList().get(index));
								}
						});
					} catch (InvocationTargetException e) {
						// TODO Auto-generated catch block
						e.printStackTrace();
					} catch (InterruptedException e) {
						// TODO Auto-generated catch block
						e.printStackTrace();
					}
	            	
	            	String uid=uidSB.toString();
	            	byte flag=rfid_def.RFID_SEEK_FIRST;
	            	
	            
	            	
	            	if(HasCmdReadData)
	            	{
	            		long hCmd = 0;	
                        hCmd =rfidlib_reader.RDR_GetTagAccessCommand(hTagSet, flag);
                        
                        if(hCmd!=0)
                        {
                        	Integer numofBlk=new Integer(0);
                        	byte[]blockdata=new byte[255*4];
                        	Integer size=new Integer(255*4);
                        	
                        	iret=rfidlib_AIP_ISO15693.ISO15693_ParseReadMultiBlocksResult(hCmd, numofBlk, blockdata, size);
                        	
                        	if(iret==0)
                        	{
                        		str=String.format("[%s] ¿éÊýÁ¿:%d ¿éÊý¾Ý:%s",uid,numofBlk,gFunction.encodeHexStr(blockdata,size));
                        	}else
                        	{
                        		str=String.format("[%s]¶Á¿éÊ§°Ü:%d", uid,iret);
                        	}
                        	
                        	PrintLog(str);
                        }
                        
                        flag=rfid_def.RFID_SEEK_NEXT;
	            	}
	            	
	            	if(HasCmdWriteData)
	            	{
	            		long hCmd = 0;
                        hCmd =rfidlib_reader.RDR_GetTagAccessCommand(hTagSet, flag);
                        if(hCmd!=0)
                        {
                        	iret=rfidlib_AIP_ISO15693.ISO15693_ParseWriteMultipleBlocksResult(hCmd);
                        	
                        	if(iret==0)
                        	{
                        		str=String.format("[%s] Ð´¿é³É¹¦",uid);
                        	}
                        	else
                        	{
                        		str=String.format("[%s] Ð´¿éÊ§°Ü:%d",uid,iret);
                        	}
                        	
                        	PrintLog(str);
                        	
                        }
                        
                        flag=rfid_def.RFID_SEEK_NEXT;
	            	}
	            	
	            	if(HasCmdWriteAFI)
	            	{
	            		long hCmd = 0;
                        hCmd =rfidlib_reader.RDR_GetTagAccessCommand(hTagSet, flag);
                        
                        if(hCmd!=0)
                        {
                        	iret=rfidlib_AIP_ISO15693.ISO15693_ParseWriteAFIResult(hCmd);
                        	
                        	if(iret==0)
                        	{
                        		str=String.format("[%s] Ð´AFI³É¹¦",uid);
                        	}
                        	else
                        	{
                        		str=String.format("[%s] Ð´AFIÊ§°Ü:%d", uid,iret);
                        	}
                        	
                        	PrintLog(str);
                        }
                        flag=rfid_def.RFID_SEEK_NEXT;
	            	}
	            	
	            	if(HasCmdEas)
	            	{
	            		long hCmd = 0;
                        hCmd =rfidlib_reader.RDR_GetTagAccessCommand(hTagSet, flag);
                        
                        if(hCmd!=0)
                        {
                        	
                        	if(!CmdEnableEas)
                        	{
                        		iret=rfidlib_AIP_ISO15693.NXPICODESLI_ParseDisableEASResult(hCmd);
                        		
                        		if(iret==0)
                            	{
                            		str=String.format("[%s] Disable EAS³É¹¦",uid);
                            	}
                            	else
                            	{
                            		str=String.format("[%s] Disable EASÊ§°Ü:%d", uid,iret);
                            	}
                        		PrintLog(str);
                        	}
                        	else
                        	{
                        		iret=rfidlib_AIP_ISO15693.NXPICODESLI_ParseDisableEASResult(hCmd);
                        	}
                        	
                        }
	            	}
	            	
	            	_iret = rfidlib_reader.RDR_SeekAccessTag(hTagSet,i+1);
		            
	            }
	            
	         	rfidlib_reader.DNODE_Destroy(hTagSet); 
	         	hTagSet=0;
	         	
	         	
	            return;
			}
			
		}
	}
	
	
	
}
