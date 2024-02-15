using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ShaoChenYe.DevFramework.MySqlLib;

namespace KnowedgeBox.WikiReader
{
    /// <summary>
    /// 由于内存太小，无法缓存所有wiki内容，因此使用这个结构提取出每个page的基本信息进行预览
    /// </summary>
    [AttDbTable("wiki_pages", EngineType = AttDbTable.EEngine.MyISAM, TextEncoding = "utf8mb4_0900_as_cs")]
    public class CPageSummary
    {
        [AttDbColumn("PageId", HasDefaultIndex = true, IsNotNull = true,
            Comments = "", AutoFill = AttDbColumn.EAutoFill.NoAutoFill, IsPrimaryKey = true)]
        public int PageId;

        [AttDbColumn("TitleStr", HasDefaultIndex = true, IsNotNull = true,
           Comments = "", AutoFill = AttDbColumn.EAutoFill.NoAutoFill, MaxLength = 100)]
        public string TitleStr { get; set; }

        [AttDbColumn("TextLength", HasDefaultIndex = true, IsNotNull = true,
            Comments = "Only a reference length parsed in memory,not realy file length", AutoFill = AttDbColumn.EAutoFill.NoAutoFill)]
        public int TextLength;

        [AttDbColumn("nBeginLine", HasDefaultIndex = true, IsNotNull = true,
            Comments = "The begin line number in xml file.", AutoFill = AttDbColumn.EAutoFill.NoAutoFill)]
        public int nBeginLine;

        [AttDbColumn("nLines", HasDefaultIndex = true, IsNotNull = true,
            Comments = "It indicates how many lines in xml file", AutoFill = AttDbColumn.EAutoFill.NoAutoFill)]
        public int nLines;

        [AttDbColumn("nLinksOut", HasDefaultIndex = true, IsNotNull = true,
            Comments = "Links in current page point to other pages", AutoFill = AttDbColumn.EAutoFill.NoAutoFill)]
        public int nLinksOut;

        [AttDbColumn("nLinksIn", HasDefaultIndex = true, IsNotNull = true,
            Comments = "Links from other pages point to current page.", AutoFill = AttDbColumn.EAutoFill.NoAutoFill)]
        public int nLinksIn;

        [AttDbColumn("nChapterOut", HasDefaultIndex = true, IsNotNull = true,
            Comments = "Links in current page point to other pages", AutoFill = AttDbColumn.EAutoFill.NoAutoFill)]
        public int nChapterOut;

        [AttDbColumn("nChapterIn", HasDefaultIndex = true, IsNotNull = true,
            Comments = "Links from other pages point to current page.", AutoFill = AttDbColumn.EAutoFill.NoAutoFill)]
        public int nChapterIn;



        //public long RevId;

        [AttDbColumn("RevTime", HasDefaultIndex = true, IsNotNull = true,
            Comments = "PageRevision time", AutoFill = AttDbColumn.EAutoFill.NoAutoFill)]
        public DateTime RevTime;

        public CPageSummary(CWikiPage _page)
        {
            PageId = _page.Id;
            this.TitleStr = _page.Title;
            this.TextLength = _page.Revision.Text.Length;

            this.RevTime = _page.Revision.Timestamp;
        }//CPageSummary()

    }//class CPageSummary

}//namespace
