using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Views;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.StandardBrand
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request.QueryString["ID"];
                this.Model.Entity = Erp.Current.CrmPlus.StandardBrands[id];
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string name = Request["name"];
            string chinesename = Request["ChineseName"];
            string code = Request["Code"];
            bool isAgent = Request["isAgent"] != null;
            string summry = Request["Summary"];
            string id = Request["ID"];
            var brands = Erp.Current.CrmPlus.StandardBrands;
            //新增防止品牌名称重复
            if (string.IsNullOrEmpty(id))
            {
                if (brands.Count(item => item.Name == name && item.Status == Underly.DataStatus.Normal) >= 1)
                {
                    Easyui.Reload("提示", "品牌已存在!", Web.Controls.Easyui.Sign.Error);
                }
            }
            var entity = Erp.Current.CrmPlus.StandardBrands[id] ?? new Yahv.CrmPlus.Service.Models.Origins.StandardBrand { Name = name };
            entity.ChineseName = chinesename;
            entity.Code = code;
            entity.IsAgent = isAgent;
            entity.Summary = summry;
            entity.CreatorID = Erp.Current.ID;
            entity.EnterError += Entity_EnterError;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();

        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success); ;
        }

        private void Entity_EnterError(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.AutoAlert(e.Message, Web.Controls.Easyui.AutoSign.Error);
        }

        #region  人员配置
        /// <summary>
        /// 人员配置
        /// </summary>
        /// <returns></returns>
        protected object admins()
        {
            string brandid = Request.QueryString["id"];
            using (var view = new Service.Views.Rolls.vBrandsRoll())
            {
                return this.Paging(view.Where(item => item.BrandID == brandid), item => new
                {
                    item.ID,
                    item.RealName,
                    item.RoleName,
                    item.UserName
                });

            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        protected void abandonAdmins()
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
        #endregion

        #region 内部公司
        protected object companys()
        {
            string brandid = Request.QueryString["id"];
            var view = new CompanynBrandView().Where(x=>x.BrandID==brandid);
            return this.Paging(view, item => new
            {
                item.ID,
                item.nBrandID,
                item.CompanyName,
                item.BrandID,
                item.BrandName
            });
        }

        protected void abandonCompany()
        {
            var id = Request.Form["ID"];
            try
            {
                var entity = new Service.Views.Origins.nBrandOrigin()[id];
                entity.Abandon();
                LogsOperating.LogOperating(Erp.Current, entity.ID, $"删除合作公司:{entity.EnterpriseName}',品牌'{entity.BrandID}'");

            }
            catch (Exception ex)
            {
                LogsOperating.LogOperating(Erp.Current, id, $"删除合作公司" + ex);
            }
        }
        #endregion 
        protected object suppliers()
        {

            string brandid = Request.QueryString["id"];
            var view = new SuppliernBrandView().Where(x => x.BrandID == brandid);
            return this.Paging(view, item => new
            {
                item.ID,
                item.nBrandID,
                item.SupplierName,
                item.BrandID,
                item.BrandName
            });

        }

        protected void abandonSupplier()
        {
            var id = Request.Form["ID"];
            try
            {
                var entity = new Service.Views.Origins.nBrandOrigin()[id];
                entity.Abandon();
                LogsOperating.LogOperating(Erp.Current, entity.ID, $"删除供应商:{entity.EnterpriseName}',品牌'{entity.BrandID}'");

            }
            catch (Exception ex)
            {
                LogsOperating.LogOperating(Erp.Current, id, $"删除品牌负责人" + ex);
            }
        }
    }
}