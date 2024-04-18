using Yahv.Underly;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 发票
    /// </summary>
    public class Invoice : Linq.IUnique
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Invoice()
        {

        }
        #region 属性

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 企业电话
        /// </summary>
        public string CompanyTel { set; get; }
        /// <summary>
        /// 企业名称
        /// </summary>
        public string CompanyName { set; get; }
        /// <summary>
        /// 发票类型 1 普通发票 2 增值税发票 3 海关发票
        /// </summary>
        public InvoiceType Type { get; set; }
        /// <summary>
        /// 开户银行
        /// </summary>
        public string Bank { get; set; }
        /// <summary>
        /// 开户行地址
        /// </summary>
        public string BankAddress { get; set; }
        /// <summary>
        /// 银行账号
        /// </summary>
        public string Account { set; get; }
        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxperNumber { get; set; }
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
        /// 收货地区
        /// </summary>
        public District District { set; get; }
        /// <summary>
        /// 收货地址
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string Postzip { set; get; }

        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus Status { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }
        /// <summary>
        /// 统一社会信用编码，纳税人识别号
        /// </summary>
        public string Uscc { set; get; }
        /// <summary>
        /// 交付类型
        /// </summary>
        public Underly.InvoiceDeliveryType DeliveryType { set; get; }
        /// <summary>
        /// 企业注册地址
        /// </summary>
        public string RegAddress { set; get; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string AdminID { get; set; }
        #endregion

        #region 扩展属性
        /// <summary>
        /// 企业
        /// </summary>
        public Enterprise Enterprise { get; set; }

        /// <summary>
        /// 类型描述
        /// </summary>
        public string TypeDes
        {
            get
            {
                return this.Type.GetDescription();
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
        /// 状态描述
        /// </summary>
        public string StatusDes
        {
            get
            {
                return this.Status.GetDescription();
            }
        }
        #endregion
    }
}
