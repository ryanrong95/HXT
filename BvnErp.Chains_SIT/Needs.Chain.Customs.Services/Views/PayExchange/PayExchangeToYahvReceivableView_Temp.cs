using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class PayExchangeToYahvReceivableView_Temp
    {
        ScCustomsReponsitory Reponsitory { get; set; }

        public PayExchangeToYahvReceivableView_Temp()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public List<PayExchangeToYahvReceivableViewModel_Temp> GetData(string payExchangeApplyID)
        {
            var payExchangeApplies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>();
            var payExchangeApplyItems = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();

            var linq = from payExchangeApply in payExchangeApplies
                       join payExchangeApplyItem in payExchangeApplyItems
                            on new
                            {
                                PayExchangeApplyID = payExchangeApply.ID,
                                PayExchangeApplyDataStatus = payExchangeApply.Status,
                                PayExchangeApplyItemDataStatus = (int)Enums.Status.Normal,
                            }
                            equals new
                            {
                                PayExchangeApplyID = payExchangeApplyItem.PayExchangeApplyID,
                                PayExchangeApplyDataStatus = (int)Enums.Status.Normal,
                                PayExchangeApplyItemDataStatus = payExchangeApplyItem.Status,
                            }
                       join order in orders
                            on new
                            {
                                OrderID = payExchangeApplyItem.OrderID,
                                OrderDataStatus = (int)Enums.Status.Normal,
                            }
                            equals new
                            {
                                OrderID = order.ID,
                                OrderDataStatus = order.Status,
                            }
                       join client in clients on order.ClientID equals client.ID
                       join company in companies on client.CompanyID equals company.ID
                       where order.Status == (int)Enums.Status.Normal
                          && client.Status == (int)Enums.Status.Normal
                          && company.Status == (int)Enums.Status.Normal
                          && payExchangeApply.ID == payExchangeApplyID
                       select new PayExchangeToYahvReceivableViewModel_Temp
                       {
                           Amount = payExchangeApplyItem.Amount,
                           MainOrderId = order.MainOrderId,
                           TinyOrderID = payExchangeApplyItem.OrderID,
                           PayExchangeApplyID = payExchangeApply.ID,
                           ExchangeRate = payExchangeApply.ExchangeRate,
                           PayerName = company.Name,
                           DateTime = payExchangeApply.UpdateDate
                       };

            return linq.ToList();
        }
    }

    public class PayExchangeToYahvReceivableViewModel_Temp
    {
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 主订单号
        /// </summary>
        public string MainOrderId { get; set; } = string.Empty;

        /// <summary>
        /// 子订单号
        /// </summary>
        public string TinyOrderID { get; set; } = string.Empty;

        /// <summary>
        /// 付汇申请ID
        /// </summary>
        public string PayExchangeApplyID { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string PayerName { get; set; } = string.Empty;

        /// <summary>
        /// 汇率
        /// </summary>
        public decimal ExchangeRate { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime DateTime { get; set; }
    }
}
