using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApp.Srm.AgentBrands
{
    public partial class Add : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
            //    this.Model.Admin = new YaHv.Csrm.Services.Views.ConAdminsView().Search(FixedRole.PM,FixedRole.PMa).Select(item => new { item.ID, item.RealName });
            //}
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string brandid = Request["standardBrand"];
            string supplierid = Request.QueryString["EnterpriseID"];
            //string admin = Request["Admins"];
            //string[] adminids = new string[] { };
            //if (!string.IsNullOrWhiteSpace(admin))
            //{
            //    adminids = admin.Split(',');
            //}
            var entity = new nBrand();
            entity.BrandID = brandid;
            entity.EnterpriseID = supplierid;
            entity.Reapt += Entity_Reapt;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();

        }

        private void Entity_Reapt(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Ttop.Close("已存在!", Web.Controls.Easyui.AutoSign.Error);
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            Easyui.Ttop.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}