using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using ShaoChenYe.DevFramework.MySqlLib;

namespace KnowedgeBox.WinApp
{
    class GtWinApp
    {

        public static CSingleSqlExecuter DbConn_public;


        public static readonly int Cfg_MaxMultiThread = 10;


        static GtWinApp()
        {
            InitDbConn_public("nb_wiki");
        }

        public static void InitDbConn_public(string _dbName)
        {
            if (DbConn_public == null)
            {
                CSingleSqlExecuter db_conn;
                db_conn = new CSingleSqlExecuter($"server=127.0.0.1;uid=root;pwd=123456;database={_dbName}");
                db_conn.CloseDbConnWhenExecuted = false;
                db_conn.SqlTimeOutSec = 3600;
                DbConn_public = db_conn;
            }

        }//void InitDbConn_Acc()




    }//class GtWinApp
}//namespace
