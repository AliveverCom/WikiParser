using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace NLP_Segmentation.Lexical
{
    /// <summary>
    /// Augithem. cutting on  article to words and save them into a referenced words manager
    /// </summary>
    public class CWordsCutter
    {
        #region public attributes

        public int In_ArticleId;

        public string In_ArticleText;

        public CArticleWordsManager InOut_WordMgr;

        protected int In_MaxWordLength;
        

        #endregion // public attributes

        protected int Proc_FileLines = 0;

        #region public methods

        public CWordsCutter(int _ArticleId, 
            string _ArticleText, 
            CArticleWordsManager _WordMgr,
            int _MaxWordLength)
        {
            this.In_ArticleId = _ArticleId;
            this.In_ArticleText = _ArticleText;
            this.InOut_WordMgr = _WordMgr;
            this.In_MaxWordLength = _MaxWordLength;

        }//CWordsCutter()


        public void CuttingWords()
        {
            Preprocessing();
            Do();
        }// CuttingWords()

        protected bool Do()
        {
            //StringReader sr = new StringReader(this.In_ArticleText);
            string[] lines = this.In_ArticleText.Split('\n');

            //string crrLine = sr.ReadLine();
            //int nLine = 1;
            int debug_nEmptyLines = 0;
            int n双括号 = 0;
            int debug_BreakPoint =0;
            for(int nLine = 0; nLine < lines.Length; nLine++)
            {
                string crrLine = lines[nLine];
                Interlocked.Increment(ref Proc_FileLines);
                //if (Proc_FileLines % 1000 == 0)
                //    debug_BreakPoint++;

                crrLine = crrLine.Trim();
                //if (nLine >= 170)
                //        debug_BreakPoint++;

                if (crrLine.Length == 0)
                {
                    debug_nEmptyLines++;

                    if (debug_nEmptyLines >= 100)
                        throw new Exception("连续100行为空行");

                    continue;
                }

                debug_nEmptyLines = 0;

                //跳过所有双括号
                //扫描整个字符串，对双括号进行计数。
                string _strLeft = "{{";
                if (crrLine.Contains(_strLeft) )
                    GStrHelper.符号计数_Increase(_strLeft, crrLine, ref n双括号);

                string _strRight = "}}";
                if ( crrLine.Contains(_strRight))
                    GStrHelper.符号计数_Decrease(_strRight, crrLine, ref n双括号);

                //if (n双括号!=0 && crrLine == _strRight)
                //    n双括号 = 0;

                if (n双括号 != 0)
                    continue;

                if (crrLine[0] == '|')
                    continue;

                ParseOneLine(crrLine);
               // crrLine = sr.ReadLine();

                //nLine++;
            }// while(crrLine != null)

            return true;
        }//Do()


        protected void 区分中英文的分割字符串(string _str, ref List<string> _rstList)
        {
            //List<string> rstList = new List<string>();
            bool crrWordIsChinese = true;
            StringBuilder sb = new StringBuilder();

            for(int i=0; i < _str.Length; i++ )
            {
                if (GStrHelper.IsChinese(_str[i]) == crrWordIsChinese)
                    sb.Append(_str[i]);
                else 
                {
                    if (sb.Length != 0)
                        _rstList.Add(sb.ToString());
                    sb.Clear();
                    sb.Append(_str[i]);
                    crrWordIsChinese = !crrWordIsChinese;
                }//if ( char.IsLetterOrDigit(crrSntcTrim[i]))
            }//循环每一个字，for (int i = 0; i < _str.Length; i++)

            if (sb.Length !=0)
                _rstList.Add(sb.ToString());

        }//区分中英文的分割字符串(string _str, ref _rstList)()

        /// <summary>
        /// 循环所有被切割的字符串。并将中英文从这些字符串中分离出来。
        /// 同时去掉所有空字符串。最终生成一个新的字符串列表。
        /// 本函数不再考虑其中包含了 分隔符和标点的情况。这一步需要在之前就做好。
        /// </summary>
        /// <param name="_srcList"></param>
        /// <returns></returns>
        protected List<string> 区分中英文的分割字符串(IEnumerable<string> _srcList)
        {
            List<string> rstList = new List<string>();

            foreach (string crrStr in _srcList)
            {
                string crrStrTrim = crrStr.Trim();
                if (crrStrTrim.Length == 0)
                    continue;

                ///如果当前字符串中有“非中文”，则进一步进行中英文切分。
                if (GStrHelper.isAllChinese(crrStrTrim))
                    rstList.Add(crrStrTrim);
                else
                    区分中英文的分割字符串(crrStrTrim, ref rstList);
            }//foreach (string crrStr in _srcList)

            return rstList;
        }//区分中英文的分割字符串(string _crrLine)

        protected void ParseOneLine(string _crrLine)
        {
            string[] shortSentenceList = _crrLine.Split(GStrHelper. Cfg_BiaoDian);
            List<string> cn_en_split_List = 区分中英文的分割字符串(shortSentenceList);

            //循环每一个短语
            foreach (string crrSentence in cn_en_split_List)
            {
                ///loop for each type of length of the word 循环每一个长度
                for (int crrWordLength = 1; crrWordLength <= this.In_MaxWordLength; crrWordLength++)
                {
                    //string crrWord;
                    if (crrWordLength > crrSentence.Length)
                        break;

                    //如果当前是英文单词，则只有在 切分长度是1的时候才处理（一个单词看作长度为1的中文字）
                    if (!GStrHelper.IsChinese(crrSentence[0]))
                    {
                        if (crrWordLength == 1 )
                        {
                            if ( crrSentence.Length < 32)
                                this.InOut_WordMgr.AddWordItemFromArticle(crrSentence, this.In_ArticleId, false);

                            continue;
                        }
                        else
                            break;
                    }
                    else //如果是中文
                    {
                        //按当前指定长度切分
                        for (int i = 0; i < crrSentence.Length ; i++)
                        {
                            string tpStr = crrSentence.Substring(i, crrWordLength);

                            if (!tpStr.Contains('的'))
                                this.InOut_WordMgr.AddWordItemFromArticle(tpStr, this.In_ArticleId,false);

                            if (i + crrWordLength >= crrSentence.Length)
                                break;
                        }
                    }//else //如果是中文


                }// foreach (string crrSentence in shortSentenceList)
            }//for(int crrWordLength =1; crrWordLength <= this.In_MaxWordLength; crrWordLength++)

        }//ParseOneLine(string _crrLine)

        protected bool Preprocessing()
        {
            if (In_ArticleText == null || In_ArticleText.Length == 0 )
            {
                throw new Exception("In_ArticleText is null or empty");
            }

            if (InOut_WordMgr == null || InOut_WordMgr.Words == null || InOut_WordMgr.Words.Count == 0)
                throw new Exception("In_WordMgr is null or In_WordMgr.Words is not set.");

            return true;
        }//Preprocessing()

        #endregion //public methods

    }// CCutArticle2Words
}
