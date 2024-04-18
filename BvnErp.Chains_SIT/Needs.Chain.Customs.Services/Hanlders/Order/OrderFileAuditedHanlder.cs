using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Hanlders
{
    /// <summary>
    /// 跟单员审核客户上传文件（对账单、代理报关委托书等）的委托定义
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void OrderFileAuditedHanlder(object sender, OrderFileAuditedEventArgs e);

    /// <summary>
    /// 审核订单附件参数
    /// </summary>
    public class OrderFileAuditedEventArgs : EventArgs
    {
        public Models.OrderAgentProxy AgentProxy { get; set; }

        public Models.OrderBill Bill { get; set; }

        public OrderFileAuditedEventArgs(Models.OrderAgentProxy agentProxy)
        {
            this.AgentProxy = agentProxy;
        }

        public OrderFileAuditedEventArgs(Models.OrderBill bill)
        {
            this.Bill = bill;
        }

        public OrderFileAuditedEventArgs() { }
    }
}
