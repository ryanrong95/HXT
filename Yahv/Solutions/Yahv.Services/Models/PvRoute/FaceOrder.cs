using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly.Attributes;

namespace Yahv.Services.Models
{
    public enum PrintSource 
    {
        [Description("顺丰")]
        SF = 10,
        [Description("跨越速运")]
        KYSY = 20
    }

    public enum PrintState
    {
        [Description("正常的")]
        Normal = 10,
        [Description("无效的")]
        Waiting = 20
    }
    public class FaceOrder : IUnique
    {
        public string ID { get; set; }

        /// <summary>
        /// 运单的唯一码，运单号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 来源：10.顺丰，20.跨越
        /// </summary>
        public PrintSource Source { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 面单Html
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// 传递数据
        /// </summary>
        public string SendJson { get; set; }

        /// <summary>
        /// 接收数据
        /// </summary>
        public string ReceiveJson { get; set; }

        /// <summary>
        /// 会话ID：可空设计
        /// </summary>
        public string SessionID { get; set; }

        /// <summary>
        /// 目前指订单ID
        /// </summary>
        public string MainID { get; set; }

        /// <summary>
        /// 根据订单号产生的ID
        /// </summary>
        public string MyID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 状态：10（Normal：有效），20（Waiting：无效）
        /// </summary>
        public PrintState Status { get; set; }
    }
}
