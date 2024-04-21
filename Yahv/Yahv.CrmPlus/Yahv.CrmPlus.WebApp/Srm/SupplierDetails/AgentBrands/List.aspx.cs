using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails.AgentBrands
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.EnterpriseID = Request.QueryString["id"];
            }
        }
        protected object data()
        {
            string enterpriseid = Request.QueryString["id"];
            using (var view = new Service.Views.Rolls.AgentBrandsRoll())
            {
                return this.Paging(view.Where(item => item.EnterpriseID == enterpriseid), item => new
                {
                    item.ID,
                    item.BrandID,
                    item.BrandName,
                    item.Summary
                    //item.CompanyName
                });

            }
        }
        /// <summary>
        /// 停用
        /// </summary>
        protected void abandon()
        {
            var id = Request.Form["ID"];
            try
            {
                var entity = new Service.Views.Rolls.AgentBrandsRoll()[id];
                entity.Abandon();
                LogsOperating.LogOperating(Erp.Current, entity.EnterpriseID, $"废弃代理品牌:供应商'{entity.EnterpriseName}',品牌'{entity.BrandName}'");

            }
            catch (Exception ex)
            {
                LogsOperating.LogOperating(Erp.Current, id, $"废弃代理品牌失败" + ex);
            }
        }
    }
}