using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowedgeBox.WikiReader
{
    /// <summary>
    /// 这是一个用于实验的类。2021年8月，第一次解析wiki内容时候的实验。
    /// 
    /// </summary>
    public class CWikiPackage_Pages_Parser
    {

        public Dictionary<EPageTitleClass, List<CPageSummary>> PageSmyDict = new();

        //public Dictionary<string, Int32>  TitleWithMaoHao = new ();

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
            //{[Topic, Count = 35861]}
            foreach (EPageTitleClass crrE in Enum.GetValues(typeof(EPageTitleClass)))
                 PageSmyDict.Add(crrE, new List<CPageSummary>());


        }//CWikiPackage_Pages_TesterShao()

        public void ParseOnePage_xml(string _pageXmlStr)
        {
            CWikiPage crrPage = new CWikiPage();
            crrPage.ParseXml(_pageXmlStr);

            CPageSummary ps = new CPageSummary(crrPage);

            //int nMaoHao = ps.Title.IndexOf(':');
            //if (nMaoHao > -1)
            //{
            //    string subTitile = ps.Title.Split(':').First();
            //    if (!this.PageSmyDict.ContainsKey(subTitile))
            //        PageSmyDict.Add(subTitile, new List<CPageSummary>());
            //    PageSmyDict[subTitile].Add(ps);

            //}
            //else
            //{
            //    PageSmyDict[string.Empty].Add(ps);

            //}

            //if ( ! this.PageSmyDict.ContainsKey(crrPage.TitleClass))
            //    this.PageSmyDict.Add(crrPage.TitleClass,new List<CPageSummary>());

            lock (PageSmyDict[crrPage.TitleClass])
            {
                PageSmyDict[crrPage.TitleClass].Add(ps);
            }




            //this.PageSmyList.Add(ps);
        }//ParseOnePage_xml(string _pageXmlStr)

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

    }//CWikiPackage_Pages_TesterShao
}
