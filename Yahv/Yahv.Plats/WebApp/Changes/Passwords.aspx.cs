using System;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace WebApp.Changes
{
    public partial class Passwords : ErpSsoPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                Yahv.Erp.Current.ChangePassword(Request.Form["repassword"]);
                Easyui.Redirect("提示", "修改成功！", Request.Url.OriginalString, Sign.Info);
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Plat), "修改密码", $"修改密码：{Yahv.Erp.Current.ID}", $"站内修改密码!");
            }
            catch (Exception ex)
            {
                Easyui.Alert("操作提示", ex.Message, Sign.Error);
            }
        }
    }
}