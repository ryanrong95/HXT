using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace WebApp.examples.controls.crmplus
{
    public partial class demos_liufang : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Easyui.Alert("操作提示", $"提交成功!<br>联系人:{Request.Form["contact"]}；<br>收货地址{Request.Form["consignee"]}；<br/>标准型号：{Request.Form["standardPartNumber"]}；<br/>内部公司：{Request.Form["company"]}");
        }
    }
}