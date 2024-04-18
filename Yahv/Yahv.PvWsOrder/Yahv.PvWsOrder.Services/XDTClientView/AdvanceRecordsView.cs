using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    public class AdvanceRecordsView
    {
        /// <summary>
        /// 从 AdvanceRecords 表中获取一个大订单的 Amount 求和
        /// </summary>
        /// <param name="mainOrderID"></param>
        /// <returns></returns>
        public decimal GetAmountForDeclareTotalPrice(string mainOrderID)
        {
            decimal amount = 0;

            using (Layers.Data.Sqls.ScCustomReponsitory reponsitory = new Layers.Data.Sqls.ScCustomReponsitory())
            {
                var advanceRecords = reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.AdvanceRecords>();
                var orders = reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Orders>();

                var result = (from advanceRecord in advanceRecords
                              join order in orders on advanceRecord.OrderID equals order.ID
                              where order.MainOrderId == mainOrderID
                              group new { order, advanceRecord } by new { order.MainOrderId } into g
                              select new
                              {
                                  MainOrderId = g.Key.MainOrderId,
                                  TotalAmount = g.Sum(t => t.advanceRecord.Amount),
                              }).FirstOrDefault();

                if (result != null)
                {
                    amount = result.TotalAmount;
                }

                return amount;
            }



        }
    }
}
