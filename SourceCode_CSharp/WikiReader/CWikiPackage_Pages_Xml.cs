using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ShaoChenYe.DevFramework.MySqlLib;

namespace KnowedgeBox.WikiReader
{
    public class CWikiPackage_Pages_Xml
    {
        public static readonly int Cfg_MaxPagesPerParse = 5000;


        public delegate void Fun_ParseOnePage_xml(CPageLines _pageLines);
        public delegate void Fun_ReportProcessingSteps(string _reportStr);

        public static Fun_ReportProcessingSteps Cfg_ReporterFun;

        /// <summary>
        /// 该函数每次处理完一个页面后都会被调用一次，函数内自己判定本次是否需要执行写数据库操作。
        /// </summary>
        public delegate void WriteToDB(CSingleSqlExecuter In_DbConn, bool _bForceWriteRestAllItems);

        //public string SrcFileUrl;

        //public string OutputDirRoot;

        //public int NPagesPreFile;

        /// <summary>
        /// saperate a big package into many small files.
        /// </summary>
        /// <returns> pages found in the big package </returns>
        public static int SplitBigPackage(string _srcFileUrl,
            string _outputDirRoot,
            int _nPagesPreFile)
        {

            FileStream fs = new FileStream(_srcFileUrl, FileMode.Open, FileAccess.Read);
            StreamReader sd = new StreamReader(fs);

            List<List<string>> pages = new List<List<string>>();
            List<string> crrPage = null;
            //int pagesPerFile = 2000;
            int lastnPage = 0, nPages = 0, nfiles = 0;

            for (int i = 0; ; i++)
            {
                string crrLine = sd.ReadLine();

                if (crrLine == null)
                    break;

                if (nPages != 0
                    && lastnPage != nPages
                    && nPages % _nPagesPreFile == 0)
                {
                    if (pages != null)
                    {
                        lastnPage = nPages;
                        WritePagesToFile(pages, nfiles, _nPagesPreFile, _outputDirRoot);
                        pages.Clear();
                    }
                    pages = new List<List<string>>();
                    nfiles++;
                }

                if (crrLine == "  <page>")
                {
                    crrPage = new List<string>();
                    pages.Add(crrPage);
                    nPages++;
                }
                else if (crrLine == "  </page>")
                {
                    crrPage.Add(crrLine);
                    crrPage = null;

                }

                if (crrPage == null)
                    continue;

                crrPage.Add(crrLine);

            }//for (int i = 0; i < 100; i++)

            if (pages != null)
            {
                lastnPage = nPages;
                WritePagesToFile(pages, nfiles, _nPagesPreFile, _outputDirRoot);
                pages.Clear();
            }

            pages.Clear();

            return nPages;
        }//SplitBigPackage

        static void WritePagesToFile(List<List<string>> _pages,
            int _fNumber,
            int _pagesPerFile,
            string _outputRootDir)
        {
            int bPageNumber = _pagesPerFile * _fNumber;
            //string rootDir = @"D:\AIresources\Wiki-Cn\zhwiki-20210720-pages-articles\";
            string filename = $"page{bPageNumber}_{bPageNumber + _pages.Count}.xml";
            string fileURL = Path.Combine(_outputRootDir, filename);
            //FileStream fs = new FileStream(
            //     Path.Combine(rootDir, filename),
            //      FileMode.OpenOrCreate,
            //       FileAccess.Write);

            StringBuilder st = new StringBuilder();
            foreach (var pageLines in _pages)
            {
                foreach (var crrLine in pageLines)
                    st.AppendLine(crrLine);
            }

            File.WriteAllText(fileURL, st.ToString(), Encoding.UTF8);
        }//WritePagesToFile(List<List<string>> _pages)

        /// <summary>
        /// saperate a big package into many small files.
        /// </summary>
        /// <returns> pages found in the big package </returns>
        public static int ScanBigPackage(
            string _srcFileUrl,
            //string _outputDirRoot,
            //int _nPagesPreFile,
            Fun_ParseOnePage_xml _funParseOnePage,
            WriteToDB _funWrite2DB ,
            CSingleSqlExecuter _dbConn,
            int _MaxMultiThread)
        {

            FileStream fs = new FileStream(_srcFileUrl, FileMode.Open, FileAccess.Read);
            StreamReader sd = new StreamReader(fs);

            //string[] allLines = File.ReadAllLines(_srcFileUrl);

            //List<List<string>> pages = new();
            List<string> crrPageLines = null;
            List<CPageLines> crrPagesStrList = new List<CPageLines>();
            //int pagesPerFile = 2000;
            //int lastnPage = 0,  nfiles = 0;
            int nPages = 0;

            int crrBeginLine = -1;
            for (int crrLineNum = 1; ; crrLineNum++)
            {
                //string crrLine = allLines[i];
                string crrLine = sd.ReadLine();

                if (crrLine == null)
                    break;

                //if (nPages != 0
                //    && lastnPage != nPages
                //    && nPages % _nPagesPreFile == 0)
                //{
                //    if (pages != null)
                //    {
                //        lastnPage = nPages;
                //        WritePagesToFile(pages, nfiles, _nPagesPreFile, _outputDirRoot);
                //        pages.Clear();
                //    }
                //    pages = new List<List<string>>();
                //    nfiles++;
                //}

                if (crrLine == "  <page>")
                {
                    crrBeginLine = crrLineNum;
                     crrPageLines = new List<string>();
                    //pages.Add(crrPage);
                    nPages++;
                }
                else if (crrLine == "  </page>")
                {
                    crrPageLines.Add(crrLine);

                    //将crrPage 加入到crrPageList中，并处理 crrPageList
                    crrPagesStrList.Add(new CPageLines() { Lines = crrPageLines, nBeginLine = crrBeginLine });
                    if (crrPagesStrList.Count >= Cfg_MaxPagesPerParse)
                    {
                        RunPageStrList(_funParseOnePage, _funWrite2DB, _dbConn, _MaxMultiThread, crrPagesStrList);
                        crrPagesStrList.Clear();

                        //if (Cfg_ReporterFun != null)
                        //   Cfg_ReporterFun($"{nPages}");
                    }
                    crrPageLines = null;
                    crrBeginLine = -1;

                    continue;

                }//else if (crrLine == "  </page>")
                
                //如果是非Page之间的内容，就跳过。
                if (crrPageLines == null)
                    continue;                    
                
                crrPageLines.Add(crrLine);

            }//for (int i = 0; i < 100; i++)

            //如果还有没处理完的Pages，就补充处理一次。
            if (crrPagesStrList.Count > 0)
            {
                RunPageStrList(_funParseOnePage, _funWrite2DB, _dbConn, _MaxMultiThread, crrPagesStrList);
                crrPagesStrList.Clear();
                GC.Collect();
                //if (_MaxMultiThread != 1)
                //{
                //    Parallel.ForEach(crrPagesStrList,
                //        new ParallelOptions() { MaxDegreeOfParallelism = _MaxMultiThread },
                //        crrItem =>
                //        {
                //            //string xmlStr = GWikiHelper.StrLines2Str(crrItem);
                //            _funParseOnePage.Invoke(crrItem);
                //        });
                //}
                //else
                //{
                //    //foreach(var crrPage in crrPagesStrList )
                //    for (int i = 0; i < crrPagesStrList.Count; i++)
                //    {
                //        CPageLines crrPage = crrPagesStrList[i];
                //        _funParseOnePage.Invoke(crrPage);
                //    }
                //}

                //crrPagesStrList.Clear();
            }//if (crrPagesStrList.Count > 0)

                ////再次写入剩余的数据。
                //if (_funWrite2DB != null)
                //_funWrite2DB.Invoke(_dbConn, true);


            //if (pages != null)
            //{
            //    lastnPage = nPages;
            //    WritePagesToFile(pages, nfiles, _nPagesPreFile, _outputDirRoot);
            //    pages.Clear();
            //}

            //pages.Clear();
            return nPages;

        }//SplitBigPackage

        private static void RunPageStrList(Fun_ParseOnePage_xml _funParseOnePage, WriteToDB _funWrite2DB, CSingleSqlExecuter _dbConn, int _MaxMultiThread, List<CPageLines> crrPagesStrList)
        {
            /// if multi thread
            if (_MaxMultiThread != 1)
            {

                Parallel.ForEach(crrPagesStrList,
                new ParallelOptions() { MaxDegreeOfParallelism = _MaxMultiThread },
                crrItem =>
                {
                    //string xmlStr = GWikiHelper.StrLines2Str(crrItem);
                    _funParseOnePage.Invoke(crrItem);

                    if (_funWrite2DB != null)
                        _funWrite2DB.Invoke(_dbConn, false);

                });
            }
            else ///else if single thread
            {
                //foreach(var crrPage in crrPagesStrList )
                for (int i = 0; i < crrPagesStrList.Count; i++)
                {
                    CPageLines crrPage = crrPagesStrList[i];
                    _funParseOnePage.Invoke(crrPage);

                    if (_funWrite2DB != null)
                        _funWrite2DB.Invoke(_dbConn, false);

                }
            }//else ///else if single thread

        }//RunPageStrList

    }//class CSrcPackage_Pages
}//namespace
