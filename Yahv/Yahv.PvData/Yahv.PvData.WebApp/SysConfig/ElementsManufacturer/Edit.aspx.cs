using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Services.Models;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.PvData.WebApp.SysConfig.ElementsManufacturer
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string id = Request.QueryString["id"];
                if (string.IsNullOrEmpty(id))
                {
                    this.Model.ElementsManufacturer = null;
                }
                else
                {
                    var em = Yahv.Erp.Current.PvData.ElementsManufacturers.Single(item => item.ID == id);
                    this.Model.ElementsManufacturer = new
                    {
                        em.ID,
                        em.Manufacturer,
                        em.MfrMapping
                    };
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string id = Request["id"];
            string mfr = Request["Manufacturer"].Trim();
            string mfrMapping = Request["MfrMapping"].Trim();
            if (!mfrMapping.EndsWith("牌"))
                mfrMapping = mfrMapping + "牌";

            var em = Yahv.Erp.Current.PvData.ElementsManufacturers.SingleOrDefault(item => item.ID == id) ??
                new YaHv.PvData.Services.Models.ElementsManufacturer();
            em.Manufacturer = mfr.TrimEnd('牌');
            em.MfrMapping = mfrMapping;
            em.SetAdmin(new Admin { ID = Yahv.Erp.Current.ID, RealName = Yahv.Erp.Current.RealName });
            em.Enter();

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