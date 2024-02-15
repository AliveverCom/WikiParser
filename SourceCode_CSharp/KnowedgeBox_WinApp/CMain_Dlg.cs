using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Speech.Synthesis;
using System.IO;
using ShaoChenYe.DevFramework.MySqlLib;
using KnowedgeBox.WikiReader;
using NLP_Segmentation.Lexical;

namespace KnowedgeBox.WinApp
{
    public partial class CMain_Dlg : Form
    {

        public CMain_Dlg()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Btn_解析XML2DB_Click(object sender, EventArgs e)
        {
            bool bWriteDB_Pages = false;
            bool bWriteDB_PageLink = true;
            CWikiPackage_Pages_Parser packageTester = new CWikiPackage_Pages_Parser();

            ////// 分页解析源文件
            Console.WriteLine("Hello World!");
            string srcFileUrl = @"D:\AiSource\Wiki-Cn\zhwiki-20210720-pages-articles-multistream.xml";
            //string _outputDirRoot = null;
            //int _nPagesPreFile = 1000 

            //将所有的BigPages写入指定文件(srcFileUrl);

            //DateTime bTime;
            TimeSpan duration_Pages = TimeSpan.Zero;
            duration_Pages = ParsePageSummarys(bWriteDB_Pages, packageTester, srcFileUrl);

            ///////////////////////
            TimeSpan duration_Linkes = ParseLinks(bWriteDB_PageLink, packageTester, srcFileUrl);
            //double seconds_Linkes = duration_Linkes.TotalSeconds;
            var synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();

            string finishStr = $"Finished.\n Pages: {duration_Pages} sec.\n Links: {duration_Linkes.TotalSeconds} sec.";
            synthesizer.Speak(finishStr);

            MessageBox.Show(finishStr);
        }//Btn_解析XML2DB_Click()

        private static TimeSpan ParseLinks(bool bWriteDB_PageLink, 
            CWikiPackage_Pages_Parser packageTester, string srcFileUrl )
        {
            DateTime bTime = DateTime.Now;

            //packageTester.PageSmyDict.Clear();
            if (!bWriteDB_PageLink)
            {
                CWikiPackage_Pages_Xml.ScanBigPackage(srcFileUrl,
                    packageTester.ParseOnePageText_PageLinks,
                    null,
                    null,
                    GtWinApp.Cfg_MaxMultiThread );
            }
            else
            {
                string tableName = "page_link_" + EPageTitleClass.DeftPage.ToString();
                GtWinApp.DbConn_public.CreateTable(typeof(CPageLink),
                    null, tableName, null,
                    ShaoChenYe.DevFramework.MySqlLib.EWhenExist.DeleteAndDoAgain, false);

                CWikiPackage_Pages_Xml.ScanBigPackage(srcFileUrl,
                    packageTester.ParseOnePageText_PageLinks,
                    packageTester.WriteToDB_WhenProcessing_PageLinks,
                    GtWinApp.DbConn_public, GtWinApp.Cfg_MaxMultiThread);

                GtWinApp.DbConn_public.CreateIndexies(typeof(CPageLink), tableName, false);

                string sqlStrUpdate = "UPDATE page_link_deftpage l "
                    + " INNER JOIN  {0} p "
                    + " ON l.UrlPageTitle = p.TitleStr"
                    + " SET l.UrlPageId = p.PageId "
                    + " where l.UrlPageClass = '{0}' ";

                sqlStrUpdate = string.Format(sqlStrUpdate, EPageTitleClass.Category);
                GtWinApp.DbConn_public.ExecuteSqlStrAsVoid(sqlStrUpdate);

                //sqlStrUpdate = "UPDATE page_link_deftpage l "
                //    + " INNER JOIN  wiki_pages_deftpage p "
                //    + " ON l.UrlPageTitle = p.TitleStr"
                //    + " SET l.UrlPageId = p.PageId"
                //    +" where l.UrlPageClass = 'DeftPage' ";

                sqlStrUpdate = string.Format(sqlStrUpdate, EPageTitleClass.DeftPage);
                GtWinApp.DbConn_public.ExecuteSqlStrAsVoid(sqlStrUpdate);

                sqlStrUpdate = "ALTER TABLE `page_link_deftpage`   ADD INDEX `UrlPageId` (`UrlPageId`); ";
                GtWinApp.DbConn_public.ExecuteSqlStrAsVoid(sqlStrUpdate);

                CSingleSqlExecuter _dbConn = GtWinApp.DbConn_public;

                List<string> sqlList_CreateTableData = new List<string>(), sqlList_CreateIndex = new List<string>();
                CWikiPackage_Pages_Parser.GeneratePageLinkStatistic_TableDataIndex(ref sqlList_CreateTableData, ref sqlList_CreateIndex);
                _dbConn.ExecuteSqlStrAsVoid_Parallel(sqlList_CreateTableData);
                _dbConn.ExecuteSqlStrAsVoid_Parallel(sqlList_CreateIndex);

            }//else 如果需要写数据库。

            return  DateTime.Now - bTime;
        }//ParseLinks()


        private static TimeSpan ParsePageSummarys(bool bWriteDB_Pages, CWikiPackage_Pages_Parser packageTester, string srcFileUrl)
        {
            DateTime bTime = DateTime.Now;

            CWikiPackage_Pages_Xml.ScanBigPackage(srcFileUrl, packageTester.ParseOnePageSummary_xml,null,null, GtWinApp.Cfg_MaxMultiThread);

            //psList.PageSmyList = psList.PageSmyList.OrderByDescending(a => a.TextLength).ToList();
            //packageTester.MoveSingleSubtitleItemIntoNormal();

            //Dictionary<string, List<CPageSummary>> wikiDefSubtitle = new();

            //foreach (var crrPair in packageTester.PageSmyDict.Where(a => a.Value.Count > 10))
            //     wikiDefSubtitle.Add(crrPair.Key, crrPair.Value);


            //核对 category 和 normal pages 之间的集合关系
            //List<string> dftPageTitles = new List<string>();
            //List<string> categoryTitles = new List<string>();

            //packageTester.Out_PageSmyDict[EPageTitleClass.DeftPage].ForEach(a => dftPageTitles.Add(a.TitleStr));
            //packageTester.Out_PageSmyDict[EPageTitleClass.Category].ForEach(a => dftPageTitles.Add(a.TitleStr));

            //var lonelyCategories = categoryTitles.Except(dftPageTitles);

            if (bWriteDB_Pages)
            {
                //GtWinApp.DbConn_public.ExecuteSqlStrAsVoid("DROP DATABASE `nb_wiki`");
                //GtWinApp.DbConn_public.ExecuteSqlStrAsVoid("CREATE DATABASE `nb_wiki` /*!40100 COLLATE 'utf8mb4_0900_ai_ci' */;");
                //GtWinApp.DbConn_public.ExecuteSqlStrAsVoid("use  `nb_wiki`;");

                foreach (var pageClassE in packageTester.Out_PageSmyDict.Keys)
                {
                    string tableName1 = GDBNameMaker.GetPageSummary(pageClassE) ;// "wiki_pages_" + pageClassE.ToString();
                    GtWinApp.DbConn_public.CreateTable(typeof(CPageSummary),
                            null, tableName1, null,
                            ShaoChenYe.DevFramework.MySqlLib.EWhenExist.DeleteAndDoAgain, false);

                    foreach (CPageSummary crrItem in packageTester.Out_PageSmyDict[pageClassE])
                        crrItem.TitleStr = CSqlHelper.EncodeString2SqlString_Insert(crrItem.TitleStr).Replace("\\\"", "_");

                    GtWinApp.DbConn_public.Insert2Db<CPageSummary>(packageTester.Out_PageSmyDict[pageClassE], tableName1, 1000);

                    GtWinApp.DbConn_public.CreateIndexies(typeof(CPageSummary), tableName1, false);

                }//foreach (var pageClassE in packageTester.PageSmyDict.Keys)
            }//if (bWriteDB)

            var duration_Pages = DateTime.Now - bTime;
            return duration_Pages;
        }

        private static void 将所有的BigPages写入指定文件(string srcFileUrl)
        {
            string bigPageFileUrl = @"D:\AiSource\Wiki-Cn\BigPages-20210720.xml";
            CWikiPackage_Pages_SaveBigPagesInto1File bigPageWiter = new CWikiPackage_Pages_SaveBigPagesInto1File(bigPageFileUrl);
            CWikiPackage_Pages_Xml.ScanBigPackage(srcFileUrl, bigPageWiter.CheckAndSaveOnePage_xml, null,null, GtWinApp.Cfg_MaxMultiThread);
        }

        private void Btn_语音播报测试_Click(object sender, EventArgs e)
        {

            var speaker = new SpeechSynthesizer();
            var voiceList = speaker.GetInstalledVoices();
            string v1 = voiceList.First().VoiceInfo.Name;
             speaker.SetOutputToDefaultAudioDevice();

           speaker.SelectVoice("Microsoft Zira Desktop");
            speaker.Rate = 3;
            speaker.Speak("Finished. Totally used 150 sec. .");

            speaker.SelectVoice("Microsoft Huihui Desktop");
            speaker.Speak("运行完毕.\n 页面耗时150秒.\n 连接耗时218秒.");
        }//Btn_语音播报测试_Click()

        private void Btn_解析Pages_Click(object sender, EventArgs e)
        {
            bool bWriteDB_Pages = true;
            CWikiPackage_Pages_Parser packageTester = new CWikiPackage_Pages_Parser();

            ////// 分页解析源文件
            Console.WriteLine("Hello World!");
            string srcFileUrl = @"D:\AiSource\Wiki-Cn\zhwiki-20210720-pages-articles-multistream.xml";

            //DateTime bTime;
            // TimeSpan duration_Pages = TimeSpan.Zero;
            int duration_Pages= (int)ParsePageSummarys(bWriteDB_Pages, packageTester, srcFileUrl).TotalSeconds;



            ///// 报告任务结束
            var synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();

            string finishStr = $"Finished.\n Pages: {duration_Pages} sec.";
            synthesizer.Speak(finishStr);

            MessageBox.Show(finishStr);

        }//Btn_解析Pages_Click(object sender, EventArgs e)

        private void Btn_解析Links_Click(object sender, EventArgs e)
        {
            bool bWriteDB_PageLink = true;
            CWikiPackage_Pages_Parser packageTester = new CWikiPackage_Pages_Parser();

            ////// 分页解析源文件
            Console.WriteLine("Hello World!");
            string srcFileUrl = @"D:\AiSource\Wiki-Cn\zhwiki-20210720-pages-articles-multistream.xml";
            //string _outputDirRoot = null;
            //int _nPagesPreFile = 1000 

            //将所有的BigPages写入指定文件(srcFileUrl);

            ///////////////////////
            int duration_Linkes = (int)ParseLinks(bWriteDB_PageLink, packageTester, srcFileUrl).TotalSeconds;
            //double seconds_Linkes = duration_Linkes.TotalSeconds;

            ///// 报告任务结束
            var synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();

            string finishStr = $"Finished.\n \n Links: {duration_Linkes} sec.";
            synthesizer.Speak(finishStr);

            MessageBox.Show(finishStr);

        }//Btn_解析Links_Click(object sender, EventArgs e)

        private void Btn_Temp_Click(object sender, EventArgs e)
        {
            string aa = GChineseHelper.JianTi2FanTi("蓝蓝的天上白云飘", GChineseHelper.Cfg_DeftEncoding );
            string bb = GChineseHelper.FanTi2JianTi("韓國電影列表", GChineseHelper.Cfg_DeftEncoding);

            string zz = aa + bb;
            zz += " ";
        }//Btn_Temp_Click(object sender, EventArgs e)

        private void btn_解析Charpter_Click(object sender, EventArgs e)
        {
            bool bWriteDB = true;
            DateTime bTime = DateTime.Now;

            CWikiPackage_Pages_Parser packageTester = new CWikiPackage_Pages_Parser();

            ////// 分页解析源文件
            //string bigPageFileUrl = @"D:\AiSource\Wiki-Cn\BigPages-20210720.xml";
             string bigPageFileUrl = @"D:\AiSource\Wiki-Cn\zhwiki-20210720-pages-articles-multistream.xml";
            //CWikiPackage_Pages_SaveBigPagesInto1File bigPageWiter = new CWikiPackage_Pages_SaveBigPagesInto1File(bigPageFileUrl);
            if (bWriteDB)
            {
                string tableName = GDBNameMaker.GetPageChapter_Table(EPageTitleClass.DeftPage);//"page_topic_" + EPageTitleClass.DeftPage.ToString();
                GtWinApp.DbConn_public.CreateTable(typeof(CPageChapter),
                    null, tableName, null,
                    ShaoChenYe.DevFramework.MySqlLib.EWhenExist.DeleteAndDoAgain, false);

                CWikiPackage_Pages_Xml.ScanBigPackage(bigPageFileUrl,
                    packageTester.ParseOnePageText_PageChapter,
                    packageTester.WriteToDB_WhenProcessing_PageChapter,
                    GtWinApp.DbConn_public, GtWinApp.Cfg_MaxMultiThread*2);


                //GtWinApp.DbConn_public.Insert2Db(packageTester.Out_PageTopic, tableName, 100);
                GtWinApp.DbConn_public.CreateIndexies(typeof(CPageChapter), tableName, false);
            }//if (bWriteDB)
            else
            {
                CWikiPackage_Pages_Xml.ScanBigPackage(bigPageFileUrl,
                    packageTester.ParseOnePageText_PageChapter,
                    null, null, GtWinApp.Cfg_MaxMultiThread);
            }

            ///// 报告任务结束

            int duration_Linkes = (int)(DateTime.Now - bTime).TotalSeconds;
            //ttssynthesizer.Voice = speaker.Voice;
            string finishStr = $"Finished.\n \nParsing topics: {duration_Linkes} seconds.";
            GSpeaker.Speak(finishStr);

            MessageBox.Show(finishStr);

        }//btn_解析Topic_Click(object sender, EventArgs e)

        private void Btn_解析All_Click(object sender, EventArgs e)
        {
            bool bWriteDB = true;
            DateTime bTime = DateTime.Now;

            bool bParsePages = false, 
                bWriteDB_Pages = false, 
                bWriteDB_CharpterLinks = false,
                bWriteDB_statistics = true;

            CWikiPackage_Pages_Parser packageTester = new CWikiPackage_Pages_Parser();

            //string bigPageFileUrl = @"D:\AiSource\Wiki-Cn\BigPages-20210720.xml";
            string bigPageFileUrl = @"D:\AiSource\KnowledgeBox\Wiki-Cn\zhwiki-20210720-pages-articles-multistream.xml";
            string dlgStr = string.Empty;

            if (bWriteDB && bParsePages)
            {
                ////// 询问是否执行
                dlgStr = "To parse all wiki again, it will delete database.\n You may lose all previous wiki data.\n Do you want to continue?\n" +
                    "Yes for delete and recreate. No for skip and continue. Cancel for quit";
                GSpeaker.Speak(dlgStr);
                var dlgRst = MessageBox.Show(dlgStr, "Warning", MessageBoxButtons.YesNoCancel);
                if (dlgRst == DialogResult.Cancel)
                    return;
                else if (dlgRst == DialogResult.Yes)
                {
                    ////// 删除数据库并重新创建
                    GtWinApp.DbConn_public.ExecuteSqlStrAsVoid("DROP DATABASE `nb_wiki`");
                    GtWinApp.DbConn_public.ExecuteSqlStrAsVoid("CREATE DATABASE `nb_wiki` /*!40100 COLLATE 'utf8mb4_0900_ai_ci' */;");
                    GtWinApp.DbConn_public.ExecuteSqlStrAsVoid("use  `nb_wiki`;");
                    dlgStr = $"Database is deleted and recreated.";
                    GSpeaker.Speak(dlgStr);
                }
                else
                {
                    dlgStr = "Skip deleting database and continue.";
                    GSpeaker.Speak(dlgStr);

                }
            }//if (bWriteDB)

            TimeSpan duration_Pages;
            int nAllChapters = 0;
            string stepStr1 = $"Parsing pages skiped.";
            string stepStr99 = $"Parsing wiki skiped.";

            ////// 分页解析源文件
            if (bParsePages)
            {
                dlgStr = $"Now, beging parsing wiki.";
                GSpeaker.Speak(dlgStr);

                packageTester.Out_Pages_MemIdx.Initial_Idx_PageTitle();

                CWikiPackage_Pages_Xml.Cfg_ReporterFun = GSpeaker.Speak;
                CWikiPackage_Pages_Xml.ScanBigPackage(bigPageFileUrl,
                    packageTester.ParseOnePage_All,
                    null, null, GtWinApp.Cfg_MaxMultiThread * 2);

                duration_Pages = DateTime.Now - bTime;
                stepStr1 = $"Parsing pages finished. {(int)duration_Pages.TotalSeconds} seconds.";
                GSpeaker.Speak(stepStr1);
                //bTime = DateTime.Now;
                ////// update topic id of links and topics.
                //List<CPageChapter> allChapters = new List<CPageChapter>();

                foreach (List<CPageChapter> crrList in packageTester.Out_PageChapter.Values)
                {
                    foreach (CPageChapter crrChapter in crrList)
                    {
                        //if (packageTester.Out_Pages_MemIdx.Idx_PageTitle[EPageTitleClass.DeftPage].ContainsKey(crrChapter.ChapterText))
                        CPageSummary ps = packageTester.Out_Pages_MemIdx.GetPageInDeftPage(crrChapter.ChapterText);
                        if (ps != null)
                        {
                            crrChapter.Chapter2PageId = ps.PageId;
                            ps.nChapterIn++;
                            packageTester.Out_Pages_MemIdx.Idx_PageTitle[EPageTitleClass.DeftPage][crrChapter.PageTitle].nChapterOut++;
                        }

                    }//foreach (CPageChapter crrChapter in crrList)

                    //allChapters.AddRange(crrList);
                    nAllChapters += crrList.Count;
                }// 
                 //packageTester.Out_PageChapter.Clear();

                foreach (CPageLink crrLink in packageTester.Out_PageLinks)
                {
                    CPageSummary ps = packageTester.Out_Pages_MemIdx.GetPageInDeftPage(crrLink.UrlPageTitle);
                    if (ps != null)
                    {
                        crrLink.UrlPageId = ps.PageId;
                        ps.nLinksIn++;
                        packageTester.Out_Pages_MemIdx.Idx_PageTitle[EPageTitleClass.DeftPage][crrLink.PageTitle].nChapterOut++;
                    }
                    else
                    {
                        ps = packageTester.Out_Pages_MemIdx.GetPageInCategory(crrLink.UrlPageTitle);
                        if (ps != null)
                        {
                            crrLink.UrlPageId = ps.PageId;
                            crrLink.UrlPageClass = EPageTitleClass.Category;
                            ps.nLinksIn++;
                            packageTester.Out_Pages_MemIdx.Idx_PageTitle[EPageTitleClass.DeftPage][crrLink.PageTitle].nChapterOut++;
                        }
                    }
                }//foreach (CPageLink crrLink in packageTester.Out_PageLinks)
                duration_Pages = DateTime.Now - bTime;
                stepStr99 = $"Parsing wiki finished.  {(int)duration_Pages.TotalSeconds} seconds.";
                GSpeaker.Speak(stepStr99);

            }//if (bParsePages)

            ////////// writing all data to database
            string writeData, writeIndex;
            if (bWriteDB && bParsePages )
            {

                List<string> idxPages =  bWriteDB_Pages? new List<string> ():  packageTester.WriteToDB_AllData_Pages(GtWinApp.DbConn_public);
                List<string> idxCharpters = bWriteDB_CharpterLinks ? new List<string>() : packageTester.WriteToDB_AllData_Charpters(GtWinApp.DbConn_public);
                List<string> idxLinks = bWriteDB_CharpterLinks ? new List<string>() : packageTester.WriteToDB_AllData_Links(GtWinApp.DbConn_public);

                duration_Pages = DateTime.Now - bTime;
                writeData = $"Finished data to DB. {(int)duration_Pages.TotalSeconds} seconds.";
                GSpeaker.Speak(writeData);

                List<string> allIdxSql = new List<string>();
                allIdxSql.AddRange(idxPages);
                allIdxSql.AddRange(idxCharpters);
                allIdxSql.AddRange(idxLinks);

                GtWinApp.DbConn_public.ExecuteSqlStrAsVoid_Parallel(allIdxSql);

                duration_Pages = DateTime.Now - bTime;
                writeIndex = $"Finished data index. {(int)duration_Pages.TotalSeconds} seconds.";
                GSpeaker.Speak(writeIndex);
            }
            else
            {
                writeData = "No data writen.";
                writeIndex = "No data index writen.";

            }

            string rptStr_staTable, rptStr_staIndex;
            /////////////// generate statistic tables
            if (bWriteDB && bWriteDB_statistics)
            {
                /////LinkStatistic
                List<string> sqlList_CreateTableData = new List<string>(), sqlList_CreateIndex = new List<string>();
               //由于pages中已经添加LinksInOut字段，因此理论上Links就不需要再次统计了。 但由于debug发现有少量无法对应到pages，所以依然保留。
               CWikiPackage_Pages_Parser.GeneratePageLinkStatistic_TableDataIndex(ref sqlList_CreateTableData, ref sqlList_CreateIndex);

                /////ChapterStatistic
                CWikiPackage_Pages_Parser.GeneratePageChapterStatistic_TableDataIndex(ref sqlList_CreateTableData, ref sqlList_CreateIndex);

                ////// create Statistic tables and data
                GSpeaker.Speak("Start statistic tables. ");
                GtWinApp.DbConn_public.ExecuteSqlStrAsVoid_Parallel(sqlList_CreateTableData);
                duration_Pages = DateTime.Now - bTime;
                rptStr_staTable = $"Finished statistic tables. {(int)duration_Pages.TotalSeconds} seconds.";
                GSpeaker.Speak(rptStr_staTable);


                ////// create Statistic index
                GSpeaker.Speak("Start statistic index. ");
                GtWinApp.DbConn_public.ExecuteSqlStrAsVoid_Parallel(sqlList_CreateIndex);
                duration_Pages = DateTime.Now - bTime;
                rptStr_staIndex = $"Finished statistic index. {(int)duration_Pages.TotalSeconds} seconds.";
                GSpeaker.Speak(rptStr_staTable);

            }
            else
            {
                rptStr_staTable = $"No statistic tables writen.";
                rptStr_staTable = $"No statistic index writen.";
            }

            /////////////// finial report
            string staticStr = string.Empty;
            
            if (bParsePages)
            {
                staticStr = $"{packageTester.Out_Pages_MemIdx.Idx_PageTitle[EPageTitleClass.DeftPage].Count} default pages, " +
                $"{nAllChapters} Chapters, {packageTester.Out_PageLinks.Count} Links. ";
                    GSpeaker.Speak(staticStr);
            }

            dlgStr = stepStr1 + "\n" + stepStr99 + "\n"
                + writeData + "\n" + writeIndex + "\n"
                + rptStr_staTable + "\n" + rptStr_staTable + "\n"
                + staticStr + "\n";
            MessageBox.Show(dlgStr);

        }//Btn_解析All_Click

        private void button1_Click(object sender, EventArgs e)
        {
            
            //bool bWriteDB_Pages = true;
            CWikiPackage_Pages_Parser packageTester = new CWikiPackage_Pages_Parser();

            ////// 分页解析源文件
            Console.WriteLine("Hello World!");
            string srcFileUrl = @"D:\AiSource\KnowledgeBox\Wiki-Cn\zhwiki-20210720-pages-articles-multistream.xml";
            string outFilename = @"D:\AiSource\KnowledgeBox\Wiki-Cn\zhwiki-20210720-pages-articles-multistream.json";

            //DateTime bTime;
            // TimeSpan duration_Pages = TimeSpan.Zero;
            CWikiPackage_Pages_Xml.ScanBigPackage(srcFileUrl, packageTester.ParsePageToJson, null, null, GtWinApp.Cfg_MaxMultiThread);

            int duration_Pages = (int)CWikiPackage_Pages_Xml.ScanBigPackage(srcFileUrl, packageTester.ParsePageToJson, null, null, GtWinApp.Cfg_MaxMultiThread); ;

            File.AppendAllLines(outFilename, packageTester.Out_wikiPages, GChineseHelper.Cfg_DeftEncoding);

            ///// 报告任务结束
            var synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();

            string finishStr = $"Finished.\n Pages: {duration_Pages} sec.";
            synthesizer.Speak(finishStr);

            MessageBox.Show(finishStr);


        }

        private void Btn_从WIKI中提取所有单词并计数_Click(object sender, EventArgs e)
        {
            bool bWriteDB_Pages = false;
            int nMaxCnWordLength = 4;
            string srcFileUrl = @"E:\AiSource\KnowledgeBox\Wiki-Cn\zhwiki-20210720-pages-articles-multistream.xml";
            string outDirRoot = @"E:\Temp\WikiParssing";
            //string srcFileUrl = @"E:\AiSource\KnowledgeBox\Wiki-Cn\BigPages-20210720.xml";

            CNLP_ArticleWordCuting awCutter = new CNLP_ArticleWordCuting(nMaxCnWordLength)
            {
                Cfg_AutoSaveRootDir = outDirRoot,
                //Cfg_MaxWordLength = 1000,

            };

            CWikiPackage_Pages_Parser packageTester = new CWikiPackage_Pages_Parser();
            packageTester.ExtFun_ParseOnePage_For_xml = awCutter.ExtFun_ParseOnePage_xml;

            ////// 分页解析源文件
            Console.WriteLine("Hello World!");

            //DateTime bTime;
            // TimeSpan duration_Pages = TimeSpan.Zero;
            int duration_Pages = (int)ParsePage2Words(bWriteDB_Pages, packageTester, srcFileUrl).TotalSeconds;
            //CWikiPackage_Pages_Xml.ScanBigPackage(srcFileUrl, , null, null, GtWinApp.Cfg_MaxMultiThread);

            ///// 报告任务结束
            var synthesizer = new SpeechSynthesizer();
            synthesizer.SetOutputToDefaultAudioDevice();

            string finishStr = $"Finished.\n Pages: {duration_Pages} seconds.";
            synthesizer.Speak(finishStr);
            MessageBox.Show(finishStr);

            MessageBox.Show(finishStr);

        }//Btn_从WIKI中提取所有单词并计数_Click(object sender, EventArgs e)

        private static TimeSpan ParsePage2Words(bool bWriteDB_Pages, 
            CWikiPackage_Pages_Parser packageTester, 
            string srcFileUrl)
        {
            DateTime bTime = DateTime.Now;

            CWikiPackage_Pages_Xml.ScanBigPackage(srcFileUrl,
                packageTester.ParseOnePage_xml_ExtFun,
                null, null,
                1); // GtWinApp.Cfg_MaxMultiThread);

            if (bWriteDB_Pages)
            {

            }//if (bWriteDB)

            var duration_Pages = DateTime.Now - bTime;
            return duration_Pages;
        }

    }//class CMain_Dlg
}//namespace
