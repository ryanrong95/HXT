using System;
using System.Linq;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Srm.Beneficiaries
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Entity = new BeneficiariesRoll()[Request.QueryString["id"]];
                this.Model.Enterprise = new EnterprisesRoll()[Request.QueryString["supplierid"]];
                // 1.币种
                this.Model.Currency = ExtendsEnum.ToArray<Currency>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                //2.支付方式
                this.Model.Methord = ExtendsEnum.ToArray<Methord>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                //3.所在地区
                this.Model.District = ExtendsEnum.ToArray<District>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                //4.发票
                this.Model.InvoiceType = ExtendsEnum.ToArray(InvoiceType.Unkonwn).Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });

            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            var id = Request.QueryString["id"];
            var supplierid = Request.QueryString["supplierid"];
            var entity = Erp.Current.Srm.Suppliers[supplierid].Beneficiaries[id] ?? new TradingBeneficiary();

            entity.EnterpriseID = supplierid;
            entity.Currency = (Currency)int.Parse(Request["Currency"]);
            entity.District = (District)int.Parse(Request["selDistrict"]);
            entity.Methord = (Methord)int.Parse(Request["Methord"]);
            entity.Bank = Request.Form["Bank"].Trim();
            entity.BankAddress = Request.Form["BankAddress"].Trim();
            entity.RealName = Request.Form["RealName"].Trim();
            entity.Account = Request.Form["Account"].Trim();
            entity.SwiftCode = Request.Form["SwiftCode"].Trim();
            entity.InvoiceType = (InvoiceType)int.Parse(Request["InvoiceType"]);
            entity.Name = Request.Form["Name"].Trim();
            entity.Tel = Request.Form["Tel"].Trim();
            entity.Mobile = Request.Form["Mobile"].Trim();
            entity.Email = Request.Form["Email"].Trim();
            entity.BankCode = Request.Form["BankCode"].Trim();
            if (string.IsNullOrWhiteSpace(id))
            {
                entity.CreatorID = Yahv.Erp.Current.ID;
            }
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();


        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as TradingBeneficiary;
            if (string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Srm),
                                        "BeneficiaryInsert", "新增受益人：" + entity.ID, "");
            }
            else
            {
                Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(),
                                         nameof(Yahv.Systematic.Srm),
                                        "BeneficiaryUpdate", "修改受益人信息：" + entity.ID, "");
            }
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }

    }
}