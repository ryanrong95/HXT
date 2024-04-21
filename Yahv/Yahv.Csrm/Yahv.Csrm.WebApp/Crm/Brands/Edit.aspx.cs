using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Web.Forms;

namespace Yahv.Csrm.WebApp.Crm.Brands
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string name = Request["name"];
            var brands = new YaHv.Csrm.Services.Views.Rolls.BrandsRoll();
            if (brands.Any(item => item.Name == name))
            {
                Easyui.Reload("提示", "品牌已存在!", Web.Controls.Easyui.Sign.Error);
            }
            else {
                var entity = new YaHv.Csrm.Services.Models.Origins.Brand { Name = name };
                entity.EnterSuccess += Entity_EnterSuccess;
                entity.Enter();
            }
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success); ;
        }
    }
}