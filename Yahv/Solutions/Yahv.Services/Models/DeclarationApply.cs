using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Services.Models
{
    /// <summary>
    /// 报关申请数据要求
    /// </summary>
    public class DeclarationApply
    {

        /// <summary>
        /// 报关申请项
        /// </summary>
        public List<DeclarationApplyItem> Items { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        public string AdminID { get; set; }
    }



    /// <summary>
    /// 报关申请项
    /// </summary>
    public class DeclarationApplyItem
    {

        /// <summary>
        /// 大订单ID
        /// </summary>
        public string VastOrderID { get; set; }

        /// <summary>
        /// 小订单ID
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 分拣ID
        /// </summary>
        public string SortingID { get; set; }

        /// <summary>
        /// 库存ID
        /// </summary>
        public string StoragesID { get; set; }

        /// <summary>
        /// 订单项ID
        /// </summary>
        public string OrderItemID { get; set; }
    }
}
