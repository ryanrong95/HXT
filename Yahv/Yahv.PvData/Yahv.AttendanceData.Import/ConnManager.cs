using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.AttendanceData.Import
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
        /// Erm数据库连接
        /// </summary>
        public SqlConnection PvbErm
        {
            get
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PvbErmConnectionString"].ConnectionString;
                return new SqlConnection(connectionString);
            }
        }
    }
}
