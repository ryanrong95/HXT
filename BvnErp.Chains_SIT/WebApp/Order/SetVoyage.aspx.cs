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
    public partial class SetVoyage : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 保存数据
        /// </summary>
        protected void Save()
        {
            try
            {
                var orderId = Request.Form["OrderId"];
                var transportRequire= Request.Form["TransportRequire"];
                //生成单独运输数据
                //var orderVoyage = new Needs.Ccs.Services.Models.OrderVoyage(Needs.Ccs.Services.Enums.ModifyTypeEnum.Add, Needs.Ccs.Services.Enums.TransportType.CharterBus);
                var orderVoyage = new Needs.Ccs.Services.Models.OrderVoyage();
                var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[orderId];
                orderVoyage.Order = order;
                orderVoyage.Type = Needs.Ccs.Services.Enums.OrderSpecialType.CharterBus;
                orderVoyage.Summary = System.Web.HttpUtility.UrlDecode(transportRequire);
                orderVoyage.EnterSuccess += OrderVoyage_EnterSuccess;
                orderVoyage.Enter();

                Response.Write((new { success = true, message = "设置包车保存成功" }).Json());
            }
            catch (Exception e)
            {
                Response.Write((new { success = false, message = "设置包车保存失败: " + e.Message }).Json());
            }
        }

        private void OrderVoyage_EnterSuccess(object sender, SuccessEventArgs e)
        {
            var orderVoyage = (OrderVoyage)e.Object;
            var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
            var summary = "跟单员[" + admin.RealName + "]设置包车运输";
            orderVoyage.Order.Log(admin, summary);
        }
    }
}