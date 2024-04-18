using System;
using Needs.Utils.Descriptions;

namespace Needs.Ccs.Services.Models
{
    public class Input
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string OriginID { get; set; }
        public string OrderID { get; set; }
        public string TinyOrderID { get; set; }
        public string ItemID { get; set; }
        public string ProductID { get; set; }
        public string ClientID { get; set; }
        /// <summary>
        /// 收款人
        /// </summary>
        public string PayeeID { get; set; }

        /// <summary>
        /// 第三方收款人
        /// </summary>
        public string ThirdID { get; set; }
        public string TrackerID { get; set; }
        public string SalerID { get; set; }
        public string PurchaserID { get; set; }
        public Currency Currency { get; set; }
        public decimal? UnitPrice { get; set; }
        public string DateCode { get; set; }
        public string Origin { get; set; }
        public string OriginDescription
        {
            get
            {
                if (string.IsNullOrEmpty(this.OrderID))
                {
                    return "";
                }
                Yahv.Underly.Origin origin;
                if (Yahv.Underly.Origin.TryParse(this.Origin, out origin))
                {
                    return origin.GetDescription();
                }
                else
                {
                    return "";
                }
            }
        }


        public DateTime CreateDate { get; set; }
    }
}
