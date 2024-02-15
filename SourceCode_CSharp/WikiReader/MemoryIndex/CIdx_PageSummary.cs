using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace KnowedgeBox.WikiReader.MemoryIndex
{
    public class CIdx_PageSummary
    {
        public Dictionary<EPageTitleClass, Dictionary<string, CPageSummary>> Idx_PageTitle ;

        public void Initial_Idx_PageTitle()
        {
            this.Idx_PageTitle = new Dictionary<EPageTitleClass, Dictionary<string, CPageSummary>>();

            foreach (EPageTitleClass crrE in Enum.GetValues(typeof(EPageTitleClass)))
                Idx_PageTitle.Add(crrE, new  Dictionary<string, CPageSummary>());
        }

        public List<CPageSummary> GetPageList(EPageTitleClass _pageClass)
        {
            ////List<CPageSummary> rst = new List<CPageSummary>();

            //foreach(List<CPageSummary> crrSubList in this.Idx_PageTitle[_pageClass])
            //{
            //    rst.AddRange(crrSubList);
            //}

            //return rst;
            return this.Idx_PageTitle[_pageClass].Values.ToList();
        }//GetPageList(EPageTitleClass _pageClass)

        public void AddPageSummary(EPageTitleClass _pageClass, CPageSummary _pageSummary)
        {
            if (_pageSummary == null)
                throw new Exception("Error: _pageSummary == null");

            if (this.Idx_PageTitle != null)
            {
                lock (this.Idx_PageTitle[_pageClass])
                {
                    if (this.Idx_PageTitle[_pageClass].ContainsKey(_pageSummary.TitleStr))
                        return;

                    this.Idx_PageTitle[_pageClass].Add(_pageSummary.TitleStr, _pageSummary);
                }
            }
            else
                throw new Exception("No index can be added.");


        }//AddPageSummary(CPageSummary _pageSummary)

        public IEnumerable<CPageSummary> GetPages(EPageTitleClass _pageClass)
        {
            return Idx_PageTitle[_pageClass].Values;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CPageSummary GetPageInDeftPage(string _pageTitle)
        {
            if (this.Idx_PageTitle[EPageTitleClass.DeftPage].ContainsKey(_pageTitle))
                return this.Idx_PageTitle[EPageTitleClass.DeftPage][_pageTitle];
            else
                return null;

        }// GetPageInDeftPage(string _pageTitle)

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CPageSummary GetPageInCategory(string _pageTitle)
        {
            CPageSummary rst = null;
            this.Idx_PageTitle[EPageTitleClass.Category].TryGetValue(_pageTitle,out rst);
            return rst;

            //if (this.Idx_PageTitle[EPageTitleClass.Category]ContainsKey(_pageTitle))
            //    return this.Idx_PageTitle[EPageTitleClass.DeftPage][_pageTitle];
            //else
            //    return null;

        }// GetPageInDeftPage(string _pageTitle)

    }// class CIdx_PageSummary
}
