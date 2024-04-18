using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 每个订单的增值税总额
    /// </summary>
    public class AddedValueTaxTotalView
    {
        ScCustomsReponsitory _reponsitory { get; set; }

        public AddedValueTaxTotalView()
        {
            this._reponsitory = new ScCustomsReponsitory();
        }

        public AddedValueTaxTotalView(ScCustomsReponsitory reponsitory)
        {
            this._reponsitory = reponsitory;
        }

        public List<AddedValueTaxTotalViewModel> GetAddedValueTaxTotal(string[] orderIDs)
        {
            var orderItems = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
            var orderItemTaxes = this._reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>();

            var linq = from orderItem in orderItems
                       join orderItemTax in orderItemTaxes on orderItem.ID equals orderItemTax.OrderItemID
                       where orderIDs.Contains(orderItem.OrderID)
                          && orderItem.Status == (int)Enums.Status.Normal
                          && orderItemTax.Status == (int)Enums.Status.Normal
                          && orderItemTax.Type == (int)Enums.CustomsRateType.AddedValueTax
                       group orderItemTax by new { orderItem.OrderID } into g
                       select new AddedValueTaxTotalViewModel
                       {
                           OrderID = g.Key.OrderID,
                           TotalAmount = g.Sum(t => t.Value ?? 0),
                       };

            return linq.ToList();
        }

    }

    public class AddedValueTaxTotalViewModel
    {
        /// <summary>
        /// OrderID
        /// </summary>
        public string OrderID { get; set; } = string.Empty;

        /// <summary>
        /// 增值税总金额
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
}
