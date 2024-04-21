using System;
using System.Linq;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Erm.WebApp.Erm.Staffs
{
    public partial class BankcardEdit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
                this.Model.AllData = Alls.Current.BankCards[Request.QueryString["ID"]];
            }
        }
        protected void LoadComboBoxData()
        {
            this.Model.BankDate = ExtendsEnum.ToDictionary<Services.Bank>().Select(item => new { Value = item.Value, Text = item.Value });
        }
        protected void Save()
        {
            var id = Request.Form["ID"];
            //TODO:新增问题
            #region Bankcard
            var bankcard = Alls.Current.BankCards[id] ?? new BankCard();
            bankcard.ID = id;
            bankcard.Bank = Request.Form["Bank"].Trim();
            bankcard.Account = Request.Form["Account"].Trim();
            bankcard.EnterError += BankCard_EnterError;
            bankcard.EnterSuccess += BankCard_EnterSuccess;
            Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工信息",
                   string.IsNullOrWhiteSpace(id) ? "新增银行卡信息" : "修改银行卡信息", bankcard.Json());
            bankcard.Enter();
            #endregion
        }

        /// <summary>
        /// 保存成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BankCard_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 保存失败触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BankCard_EnterError(object sender, Usually.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
    }
}