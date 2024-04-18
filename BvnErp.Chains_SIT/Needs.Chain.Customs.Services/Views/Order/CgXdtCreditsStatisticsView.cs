using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class CgXdtCreditsStatisticsView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public CgXdtCreditsStatisticsView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public CgXdtCreditsStatisticsView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        public string GetClientNameByMainOrderID(string mainOrderID)
        {
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();

            var model = (from order in orders
                         join client in clients on order.ClientID equals client.ID
                         join company in companies on client.CompanyID equals company.ID
                         where order.MainOrderId == mainOrderID
                            && order.Status == (int)Enums.Status.Normal
                            && client.Status == (int)Enums.Status.Normal
                            && company.Status == (int)Enums.Status.Normal
                         select new
                         {
                             ClientName = company.Name,
                         }).FirstOrDefault();

            return model.ClientName;
        }

        public List<CgXdtCreditsStatisticsForDebtTipModel> GetCgXdtCreditsStatisticsForDebtTip(string clientName)
        {
            string payeeName = PurchaserContext.Current.CompanyName;

            var cgXdtCreditsStatisticsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CgXdtCreditsStatisticsView>();

            var result = from cgXdtCreditsStatistics in cgXdtCreditsStatisticsView
                         where cgXdtCreditsStatistics.PayerName == clientName
                            && cgXdtCreditsStatistics.PayeeName == payeeName
                            && cgXdtCreditsStatistics.Business == "供应链"
                         group cgXdtCreditsStatistics by 
                         new
                         {
                             cgXdtCreditsStatistics.Payer,
                             cgXdtCreditsStatistics.PayerName,
                             cgXdtCreditsStatistics.Payee,
                             cgXdtCreditsStatistics.PayeeName,
                             cgXdtCreditsStatistics.Currency,
                             cgXdtCreditsStatistics.Business,
                         } into g
                         select new CgXdtCreditsStatisticsForDebtTipModel
                         {
                             PayerID = g.Key.Payer,
                             PayerName = g.Key.PayerName,
                             PayeeID = g.Key.Payee,
                             PayeeName = g.Key.PayeeName,
                             Business = g.Key.Business,

                             CurrencyInt = g.Key.Currency,
                             TotalSum = g.Sum(t => t.Total),
                             CostSum = g.Sum(t => t.Cost),
                         };

            return result.ToList();
        }

        public bool GetIsOverdue(string payerID, string payeeID, string business, int currency)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"
                                DECLARE @rtn INT = -200;

                                EXEC dbo.[dp_Overdue] @payer = '{0}',
                                    @payee = '{1}', @business = '{2}',
                                    @date = '{3}', @currency = {4}, @rtn = @rtn OUTPUT;

                                SELECT  @rtn;    ", payerID, payeeID, business, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), currency);

            //var rtn = this.Reponsitory.Query<List<string>>(sbSql.ToString()).ToList();

            string strConn = ConfigurationManager.ConnectionStrings["ScCustomsConnectionString"].ConnectionString;

            DataSet mydataset = new DataSet();
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                conn.Open();
                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sbSql.ToString();
                    using (SqlDataAdapter adapter = new SqlDataAdapter())
                    {
                        adapter.SelectCommand = command;
                        adapter.Fill(mydataset);
                    }
                }
            }
            //DataTable mytable = mydataset.Tables[0];
            //foreach (DataRow mydatarow in mytable.Rows)
            //{
            //    string username = mydatarow["UserName"].ToString();
            //    string id = mydatarow["ID"].ToString();
            //    string password = mydatarow["Password"].ToString();
            //}

            decimal overdueMoney = Convert.ToDecimal(mydataset.Tables[0].Rows[0][1]);

            return overdueMoney > 0;
        }
    }

    public class CgXdtCreditsStatisticsForDebtTipModel
    {
        /// <summary>
        /// PayerID
        /// </summary>
        public string PayerID { get; set; } = string.Empty;

        /// <summary>
        /// PayeeID
        /// </summary>
        public string PayeeID { get; set; } = string.Empty;

        /// <summary>
        /// 客户名称
        /// </summary>
        public string PayerName { get; set; } = string.Empty;

        /// <summary>
        /// PayeeName
        /// </summary>
        public string PayeeName { get; set; } = string.Empty;

        /// <summary>
        /// Business
        /// </summary>
        public string Business { get; set; } = string.Empty;

        /// <summary>
        /// 币种 Int
        /// </summary>
        public int CurrencyInt { get; set; }

        /// <summary>
        /// 总额度
        /// </summary>
        public decimal TotalSum { get; set; }

        /// <summary>
        /// 总共欠款（花费）
        /// </summary>
        public decimal CostSum { get; set; }

        /// <summary>
        /// 是否逾期
        /// </summary>
        public bool IsOverdue { get; set; } = false;
    }

}
