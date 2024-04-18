using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views.Payment
{
    /// <summary>
    /// 现金记录通用视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class CashRecordsTopView<TReponsitory> : UniqueView<FlowAccount, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public CashRecordsTopView()
        {

        }

        public CashRecordsTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<FlowAccount> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.CashRecordsTopView>()
                   select new FlowAccount()
                   {
                       Currency = (Currency)entity.Currency,
                       Price = entity.Price,
                       Business = entity.Business,
                       Catalog = entity.Catalog,
                       Payer = entity.Payer,
                       Payee = entity.Payee,
                       ID = entity.ID,
                       CreateDate = entity.CreateDate,
                       WaybillID = entity.WaybillID,
                       Subject = entity.Subject,
                       Type = (AccountType)entity.Type,
                       OrderID = entity.OrderID,
                       Bank = entity.Bank,
                       FormCode = entity.FormCode,
                       AdminID = entity.AdminID,
                       ChangeDate = entity.ChangeDate,
                       ChangeIndex = entity.ChangeIndex,
                       Currency1 = (Currency)entity.Currency1,
                       ERate1 = entity.ERate1,
                       OriginIndex = entity.OriginIndex,
                       OriginalDate = entity.OriginalDate,
                       Price1 = entity.Price1,
                   };
        }
    }
}
