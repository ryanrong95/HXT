using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// OrderReceipts 行转列
    /// </summary>
    public class PaymentExchangeRateView : UniqueView<Models.PaymentExchangeRate, ScCustomsReponsitory>
    {
        public PaymentExchangeRateView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public PaymentExchangeRateView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.PaymentExchangeRate> GetIQueryable()
        {
            var result = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>()
                         join d in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>() on c.PayExchangeApplyID equals d.ID
                         select new Models.PaymentExchangeRate
                         {
                             OrderID = c.OrderID,
                             ExchangeRate = d.ExchangeRate,
                         };

            return result;
        }
    }
}
