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
    /// 本位币应收实收视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class VouchersCnyStatisticsView<TReponsitory> : QueryView<VoucherCnyStatistic, TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public VouchersCnyStatisticsView()
        {

        }

        public VouchersCnyStatisticsView(TReponsitory reponsitory) : base(reponsitory)
        {

        }
        protected override IQueryable<VoucherCnyStatistic> GetIQueryable()
        {
            var statistics = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.VouchersCnyStatisticsView>()
                .Where(item => item.Status == (int)GeneralStatus.Normal);

            var admins = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.AdminsTopView>();
            var enterprises = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>();

            return from entity in statistics
                   join payer in enterprises on entity.Payer equals payer.ID
                   join payee in enterprises on entity.Payee equals payee.ID
                   join _a in admins on entity.AdminID equals _a.ID into joinA
                   from a in joinA.DefaultIfEmpty()
                   select new VoucherCnyStatistic()
                   {
                       OrderID = entity.OrderID,
                       ApplicationID = entity.ApplicationID,
                       ReceivableID = entity.ReceivableID,
                       Business = entity.Business,
                       Subject = entity.Subject,
                       Catalog = entity.Catalog,
                       PayerID = entity.Payer,
                       PayeeID = entity.Payee,

                       OriginCurrency = (Currency)entity.OriginCurrency,
                       OriginPrice = entity.OriginPrice,
                       Currency = (Currency)entity.Currency,
                       LeftPrice = entity.LeftPrice,
                       RightPrice = entity.RightPrice,
                       ReducePrice = entity.ReducePrice,
                       LeftDate = entity.LeftDate,
                       OriginalDate = entity.OriginalDate,
                       RightDate = entity.RightDate,
                       AdminID = entity.AdminID,                      
                       Status = (GeneralStatus)entity.Status,
                       Rate = entity.Rate,

                       Admin = new Admin
                       {
                           ID = a.ID,
                           RealName = a.RealName,
                       }, 
                       Payer = new Enterprise
                       {
                           ID = payer.ID,
                           Name = payer.Name,
                       },
                       Payee = new Enterprise
                       {
                           ID = payee.ID,
                           Name = payee.Name,
                       },
                   };
        }
    }
}
