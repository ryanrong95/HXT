using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 库房费用取消事件
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">封箱事件参数</param>
    public delegate void OrderWhesPremiumCanceledHanlder(object sender, OrderWhesPremiumCanceledEventArgs e);

    /// <summary>
    /// 库房费用审批事件
    /// </summary>
    /// <param name="sender">发出者</param>
    /// <param name="e">封箱事件参数</param>
    public delegate void OrderWhesPremiumApprovaledHanlder(object sender, OrderWhesPremiumApprovaledEventArgs e);

    /// <summary>
    /// 事件参数
    /// </summary>
    public class OrderWhesPremiumCanceledEventArgs : EventArgs
    {
        public string OrderWhesPremiumID { get; set; }

        public OrderWhesPremium OrderWhesPremium { get; set; }

        public OrderWhesPremiumCanceledEventArgs(string ID)
        {
            this.OrderWhesPremiumID = ID;
        }

        public OrderWhesPremiumCanceledEventArgs(OrderWhesPremium orderWhesPremium)
        {
            this.OrderWhesPremium = orderWhesPremium;
        }
    }

    /// <summary>
    /// 事件参数
    /// </summary>
    public class OrderWhesPremiumApprovaledEventArgs : EventArgs
    {
        public string OrderWhesPremiumID { get; set; }

        public OrderWhesPremium OrderWhesPremium { get; set; }

        public OrderWhesPremiumApprovaledEventArgs(string ID)
        {
            this.OrderWhesPremiumID = ID;
        }

        public OrderWhesPremiumApprovaledEventArgs(OrderWhesPremium orderWhesPremium)
        {
            this.OrderWhesPremium = orderWhesPremium;
        }
    }
}