using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Srm.Dishonests
{
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Suppliers = Erp.Current.CrmPlus.Suppliers.Select(item => new
                {
                    EnterpriseID = item.ID,
                    Name = item.Name
                });
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string EnterpriseID = Request.Form["Supplier"];
            string reason = Request.Form["Reason"];
            string OccurTime = Request.Form["OccurTime"];
            string code = Request.Form["Code"];
            string summary = Request.Form["Summary"];
            Dishonest entity = new Dishonest();
            entity.EnterpriseID = EnterpriseID;
            entity.Reason = reason;
            entity.Code = code;
            entity.OccurTime = DateTime.Parse(OccurTime);
            entity.Summary = summary;
            entity.CreatorID = Erp.Current.ID;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as Dishonest;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"新增Supplier失信记录");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}