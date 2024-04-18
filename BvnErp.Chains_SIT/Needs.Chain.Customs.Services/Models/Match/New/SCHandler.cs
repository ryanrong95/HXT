using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// SplitConfirm 缩写SC
    /// </summary>
    public abstract class SCHandler
    {
        public SCHandler next { get; set; }
        /// <summary>
        /// 页面勾选信息
        /// </summary>
        public List<MatchViewModel> SelectedItems { get; set; }      
        /// <summary>
        /// 拆分之前的订单
        /// </summary>
        public Order OriginOrder { get; set; }
        /// <summary>
        /// 新拆出来的订单
        /// </summary>
        public Order CurrentOrder { get; set; }
  
        public abstract void handleRequest();
    }
}
