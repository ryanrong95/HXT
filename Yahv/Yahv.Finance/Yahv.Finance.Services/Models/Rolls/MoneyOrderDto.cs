using System;
using Yahv.Finance.Services.Enums;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Models
{
    /// <summary>
    /// 承兑汇票传输类
    /// </summary>
    public class MoneyOrderDto : IEntity
    {
        public string ID { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public MoneyOrderType Type { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 票据号码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string BankCode { get; set; }

        /// <summary>
        /// 开户行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 开户行行号
        /// </summary>
        public string BankNo { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 出票人账户ID
        /// </summary>
        public string PayerAccountID { get; set; }

        /// <summary>
        /// 出票人
        /// </summary>
        public string PayerAccountName { get; set; }

        /// <summary>
        /// 是否允许转让
        /// </summary>
        public bool IsTransfer { get; set; }

        /// <summary>
        /// 当前持票人账户ID
        /// </summary>
        public string PayeeAccountID { get; set; }

        /// <summary>
        /// 当前持票人名称
        /// </summary>
        public string PayeeAccountName { get; set; }

        /// <summary>
        /// 出票日期
        /// </summary>
        public DateTime StartDate { get; set; }

        public string StartDateString => StartDate.ToString("yyyy-MM-dd");

        /// <summary>
        /// 汇票到期日
        /// </summary>
        public DateTime EndDate { get; set; }
        public string EndDateString => EndDate.ToString("yyyy-MM-dd");

        /// <summary>
        /// 承兑性质
        /// </summary>
        public MoneyOrderNature Nature { get; set; }

        /// <summary>
        /// 兑换日期
        /// </summary>
        public DateTime? ExchangeDate { get; set; }

        /// <summary>
        /// 兑换金额
        /// </summary>
        public decimal? ExchangePrice { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        public string CreateDateString => CreateDate.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// 修改人ID
        /// </summary>
        public string ModifierID { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 状态名称
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// 是否能贴现
        /// </summary>
        public bool IsMoney { get; set; }
    }
}