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
    /// 信用还款账单
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    //public class CreditsRepayStatisticsView<TReponsitory> : QueryView<CreditsStatistic, TReponsitory>
    //     where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    //{
    //    protected override IQueryable<CreditsStatistic> GetIQueryable()
    //    {
    //        return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.CreditsRepayStatisticsView>()
    //               select new CreditsStatistic()
    //               {
    //                   OrderID = entity.OrderID,
    //                   Currency = (Currency)entity.Currency,
    //                   Business = entity.Business,
    //                   Subject = entity.Subject,
    //                   Catalog = entity.Catalog,
    //                   ReceivedID = entity.ReceivedID,
    //                   LeftDate = entity.LeftDate ?? Convert.ToDateTime("1900-01-01"),
    //                   LeftPrice = entity.LeftPrice ?? 0,
    //                   RightDate = entity.RightDate,
    //                   RightPrice = entity.RightPrice,
    //                   Payer = entity.Payer,
    //                   Payee = entity.Payee,
    //                   WaybillID = entity.WaybillID,
    //                   OriginalDate = entity.OriginalDate,
    //                   ChangeDate = entity.ChangeDate,
    //                   ReceivableID = entity.ReceivableID,
    //                   OriginalIndex = entity.OriginalIndex.Value,
    //                   ChangeIndex = entity.ChangeIndex.Value,
    //               };
    //    }
    //}
}
