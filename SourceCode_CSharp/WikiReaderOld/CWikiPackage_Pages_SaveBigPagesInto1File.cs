using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KnowedgeBox.WikiReader
{
    /// <summary>
    ///  接受一个pase的字符串。如果内容的长度超过 Cfg_MinPageBytes ， 就把结果存入指定文件。
    /// </summary>
    public class CWikiPackage_Pages_SaveBigPagesInto1File
    {
        /// <summary>
        /// 
        /// </summary>
        public int Cfg_MinPageBytes = 204800;

        public int Cfg_MaxPages = 100;
        protected int Proc_writtenPages = 0;

        public string In_OutputFileUrl;

        public CWikiPackage_Pages_SaveBigPagesInto1File(string _OutputFileUrl)
        {
            this.In_OutputFileUrl = _OutputFileUrl;

        }//CWikiPackage_Pages_SaveBigPagesInto1File()

        public void CheckAndSaveOnePage_xml(string _pageXmlStr)
        {
            if (Proc_writtenPages >= Cfg_MaxPages)
                return;

            if (_pageXmlStr.Length >= this.Cfg_MinPageBytes)
            {
                lock (GWikiHelper.MultiThreadLocker)
                {
                    File.AppendAllText(this.In_OutputFileUrl, _pageXmlStr, Encoding.UTF8);
                    Proc_writtenPages++;
                }//lock
            }//if (_pageXmlStr.Length >= this.Cfg_MinPageBytes)

        }//CheckAndSaveOnePage_xml(string _pageXmlStr)


    }//public class CWikiPackage_Pages_SaveBigPagesInto1File
}//namespace
