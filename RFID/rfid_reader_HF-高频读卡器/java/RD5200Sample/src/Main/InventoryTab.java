package Main;

import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.lang.reflect.InvocationTargetException;
import java.util.ArrayList;
import java.util.Dictionary;
import java.util.Enumeration;
import java.util.Hashtable;
import java.util.List;
import java.util.Vector;

import javax.swing.BorderFactory;
import javax.swing.GroupLayout;
import javax.swing.JButton;
import javax.swing.JCheckBox;
import javax.swing.JLabel;
import javax.swing.JList;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTable;
import javax.swing.JTextField;
import javax.swing.ListSelectionModel;
import javax.swing.SwingUtilities;
import javax.swing.table.DefaultTableModel;

import RFID.rfid_def;
import RFID.rfidlib_AIP_ISO15693;
import RFID.rfidlib_reader;

public class InventoryTab extends JPanel implements ActionListener,Runnable {

	/**
	 * 
	 */
	private static final long serialVersionUID = 1L;
	private static final String START="Start";
	private static final String STOP="Stop";
	
	public JCheckBox checkEnableData = new JCheckBox("Enable Read Data");
	JList<String> listAntenaSelection = new JList<>();
	JScrollPane spListAntenaSelection = new JScrollPane();
	JPanel panelAntenaSelection = new JPanel();

	public JCheckBox checkDisableEAS=new JCheckBox("EAS Disable");
	public JCheckBox checkWriteAfi=new JCheckBox("Modify AFI");
	public JTextField textAFI=new JTextField();
	
	
	public JTextField textStartByte = new JTextField();
	public JTextField textNumOfByte = new JTextField();

	JScrollPane  spInventory = new JScrollPane();
	private JTable tableInventory = new JTable();
	private DefaultTableModel TableModel = new DefaultTableModel();
	Vector<String> vTitile = new Vector<String>();
	Vector<Object> vRowData = new Vector<Object>();

	public JButton btnStart = new JButton("Start");
	public JButton btnStop = new JButton("Stop");

	private JLabel jlbTagCnt = new JLabel("Tag:0");
	private JLabel jlbTime = new JLabel("Time:0");
	
	public boolean inventoryFlag = false;
	
	
	
	
	public InventoryTab()
	{
		initComponents();
	}
	
	public void initComponents()
	{

		checkEnableData.setSelected(false);
		checkEnableData.setEnabled(false);


		btnStart.setEnabled(false);
		btnStop.setEnabled(false);

		vTitile.add("Antenna");
		vTitile.add("UID");
		vTitile.add("RSSI");
		vTitile.add("Data");
		vTitile.add("Read count");
		vTitile.add("EAS");
		vTitile.add("AFI");
		
		textStartByte.setText("0");
		textNumOfByte.setText("4");

		checkDisableEAS.setEnabled(false);
		checkWriteAfi.setEnabled(false);
		
		spListAntenaSelection.setViewportView(listAntenaSelection);

		btnStart.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				// TODO Auto-generated method stub

				btnStart.setEnabled(false);
				btnStop.setEnabled(true);
				checkEnableData.setEnabled(false);
				
				vRowData.clear();
				TableModel.setDataVector(vRowData, vTitile);
				tableInventory.setModel(TableModel);
				
				checkDisableEAS.setEnabled(false);
				checkWriteAfi.setEnabled(false);
				
				Enumeration<String> enumKeys=MainFrm.commandTab.tagReports.keys();
				
				while(enumKeys.hasMoreElements())
				{
					
					String key=enumKeys.nextElement();
					MainFrm.commandTab.tagReports.remove(key);
				}
				
				MainFrm.commandTab.uids.clear();
				//MainFrm.commandTab.listUIDSelection.setListData(null);
				MainFrm.commandTab.listUIDSelection.updateUI();
				
				Thread thread=new Thread(InventoryTab.this);
				thread.start();
				

			}
		});

		btnStop.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				// TODO Auto-generated method stub
				inventoryFlag=false;
				rfidlib_reader.RDR_SetCommuImmeTimeout(MainFrm.m_hr);
			}
		});
		
		JPanel jpanInventory = new JPanel();
		jpanInventory.setBorder(BorderFactory.createTitledBorder("Inventory"));
		
		tableInventory.setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
		TableModel.setDataVector(vRowData, vTitile);
		tableInventory.setModel(TableModel);
		
		spInventory.setViewportView(tableInventory);
		
		textAFI.setText("aa");
		
		JLabel jlb=new JLabel("AFI:0x");
		
		JLabel jlbStartBYte = new JLabel("Start Byte");
		JLabel jlbNumOfBYtes = new JLabel("Num of Bytes");
		JLabel jlbAnt = new JLabel("Antenna Selected");


		GroupLayout layInventory = new GroupLayout(this);

		layInventory.setHorizontalGroup(layInventory.createSequentialGroup().addComponent(spInventory)
				.addGroup(layInventory.createParallelGroup().addComponent(checkEnableData)
						.addGroup(layInventory.createSequentialGroup().addComponent(jlbStartBYte)
								.addComponent(textStartByte))
						.addGroup(layInventory.createSequentialGroup().addComponent(jlbNumOfBYtes)
								.addComponent(textNumOfByte))
						
						.addComponent(checkDisableEAS)
						.addComponent(checkWriteAfi)
						.addGroup(layInventory.createSequentialGroup().addComponent(jlb).addComponent(textAFI))
						.addComponent(jlbAnt).addComponent(spListAntenaSelection).addComponent(btnStart)
						.addComponent(btnStop).addComponent(jlbTagCnt).addComponent(jlbTime)));

		layInventory.setVerticalGroup(layInventory.createParallelGroup().addComponent(spInventory)
				.addGroup(layInventory.createSequentialGroup().addComponent(checkEnableData)
						.addGroup(layInventory.createParallelGroup().addComponent(jlbStartBYte)
								.addComponent(textStartByte))
						.addGroup(layInventory.createParallelGroup().addComponent(jlbNumOfBYtes)
								.addComponent(textNumOfByte))
						.addComponent(checkDisableEAS)
						.addComponent(checkWriteAfi)
						.addGroup(layInventory.createParallelGroup().addComponent(jlb).addComponent(textAFI))
						.addComponent(jlbAnt).addComponent(spListAntenaSelection).addComponent(btnStart)
						.addComponent(btnStop).addComponent(jlbTagCnt).addComponent(jlbTime)));

		this.setLayout(layInventory);
		
		
	}

	long spentTime=0;
	@Override
	public void run() {
		// TODO Auto-generated method stub

		inventoryFlag = true;

		byte[] AntennaIDs = new byte[16];
		byte Antennacount = 0;

		List<String> strings = listAntenaSelection.getSelectedValuesList();

		for (int i = 0; i < strings.size(); i++) {

			byte antena = (byte) (Byte.parseByte(strings.get(i).split("#")[1]));

			AntennaIDs[Antennacount] = antena;

			Antennacount++;
		}

		boolean enableReadData = checkEnableData.isSelected();

		long startByte = Integer.parseInt(textStartByte.getText());
		long numOfBytes = Integer.parseInt(textNumOfByte.getText());
		
		boolean ckEAS=checkDisableEAS.isSelected();
		boolean ckAFI=checkWriteAfi.isSelected(); 
		
		long afival=Integer.parseInt(textAFI.getText(), 16);
		

		while (inventoryFlag) {
			long InvenParamSpecList = rfidlib_reader.RDR_CreateInvenParamSpecList();

			if (InvenParamSpecList == 0) {
				break;
			}

			long hIso15693InvenParam = rfidlib_AIP_ISO15693.ISO15693_CreateInvenParam(InvenParamSpecList, (byte) 0,
					(byte) 0, (byte) 0, (byte) 0);

			if (hIso15693InvenParam == 0) {
				break;
			}

			if (enableReadData) {
				rfidlib_AIP_ISO15693.ISO15693_SetInventoryReadParam(hIso15693InvenParam, (byte) 0x00, (byte) 0x00);
				rfidlib_AIP_ISO15693.ISO15693_AddInventoryReadBlockArea(hIso15693InvenParam, startByte, numOfBytes);
			}
			
			long hDisableEas=0;
			long hWriteAFI=0;
			if(ckEAS)
			{
				hDisableEas=rfidlib_AIP_ISO15693.NXPICODESLI_CreateTADisableEAS(0);
				if(hDisableEas!=0)
				rfidlib_reader.RDR_AddTagAccessToInvenParam(hIso15693InvenParam, hDisableEas);
			}
			
			if(ckAFI)
			{
				hWriteAFI=rfidlib_AIP_ISO15693.ISO15693_CreateTAWriteAFI(0, (byte)afival);
				if(hWriteAFI!=0)
					rfidlib_reader.RDR_AddTagAccessToInvenParam(hIso15693InvenParam, hWriteAFI);
				
			}
			

			long beginTick = System.currentTimeMillis();
			int iret = rfidlib_reader.RDR_TagInventory(MainFrm.m_hr, (byte) 1, Antennacount, AntennaIDs, InvenParamSpecList);
			spentTime = System.currentTimeMillis() - beginTick;

			if (iret == 0) 
			{

				long hReport=rfidlib_reader.RDR_GetTagDataReport(MainFrm.m_hr, rfid_def.RFID_SEEK_FIRST);
				
				while(hReport!=0)
				{
					Integer aip_id=new Integer(0);
					Integer tag_id=new  Integer(0);
					Integer ant_id=new Integer(0);
					Byte dsfid=new Byte((byte)0);
					Integer rssi=new Integer(0);
					Integer readCnt=new Integer(0);
					byte []uid=new byte[8];
					
					byte []data=new byte[64];
					Integer nSize=new Integer(64);
					String sData="";
					
					Integer EAScmdRes=new Integer(0);
					Integer AFIcmdRes=new Integer(0);
					
					 iret=rfidlib_AIP_ISO15693.ISO15693_ParseTagDataReportEx(hReport,aip_id,tag_id,ant_id,dsfid,rssi, readCnt,uid );
					
					 if(iret==0)
					 {
						 rfidlib_reader.RDR_ParseTagDataReportBlockData(hReport, data, nSize);
						 
						 if(nSize>0)
						 {
							 sData= gFunction.encodeHexStr(data,nSize);
						 } 
						 if(ckEAS)
						 {
							 rfidlib_reader.RDR_ParseTagDataReportWriteResult(hReport, hDisableEas, EAScmdRes);
						 }
						 if(ckAFI)
						 {
							 rfidlib_reader.RDR_ParseTagDataReportWriteResult(hReport, hWriteAFI, AFIcmdRes);
						 }
					 }
					
					String sUid = gFunction.encodeHexStr(uid);
					
					DataReport report=new DataReport(tag_id,ant_id,sUid,uid);
					
					if(MainFrm.commandTab.tagReports.get(sUid)==null)
					{
						MainFrm.commandTab.tagReports.put(sUid, report);
						
						MainFrm.commandTab.uids.add(sUid);
					}
					else
					{
						MainFrm.commandTab.tagReports.remove(sUid);
						MainFrm.commandTab.tagReports.put(sUid, report);
					}
					
					
					AddTagToTable(ant_id,sUid,rssi,sData,EAScmdRes,AFIcmdRes);
					 
					hReport=rfidlib_reader.RDR_GetTagDataReport(MainFrm.m_hr, rfid_def.RFID_SEEK_NEXT);
				}
				
			}
			
			String[]strs=new String[MainFrm.commandTab.uids.size()];
			
			try {
				SwingUtilities.invokeAndWait(new Runnable()
				{
	 
					@Override
					public void run()
				        {
							MainFrm.commandTab.listUIDSelection.setListData( MainFrm.commandTab.uids.toArray(strs));
						}
				});
			} catch (InvocationTargetException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
			
			
			rfidlib_reader.DNODE_Destroy(InvenParamSpecList);

		}
	
		rfidlib_reader.RDR_ResetCommuImmeTimeout(MainFrm.m_hr);
		inventoryFlag=false;
		SwingUtilities.invokeLater(new Runnable()
		{
 
			@Override
			public void run()
                {
					btnStart.setEnabled(true);
					btnStop.setEnabled(false);
					checkEnableData.setEnabled(true);
					
					checkDisableEAS.setEnabled(true);
					checkWriteAfi.setEnabled(true);
				}
			
		});

		
	}
	
	
	
	
	
	private void AddTagToTable(int ant_id, String sUid,long rssi,String userdata,int eas,int afi)
	{
		int readCnt=1;
		boolean finded=false;
		int indeX=0;
		for (int i = 0; i < vRowData.size(); i++)
		{
			if (sUid.equals( ((Vector<String>)(vRowData.get(i))).get(1).toString()))
			{
				finded=true;
				indeX=i;
				
				readCnt=Integer.parseInt(((Vector<String>)(vRowData.get(i))).get(4).toString());
				
				readCnt+=1;
				break;
			}
		}
		
		if(!finded)
		{
		Vector<String> vectorRow = new Vector<String>();
		vectorRow.addElement(String.format("%d", ant_id));
		vectorRow.addElement(new String(sUid));
		vectorRow.addElement(String.format("%d", rssi));
		vectorRow.addElement(new String(userdata));
		vectorRow.addElement(String.format("%d", readCnt));
		vectorRow.addElement(String.format("%d", eas));
		vectorRow.addElement(String.format("%d", afi));
		
		vRowData.addElement(vectorRow);
		
	
		
		
		}
		else
		{
			((Vector<String>)(vRowData.get(indeX))).set(4, String.format("%d", readCnt));
			((Vector<String>)(vRowData.get(indeX))).set(2, String.format("%d", rssi));
			
			
			((Vector<String>)(vRowData.get(indeX))).set(5, String.format("%d", eas));
			((Vector<String>)(vRowData.get(indeX))).set(6, String.format("%d", afi));
		}
		
	
	
		try {
			SwingUtilities.invokeAndWait(new Runnable()
			{
 
				@Override
				public void run()
			        {
						tableInventory.updateUI();
						jlbTagCnt.setText(String.format("Tag:%d",vRowData.size()));
						jlbTime.setText(String.format("Time:%d", spentTime));
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
	
	
	
	
	
	private void startThread() {
		Thread m_thread = new Thread(this);
		vRowData.clear();
		TableModel.setDataVector(vRowData, vTitile);
		m_thread.start();
		btnStart.setEnabled(false);
		btnStop.setEnabled(true);
	}
	
	
	
	@Override
	public void actionPerformed(ActionEvent e) {
		// TODO Auto-generated method stub
		
		switch(e.getActionCommand())
		{
		case START:
			
			break;
		case STOP:
		
			break;
		}
	}
	
	
}
