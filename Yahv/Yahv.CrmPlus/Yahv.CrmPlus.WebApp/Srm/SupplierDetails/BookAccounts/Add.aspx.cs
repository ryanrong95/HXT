using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Srm.SupplierDetails.BookAccounts
{
    public partial class Add : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.EnterpriseID = Request.QueryString["enterpriseid"];
                this.Model.BookAccountMethord = ExtendsEnum.ToArray<BookAccountMethord>().Select(item => new
                {
                    value = (int)item,
                    text = item.GetDescription()
                });
            
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string bank = Request.Form["Bank"].Trim();
            string bankAddress = Request.Form["BankAddress"].Trim();
            string bankCode = Request.Form["BankCode"].Trim();
            string account = Request.Form["Account"].Trim();
            string swiftCode = Request.Form["SwiftCode"].Trim();

            string transfer = Request.Form["Transfer"];
            bool isPersonal = Request.Form["IsPersonal"] != null;
            string enterpriseid = Request.QueryString["id"];
            BookAccountMethord methord = (BookAccountMethord)int.Parse(Request.Form["BookAccountMethord"]);

            var entity = new Yahv.CrmPlus.Service.Models.Origins.BookAccount();
            entity.BookAccountMethord = methord;
            if (methord == BookAccountMethord.Bank)
            {
                entity.Bank = bank;
                entity.SwiftCode = swiftCode;
                entity.BankAddress = bankAddress;
                entity.BankCode = bankCode;
                entity.Currency = (Currency)int.Parse(Request.Form["Currency"]);
                entity.Transfer = transfer;
            }
            else
            {
                entity.Currency = Currency.Unknown;
            }
            entity.EnterpriseID = enterpriseid;
            entity.RelationType = RelationType.Suppliers;
            entity.BookAccountType = BookAccountType.Payee;
            entity.Account = account;
            entity.IsPersonal = isPersonal;
            entity.CreatorID = Erp.Current.ID;
            entity.EnterSuccess += Entity_EnterSuccess; ;
            entity.Enter();
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as BookAccount;
            LogsOperating.LogOperating(Erp.Current, entity.ID, $"新增账户:{ entity.ID}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
    }
}