using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;
using YaHv.CrmPlus.Services.Models.Origins;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Credits
{
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var company = Erp.Current.CrmPlus.Companies[Request.QueryString["makerid"]];
                this.Model.CompanyName = company.Name;
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var entity = new FlowCredit();
            bool isCredit = bool.Parse(Request.QueryString["IsCredit"]);
            entity.MakerID = Request.QueryString["makerid"];
            entity.TakerID = Request.QueryString["TakerID"];
            entity.Currency = (Currency)int.Parse(Request.Form["Currency"]);
            decimal price = Math.Abs(decimal.Parse(Request.Form["Price"]));
            entity.Price = isCredit ? price : -price;
            entity.Catalog = Request.Form["Catalog"].Trim();
            entity.Subject = Request.Form["Subject"].Trim();
            entity.Summary = Request.Form["Summary"];
            entity.Type = CreditType.GrantingParty;
            entity.CreatorID = Erp.Current.ID;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();

        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as FlowCredit;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"新增信用流水:{entity.ID}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}