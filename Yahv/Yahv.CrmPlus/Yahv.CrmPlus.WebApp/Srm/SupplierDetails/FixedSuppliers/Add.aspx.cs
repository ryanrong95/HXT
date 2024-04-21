using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Models.Rolls;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails.FixedSuppliers
{
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string entepriseid = Request.QueryString["enterpriseid"];
            nFixedBrand entity = new nFixedBrand();
            entity.EnterpriseID = entepriseid;
            string brandid = Request.Form["BrandID"];
            var brand = new Service.Views.Rolls.BrandsRoll()[brandid];
            entity.Brand = brand.Name;
            entity.IsAdvantaged = Request.Form["IsAdvantaged"] != null;
            entity.IsPromoted = Request.Form["IsPromoted"] != null;
            entity.IsProhibited = Request.Form["IsProhibited"] != null;
            entity.IsSpecial = Request.Form["IsSpecial"] != null;
            entity.IsDiscounted = Request.Form["IsDiscounted"] != null;
            entity.Summary = Request.Form["Summary"].Trim();
            entity.CreatorID = Erp.Current.ID;
            entity.Repeat += Entity_Repeat;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }

        private void Entity_Repeat(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Dialog.Close("品牌已存在!", Web.Controls.Easyui.AutoSign.Error);
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as nFixedBrand;
            Service.LogsOperating.LogOperating(Erp.Current, entity.ID, $"新增nFixedBrand:{entity.ID}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}