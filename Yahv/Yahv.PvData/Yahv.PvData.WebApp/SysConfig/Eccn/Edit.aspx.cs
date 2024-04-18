using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Utils.Converters.Contents;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.PvData.WebApp.SysConfig.Eccn
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.LastOrigin = new string[] { "", "Americas", "Asia", "EMEA" }
                .Select(item => new
                {
                    value = item,
                    text = item,
                });

                string id = Request.QueryString["id"];
                if (string.IsNullOrEmpty(id))
                {
                    this.Model.Eccn = null;
                }
                else
                {
                    var eccn = Yahv.Erp.Current.PvData.Eccns.SingleOrDefault(item => item.ID == id);
                    this.Model.Eccn = new
                    {
                        eccn.ID,
                        eccn.PartNumber,
                        eccn.Manufacturer,
                        eccn.Code,
                        eccn.LastOrigin
                    };
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string id = Request["id"];
            string partnumber = Request["partnumber"].Trim();
            string manufacturer = Request["manufacturer"].Trim();
            string code = Request["code"].Trim();
            string lastOrigin = Request["lastOrigin"].Trim();

            if (string.IsNullOrEmpty(id))
            {
                id = string.Concat(partnumber, manufacturer.ToLower(), lastOrigin).MD5();
            }

            var eccn = new Yahv.Services.Models.Eccn()
            {
                ID = id,
                PartNumber = partnumber.Trim(),
                Manufacturer = manufacturer.Trim(),
                Code = code.Trim(),
                LastOrigin = lastOrigin.Trim(),
            };

            Yahv.Erp.Current.PvData.Eccns.Enter(eccn);

            if (string.IsNullOrEmpty(id))
            {
                Easyui.Dialog.Close("添加成功!", Web.Controls.Easyui.AutoSign.Success);
            }
            else
            {
                Easyui.Dialog.Close("编辑成功!", Web.Controls.Easyui.AutoSign.Success);
            }
        }
    }
}