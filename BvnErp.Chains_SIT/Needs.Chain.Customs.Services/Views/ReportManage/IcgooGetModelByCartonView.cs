using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class IcgooGetModelByCartonView : UniqueView<Models.IcgooModelDetail, ScCustomsReponsitory>
    {
        //实际是PackingID
        private string Carton;
        public IcgooGetModelByCartonView()
        { }

        public IcgooGetModelByCartonView(string Carton)
        {
            this.Carton = Carton;
        }

        internal IcgooGetModelByCartonView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<IcgooModelDetail> GetIQueryable()
        {
            throw new NotImplementedException();
        }

        public IQueryable<IcgooModelDetail> GetModelDetail()
        {
            var orderitems = GetOrderItems();
            var OrderItemIds = orderitems.Select(o => o.ID).ToArray();
            var OrderIDs = orderitems.Select(t => t.OrderID).Distinct().ToArray();
            var orderItemTaxes = new OrderItemTaxesView(this.Reponsitory).Where(t => OrderItemIds.Contains(t.OrderItemID)).ToArray();
            var categories = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>().Where(t => OrderItemIds.Contains(t.OrderItemID)).ToArray();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Where(t => OrderIDs.Contains(t.ID)).ToArray();
            var decheads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(t => t.CusDecStatus != "04" && OrderIDs.Contains(t.OrderID)).ToArray();
            var TaxFlows = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>().Where(t => decheads.Select(c => c.ID).Contains(t.DecTaxID)).ToArray();
            var invoices = GetInvoiceInfo(OrderIDs);

            //
            var CMXXX = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>().Where(t => t.OrderID == OrderIDs.FirstOrDefault())?.FirstOrDefault().IcgooOrder;

            var result = from orderitem in orderitems
                         join importTax in orderItemTaxes on new { OrderItemID = orderitem.ID, Type = Enums.CustomsRateType.ImportTax } equals new { importTax.OrderItemID, importTax.Type }
                         join addedValueTax in orderItemTaxes on new { OrderItemID = orderitem.ID, Type = Enums.CustomsRateType.AddedValueTax } equals new { addedValueTax.OrderItemID, addedValueTax.Type }
                         join category in categories on orderitem.ID equals category.OrderItemID
                         join order in orders on orderitem.OrderID equals order.ID

                         //left join
                         join dechead in decheads on orderitem.OrderID equals dechead.OrderID into decheadinto
                         from dechead in decheadinto.DefaultIfEmpty()
                         join importtax in TaxFlows on new { dechead.ID, Type = (int)Enums.DecTaxType.Tariff } equals new { ID = importtax.DecTaxID, Type = importtax.TaxType } into importtaxinto
                         from importtax in importtaxinto.DefaultIfEmpty()
                         join addedvaluetax in TaxFlows on new { dechead.ID, Type = (int)Enums.DecTaxType.AddedValueTax } equals new { ID = addedvaluetax.DecTaxID, Type = addedvaluetax.TaxType } into addedvaluetaxinto
                         from addedvaluetax in addedvaluetaxinto.DefaultIfEmpty()
                         join invoice in invoices on orderitem.OrderID equals invoice.OrderID into invoiceinto
                         from invoice in invoiceinto.DefaultIfEmpty()

                         select new IcgooModelDetail
                         {
                             ID = orderitem.ID,
                             CustomsUnionID = orderitem.ProductUniqueCode,
                             //
                             ModelStatus = ((Enums.OrderStatus)order.OrderStatus).GetDescription(),
                             InvoiceStatus = ((Enums.InvoiceStatus)order.InvoiceStatus).GetDescription(),
                             ImportTaxCode = importtax?.TaxNumber,
                             AddedValueTaxCode = addedvaluetax?.TaxNumber,

                             Model = orderitem.Model,
                             ProductName = orderitem.Name,
                             Manufacturer = orderitem.Manufacturer,
                             HSCode = category.HSCode,
                             Origin = orderitem.Origin,
                             Qty = orderitem.Quantity,
                             AgentUnitPrice = orderitem.UnitPrice,
                             AgentTotalPrice = orderitem.TotalPrice,
                             Currency = order.Currency,
                             CustomsExchangeRate = order.CustomsExchangeRate.Value,
                             TariffRate = importTax.Rate,
                             RealTariffRate = importTax.ReceiptRate,
                             AddTaxRate = addedValueTax.Rate,
                             RealAddTaxRate = addedValueTax.ReceiptRate,
                             TaxName = category.TaxName,
                             TaxCode = category.TaxCode,
                             OrderID = orderitem.OrderID,

                             DeclareDate = dechead != null ? dechead.DDate.Value : DateTime.MinValue,
                             ContractNo = dechead != null ? dechead.ContrNo : "",
                             EntryID = dechead != null ? dechead.EntryId : "",
                             CustomsOrderID = CMXXX,

                             InvoiceType = invoice != null ? invoice.InvoiceType.GetDescription() : "",
                             InvoiceTaxRate = invoice != null ? invoice.InvoiceTaxRate : 0M,
                             InvoiceDate = invoice != null ? invoice.InvoiceDate : DateTime.MinValue,
                             InvoiceNo = invoice != null ? invoice.InvoiceNo : "",
                         };

            return result.AsQueryable();
        }

        public Layer.Data.Sqls.ScCustoms.OrderItems[] GetOrderItems()
        {
            return (from packitem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PackingItems>()
                    join sort in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>() on packitem.SortingID equals sort.ID
                    join orderitem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on sort.OrderItemID equals orderitem.ID
                    where packitem.PackingID == this.Carton
                    select orderitem
                    ).ToArray();
        }

        public List<ProfitInvoiceInfo> GetInvoiceInfo(string[] OrderIDs)
        {
            var results = new List<ProfitInvoiceInfo>();
            var invoices = (from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>()
                            join invoice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>() on item.InvoiceNoticeID equals invoice.ID
                            select new ProfitInvoiceInfo
                            {
                                InvoiceType = (Enums.InvoiceType)invoice.InvoiceType,
                                InvoiceTaxRate = invoice.InvoiceTaxRate,
                                InvoiceDate = invoice.UpdateDate,
                                InvoiceNo = item.InvoiceNo,
                                OrderID = item.OrderID
                            }).ToList();

            foreach (var orderid in OrderIDs)
            {
                var invoice = invoices.Where(t => t.OrderID.Contains(orderid))?.FirstOrDefault();
                if (invoice != null)
                {
                    results.Add(new ProfitInvoiceInfo
                    {
                        InvoiceType = invoice.InvoiceType,
                        InvoiceTaxRate = invoice.InvoiceTaxRate,
                        InvoiceDate = invoice.InvoiceDate,
                        InvoiceNo = invoice.InvoiceNo,
                        OrderID = orderid
                    });
                }
            }

            return results;
        }
    }
}
