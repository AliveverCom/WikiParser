using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLP_Segmentation.Lexical
{
    public class CWordItem_PartsOfSpeech
    {
        public int WordId;

        public bool IsConfirmed = false;

        /// <summary>
        /// 1 名词 noun n. student 学生
        /// </summary>
        public bool N;

        /// <summary>
        /// 2 代词 pronoun pron. you 你
        /// </summary>
        public bool Pron;

        /// <summary>
        /// 形容词 adjective adj. happy 高兴的
        /// </summary>
        public bool Adj;

        /// <summary>
        /// 副词 adverb adv. quickly 迅速地
        /// </summary>
        public bool Adv;

        /// <summary>
        /// 5 动词 verb v. cut 砍、割
        /// </summary>
        public bool V;

        /// <summary>
        /// 数词 numeral num. three 三
        /// </summary>
        public bool Num;

        /// <summary>
        /// 7 冠词 article art. a 一个
        /// </summary>
        public bool Art;

        /// <summary>
        /// 8 介词 preposition prep. at 在...
        /// </summary>
        public bool Prep;

        /// <summary>
        /// 9 连词 conjunction conj. and
        /// </summary>
        public bool Conj;

        /// <summary>
        /// 10 感叹词 interjection interj. oh 哦
        /// </summary>
        public bool Interj;

    }//class CWordItem_PartsOfSpeech
}
