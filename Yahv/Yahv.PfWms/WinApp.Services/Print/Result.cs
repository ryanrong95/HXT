using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Services.Print
{
    public class Result
    {


        /// <summary>
        /// 运单编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 运单号类型1：母单 2 :子单 3 : 签回单
        /// </summary>
        public int Type { get; set; }


        /// <summary>
        /// 来源：顺丰/跨越
        /// </summary>
        public int Source { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 对应html
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// 发送内容
        /// </summary>
        public string SendJson { get; set; }

        /// <summary>
        /// 接收内容
        /// </summary>
        public string ReceiveJson { get; set; }

        /// <summary>
        /// 会话对象，可空
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
    }

    #region 舍弃
    //public class Result
    //{
    //    /// <summary>
    //    /// 运单编号
    //    /// </summary>
    //    public string Code { get; set; }

    //    /// <summary>
    //    /// 来源：顺丰/跨越
    //    /// </summary>
    //    public int Source { get; set; }

    //    /// <summary>
    //    /// 创建者
    //    /// </summary>
    //    public string CreatorID { get; set; }

    //    /// <summary>
    //    /// 对应html
    //    /// </summary>
    //    public string Html { get; set; }

    //    /// <summary>
    //    /// 发送内容
    //    /// </summary>
    //    public string SendJson { get; set; }

    //    /// <summary>
    //    /// 接收内容
    //    /// </summary>
    //    public string ReceiveJson { get; set; }

    //    /// <summary>
    //    /// 会话对象，可空
    //    /// </summary>
    //    public string SessionID { get; set; }
    //}

    #endregion

}
