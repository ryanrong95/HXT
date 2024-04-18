namespace Yahv.PvWsClient.WebAppNew.Models
{

    #region 供应商
    /// <summary>
    /// 供应商新增页面model
    /// </summary>
    public class AddSupplierViewModel
    {
        /// <summary>
        /// 中文名称
        /// </summary>
        public string ChineseName { get; set; }

        /// <summary>
        /// 简称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 英文名称
        /// </summary>
        public string EnglishName { get; set; }

        /// <summary>
        /// 供应商地址
        /// </summary>
        public int? nGrade { get; set; }

        /// <summary>
        /// 国家地区
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegAddress { get; set; }

        /// <summary>
        /// 银行账户名称
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 银行账户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 银行地区
        /// </summary>
        public string BankPlace { get; set; }

        /// <summary>
        /// 银行币种
        /// </summary>
        public string BankCurrency { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string Bank { get; set; }

        /// <summary>
        /// 银行地址
        /// </summary>
        public string BankAddress { get; set; }

        /// <summary>
        /// 银行代码
        /// </summary>
        public string SwiftCode { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string ContactTel { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string ContactMobile { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        public string ContactQQ { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        public string[] Land { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string DetailAddress { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string AddressName { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string AddressTel { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string AddressMobile { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string AddressEmail { get; set; }

        /// <summary>
        /// 银行
        /// </summary>
        public bool IsBank { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public bool IsContact { get; set; }

        /// <summary>
        /// 提货地址
        /// </summary>
        public bool IsAddress { get; set; }
    }



    /// <summary>
    /// 供应商信息
    /// </summary>
    public class SupplierInfoViewModel
    {
        //ID
        public string ID { get; set; }

        //中文名称
        public string ChineseName { get; set; }

        /// <summary>
        /// 中文简称
        /// </summary>
        public string ChineseSName { get; set; }

        //英文名称
        public string EnglishName { get; set; }

        //备注
        public string Summary { get; set; }

        /// <summary>
        /// 管理员编码
        /// </summary>
        public string AdminCode { get; set; }

        /// <summary>
        /// 级别
        /// </summary>
        public int Grade { get; set; }

        /// <summary>
        /// 法人
        /// </summary>
        public string Corporation { get; set; }

        /// <summary>
        /// 国家/地区
        /// </summary>
        public string Place { get; set; }

        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegAddress { get; set; }

        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        public string Uscc { get; set; }
    }
    /// <summary>
    /// 供应商银行账号
    /// </summary>
    public class BeneficiarieInfoViewModel
    {
        //ID
        public string ID { get; set; }

        public string SupplierID { get; set; }

        public string RealName { get; set; }

        public string Bank { get; set; }

        public string BankAddress { get; set; }

        public string Account { get; set; }

        public string SwiftCode { get; set; }

        public string Method { get; set; }

        public string Currency { get; set; }

        public string Place { get; set; }
    }

    /// <summary>
    /// 供应商提货地址
    /// </summary>
    public class SupplierAddressesViewModel
    {
        //ID
        public string ID { get; set; }

        //供应商编号
        public string SupplierID { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string Postzip { get; set; }

        //联系人姓名
        public string Name { get; set; }

        //联系电话
        public string Tel { get; set; }

        //手机号码
        public string Mobile { get; set; }

        //邮箱
        public string Email { get; set; }

        //是否默认
        public bool IsDefault { get; set; }
    }

    /// <summary>
    /// 供应商联系人
    /// </summary>
    public class SupplierContactModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 供应商ID
        /// </summary>
        public string SupplierID { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        public string QQ { get; set; }


        public string Status { get; set; }
    }
    #endregion


    /// <summary>
    /// 发票对象
    /// </summary>
    public class InvoiceViewModel
    {

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 交付类型
        /// </summary>
        public string DeliveryType { set; get; }


        public string DeliveryTypeName { get; set; }

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
        public string Type { get; set; }

        public string TypeName { get; set; }
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
        /// 企业注册地址
        /// </summary>
        public string RegAddress { set; get; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string Postzip { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 联系人电话
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
        /// 开票地址
        /// </summary>
        public string Address { get; set; }

        public string InvoiceDeliveryTypeOptions { get; set; }

        public string InvoiceTypeOptions { get; set; }

        /// <summary>
        /// 是否主账号
        /// </summary>
        public bool IsMain { get; set; }

        /// <summary>
        /// 开票地址数组
        /// </summary>
        public string[] AddressArray { get; set; }

        /// <summary>
        /// 开票地址详情
        /// </summary>
        public string AddressDetail { get; set; }
    }

    /// <summary>
    /// 收货人
    /// </summary>
    public class ConsigneeViewModel
    {
        public string ID { get; set; }

        public string Title { get; set; }

        public string Place { get; set; }

        public string[] Address { get; set; }

        public string AddressDetail { get; set; }

        public string Name { get; set; }

        public string Tel { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public bool IsDefault { get; set; }
        
        public string IsDefaultVal { get; set; }

        public string PlaceOptions { get; set; }

        /// <summary>
        /// 默认国家地区
        /// </summary>
        public string PlaceDefault { get; set; }
    }


    /// <summary>
    /// 修改手机号码视图模型
    /// </summary>
    public class MobileViewModel
    {
        /// <summary>
        /// 原手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 原手机号码2
        /// </summary>
        public string Phone2 { get; set; }

        /// <summary>
        /// 新手机号码
        /// </summary>
        public string NewPhone { get; set; }

        /// <summary>
        /// 短信验证码
        /// </summary>
        public string Code { get; set; }

        public bool IsMain { get; set; }
    }

    /// <summary>
    /// 修改邮箱视图模型
    /// </summary>
    public class EmailViewModel
    {
        public string Code { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        public string NewEmail { get; set; }

        public bool IsMain { get; set; }
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    public class PasswordViewModel
    {
        public string Password { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// 个人发票信息
    /// </summary>
    public class PersonInvoiceViewModel
    {
        /// <summary>
        /// vInvoices 的 ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 是否个人发票, 是否企业发票
        /// </summary>
        public string IsPersonalVal { get; set; }

        /// <summary>
        /// 发票类型 Int
        /// </summary>
        public string InvoiceTypeInt { get; set; }

        /// <summary>
        /// 发票抬头
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxNumber { get; set; }

        /// <summary>
        /// 企业注册地址
        /// </summary>
        public string RegAddress { get; set; }

        /// <summary>
        /// 企业电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 开户银行
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 收票地址
        /// </summary>
        public string PostAddress { get; set; }

        /// <summary>
        /// 收票人
        /// </summary>
        public string PostRecipient { get; set; }

        /// <summary>
        /// 收票人联系电话
        /// </summary>
        public string PostTel { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string PostZipCode { get; set; }

        /// <summary>
        /// 支付方式 Int
        /// </summary>
        public string DeliveryTypeInt { get; set; }

        /// <summary>
        /// 是否默认
        /// </summary>
        public string IsDefaultVal { get; set; }
    }

}