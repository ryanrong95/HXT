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
    public class OrderReceiptsC2RView : UniqueView<Models.ReceiveDetail, ScCustomsReponsitory>
    {
        public OrderReceiptsC2RView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public OrderReceiptsC2RView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ReceiveDetail> GetIQueryable()
        {
            var result = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>()
                         where c.Type == (int)OrderReceiptType.Received
                         group c by new { c.FinanceReceiptID, c.OrderID } into m
                         select new Models.ReceiveDetail
                         {
                             FinanceReceiptID = m.Key.FinanceReceiptID,
                             OrderID = m.Key.OrderID,
                             Tariff = m.Where(n => n.FeeType == (int)Enums.OrderFeeType.Tariff).Count() == 0 ? 0 : -(m.Where(n => n.FeeType == (int)Enums.OrderFeeType.Tariff).Sum(n => n.Amount)),
                             ExciseTax = m.Where(n => n.FeeType == (int)Enums.OrderFeeType.ExciseTax).Count() == 0 ? 0 : -(m.Where(n => n.FeeType == (int)Enums.OrderFeeType.ExciseTax).Sum(n => n.Amount)),
                             AddedValueTax = m.Where(n => n.FeeType == (int)Enums.OrderFeeType.AddedValueTax).Count() == 0 ? 0 : -(m.Where(n => n.FeeType == (int)Enums.OrderFeeType.AddedValueTax).Sum(n => n.Amount)),
                             GoodsAmount = m.Where(n => n.FeeType == (int)Enums.OrderFeeType.Product).Count() == 0 ? 0 : -(m.Where(n => n.FeeType == (int)Enums.OrderFeeType.Product).Sum(n => n.Amount)),
                             AgencyFee = m.Where(n => n.FeeType == (int)Enums.OrderFeeType.AgencyFee).Count() == 0 ? 0 : -(m.Where(n => n.FeeType == (int)Enums.OrderFeeType.AgencyFee).Sum(n => n.Amount)),
                             Incidental = m.Where(n => n.FeeType == (int)Enums.OrderFeeType.Incidental).Count() == 0 ? 0 : -(m.Where(n => n.FeeType == (int)Enums.OrderFeeType.Incidental).Sum(n => n.Amount)),
                         };

            return result;

        }
    }
}
