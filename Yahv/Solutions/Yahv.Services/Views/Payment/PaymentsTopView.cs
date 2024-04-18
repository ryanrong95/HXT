using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 实付通用视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class PaymentsTopView<TReponsitory> : QueryView<Models.Payment, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public PaymentsTopView()
        {

        }

        public PaymentsTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.Payment> GetIQueryable()
        {
            var admins = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.AdminsTopView>();
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.PaymentsTopView>()
                   join a in admins on entity.AdminID equals a.ID into adminJoin
                   from _a in adminJoin.DefaultIfEmpty()
                   select new Models.Payment()
                   {
                       Price = entity.Price,
                       ID = entity.ID,
                       OrderID = entity.OrderID,
                       CreateDate = entity.CreateDate,
                       AccountType = (AccountType)entity.AccountType,
                       WaybillID = entity.WaybillID,
                       AdminID = entity.AdminID,
                       PayableID = entity.PayableID,
                       FlowID = entity.FlowID,
                       Summay = entity.Summay,
                       Currency1 = (Currency)entity.Currency1,
                       AccountCode = entity.AccountCode,
                       Price1 = entity.Price1,
                       Rate1 = entity.Rate1,
                       AdminName = _a.RealName,
                   };
        }

        public IQueryable<Models.Payment> GetIQueryableEx()
        {
            var admins = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.AdminsTopView>();
            var flowAccounts = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.FlowAccountsTopView>();

            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.PaymentsTopView>()
                   join a in admins on entity.AdminID equals a.ID into adminJoin
                   from _a in adminJoin.DefaultIfEmpty()
                   join f in flowAccounts on entity.FlowID equals f.ID into flowJoin
                   from _f in flowJoin.DefaultIfEmpty()
                   select new Models.Payment()
                   {
                       Price = entity.Price,
                       ID = entity.ID,
                       OrderID = entity.OrderID,
                       CreateDate = entity.CreateDate,
                       AccountType = (AccountType)entity.AccountType,
                       WaybillID = entity.WaybillID,
                       AdminID = entity.AdminID,
                       PayableID = entity.PayableID,
                       FlowID = entity.FlowID,
                       Summay = entity.Summay,
                       Currency1 = (Currency)entity.Currency1,
                       AccountCode = entity.AccountCode,
                       Price1 = entity.Price1,
                       Rate1 = entity.Rate1,
                       AdminName = _a.RealName,
                       FormCode = _f == null ? "" : _f.FormCode,
                   };
        }
    }
}
