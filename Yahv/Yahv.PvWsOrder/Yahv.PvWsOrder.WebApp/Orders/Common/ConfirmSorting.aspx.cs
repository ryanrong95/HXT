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
    public partial class ConfirmSorting : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.LoadComboboxData();
                LoadData();
            }
        }

        private void LoadComboboxData()
        {
            //币种数据
            this.Model.currencyData = ExtendsEnum.ToArray(Currency.Unknown)
                .Select(item => new { Value = (int)item, Text = item.GetCurrency().ShortName + " " + item.GetDescription() });
            //原产地数据
            this.Model.originData = ExtendsEnum.ToArray<Origin>()
                .Select(item => new { Value = (int)item, Text = item + " " + item.GetDescription() });
            //包装类型数据
            this.Model.packageData = ExtendsEnum.ToArray<Package>()
                .Select(item => new { Value = (int)item, Text = item.GetPackage().Code + " " + item });
            //海关单位数据
            this.Model.unitData = ExtendsEnum.ToArray<LegalUnit>()
                .Select(item => new { UnitValue = item.GetUnit().Code, UnitText = item.GetUnit().Name });
            //支付方式
            this.Model.paymentType = ExtendsEnum.ToArray<Methord>()
                .Select(item => new { Value = item, Text = item.GetDescription() });
        }

        protected void LoadData()
        {
            var orderId = Request.QueryString["ID"];
            var order = Erp.Current.WsOrder.Orders[orderId];
            this.Model.Info = new
            {
                ClientName = order.OrderClient.Name,
                EnterCode = order.OrderClient.EnterCode,
                Supplier = order.OrderSupplier?.Name,
                SortingInfo = order.OrderInput?.Waybill?.Summary,
            };
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
                    ID = item?.ID,
                    InputID = item.InputID,
                    PartNumber = item?.Product.PartNumber,
                    Manufacturer = item?.Product.Manufacturer,
                    DateCode = item?.DateCode,
                    Origin = item?.Origin,
                    Quantity = item?.Quantity,
                    Currency = item?.Currency,
                    TotalPrice = item.TotalPrice,
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
                    //DeliveryOrigin = ((Origin)Enum.Parse(typeof(Origin), item?.Origin)).GetOrigin().Code,
                    DeliveryOrigin = item?.OriginGetCode,
                    DeliveryQuantity = item?.Quantity,
                    //Origin = ((Origin)Enum.Parse(typeof(Origin), item?.Origin)),
                    Origin = item?.Origin,
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

        /// <summary>
        /// 提交订单
        /// </summary>
        protected void SubmitOrder()
        {
            try
            {
                string orderID = Request.Form["orderID"];
                string currency = Request.Form["currency"];
                Order order = Erp.Current.WsOrder.Orders.Where(item => item.ID == orderID).FirstOrDefault();
                if (order == null)
                {
                    throw new Exception("订单" + orderID + "不存在");
                }
                //产品信息
                var products = Request.Form["products"].Replace("&quot;", "'").Replace("amp;", "");
                var productList = products.JsonTo<List<dynamic>>();
                List<OrderItem> orderItems = new List<OrderItem>();
                foreach (var item in productList)
                {
                    OrderItem orderItem = new OrderItem();
                    orderItem.ID = item.ID;
                    orderItem.OrderID = item.OrderID;
                    orderItem.InputID = item.InputID;
                    orderItem.Product = new Services.Models.CenterProduct { Manufacturer = item.Manufacturer, PartNumber = item.PartNumber };
                    orderItem.Origin = item.Origin;
                    orderItem.DateCode = item.DateCode;
                    orderItem.Quantity = item.Quantity;
                    orderItem.Currency = (Currency)int.Parse(currency);
                    orderItem.Unit = LegalUnit.个;
                    orderItem.TotalPrice = item.TotalPrice;
                    orderItem.UnitPrice = orderItem.TotalPrice / orderItem.Quantity;
                    orderItem.GrossWeight = 0.00m;
                    orderItem.Volume = 0.00m;
                    orderItem.Conditions = new OrderItemCondition().Json();
                    orderItems.Add(orderItem);
                }
                order.OperatorID = Erp.Current.ID;
                order.ModifyOrderItems(orderItems);
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}