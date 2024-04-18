using Layer.Data.Sqls.ScCustoms;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Payment
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                this.LoadData();
            }
        }

        protected void LoadData()
        {
            this.Model.OrderData = "".Json();
            string OrderID = Request.QueryString["ID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders.Where(item => item.ID == OrderID).FirstOrDefault();
            if (order != null)
            {
                this.Model.OrderData = new
                {
                    OrderID = order.ID,
                    ClientCode = order.Client.ClientCode,
                    Salesman = order.Client.ServiceManager.RealName,
                    Merchandiser = order.Client.Merchandiser.RealName,
                }.Json();
            }

        }

        protected void data()
        {
            string OrderID = Request.QueryString["ID"];
            //查询订单付款通知明确
            var Applys = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.PaymentApply.Where(item => item.OrderID == OrderID);
            //查询订单付款通知
            Func<Needs.Ccs.Services.Models.PaymentApply, object> linq = item => new
            {
                ID = item.ID,
                PayeeName = item.PayeeName,                
                Amount = item.Amount,
                Currency = item.Currency,
                FeeDesc = item.FeeDesc,
                Status = item.ApplyStatus.GetDescription(),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                PayDate = item.PayDate.ToString("yyyy-MM-dd"),
            };
            Response.Write(new
            {
                rows = Applys.Select(linq).ToArray()
            }.Json());
        }

        protected void Cancel()
        {
            try
            {
                string ApplyID = Request.Form["ApplyID"];
                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var apply = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.PaymentApply.Where(item => item.ID == ApplyID).FirstOrDefault();
                apply.SetOperator(admin);
                apply.Abandon();
                Response.Write((new { success = true, message = "删除成功"}).Json());
            }
            catch(Exception ex)
            {
                Response.Write((new { success = false, message = "删除失败" + ex.Message }).Json());
            }
        }
    }
}