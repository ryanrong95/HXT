using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Receipt.Order
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void LoadData()
        {
            var orderId = Request.QueryString["ID"];

            this.Model.orderReceivedDetail = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderReceivedDetails[orderId].Json();
        }

        protected void Submit()
        {
            try
            {
                var orderId = Request.Form["OrderId"];
                var noticeId = Request.Form["NoticeId"];
                var clientId = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.MyReceiptNotices[noticeId].Client.ID;
                var Sources = Request.Form["Sources"].Replace("&quot;", "\'").Replace("amp;", "");
                var model = Sources.JsonTo<dynamic>();

                foreach (var item in model)
                {
                    var received = new OrderReceived();
                    received.ReceiptNoticeID = noticeId;
                    received.ClientID = clientId;
                    received.OrderID = orderId;
                    received.FeeSourceID = item.FeeSourceID;
                    received.FeeType = (OrderFeeType)Enum.Parse(typeof(OrderFeeType), (string)item.FeeType);
                    received.Amount = item.Amount;
                    received.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                    received.Enter();
                }

                Response.Write((new { success = true, message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "提交失败" + ex.Message }).Json());
            }
        }
    }
}