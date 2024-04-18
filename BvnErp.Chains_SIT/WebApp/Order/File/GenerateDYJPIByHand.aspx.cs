using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.File
{
    public partial class GenerateDYJPIByHand : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected void GeneratePIFiles()
        {
            try
            {
                string OrderIDs = Request.Form["OrderIDs"];
                string OrderCompany = Request.Form["OrderCompany"];

                var orderids = OrderIDs.Split(',').ToList();

                if (string.IsNullOrEmpty(OrderCompany)) {

                    Response.Write((new { success = false, message = "公司名称不能为空" }).Json());
                    return;
                }
                if (string.IsNullOrEmpty(OrderIDs) || orderids.Count() < 1)
                {

                    Response.Write((new { success = false, message = "订单号错误" }).Json());
                    return;
                }

                foreach (var t in orderids)
                {
                    PIGener pIGener = new PIGener(t, OrderCompany);
                    pIGener.Execute();
                }

                Response.Write((new { success = false, message = "生成成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "生成失败：" + ex.Message }).Json());
            }
        }
    }
}