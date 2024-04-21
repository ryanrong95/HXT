using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.StandardBrand
{
    public partial class Companys : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //this.Model.Entity=new Yahv.CrmPlus.Service.Views.Rolls.CompaniesRoll()
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {

            string brandid = Request.QueryString["brandid"];
            string id = Request.Form["Company"];
            try
            {
                var vs = new Service.Views.CompanynBrandView();
                if (vs.Any(item => item.BrandID == brandid && item.ID ==id ))
                {
                    Easyui.Dialog.Close("合作公司已存在!", Web.Controls.Easyui.AutoSign.Success);
                }
                else
                {
                    var entity = new nBrand();
                   // var entity = new Service.Models.Origins.vBrand();
                    entity.BrandID = brandid;
                    entity.Type = 0;
                    entity.EnterpriseID = id;
                    entity.CreatorID = Erp.Current.ID;
                    entity.EnterSuccess +=Entity_EnterSuccess; 
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