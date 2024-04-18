using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.ApiSettings
{
    public static class FinanceApiSetting
    {
        public static string ApiName = "FinanceApiUrl";
        /// <summary>
        /// 金库地址
        /// </summary>
        public static string VaultUrl = "GoldStore/Enter";
        /// <summary>
        /// 账号地址
        /// </summary>
        public static string AccountUrl = "Account/Enter";
        /// <summary>
        /// 收款地址
        /// </summary>
        public static string ReceiptUrl = "Payee/PayeeLeftEnter";
        /// <summary>
        /// 批量收款地址
        /// </summary>
        public static string ReceiptBatchUrl = "Payee/PayeeLeftBatchEnter";
        /// <summary>
        /// 核销地址
        /// </summary>
        public static string VerificationUrl = "Payee/PayeeRightEnter";
        /// <summary>
        /// 付款地址
        /// </summary>
        public static string SwapUrl = "Payer/ProductFeeEnter";
        /// <summary>
        /// 费用地址
        /// </summary>
        public static string FeeUrl = "Payer/ChargeEnter";
        /// <summary>
        /// 调拨地址
        /// </summary>
        public static string FundTransferUrl = "Payer/FundTransferEnter";
        /// <summary>
        /// 付汇地址
        /// </summary>
        public static string PayExchangeUrl = "Payee/PayeeRightEnter";
        /// <summary>
        /// 费用地址
        /// </summary>
        public static string MultiFeeUrl = "Payer/ChargeBatchEnter";
        /// <summary>
        /// 费用地址
        /// </summary>
        public static string PaymentUrl = "Payer/PaymentEnter";
        /// <summary>
        /// 费用地址
        /// </summary>
        public static string AcceptanceUrl = "MoneyOrder/MoneyOrderEnter";

        /// <summary>
        /// 贴现地址
        /// </summary>
        public static string AcceptanceCharge = "MoneyOrder/AcceptanceEnter";
    }
}
