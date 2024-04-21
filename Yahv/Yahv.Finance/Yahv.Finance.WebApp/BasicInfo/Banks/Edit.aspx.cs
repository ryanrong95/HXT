using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.BasicInfo.Banks
{
    public partial class Edit : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var BankID = Request.QueryString["BankID"];
            this.Model.BankID = BankID;
            this.Model.Data = Yahv.Erp.Current.Finance.Banks.Where(t => t.ID == BankID).FirstOrDefault();

            Dictionary<string, string> dic_isAccountCost = new Dictionary<string, string>();
            dic_isAccountCost.Add("1", "是");
            dic_isAccountCost.Add("2", "否");
            this.Model.IsAccountCost = dic_isAccountCost.Select(item => new { value = item.Key, text = item.Value });
        }

        #region 提交保存

        protected void Submit()
        {
            var BankID = Request.Form["BankID"];
            Bank bank = null;
            try
            {
                var Name = Request.Form["Name"];
                var EnglishName = Request.Form["EnglishName"];
                var IsAccountCost = Request.Form["IsAccountCost"];
                var AccountCost = Request.Form["AccountCost"];
                var CostSummay = Request.Form["CostSummay"];

                var banks = Erp.Current.Finance.Banks;
                if (string.IsNullOrWhiteSpace(BankID))
                {
                    bank = new Bank()
                    {
                        Name = Name,
                        EnglishName = EnglishName,
                        CostSummay = CostSummay,
                        IsAccountCost = IsAccountCost == "1",
                        AccountCost = !string.IsNullOrEmpty(AccountCost) ? (decimal?)Convert.ToDecimal(AccountCost) : null,
                        CreatorID = Erp.Current.ID,
                        ModifierID = Erp.Current.ID,
                    };

                    if (bank != null && banks.Any(item => item.Name == bank.Name))
                    {
                        Response.Write((new { success = false, message = $"该名称已经存在，不能重复添加!", }).Json());
                        return;
                    }
                }
                else
                {
                    bank = banks.Where(t => t.ID == BankID).FirstOrDefault();
                    bank.Name = Name;
                    bank.EnglishName = EnglishName;
                    bank.CostSummay = CostSummay;
                    bank.IsAccountCost = IsAccountCost == "1";
                    bank.AccountCost = !string.IsNullOrEmpty(AccountCost) ? (decimal?)Convert.ToDecimal(AccountCost) : null;
                    bank.ModifierID = Erp.Current.ID;
                    bank.ModifyDate = DateTime.Now;

                    if (bank != null && banks.Any(item => item.ID != BankID && item.Name == bank.Name))
                    {
                        Response.Write((new { success = false, message = $"该名称已经存在，不能重复添加!", }).Json());
                        return;
                    }
                }

                bank.Enter();
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.银行管理, Services.Oplogs.GetMethodInfo(), string.IsNullOrWhiteSpace(BankID) ? "新增" : "修改", bank.Json());
                Response.Write((new { success = true, message = "提交成功", }).Json());
            }
            catch (Exception ex)
            {
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.银行管理, Services.Oplogs.GetMethodInfo(), (string.IsNullOrWhiteSpace(BankID) ? "新增" : "修改") + " 异常!", new { bank, exception = ex.ToString() }.Json());
                Response.Write((new { success = false, message = $"提交异常!{ex.Message}", }).Json());
            }
        }

        #endregion

    }
}