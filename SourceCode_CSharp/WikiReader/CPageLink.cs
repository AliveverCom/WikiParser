using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShaoChenYe.DevFramework.MySqlLib;

namespace KnowedgeBox.WikiReader
{
    /// <summary>
    /// wiki page中的 交叉连接
    /// </summary>
    [AttDbTable("page_link", EngineType = AttDbTable.EEngine.MyISAM, TextEncoding = "utf8mb4_0900_as_cs")]
    public class CPageLink
    {
        [AttDbColumn("PageId", HasDefaultIndex = true, IsNotNull = true,
            Comments = "", AutoFill =  AttDbColumn.EAutoFill.NoAutoFill , IsPrimaryKey = false)]
        public int PageId;

        [AttDbColumn("PageTitle", HasDefaultIndex = false, IsNotNull = false,
            Comments = "", AutoFill = AttDbColumn.EAutoFill.NoAutoFill, MaxLength = 100)]
        public string PageTitle;

        //public EPageTitleClass MainPageClass = EPageTitleClass.DeftPage;

        [AttDbColumn("Chapterid", HasDefaultIndex = true, IsNotNull = true,
    Comments = "", AutoFill = AttDbColumn.EAutoFill.NoAutoFill, IsPrimaryKey = false)]
        public int ChapterId = -1;

        /// <summary>
        /// 主页面中的话题ID
        /// </summary>
        [AttDbColumn("ChapterStr", HasDefaultIndex = false, IsNotNull = false,
            Comments = "", AutoFill = AttDbColumn.EAutoFill.NoAutoFill, MaxLength = 100)]
        public string ChapterStr;

        [AttDbColumn("UrlPageId", HasDefaultIndex = true, IsNotNull = true,
            Comments = "", AutoFill = AttDbColumn.EAutoFill.NoAutoFill, IsPrimaryKey = false)]
        public int UrlPageId = -1;

        /// <summary>
        /// 被连接的页面id
        /// </summary>
        [AttDbColumn("UrlPageTitle", HasDefaultIndex = true, IsNotNull = true,
            Comments = "", AutoFill = AttDbColumn.EAutoFill.NoAutoFill, MaxLength = 100)]
        public string UrlPageTitle;


        [AttDbColumn("UrlPageClass", HasDefaultIndex = true, IsNotNull = true,
            Comments = "", AutoFill = AttDbColumn.EAutoFill.NoAutoFill)]
        public EPageTitleClass UrlPageClass = EPageTitleClass.DeftPage;

        [AttDbColumn("DisplayText", HasDefaultIndex = false, IsNotNull = false,
           Comments = "null means (DisplayText == UrlPageTitle)", AutoFill = AttDbColumn.EAutoFill.NoAutoFill, MaxLength = 100)]
        public string DisplayText;// DisplayText;

    }//class CPageLink
}
