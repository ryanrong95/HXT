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
    /// 应付、实付 统计视图
    /// </summary>
    public class PaymentsStatisticsView<TReponsitory> : QueryView<PaymentsStatistic, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public PaymentsStatisticsView()
        {

        }

        public PaymentsStatisticsView(TReponsitory reponsitory) : base(reponsitory)
        {

        }
        protected override IQueryable<PaymentsStatistic> GetIQueryable()
        {
            var admins = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.AdminsTopView>();
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.PaymentsStatisticsView>()
                   join _a in admins on entity.AdminID equals _a.ID into join_a
                   from a in join_a.DefaultIfEmpty()
                   where entity.Status == (int)GeneralStatus.Normal
                   select new PaymentsStatistic()
                   {
                       OrderID = entity.OrderID,
                       Currency = (Currency)entity.Currency,
                       Business = entity.Business,
                       Subject = entity.Subject,
                       Catalog = entity.Catalog,
                       PayableID = entity.PayableID,
                       LeftDate = entity.LeftDate,
                       LeftPrice = entity.LeftPrice ?? 0,
                       RightDate = entity.RightDate,
                       RightPrice = entity.RightPrice,
                       Payer = entity.Payer,
                       Payee = entity.Payee,
                       WaybillID = entity.WaybillID,
                       Summay = entity.Summay,
                       AdminID = entity.AdminID,
                       Status = (GeneralStatus)entity.Status,
                       ApplicationID = entity.ApplicationID,
                       TinyID = entity.TinyID,
                       AdminName = a.RealName,
                       ReducePrice = entity.ReducePrice,
                       PayerAnonymous = entity.PayerAnonymous,
                       PayeeAnonymous = entity.PayeeAnonymous,
                       VoucherID = entity.VoucherID,
                       Source = entity.Source,
                       TrackingNumber = entity.TrackingNumber,
                   };
        }
    }
}
