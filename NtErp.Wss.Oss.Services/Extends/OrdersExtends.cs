using NtErp.Wss.Oss.Services.Models;
using NtErp.Wss.Oss.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Extends
{
    static class OrdersExtends
    {
        /// <summary>
        /// 获取已支付各种账户的汇总信息
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        static public OrderPay[] GetPaids(this Order order)
        {
            using (var reponsitory = new Layer.Data.Sqls.CvOssReponsitory())
            {
                var linq = from item in reponsitory.ReadTable<Layer.Data.Sqls.CvOss.UserOutputs>()
                           where item.OrderID == order.ID
                           group item by item.Type into groups
                           select new OrderPay
                           {
                               Type = (UserAccountType)groups.Key,
                               Price = groups.Sum(item => item.Amount)
                           };

                return linq.ToArray();
            }
        }
    }
}
