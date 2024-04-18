using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Controls.Easyui;
using Yahv.Web.Forms;

namespace WebApp
{
    public partial class msg : ClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //string url = "?";
            //Easyui.Redirect("操作提示", "密码已经过期，请您修改密码!", url, Sign.Warning);



            //IEnumerable<string>

            Easyui.AutoAlert("密码已经过期，请您修改密码!", AutoSign.Warning);

        }



    }
}