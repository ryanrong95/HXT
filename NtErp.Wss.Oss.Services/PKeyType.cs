using Layer.Data.Sqls;
using Layer.Linq;
using Needs.Overall;

namespace NtErp.Wss.Oss.Services
{
    /// <summary>
    /// 主键类型
    /// </summary>
    public enum PKeyType
    {
        /// <summary>
        /// 订单
        /// </summary>
        [Repository(typeof(CvOssReponsitory))]
        [PKey("D", PKeySigner.Mode.Time, 10)]
        Order = 10000,

        /// <summary>
        /// 发票
        /// </summary>
        [Repository(typeof(CvOssReponsitory))]
        [PKey("IVE", PKeySigner.Mode.Normal, 10)]
        Invoice = 10001,

        /// <summary>
        /// 产品 [销项标识]
        /// </summary>
        [Repository(typeof(CvOssReponsitory))]
        [PKey("SON", PKeySigner.Mode.Normal, 10)]
        ServiceOutput = 10007,
        /// <summary>
        /// 附加价值
        /// </summary>
        [Repository(typeof(CvOssReponsitory))]
        [PKey("PUM", PKeySigner.Mode.Normal, 10)]
        Premium = 10008,

        /// <summary>
        /// 运输条款
        /// </summary>
        [Repository(typeof(CvOssReponsitory))]
        [PKey("STM", PKeySigner.Mode.Normal, 10)]
        ShipmentTerm = 10009,
        /// <summary>
        /// 运单
        /// </summary>
        [Repository(typeof(CvOssReponsitory))]
        [PKey("WBL", PKeySigner.Mode.Normal, 10)]
        Waybill = 10010,
        /// <summary>
        /// 运单项
        /// </summary>
        [Repository(typeof(CvOssReponsitory))]
        [PKey("WBLI", PKeySigner.Mode.Normal, 10)]
        WaybillItem = 10011,
        /// <summary>
        /// 订单
        /// </summary>
        [Repository(typeof(CvOssReponsitory))]
        [PKey("DI", PKeySigner.Mode.Time, 10)]
        OrderItem = 10012,
        /// <summary>
        /// 用户收入
        /// </summary>
        [Repository(typeof(CvOssReponsitory))]
        [PKey("UIP", PKeySigner.Mode.Normal, 10)]
        UserInput = 20001,
        /// <summary>
        /// 用户支出
        /// </summary>
        [Repository(typeof(CvOssReponsitory))]
        [PKey("UOP", PKeySigner.Mode.Normal, 10)]
        UserOutput = 20002,
        /// <summary>
        /// 订单项 Log
        /// </summary>
        [Repository(typeof(CvOssReponsitory))]
        [PKey("OIL", PKeySigner.Mode.Normal, 10)]
        OrderItemLog = 20003,
    }
}
