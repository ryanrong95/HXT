using Needs.Ccs.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 客户端拓展类
    /// </summary>
   public class UserOrderExtends:Order
    {
        /// <summary>
        /// 我的订单列表是否显示提货按钮
        /// </summary>
        public bool IsShowTihou
        {
            get;set;
        }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DeclareDate { get; set; }

        /// <summary>
        /// 是否因为修改型号（删除型号/修改数量）而挂起订单
        /// </summary>
        public bool IsBecauseModified { get; set; }
    }
}
