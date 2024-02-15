using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnowedgeBox.WikiReader
{

    /// <summary>
    /// wiki会在Title的前面用冒号分割。前面就是 TitleClass
    /// 下面是通过数据分析获得的wiki固有预定义分类。
    /// 其实还有非常多网友自己创建的类别，但是内容都非常少。都在1000以下。
    /// {[, Count = 2204153]}
    /// {[Wikipedia, Count = 68370]
    /// {[Help, Count = 917]}
    /// {[WikiProject, Count = 3132]}
    /// {[Template, Count = 982353]}
    /// {[File, Count = 57945]}
    /// {[MediaWiki, Count = 8579]}
    /// {[Category, Count = 398598]}
    /// {[Portal, Count = 10761]}
    /// {[Module, Count = 3901]}
    /// {[Draft, Count = 1006]}
    /// {[Topic, Count = 35861]}
    /// </summary>
    public enum EPageTitleClass
    {
        DeftPage = 1,
        UserDefClass = 2,
        Wikipedia = 3,
        Help = 4,
        WikiProject = 5,
        Template = 6,
        File = 7,
        MediaWiki = 8,
        Category = 9,
        Portal = 10,
        Module = 11,
        Draft = 12,
        Topic =13
    }//enum EPageTitleClass
}
