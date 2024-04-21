using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Csrm.WebApp;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace WebApp.Srm.MySuppliers
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var entity =
                this.Model.Entity = new TradingSuppliersRoll()[Request.QueryString["id"]];
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
                this.Model.Currency = ExtendsEnum.ToArray(Currency.Unknown).Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var id = Request.QueryString["id"];
            var entity = new SuppliersRoll()[id] ?? new Supplier();
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
                entity.Place = Request["Origin"];
                if (string.IsNullOrEmpty(id))
                {
                    entity.CreatorID = Yahv.Erp.Current.ID;
                    entity.StatusUnnormal += Entity_StatusUnnormal; ;
                }
                entity.IsForwarder = Request.Form["IsForwarder"] != null;
                entity.EnterSuccess += Entity_EnterSuccess;
                entity.Enter();
            }
        }
        private void Entity_StatusUnnormal(object sender, Yahv.Usually.ErrorEventArgs e)
        {
            var entity = sender as Supplier;
            Easyui.Reload("提示", "供应商已存在，客户状态：" + entity.SupplierStatus.GetDescription(), Yahv.Web.Controls.Easyui.Sign.Warning);
        }
        private void Entity_EnterSuccess(object sender, Yahv.Usually.SuccessEventArgs e)
        {
            var entity = sender as Supplier;
            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Srm),
                                        "Supplierinsert", "新增供应商：" + entity.ID, "");
            }
            else
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Srm),
                                        "SupplierUpdate", "修改供应商信息：" + entity.ID, "");
            }
            //Easyui.Alert("提示", "保存成功", Yahv.Web.Controls.Easyui.Sign.Info, true, Yahv.Web.Controls.Easyui.Method.Window);
            Easyui.Window.Close("保存成功!", Yahv.Web.Controls.Easyui.AutoSign.Success);
        }
    }
}