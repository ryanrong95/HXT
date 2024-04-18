using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DyjReceipt
    {
        #region 属性

        public string key { get; set; }

        public string uid { get; set; }

        public DyjReceiptMain main { get; set; }

        public List<DyjReceiptDeta> detas { get; set; }

        #endregion


        public DyjReceipt() { }

        public DyjReceipt(FinanceReceipt fReceipt)
        {
            this.key = DyjCwConfig.Key;
            this.uid = DyjCwConfig.uid;

            //转换收款类型
            DyjReceiptType type;
            switch (fReceipt.ReceiptType)
            {
                case Enums.PaymentType.Check:
                    type = DyjReceiptType.Check;
                    break;
                case Enums.PaymentType.Cash:
                    type = DyjReceiptType.Cash;
                    break;
                case Enums.PaymentType.TransferAccount:
                    type = DyjReceiptType.TransferAccount;
                    break;
                case Enums.PaymentType.AcceptanceBill:
                    type = DyjReceiptType.AcceptanceBill;
                    break;
                default:
                    type = DyjReceiptType.TransferAccount;
                    break;
            }

            //转换币种
            DyjCurrency currency;
            switch (fReceipt.Currency)
            {
                case "CNY":
                    currency = DyjCurrency.CNY;
                    break;
                case "USD":
                    currency = DyjCurrency.USD;
                    break;
                case "HKD":
                    currency = DyjCurrency.HKD;
                    break;
                case "EUR":
                    currency = DyjCurrency.EUR;
                    break;
                case "GBP":
                    currency = DyjCurrency.GBP;
                    break;
                case "JPY":
                    currency = DyjCurrency.JPY;
                    break;
                default:
                    currency = DyjCurrency.CNY;
                    break;
            }

            //转换金库
            var vault = new Views.FinanceVaultsView().Where(t => t.ID == fReceipt.Vault.ID).FirstOrDefault();

            //转换账户
            var account = new Views.FinanceAccountsView().Where(t => t.ID == fReceipt.Account.ID).FirstOrDefault();

            string EmployeeID = "3055";
            //转换AdminID
            var client = new Views.ClientsView().Where(t => t.Company.Name == fReceipt.Payer).FirstOrDefault();
            if (client != null)
            {
                var admin2 = new Views.AdminsTopView2().Where(t => t.OriginID == client.ServiceManager.ID).FirstOrDefault();
                //if (admin2 == null)
                //{
                //    admin2 = new Views.AdminsTopView2().Where(t => t.OriginID == client.StorageServiceManager.ID).FirstOrDefault();
                //}
                var dyjInfo = new Views.PvbErmStaffsTopView().Where(t => t.AdminID == admin2.ID).FirstOrDefault();
                if (dyjInfo != null)
                {
                    EmployeeID = dyjInfo.OriginID;
                }
            }

            this.main = new DyjReceiptMain
            {
                FiskID = vault.BigWinVaultID,
                AccountID = account.BigWinAccountID,//大赢家账户ID
                CurrencyID = currency.GetHashCode().ToString(),
                Client = fReceipt.Payer,
                BalanceStyle = type.GetHashCode().ToString(),
                ForeignAmount = 0M,
                Amount = fReceipt.Amount,
                Note = "",
                FKAmount = 0M
            };

            var list = new List<DyjReceiptDeta>();
            var deta = new DyjReceiptDeta
            {
                BillType = "3",
                CorpID = "6900",
                DeptID = "6901",
                EmployeeID = EmployeeID,
                Amount = fReceipt.Amount,
                Note = fReceipt.Summary
            };

            list.Add(deta);
            this.detas = list;
        }

        /// <summary>
        /// 返回值，返回大赢家ID
        /// </summary>
        /// <returns></returns>
        public string PostDyjReceipt()
        {
            var api = System.Configuration.ConfigurationManager.AppSettings["DYJFianceApiURL"];
            //调用接口
            var post = DyjBaseInfo.PostDYJ(api + DyjCwConfig.SetShouKuan, "POST", this.Json());
            //格式化json字符串
            var result = new Newtonsoft.Json.JsonSerializer().Deserialize(new Newtonsoft.Json.JsonTextReader(new System.IO.StringReader(post)));
            //反序列化Response
            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<DyjResponse>(result.ToString());

            //测试收款返回值
            var ex = new Exception(result.ToString());
            ex.Source += "PostDyjReceipt";
            ex.CcsLog("收款调用大赢家返回值");

            if (!response.isSuccess || response.status != "200")
            {
                var ex1 = new Exception(response.message);
                ex1.CcsLog("收款调用大赢家错误");
                return "";
            }
            else
            {
                return response.data.ToString();
            }
        }
    }


    public class DyjReceiptMain
    {
        /// <summary>
        /// 金库ID
        /// </summary>
        public string FiskID { get; set; }

        /// <summary>
        /// 账户ID
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// 币种ID 1-人民币 2-美元 3-港币 4-欧元 5-英镑 7-日元
        /// </summary>
        public string CurrencyID { get; set; }

        /// <summary>
        /// 付款单位
        /// </summary>
        public string Client { get; set; }

        /// <summary>
        /// 收款方式ID 1-现金 2-支票 3-电汇 4-承兑 5-转账 6-同城转账 7-内部转账 9-预收(付)转应收(付)
        /// </summary>
        public string BalanceStyle { get; set; }

        /// <summary>
        /// 外币金额
        /// </summary>
        public decimal ForeignAmount { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// 反款金额
        /// </summary>
        public decimal FKAmount { get; set; }
    }

    public class DyjReceiptDeta
    {
        /// <summary>
        /// 收款类型ID,3 ： 预收账款
        /// </summary>
        public string BillType { get; set; }

        /// <summary>
        /// 公司 默认：6900
        /// </summary>
        public string CorpID { get; set; }

        /// <summary>
        /// 部门 默认：6901
        /// </summary>
        public string DeptID { get; set; }

        /// <summary>
        /// 员工：业务员ID
        /// </summary>
        public string EmployeeID { get; set; }

        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }

    }


}
