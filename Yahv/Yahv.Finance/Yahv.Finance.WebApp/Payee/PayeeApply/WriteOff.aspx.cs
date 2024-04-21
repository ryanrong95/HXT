using Layers.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.Finance.WebApp.Payee
{
    public partial class WriteOff : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region 提交保存

        protected void Submit()
        {
            FlowAccount writeOffFlowAccount = null;
            try
            {
                string FormCode = Request.Form["FormCode"];
                string Price = Request.Form["Price"];
                string Summary = Request.Form["Summary"];

                var ReceiptFlowAccount = new Yahv.Finance.Services.Views.Rolls.FlowAccountsRoll()
                    .Where(t => t.FormCode == FormCode
                             && t.AccountMethord == AccountMethord.Receipt
                             && t.Price > 0).FirstOrDefault();

                if (ReceiptFlowAccount == null)
                {
                    Response.Write((new { success = false, message = "未查询到流水号为 " + FormCode + " 的收款记录", }).Json());
                    return;
                }

                var eRate1 = Yahv.Finance.Services.ExchangeRates.Universal[ReceiptFlowAccount.Currency, Currency.CNY];

                string newFlowID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc);

                writeOffFlowAccount = new FlowAccount()
                {
                    ID = newFlowID,
                    AccountMethord = AccountMethord.Receipt,
                    AccountID = ReceiptFlowAccount.AccountID,
                    Currency = ReceiptFlowAccount.Currency,
                    Price = 0 - decimal.Parse(Price),

                    Balance = 0,

                    FormCode = FormCode,
                    Currency1 = Currency.CNY,
                    ERate1 = eRate1,
                    Price1 = 0 - decimal.Parse(Price) * eRate1,

                    Balance1 = 0,

                    CreatorID = Erp.Current.ID,
                    //NatureType = Underly.NatureType.UnKnown,
                };

                writeOffFlowAccount.Enter();

                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.收款管理, Services.Oplogs.GetMethodInfo(), "冲销-新增", writeOffFlowAccount.Json());
                Response.Write((new { success = true, message = "提交成功", }).Json());
            }
            catch (Exception ex)
            {
                Services.Oplogs.Oplog(Erp.Current.ID, LogModular.收款管理, Services.Oplogs.GetMethodInfo(), "冲销-新增 异常!", new { writeOffFlowAccount, exception = ex.ToString() }.Json());
                Response.Write((new { success = false, message = $"提交异常!{ex.Message}", }).Json());
            }
        }

        #endregion

    }
}