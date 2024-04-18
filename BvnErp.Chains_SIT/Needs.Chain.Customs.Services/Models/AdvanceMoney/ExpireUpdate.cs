using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ExpireUpdate
    {
        public string OrderID { get; set; }
        public string ConnectionString { get; set; }

        public ExpireUpdate(string orderID)
        {
            this.OrderID = orderID;
            this.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["foricScCustomsConnectionString"].ConnectionString;
        }

        public void Update()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(this.ConnectionString))
                {
                    conn.Open();
                    SqlCommand sqlMainOrder = new SqlCommand("ExpireOrderFeePro", conn);
                    sqlMainOrder.CommandType = CommandType.StoredProcedure;

                    sqlMainOrder.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.VarChar, 50));
                    sqlMainOrder.Parameters["@OrderID"].Value = this.OrderID;

                    sqlMainOrder.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                ex.CcsLog("更新订单超时金额信息!");
            }            
        }
    }
}
