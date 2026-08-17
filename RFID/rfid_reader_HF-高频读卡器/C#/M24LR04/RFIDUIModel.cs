using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M24LR04
{
    class RFIDUIModel 
    {

        bool select = false;
        String uid = "";
        String tips = "";
        int cnt = 1;

        List<Byte> antFoundTheTag =new List<byte>();

        public RFIDUIModel(bool select,String uid,int cnt,String tips)
        {
            this.select = select;
            this.uid = uid;
            this.tips = tips;
            this.cnt = cnt;
        }

        public void SetFindTheTagInTheAnt(byte ant)
        {

            if (!AntFoundTheTag.Contains(ant))
            {
                AntFoundTheTag.Add(ant);
            }
        }

        public bool Select
        {
            get
            {
                return select;
            }

            set
            {
                select = value;
            }
        }

        public string UID
        {
            get
            {
                return uid;
            }

            set
            {
                uid = value;
            }
        }




        public string Tips
        {
            get
            {
                return tips;
            }

            set
            {
                tips = value;
            }
        }

        public int Count
        {
            get
            {
                return cnt;
            }

            set
            {
                cnt = value;
            }
        }

        public List<byte> AntFoundTheTag
        {
            get
            {
                return antFoundTheTag;
            }

            set
            {
                antFoundTheTag = value;
            }
        }
    }
}
