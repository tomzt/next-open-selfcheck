package Main;

public class DataReport {
	public int tag_id;
	public int ant_id; 
	public	String sUid;
	public byte uid[];
	
	public DataReport(int tag_id,int ant_id,String sUid,byte[] uid)
	{
		this.tag_id=tag_id;
		this.ant_id=ant_id;
		
		this.sUid=sUid;
		
		this.uid=new byte[uid.length];
		
		System.arraycopy(uid, 0, this.uid, 0, uid.length);
		
	}
}
