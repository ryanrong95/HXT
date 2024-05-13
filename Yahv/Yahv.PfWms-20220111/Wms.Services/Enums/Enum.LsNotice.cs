using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Wms.Services
{
    ///// <summary>
    ///// 合同类型
    ///// </summary>
    //public enum ContractType
    //{
    //    [Description("租赁")]
    //    Hire = 10,
    //    [Description("交易")]
    //    Trade = 20,
    //    [Description("杂货")]
    //    Sundry=30
    //}

    /// <summary>
    /// 租赁状态
    /// </summary>
    public enum LsNoticeStatus
    {
        [Description("未分配")]
        Unallocated=100,
        [Description("已分配")]
        Allocated = 200,
        [Description("已删除")]
        Deleted=400,
        [Description("已过期")]
        Expired = 401
        
    }
}
