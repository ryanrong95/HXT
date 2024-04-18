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
    public class PayeeLefts_Show_View : QueryRoll<PayeeLeftShow, PayeeLeftShow, PsOrderRepository>
    {
        #region 构造函数

        public PayeeLefts_Show_View()
        {
        }

        #endregion

        protected override IQueryable<PayeeLeftShow> GetIQueryable(Expression<Func<PayeeLeftShow, bool>> expression, params LambdaExpression[] expressions)
        {
            var clients = new Origins.ClientsOrigin(this.Reponsitory);
            var payeeLefts = new Origins.PayeeLeftsOrigin(this.Reponsitory);
            var payeeRights = new Origins.PayeeRightsOrigin(this.Reponsitory);
            var linq = from entity in payeeLefts
                       join client in clients on entity.PayerID equals client.ID
                       join right in payeeRights on entity.ID equals right.LeftID into rights
                       select new PayeeLeftShow
                       {
                           ID = entity.PayerID,
                           PayerName = client.Name,
                           Total = entity.Total,
                           CutDateIndex = entity.CutDateIndex,
                           ReceiptTotal = rights.Sum(t => t.Price),
                       };
            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<PayeeLeftShow, bool>>);
            }
            linq = linq.Where(expression).GroupBy(x => new { x.ID, x.PayerName, x.CutDateIndex }).Select(t => new PayeeLeftShow()
            {
                ID = t.Key.ID,
                PayerName = t.Key.PayerName,
                CutDateIndex = t.Key.CutDateIndex,
                Total = t.Sum(k => k.Total),
                ReceiptTotal = t.Sum(k => k.ReceiptTotal),
            });
            return linq;
        }

        protected override IEnumerable<PayeeLeftShow> OnReadShips(PayeeLeftShow[] results)
        {
            var linq = from entity in results
                       select new PayeeLeftShow
                       {
                           ID = entity.ID,
                           PayerName = entity.PayerName,
                           Total = entity.Total,
                           CutDateIndex = entity.CutDateIndex,
                           ReceiptTotal = entity.ReceiptTotal
                       };
            return linq;
        }
    }

    public class PayeeLeftShow : IUnique
    {
        public string ID { get; set; }

        public string PayerName { get; set; }

        /// <summary>
        /// 期号
        /// </summary>
        public int? CutDateIndex { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// 实收总金额
        /// </summary>
        public decimal? ReceiptTotal { get; set; }
    }
}
