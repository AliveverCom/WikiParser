using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Web;

namespace KnowedgeBox.WikiReader
{
    public class CPageRevision
    {
        public int Id;
        public int Parentid;
        public DateTime Timestamp;
        public string Contributor;
        public string Comment;
        public string Model;
        public string Format;
        public string Text;
        public string Sha1;

        public void ParseXml(XmlNode _xmlNode)
        {
            this.Contributor = GWikiHelper.GetTextByChildName(_xmlNode, "contributor");
            this.Comment = GWikiHelper.GetTextByChildName(_xmlNode, "comment");
            this.Model = GWikiHelper.GetTextByChildName(_xmlNode, "model");
            this.Format = GWikiHelper.GetTextByChildName(_xmlNode, "format");
            this.Text = GWikiHelper.GetTextByChildName(_xmlNode, "text");
            this.Sha1 = GWikiHelper.GetTextByChildName(_xmlNode, "sha1");

            this.Text = GChineseHelper.FanTi2JianTi(this.Text, GChineseHelper.Cfg_DeftEncoding);


            if (this.Text != null)
                this.Text = HttpUtility.HtmlDecode(this.Text);

            string tpStr = GWikiHelper.GetTextByChildName(_xmlNode, "parentid"); 
            if (tpStr != null )
                this.Parentid = int.Parse(tpStr);

            tpStr = GWikiHelper.GetTextByChildName(_xmlNode, "timestamp");
            if (tpStr != null)
                this.Timestamp = DateTime.Parse(tpStr) ;
            //this.  = _xmlDom.GetAttribute("");

        }//Parse(string _xmlStr)

    }//CPageRevision
}//namespace
