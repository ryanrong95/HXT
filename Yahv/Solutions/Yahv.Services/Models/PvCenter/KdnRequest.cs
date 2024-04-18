using System;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Attributes;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Services.Models
{
    public enum PayType
    {
        /// <summary>
        /// 寄付
        /// </summary>
        [Description("寄付")]
        DeliveryPay = 1,

        /// <summary>
        /// 到付
        /// </summary>
        [Description("到付")]
        CollectPay = 2,

        /// <summary>
        /// 月结
        /// </summary>
        [Description("月结")]
        MonthlyPay = 3
    }

    /// <summary>
    /// 快递鸟 请求
    /// </summary>
    public class KdnRequest : IUnique
    {
        public string ID { get; set; }
        public string ShipperCode { get; set; }
        public string OrderCode { get; set; }
        public int ExpType { get; set; }
        public int Quantity { get; set; }
        public PayType PayType { get; set; }
        public string MonthCode { get; set; }
        public string SenderAddress { get; set; }
        public string SenderCompany { get; set; }
        public string SenderName { get; set; }
        public string SenderMobile { get; set; }
        public string SenderTel { get; set; }
        public string ReceiverAddress { get; set; }
        public string ReceiverCompany { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverMobile { get; set; }
        public string ReceiverTel { get; set; }
        public string Remark { get; set; }
        public Currency Currency { get; set; }
        public decimal? Cost { get; set; }
        public decimal? OtherCost { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
