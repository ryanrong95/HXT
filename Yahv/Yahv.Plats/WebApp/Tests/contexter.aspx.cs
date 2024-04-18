using System;
using Yahv.Web.Forms;

namespace WebApp.Tests
{
    public partial class contexter : ClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Easyui.AutoAlert("提交成功", Yahv.Web.Controls.Easyui.AutoSign.Success);
        }
    }
}