using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Srm.AgentBrands
{
    public partial class Admins : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.BrandID = Request.QueryString["BrandID"];
                this.Model.Admin = new YaHv.Csrm.Services.Views.ConAdminsView().Search(FixedRole.PM, FixedRole.PMa).Select(item => new
                {
                    item.ID,
                    item.RealName
                });
            }
        }
        object addresult;
        protected object Add()
        {
            try
            {
                string brandid = Request["BrandID"];
                string id = Request["AdminID"];
                var entity = new vBrand();
                entity.BrandID = brandid;
                entity.AdminID += id;
                entity.NameReapt += Entity_NameReapt;
                entity.EnterSuccess += Entity_EnterSuccess;
                entity.Enter();
                return addresult;
            }
            catch (Exception ex)
            {
                return new { success = false, message = ex.ToString() };
            }

        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            addresult = new { success = true, message = "新增成功" };
        }

        private void Entity_NameReapt(object sender, Usually.ErrorEventArgs e)
        {
            addresult = new { success = false, message = "已存在" };
        }

        protected object data()
        {
            string brandid = Request.QueryString["BrandID"];
            var entity = new vBrandsRoll().Where(item => item.BrandID == brandid);

            return this.Paging(entity, item => new
            {
                ID = item.ID,
                RealName = item.AdminRealName,
                RoleName = item.AdminRoleName
            });
        }
        bool AbandonSuccess = false;
        protected bool Del()
        {
            string id = Request["ID"];
            var entity = new vBrandsRoll()[id];
            entity.AbandonSuccess += Entity_AbandonSuccess;
            entity.Abandon();
            return AbandonSuccess;
        }

        private void Entity_AbandonSuccess(object sender, Usually.SuccessEventArgs e)
        {
            AbandonSuccess = true;
        }
    }
}