using System;
using Yahv.Underly.Attributes;

namespace Yahv.Underly.CrmPlus
{
    /// <summary>
    ///内部公司类型
    /// </summary>
    [Flags]
    public enum CompanyType
    {

        /// <summary>
        /// 线上
        /// </summary>
        [Description("线上")]
        Online = 1,
        /// <summary>
        /// 贸易
        /// </summary>
        [Description("贸易")]
        Trade = 2,
        /// <summary>
        /// 报关
        /// </summary>
        [Description("报关")]
        Chains = 4,
       
    }

    
}
