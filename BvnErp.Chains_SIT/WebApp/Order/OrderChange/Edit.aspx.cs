using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.OrderChange
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            string ID = Request.QueryString["ID"];
            var orderChange = new OrderChangeDetalView().Where(item => item.OrderID == ID).FirstOrDefault();

            this.Model.OrderChange = new
            {
                OrderID = orderChange.OrderID,
                orderChange.ContrNo,
                orderChange.EntryId,
                DDate = orderChange.DDate?.ToShortDateString(),
                CreateDate = orderChange.CreateDate.ToShortDateString(),//下单日期
                orderChange.Currency,
                orderChange.DecAmount,//报关总价
                orderChange.CustomsExchangeRate,
                TariffValue = orderChange.TariffValue == null ? 0M : orderChange.TariffValue,
                AddedValue = orderChange.AddedValue == null ? 0M : orderChange.AddedValue,
                TransPremiumInsurance = ConstConfig.TransPremiumInsurance,//运保杂
            }.Json();
        }

        /// <summary>
        /// 加载产品信息
        /// </summary>
        protected void ProductData()
        {
            var OrderID = Request.QueryString["ID"];
            var orderItems = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderItems.Where(x => x.OrderID == OrderID).AsQueryable();

            //前台显示
            Func<OrderItem, object> convert = item => new
            {
                item.ID,
                item.Category.HSCode,
                ProductName = item.Category.Name,  //归类后品名
                ProductModel = item.Model,//型号
                Origin = item.Origin,
                item.TotalPrice,
                ImportTax = item.ImportTax.Value,
                ImportRate = item.ImportTax.Rate,
                AddedValueRate = item.AddedValueTax.Rate,
                AddedValueTax = item.AddedValueTax.Value,

            };
            Response.Write(new { rows = orderItems.Select(convert).ToArray(), }.Json());

        }

    }
}