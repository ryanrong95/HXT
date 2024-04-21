using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Srm.PendingSuppliers
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var id = Request.QueryString["id"];
                this.Model.Entity = new SuppliersRoll()[id];
                this.Model.Grade = ExtendsEnum.ToArray<SupplierGrade>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                this.Model.SupplierNature = ExtendsEnum.ToArray<SupplierNature>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                this.Model.SupplierType = ExtendsEnum.ToArray<SupplierType>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                this.Model.InvoiceType = ExtendsEnum.ToArray<InvoiceType>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                this.Model.Currency = ExtendsEnum.ToArray<Currency>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
            }
        }

        // 审批通过
        protected void btnPass_Click(object sender, EventArgs e)
        {
            var id = Request.QueryString["id"];
            var entity = new SuppliersRoll()[id];
            if (entity != null)
            {
                string AgentCompany = Request.Form["txt_InternalCompany"] == null ? null : new CompaniesRoll()[Request.Form["txt_InternalCompany"]]?.Enterprise.Name;

                entity.Type = (SupplierType)int.Parse(Request["SupplierType"]);
                entity.AreaType = AreaType.domestic;
                entity.Nature = (SupplierNature)int.Parse(Request["SupplierNature"]);
                string dyjcode = Request.Form["DyjCode"].Trim();
                entity.DyjCode = string.IsNullOrWhiteSpace(Request.Form["DyjCode"]) ? "" : dyjcode;
                entity.TaxperNumber = Request.Form["TaxperNumber"].Trim();
                entity.AgentCompany = AgentCompany;
                entity.IsFactory = Request["IsFactory"] == null ? false : true;
                entity.InvoiceType = (InvoiceType)int.Parse(Request["InvoiceType"]);
                string admincode = Request["AdminCode"].Trim();
                entity.RepayCycle = int.Parse(Request["RepayCycle"]);
                entity.Currency = (Currency)int.Parse(Request["Currency"]);
                entity.Price = decimal.Parse(Request["Price"]);
                entity.Enterprise = new Enterprise
                {
                    Name = Request.Form["Name"],
                    AdminCode = string.IsNullOrWhiteSpace(admincode) ? "" : admincode
                };
                entity.Grade = (SupplierGrade)int.Parse(Request["grade"]);
                entity.Place = Request["Origin"];
                entity.IsForwarder = Request.Form["IsForwarder"] != null;
                entity.Approve(ApprovalStatus.Normal);
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                            nameof(Yahv.Systematic.Crm),
                                           "SupplierApprove", "供应商" + entity.ID + "审批通过", "");
                //Easyui.Alert("提示", "审批已通过", Yahv.Web.Controls.Easyui.Sign.Info, true, Yahv.Web.Controls.Easyui.Method.Window);
                Easyui.Window.Close("审批已通过!", Yahv.Web.Controls.Easyui.AutoSign.Success);
            }
            else
            {
                Easyui.Alert("提示", "失败", Yahv.Web.Controls.Easyui.Sign.Info, true, Yahv.Web.Controls.Easyui.Method.Window);
            }

        }

        /// <summary>
        /// 审批不通过
        /// </summary>
        protected JMessage reject()
        {
            var id = Request["id"];
            var entity = new SuppliersRoll()[id];
            if (entity != null)
            {
                entity.Approve(ApprovalStatus.Voted);
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                          nameof(Yahv.Systematic.Crm),
                                         "SupplierApprove", "供应商" + entity.ID + "审批不通过", "");
                return new JMessage
                {
                    success = true,
                    code = 200,
                    data = "",
                };
            }
            else
            {
                return new JMessage
                {
                    success = true,
                    code = 300,
                    data = "内部错误",
                };
            }
        }


    }
}