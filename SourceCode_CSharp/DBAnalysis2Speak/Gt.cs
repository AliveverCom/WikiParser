using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShaoChenYe.DevFramework.MySqlLib;

namespace DBAnalysis2Speak
{
    public class Gt
    {

        public delegate void DelegateDef_Speak(string _speakStr);

        public static DelegateDef_Speak FunRef_Speak;

        public static void InitGt( DelegateDef_Speak _FunRef_Speak)
        {
            //DbConn_public = _DbConn_public;
            FunRef_Speak = _FunRef_Speak;
        }

        public static void Speak(string _speakStr)
        {
            FunRef_Speak.Invoke( _speakStr);
        }


    }//class Gt
}//namespace DBAnalysis2Speak
