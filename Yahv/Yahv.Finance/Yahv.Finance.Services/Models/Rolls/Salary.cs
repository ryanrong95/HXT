namespace Yahv.Finance.Services.Models.Rolls
{
    /// <summary>
    /// 薪水实体
    /// </summary>
    public class Salary
    {
        /// <summary>
        /// 流水号
        /// </summary>
        public string FormCode { get; set; }

        /// <summary>
        /// 付款账号
        /// </summary>
        public string PayerCode { get; set; }

        /// <summary>
        /// 付款日期
        /// </summary>
        public string PaymentDate { get; set; }

        /// <summary>
        /// 收款姓名
        /// </summary>
        public string PayeeName { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdCard { get; set; }

        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 收款账号
        /// </summary>
        public string PayeeCode { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price { get; set; }
    }
}