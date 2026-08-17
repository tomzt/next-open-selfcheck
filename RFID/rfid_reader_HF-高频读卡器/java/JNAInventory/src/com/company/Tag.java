package com.company;

public class Tag
{
    public Tag()
    {

    }


    private byte []uid;
    private byte ant;
    private byte tagid;
    private byte []readBlockData;


    public byte[] getUid() {
        return uid;
    }

    public void setUid(byte[] uid) {
        this.uid = uid;
    }

    public byte getAnt() {
        return ant;
    }

    public void setAnt(byte ant) {
        this.ant = ant;
    }

    public byte getTagid() {
        return tagid;
    }

    public void setTagid(byte tagid) {
        this.tagid = tagid;
    }

    public byte[] getReadBlockData() {
        return readBlockData;
    }

    public void setReadBlockData(byte[] readBlockData) {
        this.readBlockData = readBlockData;
    }
}
