package RFID;

public class rfidlib_aip_iso14443B
{
	public static void LoadLib(String sLibPath, int osType, int arType)
	{
		String libPath = "";
		String osName = "";
		String architecture = "";
		if (osType == rfid_def.VER_LINUX)
		{
			osName = "Linux";
		}
		else if (osType == rfid_def.VER_WINDOWS)
		{
			osName = "Windows";
		}

		if (arType == rfid_def.AR_X86)
		{
			architecture = "x86";
		}
		else if (arType == rfid_def.AR_X64)
		{
			architecture = "x64";
		}

		if (osName.equals("Windows"))
		{
			libPath = String.format("%s/libs/%s/%s/rfidlib_aip_iso14443b.dll",
					sLibPath, osName, architecture);
			System.load(libPath);
			libPath = String.format("%s/libs/%s/%s/jni_rfidlib_aip_iso14443B.dll",
					sLibPath, osName, architecture);
			System.load(libPath);
		}
		else if(osName.equals("Linux"))
		{
		}
	}
	
	
	public native static int ISO14443B_ParseTagDataReport(long hTagReport,
										  Integer aip_id,
										  Integer tag_id,
										  Integer ant_id,
										  Integer metaFlags,
										  byte tagData[],
										  Integer tagDataLen)  ;


	public native static int ISO14443B_CreateInvenParam(long hInvenParamSpecList,
															byte AntennaID,
															byte AFI ,
															byte SlotNum
															);


}
