using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 大赢家数据对接接口类型
    /// </summary>
    public enum DyjInterfaceType
    {
        /// <summary>
        /// 内部公司
        /// </summary>
        [Description("内部公司")]
        Invoicelist = 1,

        /// <summary>
        /// 报关公司
        /// </summary>
        [Description("报关公司")]
        BaoGuanCompanylist = 2,

        /// <summary>
        /// 备案文件信息
        /// </summary>
        [Description("备案文件信息")]
        FilingDocumentslist = 3,

        /// <summary>
        /// 供应商
        /// </summary>
        [Description("供应商")]
        Supplier = 4,

        /// <summary>
        /// 客户
        /// </summary>
        [Description("客户")]
        Client = 5,

    }
}
