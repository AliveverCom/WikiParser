using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLP_Segmentation.Lexical
{
    /// <summary>
    /// a single word
    /// </summary>
    public class CWordItem
    {
        public enum EComfirmType
        {
            Unknow = 0,
            RealWord = 1,
            Suspected = 2,
            NotWord = 99
        }

        public int WordId;

        public string WordText;

        public long UsedTimes;

        public EComfirmType ComfirmType = EComfirmType.Unknow;

        public bool PoS_Confirmed = false;

    }//class CWordItem
}
