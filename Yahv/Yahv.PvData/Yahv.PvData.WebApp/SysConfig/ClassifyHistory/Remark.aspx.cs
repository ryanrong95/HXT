using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.PvData.WebApp.SysConfig.ClassifyHistory
{
    public partial class Remark : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string id = Request["id"];
            var other = Yahv.Erp.Current.PvData.Others[id];

            this.Model.Other = new
            {
                other.PartNumber,
                other.Manufacturer,
                other.Summary
            };
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string id = Request["id"];
            var other = Yahv.Erp.Current.PvData.Others[id];

            var ch = new YaHv.PvData.Services.Models.ClassifiedHistory()
            {
                PartNumber = other.PartNumber,
                Manufacturer = other.Manufacturer,
                Summary = Request["summary"]
            };
            ch.Remark();

            Easyui.Dialog.Close("备注成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}