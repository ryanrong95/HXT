using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Payments;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Extends
{
    /// <summary>
    /// 财务记账接口
    /// </summary>
    public static class FinanceExtends
    {
        /// <summary>
        /// 管理端产生租赁费用
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static void LsOrderFee(this LsOrder order)
        {
            var fee = order.OrderItems.Sum(item => item.Quantity * item.UnitPrice * item.Lease.Month);
            if (order.VouchersStatistic == null)
            {
                //添加
                PaymentManager.Erp(order.OperatorID)[order.ClientID, order.PayeeID][ConductConsts.供应链]
                    .Receivable["杂费", "库位租赁费"].Record(Currency.CNY, fee, order.ID);
            }
            else
            {
                if (order.Status == LsOrderStatus.Closed)
                {
                    //重记应收为零
                    PaymentManager.Erp(order.OperatorID)[order.ClientID, order.PayeeID][ConductConsts.供应链]
                    .Receivable.For(order.VouchersStatistic.ID).ReRecord(0);
                }
                else
                {
                    //重记应收
                    PaymentManager.Erp(order.OperatorID)[order.ClientID, order.PayeeID][ConductConsts.供应链]
                    .Receivable.For(order.VouchersStatistic.ID).ReRecord(fee);
                }
            }
        }

        /// <summary>
        /// 管理端产生付款申请费用
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static void PaymentApplicationFee(this Application application)
        {
            //if (application.Type != ApplicationType.Payment)
            //{
            //    return;
            //}
            //if (application.VouchersStatistic == null)
            //{
            //    //添加 //TODO:
            //    PaymentManager.Erp(application.Operator)[application.ClientID, "香港万路通国际物流有限公司"][ConductConsts.供应链].Receivable[CatalogConsts.货款, "代付货款"]
            //        .Record(application.Beneficiary.Currency, application.Price, application.OrderID, applicationID: application.ID);
            //}
            //else
            //{
            //    if (application.Status == GeneralStatus.Closed)
            //    {
            //        //重记应收为零 //TODO:
            //        PaymentManager.Erp(application.Operator)[application.ClientID, "香港万路通国际物流有限公司"][ConductConsts.供应链]
            //            .Receivable.For(application.VouchersStatistic.ID).ReRecord(0);
            //    }
            //    else
            //    {
            //        //重记应收 //TODO:
            //        PaymentManager.Erp(application.Operator)[application.ClientID, "香港万路通国际物流有限公司"][ConductConsts.供应链]
            //            .Receivable.For(application.VouchersStatistic.ID).ReRecord(application.Price);
            //    }
            //}
        }
    }
}
