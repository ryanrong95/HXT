using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 应收实收 统计视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class VouchersStatisticsView<TReponsitory> : QueryView<VoucherStatistic, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public VouchersStatisticsView()
        {

        }

        public VouchersStatisticsView(TReponsitory reponsitory) : base(reponsitory)
        {

        }
        protected override IQueryable<VoucherStatistic> GetIQueryable()
        {
            var statistics = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.VouchersStatisticsView>().Where(item => item.Status == (int)GeneralStatus.Normal);

            var receiveds = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.ReceivedsTopView>().Where(item => item.CouponID != null);
            var coupons = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.CouponsTopView>();
            var admins = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.AdminsTopView>();


            var linq = from received in receiveds
                       join coupon in coupons on received.CouponID equals coupon.ID
                       select new
                       {
                           CouponID = coupon.ID,
                           ReceivableID = received.ReceivableID,
                           coupon.Name,
                           coupon.Price,
                       };

            return from entity in statistics
                   join _c in linq on entity.ReceivableID equals _c.ReceivableID into cs
                   from c in cs.DefaultIfEmpty()
                   join _a in admins on entity.AdminID equals _a.ID into joinA
                   from a in joinA.DefaultIfEmpty()
                   select new VoucherStatistic()
                   {
                       OrderID = entity.OrderID,
                       Currency = (Currency)entity.Currency,
                       Business = entity.Business,
                       Subject = entity.Subject,
                       Catalog = entity.Catalog,
                       ReceivableID = entity.ReceivableID,
                       OriginCurrency = (Currency)entity.OriginCurrency,
                       OriginPrice = entity.OriginPrice,
                       LeftDate = entity.LeftDate,
                       LeftPrice = entity.LeftPrice,
                       RightDate = entity.RightDate,
                       RightPrice = entity.RightPrice,
                       Payer = entity.Payer,
                       PayerID = entity.PayerID,
                       Payee = entity.Payee,
                       PayeeID = entity.PayeeID,
                       WaybillID = entity.WaybillID,
                       OriginalDate = entity.OriginalDate,
                       ChangeDate = entity.ChangeDate,
                       Summay = entity.Summay,
                       AdminID = entity.AdminID,
                       AdminName = a.RealName,
                       Status = (GeneralStatus)entity.Status,
                       TinyID = entity.TinyID,
                       ApplicationID = entity.ApplicationID,
                       ItemID = entity.ItemID,
                       ReducePrice = entity.ReducePrice,

                       CouponID = c.CouponID,
                       CouponName = c.Name,
                       CouponPrice = c.CouponID != null ? (c.Price ?? entity.RightPrice) : null,

                       OriginalIndex = entity.OriginalIndex,
                       ChangeIndex = entity.ChangeIndex,
                       PayeeAnonymous = entity.PayeeAnonymous,
                       PayerAnonymous = entity.PayerAnonymous,

                       Source = entity.Source,
                       TrackingNumber = entity.TrackingNumber,
                       Data = entity.Data,
                       Quantity = entity.Quantity,
                   };
        }
    }
}
