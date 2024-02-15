using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowedgeBox.WikiReader
{
    /// <summary>
    /// All lines of one page in the raw wiki xml file.
    /// </summary>
    public class CPageLines
    {
        public List<string> Lines;

        public int nBeginLine;

        public string GetAllLinesAsOneString()
        {
            return GWikiHelper.StrLines2Str(this.Lines);

        }
    }///class CPageLines
}
