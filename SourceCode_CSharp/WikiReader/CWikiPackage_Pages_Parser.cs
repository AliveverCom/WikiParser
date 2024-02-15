using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.InteropServices;
using KnowedgeBox.WikiReader.MemoryIndex;
using System.Web.Script.Serialization;

using ShaoChenYe.DevFramework.MySqlLib;

namespace KnowedgeBox.WikiReader
{
    /// <summary>
    /// 这是一个用于实验的类。2021年8月，第一次解析wiki内容时候的实验。
    /// 
    /// </summary>
    public class CWikiPackage_Pages_Parser
    {

        /// <summary>
        /// 超过这个长度的 page 都将被丢弃
        /// </summary>
        public short Cfg_MaxPageTitle = 100;

        /// <summary>
        /// 当内存中积累了多少条记录以后强制写入数据库。
        /// </summary>
        public int Cfg_MaxMemoryItems2WrittenDB = 100000;

        public bool Cfg_Write2DB_PageSummary = false;

        //public bool Cfg_Write2DB_PageLinks = false;

        /// <summary>
        /// 所有重定向的词条实际上是作废的词条，直接跳过
        /// </summary>
        public string[] Cfg_skipPageTextPrefix = new string[] { "#重定向", "WikiProject:" };

        /// <summary>
        /// 所有重定向的词条实际上是作废的词条，直接跳过
        /// </summary>
        public string[] Cfg_skipPageTitlePrefix = new string[] {  "WikiProject:",
            "跨语言链接/错误", "申请成为管理员", "〇", "Ꙩ", "Ꙫ" };

        public Dictionary<EPageTitleClass, List<CPageSummary>> Out_PageSmyDict = new Dictionary<EPageTitleClass, List<CPageSummary>>();


        public List<CPageLink> Out_PageLinks = new List<CPageLink>();

        public CIdx_PageSummary Out_Pages_MemIdx = new CIdx_PageSummary();

        public List<string> Out_wikiPages = new List<string>();

        /// <summary>
        /// Dictionary<int pageId, CPageChapter>
        /// </summary>
        public Dictionary<int, List<CPageChapter>> Out_PageChapter = new Dictionary<int, List<CPageChapter>>();
        //public Dictionary<string, Int32>  TitleWithMaoHao = new ();

        public delegate void ExtFun_ParseOnePage_xml(CWikiPage _crrPage);

        private int Proc_CrrPageChapterId = 0;

        public ExtFun_ParseOnePage_xml ExtFun_ParseOnePage_For_xml;
        #region Public Methods

        public CWikiPackage_Pages_Parser()
        {
            //预先设定wiki标题分类，从数据分析中得出
            //PageSmyDict.Add(string.Empty, new List<CPageSummary>());
            //{[, Count = 2204153]}
            //{[Wikipedia, Count = 68370]}
            //{[Help, Count = 917]}
            //{[WikiProject, Count = 3132]}
            //{[Template, Count = 982353]}
            //{[File, Count = 57945]}
            //{[MediaWiki, Count = 8579]}
            //{[Category, Count = 398598]}
            //{[Portal, Count = 10761]}
            //{[Module, Count = 3901]}
            //{[Draft, Count = 1006]}
            //{[Chapter, Count = 35861]}
            foreach (EPageTitleClass crrE in Enum.GetValues(typeof(EPageTitleClass)))
                Out_PageSmyDict.Add(crrE, new List<CPageSummary>());


        }//CWikiPackage_Pages_TesterShao()

        public void ParseOnePage_All(CPageLines _pageLines)
        {
            string _pageXmlStr = _pageLines.GetAllLinesAsOneString();
            CWikiPage crrPage = new CWikiPage();
            crrPage.ParseXml_Chinese(_pageXmlStr);

            //bool bSikp = 
            if (this.IsSikpThisPage(crrPage))
                return;

            ////// Parsing PageSummary
            CPageSummary ps = new CPageSummary(crrPage);
            ps.nBeginLine = _pageLines.nBeginLine;
            ps.nLines = _pageLines.Lines.Count;
            this.Out_Pages_MemIdx.AddPageSummary(crrPage.TitleClass, ps);

            ProcessOnePage_All(crrPage );


        }//ParseOnePage_xml(string _pageXmlStr)
        public void ParseOnePageSummary_xml(CPageLines _pageLines)
        {
            string _pageXmlStr = _pageLines.GetAllLinesAsOneString();
            CWikiPage crrPage = new CWikiPage();
            crrPage.ParseXml_Chinese(_pageXmlStr);

            //bool bSikp = 
            if (this.IsSikpThisPage(crrPage))
                return;

            ProcessOnePage_PageSummary(crrPage);
            //this.PageSmyList.Add(ps);
        }//ParseOnePage_xml(string _pageXmlStr)

        public void ParseOnePage_xml_ExtFun(CPageLines _pageLines)
        {
            string _pageXmlStr = _pageLines.GetAllLinesAsOneString();
            CWikiPage crrPage = new CWikiPage();
            crrPage.ParseXml_Chinese(_pageXmlStr);

            //bool bSikp = 
            if (this.IsSikpThisPage(crrPage))
                return;


            ExtFun_ParseOnePage_For_xml(crrPage);
            //this.PageSmyList.Add(ps);
        }//ParseOnePage_xml(string _pageXmlStr)


        public void ParseOnePageText_PageChapter(CPageLines _pageLines)
        {
            string _pageXmlStr = _pageLines.GetAllLinesAsOneString();
            CWikiPage crrPage = new CWikiPage();
            crrPage.ParseXml_Chinese(_pageXmlStr);

            if (this.IsSikpThisPage(crrPage))
                return;

            if (crrPage.TitleClass != EPageTitleClass.DeftPage)
                return;

            ProcessOnePage_PageCharpter(crrPage);

        }// ParseOnePageText_PageLinks(string _pageXmlStr)

        /// <summary>
        /// 解析一个页面 正文 
        /// </summary>
        /// <param name="_pageXmlStr"></param>
        public void ParseOnePageText_PageLinks(CPageLines _pageLines)
        {
            string _pageXmlStr = _pageLines.GetAllLinesAsOneString();
            CWikiPage crrPage = new CWikiPage();
            crrPage.ParseXml_Chinese(_pageXmlStr);

            if (this.IsSikpThisPage(crrPage))
                return;

            if (crrPage.TitleClass != EPageTitleClass.DeftPage)
                return;

            ProcessOnePage_PageLinks(crrPage);

        }//ParseOnePageText_xml(string _pageXmlStr)


        #endregion        //Public Methods

        #region  private main methods

        private void ProcessOnePage_All(CWikiPage crrPage)
        {
            if (crrPage.TitleClass != EPageTitleClass.DeftPage)
                return;

            StringReader sr = new StringReader(crrPage.Revision.Text);

            //int nlastDeng = 0;

            List<CPageChapter> crrChapterList = new List<CPageChapter>();

            CPageChapter lastChapterRef = null;
            List<string> linesInCrrChapter = new List<string>();
            List<CPageLink> linksInThisPage = new List<CPageLink>();

            for (; ; )
            {
                string crrLine = sr.ReadLine();
                if (crrLine == null)
                    break;

                if (crrLine.StartsWith("=") && crrLine.EndsWith("="))
                {
                    CPageChapter pl = ProcessOneLine_Chapter( crrPage,  crrLine, lastChapterRef);
                    
                    if (pl != null)
                    {
                        //先解析上一个 章节的内容里的 Links
                        List<CPageLink> linksInCrrChapter = ParsePageLinksInOnChapter(crrPage, linesInCrrChapter, lastChapterRef);
                        linksInThisPage.AddRange(linksInCrrChapter);

                        // 将当前章节标题加入到章节列表
                        crrChapterList.Add(pl);
                        lastChapterRef = pl;

                        // 重置章节内容行表
                        linesInCrrChapter.Clear();
                        continue; // skip current line if it has been already processed as a page chapter.
                    }
                }//if (crrLine.StartsWith("=") && crrLine.EndsWith("="))

                linesInCrrChapter.Add(crrLine);

            }//for(; ; )

            lock (this.Out_PageChapter)
            {
                if (crrChapterList.Count > 0)
                    this.Out_PageChapter.Add(crrPage.Id, crrChapterList);
            }//lock (Out_PageChapter)

            lock (this.Out_PageLinks)
            {
                if (linksInThisPage.Count > 0)
                    this.Out_PageLinks.AddRange(linksInThisPage);
            }
        }//ProcessOnePage_All

        private  List<CPageLink>   ParsePageLinksInOnChapter(CWikiPage crrPage, List<string> linesInCrrChapter, CPageChapter lastChapterRef)
        {
            if (lastChapterRef != null && lastChapterRef.ChapterText.Length > this.Cfg_MaxPageTitle)
                return new  List<CPageLink>();

            List<CPageLink> linksInCrrChapter = new List<CPageLink>();

            foreach (string crrLine in linesInCrrChapter)
            {
                ///// 解析page 内容的 交叉引用
                var refList = Regex.Matches(crrLine, @"(\[\[).*?]]", RegexOptions.IgnoreCase);


                if (!(refList != null && refList.Count != 0))
                    continue;

                int ndebug = 0;
                foreach (Match crrMatch in refList)
                {
                    string crrStr = crrMatch.Value.Substring(2, crrMatch.Value.Length - 4);
                    //先忽略[[:开头的各种特殊引用
                    if (crrStr.StartsWith(":") || crrStr.StartsWith("File:") || crrStr.StartsWith("Image:"))
                        continue;

                    if (crrStr.Length > 6)
                    {
                        string headStr = crrStr.Substring(0, 6).ToLower();
                        if (headStr.StartsWith("file:")
                            || headStr.StartsWith("文件:")
                            || headStr.StartsWith("图像:")
                            || headStr.StartsWith("圖像:")
                            || headStr.StartsWith("image:")) //[0] = "圖像:SongstenGampoandwives.jpg|thumb|left|400px|[[尺尊公主"
                            continue;
                    }

                    List<string> matchWords = new List<string>();
                    //找到 中间还有 [[ 的字段，这有可能是因为 正则表达式的问题造成的。
                    if (crrStr.LastIndexOf("[[") > 0)
                    {
                        if (crrStr.StartsWith("包括"))
                            crrStr = crrStr.Replace("包括[[", string.Empty).Replace("]]", string.Empty);
                        else
                            matchWords = 从字符串中扫描括号中的内容(crrMatch.Value, "[[", "]]");

                        ndebug++;
                    }
                    else
                        matchWords.Add(crrStr);

                    foreach (string crrWord in matchWords)
                    {
                        string refPageTitle = string.Empty;
                        string refDisplayTitle = null;

                        /////处理 引用的转译文字
                        if (crrWord.Contains('|'))
                        {
                            string[] sp = crrWord.Split('|');
                            if (sp.Length > 2)
                            {
                                refPageTitle = sp[0];
                                //throw new Exception($"没有见过的 交叉引用格式 = {crrStr}");
                            }
                            else
                            {
                                refPageTitle = sp[0];
                                refDisplayTitle = sp[1];
                            }
                        }//if (crrStr.Contains('|') )
                        else
                            refPageTitle = crrWord;

                        if (refPageTitle.Length >= this.Cfg_MaxPageTitle)
                            continue;

                        if (refDisplayTitle != null &&
                             (refDisplayTitle.Contains("</font>")
                              || refDisplayTitle.Contains("<br>"))
                            )
                            refDisplayTitle = null;

                        if (refDisplayTitle != null && refDisplayTitle.Length >= 100)
                            refDisplayTitle = refDisplayTitle.Substring(99);

                        EPageTitleClass urlPageClass = EPageTitleClass.DeftPage;
                        if (refPageTitle.Contains(":"))
                        {
                            string ctgStr = "category:";

                            if (refPageTitle.Length > ctgStr.Length)
                            {
                                string tpStr = refPageTitle.Substring(0, ctgStr.Length).ToLower();
                                if (tpStr == ctgStr)
                                {
                                    urlPageClass = EPageTitleClass.Category;
                                    refPageTitle = refPageTitle.Substring(ctgStr.Length, refPageTitle.Length - ctgStr.Length);
                                }
                            }
                        }//if (refPageTitle.Contains(":"))

                        CPageLink pl = new CPageLink()
                        {
                            PageId = crrPage.Id,
                            UrlPageTitle = refPageTitle,
                            DisplayText = refDisplayTitle,
                            PageTitle = crrPage.Title,
                            UrlPageClass = urlPageClass,
                            ChapterStr = lastChapterRef == null ? null : lastChapterRef.ChapterText,
                             ChapterId = lastChapterRef == null ? -1 : lastChapterRef.ChapterId,
                        };

                        linksInCrrChapter.Add(pl);
                    }//foreach (string crrWord in matchWords)
                }//foreach (Match crrMatch in refList)

            }//foreach(string crrLine in linesInCrrChapter)

            return linksInCrrChapter;
        }//ParsePageLinksInOnChapter(linesInCrrChapter, lastChapterRef)

        private CPageChapter ProcessOneLine_Chapter(CWikiPage crrPage,  string crrLine, CPageChapter lastChapterRef)
        {
            ///循环统计看看是连续多少个 =
            short nDengHao_front = -1, nDengHao_end = -1;
            for (short i = 0; i < crrLine.Length; i++)
            {
                // if first end of ==
                if (crrLine[i] != '=' && nDengHao_front == -1)
                    nDengHao_front = (short)(i - 1);

                // if = again
                if (nDengHao_front > 0 && crrLine[i] == '=')
                {
                    nDengHao_end = i;
                    break;
                }
            }

            // if not found =xx=
            if (nDengHao_front == -1 || nDengHao_end == -1)
                return null;

            string topicTxt = crrLine.Substring(nDengHao_front + 1, nDengHao_end - nDengHao_front - 1);

            if (topicTxt.Length >= this.Cfg_MaxPageTitle)
                return null;

            //计算上级节点
            //lock (Out_PageChapter)
            //{
            CPageChapter parentRef = null;
            if (lastChapterRef == null)
                parentRef = null;
            else if (nDengHao_front == lastChapterRef.ChapterLevel)
                parentRef = lastChapterRef.ParentChapter_MemoryOnly;
            else if (nDengHao_front > lastChapterRef.ChapterLevel)
                parentRef = lastChapterRef;
            else
                parentRef = 按照等号逐渐回退找到上级(lastChapterRef, nDengHao_front);


            CPageChapter pl = new CPageChapter()
            {
                ChapterId = System.Threading.Interlocked.Increment(ref this.Proc_CrrPageChapterId),
                PageId = crrPage.Id,
                ParentChapterId = parentRef == null ? -1 : parentRef.ChapterId,
                ChapterLevel = nDengHao_front,
                ChapterText = topicTxt.Trim(),
                PageTitle = crrPage.Title,
                ParentChapter_MemoryOnly = parentRef,
            };

            return pl;

        }//ProcessOneLine_Chapter(string _crrLine)


        private void ProcessOnePage_PageCharpter(CWikiPage crrPage)
        {
            StringReader sr = new StringReader(crrPage.Revision.Text);

            //int nlastDeng = 0;

            List<CPageChapter> crrChapterList = new List<CPageChapter>();

            CPageChapter lastChapterRef = null;
            for (; ; )
            {
                string crrLine = sr.ReadLine();
                if (crrLine == null)
                    break;

                if (crrLine.StartsWith("=") && crrLine.EndsWith("="))
                {
                    ///循环统计看看是连续多少个 =
                    short nDengHao_front = -1, nDengHao_end = -1;
                    for (short i = 0; i < crrLine.Length; i++)
                    {
                        // if first end of ==
                        if (crrLine[i] != '=' && nDengHao_front == -1)
                            nDengHao_front = (short)(i - 1);

                        // if = again
                        if (nDengHao_front > 0 && crrLine[i] == '=')
                        {
                            nDengHao_end = i;
                            break;
                        }
                    }

                    // if not found =xx=
                    if (nDengHao_front == -1 || nDengHao_end == -1)
                        continue;

                    string topicTxt = crrLine.Substring(nDengHao_front + 1, nDengHao_end - nDengHao_front - 1);

                    if (topicTxt.Length >= this.Cfg_MaxPageTitle)
                        break;

                    //计算上级节点
                    //lock (Out_PageChapter)
                    //{
                    CPageChapter parentRef = null;
                    if (lastChapterRef == null)
                        parentRef = null;
                    else if (nDengHao_front == lastChapterRef.ChapterLevel)
                        parentRef = lastChapterRef.ParentChapter_MemoryOnly;
                    else if (nDengHao_front > lastChapterRef.ChapterLevel)
                        parentRef = lastChapterRef;
                    else
                        parentRef = 按照等号逐渐回退找到上级(lastChapterRef, nDengHao_front);


                    CPageChapter pl = new CPageChapter()
                    {
                        ChapterId = System.Threading.Interlocked.Increment(ref this.Proc_CrrPageChapterId),
                        PageId = crrPage.Id,
                        ParentChapterId = parentRef == null ? 0 : parentRef.ChapterId,
                        ChapterLevel = nDengHao_front,
                        ChapterText = topicTxt.Trim(),
                        PageTitle = crrPage.Title,
                    };

                    crrChapterList.Add(pl);

                    lastChapterRef = pl;

                    //排除空的情况
                }//if (crrLine.StartsWith("=") && crrLine.EndsWith("="))
            }//for(; ; )

            lock (this.Out_PageChapter)
            {
                //if (!this.Out_PageChapter.ContainsKey(crrPage.Id))
                this.Out_PageChapter.Add(crrPage.Id, crrChapterList);

            }//lock (Out_PageChapter)

        }//ProcessOnePage_PageCharpter

        private void ProcessOnePage_PageLinks(CWikiPage crrPage)
        {
            ///// 解析page 内容的 交叉引用
            var refList = Regex.Matches(crrPage.Revision.Text, @"(\[\[).*?]]", RegexOptions.IgnoreCase);

            if (refList != null && refList.Count != 0)
            {
                int ndebug = 0;
                foreach (Match crrMatch in refList)
                {
                    string crrStr = crrMatch.Value.Substring(2, crrMatch.Value.Length - 4);
                    //先忽略[[:开头的各种特殊引用
                    if (crrStr.StartsWith(":") || crrStr.StartsWith("File:") || crrStr.StartsWith("Image:"))
                        continue;

                    if (crrStr.Length > 6)
                    {
                        string headStr = crrStr.Substring(0, 6).ToLower();
                        if (headStr.StartsWith("file:")
                            || headStr.StartsWith("文件:")
                            || headStr.StartsWith("图像:")
                            || headStr.StartsWith("圖像:")
                            || headStr.StartsWith("image:")) //[0] = "圖像:SongstenGampoandwives.jpg|thumb|left|400px|[[尺尊公主"
                            continue;
                    }

                    List<string> matchWords = new List<string>();
                    //找到 中间还有 [[ 的字段，这有可能是因为 正则表达式的问题造成的。
                    if (crrStr.LastIndexOf("[[") > 0)
                    {
                        if (crrStr.StartsWith("包括"))
                            crrStr = crrStr.Replace("包括[[", string.Empty).Replace("]]", string.Empty);
                        else
                            matchWords = 从字符串中扫描括号中的内容(crrMatch.Value, "[[", "]]");

                        ndebug++;
                    }
                    else
                        matchWords.Add(crrStr);

                    foreach (string crrWord in matchWords)
                    {
                        string refPageTitle = string.Empty;
                        string refDisplayTitle = null;

                        /////处理 引用的转译文字
                        if (crrWord.Contains('|'))
                        {
                            string[] sp = crrWord.Split('|');
                            if (sp.Length > 2)
                            {
                                refPageTitle = sp[0];


                                //throw new Exception($"没有见过的 交叉引用格式 = {crrStr}");

                            }
                            else
                            {

                                refPageTitle = sp[0];
                                refDisplayTitle = sp[1];
                            }
                        }//if (crrStr.Contains('|') )
                        else
                            refPageTitle = crrWord;

                        if (refPageTitle.Length >= this.Cfg_MaxPageTitle)
                            return;

                        if (refDisplayTitle != null &&
                             (refDisplayTitle.Contains("</font>")
                              || refDisplayTitle.Contains("<br>"))
                            )
                            refDisplayTitle = null;

                        if (refDisplayTitle != null && refDisplayTitle.Length >= 100)
                            refDisplayTitle = refDisplayTitle.Substring(99);

                        EPageTitleClass urlPageClass = EPageTitleClass.DeftPage;
                        if (refPageTitle.Contains(":"))
                        {
                            string ctgStr = "category:";

                            if (refPageTitle.Length > ctgStr.Length)
                            {
                                string tpStr = refPageTitle.Substring(0, ctgStr.Length).ToLower();
                                if (tpStr == ctgStr)
                                {
                                    urlPageClass = EPageTitleClass.Category;
                                    refPageTitle = refPageTitle.Substring(ctgStr.Length, refPageTitle.Length - ctgStr.Length);
                                }
                            }
                        }//if (refPageTitle.Contains(":"))

                        CPageLink pl = new CPageLink()
                        {
                            PageId = crrPage.Id,
                            UrlPageTitle = refPageTitle,
                            DisplayText = refDisplayTitle == refPageTitle ? null: refDisplayTitle,
                            ChapterStr = null,
                            PageTitle = crrPage.Title,
                            UrlPageClass = urlPageClass,
                        };

                        lock (this.Out_PageLinks)
                        {
                            this.Out_PageLinks.Add(pl);
                        }//lock (this.Out_PageLinks)
                    }//foreach (string crrWord in matchWords)
                }//foreach (Match crrMatch in refList)

            }//if (refList != null && refList.Count != 0)
        }//ProcessOnePage_PageLinks

        private void ProcessOnePage_PageSummary(CWikiPage crrPage)
        {
            //解析得到 page 的基本索引信息
            CPageSummary ps = new CPageSummary(crrPage);

            lock (Out_PageSmyDict[crrPage.TitleClass])
            {
                Out_PageSmyDict[crrPage.TitleClass].Add(ps);
            }
        }//ProcessOnePage_PageSummary


        #endregion //private main methods


        protected List<string> 从字符串中扫描括号中的内容(string _srcStr, string _fuHaoLeft, string fuHaoRight)
        {
            List<string> rstList = new List<string>();

            if (_fuHaoLeft == "[[")
                _srcStr = _srcStr.Replace("[ [", "[[").Replace("] ]", "]]");

            //如果 只有一个]]，说明是wiki本身的错误。直接返回最后一个括号中的内容。
            if (_srcStr.IndexOf(fuHaoRight) == _srcStr.Length - fuHaoRight.Length)
            {
                int iLeft = _srcStr.LastIndexOf(_fuHaoLeft);

                if (iLeft < 0)
                    return rstList;
                else
                    iLeft += _fuHaoLeft.Length;

                int iRight = _srcStr.IndexOf(fuHaoRight, iLeft);
                string crrWord = _srcStr.Substring(iLeft, iRight - iLeft);
                rstList.Add(crrWord);
                return rstList;
            }

            //char crrFuHao = 'L';
            for (int i = 0; i < _srcStr.Length;)
            {
                int iLeft = _srcStr.IndexOf(_fuHaoLeft, i);

                if (iLeft < 0)
                    break;
                else
                    iLeft += _fuHaoLeft.Length;

                int iRight = _srcStr.IndexOf(fuHaoRight, iLeft);

                if (iRight < 0)
                    iRight = _srcStr.Length;

                string crrWord = _srcStr.Substring(iLeft, iRight - iLeft);
                rstList.Add(crrWord);

                i = iRight;
            }

            return rstList;
        }//从字符串中扫描括号中的内容()



        private CPageChapter 按照等号逐渐回退找到上级(CPageChapter _lastChapterRef, int _nDengHao_front)
        {
            //int headerDengHao = _nDengHao_front - 1;

            //lock (Out_PageChapter)
            //{
            //var rst = from a in this.Out_PageChapter where a.PageId == _crrPageId select a;

            if (_lastChapterRef == null)
                throw new Exception("_lastChapterRef should not be Null !");
                for (CPageChapter crrChapter= _lastChapterRef ; crrChapter!= null ; crrChapter = crrChapter.ParentChapter_MemoryOnly)
                {
                    if (crrChapter.ChapterLevel == _nDengHao_front)
                        return crrChapter;
                }//for(int i=this.Out_PageChapter.Count-1; i > -1; i --)
            //}//lock

            return null;

        }//按照等号逐渐回退找到上级(nDengHao_front)



        protected bool IsSikpThisPage(CWikiPage crrPage)
        {
            if (crrPage.Title.Length >= Cfg_MaxPageTitle)
                return true;


            foreach (string crrPrefix in this.Cfg_skipPageTitlePrefix)
                if (crrPage.Title.StartsWith(crrPrefix))
                    return true;

            if (crrPage.Revision.Text.StartsWith("#"))
            {
                string rdStr = "#REDIRECT";
                if (crrPage.Revision.Text.Length > rdStr.Length)
                    if (crrPage.Revision.Text.Substring(0, rdStr.Length).ToUpper() == rdStr)
                        return true;

                foreach (string crrPrefix in this.Cfg_skipPageTextPrefix)
                    if (crrPage.Revision.Text.StartsWith(crrPrefix))
                        return true;

                //debugStr = crrPage.Revision.Text;
            }

            return false;

        }//IsSikpThisPage()



        public void WriteToDB_WhenProcessing_PageLinks(CSingleSqlExecuter _DbConn, bool _bForceWriteRestAllItems)
        {
            string tableName = "page_link_" + EPageTitleClass.DeftPage.ToString();

            lock (this.Out_PageLinks)
            {
                //如果需要写数据库，就将列表中的items都写入数据库，并清空列表
                if (this.Out_PageLinks.Count > this.Cfg_MaxMemoryItems2WrittenDB
                    || _bForceWriteRestAllItems)
                {
                    //写入数据库以前全面转码
                    foreach (CPageLink crrLink in this.Out_PageLinks)
                    {
                        crrLink.UrlPageTitle = CSqlHelper.EncodeString2SqlString_Insert(crrLink.UrlPageTitle).Replace("\\\"", "_");
                        crrLink.DisplayText = (crrLink.DisplayText == null) ? null : CSqlHelper.EncodeString2SqlString_Insert(crrLink.DisplayText).Replace("\\\"", "_");
                        crrLink.PageTitle = CSqlHelper.EncodeString2SqlString_Insert(crrLink.PageTitle).Replace("\\\"", "_");
                        crrLink.ChapterStr = (crrLink.ChapterStr == null) ? null : CSqlHelper.EncodeString2SqlString_Insert(crrLink.ChapterStr).Replace("\\\"", "_");
                    }

                    _DbConn.Insert2Db<CPageLink>(this.Out_PageLinks, tableName, 1000);
                    // _DbConn.CreateIndexies(typeof(CPageLink), tableName, false);

                    this.Out_PageLinks.Clear();

                }//if (this.Cfg_Write2DB_PageLinks
            }// lock(this.Out_PageLinks)

        }//WriteToDB_WhenProcessing_PageLinks()

        public void WriteToDB_WhenProcessing_PageChapter(CSingleSqlExecuter _DbConn, bool _bForceWriteRestAllItems)
        {
            string tableName = GDBNameMaker.GetPageChapter_Table(EPageTitleClass.DeftPage);
            //this.Out_PageChapter;

            if (!(this.Out_PageChapter.Count > this.Cfg_MaxMemoryItems2WrittenDB / 5
                    || _bForceWriteRestAllItems))
                return;

            lock (this.Out_PageChapter)
            {
                //如果需要写数据库，就将列表中的items都写入数据库，并清空列表
                if (this.Out_PageChapter.Count > this.Cfg_MaxMemoryItems2WrittenDB / 5
                    || _bForceWriteRestAllItems)
                {
                    List<CPageChapter> allItems = new List<CPageChapter>();
                    foreach (List<CPageChapter> crrLst in this.Out_PageChapter.Values)
                        allItems.AddRange(crrLst);

                    //写入数据库以前全面转码
                    foreach (CPageChapter crrItem in allItems)
                    {
                        crrItem.ChapterText = CSqlHelper.EncodeString2SqlString_Insert(crrItem.ChapterText).Replace("\\\"", "_");
                        crrItem.PageTitle = CSqlHelper.EncodeString2SqlString_Insert(crrItem.PageTitle).Replace("\\\"", "_");
                    }

                    _DbConn.Insert2Db<CPageChapter>(allItems, tableName, 20);
                    // _DbConn.CreateIndexies(typeof(CPageLink), tableName, false);


                    foreach (List<CPageChapter> crrLst in this.Out_PageChapter.Values)
                        crrLst.Clear();

                    this.Out_PageChapter.Clear();

                }//if (this.Cfg_Write2DB_PageLinks
            }// lock(this.Out_PageLinks)

        }//WriteToDB_WhenProcessing_PageLinks()

        public List<string> WriteToDB_AllData_Charpters(CSingleSqlExecuter _dbConn)
        {
            //create database and write data
            //string tableName = GDBNameMaker.GetPageChapter_Table(EPageTitleClass.DeftPage);
        
                string tableName = GDBNameMaker.GetPageChapter_Table(EPageTitleClass.DeftPage);
                
                _dbConn.CreateTable(typeof(CPageChapter),
                null, tableName, null,
                ShaoChenYe.DevFramework.MySqlLib.EWhenExist.DeleteAndDoAgain, false);

                this.WriteToDB_WhenProcessing_PageChapter(_dbConn, true);

           //List<CPageChapter> allChapters = new List<CPageChapter>();

           // foreach (List<CPageChapter> crrList in this.Out_PageChapter.Values)
           // {
           //     foreach (CPageChapter crrChapter in crrList)
           //     {
           //         //if (packageTester.Out_Pages_MemIdx.Idx_PageTitle[EPageTitleClass.DeftPage].ContainsKey(crrChapter.ChapterText))
           //         CPageSummary ps = this.Out_Pages_MemIdx.GetPageInDeftPage(crrChapter.ChapterText);
           //         if (ps != null)
           //             crrChapter.Chapter2PageId = ps.PageId;
           //     }//foreach (CPageChapter crrChapter in crrList)

           //     allChapters.AddRange(crrList);
           // }// 

           // _dbConn.Insert2Db(allChapters, tableName , 20);

            //write index
            List<string> idxSqlList = new List<string>();
            idxSqlList.Add(_dbConn.GetCreateIndexies(typeof(CPageChapter), tableName, false));

            return idxSqlList; //_dbConn.ExecuteSqlStrAsVoid_Parallel(idxSqlList);

        }//WriteToDB_All_Charpters(CSingleSqlExecuter _dbConn)

        public List<string> WriteToDB_AllData_Links(CSingleSqlExecuter _dbConn)
        {
            //create database and write data
            string tableName = GDBNameMaker.GetPageLinks(EPageTitleClass.DeftPage);
            _dbConn.CreateTable(typeof(CPageLink),
                null, tableName, null,
                ShaoChenYe.DevFramework.MySqlLib.EWhenExist.DeleteAndDoAgain, false);

            //写入数据库以前全面转码
            foreach (CPageLink crrLink in this.Out_PageLinks)
            {
                crrLink.UrlPageTitle = CSqlHelper.EncodeString2SqlString_Insert(crrLink.UrlPageTitle).Replace("\\\"", "_");
                crrLink.DisplayText = (crrLink.DisplayText == null) ? null : CSqlHelper.EncodeString2SqlString_Insert(crrLink.DisplayText).Replace("\\\"", "_");
                crrLink.PageTitle = CSqlHelper.EncodeString2SqlString_Insert(crrLink.PageTitle).Replace("\\\"", "_");
                crrLink.ChapterStr = (crrLink.ChapterStr == null) ? null : CSqlHelper.EncodeString2SqlString_Insert(crrLink.ChapterStr).Replace("\\\"", "_");
            }

            _dbConn.Insert2Db<CPageLink>(this.Out_PageLinks, tableName, 20);
            // _DbConn.CreateIndexies(typeof(CPageLink), tableName, false);

            //this.Out_PageLinks.Clear();


            //write index
            List<string> idxSqlList = new List<string>();
            idxSqlList.Add(_dbConn.GetCreateIndexies(typeof(CPageLink), tableName, false));
            
            return idxSqlList; //_dbConn.ExecuteSqlStrAsVoid_Parallel(idxSqlList);

        }//WriteToDB_All_Links(CSingleSqlExecuter _dbConn)

        public List<string> WriteToDB_AllData_Pages( CSingleSqlExecuter _dbConn)
        {

            //create database and write data
            foreach (var pageClassE in this.Out_Pages_MemIdx.Idx_PageTitle.Keys)
            {
                string tableName1 = GDBNameMaker.GetPageSummary(pageClassE);// "wiki_pages_" + pageClassE.ToString();
                _dbConn.CreateTable(typeof(CPageSummary),
                            null, tableName1, null,
                            ShaoChenYe.DevFramework.MySqlLib.EWhenExist.DeleteAndDoAgain, false);

                List<CPageSummary> pagesOfcurrClass = this.Out_Pages_MemIdx.GetPageList(pageClassE);
                foreach (CPageSummary crrItem in pagesOfcurrClass)
                    crrItem.TitleStr = CSqlHelper.EncodeString2SqlString_Insert(crrItem.TitleStr).Replace("\\\"", "_");


                _dbConn.Insert2Db(pagesOfcurrClass, tableName1, 20);

            }//foreach (var pageClassE in packageTester.PageSmyDict.Keys)

            //write index
            List<string> idxSqlList = new List<string>();
            foreach (var pageClassE in this.Out_PageSmyDict.Keys)
            {
                string tableName1 = GDBNameMaker.GetPageSummary(pageClassE);// "wiki_pages_" + pageClassE.ToString();
                //_dbConn.CreateIndexies(typeof(CPageSummary), tableName1, false);
                idxSqlList.Add(_dbConn.GetCreateIndexies(typeof(CPageSummary), tableName1, false));
            }

            return idxSqlList; //_dbConn.ExecuteSqlStrAsVoid_Parallel(idxSqlList);

        }// WriteToDB_All_Pages()

        public static void GeneratePageChapterStatistic_TableDataIndex(ref List<string> sqlList_CreateTableData, ref List<string> sqlList_CreateIndex)
        {
            EPageTitleClass ptc = EPageTitleClass.DeftPage;

            string tableName = GDBNameMaker.GetPageChapter_Sta_GrpByChapter(ptc);

            string sqlTable = $"CREATE TABLE `{tableName}` "
                + " ("
                + "	`ChapterText` VARCHAR(100) NOT NULL COLLATE 'utf8mb4_0900_as_cs',"
                + "	`ChapterPageId` INT(11) NOT NULL,"
                + "	`ChapterCnt` INT(11) NOT NULL DEFAULT '0',"
                + "	`PageCnt` INT(11) NOT NULL DEFAULT '0',"
                + "	`MinLevel` SMALLINT(6) NULL DEFAULT NULL,"
                + "	`MaxLevel` SMALLINT(6) NULL DEFAULT NULL,"
                + "	`AvgLevel` FLOAT NULL DEFAULT NULL"
                + " )"
                + " COLLATE='utf8mb4_0900_ai_ci'"
                + " ENGINE=MYISAM "
                + " AS "
                + " SELECT  ChapterText, ChapterPageId, "
                + "         COUNT(ChapterId) ChapterCnt, "
                + "			COUNT(DISTINCT(PageId)) PageCnt, "
                + "			Min(ChapterLevel) MinLevel,"
                + "			Max(ChapterLevel) MaxLevel,"
                + "			round(Avg(ChapterLevel),1) AvgLevel"
                + $"	FROM {GDBNameMaker.GetPageChapter_Table(ptc)} "
                + "	GROUP BY ChapterText;";
            sqlList_CreateTableData.Add(sqlTable);

            string idxStr = $"ALTER TABLE `{tableName}`"
                + "    ADD INDEX `ChapterText` (`ChapterText`),"
                + "	ADD INDEX `ChapterPageId` (`ChapterPageId`),"
                + "	ADD INDEX `ChapterCnt` (`ChapterCnt`),"
                + "	ADD INDEX `PageCnt` (`PageCnt`),"
                + "	ADD INDEX `MinLevel` (`MinLevel`),"
                + "	ADD INDEX `MaxLevel` (`MaxLevel`),"
                + "	ADD INDEX `AvgLevel` (`AvgLevel`);";
            sqlList_CreateIndex.Add(idxStr);
        }//GeneratePageChapterStatistic_TableDataIndex()

        public static void GeneratePageLinkStatistic_TableDataIndex(ref List<string> sqlList_CreateTableData, ref List<string> sqlList_CreateIndex)
        {

            //sqlList_CreateTableData = new List<string>();
            EPageTitleClass ptc =  EPageTitleClass.DeftPage;
            //生成 页面引出统计表
            string sqlStrPo = $"CREATE TABLE {GDBNameMaker.GetPageLinks_StatisticOut(ptc)} "
                + " SELECT PageId, PageTitle, COUNT(*) cnt"
                + $"	FROM {GDBNameMaker.GetPageLinks(ptc)} l"
                + "	GROUP BY l.PageTitle;";
            sqlList_CreateTableData.Add(sqlStrPo); // GtWinApp.DbConn_public.ExecuteSqlStrAsVoid(sqlStrPo);

            //生成 页面被引用统计表
            string sqlStrPi = $"CREATE TABLE {GDBNameMaker.GetPageLinks_StatisticIn(ptc)} "
                + " (	`UrlPageId` INT(11) NOT NULL,"
                + "	`UrlPageClass` ENUM('DeftPage','UserDefClass','Wikipedia','Help','WikiProject','Template','File','MediaWiki','Category','Portal','Module','Draft','Topic') NOT NULL COLLATE 'utf8mb4_0900_as_cs',"
                + "	`UrlPageTitle` VARCHAR(100) NOT NULL COLLATE 'utf8mb4_0900_as_cs',"
                + "	`cnt` INT(11) NOT NULL DEFAULT '0'"
                + " )"
                + " COLLATE='utf8mb4_0900_ai_ci'"
                + " ENGINE=MyISAM AS"

                + "	SELECT UrlPageId,UrlPageClass, UrlPageTitle, COUNT(*) cnt"
                + $"	FROM {GDBNameMaker.GetPageLinks(ptc)} l "
                + "	GROUP BY l.UrlPageTitle;";
            sqlList_CreateTableData.Add(sqlStrPi);//GtWinApp.DbConn_public.ExecuteSqlStrAsVoid(sqlStrPi);


            //sqlList_CreateIndex = new List<string>();

            string sqlStrPoIdx = $"ALTER TABLE `{GDBNameMaker.GetPageLinks_StatisticOut(ptc)}`"
                + "	ADD INDEX `PageId` (`PageId`),"
                + "	ADD INDEX `PageTitle` (`PageTitle`),"
                + "	ADD INDEX `cnt` (`cnt`);";
            sqlList_CreateIndex.Add(sqlStrPoIdx);// GtWinApp.DbConn_public.ExecuteSqlStrAsVoid(sqlStrPoIdx);

            string sqlStrPiIdx = $"ALTER TABLE `{GDBNameMaker.GetPageLinks_StatisticIn(ptc)}`"
                + "	ADD INDEX `UrlPageId` (`UrlPageId`),"
                + "	ADD INDEX `UrlPageClass` (`UrlPageClass`),"
                + "	ADD INDEX `UrlPageTitle` (`UrlPageTitle`),"
                + "	ADD INDEX `cnt` (`cnt`);";
            sqlList_CreateIndex.Add(sqlStrPiIdx);//GtWinApp.DbConn_public.ExecuteSqlStrAsVoid(sqlStrPiIdx);
        }//GeneratePageLinkStatistic_TableDataIndex()

        ///// <summary>
        ///// wiki page title中存有冒号分割的分类。但实际上有大量 只有一个元素的分类。这样的分类是没有意义的。需要将他们转移到普通分类中 
        ///// </summary>
        //public void MoveSingleSubtitleItemIntoNormal()
        //{
        //    Dictionary<string, List<CPageSummary>> tpDict = new();
        //    foreach (var crrPair in this.PageSmyDict.Where(a => a.Value.Count > 1))
        //        tpDict.Add(crrPair.Key, crrPair.Value);

        //    foreach ( var crrPair in this.PageSmyDict.Where(a => a.Value.Count == 1))
        //        tpDict[string.Empty].Add(crrPair.Value.First());

        //    this.PageSmyDict = tpDict;
        //}//void MoveSingleSubtitleItemIntoNormal()

        public void ParsePageToJson(CPageLines _pageLines)
        {

            string _pageXmlStr = _pageLines.GetAllLinesAsOneString();
            CWikiPage crrPage = new CWikiPage();
            crrPage.ParseXml_Chinese(_pageXmlStr);

            //bool bSikp = 
            if (this.IsSikpThisPage(crrPage))
                return;

            var serializer = new JavaScriptSerializer();
            var serializedResult = serializer.Serialize(crrPage);
            //List<string> lines = new List<string>();

            lock(this.Out_wikiPages)
                Out_wikiPages.Add(serializedResult);
            //string txt = 


        }//ParsePageToList(CPageLines _pageLines)

    }//class CWikiPackage_Pages_TesterShao
}
