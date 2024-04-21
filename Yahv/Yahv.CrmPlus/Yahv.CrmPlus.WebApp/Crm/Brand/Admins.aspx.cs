using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.Underly;
using Yahv.Web.Erp;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.WebApp.Crm.Brand
{
    public partial class Admins : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var entity = new Service.Views.Rolls.BrandsRoll()[Request.QueryString["id"]];
                this.Model.BrandID = entity.ID;
                this.Model.BrandName = entity.Name;
                this.Model.Admins = new YaHv.CrmPlus.Services.Views.Rolls.AdminsAllRoll().Where(item => item.RoleID == FixedRole.PM.GetFixedID() || item.RoleID == FixedRole.PMa.GetFixedID() ||item.RoleID==FixedRole.FAE.GetFixedID()).Select(item => new
                {
                    ID = item.ID,
                    Text = $"{item.RealName}-{item.RoleName}"
                });
            }
        }
        protected object data()
        {
            string brandid = Request.QueryString["id"];
            using (var view = new Service.Views.Rolls.vBrandsRoll())
            {
                return this.Paging(view.Where(item => item.BrandID == brandid), item => new
                {
                    item.ID,
                    item.RealName,
                    item.RoleName
                });

            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        protected void abandon()
        {
            var id = Request.Form["ID"];
            try
            {
                var entity = new Service.Views.Rolls.vBrandsRoll()[id];
                entity.Abandon();
                LogsOperating.LogOperating(Erp.Current, entity.ID, $"删除品牌负责人:{entity.AdminID}',品牌'{entity.BrandID}'");

            }
            catch (Exception ex)
            {
                LogsOperating.LogOperating(Erp.Current, id, $"删除品牌负责人" + ex);
            }
        }
        /// <summary>
        /// 停用
        /// </summary>
        protected object add()
        {
            string brandid = Request.Form["BrandID"];
            string adminid = Request.Form["AdminID"];
            //string supplierid = Request.Form["supplierid"];
            try
            {
                var vs = new Service.Views.Rolls.vBrandsRoll();
                if (vs.Any(item => item.BrandID == brandid && item.AdminID == adminid))
                {
                    return new { success = false, msg = "负责人已存在" };
                }
                else
                {
                    var admin = new AdminsAllRoll()[adminid];
                    new Service.Models.Origins.vBrand
                    {
                        BrandID = brandid,
                        AdminID = adminid,
                        RoleID = admin.RoleID
                    }.Enter();
                    LogsOperating.LogOperating(Erp.Current, brandid, $"新增品牌负责人:{admin.ID}-{admin.RealName}");
                    return new { success = true, msg = "新增成功" };
                }
            }
            catch (Exception ex)
            {
                return new { success = false, msg = ex.ToString() };
            }
        }
    }
}