using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Company
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                var id = Request.QueryString["ID"];
                this.Model.Entity = Erp.Current.CrmPlus.Companies[id];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string corporation = Request["corporation"].Trim();
            string regAddress = Request["regAddress"].Trim();
            string uscc = Request["uscc"].Trim();
            string id = Request["id"];
            
            var entity = Erp.Current.CrmPlus.Companies[id];
            //entity.Enterprise = new Service.Models.Origins.Enterprise
            //{
            //    ID = entity.ID,
            //    IsDraft = false,
            //};
            entity.EnterpriseRegister = new Service.Models.Origins.EnterpriseRegister
            {
                Corperation = corporation,
                RegAddress = regAddress,
                Uscc = uscc,
            };
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
        //private void Entity_EnterError(object sender, Usually.ErrorEventArgs e)
        //{
        //    Easyui.Dialog.Close("保存失败!", Web.Controls.Easyui.AutoSign.Error);
        //}


      
    }
}