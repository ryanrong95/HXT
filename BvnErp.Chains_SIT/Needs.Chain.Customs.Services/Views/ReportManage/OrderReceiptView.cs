using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 行转列 by yeshuangshuang
    /// </summary>
    public class OrderReceiptView : UniqueView<Models.BillSummary, ScCustomsReponsitory>
    {
        public OrderReceiptView()
        {
        }
        public OrderReceiptView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.BillSummary> GetIQueryable()
        {
            var orderreceipts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>();
            var result = from receipts in orderreceipts
                         where receipts.Type == (int)Enums.OrderReceiptType.Receivable
                         && receipts.Status == (int)Enums.Status.Normal
                         group receipts by new { receipts.OrderID } into m
                         select new Models.BillSummary
                         {
                             OrderID = m.Key.OrderID,
                             Tariff = m.Where(n => n.FeeType == (int)Enums.OrderFeeType.Tariff).Count() == 0 ? 0 : (m.Where(n => n.FeeType == (int)Enums.OrderFeeType.Tariff).Sum(n => n.Amount)),
                             AddedValueTax = m.Where(n => n.FeeType == (int)Enums.OrderFeeType.AddedValueTax).Count() == 0 ? 0 : (m.Where(n => n.FeeType == (int)Enums.OrderFeeType.AddedValueTax).Sum(n => n.Amount)),
                             AgencyFee = m.Where(n => n.FeeType == (int)Enums.OrderFeeType.AgencyFee).Count() == 0 ? 0 : (m.Where(n => n.FeeType == (int)Enums.OrderFeeType.AgencyFee).Sum(n => n.Amount)),
                             Incidental = m.Where(n => n.FeeType == (int)Enums.OrderFeeType.Incidental).Count() == 0 ? 0 : (m.Where(n => n.FeeType == (int)Enums.OrderFeeType.Incidental).Sum(n => n.Amount)),
                         };

            return result;
        }
    }
}
