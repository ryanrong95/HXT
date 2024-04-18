using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views.Alls
{
    /// <summary>
    /// 应付实付视图
    /// </summary>
    public class PaymentsStatisticsAll : UniqueView<PaymentStatistic, PvWsOrderReponsitory>
    {
        public PaymentsStatisticsAll()
        {
        }
        protected override IQueryable<PaymentStatistic> GetIQueryable()
        {
            var Vouchers = new Yahv.Services.Views.PaymentsStatisticsView<PvWsOrderReponsitory>(this.Reponsitory);
            var Enterprises = new Yahv.Services.Views.EnterprisesTopView<PvWsOrderReponsitory>(this.Reponsitory);
            var linq = from entity in Vouchers
                       join payer in Enterprises on entity.Payer equals payer.ID
                       join payee in Enterprises on entity.Payee equals payee.ID
                       where entity.Business == Payments.ConductConsts.供应链
                       select new PaymentStatistic
                       {
                           OrderID = entity.OrderID,
                           WaybillID = entity.WaybillID,
                           PayableID = entity.PayableID,
                           Payer = entity.Payer,
                           Payee = entity.Payee,
                           PayerName = payer.Name,
                           PayeeName = payee.Name,
                           Subject = entity.Subject,
                           Catalog = entity.Catalog,
                           Currency = entity.Currency,
                           LeftDate = entity.LeftDate,
                           LeftPrice = entity.LeftPrice,
                           RightDate = entity.RightDate,
                           RightPrice = entity.RightPrice ?? 0,
                           Summay = entity.Summay,
                           AdminID = entity.AdminID,
                           AdminName=entity.AdminName,
                           Status = entity.Status,
                           ApplicationID = entity.ApplicationID,
                       };
            return linq;
        }
    }

    /// <summary>
    /// 某订单的应付实付视图
    /// </summary>
    public class PaymentsStatisticsRoll : PaymentsStatisticsAll
    {
        string orderId;

        public PaymentsStatisticsRoll(string orderId)
        {
            this.orderId = orderId;
        }

        protected override IQueryable<PaymentStatistic> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.OrderID == this.orderId);
        }
    }

    /// <summary>
    /// 付款申请的应付实付视图
    /// </summary>
    public class ApplicationPaymentsStatisticsRoll : PaymentsStatisticsAll
    {
        string applicationId;

        public ApplicationPaymentsStatisticsRoll(string applicationId)
        {
            this.applicationId = applicationId;
        }

        protected override IQueryable<PaymentStatistic> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.ApplicationID == this.applicationId);
        }
    }

}
