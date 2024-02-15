using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace NLP_Segmentation.Lexical
{
    /// <summary>
    /// this class doesn'manage articles. It only manage of articles. 
    /// </summary>
    public class CArticleWordsManager
    {
        #region public members
        /// <summary>
        /// a List that saved all words. using it's index number to determean the length of the word;
        /// Words[4] means all words that length = 4, such as 'Hello', 'Jack' 
        /// </summary>
        public Dictionary<int, Dictionary<string, CWordItem>> Words = new  Dictionary<int, Dictionary<string, CWordItem>>();

        /// <summary>
        /// a index that from article to words. Dictionary[int articleId, Dictionary[string Word, CArticle2Word]]
        /// </summary>
        public Dictionary<int, Dictionary<string, CArticle2Word>> Article2Words= new Dictionary<int, Dictionary<string, CArticle2Word>>();

        /// <summary>
        /// a index that from words to article. 
        /// Dictionary[int articleId, Dictionary[string Word, CArticle2Word]]
        /// </summary>
        public Dictionary<string, Dictionary<int, CArticle2Word>> Words2Article = new Dictionary<string, Dictionary<int, CArticle2Word>>();

        public bool Cfg_IsUse_Article2Words = false;

        public bool Cfg_IsUse_Words2Article = true;

        #endregion //public members

        #region protected members

        protected int Cfg_MaxWordLength ;

        protected int LastWordId = 0;
        protected object LastWordIdLoder = new object();

        public int GetNextWordId() { return Interlocked.Increment(ref this.LastWordId);  }
        //protected int WordIdPlusRange(int ) { return Interlocked.Increment(ref this.LastWordId); }

        #endregion //protected members

        #region public methods

        public int CountTotalWords()
        {
            int nRst = 0;

            lock (this.Words)
            {
                foreach (Dictionary<string, CWordItem> crrItem in this.Words.Values)
                    nRst += crrItem.Count();
            }

            return nRst;
        }//CountTotalWords()

        public void SetMaxWordLength(int _wLen)
        {
            if (this.Words.Count != 0)
                throw new Exception("Can't set MaxWordLength again.It can only be set once");

            this.Cfg_MaxWordLength = _wLen;

            for (int i = 0; i <= _wLen; i++)
                this.Words.Add(i,new Dictionary<string, CWordItem>());
        }

        public void SetLastWordId(int _lastId)
        {
            lock (this)
                this.LastWordId = _lastId;
        }

        /// <summary>
        /// if word has been existed, then add used counts.
        /// if word is not existed, the add a new word item and all it into index.
        /// </summary>
        /// <param name="_wordText"></param>
        /// <param name="_articleId"></param>
        /// <returns></returns>
        public void AddWordItemFromArticle(string _wordText, int _articleId, bool _bConfirmedRealWord)
        {
            if (_wordText == null || _wordText.Length == 0)
                throw new Exception("_wordText can't be null or empty");

            ////// add wordText into CWordItem list.
            CWordItem item = AddTextIntoWords_Only(_wordText,  _bConfirmedRealWord);

            //////Add item into index 

            CArticle2Word a2w = AddTextIntoIndexes(
                this.Cfg_IsUse_Article2Words ? this.Article2Words : null,
                this.Cfg_IsUse_Words2Article ? this.Words2Article : null,
                _articleId, 
                item);
            //return item;
        }//CWordItem()

        private static CArticle2Word AddTextIntoIndexes(
            Dictionary<int, Dictionary<string, CArticle2Word>> _article2Words,
            Dictionary<string, Dictionary<int, CArticle2Word>> _words2Articleint,
            int _articleId, 
            CWordItem item)
        {

            CArticle2Word a2w = null;
            
            if (_article2Words != null)
                AddTextIntoArticle2Word(_article2Words, _articleId, item);

            //如果 Cfg_IsUse_Article2Words 没有生成过a2w，则现在重新生成

            if (!_words2Articleint.ContainsKey(item.WordText))
            {
                lock (_words2Articleint)
                {
                    if (!_words2Articleint.ContainsKey(item.WordText))
                        _words2Articleint.Add(item.WordText, new Dictionary<int, CArticle2Word>());
                }
            }

            if (!_words2Articleint[item.WordText].ContainsKey(_articleId))
            {
                if (a2w == null)
                {
                    a2w = new CArticle2Word()
                    {
                        ArticleId = _articleId,
                        WordId = item.WordId,
                        UsedTimes = 1
                    };
                    //this.Article2Words[_articleId].Add(item.WordText, a2w);
                }//if (a2w == null)

                lock (_words2Articleint[item.WordText])
                {

                    if (!_words2Articleint[item.WordText].ContainsKey(_articleId))
                    {
                        _words2Articleint[item.WordText].Add(_articleId, a2w);
                    }
                }
            }
            else
                Interlocked.Increment(ref _words2Articleint[item.WordText][_articleId].UsedTimes);

            return a2w;
        }//AddTextIntoIndexes

        private static CArticle2Word AddTextIntoArticle2Word(
            Dictionary<int, Dictionary<string, CArticle2Word>> _article2Words, 
            int _articleId, CWordItem item)
        {

            if (_article2Words == null)
                return null;

            CArticle2Word a2w = null;
            if (!_article2Words.ContainsKey(_articleId))
            {
                lock (_article2Words)
                {
                    if (!_article2Words.ContainsKey(_articleId))
                        _article2Words.Add(_articleId, new Dictionary<string, CArticle2Word>());
                }
            }

            if (!_article2Words[_articleId].ContainsKey(item.WordText))
            {
                lock (_article2Words[_articleId])
                {
                    if (!_article2Words[_articleId].ContainsKey(item.WordText))
                    {
                        a2w = new CArticle2Word()
                        {
                            ArticleId = _articleId,
                            WordId = item.WordId,
                            UsedTimes = 1
                        };
                        _article2Words[_articleId].Add(item.WordText, a2w);
                    }
                    else
                    {
                        a2w = _article2Words[_articleId][item.WordText];
                        Interlocked.Increment(ref a2w.UsedTimes);

                    }//if (!this.Article2Words[_articleId].ContainsKey(item.WordText))

                }//lock (this.Article2Words[_articleId])
            }
            else
            {
                a2w = _article2Words[_articleId][item.WordText];
                Interlocked.Increment(ref a2w.UsedTimes);

            }//if (!this.Article2Words[_articleId].ContainsKey(item.WordText))
            lock (a2w)
                a2w.UsedTimes++;

            return a2w;
        }//AddTextIntoArticle2Word

        private CWordItem AddTextIntoWords_Only(string _wordText, bool _bConfirmedRealWord)
        {
            if (_wordText.Length> this.Cfg_MaxWordLength && !this.Words.ContainsKey(_wordText.Length))
            {
                lock (this.Words)
                {
                    if (!this.Words.ContainsKey(_wordText.Length))
                    {
                        this.Words.Add(_wordText.Length, new Dictionary<string, CWordItem>());
                    }
                }//lock (this.Words)
            }//if (!this.Words.ContainsKey(_wordText.Length))

            bool bWordFound = this.Words[_wordText.Length].ContainsKey(_wordText);

            CWordItem item;
            if (!bWordFound)
            {
                item = new CWordItem()
                {
                    UsedTimes = 1,
                    WordId = this.GetNextWordId(),
                    WordText = _wordText,
                     ComfirmType = CWordItem.EComfirmType.RealWord
                };

                lock (this.Words[_wordText.Length])
                {
                    if (!this.Words[_wordText.Length].ContainsKey(_wordText))
                        this.Words[_wordText.Length].Add(item.WordText, item);
                }
            }//if (!bWordFound)
            else //if (bWordFound== true)
            {
                item = this.Words[_wordText.Length][_wordText];
                Interlocked.Increment(ref item.UsedTimes);
                if (_bConfirmedRealWord)
                    item.ComfirmType = CWordItem.EComfirmType.RealWord;
            }//else //if (bWordFound== true)

            return item;
        }

        /// <summary>
        /// 移除所有计数小于指定数量的单词。
        /// 主要用于在NLP切词以后，删除掉那些低频词。（一次解析就可能占用内存100GB，所以必须删除低频词）
        /// </summary>
        /// <returns></returns>
        public int RemoveAllOnceWords(int _RemmoveMinWordLength)
        {
            int nRemovedWords = 0;
            List<CWordItem> onetimeWords = new List<CWordItem>();

            Dictionary<int, Dictionary<string, CWordItem>> newWords = new Dictionary<int, Dictionary<string, CWordItem>>();
            //for (int i = 0; i <= this.Cfg_MaxWordLength; i++)
            //    newWords.Add(i, new Dictionary<string, CWordItem>());

            //List<string> trace_log = new List<string>();
                //CTimeCounter trace_totalTimer = new CTimeCounter(true) { Name = $"Total Remove Len <= {_RemmoveMinWordLength}" };

            lock (this.Words)
            {
                //CTimeCounter trace_WordsTimer = new CTimeCounter(true) { Name = $"Remove Words.Len <= {_RemmoveMinWordLength}" };
                foreach (int crrLen in this.Words.Keys.OrderBy(a => a))
                {
                    Dictionary<string, CWordItem> wordsDict = this.Words[crrLen];
                    int srcWordsCount = wordsDict.Count();
                    //CTimeCounter crrDictTimer = new CTimeCounter(true) { Name = $"Words[{crrLen}]" };
                    lock (wordsDict)
                    {
                        if (srcWordsCount != 0)
                        {
                            ///delete onetime words from this.words;
                            // CTimeCounter dt1 = new CTimeCounter(true) { Name = $"    Linq" };

                            List<CWordItem> restWords = (from a in wordsDict.Values where a.UsedTimes > _RemmoveMinWordLength select a).ToList();

                            //dt1.SetEnd();
                            //trace_log.Add(dt1.GetSpentSeconds_StrWithName());


                           // CTimeCounter dt2 = new CTimeCounter(true) { Name = $"    Add & Clear" };

                            onetimeWords.AddRange(restWords);

                            nRemovedWords += wordsDict.Count - restWords.Count;
                            wordsDict.Clear();

                            //dt2.SetEnd();
                            //trace_log.Add(dt2.GetSpentSeconds_StrWithName());


                            // CTimeCounter dt3 = new CTimeCounter(true) { Name = $"    Rebuild Dict" };

                            foreach (CWordItem crrWord in restWords)
                            {
                                //if (crrWord.WordText.Length >= this.Cfg_MaxWordLength && !newWords.ContainsKey(crrWord.WordText.Length))
                                //    newWords.Add(crrWord.WordText.Length, new Dictionary<string, CWordItem>());

                                wordsDict.Add(crrWord.WordText, crrWord);

                            }

                            restWords.Clear();
                            //dt3.SetEnd();
                            //trace_log.Add(dt3.GetSpentSeconds_StrWithName());

                        }
                    }//lock (wordsDict)

                    //crrDictTimer.SetEnd();
                    //string trace_str = $"{crrDictTimer.GetSpentSeconds_StrWithName()}; Words[{crrLen}].Cunt: {srcWordsCount} -> {wordsDict.Count};";
                    //Trace.WriteLine(trace_str);
                    //trace_log.Add(trace_str);
                }//foreach (Dictionary<string, CWordItem> wordsDict in this.Words.Values)

                foreach (int i in this.Words.Keys.ToArray())
                {
                    if (this.Words[i].Count == 0)
                        this.Words.Remove(i);
                }
                //trace_WordsTimer.SetEnd();
                //Trace.WriteLine(trace_WordsTimer.GetSpentSeconds_StrWithName());
                //trace_log.Add(trace_WordsTimer.GetSpentSeconds_StrWithName());

            }//lock (this.Words)

            //this.Words = newWords;


            ////delete onetime words from this.Article2Words
            
            if (this.Cfg_IsUse_Article2Words)
            {
                CTimeCounter trace_Article2WordsTimer = new CTimeCounter(true) { Name = $"Remove Article2Words if Words.Len <= {_RemmoveMinWordLength}. " };

                foreach (Dictionary<string, CArticle2Word> crrDict in this.Article2Words.Values)
                {
                    lock (crrDict)
                    {
                        List<KeyValuePair<string, CArticle2Word>> pairs = (from a in crrDict where a.Value.UsedTimes > _RemmoveMinWordLength select a).ToList();
                        crrDict.Clear();

                        foreach (var crrPair in pairs)
                        {
                            crrDict.Add(crrPair.Key, crrPair.Value);
                        }
                    }
                }

                //trace_Article2WordsTimer.SetEnd();
                //Trace.WriteLine(trace_Article2WordsTimer.GetSpentSeconds_StrWithName());
                //trace_log.Add(trace_Article2WordsTimer.GetSpentSeconds_StrWithName());

            }//if (this.Cfg_IsUse_Article2Words)


            ////delete onetime words from this.Words2Article
            Dictionary<string, Dictionary<int, CArticle2Word>> newW2A = new Dictionary<string, Dictionary<int, CArticle2Word>>();
            if (this.Cfg_IsUse_Words2Article)
            {
                CTimeCounter trace_Words2Article = new CTimeCounter(true) { Name = $"Remove Words2Article if Words.Len <= {_RemmoveMinWordLength}. " };

                foreach (CWordItem crrWordItem in onetimeWords)
                {
                    //this.Words2Article[crrWordItem.WordText].Clear();
                    //this.Words2Article.Remove(crrWordItem.WordText);
                    newW2A.Add(crrWordItem.WordText, this.Words2Article[crrWordItem.WordText]);
                }

                lock (this.Words2Article)
                {
                    this.Words2Article.Clear();
                }

                this.Words2Article = newW2A;

                trace_Words2Article.SetEnd();
                Trace.WriteLine(trace_Words2Article.GetSpentSeconds_StrWithName());
                //trace_log.Add(trace_Words2Article.GetSpentSeconds_StrWithName());

            }//if (this.Cfg_IsUse_Words2Article)

            //}//foreach(Dictionary<string, CWordItem> wordsDict in this.Words.Values)
            //////return
            onetimeWords.Clear();
            GC.Collect();
            //trace_totalTimer.SetEnd();
            //Trace.WriteLine(trace_totalTimer.GetSpentSeconds_StrWithName());
            //trace_log.Add(trace_totalTimer.GetSpentSeconds_StrWithName());
            //trace_log.Clear();
            return nRemovedWords;


        }//RemoveAllOnceWords()

        /// <summary>
        /// Save all public attribute in to different file, so that we can merge them together later.
        /// it because words cutting result will be hundrads of GB, the computer's memory can't do that.
        /// </summary>
        /// <param name="_outputRoot"></param>
        public void SaveToFiles(
            string _outputRoot, 
            bool _bSaveWords2File,
            bool _bSaveIndexies2File,
            bool _bClearAfterSave_Words, 
            bool _bClearAfterSave_Indexes, 
            out string _wordsFile, 
            out string _article2WordFile)
        {
            ////// Save words.
            _wordsFile = SaveWords2File(_outputRoot, _bClearAfterSave_Words );

            ////// Save Art2words
            _article2WordFile = SaveIndexies2File(_outputRoot, _bClearAfterSave_Indexes);

            //写文件以及继续保存Artic2Word的逻辑代码都丢失了;

        }//SaveToFile(string _outputRoot)

        public string SaveIndexies2File(string _outputRoot, bool _bClearAfterSave_Indexes)
        {
            string _article2WordFile = Path.Combine(_outputRoot, $"article2words_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.json");
            CTimeCounter tc = new CTimeCounter(false);
            tc.SetStart($"Write File {_article2WordFile} used");
            List<CArticle2Word> article2wordList = new List<CArticle2Word>();

            if (this.Cfg_IsUse_Article2Words)
            {
                lock (this.Article2Words)
                {
                    foreach (Dictionary<string, CArticle2Word> crrDict in this.Article2Words.Values)
                    {
                        if (crrDict == null || crrDict.Count == 0)
                            continue;

                        lock (crrDict)
                        {
                            article2wordList.AddRange(crrDict.Values);

                            if (_bClearAfterSave_Indexes)
                                crrDict.Clear();
                        }//lock(crrPair)

                    }//foreach(Dictionary<string, CWordItem> crrPair in this.Words.Values )
                }//lock (this.Article2Words)
            }
            else if (this.Cfg_IsUse_Words2Article)
            {
                lock (this.Words2Article)
                {
                    foreach (Dictionary<int, CArticle2Word> crrDict in this.Words2Article.Values)
                    {
                        if (crrDict == null || crrDict.Count == 0)
                            continue;

                        lock (crrDict)
                        {
                            article2wordList.AddRange(crrDict.Values);

                            if (_bClearAfterSave_Indexes)
                                crrDict.Clear();
                        }//lock(crrPair)

                    }//foreach(Dictionary<string, CWordItem> crrPair in this.Words.Values )
                }//lock (this.Article2Words)
            }//else if (this.Cfg_IsUse_Words2Article)

            if (article2wordList.Count > 0)
            {
                GJsonHelper.SaveList2File(article2wordList, _article2WordFile);
            }
            else
                _article2WordFile = null;

            Trace.WriteLine($"{tc.GetSpentSeconds_StrWithName()}; lines = {article2wordList.Count}");
            article2wordList.Clear();
            this.Article2Words.Clear();
            this.Words2Article.Clear();
            GC.Collect();
            return _article2WordFile;
        }

        public string SaveWords2File(string _outputRoot, bool _bClearAfterSave_Words)
        {
            List<CWordItem> wordsList = new List<CWordItem>();

            CTimeCounter  tc = new CTimeCounter(false);
            string _wordsFile = Path.Combine(_outputRoot, $"words_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.json");
            tc.SetStart($"Write File {_wordsFile} used");

            lock (this.Words)
            {
                foreach (Dictionary<string, CWordItem> crrDict in this.Words.Values)
                {
                    if (crrDict == null || crrDict.Count == 0)
                        continue;

                    lock (crrDict)
                    {
                        wordsList.AddRange(crrDict.Values);

                        if (_bClearAfterSave_Words)
                            crrDict.Clear();
                    }//lock(crrPair)

                }//foreach(Dictionary<string, CWordItem> crrPair in this.Words.Values )
            }

            GJsonHelper.SaveList2File(wordsList, _wordsFile);
            //wordsList.Clear();
            Trace.WriteLine($"{tc.GetSpentSeconds_StrWithName()}; lines = {wordsList.Count}");
            wordsList.Clear();
            GC.Collect();

            return _wordsFile;
        }

        /// <summary>
        /// 删除那些仅仅是一个更长的词的一部分，而且从来没有单独出现过的词。
        /// 比如“法兰克福”中“兰克福”的出现频次 小于等于 “法兰克福”，那么我们就删掉他。
        /// 而“法兰克”的出现频次高于 “法兰克福”，说明它是一个单独的词。
        /// </summary>
        /// <returns></returns>
        public int RemoveAllInnerLetterWords()
        {
            int minWordLen = 2;
            int delCount = 0;

            lock (this.Words)
            {
                var lenList = this.Words.Keys.OrderBy(a=>a);//.OrderByDescending(a => a);
                foreach (int crrWordLen in lenList) 
                {
                    //跳过太短的词
                    if (crrWordLen <= minWordLen)
                        continue;

                    lock (this.Words[crrWordLen])
                    {
                        if (this.Words[crrWordLen].Count == 0)
                            continue;

                        var crrWordDict = this.Words[crrWordLen];
                        Dictionary<string,int> deldict = new  Dictionary<string, int>();

                        foreach (CWordItem crrWord in crrWordDict.Values)
                        {
                            List<string> subWords = GetAllSubWords(crrWord.WordText);

                            if (subWords == null || subWords.Count == 0)
                                continue;

                            foreach (string crrSubword in subWords)
                            {
                                if (crrSubword.Length == 0 || !this.Words[crrSubword.Length].ContainsKey(crrSubword))
                                    continue;

                                lock (this.Words[crrSubword.Length])
                                {
                                    if (this.Words[crrSubword.Length][crrSubword].UsedTimes <= crrWord.UsedTimes)
                                    {
                                        this.Words[crrSubword.Length].Remove(crrSubword);

                                        if( !deldict.ContainsKey(crrSubword))
                                            deldict.Add(crrSubword, 1);
                                    }
                                }//lock (this.Words[crrSubword.Length])

                                /// remove from Words2Article

                            }//foreach (CWordItem crrWord in crrWordDict.Values)

                        }//foreach(Dictionary<string, CWordItem> crrdict in this.Words[crrWordLen])

                        ///delete all inner letter words from dict

                        //foreach (string delSubWord in deldict.Keys)

                        ///这里还需要继续从 Article2Word 中删除掉这些单词。
                        ///由于Article2Word的删除过于繁琐， 因此需要考虑把这个结构题简化成一个List。
                        ///或者利用 Word2Article 来管理。
                        if (!this.Cfg_IsUse_Words2Article)
                                    continue;
                        lock (this.Words2Article)
                        {
                            foreach (string crrDelStr in deldict.Keys)
                            {
                                this.Words2Article[crrDelStr].Clear();
                                this.Words2Article.Remove(crrDelStr);
                            }
                        }

                        delCount += deldict.Count;
                        deldict.Clear();
                    }//lock (this.Words[crrWordLen])
                }//foreach (int crrWordLen in this.Words.Keys)
            }//lock (this.Words)
            GC.Collect();

            return delCount;
        }//RemoveAllInnerLetterWords()


        /// <summary>
        /// 把一个较长的 词 切分成 多种更短的词。
        /// </summary>
        /// <param name="wordText"></param>
        /// <returns></returns>
        public  List<string> GetAllSubWords(string wordText)
        {
            if (wordText.Length <= 2)
                return null;

            if (!GStrHelper.isAllChinese(wordText))
            {
                List<string> tpList = wordText.Split(GStrHelper.Cfg_BiaoDian).ToList();

                if (tpList.Count == 1 || tpList[0] == wordText)
                    return null;

                return tpList;

            }

            List<string> rstList = new List<string>();

            for(int crrLength = wordText.Length-1; crrLength < wordText.Length; crrLength ++)
            {
                for (int i = 0; i + crrLength <= wordText.Length; i++)
                    rstList.Add(wordText.Substring(i, crrLength));

            }//for(int crrLength = 2; crrLength < wordText.Length; crrLength ++)

            return rstList;
        }//GetAllSubWords(string wordText)

        #endregion //public methods
    }//Class CWordManagr
}
