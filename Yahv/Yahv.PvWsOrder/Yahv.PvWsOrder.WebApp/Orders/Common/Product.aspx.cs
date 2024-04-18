using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Orders.Common
{
    public partial class Product : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            var orderId = Request.QueryString["ID"];
            var order = Erp.Current.WsOrder.Orders[orderId];
            this.Model.Info = new
            {
                ClientName = order.OrderClient.Name,
                EnterCode = order.OrderClient.EnterCode,
                SettlementCurrency = order.SettlementCurrency.GetDescription(),
                Supplier = order.OrderSupplier?.Name,
                IsPayForGoods = order.OrderInput?.IsPayCharge,
                IsReciveCharge = order.OrderOutput?.IsReciveCharge,
            };
            //代收货供应商收款人
            if (order.OrderInput != null)
            {
                this.Model.Beneficiarie = Erp.Current.WsOrder.SupplierPayees
                    .Where(item => item.ID == order.OrderInput.BeneficiaryID).FirstOrDefault()?.RealEnterpriseName;
            }

            //内部公司及受益人
            var company = Erp.Current.WsOrder.Companys.Where(item => item.ID == order.PayeeID).FirstOrDefault();
            this.Model.company = company?.Name;
            //var beneficiary = Erp.Current.WsOrder.BeneficiariesAll.Where(item => item.ID == order.BeneficiaryID).FirstOrDefault();
            //this.Model.companyBeneficiarie = beneficiary?.Account;
            var beneficiary = Erp.Current.WsOrder.Payees.Where(item => item.ID == order.BeneficiaryID).FirstOrDefault();
            this.Model.companyBeneficiarie = beneficiary?.Account;

            //下单信息
            var orderitems = Erp.Current.WsOrder.OrderItems.Where(o => o.OrderID == orderId);
            var orderitem = orderitems
               .Where(o => o.Type == OrderItemType.Normal).OrderBy(o => o.Product.PartNumber).AsEnumerable();
            var result2 = new List<object>();
            foreach (var item in orderitem)
            {
                result2.Add(new
                {
                    OrderItemID = item?.ID,
                    InputID = item.InputID,
                    PartNumber = item?.Product.PartNumber,
                    Manufacturer = item?.Product.Manufacturer,
                    DateCode = item?.DateCode,
                    Origin = item?.OriginGetCode,
                    Quantity = item?.Quantity,
                    Currency = item?.Currency.GetDescription(),
                    UnitPrice=item?.UnitPrice,
                    TotalPrice=item?.TotalPrice,
                });
            };
            this.Model.itemData = new { rows = result2.ToArray(), total = result2.Count() };

            //到货信息
            var delivery = orderitems
                .Where(o => o.Type == OrderItemType.Modified).OrderBy(o => o.Product.PartNumber).AsEnumerable();
            var result = new List<object>();
            foreach (var item in delivery)
            {
                result.Add(new
                {
                    DeliveryOrderItemID = item?.ID,
                    DeliveryInputID = item.InputID,
                    DeliveryPartNumber = item?.Product.PartNumber,
                    DeliveryManufacturer = item?.Product.Manufacturer,
                    DeliveryDateCode = item?.DateCode,
                    DeliveryOrigin = item?.OriginGetCode,
                    DeliveryQuantity = item?.Quantity,
                    DeliveryCurrency = item?.Currency.GetDescription(),
                    DeliveryUnitPrice = item?.UnitPrice,
                    DeliveryTotalPrice = item?.TotalPrice,
                });
            };
            this.Model.deliveryData = new { rows = result.ToArray(), total = result.Count() };
        }

        protected object data()
        {
            return new
            {
                rows = new List<object>(),
                total = 0,
            }.Json();
        }

    }
}