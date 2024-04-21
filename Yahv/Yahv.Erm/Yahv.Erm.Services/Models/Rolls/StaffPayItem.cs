namespace Yahv.Erm.Services.Models.Rolls
{
    /// <summary>
    /// 员工工资
    /// </summary>
    public class StaffPayItem
    {
        /// <summary>
        /// 月账单ID
        /// </summary>
        public string PayID { get; set; }

        /// <summary>
        /// 员工ID
        /// </summary>
        public string StaffID { get; set; }

        /// <summary>
        /// 员工姓名
        /// </summary>
        public string StaffName { get; set; }

        /// <summary>
        /// 员工编码
        /// </summary>
        public string StaffCode { get; set; }

        /// <summary>
        /// 员工自定义编码
        /// </summary>
        public string StaffSelCode { get; set; }

        /// <summary>
        /// 工资月
        /// </summary>
        public string DateIndex { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 大赢家公司编码
        /// </summary>
        public string DyjCompanyCode { get; set; }

        /// <summary>
        /// 大赢家部门编码
        /// </summary>
        public string DyjDepartmentCode { get; set; }

        /// <summary>
        /// 大赢家员工ID
        /// </summary>
        public string DyjCode { get; set; }

        /// <summary>
        /// 岗位名称
        /// </summary>
        public string PostionName { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string IDCard { get; set; }

        /// <summary>
        /// 银行卡号
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 工资项
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 工资项值
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// 工资类型
        /// </summary>
        public WageItemType WageType { get; set; }

        /// <summary>
        /// 员工状态
        /// </summary>
        public int StaffStatus { get; set; }

        /// <summary>
        /// 员工状态
        /// </summary>
        public string StaffStatusName { get; set; }
    }
}