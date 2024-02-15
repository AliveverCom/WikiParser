using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace KnowedgeBox.WikiReader
{
    public class GChineseHelper
    {
        [DllImport("kernel32.dll", EntryPoint = "LCMapStringA")]
        public static extern int LCMapStr(int local, int wdMapFlag, byte[] lpSrcStr, int cchSrc, byte[] lpDestStr, int cchDest);
        private static int LCMap_JianTi = 0x02000000;
        private static int LCMap_FanTi = 0x04000000;

        public static Encoding Cfg_DeftEncoding = Encoding.GetEncoding("GB18030");

        public static string JianTi2FanTi(string _src, Encoding _coding)
        {
            byte[] srcBytes = _coding.GetBytes(_src);
            byte[] rstBytes = new byte[srcBytes.Length];

            LCMapStr(2052, LCMap_FanTi, srcBytes, srcBytes.Length, rstBytes, rstBytes.Length);

            string rstStr = _coding.GetString(rstBytes);

            return rstStr;
        }//JianTi2FanTi(string _src)

        public static string FanTi2JianTi(string _src, Encoding _coding)
        {
            byte[] srcBytes = _coding.GetBytes(_src);
            byte[] rstBytes = new byte[srcBytes.Length];

            LCMapStr(2052, LCMap_JianTi, srcBytes, -1, rstBytes, rstBytes.Length);

            string rstStr = _coding.GetString(rstBytes);

            return rstStr;

        }//JianTi2FanTi(string _src)


    }//class GChineseEncodeHelper
}
