using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Prm.Consignees
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //地区
                this.Model.District = ExtendsEnum.ToArray<District>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                this.Model.Entity = new ConsigneesRoll()[Request.QueryString["id"]];
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            //try
            //{
            string clientid = Request.QueryString["clientid"];
            var id = Request.QueryString["id"];
            var entity = new CompaniesRoll()[clientid].Consignees[id] ?? new Consignee();
            entity.EnterpriseID = clientid;
            entity.Address = Request.Form["Address"].Trim();
            entity.District = (District)int.Parse(Request["selDistrict"]);
            entity.Postzip = Request.Form["Postzip"].Trim();
            entity.DyjCode = Request.Form["DyjCode"].Trim();
            entity.Name = Request.Form["Name"].Trim();
            entity.Tel = Request.Form["Tel"].Trim();
            entity.Mobile = Request.Form["Mobile"].Trim();
            entity.Email = Request.Form["Email"].Trim();
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
            var entity = sender as Consignee;
            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                          nameof(Yahv.Systematic.Crm),
                                         "ConsigneeInsert", "新增到货地址：" + entity.ID, "");
            }
            else
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                          nameof(Yahv.Systematic.Crm),
                                         "ConsigneeUpdate", "修改到货地址：" + entity.ID, "");
            }
            //Easyui.Alert("提示", "保存成功", Yahv.Web.Controls.Easyui.Sign.Info, true, Web.Controls.Easyui.Method.Dialog);
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}