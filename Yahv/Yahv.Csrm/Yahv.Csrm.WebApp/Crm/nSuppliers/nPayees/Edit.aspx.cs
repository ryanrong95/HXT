using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Models.Rolls;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.nSuppliers.nPayees
{
    public partial class Edit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 1.币种
                //this.Model.Currency = ExtendsEnum.ToArray<Currency>().Select(item => new
                //{
                //    value = (int)item,
                //    text = item.GetDescription()
                //});
                //2.支付方式
                this.Model.Methord = ExtendsEnum.ToArray<Methord>(Methord.TT, Methord.Exchange).Select(item => new
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
                this.Model.InvoiceType = ExtendsEnum.ToArray<InvoiceType>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
                this.Model.Place = ExtendsEnum.ToArray<Origin>(Origin.NG,Origin.Unknown).Select(item => new
                {
                    value = item.ToString(),
                    text = item.GetDescription()
                });
                string clientid = Request.QueryString["clientid"];
                string supplierid = Request.QueryString["supplierid"];
                string payeeid = Request.QueryString["payeeid"];
                var supplier = Erp.Current.Whs.WsClients[clientid].nSuppliers[supplierid];
                string place = supplier.RealEnterprise.Place;
                this.Model.Entity = supplier.nPayees[payeeid];
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string clientid = Request.QueryString["clientid"];
            string supplierid = Request.QueryString["supplierid"];
            string payeeid = Request.QueryString["payeeid"];
            var client = Erp.Current.Whs.WsClients[clientid];
            if (client.Enterprise.Name.StartsWith("reg-", StringComparison.OrdinalIgnoreCase))
            {
                Easyui.AutoAlert("请先规范客户名称", Web.Controls.Easyui.AutoSign.Warning);
                return;
            }
            var supplier = client.nSuppliers[supplierid];
            var payees = supplier.nPayees;
            var payee = payees[payeeid] ?? new nPayee();
            Methord methord = (Methord)int.Parse(Request["Methord"]);
            payee.RealID = supplier.RealID;
            if (string.IsNullOrWhiteSpace(payeeid))
            {
                payee.nSupplierID = supplierid;
                payee.EnterpriseID = clientid;
                payee.Creator = Erp.Current.ID;
                payee.Repeat += Payee_Repeat;
                payee.RealEnterprise = supplier.RealEnterprise;
            }
            payee.Place = Request["Place"];
            payee.Methord = methord;
            payee.Bank = Request["Bank"].Trim();
            payee.BankAddress = Request["BankAddress"].Trim();
            payee.Account = Request["Account"].Trim();
            payee.SwiftCode = Request["SwiftCode"].Trim();

            payee.Contact = Request["Name"].Trim();
            payee.Tel = Request["Tel"].Trim();
            payee.Mobile = Request["Mobile"].Trim();
            payee.Email = Request["Email"].Trim();
            payee.IsDefault = Request["IsDefault"] != null;
            payee.EnterSuccess += Payer_EnterSuccess;
            payee.Enter();
        }

        private void Payee_Repeat(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Reload("提示", "收款人已存在", Yahv.Web.Controls.Easyui.Sign.Warning);
        }

        private void Payer_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as nPayee;
            //调用接口
            var api = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
            if (!string.IsNullOrWhiteSpace(api))
            {
                Unify(api, entity);
            }
            Easyui.Ttop.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
        //调用接口
        object Unify(string api, nPayee data)
        {
            var json = new
            {
                Enterprise = new EnterprisesRoll()[data.EnterpriseID],
                Place = data.Place,
                SupplierName = data.RealEnterprise.Name,
                IsDefault = data.IsDefault,
                Bank = data.Bank,
                BankAddress = data.BankAddress,
                Account = data.Account,
                SwiftCode = data.SwiftCode,
                Methord = data.Methord,
                Currency = data.Currency,
                Name = data.Contact,
                Tel = data.Tel,
                Mobile = data.Mobile,
                Email = data.Email,
            }.Json();
            var response = HttpClientHelp.HttpPostRaw(api + "/suppliers/banks", json);
            return response;
        }
    }
}