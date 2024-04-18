using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.SzMvc.Services.Enums
{
    /// <summary>
    /// 主键 KeyType
    /// </summary>
    public enum PKeyType
    {
        /// <summary>
        /// 订单
        /// </summary>
        [Repository(typeof(PsOrderRepository))]
        [PKey("", PKeySigner.Mode.Date, 3)]
        Order = 10000, //TODO:客户编号+年月日+3位

        /// <summary>
        /// 订单项
        /// </summary>
        [Repository(typeof(PsOrderRepository))]
        [PKey("OrderItem", PKeySigner.Mode.Date, 6)]
        OrderItem = 10001,

        /// <summary>
        /// 产品
        /// </summary>
        [Repository(typeof(PsOrderRepository))]
        [PKey("Product", PKeySigner.Mode.Date, 6)]
        Product = 10002,

        /// <summary>
        /// 货运信息
        /// </summary>
        [Repository(typeof(PsOrderRepository))]
        [PKey("OrderTransport", PKeySigner.Mode.Date, 6)]
        OrderTransport = 10003,

        /// <summary>
        /// 特殊要求
        /// </summary>
        [Repository(typeof(PsOrderRepository))]
        [PKey("Requires", PKeySigner.Mode.Date, 6)]
        Requires = 10004,

        /// <summary>
        /// 提货人
        /// </summary>
        [Repository(typeof(PsOrderRepository))]
        [PKey("Picker", PKeySigner.Mode.Date, 6)]
        Picker = 10005,

        /// <summary>
        /// 文件
        /// </summary>
        [Repository(typeof(PsOrderRepository))]
        [PKey("PcFile", PKeySigner.Mode.Date, 6)]
        PcFile = 10006,

        /// <summary>
        /// 地址
        /// </summary>
        [Repository(typeof(PsOrderRepository))]
        [PKey("Address", PKeySigner.Mode.Date, 6)]
        Address = 10007,

        /// <summary>
        /// 开票信息
        /// </summary>
        [Repository(typeof(PsOrderRepository))]
        [PKey("Invoice", PKeySigner.Mode.Date, 6)]
        Invoice = 10008,

        /// <summary>
        /// 账单信息
        /// </summary>
        [Repository(typeof(PsOrderRepository))]
        [PKey("Vourcher", PKeySigner.Mode.Date, 6)]
        Vourcher = 10009,

        /// <summary>
        /// 应收信息
        /// </summary>
        [Repository(typeof(PsOrderRepository))]
        [PKey("PayeeLf", PKeySigner.Mode.Date, 6)]
        PayeeLeft = 10010,

        /// <summary>
        /// 实收信息
        /// </summary>
        [Repository(typeof(PsOrderRepository))]
        [PKey("PayeeRight", PKeySigner.Mode.Date, 6)]
        PayeeRight = 10011,
    }
}
