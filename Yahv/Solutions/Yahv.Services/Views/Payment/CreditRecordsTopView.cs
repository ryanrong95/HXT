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
    /// 信用消费记录
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class CreditRecordsTopView<TReponsitory> : UniqueView<FlowAccount, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public CreditRecordsTopView()
        {

        }

        public CreditRecordsTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<FlowAccount> GetIQueryable()
        {
            return null;
            //return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.CreditRecordsTopView>()
            //       select new FlowAccount()
            //       {
            //           Currency = (Currency)entity.Currency,
            //           Price = entity.Price,
            //           Business = entity.Business,
            //           Catalog = entity.Catalog,
            //           Payer = entity.Payer,
            //           Payee = entity.Payee,
            //           ID = entity.ID,
            //           CreateDate = entity.CreateDate,
            //           WaybillID = entity.WaybillID,
            //           Subject = entity.Subject,
            //           Type = (AccountType)entity.Type,
            //           OrderID = entity.OrderID,
            //           Bank = entity.Bank,
            //           FormCode = entity.FormCode,
            //           AdminID = entity.AdminID,
            //           ChangeDate = entity.ChangeDate,
            //           ChangeIndex = entity.ChangeIndex,
            //           Currency1 = (Currency)entity.Currency1,
            //           ERate1 = entity.ERate1,
            //           OriginIndex = entity.OriginIndex,
            //           OriginalDate = entity.OriginalDate,
            //           Price1 = entity.Price1,
            //       };
        }
    }
}
