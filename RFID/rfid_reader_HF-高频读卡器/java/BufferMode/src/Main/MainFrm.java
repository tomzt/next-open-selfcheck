package Main;

import java.awt.GridLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.ItemEvent;
import java.awt.event.ItemListener;
import java.util.ArrayList;
import java.util.Date;
import java.util.Vector;

import javax.swing.BorderFactory;
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTable;
import javax.swing.JTextField;

import RFID.rfid_def;
import RFID.rfidlib_reader;

public class MainFrm extends JFrame {

	private static final long serialVersionUID = 1L;
	private long m_hr = 0;// �������������
	private static final String OPEN = "Open";
	private static final String CLOSE = "Close";
	private static final String SetTime = "Set Time";

	private JLabel jlbReaderType;
	private JComboBox<String> jcmbReaderType;// Reader����
	private JLabel JlbcmbCommType;
	private JComboBox<String> cmbCommType;

	private JButton jbOpen, jbClose, jbSetTime;// �򿪰�ť���رհ�ť���趨ʱ�䰴ť

	private JLabel jlbSerialName;
	private JComboBox<String> jcmbComName;
	private JLabel jlbBaud;
	private JComboBox<String> jcmbBaud;
	private JLabel jlbFrame;
	private JComboBox<String> jcmbFrame;

	private JLabel jlbIP = new JLabel("IP:");
	private JTextField textIP = new JTextField();

	private JLabel jlbPort = new JLabel("Port:");
	private JTextField textPort = new JTextField();

	private JLabel jlbBlueToothName = new JLabel("Name:");
	private JComboBox<String> jcmbBlueTooth = new JComboBox<String>();

	private JLabel jlbBlueToothAddr = new JLabel("Addr:");
	private JTextField jtfBlueToothAddr = new JTextField();

	private JTable tbTagInfo;
	private JScrollPane jsp;

	private Vector<Object> rawsData;

	private JButton jbtnGetRecord = new JButton("Get Record");
	private JButton jbtnStopRecord = new JButton("Stop");
	private JButton jbtnClearRecord = new JButton("Clear");

	private ArrayList<String> arrayBluetoothaddr = new ArrayList<String>();

	GetRecordThread getRecordTask = new GetRecordThread();

	void init_ui() {

		Vector<Object> columnNames = new Vector<>();
		columnNames.addElement("Record");

		rawsData = new Vector<Object>();

		jlbReaderType = new JLabel("Reader Type:", JLabel.LEFT);
		jcmbReaderType = new JComboBox<String>();
		JlbcmbCommType = new JLabel("Communicate Type:", JLabel.LEFT);
		cmbCommType = new JComboBox<String>();

		jbOpen = new JButton(OPEN);
		jbClose = new JButton(CLOSE);
		jbSetTime = new JButton(SetTime);

		jlbSerialName = new JLabel("Com:", JLabel.LEFT);
		jlbBaud = new JLabel("Baud:", JLabel.LEFT);
		jlbFrame = new JLabel("Frame:", JLabel.LEFT);

		

		tbTagInfo = new JTable(rawsData, columnNames);

		jsp = new JScrollPane(tbTagInfo);

		jcmbComName = new JComboBox<String>();
		jcmbBaud = new JComboBox<String>(new String[] { "9600", "38400",
				"57600", "115200" });
		jcmbFrame = new JComboBox<String>(new String[] { "8E1", "8N1", "8O1" });

		cmbCommType.addItem("USB");
		cmbCommType.addItem("COM");
		cmbCommType.addItem("NET");
		// cmbCommType.addItem("BlueTooth");
		cmbCommType.setSelectedIndex(0);

		jlbReaderType.setBounds(10, 10, 80, 20);
		jcmbReaderType.setBounds(110, 10, 100, 20);
		JlbcmbCommType.setBounds(210, 10, 120, 20);
		cmbCommType.setBounds(330, 10, 100, 20);
		jbOpen.setBounds(450, 10, 100, 20);
		jbClose.setBounds(560, 10, 100, 20);
		jbSetTime.setBounds(670, 10, 100, 20);

		/* Serial Panel */
		jlbSerialName.setBounds(10, 10, 50, 20);
		jlbBaud.setBounds(10, 40, 50, 20);
		jlbFrame.setBounds(10, 70, 50, 20);
		jcmbComName.setBounds(60, 10, 100, 20);
		jcmbBaud.setBounds(60, 40, 100, 20);
		jcmbFrame.setBounds(60, 70, 100, 20);

		jsp.setBounds(10, 170, 600, 380);

		jbtnGetRecord.setBounds(650, 170, 120, 50);
		jbtnStopRecord.setBounds(650, 250, 120, 50);
		jbtnClearRecord.setBounds(650, 320, 120, 50);

		JPanel jpnlSerial = new JPanel();
		jpnlSerial.setLayout(null);// ���Layout�������ſ����趨λ��
		jpnlSerial.setBorder(BorderFactory.createTitledBorder("Serial Port"));
		jpnlSerial.setBounds(10, 50, 200, 120);

		jpnlSerial.add(jlbSerialName);
		jpnlSerial.add(jlbBaud);
		jpnlSerial.add(jlbFrame);
		jpnlSerial.add(jcmbComName);
		jpnlSerial.add(jcmbBaud);
		jpnlSerial.add(jcmbFrame);

		JPanel jpnlNet = new JPanel();
		jpnlNet.setLayout(new GridLayout(2, 2));
		jpnlNet.setBorder(BorderFactory.createTitledBorder("Net:"));
		jpnlNet.add(jlbIP);
		jpnlNet.add(textIP);
		jpnlNet.add(jlbPort);
		jpnlNet.add(textPort);
		jpnlNet.setBounds(210, 50, 200, 80);

		JPanel jpnlBluetooth = new JPanel();
		jpnlBluetooth.setLayout(new GridLayout(2, 2));
		jpnlBluetooth.setBorder(BorderFactory.createTitledBorder("BlueTooth:"));
		jpnlBluetooth.setBounds(410, 50, 200, 80);

		jpnlBluetooth.add(jlbBlueToothName);
		jpnlBluetooth.add(jcmbBlueTooth);
		jpnlBluetooth.add(jlbBlueToothAddr);
		jpnlBluetooth.add(jtfBlueToothAddr);

		this.setLayout(null);// ���Layout
		this.add(jlbReaderType);
		this.add(jcmbReaderType);
		this.add(JlbcmbCommType);
		this.add(cmbCommType);
		this.add(jbOpen);
		this.add(jbClose);
		//this.add(jbSetTime);

		this.add(jpnlSerial);
		this.add(jpnlNet);
		// this.add(jpnlBluetooth);

		this.add(jsp);
		this.add(jbtnClearRecord);
		this.add(jbtnGetRecord);
		this.add(jbtnStopRecord);

		this.setSize(800, 600);
		this.setLocationRelativeTo(null);
		this.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		this.setVisible(true);
		this.setResizable(false);

		jbtnGetRecord.setEnabled(false);
		jbtnClearRecord.setEnabled(false);
		jbtnStopRecord.setEnabled(false);

		jbSetTime.setEnabled(false);
		
		

		
	}

	void init_event() {

		jbOpen.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				// TODO Auto-generated method stub

				int iret = 0;
				int commTypeIdx = cmbCommType.getSelectedIndex();
				if (commTypeIdx < 0) {
					JOptionPane.showMessageDialog(null,
							"Please select the type of communication!");
					return;
				}

				String readerDriverName = jcmbReaderType.getSelectedItem()
						.toString();
				String connstr = "";
				if (commTypeIdx == 0) {
					connstr = rfid_def.CONNSTR_NAME_RDTYPE + "="
							+ readerDriverName + ";"
							+ rfid_def.CONNSTR_NAME_COMMTYPE + "="
							+ rfid_def.CONNSTR_NAME_COMMTYPE_USB + ";"
							+ rfid_def.CONNSTR_NAME_HIDADDRMODE + "=" + "0"
							+ ";" + rfid_def.CONNSTR_NAME_HIDSERNUM + "=";
				} else if (commTypeIdx == 1) {
					connstr = rfid_def.CONNSTR_NAME_RDTYPE + "="
							+ readerDriverName + ";"
							+ rfid_def.CONNSTR_NAME_COMMTYPE + "="
							+ rfid_def.CONNSTR_NAME_COMMTYPE_COM + ";"
							+ rfid_def.CONNSTR_NAME_COMNAME + "="
							+ jcmbComName.getSelectedItem() + ";"
							+ rfid_def.CONNSTR_NAME_COMBARUD + "="
							+ jcmbBaud.getSelectedItem() + ";"
							+ rfid_def.CONNSTR_NAME_COMFRAME + "="
							+ jcmbFrame.getSelectedItem() + ";"
							+ rfid_def.CONNSTR_NAME_BUSADDR + "=" + "255";

				} else if (commTypeIdx == 2) {
					String ipAddr;
					int port;
					ipAddr = textIP.getText();
					port = Integer.parseInt(textPort.getText());
					connstr = rfid_def.CONNSTR_NAME_RDTYPE + "="
							+ readerDriverName + ";"
							+ rfid_def.CONNSTR_NAME_COMMTYPE + "="
							+ rfid_def.CONNSTR_NAME_COMMTYPE_NET + ";"
							+ rfid_def.CONNSTR_NAME_REMOTEIP + "=" + ipAddr
							+ ";" + rfid_def.CONNSTR_NAME_REMOTEPORT + "="
							+ port + ";" + rfid_def.CONNSTR_NAME_LOCALIP + "="
							+ "";
				} else if (commTypeIdx == 3) {
					if (jtfBlueToothAddr.getText() == "") {
						JOptionPane
								.showMessageDialog(null,
										"The address of the bluetooth can not be null!");
						return;
					}
					connstr = rfid_def.CONNSTR_NAME_RDTYPE + "="
							+ readerDriverName + ";"
							+ rfid_def.CONNSTR_NAME_COMMTYPE + "="
							+ rfid_def.CONNSTR_NAME_COMMTYPE_BLUETOOTH + ";"
							+ rfid_def.CONNSTR_NAME_BLUETOOTH_SN + "="
							+ jtfBlueToothAddr.getText();
				}

				Long hrOut = new Long(0);

				iret = rfidlib_reader.RDR_Open(connstr, hrOut);
				if (iret != 0) {

					JOptionPane.showMessageDialog(null, "Open reader failed!");

					return;
				}

				m_hr = hrOut;

				jbClose.setEnabled(true);
				jbOpen.setEnabled(false);
				jbSetTime.setEnabled(true);
				jbtnGetRecord.setEnabled(true);
				jbtnStopRecord.setEnabled(false);
				jbtnClearRecord.setEnabled(true);
			}
		});

		jbClose.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				// TODO Auto-generated method stub

				if (!getRecordTask.isClosed()) {

					JOptionPane.showMessageDialog(null, "Please stop record!");

					return;
				}

				int ret = rfidlib_reader.RDR_Close(m_hr);

				if (ret != 0) {

					JOptionPane.showMessageDialog(null, "Fail!");
					return;
				}

				jbOpen.setEnabled(true);
				jbSetTime.setEnabled(false);
				jbtnClearRecord.setEnabled(false);
				jbtnGetRecord.setEnabled(false);
				jbtnStopRecord.setEnabled(false);
				jbClose.setEnabled(false);
				
				
				
				m_hr=0;
			}
		});

		jbSetTime.addActionListener(new ActionListener() {

			@SuppressWarnings("deprecation")
			@Override
			public void actionPerformed(ActionEvent e) {
				// TODO Auto-generated method stub

				Date dt = new Date();

				int ret = rfidlib_reader.RDR_SetSystemTime(m_hr, dt.getYear(),
						dt.getMonth(), dt.getDay(), (byte) dt.getHours(),
						(byte) dt.getMinutes(), (byte) dt.getSeconds());

				if (ret != 0) {
					JOptionPane.showMessageDialog(null, "Failed");
				}

			}
		});

		jbtnGetRecord.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				// TODO Auto-generated method stub

				getRecordTask.start();

				jbtnGetRecord.setEnabled(false);
				jbtnClearRecord.setEnabled(false);
				jbtnStopRecord.setEnabled(true);

			}
		});

		jbtnStopRecord.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				// TODO Auto-generated method stub

				getRecordTask.Stop();

				jbtnGetRecord.setEnabled(true);
				jbtnClearRecord.setEnabled(true);
				jbtnStopRecord.setEnabled(false);

			}
		});

		jbtnClearRecord.addActionListener(new ActionListener() {

			@Override
			public void actionPerformed(ActionEvent e) {
				// TODO Auto-generated method stub

				int iret = rfidlib_reader.RDR_BuffMode_ClearRecords(m_hr);
				if (iret == 0) {
					JOptionPane.showMessageDialog(null,
							"Clear record successfully!");
				} else 
				{
					JOptionPane.showMessageDialog(null,
							"Failure to clear record!");
				}

			}
		});

		jcmbBlueTooth.addItemListener(new ItemListener() {

			@Override
			public void itemStateChanged(ItemEvent e) {
				// TODO Auto-generated method stub

				int index = jcmbBlueTooth.getSelectedIndex();

				String content = jcmbBlueTooth.getSelectedItem().toString();

				if (arrayBluetoothaddr.size() > index) {
					jtfBlueToothAddr.setText(content);
				}
			}
		});

	}

	class GetRecordThread implements Runnable {

		private Thread thread;

		public Boolean start() {

			if (thread == null)
			{
				thread = new Thread(this);
				thread.start();
			}
			else
				return false;

			return true;
		}

		public Boolean isClosed() {
			if (thread == null)
				return true;

			return !thread.isAlive();
		}

		public Boolean Stop() {

			if (thread == null)
				return false;

			thread.interrupt();
			return true;
		}

		@Override
		public void run() {
			while (!thread.isInterrupted()) {

				long iret = rfidlib_reader.RDR_BuffMode_FetchRecords(m_hr, 0);

				if (iret != 0) {

					continue;
				}
				long hTagReport = rfidlib_reader.RDR_GetTagDataReport(m_hr,
						rfid_def.RFID_SEEK_FIRST);

				ArrayList<String> tags = new ArrayList<String>();

				while (hTagReport != 0) {
					byte[] rawBuffer = new byte[32];

					Integer nSize = new Integer(rawBuffer.length);

					if (rfidlib_reader.RDR_ParseTagDataReportRaw(hTagReport,
							rawBuffer, nSize) == 0) 
					{
						if (nSize > 0) {
							String s = gFunction.encodeHexStr(rawBuffer, nSize);

							//System.out.println(s);

							tags.add(s);
						}
					}
					hTagReport = rfidlib_reader.RDR_GetTagDataReport(m_hr,
							rfid_def.RFID_SEEK_NEXT);
				}

				ShowTag(tags);

			}

		
			javax.swing.SwingUtilities.invokeLater(new Runnable() {
				public void run() {
					jbtnGetRecord.setEnabled(true);
					jbtnStopRecord.setEnabled(false);
					jbtnClearRecord.setEnabled(true);
				}
			});
			thread = null;
			
		}
	}

	void ShowTag(ArrayList<String> arr) {
		rawsData.clear();

		for (String s : arr) {
			Vector<Object> tableValues = new Vector<>();
			tableValues.add(s);
			rawsData.add(tableValues);
		}

		javax.swing.SwingUtilities.invokeLater(new Runnable() {
			public void run() {
				tbTagInfo.updateUI();
			}
		});

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
		if (architecture.equals("AMD64") || architecture.equals("X64")
				|| architecture.equals("UNIVERSAL")) {
			arType = rfid_def.AR_X64;
		} else {
			arType = rfid_def.AR_X86;
		}

		rfidlib_reader.LoadLib(libPath, osType, arType);

		int m_cnt = rfidlib_reader.RDR_GetLoadedReaderDriverCount();
		int nret = 0;
		for (int i = 0; i < m_cnt; i++) {
			char[] valueBuffer = new char[256];
			Integer nSize = new Integer(0);
			String sDes;
			nret = rfidlib_reader.RDR_GetLoadedReaderDriverOpt(i,
					rfid_def.LOADED_RDRDVR_OPT_CATALOG, valueBuffer, nSize);
			if (nret == 0) {
				sDes = String.copyValueOf(valueBuffer, 0, nSize);
				if (sDes.equals(rfid_def.RDRDVR_TYPE_READER)) {
					rfidlib_reader
							.RDR_GetLoadedReaderDriverOpt(i,
									rfid_def.LOADED_RDRDVR_OPT_NAME,
									valueBuffer, nSize);
					sDes = String.copyValueOf(valueBuffer, 0, nSize);
					jcmbReaderType.addItem(sDes);
				}
			}
		}
		if (m_cnt > 0) {
			jcmbReaderType.setSelectedIndex(0);
		}

		int comCnt = rfidlib_reader.COMPort_Enum();
		for (int i = 0; i < comCnt; i++) {
			String comName = rfidlib_reader.COMPort_GetEnumItem(i);
			jcmbComName.addItem(comName);
		}

		// ö���Ѿ���Ե������豸
		// int nBluetooth = rfidlib_reader.Bluetooth_Enum();
		// for (int j = 0; j < nBluetooth; j++) {
		//
		// char nameBuf[] = new char[100];
		// Integer nameBufnSize = nameBuf.length;
		// char addrBuf[] = new char[100];
		// Integer addrBufnSize = nameBuf.length;
		//
		// rfidlib_reader.Bluetooth_GetEnumItem(j, (byte) 1, nameBuf,
		// nameBufnSize);
		//
		// rfidlib_reader.Bluetooth_GetEnumItem(j, (byte) 2, addrBuf,
		// addrBufnSize);
		//
		// jcmbBlueTooth.addItem(String
		// .copyValueOf(nameBuf, 0, nameBuf.length));
		//
		// arrayBluetoothaddr.add(String.copyValueOf(addrBuf, 0,
		// addrBuf.length));
		//
		// }
		//
		// if (jcmbBlueTooth.getItemCount() > 0) {
		// jcmbBlueTooth.setSelectedIndex(0);
		// }

	}

	public MainFrm() {
		init_ui();

		init_event();

		LoadLibrary();

	}

	public static void main(String[] args) {
		// TODO Auto-generated method stub

		new MainFrm();
	}

}
