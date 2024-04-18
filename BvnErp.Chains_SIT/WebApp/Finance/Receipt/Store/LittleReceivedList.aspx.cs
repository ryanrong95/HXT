using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Receipt.Store
{
    public partial class LittleReceivedList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            string ReceivableID = Request.QueryString["ReceivableID"];

            var receiveds = new Needs.Ccs.Services.Views.StoreReceiptReceivedReceivableIDView().GetList(ReceivableID);

            Func<Needs.Ccs.Services.Views.StoreReceiptReceivedReceivableIDViewModel, object> convert = item => new
            {
                ReceivedID = item.ReceivedID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                ReceiptAdminName = item.RealName,
                TypeName = !string.IsNullOrEmpty(item.Subject) ? item.Subject : (!string.IsNullOrEmpty(item.Catalog) ? item.Catalog : string.Empty),
                Price = item.Price,
                AccountTypeInt = item.AccountType,
            };

            Response.Write(new
            {
                rows = receiveds.Select(convert).ToArray(),
                
            }.Json());
        }


        protected void CancelReduce()
        {
            try
            {
                string ReceivedID = Request.Form["ReceivedID"];

                var admin = new Needs.Ccs.Services.Views.AdminsTopView2().Where(t => t.OriginID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID).FirstOrDefault();

                Needs.Ccs.Services.Models.ReduceReceiptToYahv reduceReceiptToYahv = new Needs.Ccs.Services.Models.ReduceReceiptToYahv(admin, ReceivedID);
                reduceReceiptToYahv.CancelReduce();

                Response.Write((new { success = "true", message = "提交成功", }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = "false", message = "提交失败" + ex.Message }).Json());
            }
        }

    }
}