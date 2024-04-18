using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 产品抽检异常
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">封箱事件参数</param>
    public delegate void SpotAbnormalHanlders(object sender, SpotAbnormalEventArgs e);

    /// <summary>
    /// 事件参数
    /// </summary>
    public class SpotAbnormalEventArgs : EventArgs
    {
        public OrderItem OrderItem { get; set; }

        public SpotAbnormalEventArgs(OrderItem orderItem)
        {
            this.OrderItem = orderItem;
        }
    }
}