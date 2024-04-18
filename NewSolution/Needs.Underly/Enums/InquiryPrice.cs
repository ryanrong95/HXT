using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Underly.Enums
{

    public enum InquiryType
    {
        Bom = 1,
        Single = 2
    }
    public enum ProductPriceInquiryType
    {
        Inquiry=1,
        PCB=2
    }

    public enum InquiryStatus
    {
        Pending = 100,
        Quoted = 200,
        Deleted=400,
        Error = 500
    }

    public enum FromType
    {
        IC360=1
    }

    public enum ProductPriceInquiryStatus
    {
        /// <summary>
        /// 待回复
        /// </summary>
        Restore = 1,
        /// <summary>
        /// 已回复
        /// </summary>
        Restored = 2,
        /// <summary>
        /// 申请购买
        /// </summary>
        App = 3,
        /// <summary>
        /// 删除
        /// </summary>
        Delete = 400,
        /// <summary>
        /// 已经被购买
        /// </summary>
        Ordered = 10,
    }


    public enum InquiryItemStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 200,
        /// <summary>
        /// 已经被购买
        /// </summary>
        Ordered = 300,
    }


}
