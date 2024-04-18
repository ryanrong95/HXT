using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Services
{
    public class CustomsFeeService
    {
        public class CustomsTotalViewModel
        {
            public string ID { get; set; }
            public string OrderID { get; set; }
            /// <summary>
            /// 关税
            /// </summary>
            public decimal? TariffValue { get; set; }
            /// <summary>
            /// 增值税
            /// </summary>

            public decimal? AddedValue { get; set; }

            /// <summary>
            /// 税费合计
            /// </summary>
            public decimal TaxFeeTotal { get; set; }


        }

        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public CustomsTotalViewModel GetCustomsFeeSumOldSlow(string OrderID)
        {
            using (var reponsitory = new ScCustomsReponsitory())
            {
                var items_linq = from item in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                                 where item.OrderID.Contains(OrderID)
                                 select new
                                 {
                                     item.ID,
                                     item.OrderID,
                                     item.DeclaredQuantity,
                                     item.TotalPrice,
                                 };

                var items = items_linq.ToArray();
                var orderItemTaxes = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>();

                var importtax = from orderItem in items
                                join importTaxTax in orderItemTaxes on orderItem.ID equals importTaxTax.OrderItemID
                                where importTaxTax.Type == (int)Enums.CustomsRateType.ImportTax
                                group new { orderItem, importTaxTax } by new { orderItem.OrderID } into g
                                select new
                                {

                                    OrderID = g.Key.OrderID,
                                    Value = g.Sum(t => t.importTaxTax.Value)
                                };

                var addedValue = from orderItem in items
                                 join addedValuTax in orderItemTaxes on orderItem.ID equals addedValuTax.OrderItemID
                                 where addedValuTax.Type == (int)Enums.CustomsRateType.AddedValueTax
                                 group new { orderItem, addedValuTax } by new { orderItem.OrderID } into g
                                 select new
                                 {

                                     OrderID = g.Key.OrderID,
                                     Value = g.Sum(t => t.addedValuTax.Value)
                                 };

                var entity = new CustomsTotalViewModel();
                entity.OrderID = OrderID;
                entity.AddedValue = addedValue.FirstOrDefault().Value.Value;
                entity.TariffValue = importtax.FirstOrDefault().Value.Value;
                entity.TaxFeeTotal = (importtax.FirstOrDefault().Value.Value + addedValue.FirstOrDefault().Value.Value);
                return entity;
            }

        }

        public CustomsTotalViewModel GetCustomsFeeSum(string OrderID)
        {
            using (var reponsitory = new ScCustomsReponsitory())
            {
                var items_linq = from item in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                                 where item.OrderID== OrderID
                                 select new
                                 {
                                     item.ID,
                                     item.OrderID,
                                     item.DeclaredQuantity,
                                     item.TotalPrice,
                                 };

                var items = items_linq.ToArray();               

                var orderitemIDs = items.Select(t => t.ID).ToList();

                var importtax = from item in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>()
                                 where orderitemIDs.Contains(item.OrderItemID) && item.Type == (int)Enums.CustomsRateType.ImportTax
                                 select item.Value;

                var addedValue = from item in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>()
                                  where orderitemIDs.Contains(item.OrderItemID) && item.Type == (int)Enums.CustomsRateType.AddedValueTax
                                  select item.Value;

      
                var entity = new CustomsTotalViewModel();
                entity.OrderID = OrderID;
                entity.AddedValue = addedValue.Sum();
                entity.TariffValue = importtax.Sum();
                entity.TaxFeeTotal = (addedValue.Sum().Value + importtax.Sum().Value);
                return entity;
            }

        }
    }
}
