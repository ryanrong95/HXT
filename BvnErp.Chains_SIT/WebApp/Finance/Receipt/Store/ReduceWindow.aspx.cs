using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Receipt.Store
{
    public partial class ReduceWindow : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.ReceivableID = Request.QueryString["ReceivableID"];
            this.Model.FeeTypeShowName = Request.QueryString["FeeTypeShowName"];
            this.Model.ReceivableAmount = Request.QueryString["ReceivableAmount"];

        }

        protected void Reduce()
        {
            try
            {
                string ReceivableID = Request.Form["ReceivableID"];
                string ReduceNumber = Request.Form["ReduceNumber"];

                var admin = new Needs.Ccs.Services.Views.AdminsTopView2().Where(t => t.OriginID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID).FirstOrDefault();

                Needs.Ccs.Services.Models.ReduceReceiptToYahv reduceReceiptToYahv = new Needs.Ccs.Services.Models.ReduceReceiptToYahv(
                                                                                        admin, ReceivableID, decimal.Parse(ReduceNumber));
                reduceReceiptToYahv.ExecuteReduce();

                Response.Write((new { success = "true", message = "提交成功", }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = "false", message = "提交失败" + ex.Message }).Json());
            }
        }


    }
}