using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.XdtData.Import.Connections
{
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

        public SqlConnection PvData
        {
            get
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PvDataConnectionString"].ConnectionString;
                return new SqlConnection(connectionString);
            }
        }

        public SqlConnection PvWsOrder
        {
            get
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PvWsOrderConnectionString"].ConnectionString;
                return new SqlConnection(connectionString);
            }
        }

        public SqlConnection PvCenter
        {
            get
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PvCenterConnectionString"].ConnectionString;
                return new SqlConnection(connectionString);
            }
        }

        public SqlConnection PvWms
        {
            get
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PvWmsConnectionString"].ConnectionString;
                return new SqlConnection(connectionString);
            }
        }

        public SqlConnection PvbCrm
        {
            get
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PvbCrmConnectionString"].ConnectionString;
                return new SqlConnection(connectionString);
            }
        }
    }
}
