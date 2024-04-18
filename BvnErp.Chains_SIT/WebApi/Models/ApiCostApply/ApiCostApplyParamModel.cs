using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    /// <summary>
    /// 与大赢家交互费用申请的接口-输入参数
    /// </summary>
    public class ApiCostApplyParamModel
    {
        public class HeadOfficeApproveRefuseRequest
        {
            /// <summary>
            /// 单据号
            /// </summary>
            public string BillNo { get; set; } = string.Empty;

            /// <summary>
            /// 备注
            /// </summary>
            public string Summary { get; set; } = string.Empty;
        }

        public class PaymentSuccessNoticeRequest
        {
            /// <summary>
            /// 单据号
            /// </summary>
            public string BillNo { get; set; } = string.Empty;

            /// <summary>
            /// 付款流水号
            /// </summary>
            public string SeqNo { get; set; } = string.Empty;

            /// <summary>
            /// 银行账户
            /// </summary>
            public string BankAccount { get; set; } = string.Empty;

            /// <summary>
            /// 付款凭证文件地址
            /// </summary>
            public string PaymentVoucherUrl { get; set; } = string.Empty;
        }

        public class PaymentFailNoticeRequest
        {
            /// <summary>
            /// 单据号
            /// </summary>
            public string BillNo { get; set; } = string.Empty;

            /// <summary>
            /// 备注
            /// </summary>
            public string Summary { get; set; } = string.Empty;
        }

    }
}