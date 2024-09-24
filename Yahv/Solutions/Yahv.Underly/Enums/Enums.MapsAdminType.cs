using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 关系类型
    /// </summary>
    public enum MapsType
    {
        /// <summary>
        /// 服务协议
        /// </summary>
        [Description("服务协议")]
        ServiceAgreement = 1,
        /// <summary>
        /// 供应商
        /// </summary>
        [Description("传统贸易供应商")]
        Supplier = 10,
        /// <summary>
        /// 代仓储供应商
        /// </summary>
        [Description("代仓储供应商")]
        WsSupplier = 15,
        /// <summary>
        /// 客户
        /// </summary>
        [Description("传统贸易客户")]
        Client = 20,

        /// <summary>
        /// 代仓储客户
        /// </summary>
        [Description("代仓储客户")]
        WsClient = 25,
        /// <summary>
        /// 内部公司
        /// </summary>
        [Description("内部公司")]
        Company = 30,
        /// <summary>
        /// 受益人
        /// </summary>
        [Description("受益人")]
        Beneficiary = 71,
        /// <summary>
        /// 到货地址
        /// </summary>
        [Description("到货地址")]
        Consignee = 72,
        /// <summary>
        /// 联系人
        /// </summary>
        [Description("联系人")]
        Contact = 73,
        /// <summary>
        /// 发票
        /// </summary>
        [Description("发票")]
        Invoice = 74,
        /// <summary>
        /// 业务员
        /// </summary>
        [Description("华芯通业务员")]
        ServiceManager = 75,
        /// <summary>
        /// 跟单员
        /// </summary>
        [Description("跟单员")]
        Merchandiser = 76,
        /// <summary>
        /// 提货地址
        /// </summary>
        [Description("提货地址")]
        Consignor = 77,
        /// <summary>
        /// 运输工具
        /// </summary>
        [Description("运输工具")]
        Transport = 78,

        /// <summary>
        /// 合同
        /// </summary>
        [Description("合同")]
        Contract = 79,
        /// <summary>
        /// 网站用户
        /// </summary>
        [Description("网站用户")]
        SiteUser = 80,
        /// <summary>
        /// 司机
        /// </summary>
        [Description("司机")]
        Driver = 81,
        /// <summary>
        /// 引荐人
        /// </summary>
        [Description("引荐人")]
        Referrer = 82

    }
}
