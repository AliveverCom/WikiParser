using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

using NLP_Segmentation.Lexical;
using KnowedgeBox.WikiReader;
using NLP_Segmentation;


namespace KnowedgeBox.WinApp
{
    public class CNLP_ArticleWordCuting
    {

        public CArticleWordsManager Out_WordManager = new CArticleWordsManager();

        /// <summary>
        /// = 13; 中华人民共和国中央人民政府
        /// </summary>
        public int Cfg_MaxWordLength;//

        /// <summary>
        /// 每处理多少个页面就删除所有 仅出现1次的单词。以免内存崩溃。
        /// </summary>
        public int Cfg_RemoveOnetimeWords_EveryPages =20*10000;//

        /// <summary>
        /// 如果某个词出现的总频次小于等于这个数字，则会被删除。
        /// 注意，这个数值需要和 Cfg_RemoveOnetimeWords_EveryPages配合使用。
        /// </summary>
        public int Cfg_RemmoveMinWordLength = 10;

        /// <summary>
        /// 如果解析的过程中需要进行分段自动存储。则必须指定这个目录；
        /// </summary>
        public string Cfg_AutoSaveRootDir;

        public List<string> Out_AutoSaveFiles_Words;
        public List<string> Out_AutoSaveFiles_Article2Words;

        private CWordsCutter Proc_Cutter;

        private int PagesCount = 0;

        private static readonly object Proc_MultiThreadLocker = new object();

        private CTimeCounter CutterTimer = new CTimeCounter(true) { Name= "CutterTimer" };


        public CNLP_ArticleWordCuting( int _MaxWordLength )
        {
            this.Cfg_MaxWordLength = _MaxWordLength;
            Out_WordManager.SetMaxWordLength(_MaxWordLength);
        }//CNLP_ArticleWordCuting(CWikiPage _page)

        public void ExtFun_ParseOnePage_xml(CWikiPage _page)
        {
            if (!(_page.TitleClass == EPageTitleClass.DeftPage || _page.TitleClass == EPageTitleClass.Category))
                return;

            Interlocked.Increment(ref  PagesCount);

            if (PagesCount % 10000 == 0)
                Trace.WriteLine($"PagesCount = {PagesCount}; {CutterTimer.GetSpentSeconds_StrWithName()}");

            if (PagesCount >= this.Cfg_RemoveOnetimeWords_EveryPages)
            {
                lock (Proc_MultiThreadLocker)
                {
                    CTimeCounter tp_timer = new CTimeCounter(true);

                    if (PagesCount >= this.Cfg_RemoveOnetimeWords_EveryPages)
                    {
                        tp_timer.SetStart($"RemoveAllOnceWords(). MinWordLength = {Cfg_RemmoveMinWordLength}");
                        int nRemovedShortWords = this.Proc_Cutter.InOut_WordMgr.RemoveAllOnceWords(this.Cfg_RemmoveMinWordLength);

                        string debugStr1 = $"{this.Proc_Cutter.InOut_WordMgr.GetNextWordId()} words_id;" +
                            $" {this.Proc_Cutter.InOut_WordMgr.CountTotalWords()} words;" +
                            $" {this.Proc_Cutter.InOut_WordMgr.CountTotalWords() / (double)this.Proc_Cutter.InOut_WordMgr.GetNextWordId()} left;" +
                            $" nRemovedShortWords = {nRemovedShortWords}; {tp_timer.GetSpentSeconds_StrWithName()}";
                        Trace.WriteLine(debugStr1);

                        tp_timer.SetStart($"RemoveAllInnerLetterWords(). ");
                        int nRemovedInnerLetterWords = this.Proc_Cutter.InOut_WordMgr.RemoveAllInnerLetterWords();

                        string debugStr2 = $"{this.Proc_Cutter.InOut_WordMgr.GetNextWordId()} words_id;" +
                            $" {this.Proc_Cutter.InOut_WordMgr.CountTotalWords()} words;" +
                            $" {this.Proc_Cutter.InOut_WordMgr.CountTotalWords() / (double)this.Proc_Cutter.InOut_WordMgr.GetNextWordId()} left;" +
                            $" nRemovedInnerLetterWords = {nRemovedInnerLetterWords}; {tp_timer.GetSpentSeconds_StrWithName()}";
                        Trace.WriteLine(debugStr2);

                        string wordsFile, article2WordFile;

                        tp_timer.SetStart($"RemoveAllInnerLetterWords(). ");
                        //this.Proc_Cutter.InOut_WordMgr.SaveToFiles(this.Cfg_AutoSaveRootDir,true, true, out wordsFile, out article2WordFile);

                        wordsFile = this.Proc_Cutter.InOut_WordMgr.SaveWords2File(this.Cfg_AutoSaveRootDir, true);

                        ////// Save Art2words
                        article2WordFile = this.Proc_Cutter.InOut_WordMgr.SaveIndexies2File(this.Cfg_AutoSaveRootDir, true);

                        Trace.WriteLine($"{wordsFile};{article2WordFile};{tp_timer.GetSpentSeconds_StrWithName()}");

                        PagesCount = 0;
                    }
                }// lock (Proc_MultiThreadLocker)
            }//if (PagesCount >= this.Cfg_RemoveOnetimeWords_EveryPages)



            if (_page.Revision.Text.Length == 0)
                return;

            if ( this.Proc_Cutter == null)
                    this.Proc_Cutter = new CWordsCutter(_page.Id, _page.Revision.Text, this.Out_WordManager, Cfg_MaxWordLength);
            else
            {
                this.Proc_Cutter.In_ArticleId = _page.Id;
                this.Proc_Cutter.In_ArticleText = _page.Revision.Text;
                this.Proc_Cutter.InOut_WordMgr.AddWordItemFromArticle(
                    _page.Title, _page.Id, true);
            }

            this.Proc_Cutter.CuttingWords();

            ////// Release all
            this.Proc_Cutter.In_ArticleText = null;
        }//ExtFun_ParseOnePage_xml(CWikiPage _crrPage)

    }//CNLP_ArticleWordCuting
}
