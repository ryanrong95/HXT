using System;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    ///主体类型CrmPlus
    /// </summary>
    public enum MainType
    {

        /// <summary>
        /// 客户
        /// </summary>
        [Description("客户")]
        Clients = 1,
        /// <summary>
        /// 供应商
        /// </summary>
        [Description("供应商")]
        Suppliers = 2
       
    }
   
}
