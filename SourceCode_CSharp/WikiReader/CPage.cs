using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Web;

namespace KnowedgeBox.WikiReader
{
    public class CWikiPage
    {
        

        protected string SrcStr;

        public string Title;

        public EPageTitleClass TitleClass = EPageTitleClass.DeftPage;

        public string Ns;

        public int Id;

        public string RedirectTitle;

        public CPageRevision Revision;


        public CWikiPage()
        { 
        }

        public CWikiPage(string _xmlStr)
        {

        }//CXmlPage(string _xmlStr)

        public void ParseXml_Chinese(string _xmlStr)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(_xmlStr);

            this.Title = doc.FirstChild.SelectSingleNode("title").InnerText;
            this.Title = GChineseHelper.FanTi2JianTi(this.Title, GChineseHelper.Cfg_DeftEncoding);
            this.Ns = doc.FirstChild.SelectSingleNode("ns").InnerText;
            this.Id = int.Parse( doc.FirstChild.SelectSingleNode("id").InnerText);

            if (this.Title != null)
                this.Title = HttpUtility.HtmlDecode(this.Title);


            var tpNod = doc.SelectSingleNode("redirect");
            if (tpNod != null)
               this.RedirectTitle = tpNod.Attributes["title"].Value;

            this.Revision = new CPageRevision();
            this.Revision.ParseXml(doc.FirstChild.SelectSingleNode("revision"));

            int idxMaoHao = this.Title.IndexOf(':');
            //object tpObj = null;
            if (idxMaoHao > 0)
            {
                string className = this.Title.Substring(0, idxMaoHao);

                //if (Enum.TryParse(typeof(EPageTitleClass), className, out tpObj))
                //if (Enum.TryParse<EPageTitleClass>( className, out this.TitleClass))
                if (GWikiHelper.PageTileClassesNameList.ContainsKey(className))
                {
                    //this.TitleClass = (EPageTitleClass)tpObj;
                    this.TitleClass = (EPageTitleClass)Enum.Parse(typeof(EPageTitleClass),className);
                    this.Title = this.Title.Substring(idxMaoHao + 1);
                }
                else
                    this.TitleClass = EPageTitleClass.UserDefClass;

            }
        }//Parse(string _xmlStr)

        public void ParseJson(string _xmlStr)
        {

        }//Parse(string _xmlStr)


    }//class CXmlPage
}//namespace
