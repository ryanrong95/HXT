using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.StandardBrand
{
    public partial class Add : ErpParticlePage
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
            bool isAgent = Request["IsAgent"]!=null;
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
            Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success); ;
        }

        private void Entity_EnterError(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.AutoAlert(e.Message, Web.Controls.Easyui.AutoSign.Error);
        }

    }
}