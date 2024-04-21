using System;
using Yahv.Underly;

namespace Yahv.Payments.Models
{
    /// <summary>
    /// 减免信息
    /// </summary>
    public class ReducationDto
    {
        /// <summary>
        /// 应收ID或应付ID
        /// </summary>
        public string ID { get; set; }
        public string Business { get; set; }
        public string Catalog { get; set; }
        public string Subject { get; set; }
        public decimal Price { get; set; }
        public Currency Currency { get; set; }

        public string CurrencyName
        {
            get { return this.Currency.GetDescription(); }
        }

        public string CreateDate { get; set; }
    }
}