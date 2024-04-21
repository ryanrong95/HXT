using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Srm.Suppliers
{
    public partial class Edit1 : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Entity = new TradingSuppliersRoll()[Request.QueryString["id"]];
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
                this.Model.Currency = ExtendsEnum.ToArray(Currency.Unknown).Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
            }
        }

        /// <summary>
        /// 公司类型
        /// </summary>
        /// <returns>组织类型列表</returns>
        protected object selects_Type()
        {
            var id = int.Parse(Request["id"]);
            var result = ExtendsEnum.ToDictionary<SupplierType>().Select(item => new
            {
                id = int.Parse(item.Key),
                name = item.Value.ToString(),
            });
            if (id == (int)AreaType.domestic)
            {
                return result.Where(item => item.id == (int)SupplierType.Market).ToArray();
            }
            else if (id == (int)AreaType.International)
            {
                return result.Where(item => item.id == (int)SupplierType.Overseas || item.id == (int)SupplierType.TopGrade || item.id == (int)SupplierType.Fixed).ToArray();
            }
            return result;
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var id = Request.QueryString["id"];
            var entity = new TradingSuppliersRoll()[id] ?? new TradingSupplier();
            string admincode = Request["AdminCode"].Trim();
            if (string.IsNullOrEmpty(id))
            {
                //entity.CompanyID = Request["txt_PurchaseCompany"];
                ///添加人
                entity.CreatorID = Erp.Current.ID;
                entity.StatusUnnormal += Entity_StatusUnnormal;
                entity.Enterprise = new Enterprise
                {
                    Name = Request.Form["Name"].Trim(),
                    AdminCode = string.IsNullOrWhiteSpace(admincode) ? "" : admincode
                };
            }
            else
            {
                //可编辑级别
                entity.Grade = (SupplierGrade)int.Parse(Request["Grade"]);
                entity.Enterprise = new Enterprise
                {
                    Name = Request.Form["Name"],
                    AdminCode = string.IsNullOrWhiteSpace(admincode) ? "" : admincode
                };
            }
            //原厂
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

            entity.RepayCycle = int.Parse(Request["RepayCycle"]);
            entity.Currency = (Currency)int.Parse(Request["Currency"]);
            entity.Price = decimal.Parse(Request["Price"]);
            entity.Place = Request["Origin"];
            entity.IsForwarder = Request.Form["IsForwarder"] != null;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }

        private void Entity_StatusUnnormal(object sender, Usually.ErrorEventArgs e)
        {
            var entity = sender as Supplier;
            Easyui.Reload("提示", "供应商已存在，状态：" + entity.SupplierStatus.GetDescription(), Yahv.Web.Controls.Easyui.Sign.Warning);
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as TradingSupplier;
            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Srm),
                                        "SupplierInsert", "新增供应商：" + entity.Enterprise.Name + ",采购公司ID:" + entity.CompanyID, "");
            }
            else
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Srm),
                                        "SupplierUpdate", "修改供应商信息：" + entity.Enterprise.Name, "");
            }
            //Easyui.Alert("提示", "保存成功", Yahv.Web.Controls.Easyui.Sign.Info, true, Yahv.Web.Controls.Easyui.Method.Window);
            Easyui.Window.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}