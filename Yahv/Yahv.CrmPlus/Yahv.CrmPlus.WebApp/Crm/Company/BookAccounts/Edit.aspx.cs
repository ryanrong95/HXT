using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.CrmPlus.WebApp.Crm.Company.BookAccounts
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.Entity = new CompaniesRoll()[Request.QueryString["companyid"]];
                LoadData();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void LoadData()
        {

            this.Model.BookAccountType = ExtendsEnum.ToArray<BookAccountType>().Select(item => new
            {
                value = (int)item,
                text = item.GetDescription()
            });

        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string bookAccountType = Request.Form["BookAccountType"];
            string bank = Request.Form["Bank"];
            string bankAddress = Request.Form["BankAddress"];
            string bankCode = Request.Form["BankCode"];
            string account = Request.Form["Account"].Trim();
            string swiftCode = Request.Form["SwiftCode"];

            string transfer = Request.Form["Transfer"];
            string isPersonal = Request.Form["IsP"];
            string ID = Request.QueryString["companyid"];


            var entity = new Yahv.CrmPlus.Service.Models.Origins.BookAccount();
            entity.EnterpriseID = ID;
            entity.RelationType = Underly.RelationType.Own;
            entity.BookAccountType = (BookAccountType)int.Parse(bookAccountType.Trim());
            entity.BookAccountMethord = BookAccountMethord.Bank;
            entity.Bank = bank.Trim();
            entity.BankAddress = bankAddress.Trim();
            entity.BankCode = bankCode.Trim();
            entity.Account = account.Trim();
            entity.SwiftCode = swiftCode.Trim();
            entity.Transfer = transfer.Trim();
            entity.IsPersonal =isPersonal!=null;
            entity.CreatorID = Erp.Current.ID;
            entity.EnterError += Entity_EnterError;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Enter();
        }


        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as BookAccount;
            LogsOperating.LogOperating(Erp.Current, entity.ID, $"新增账户:{ entity.ID}");
            Easyui.Dialog.Close("保存成功!", Web.Controls.Easyui.AutoSign.Success);
        }
        private void Entity_EnterError(object sender, Usually.ErrorEventArgs e)
        {
            Easyui.Dialog.Close("保存失败!", Web.Controls.Easyui.AutoSign.Error);
        }
    }
}