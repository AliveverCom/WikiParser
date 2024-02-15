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
    [AttDbTable("wiki_pages", EngineType = AttDbTable.EEngine.MyISAM)]
    public class CPageSummary
    {
        [AttDbColumn("PageId", HasDefaultIndex = true, IsNotNull = false,
            Comments = "", AutoFill = AttDbColumn.EAutoFill.NoAutoFill)]
        public int PageId;

        [AttDbColumn("TitleStr", HasDefaultIndex = true, IsNotNull = false,
           Comments = "", AutoFill = AttDbColumn.EAutoFill.NullItem, MaxLength = 64)]
        public string TitleStr { get; set; }

        [AttDbColumn("TextLength", HasDefaultIndex = true, IsNotNull = false,
            Comments = "", AutoFill = AttDbColumn.EAutoFill.Zero)]
        public int TextLength;

        //public long RevId;

        [AttDbColumn("RevTime", HasDefaultIndex = true, IsNotNull = false,
            Comments = "PageRevision time", AutoFill = AttDbColumn.EAutoFill.Zero)]
        public DateTime RevTime;

        public CPageSummary(CWikiPage _page)
        {
            PageId = int.Parse(_page.Id);
            this.TitleStr = _page.Title;
            this.TextLength = _page.Revision.Text.Length;

            this.RevTime = _page.Revision.Timestamp;
        }//CPageSummary()

    }//class CPageSummary

}//namespace
