using Needs.Erp;
using Needs.Utils.Descriptions;
using Needs.Utils.Linq;
using Needs.Utils.Serializers;
using Needs.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NtErp.Vrs.Services.Enums;

namespace WebApp.Vrs.Companies
{
    public partial class List : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //this.Header.Title
        }
        protected void data()
        {
            Expression<Func<NtErp.Vrs.Services.Models.Company, bool>> expression = item => true;
            string queryName = Request.QueryString["txtName"];
            string queryAddress = Request.QueryString["txtAddress"];
            string queryCorporateRepresentative = Request.QueryString["txtCorporateRepresentative"];
            string queryType = Request.QueryString["_type"];
            if (queryName != null)
            {
                expression = expression.And(item => item.Name.Contains(queryName));
            }
            if (queryAddress != null)
            {
                expression = expression.And(item => item.Address.Contains(queryAddress));
            }
            if (queryCorporateRepresentative != null)
            {
                expression = expression.And(item => item.CorporateRepresentative.Contains(queryCorporateRepresentative));
            }
            if (queryType != "-1" && queryType != null)
            {
                expression = expression.And(item => item.Type == (ComapnyType)int.Parse(queryType));
            }
            using (var context = Needs.Linq.LinqContext.Current)
            {
                Response.Paging(ErpPlot.Current.Publishs.CompaniesAll.Where(expression).OrderBy(item=>item.CreateDate), item => new
                {
                    item.ID,
                    Type = item.Type.GetDescription(),
                    item.Name,
                    item.Code,
                    item.Address,
                    item.RegisteredCapital,
                    item.CorporateRepresentative,
                    item.CreateDate,
                    item.UpdateDate
                });

                Needs.Linq.LinqContext.Current.Dispose();
            }
        } 

        protected void CompanyAbandon()
        {
            string queryId = Request.Form["id"];
            var entity = ErpPlot.Current.Publishs.CompaniesAll[queryId] ?? new NtErp.Vrs.Services.Models.Vender();
            entity.AbandonSuccess += AbandonSuccess;
            entity.Abandon();
        }
        private void AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write(new { success = true }.Json());
        }
    }
}