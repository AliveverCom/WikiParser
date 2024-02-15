using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using KnowedgeBox.WikiReader;

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
            GtWinApp.DbConn_public.ExecuteSqlStrAsVoid("show tables like 'bady_aaa';");
            GtWinApp.DbConn_public.CreateTable(typeof(CPageSummary),
                null, "bady", "_aaa".ToString(),
                ShaoChenYe.DevFramework.MySqlLib.EWhenExist.ThrowException, false);


            ////// 分页解析源文件
            Console.WriteLine("Hello World!");
            string srcFileUrl = @"D:\AiSource\Wiki-Cn\zhwiki-20210720-pages-articles-multistream.xml";
            //string _outputDirRoot = null;
            //int _nPagesPreFile = 1000 

            //将所有的BigPages写入指定文件(srcFileUrl);

            CWikiPackage_Pages_Parser packageTester = new CWikiPackage_Pages_Parser();
            DateTime bTime = DateTime.Now;
            CWikiPackage_Pages_Xml.ScanBigPackage(srcFileUrl, packageTester.ParseOnePage_xml);

            var duration = DateTime.Now - bTime;
            double seconds = duration.TotalSeconds;
            //psList.PageSmyList = psList.PageSmyList.OrderByDescending(a => a.TextLength).ToList();
            //packageTester.MoveSingleSubtitleItemIntoNormal();

            //Dictionary<string, List<CPageSummary>> wikiDefSubtitle = new();

            //foreach (var crrPair in packageTester.PageSmyDict.Where(a => a.Value.Count > 10))
            //     wikiDefSubtitle.Add(crrPair.Key, crrPair.Value);


            //核对 category 和 normal pages 之间的集合关系
            List<string> dftPageTitles = new List<string>();
            List<string> categoryTitles = new List<string>();

            packageTester.PageSmyDict[EPageTitleClass.DeftPage].ForEach(a => dftPageTitles.Add(a.TitleStr));
            packageTester.PageSmyDict[EPageTitleClass.Category].ForEach(a => dftPageTitles.Add(a.TitleStr));

            var lonelyCategories = categoryTitles.Except(dftPageTitles);

            foreach (var pageClassE in packageTester.PageSmyDict.Keys)
            {
                GtWinApp.DbConn_public.CreateTable(typeof(CPageSummary),
                        null,null , pageClassE.ToString(),
                        ShaoChenYe.DevFramework.MySqlLib.EWhenExist.ThrowException, false);
            }


            packageTester.PageSmyDict.Clear();

        }//Btn_解析XML2DB_Click()

        private static void 将所有的BigPages写入指定文件(string srcFileUrl)
        {
            string bigPageFileUrl = @"D:\AiSource\Wiki-Cn\BigPages-20210720.xml";
            CWikiPackage_Pages_SaveBigPagesInto1File bigPageWiter = new CWikiPackage_Pages_SaveBigPagesInto1File(bigPageFileUrl);
            CWikiPackage_Pages_Xml.ScanBigPackage(srcFileUrl, bigPageWiter.CheckAndSaveOnePage_xml);
        }



    }//class CMain_Dlg
}//namespace
