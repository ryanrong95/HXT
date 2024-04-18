using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Utils.Descriptions;

namespace NtErp.Crm.Services.Enums
{
    /// <summary>
    /// 客户类型
    /// </summary>
    [Flags]
    public enum CustomerType
    {
        /// <summary>
        /// 终端客户
        /// </summary>
        [Description("终端客户")]
        Terminal = 1,

        /// <summary>
        /// 贸易商
        /// </summary>
        [Description("贸易商")]
        Merchant = 2,

        /// <summary>
        /// 待定
        /// </summary>
        [Description("待定")]
        Pending = 3,

    }

    /// <summary>
    /// 客户性质
    /// </summary>
    [Flags]
    public enum CustomerNature
    {
        /// <summary>
        /// 国有或国有控股企业
        /// </summary>
        [Description("国有或国有控股企业")]
        StateOwned = 1,

        /// <summary>
        /// 上市公司
        /// </summary>
        [Description("上市公司")]
        ListedCompany = 2,

        /// <summary>
        /// 私营企业
        /// </summary>
        [Description("私营企业")]
        PrivateCompany = 3,

        /// <summary>
        /// 外资企业
        /// </summary>
        [Description("外资企业")]
        InternationalCompany = 4,

        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 5,

        /// <summary>
        /// 待定
        /// </summary>
        [Description("待定")]
        Pending = 6,
    }

    /// <summary>
    /// 客户所在地区
    /// </summary>
    [Flags]
    public enum CustomerArea
    {
        /// <summary>
        /// 国际
        /// </summary>
        [Description("国际")]
        International = 1,

        /// <summary>
        /// 国内
        /// </summary>
        [Description("国内")]
        Inland = 2,

        /// <summary>
        /// 待定
        /// </summary>
        [Description("待定")]
        Pending = 3,
    }

    /// <summary>
    /// 业务类别
    /// </summary>
    [Flags]
    public enum BusinessType
    {
        /// <summary>
        /// 线上客户
        /// </summary>
        [Description("线上客户")]
        Online = 1,

        /// <summary>
        /// 代理品牌客户
        /// </summary>
        [Description("代理品牌客户")]
        Agency = 2,

        /// <summary>
        /// 贸易客户
        /// </summary>
        [Description("贸易客户")]
        Trade = 3,

        /// <summary>
        /// 服务客户
        /// </summary>
        [Description("服务客户")]
        Service = 4,

        /// <summary>
        /// 混合客户
        /// </summary>
        [Description("混合客户")]
        Mixture = 5,

        /// <summary>
        /// 待定
        /// </summary>
        [Description("待定")]
        Pending = 6,
    }

    /// <summary>
    /// 联系紧密度
    /// </summary>
    [Flags]
    public enum ContactTightness
    {
        /// <summary>
        /// A
        /// </summary>
        [Description("A")]
        A = 1,

        /// <summary>
        /// B
        /// </summary>
        [Description("B")]
        B = 2,

        /// <summary>
        /// C
        /// </summary>
        [Description("C")]
        C = 3,

        /// <summary>
        /// D
        /// </summary>
        [Description("D")]
        D = 4,

        /// <summary>
        /// E
        /// </summary>
        [Description("E")]
        E = 5
    }

    /// <summary>
    /// 客户级别
    /// </summary>
    [Flags]
    public enum CustomerLevel
    {
        /// <summary>
        /// 1
        /// </summary>
        [Description("1")]
        A = 1,

        /// <summary>
        /// 2
        /// </summary>
        [Description("2")]
        B = 2,

        /// <summary>
        /// 3
        /// </summary>
        [Description("3")]
        C = 3,

        /// <summary>
        /// 4
        /// </summary>
        [Description("4")]
        D = 4,

        /// <summary>
        /// 5
        /// </summary>
        [Description("5")]
        E = 5,

        /// <summary>
        /// 6
        /// </summary>
        [Description("6")]
        F = 6,

        /// <summary>
        /// 7
        /// </summary>
        [Description("7")]
        G = 7,

        /// <summary>
        /// 8
        /// </summary>
        [Description("8")]
        H = 8,

        /// <summary>
        /// 9
        /// </summary>
        [Description("9")]
        I = 9,

        /// <summary>
        /// 10
        /// </summary>
        [Description("10")]
        J = 10
    }

    /// <summary>
    /// 客户职位
    /// </summary>
    [Flags]
    public enum CustomerPosition
    {
        /// <summary>
        /// 技术
        /// </summary>
        [Description("技术")]
        Technology = 1,

        /// <summary>
        /// 采购
        /// </summary>
        [Description("采购")]
        Purchase = 2,

        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        Other = 3
    }

    /// <summary>
    /// 公司
    /// </summary>
    [Flags]
    public enum CompanyID
    {
        /// <summary>
        /// A公司
        /// </summary>
        [Description("A公司")]
        CmpA = 1,

        /// <summary>
        /// B公司
        /// </summary>
        [Description("B公司")]
        CmpB = 2
    }


    /// <summary>
    /// 客户状态
    /// </summary>
    [Flags]
    public enum CustomerStatus
    {
        /// <summary>
        /// 潜在客户
        /// </summary>
        [Description("潜在客户")]
        Potential = 10,

        /// <summary>
        /// 意向客户
        /// </summary>
        [Description("意向客户")]
        Intentional = 20,

        /// <summary>
        /// 成交客户
        /// </summary>
        [Description("成交客户")]
        Transaction = 30,

        /// <summary>
        /// 待定
        /// </summary>
        [Description("待定")]
        Pending = 40,

    }


    /// <summary>
    /// 保护级别
    /// </summary>
    [Flags]
    public enum ProtectLevel
    {
        /// <summary>
        /// 1
        /// </summary>
        [Description("独家")]
        A = 1,

        /// <summary>
        /// 2
        /// </summary>
        [Description("共享")]
        B = 2,

    }

    public enum Sex
    {
        [Description("男")]
        Male = 1,

        [Description("女")]
        Female = 2,
    }

    /// <summary>
    /// 客户是否受保护
    /// </summary>
    public enum IsProtected
    {
        [Description("否")]
        No = 0,

        [Description("是")]
        Yes = 1,
    }

    /// <summary>
    /// 币种
    /// </summary>
    [Flags]
    public enum CurrencyType
    {
        /// <summary>
        /// 美元（USD）
        /// </summary>
        [Description("美元")]
        USD = 502,

        /// <summary>
        /// 欧元（EUR）
        /// </summary>
        [Description("欧元")]
        EUR = 300,

        /// <summary>
        /// 港币（HKD）
        /// </summary>
        [Description("港币")]
        HKD = 110,

        /// <summary>
        /// 日元（JPY）
        /// </summary>
        [Description("日元")]
        JPY = 116,

        /// <summary>
        /// 英镑（GBP）
        /// </summary>
        [Description("英镑")]
        GBP = 303,

        /// <summary>
        /// 人民币（CNY）
        /// </summary>
        [Description("人民币")]
        CNY = 142
    }

    public enum InvoiceType
    {
        /// <summary>
        /// 普票
        /// </summary>
        [Description("普通发票")]
        Normal = 1,

        /// <summary>
        /// 增值税
        /// </summary>
        [Description("增值税发票")]
        ValueAdded = 2
    }

    /// <summary>
    /// 收获人地址类型
    /// </summary>
    public enum ConsigneeType
    {
        /// <summary>
        /// 收发票
        /// </summary>
        [Description("发票")]
        Invoice = 1,

        /// <summary>
        /// 收货物
        /// </summary>
        [Description("货物")]
        Goods = 2
    }

    /// <summary>
    /// 重要客户级别
    /// </summary>
    public enum ImportantLevel
    {
        [Description("非重点")]
        Not = 0,
        [Description("1")]
        A = 1,
        [Description("2")]
        B = 2,
        [Description("3")]
        C = 3,
    }
}
