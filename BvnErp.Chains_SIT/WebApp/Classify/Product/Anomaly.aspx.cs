using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Classify.Product
{
    public partial class Anomaly : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 归类异常
        /// </summary>
        protected void ClassifyAnomaly()
        {
            try
            {
                string id = Request.Form["ID"];
                string reason = Request.Form["Reason"];
                var declarant = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                var product = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.ClassifyProductsAll[id];
                product.AnomalyReason = "报关员【" + declarant.RealName + "】：" + reason;
                product.Admin = declarant;

                var step = (ClassifyStep)Enum.Parse(typeof(ClassifyStep), Request.Form["From"]);
                var classify = ClassifyFactory.Create(step, product);
                classify.Anomaly();

                Response.Write((new { success = true, message = "归类异常处理成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "归类异常处理失败：" + ex.Message }).Json());
            }
        }
    }
}