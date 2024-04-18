using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Linq;
using Yahv.Linq.Generic;
using Yahv.PsWms.SzMvc.Services.Enums;

namespace Yahv.PsWms.SzMvc.Services.Views.Roll
{
    public class PayerLefts_Show_View : QueryRoll<PayerLeftShow, PayerLeftShow, PsOrderRepository>
    {
        #region 构造函数

        public PayerLefts_Show_View()
        {
        }

        #endregion

        protected override IQueryable<PayerLeftShow> GetIQueryable(Expression<Func<PayerLeftShow, bool>> expression, params LambdaExpression[] expressions)
        {
            var linq = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.PayerLeftsTopView>()
                       select new PayerLeftShow
                       {
                           ID = entity.PayerID,
                           Total = entity.Total,
                           CutDateIndex = entity.CutDateIndex,
                       };
            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<PayerLeftShow, bool>>);
            }
            linq = linq.Where(expression).GroupBy(x => new { x.ID, x.CutDateIndex }).Select(t => new PayerLeftShow()
            {
                ID = t.Key.ID,
                CutDateIndex = t.Key.CutDateIndex,
                Total = t.Sum(k => k.Total),
            });
            return linq;
        }

        protected override IEnumerable<PayerLeftShow> OnReadShips(PayerLeftShow[] results)
        {
            var linq = from entity in results
                       select new PayerLeftShow
                       {
                           ID = entity.ID,
                           Total = entity.Total,
                           CutDateIndex = entity.CutDateIndex,
                       };
            return linq;
        }
    }

    public class PayerLeftShow : IUnique
    {
        public string ID { get; set; }

        /// <summary>
        /// 期号
        /// </summary>
        public int CutDateIndex { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal Total { get; set; }
    }
}
