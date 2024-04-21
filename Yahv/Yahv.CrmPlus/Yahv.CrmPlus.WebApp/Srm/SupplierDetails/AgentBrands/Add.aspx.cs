using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Models.Rolls;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails.AgentBrands
{
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
            //    //this.Model.Brands = new BrandsRoll().Where(item => item.Status == DataStatus.Normal).Select(item => new
            //    //{
            //    //    ID = item.ID,
            //    //    Name = item.Name
            //    //});
            //    //this.Model.Companies = new CompaniesRoll().Where(item => item.Status == DataStatus.Normal).Select(item => new
            //    //{
            //    //    ID = item.ID,
            //    //    Name = item.Enterprise.Name
            //    //});
            //    //var admins = new AdminsAllRoll();
            //    //this.Model.PM = admins.Where(item => item.RoleID == FixedRole.PM.GetFixedID()).Select(item => new
            //    //{
            //    //    ID = item.ID,
            //    //    Name = item.RealName
            //    //});
            //    //this.Model.PMA = admins.Where(item => item.RoleID == FixedRole.PMa.GetFixedID()).Select(item => new
            //    //{
            //    //    ID = item.ID,
            //    //    Name = item.RealName
            //    //});
            //    //this.Model.FAE = admins.Where(item => item.RoleID == FixedRole.FAE.GetFixedID()).Select(item=>new {
            //    //ID = item.ID,
            //    //    Name = item.RealName
            //    //});
            //}
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string supplierid = Request.QueryString["id"];
            string companyid = Request.Form["Company"];
            string brandid = Request.Form["Brand"];
            string summary = Request.Form["Summary"];
            //var agents = new AgentBrandsRoll().Where(item => item.Type == Underly.CrmPlus.nBrandType.Agent && item.BrandID == brandid);
            //if (agents.Any())
            //{
            //    var a = agents.FirstOrDefault();
            //    Easyui.Dialog.Close($"品牌已被代理,原厂：{a.Produce.EnterpriseName},代理公司：{a.CompanyName}", Web.Controls.Easyui.AutoSign.Error);
            //}
            nBrand entity = new nBrand();
            entity.EnterpriseID = supplierid;
            entity.BrandID = brandid;
            entity.CreatorID = Erp.Current.ID;
            entity.Summary = summary;
            entity.Type = Underly.nBrandType.Agent;
            entity.Repeat += Entity_Repeat;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }

        private void Entity_Repeat(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Dialog.Close("品牌已被代理", Web.Controls.Easyui.AutoSign.Error);
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as nBrand;
            var pms = Request.Form["PM"];
            if (!string.IsNullOrWhiteSpace(pms))
            {
                var ids = pms.Split(',');
                foreach (var item in ids)
                {
                    new vBrand
                    {
                        BrandID = entity.BrandID,
                        AdminID = item,
                        RoleID = FixedRole.PM.GetFixedID()
                    }.Enter();
                }
            }
            var pmas = Request.Form["PMA"];
            if (!string.IsNullOrWhiteSpace(pmas))
            {
                var ids = pmas.Split(',');
                foreach (var item in ids)
                {
                    new vBrand
                    {
                        BrandID = entity.BrandID,
                        AdminID = item,
                        RoleID = FixedRole.PMa.GetFixedID()
                    }.Enter();
                }
            }
            var faes = Request.Form["AFE"];
            if (!string.IsNullOrWhiteSpace(faes))
            {
                //var ids = faes.Split(',');
                //foreach (var item in ids)
                //{
                //    new vBrand
                //    {
                //        BrandID = entity.BrandID,
                //        AdminID = item,
                //        RoleID = FixedRole..GetFixedID()
                //    }.Enter();
                //}
            }
            LogsOperating.LogOperating(Erp.Current, entity.ID, $"新增代理品牌:{ entity.ID}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}