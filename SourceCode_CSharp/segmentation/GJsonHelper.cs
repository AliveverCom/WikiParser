using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.IO;

namespace NLP_Segmentation
{
    public class GJsonHelper
    {
        public static void SaveList2File<T>(IEnumerable<T> _objList, string _fileURL)
        {

            //FileStream fs = new FileStream(_fileURL, FileMode.OpenOrCreate);
            var serializer = new JavaScriptSerializer();
            Encoding ecd =  Encoding.GetEncoding("GB18030");
            byte[] enterBt = ecd.GetBytes("\n");

            List<string> strList = new List<string>();

            foreach (T crrItem in _objList)
            {
                string jsonStr = serializer.Serialize(crrItem);
                strList.Add(jsonStr);
                //byte[] btArray = ecd.GetBytes(jsonStr);
                ////List<string> lines = new List<string>();
                //fs.Write(btArray, 0, btArray.Length);
                //fs.Write(enterBt, 0, enterBt.Length);
            }

            File.WriteAllLines(_fileURL, strList, ecd);

            //fs.Close();
        }//Save2File<T>(IEnumerable<T> _objList)

        //public List<T> ReadFile2List<T>( string _fileURL)
        //{


        //}//Save2File<T>(IEnumerable<T> _objList)

    }//class GJsonHelper
}//NLP_Segmentation
