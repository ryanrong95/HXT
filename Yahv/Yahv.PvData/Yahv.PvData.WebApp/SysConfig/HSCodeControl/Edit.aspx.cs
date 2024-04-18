using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.PvData.WebApp.SysConfig.HSCodeControl
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string hsCode = Request["hsCode"];

            var hsCodeControl = new YaHv.PvData.Services.Models.CustomsControl();
            hsCodeControl.Type = YaHv.PvData.Services.CustomsControlType.HSCode;
            hsCodeControl.HSCode = hsCode;
            hsCodeControl.Enter();

            Easyui.Dialog.Close("添加成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}