using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvRoute.Services.Enums;


namespace Yahv.PvRoute.Services.Models
{
    /// <summary>
    /// 面单主对象
    /// </summary>
    public class FaceOrder : IUnique
    {

        

        #region 数据库属性
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

        #endregion

        

    }
}
