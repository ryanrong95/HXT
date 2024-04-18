using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.PvWsPortal.Services.Models;

namespace Yahv.PvWsPortal.Services.Views
{
    /// <summary>
    /// 实时汇率查询视图
    /// </summary>
    public class FerobocView : UniqueView<Feroboc, PvDataReponsitory>
    {
        public FerobocView()
        {

        }

        /// <summary>
        /// 查询结果集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Feroboc> GetIQueryable()
        {
            var ferobocs = Reponsitory.ReadTable<Layers.Data.Sqls.PvData.Feroboc>();

            return from feroboc in ferobocs
                   group feroboc by new { feroboc.Type, feroboc.Currency } into g1
                   select new
                   {
                       EntityByCurrency = (from feroboc2 in ferobocs
                                           where feroboc2.Type == g1.Key.Type && feroboc2.Currency == g1.Key.Currency
                                           orderby feroboc2.PublishDate descending
                                           select feroboc2).FirstOrDefault(),
                       Type = g1.Key.Type,                    
                       Currency = g1.Key.Currency,
                   } into entityByCurrency
                   select new Feroboc
                   {
                       ID = entityByCurrency.EntityByCurrency.ID,
                       Type = entityByCurrency.Type,
                       Currency = entityByCurrency.Currency,
                       Xhmr = entityByCurrency.EntityByCurrency.Xhmr,
                       Xcmr = entityByCurrency.EntityByCurrency.Xcmr,
                       Xhmc = entityByCurrency.EntityByCurrency.Xhmc,
                       Xcmc = entityByCurrency.EntityByCurrency.Xcmc,
                       Zhzsj = entityByCurrency.EntityByCurrency.Zhzsj,
                       CreateDate = entityByCurrency.EntityByCurrency.CreateDate,
                       PublishDate = entityByCurrency.EntityByCurrency.PublishDate,
                   };
        }

    }
}
