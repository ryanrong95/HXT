using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Yahv.PvWsOrder.Services.XDTClientView;

namespace Yahv.PvWsOrder.WebApi.Models
{
    public class ClientConfirm
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 订单项号
        /// </summary>
        public string OrderItemID { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 管理员
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 确认类型
        /// </summary>
        public ConfirmType Type { get; set; }

        /// <summary>
        /// 报关标志
        /// </summary>
        public List<TinyOrderDeclareFlag> DeclareFlags { get; set; }
    }

    /// <summary>
    /// 报关标志
    /// </summary>
    public class TinyOrderDeclareFlag
    {
        /// <summary>
        /// 小订单号
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 是否报关
        /// </summary>
        public bool IsDeclare { get; set; }
    }


    public class XDTFile
    {
        public string OrderID { get; set; }

        public XDTFileType Type { get; set; }
    }

    public enum ConfirmType
    {
        [Description("正常确认")]
        Normal = 1,

        [Description("修改数量确认")]
        UpdateQuantity = 2,

        [Description("删除型号确认")]
        DeleteItem = 3,

        [Description("不需要确认")]
        DoNothing = 4,

        [Description("到货异常确认")]
        DelivaryError = 5,
    }
}