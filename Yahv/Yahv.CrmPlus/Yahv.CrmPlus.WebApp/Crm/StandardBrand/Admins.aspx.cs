using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.WebApp.Crm.StandardBrand
{
    public partial class Admins : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
               // var entity = new Service.Views.Rolls.BrandsRoll()[Request.QueryString["id"]];
                //this.Model.BrandID = entity.ID;
                //this.Model.BrandName = entity.Name;
                this.Model.Admins = new YaHv.CrmPlus.Services.Views.Rolls.AdminsRolesAllRoll().Where(item => item.RoleID == FixedRole.PM.GetFixedID() || item.RoleID == FixedRole.PMa.GetFixedID() || item.RoleID == FixedRole.FAE.GetFixedID() &&item.ID!=null).Select(item => new
                {
                    ID = item.ID,
                    Text = $"{item.RealName}-{item.RoleName}"
                });
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            string brandid = Request.QueryString["brandid"];
            string adminid = Request.Form["AdminID"];
            try
            {
                var vs = new Service.Views.Rolls.vBrandsRoll();
                if (vs.Any(item => item.BrandID == brandid && item.AdminID == adminid))
                {
                    Easyui.Dialog.Close("负责人已存在!", Web.Controls.Easyui.AutoSign.Success);
                }
                else
                {
                    var admin = new AdminsAllRoll()[adminid];
                    var entity = new Service.Models.Origins.vBrand();
                    entity.BrandID = brandid;
                    entity.AdminID = adminid;
                    entity.RoleID = admin.RoleID;
                    entity.EnterSuccess += Entity_EnterSuccess;
                    entity.Enter();

                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }


    }
}