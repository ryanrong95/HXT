using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Wl;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Ccs.Services;

namespace WebApp.Order
{
    public partial class AddPaySupplier : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }


        public void LoadData()
        {
            var id = Request.QueryString["ID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[id];
            var paySupplierIds = new Needs.Ccs.Services.Views.OrderPayExchangeSuppliersView().Where(item => item.OrderID == id).Select(item => item.ClientSupplier.ID).ToArray();
            this.Model.suppliers = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.ClientSuppliers.Where(item => item.ClientID == order.Client.ID && !paySupplierIds.Contains(item.ID))
                .Select(item => new { Key = item.ID, Value = item.Name }).Json();

        }
        /// <summary>
        /// 保存数据
        /// </summary>
        protected void Save()
        {
            try
            {
                var orderId = Request.Form["OrderId"];
                var supplierID = Request.Form["SupplierID"];
                //生成单独运输数据
                //var orderVoyage = new Needs.Ccs.Services.Models.OrderVoyage(Needs.Ccs.Services.Enums.ModifyTypeEnum.Add, Needs.Ccs.Services.Enums.TransportType.CharterBus);
                var entity = new Needs.Ccs.Services.Models.OrderPayExchangeSupplier();
                var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[orderId];
                entity.OrderID = orderId;
                entity.ClientSupplier = new ClientSupplier()
                { ID = supplierID };
                entity.Enter();

                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception e)
            {
                Response.Write((new { success = false, message = "保存失败: " + e.Message }).Json());
            }
        }
    }
}
