using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Company
{
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
            //    var id = Request.QueryString["ID"];
            //    this.Model.Entity = Erp.Current.CrmPlus.Companies[id];
            //}
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string name = Request["name"].Trim();
            string corporation = Request["corporation"].Trim();
            string regAddress = Request["regAddress"].Trim();
            string uscc = Request["uscc"].Trim();

            var entity = new Service.Models.Origins.Company();
            entity.CreatorID = Erp.Current.ID;
            entity.Name = name;
            entity.EnterpriseRegister = new Service.Models.Origins.EnterpriseRegister
            {
                Corperation = corporation,
                RegAddress = regAddress,
                Uscc = uscc,
            };
            entity.Repeat += Entity_Repeat;
          //  entity.EnterError += Entity_EnterError;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }

        private void Entity_Repeat(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Dialog.Close("已存在!", Web.Controls.Easyui.AutoSign.Error);
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
        //private void Entity_EnterError(object sender, Usually.ErrorEventArgs e)
        //{
        //    Easyui.Dialog.Close("保存失败!", Web.Controls.Easyui.AutoSign.Error);
        //}


        protected bool GetEnterpriseName()
        {
            bool result = false;
            var Name = Request.Form["Name"];
            if (!string.IsNullOrEmpty(Name))
            {
                result = Erp.Current.CrmPlus.Companies.Count(x => x.Name == Name) > 0;
            }
            return result;

        }
    }
}