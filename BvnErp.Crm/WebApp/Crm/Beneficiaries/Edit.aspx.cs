using Needs.Erp.Generic;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Beneficiaries
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PageInit();
            }
        }

        void PageInit()
        {
            this.Model.CompanyData = Needs.Erp.ErpPlot.Current.ClientSolutions.Companys.Where(item => item.Type == NtErp.Crm.Services.Enums.CompanyType.plot).
                Select(item => new { item.ID, item.Name }).OrderBy(item => item.Name).Json();
            string id = Request.QueryString["id"];
            this.Model.AllData = Needs.Erp.ErpPlot.Current.ClientSolutions.Beneficiaries[id].Json();

        }

        /// <summary>
        /// 保存
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["id"];
            var benefit = new NtErp.Crm.Services.Models.Beneficiaries();
            if (!string.IsNullOrWhiteSpace(id))
            {
                benefit = Needs.Erp.ErpPlot.Current.ClientSolutions.Beneficiaries[id];
            }
            benefit.Bank = Request.Form["Bank"];
            benefit.BankCode = Request.Form["BankCode"];
            string companyid= Request.Form["CompanyID"];
            benefit.Company = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Company>.Create(companyid);
            benefit.Address = Request.Form["Address"];
            benefit.EnterSuccess += Contact_EnterSuccess;
            benefit.Enter();
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Contact_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("保存成功", url, true);
        }
    }
}