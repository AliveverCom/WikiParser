using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowedgeBox.WikiReader
{
    public class CWikiPackage_Pages_Xml
    {
        public static readonly int Cfg_MaxPagesPerParse = 100;


        public delegate void ParseOnePage_xml(string _pageXmlStr);

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

            List<List<string>> pages = new();
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
                    crrPage = new();
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
            ParseOnePage_xml _parseFun)
        {

            FileStream fs = new FileStream(_srcFileUrl, FileMode.Open, FileAccess.Read);
            StreamReader sd = new StreamReader(fs);

            //List<List<string>> pages = new();
            List<string> crrPageLines = null;
            List<List<string>> crrPagesStrList = new();
            //int pagesPerFile = 2000;
            //int lastnPage = 0,  nfiles = 0;
            int nPages = 0;


            for (int i = 0; ; i++)
            {
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
                    crrPageLines = new();
                    //pages.Add(crrPage);
                    nPages++;
                }
                else if (crrLine == "  </page>")
                {
                    crrPageLines.Add(crrLine);

                    //将crrPage 加入到crrPageList中，并处理 crrPageList
                    crrPagesStrList.Add(crrPageLines);
                    if (crrPagesStrList.Count >= Cfg_MaxPagesPerParse)
                    {
                        Parallel.ForEach(crrPagesStrList,
                            new ParallelOptions() { MaxDegreeOfParallelism = GCfg_Wiki.Cfg_MaxMultiThread },
                            crrItem =>
                            {
                                string xmlStr = GWikiHelper.StrLines2Str(crrItem);
                                _parseFun.Invoke(xmlStr);

                            });

                        crrPagesStrList.Clear();
                    }
                    crrPageLines = null;
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
                Parallel.ForEach(crrPagesStrList,
                    new ParallelOptions() { MaxDegreeOfParallelism = GCfg_Wiki.Cfg_MaxMultiThread },
                    crrItem =>
                    {
                        string xmlStr = GWikiHelper.StrLines2Str(crrItem);
                        _parseFun.Invoke(xmlStr);
                    });

                crrPagesStrList.Clear();
            }


            //if (pages != null)
            //{
            //    lastnPage = nPages;
            //    WritePagesToFile(pages, nfiles, _nPagesPreFile, _outputDirRoot);
            //    pages.Clear();
            //}

            //pages.Clear();
            return nPages;

        }//SplitBigPackage

    }//class CSrcPackage_Pages
}//namespace
