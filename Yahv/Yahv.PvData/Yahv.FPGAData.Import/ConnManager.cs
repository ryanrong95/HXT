using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.FPGAData.Import
{
    /// <summary>
    /// 连接管理类
    /// </summary>
    public class ConnManager
    {
        static readonly object locker = new object();
        static ConnManager current;

        public static ConnManager Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        current = new ConnManager();
                    }
                }

                return current;
            }
        }

        /// <summary>
        /// PvData数据库连接
        /// </summary>
        public SqlConnection PvData
        {
            get
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PvDataConnectionString"].ConnectionString;
                return new SqlConnection(connectionString);
            }
        }

        /// <summary>
        /// PveStandard数据库连接
        /// </summary>
        public SqlConnection PveStandard
        {
            get
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PveStandardConnectionString"].ConnectionString;
                return new SqlConnection(connectionString);
            }
        }
    }
}
