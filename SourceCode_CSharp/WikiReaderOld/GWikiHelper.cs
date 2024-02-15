using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KnowedgeBox.WikiReader
{
    public class GWikiHelper
    {

        public static readonly object MultiThreadLocker = new object();

        public static Dictionary<string,int> PageTileClassesNameList= new ();

        public static string StrLines2Str(List<string> _strList)
        {
            StringBuilder sb = new StringBuilder();

            foreach(string str in _strList)
                sb.Append(str).Append("\n");

            return sb.ToString();

        }//StrLines2Str(List<string> _strList)

        static GWikiHelper()
        {
            foreach (string crrName in Enum.GetNames<EPageTitleClass>())
                PageTileClassesNameList.Add(crrName, 0);

        }

        public static string GetTextByChildName(XmlNode _node , string _childNodeName)
        {
            XmlNode  childNode = _node.SelectSingleNode(_childNodeName);

            return childNode == null? null:childNode.InnerText;

        }//GetTextByChildName(XmlNode _node, string _childNodeName)

    }//class GWikiHelper
}
