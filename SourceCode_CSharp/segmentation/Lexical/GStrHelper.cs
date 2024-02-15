using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLP_Segmentation.Lexical
{
    public class GStrHelper
    {
        public static readonly char[] Cfg_BiaoDian = " `~!@#$%^&*()[]\\{}_+|;':\",./<>?，。！-=￥【】《》？“”：（）…、".ToCharArray();

        public static bool IsChinese(char c)
        {
            return c >= 0x4E00 && c <= 0x9FA5;// 根据字节码判断
        }

        // 判断一个字符串是否含有中文
        public static bool isAllChinese(String str)
        {
            if (str == null) return false;
            //for (char c : str.toCharArray())
            foreach(char c in str)
            {
                if (!IsChinese(c)) 
                    return false;// 有一个中文字符就返回
            }
            return true;
        }

        public static void 符号计数_Increase(string _strLeft, string crrLine, ref int _nCount)
        {
            for (int i = 0; i < crrLine.Length;)
            {
                int nTmp = crrLine.IndexOf(_strLeft, i);
                if (nTmp >= 0)
                {
                    _nCount++;
                    i = nTmp + _strLeft.Length;
                }
                else
                    break;
            }

            //return n双括号;
        }

        public static void 符号计数_Decrease(string _strLeft, string crrLine, ref int _nCount)
        {
            //string _strLeft = "{{";
            for (int i = 0; i < crrLine.Length;)
            {
                int nTmp = crrLine.IndexOf(_strLeft, i);
                if (nTmp > 0)
                {
                    _nCount--;
                    i = nTmp + _strLeft.Length;
                }
                else
                    break;
            }

            //return n双括号;
        }

    }//class GStrHelper
}
