using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Forms;

namespace WebApp.Tests
{
    public partial class fuse_content : Yahv.Web.Forms.ClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Thread.Sleep(3000);
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Easyui.Ttop.Close("保存成功!", Yahv.Web.Controls.Easyui.AutoSign.Success);
        }
    }
}