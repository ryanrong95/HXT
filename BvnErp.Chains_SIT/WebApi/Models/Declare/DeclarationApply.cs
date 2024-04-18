using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    /// <summary>
    /// 报关申请数据要求
    /// </summary>
    public class DeclarationApply
    {
        /// <summary>
        /// 报关申请内容
        /// </summary>
        public List<DeclarationApplyItem> Items { get; set; }
    }

    /// <summary>
    /// 一个分拣/拣货
    /// </summary>
    public class DeclarationApplyItem
    {
        /// <summary>
        /// 小订单ID
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 分拣/拣货ID
        /// </summary>
        public string DeclareID { get; set; }
    }
}