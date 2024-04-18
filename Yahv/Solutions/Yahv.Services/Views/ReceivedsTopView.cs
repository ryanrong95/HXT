using System;
using System.Linq;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 实收通用视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class ReceivedsTopView<TReponsitory> : UniqueView<Received, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public ReceivedsTopView()
        {

        }

        public ReceivedsTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Received> GetIQueryable()
        {
            var admins = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.AdminsTopView>();
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.ReceivedsTopView>()
                   join a in admins on entity.AdminID equals a.ID into adminJoin
                   from _a in adminJoin.DefaultIfEmpty()
                   select new Received()
                   {
                       Price = entity.Price,
                       AccountType = (AccountType)entity.AccountType,
                       CreateDate = entity.CreateDate,
                       ID = entity.ID,
                       WaybillID = entity.WaybillID,
                       AdminID = entity.AdminID,
                       ReceivableID = entity.ReceivableID,
                       OrderID = entity.OrderID,
                       Summay = entity.Summay,
                       FlowID = entity.FlowID,
                       AccountCode = entity.AccountCode,
                       Currency1 = (Currency)entity.Currency1,
                       Price1 = entity.Price1,
                       Rate1 = entity.Rate1,
                       AdminName = _a.RealName,
                   };
        }

        public IQueryable<Received> GetIQueryableEx()
        {
            var admins = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.AdminsTopView>();
            var flowAccounts = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.FlowAccountsTopView>();

            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.ReceivedsTopView>()
                   join a in admins on entity.AdminID equals a.ID into adminJoin
                   from _a in adminJoin.DefaultIfEmpty()
                   join f in flowAccounts on entity.FlowID equals f.ID into flowJoin
                   from _f in flowJoin.DefaultIfEmpty()
                   select new Received()
                   {
                       Price = entity.Price,
                       AccountType = (AccountType)entity.AccountType,
                       CreateDate = entity.CreateDate,
                       ID = entity.ID,
                       WaybillID = entity.WaybillID,
                       AdminID = entity.AdminID,
                       ReceivableID = entity.ReceivableID,
                       OrderID = entity.OrderID,
                       Summay = entity.Summay,
                       FlowID = entity.FlowID,
                       AccountCode = entity.AccountCode,
                       Currency1 = (Currency)entity.Currency1,
                       Price1 = entity.Price1,
                       Rate1 = entity.Rate1,
                       AdminName = _a.RealName,
                       FormCode = _f == null ? "" : _f.FormCode,
                   };
        }
    }
}
