using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowedgeBox.WikiReader
{
    /// <summary>
    /// 负责生成所有数据库所需的 标准化名称
    /// </summary>
    public class GDBNameMaker
    {
        public static string GetPageChapter_Table(EPageTitleClass _eCls)
        {
            return "PageChapter_" + _eCls;
        }

        //public static string GetPageChapter_Sta_GrpByPage(EPageTitleClass _eCls)
        //{
        //    //return "PageChapter_sta_out" ;
        //    return $"sta_PageChapter_{_eCls}_GrpByChapter";

        //}

        public static string GetPageChapter_Sta_GrpByChapter(EPageTitleClass _eCls)
        {
            return $"sta_PageChapter_{_eCls}_GrpByChapter";
        }

        public static string GetPageSummary(EPageTitleClass _eCls)
        {
            return "Pages_" + _eCls;
        }

        public static string GetPageLinks(EPageTitleClass _eCls)
        {
            return "PageLink_" + _eCls.ToString();
        }

        public static string GetPageLinks_StatisticOut(EPageTitleClass _eCls)
        {
            return $"sta_PageLink_{_eCls}_out";
        }

        public static string GetPageLinks_StatisticIn(EPageTitleClass _eCls)
        {
            return $"sta_PageLink_{_eCls}_in";
        }

    }//class GDBNameMaker
}
