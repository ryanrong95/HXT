using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ClientConfirm
    {
        public string OrderID { get; set; }

        public string OrderItemID { get; set; }

        public decimal Quantity { get; set; }

        public string AdminID { get; set; }

        public ConfirmType Type { get; set; }

        public List<TinyOrderDeclareFlags> DeclareFlags { get; set; }
    }


    public class TinyOrderDeclareFlags
    {
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 1:不可报关，2：可报关，3已报关
        /// 1、3赋值false，2赋值true
        /// </summary>
        public bool IsDeclare { get; set; }
    }

    public enum ConfirmType
    {
        /// <summary>
        /// 正常报价，确认
        /// </summary>
        [Description("正常确认")]
        Normal = 1,

        [Description("修改数量确认")]
        UpdateQuantity = 2,

        [Description("删除型号确认")]
        DeleteItem = 3,

        /// <summary>
        /// 直接系统自动确认，重发入库通知
        /// </summary>
        [Description("直接确认")]
        DirectConfirm = 4,

        /// <summary>
        /// 跟单处理到货异常，更新订单并归类后，客户确认
        /// </summary>
        [Description("到货异常确认")]
        DeliveryConfirm = 5,
    }
}
