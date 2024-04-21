using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Srm.Consignors
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Entity = new ConsignorsRoll()[Request.QueryString["id"]];
                this.Model.EnterpriseType = Request["enterprisetype"];
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //try
            //{
            string clientid = Request.QueryString["clientid"];
            var id = Request.QueryString["id"];

            Consignor entity = new ConsignorsRoll()[id] ?? new Consignor();

            entity.Title = Request["Title"].Trim();
            entity.EnterpriseID = clientid;
            entity.Address = Request.Form["Address"].Trim();
            entity.Postzip = Request.Form["Postzip"].Trim();
            entity.DyjCode = Request.Form["DyjCode"].Trim();
            entity.Name = Request.Form["Name"].Trim();
            entity.Tel = Request.Form["Tel"].Trim();
            entity.Mobile = Request.Form["Mobile"].Trim();
            entity.Email = Request.Form["Email"].Trim();
            entity.IsDefault = Request["IsDefault"] == null ? false : true; ;
            if (string.IsNullOrWhiteSpace(id))
            {
                entity.CreatorID = Yahv.Erp.Current.ID;
            }
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
            //}
            //catch (Exception ex)
            //{
            //    Easyui.Reload("提示", ex.Message, Yahv.Web.Controls.Easyui.Sign.Warning);
            //}
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}