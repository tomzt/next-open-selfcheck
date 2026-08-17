package Main;

import java.awt.Component;
import java.awt.FlowLayout;
import java.awt.GridLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.lang.reflect.InvocationTargetException;
import java.util.List;
import java.util.Vector;

import javax.swing.BorderFactory;
import javax.swing.BoxLayout;
import javax.swing.DefaultListSelectionModel;
import javax.swing.GroupLayout;
import javax.swing.JButton;
import javax.swing.JCheckBox;
import javax.swing.JComboBox;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JList;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTable;
import javax.swing.JTextField;
import javax.swing.ListCellRenderer;
import javax.swing.ListSelectionModel;
import javax.swing.SwingUtilities;
import javax.swing.GroupLayout.Alignment;
import javax.swing.table.DefaultTableModel;


import RFID.rfid_def;
import RFID.rfidlib_AIP_ISO15693;
import RFID.rfidlib_aip_iso14443A;
import RFID.rfidlib_reader;

public class MainFrm extends JFrame  {

	private JButton btnOpen = new JButton("Open");
	private JButton btnClose = new JButton("Close");
	private JComboBox<String> cmbCommType = new JComboBox<String>();
	private JComboBox<String> cmbComName = new JComboBox<String>();
	private JComboBox<String> cmbComBaud = new JComboBox<String>();
	private JComboBox<String> cmbComFrm = new JComboBox<String>();
	private JComboBox<String> cmbUsbType = new JComboBox<String>();
	private JComboBox<String> cmbUsbSn = new JComboBox<String>();
	private JTextField textIP = new JTextField();
	private JTextField textPort = new JTextField();

	

	public static  long m_hr = 0;// ¶Á¿¨Æ÷²Ù×÷¾ä±ú

	

	
	private javax.swing.JTabbedPane jTabbedPane;
	
	public static CommandTab commandTab=null;
	public static InventoryTab inventoryTab=null;
	

	
	
	public static void main(String[] args) {
		// TODO Auto-generated method stub
		new MainFrm();
	}

	private void LoadLibrary() {
		int osType = 0;
		int arType = 0;
		String libPath = System.getProperty("user.dir");
		String osName = System.getProperty("os.name");
		String architecture = System.getProperty("os.arch");
		osName = osName.toUpperCase();
		if (osName.equals("LINUX")) {
			osType = rfid_def.VER_LINUX;
		} else if (osName.indexOf("WIN") != -1) {
			osType = rfid_def.VER_WINDOWS;
		}

		architecture = architecture.toUpperCase();
		if (architecture.equals("AMD64") || architecture.equals("X64") || architecture.equals("UNIVERSAL")) {
			arType = rfid_def.AR_X64;
		} else {
			arType = rfid_def.AR_X86;
		}

		rfidlib_reader.LoadLib(libPath, osType, arType);
		rfidlib_AIP_ISO15693.LoadLib(libPath, osType, arType);
		rfidlib_aip_iso14443A.LoadLib(libPath, osType, arType);

		int m_cnt = rfidlib_reader.RDR_GetLoadedReaderDriverCount();

	}

	
	
	public MainFrm() {
		LoadLibrary();

		InitControls();

	}
	private void ListAllCom()
	{
		int m_cnt = rfidlib_reader.COMPort_Enum();
		for (int i = 0; i < m_cnt; i++)
		{
			char[] buffer = new char[128];
			Integer nSize = new Integer(0);
			rfidlib_reader.COMPort_GetEnumItem(i, buffer, nSize);
			String comName = String.copyValueOf(buffer, 0, nSize);
			cmbComName.addItem(comName);
		}
		if (m_cnt > 0)
		{
			cmbComName.setSelectedIndex(0);
		}
		
		
		m_cnt = rfidlib_reader.HID_Enum("RD5200");
		for (int i = 0; i < m_cnt; i++)
		{
			char[] buffer = new char[128];
			Integer nSize = new Integer(0);
			rfidlib_reader.HID_GetEnumItem(i,(byte)0x01, buffer, nSize);
			String sn = String.copyValueOf(buffer, 0, nSize);
			cmbUsbSn.addItem(sn);
		}
		if (m_cnt > 0)
		{
			cmbUsbSn.setSelectedIndex(0);
		}
		
		
	}
	

	public void InitControls() {

		ListAllCom();
		
		cmbCommType.addItem("COM");
		cmbCommType.addItem("USB");
		cmbCommType.addItem("TCP/IP");
		
		cmbCommType.setSelectedIndex(0);

		cmbComBaud.addItem("9600");
		cmbComBaud.addItem("38400");
		cmbComBaud.addItem("57600");
		cmbComBaud.addItem("115200");
		cmbComBaud.setSelectedIndex(1);

		cmbComFrm.addItem("8E1");
		cmbComFrm.addItem("8O1");
		cmbComFrm.addItem("8N1");
		
		cmbComFrm.setSelectedIndex(0);
		
		cmbUsbType.addItem("None addressed");
		cmbUsbType.addItem("addressed");

		textIP.setText("10.168.1.222");
		textPort.setText("9909");

		btnOpen.setEnabled(true);
		btnClose.setEnabled(false);
		
		
		jTabbedPane = new javax.swing.JTabbedPane();
		inventoryTab = new InventoryTab();
		commandTab = new CommandTab();
		jTabbedPane.addTab("Inventory", null, inventoryTab);
		jTabbedPane.addTab("Command", null, commandTab);
		jTabbedPane.addChangeListener(new javax.swing.event.ChangeListener()
		{
			public void stateChanged(javax.swing.event.ChangeEvent evt)
			{
				
				
				if(	jTabbedPane.getSelectedIndex()==1)
				{
					if(inventoryTab.inventoryFlag)
					{
						inventoryTab.inventoryFlag=false;
						rfidlib_reader.RDR_SetCommuImmeTimeout(MainFrm.m_hr);
					}
				}
			}
		});
	
		
		
		
		
		
		btnOpen.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				// TODO Auto-generated method stub

				String connstr = "";

				switch (cmbCommType.getSelectedIndex()) {

				case 0:
					String comNameString = cmbComName.getItemAt(cmbComName.getSelectedIndex()).toString();// ();
					connstr = rfid_def.CONNSTR_NAME_RDTYPE + "=" + "RD5200" + ";" + rfid_def.CONNSTR_NAME_COMMTYPE + "="
							+ rfid_def.CONNSTR_NAME_COMMTYPE_COM + ";" + rfid_def.CONNSTR_NAME_COMNAME + "="
							+ comNameString + ";" + rfid_def.CONNSTR_NAME_COMBARUD + "=" +cmbComBaud.getSelectedItem().toString() + ";"
							+ rfid_def.CONNSTR_NAME_COMFRAME + "=" + cmbComFrm.getSelectedItem().toString() + ";" + rfid_def.CONNSTR_NAME_BUSADDR + "="
							+ "255";
					break;
				case 1:
					connstr = rfid_def.CONNSTR_NAME_RDTYPE + "=" + "RD5200" + ";" + rfid_def.CONNSTR_NAME_COMMTYPE + "="
							+ rfid_def.CONNSTR_NAME_COMMTYPE_USB + ";" + rfid_def.CONNSTR_NAME_HIDADDRMODE + "=" + cmbUsbType.getSelectedIndex()
							+ ";" + rfid_def.CONNSTR_NAME_HIDSERNUM + "="+cmbUsbSn.getSelectedItem().toString();
					break;
				case 2:
					connstr = rfid_def.CONNSTR_NAME_RDTYPE + "=" + "RD5200" + ";" + rfid_def.CONNSTR_NAME_COMMTYPE + "="
							+ rfid_def.CONNSTR_NAME_COMMTYPE_NET + ";" + rfid_def.CONNSTR_NAME_REMOTEIP + "="
							+ textIP.getText() + ";" + rfid_def.CONNSTR_NAME_REMOTEPORT + "=" + textPort.getText() + ";"
							+ rfid_def.CONNSTR_NAME_LOCALIP + "=" + "";
					break;
				}

				Long hrOut = new Long(0);

				int iret = rfidlib_reader.RDR_Open(connstr, hrOut);

				if (iret == 0) {
					m_hr = hrOut.longValue();
					inventoryTab.btnStart.setEnabled(true);
					inventoryTab.btnStop.setEnabled(false);
					
					inventoryTab.checkDisableEAS.setEnabled(true);
					inventoryTab.checkWriteAfi.setEnabled(true);
					
					btnOpen.setEnabled(false);
					btnClose.setEnabled(true);
					
					inventoryTab.checkEnableData.setEnabled(true);
					
					int AntCnt= rfidlib_reader.RDR_GetAntennaInterfaceCount(m_hr);
					
					String [] str=new String[AntCnt];
					
					for (int i = 0; i < AntCnt; i++)
					{
						str[i]="Antenna#"+(i+1);
					}
					
					inventoryTab.listAntenaSelection.setListData(str);
					
					inventoryTab.listAntenaSelection.setSelectionModel(new DefaultListSelectionModel() {
						@Override
						public void setSelectionInterval(int index0, int index1) {
							if (super.isSelectedIndex(index0)) {
								super.removeSelectionInterval(index0, index1);
							} else {
								super.addSelectionInterval(index0, index1);
							}
						}
					});
					
					MyJcheckBox cell = new MyJcheckBox();
					inventoryTab.listAntenaSelection.setCellRenderer(cell);
					
					inventoryTab.listAntenaSelection.setSelectedIndex(0);
					
					
					commandTab.listUIDSelection.setSelectionModel(new DefaultListSelectionModel() {
						@Override
						public void setSelectionInterval(int index0, int index1) {
							if (super.isSelectedIndex(index0)) {
								super.removeSelectionInterval(index0, index1);
							} else {
								super.addSelectionInterval(index0, index1);
							}
						}
					});
					
					commandTab.listUIDSelection.setCellRenderer(cell);
					
					
					
				} else {
					String sRet = "Open device failed!err=" + iret;
					JOptionPane.showMessageDialog(null, iret);
					return;
				}
			}
		});

		btnClose.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				// TODO Auto-generated method stub

				if (m_hr != 0) {

					rfidlib_reader.RDR_Close(m_hr);

					m_hr = 0;

					inventoryTab.btnStart.setEnabled(false);
					inventoryTab.btnStop.setEnabled(false);
					btnOpen.setEnabled(true);
					btnClose.setEnabled(false);
					
					inventoryTab.checkEnableData.setEnabled(false);
				}
			}
		});

		

		JPanel jpanCom = new JPanel();
		JPanel jpanUSB = new JPanel();
		JPanel jpanTCP = new JPanel();
		

		jpanCom.setBorder(BorderFactory.createTitledBorder("Serial Interface"));
		jpanUSB.setBorder(BorderFactory.createTitledBorder("Usb Interface"));
		jpanTCP.setBorder(BorderFactory.createTitledBorder("Tcp Option"));
		

		GridLayout layCom = new GridLayout(3, 2);
		jpanCom.add(new JLabel("Com"));
		jpanCom.add(cmbComName);
		jpanCom.add(new JLabel("Baud"));
		jpanCom.add(cmbComBaud);
		jpanCom.add(new JLabel("Frame"));
		jpanCom.add(cmbComFrm);
		jpanCom.setLayout(layCom);
		jpanCom.setSize(100, 200);

		GridLayout layUsb = new GridLayout(2, 2);
		jpanUSB.add(new JLabel("USB Open Type"));
		jpanUSB.add(cmbUsbType);
		jpanUSB.add(new JLabel("Serial Num"));
		jpanUSB.add(cmbUsbSn);
		jpanUSB.setLayout(layUsb);

		FlowLayout layTCP = new FlowLayout(FlowLayout.CENTER);
		jpanTCP.add(new JLabel("IP Addr"));
		jpanTCP.add(textIP);
		jpanTCP.add(new JLabel("Port"));
		jpanTCP.add(textPort);
		jpanTCP.setLayout(layTCP);

		

		JLabel lblCommType = new JLabel("Communication Type:");

	
	
	
		GroupLayout layGroup = new GroupLayout(getContentPane());
		layGroup.setHorizontalGroup(layGroup.createParallelGroup(Alignment.LEADING)
				.addGroup(layGroup.createSequentialGroup().addComponent(lblCommType).addGap(5)
						.addComponent(cmbCommType, GroupLayout.PREFERRED_SIZE, 100, GroupLayout.PREFERRED_SIZE)
						.addGap(20).addComponent(btnOpen).addGap(20).addComponent(btnClose))

				.addGroup(layGroup.createSequentialGroup()
						.addComponent(jpanCom, GroupLayout.PREFERRED_SIZE, 150, GroupLayout.PREFERRED_SIZE)
						.addComponent(jpanUSB, GroupLayout.PREFERRED_SIZE, 250, GroupLayout.PREFERRED_SIZE)
						.addComponent(jpanTCP, GroupLayout.PREFERRED_SIZE, 300, GroupLayout.PREFERRED_SIZE))
				// .addComponent(tableInventory, GroupLayout.PREFERRED_SIZE, 450,
				// GroupLayout.PREFERRED_SIZE)
				.addComponent(jTabbedPane)

		);

		layGroup.setVerticalGroup(layGroup.createSequentialGroup()
				.addGroup(layGroup.createParallelGroup(Alignment.BASELINE).addComponent(lblCommType)
						.addComponent(cmbCommType, GroupLayout.PREFERRED_SIZE, 20, GroupLayout.PREFERRED_SIZE)
						.addComponent(btnOpen).addComponent(btnClose))
				.addGroup(layGroup.createParallelGroup(Alignment.BASELINE)
						.addComponent(jpanCom, GroupLayout.PREFERRED_SIZE, 100, GroupLayout.PREFERRED_SIZE)
						.addComponent(jpanUSB, GroupLayout.PREFERRED_SIZE, 100, GroupLayout.PREFERRED_SIZE)
						.addComponent(jpanTCP, GroupLayout.PREFERRED_SIZE, 100, GroupLayout.PREFERRED_SIZE))
				// .addComponent(tableInventory, GroupLayout.DEFAULT_SIZE, 199, Short.MAX_VALUE)

				.addComponent(jTabbedPane));

		this.setLayout(layGroup);
		this.setTitle("RD5200Sample");
		this.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		this.setSize(750, 600);
		this.setResizable(false);
		this.setVisible(true);
		setLocationRelativeTo(null);


		
		
		//AddTagToTable(1,"E200",100,"12345678");
	}

	
//	private void jTabbedPaneStateChanged(javax.swing.event.ChangeEvent evt)
//	{
//		switch (jTabbedPane.getSelectedIndex())
//		{
//		case 0:
//			break;
//		case 1:
//			inventoryTab.stopInventory();
//			break;
//		default:
//			break;
//		}
//	}
	
	


	
	public class MyJcheckBox extends JCheckBox implements ListCellRenderer {
		public MyJcheckBox() {
			super();
		}
	 
		public Component getListCellRendererComponent(JList list, Object value, int index, boolean isSelected,
				boolean cellHasFocus) {
			this.setText(value.toString());
			setBackground(isSelected ? list.getSelectionBackground() : list.getBackground());
			setForeground(isSelected ? list.getSelectionForeground() : list.getForeground());
			this.setSelected(isSelected);
			return this;
		}
	}
	
	
}
