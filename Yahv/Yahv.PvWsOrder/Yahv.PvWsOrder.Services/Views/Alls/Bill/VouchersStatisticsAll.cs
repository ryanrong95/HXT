using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Payments;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views.Origins;
using Yahv.Services.Views;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views.Alls
{
    /// <summary>
    /// 代仓储订单应收实收视图
    /// </summary>
    public class VouchersStatisticsAll : UniqueView<VoucherStatistic, PvWsOrderReponsitory>
    {
        public VouchersStatisticsAll()
        {
        }

        protected override IQueryable<VoucherStatistic> GetIQueryable()
        {
            var Vouchers = new Yahv.Services.Views.VouchersStatisticsView<PvWsOrderReponsitory>(this.Reponsitory);
            var Enterprises = new Yahv.Services.Views.EnterprisesTopView<PvWsOrderReponsitory>(this.Reponsitory);
            var Receiveds = new Yahv.Services.Views.ReceivedsTopView<PvWsOrderReponsitory>(this.Reponsitory);

            var linq = from entity in Vouchers
                       join payee in Enterprises on entity.Payee equals payee.ID
                       join payer in Enterprises on entity.Payer equals payer.ID into vouchers
                       from voucher in vouchers.DefaultIfEmpty()
                       join received in Receiveds on entity.ReceivableID equals received.ReceivableID into receiveds
                       where entity.Business == ConductConsts.供应链
                       select new VoucherStatistic
                       {
                           ID = entity.ReceivableID,
                           ReceivableID = entity.ReceivableID,
                           OrderID = entity.OrderID,
                           WaybillID = entity.WaybillID,
                           Payer = entity.Payer,
                           Payee = entity.Payee,
                           Catalog = entity.Catalog,
                           Subject = entity.Subject,
                           Currency = entity.Currency,
                           LeftPrice = entity.LeftPrice,
                           LeftDate = entity.LeftDate,
                           RightPrice = entity.RightPrice,
                           RightDate = entity.RightDate,
                           OriginalDate = entity.OriginalDate,
                           Summay = entity.Summay,
                           CouponID = entity.CouponID,
                           PayerName = voucher.Name,
                           PayeeName = payee.Name,
                           //PayeeAnonymous = entity.PayeeAnonymous,
                           //PayerAnonymous = entity.PayerAnonymous,
                           ApplicationID = entity.ApplicationID,
                           Receiveds = receiveds,
                           AdminID = entity.AdminID,
                           AdminName = entity.AdminName,
                       };
            return linq;
        }
    }

    /// <summary>
    /// 某订单的应收实收视图
    /// </summary>
    public class VouchersStatisticsRoll : VouchersStatisticsAll
    {
        string orderId;

        public VouchersStatisticsRoll(string orderId)
        {
            this.orderId = orderId;
        }

        protected override IQueryable<VoucherStatistic> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.OrderID == this.orderId);
        }
    }

    /// <summary>
    /// 付汇申请的应收实收视图
    /// </summary>
    public class ApplicationVouchersStatisticsRoll : VouchersStatisticsAll
    {
        string applicationID;

        public ApplicationVouchersStatisticsRoll(string applicationID)
        {
            this.applicationID = applicationID;
        }

        protected override IQueryable<VoucherStatistic> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.ApplicationID == this.applicationID);
        }
    }

    /// <summary>
    /// 未结算的仓储费视图
    /// </summary>
    public class UnReceivedStorageChargesView : VouchersStatisticsAll
    {
        public UnReceivedStorageChargesView()
        {
        }

        protected override IQueryable<VoucherStatistic> GetIQueryable()
        {
            return base.GetIQueryable()
                .Where(item => item.Catalog == "杂费" && item.Subject == "仓储费")
                .Where(item => item.OrderID == null || item.OrderID == "")
                .Where(item => (item.RightPrice ?? 0m) != item.LeftPrice);
        }
    }

    /// <summary>
    /// 某客户未结算的仓储费视图
    /// </summary>
    public class UnReceivedStorageChargesRoll : UnReceivedStorageChargesView
    {
        private string clientID;

        public UnReceivedStorageChargesRoll(string clientID)
        {
            this.clientID = clientID;
        }

        protected override IQueryable<VoucherStatistic> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.Payer == clientID);
        }
    }
}
