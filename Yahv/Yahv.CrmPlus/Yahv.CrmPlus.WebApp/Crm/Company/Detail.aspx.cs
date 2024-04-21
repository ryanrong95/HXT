using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Company
{
    public partial class Detail : ErpParticlePage
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
            string name = Request["name"].Trim();
            string corporation = Request["corporation"].Trim();
            string regAddress = Request["regAddress"].Trim();
            string uscc = Request["uscc"].Trim();
            string id = Request["id"];

            var entity = Erp.Current.CrmPlus.Companies[id] ?? new Yahv.CrmPlus.Service.Models.Origins.Company() { };
            entity.CreatorID = Erp.Current.ID;
            entity.Name = name;
            entity.EnterpriseRegister = new Service.Models.Origins.EnterpriseRegister
            {
                IsSecret = false,
                IsInternational = false,
                Corperation = corporation,
                RegAddress = regAddress,
                Uscc = uscc,
            };
            entity.Enter();
        }
    }
}