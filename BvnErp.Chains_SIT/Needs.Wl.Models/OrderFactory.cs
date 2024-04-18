using System;
using System.Data.Linq.SqlClient;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 订单编号生成
    /// </summary>
    public class OrderFactory
    {
        /// <summary>
        /// 生成订单编号
        /// 规则：客户编号+日期+当天的序号 如：WL001720180128001
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static string CreateOrderID(Client client)
        {
            string orderIndex = "";
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int orderCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                                .Where(item => item.ClientID == client.ID && SqlMethods.DateDiffDay(item.CreateDate, DateTime.Now) == 0).Count();
                orderIndex = (orderCount + 1).ToString();
                orderIndex = orderIndex.PadLeft(3, '0');
            }

            return client.ClientCode + DateTime.Now.ToString("yyyyMMdd") + orderIndex;
        }
    }
}
