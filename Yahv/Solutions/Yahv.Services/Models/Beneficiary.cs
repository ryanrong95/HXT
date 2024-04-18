using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 受益人
    /// </summary>
    public class Beneficiary : Linq.IUnique
    {
        public Beneficiary()
        {

        }
        #region 属性
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 公司名称（企业名） 实际的企业名称
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 开户银行
        /// </summary>
        public string Bank { get; set; }
        /// <summary>
        /// 开户行地址
        /// </summary>
        public string BankAddress { get; set; }
        /// <summary>
        /// 银行账户
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 银行编码 (国际)
        /// </summary>
        public string SwiftCode { get; set; }
        /// <summary>
        /// 汇款方式
        /// </summary>
        public Methord Methord { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public District District { set; get; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 联系人手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 联系人邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 是否可以带票采购
        /// </summary>
        public InvoiceType InvoiceType { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus Status { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { set; get; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }

        public string AdminID { get; set; }
        #endregion
        #region 扩展属性   /// <summary>
        /// 企业
        /// </summary>
        public Enterprise Enterprise { get; set; }
        /// <summary>
        /// 汇款方式描述
        /// </summary>
        public string MethordDes
        {
            get
            {
                return this.Methord.GetDescription();
            }
        }
        /// <summary>
        /// 币种描述
        /// </summary>
        public string CurrencyDes
        {
            get
            {
                return this.Currency.GetDescription();
            }
        }
        /// <summary>
        /// 币种属性
        /// </summary>
        public ICurrency CurrencyAttr
        {
            get
            {
                return this.Currency.GetCurrency();
            }
        }
        /// <summary>
        /// 地区描述
        /// </summary>
        public string DistrictDes
        {
            get
            {
                return this.District.GetDescription();
            }
        }
        /// <summary>
        /// 开票类型
        /// </summary>
        public string InvoiceTypeDes
        {
            get
            {
                return this.InvoiceType.GetDescription();
            }
        }
        #endregion
    }
}
