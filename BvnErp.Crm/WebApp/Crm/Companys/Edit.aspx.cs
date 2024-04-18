using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Companys
{
    /// <summary>
    /// 编辑页面
    /// </summary>
    public partial class Edit : Uc.PageBase
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.TypeData = EnumUtils.ToDictionary<CompanyType>().Select(item => new { text = item.Value, value = item.Key }).Json();
                LoadData();
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            string type = Request.QueryString["Type"];
            if (type == "C")
            {
                this.Model.Type =(int)CompanyType.plot;
            }
            else if (type == "M")  //品牌
            {
                this.Model.Type = (int)CompanyType.Manufacture;
            }
            else if (type == "S")  //供应商
            {
                this.Model.Type = (int)CompanyType.Supplier;
            }
            var companys = Needs.Erp.ErpPlot.Current.ClientSolutions.Companys[id];
            this.Model.AllData = companys.Json();
        }

        /// <summary>
        /// 保存
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["ID"];
            string type = Request.QueryString["Type"];
            string a = Request.Form["Type"];
            var company = Needs.Erp.ErpPlot.Current.ClientSolutions.Companys[id] as NtErp.Crm.Services.Models.Company
              ?? new NtErp.Crm.Services.Models.Company();
            company.Name = Request.Form["Name"];
            company.Code = Request.Form["Code"];
            company.Summary = Request.Form["Summary"];
            if (type == "C")
            {
                company.Type = CompanyType.plot;
            }
            else if (type == "M")  //品牌
            {
                company.Type = CompanyType.Manufacture;
            }
            else if (type == "S")  //供应商
            {
                company.Type = CompanyType.Supplier;
            }
            company.EnterError += Companys_EnterError;
            company.EnterSuccess += Companys_EnterSuccess;
            company.Enter();
        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Companys_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            this.Alert(e.Message, Request.Url, true);
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Companys_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("保存成功", url, true);
        }

    }
}