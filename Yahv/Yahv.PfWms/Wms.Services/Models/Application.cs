using System;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Underly;

namespace Wms.Services.Models
{
    public class Application : IUnique
    {
        #region 属性

        public string ID { get; set; }

        public string ClientID { get; set; }

        public ApplicationType Type { get; set; }

        /// <summary>
        /// 申请金额
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 申请状态
        /// </summary>
        public GeneralStatus Status { get; set; }

        /// <summary>
        /// 审批状态
        /// </summary>
        public ApplicationStatus ApplicationStatus { get; set; }

        /// <summary>
        /// 收款状态
        /// </summary>
        public ApplicationReceiveStatus ReceiveStatus { get; set; }

        /// <summary>
        /// 付款状态
        /// </summary>
        public ApplicationPaymentStatus PaymentStatus { get; set; }

        /// <summary>
        /// 是否入账
        /// </summary>
        public bool IsEntry { get; set; }

        /// <summary>
        /// 发货时机
        /// </summary>
        public int DelivaryOpportunity { get; set; }

        /// <summary>
        /// 发票投递方式
        /// </summary>
        public CheckDeliveryType CheckDeliveryType { get; set; }

        /// <summary>
        /// 送票承运商
        /// </summary>
        public string CheckCarrier { get; set; }

        /// <summary>
        /// 送票运单号
        /// </summary>
        public string CheckWaybillCode { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 申请日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 收款核销日期
        /// </summary>
        public DateTime? ReceiveDate { get; set; }

        /// <summary>
        /// 付款核销日期
        /// </summary>
        public DateTime? PaymentDate { get; set; }

        /// <summary>
        /// 我方收款账户
        /// </summary>
        public string InCompanyName { get; set; }

        /// <summary>
        /// 我方收款银行
        /// </summary>
        public string InBankName { get; set; }

        /// <summary>
        /// 我方收款账号
        /// </summary>
        public string InBankAccount { get; set; }

        /// <summary>
        /// 我方付款账户
        /// </summary>
        public string OutCompanyName { get; set; }

        /// <summary>
        /// 我方付款银行
        /// </summary>
        public string OutBankName { get; set; }

        /// <summary>
        /// 我方付款账号
        /// </summary>
        public string OutBankAccount { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        #endregion 
    }
}