using System;
using System.Collections.Generic;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Models.Rolls
{
    /// <summary>
    /// 背书
    /// </summary>
    public class EndorsementDto : IEntity
    {
        public string ID { get; set; }

        /// <summary>
        /// 汇票ID
        /// </summary>
        public string MoneyOrderID { get; set; }

        /// <summary>
        /// 背书人账户ID
        /// </summary>
        public string PayerAccountID { get; set; }
        public string PayerAccountName { get; set; }

        /// <summary>
        /// 被背书人账户ID
        /// </summary>
        public string PayeeAccountID { get; set; }

        /// <summary>
        /// 被背书人账户名称
        /// </summary>
        public string PayeeAccountName { get; set; }

        /// <summary>
        /// 背书日期
        /// </summary>
        public DateTime EndorseDate { get; set; }

        public string EndorseDateString => EndorseDate.ToString("yyyy-MM-dd");

        /// <summary>
        /// 是否允许转让
        /// </summary>
        public bool IsTransfer { get; set; }

        public string IsTransferString => IsTransfer ? "是" : "否";

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }

        public string CreatorName { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        public string CreateDateString => CreateDate.ToString("yyyy-MM-dd");
    }
}