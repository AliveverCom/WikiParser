using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShaoChenYe.DevFramework.MySqlLib;

namespace KnowedgeBox.WikiReader
{
    /// <summary>
    /// 一个页面内会有多个层级的topic ，比如一个城市的 历史、文化、经济里面还会再细分。
    /// 他们的特征是 单独成一行 且以 =开始=结束
    /// </summary>
    [AttDbTable("pages_chapter", EngineType = AttDbTable.EEngine.MyISAM, TextEncoding = "utf8mb4_0900_as_cs")]
    public class CPageChapter
    {
        [AttDbColumn("ChapterId", HasDefaultIndex = true, IsNotNull = true,
            Comments = "", AutoFill = AttDbColumn.EAutoFill.NoAutoFill, IsPrimaryKey = true)]
        public int ChapterId;

        /// <summary>
        /// 改Chapter 所在的Page
        /// </summary>
        [AttDbColumn("PageId", HasDefaultIndex = true, IsNotNull = true,
            Comments = "", AutoFill = AttDbColumn.EAutoFill.NoAutoFill )]
        public int PageId;

        [AttDbColumn("PageTitle", HasDefaultIndex = true, IsNotNull = true,
            Comments = "", AutoFill = AttDbColumn.EAutoFill.NoAutoFill, MaxLength = 100)]
        public string PageTitle;



        [AttDbColumn("ChapterText", HasDefaultIndex = true, IsNotNull = true,
    Comments = "", AutoFill = AttDbColumn.EAutoFill.NoAutoFill, MaxLength = 100)]
        public string ChapterText;



        //[AttDbColumn("ChapterText", HasDefaultIndex = true, IsNotNull = true,
        //    Comments = "", AutoFill = AttDbColumn.EAutoFill.NoAutoFill,  MaxLength = 100)]
        //public string ChapterText;

        /// <summary>
        /// 当前话题所处的标题深度。用于日后分析，以及计算过程中寻找上级节点。
        /// </summary>
        [AttDbColumn("ChapterLevel", HasDefaultIndex = true, IsNotNull = true,
            Comments = "", AutoFill = AttDbColumn.EAutoFill.NoAutoFill )]
        public short ChapterLevel;

        /// <summary>
        /// 该Chapter 所属的本页面内上层 topic
        /// </summary>
        [AttDbColumn("ParentChapterId", HasDefaultIndex = true, IsNotNull = true,
            Comments = "", AutoFill = AttDbColumn.EAutoFill.NoAutoFill)]
        public int ParentChapterId;

        public CPageChapter ParentChapter_MemoryOnly = null;


        /// <summary>
        /// 如果一个Tpoic 的 text 可以映射到某个 deftPage上的话，就显示那个Page的ID
        /// </summary>
        [AttDbColumn("ChapterPageId", HasDefaultIndex = true, IsNotNull = true,
            Comments = "", AutoFill = AttDbColumn.EAutoFill.NoAutoFill)]
        public int Chapter2PageId = -1;


    }//public class PageChapter
}//namespace
