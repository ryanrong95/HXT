using Layer.Data.Sqls;
using Layer.Linq;

namespace Needs.Overall
{
    /// <summary>
    /// 主键类型
    /// </summary>
    public enum PKeyType
    {
        /// <summary>
        /// 申请
        /// </summary>
        [Repository(typeof(CvOssReponsitory))]
        [PKey(nameof(Apply), PKeySigner.Mode.Time, 10)]
        Apply = 1000,

        /// <summary>
        /// 申请
        /// </summary>
        [Repository(typeof(IC360CvOssReponsitory))]
        [PKey(nameof(Ic360Apply), PKeySigner.Mode.Time, 10)]
        Ic360Apply = 1050,

        /// <summary>
        /// 申请
        /// </summary>
        [Repository(typeof(IC360CvOssReponsitory))]
        [PKey(nameof(Ic360SampleApply), PKeySigner.Mode.Time, 10)]
        Ic360SampleApply = 1055,

        /// <summary>
        /// 换货申请
        /// </summary>
        [Repository(typeof(CvCSReponsitory))]
        [PKey(nameof(CvCSReplace), PKeySigner.Mode.Time, 10)]
        CvCSReplace = 1056,
        
        /// <summary>
        /// 退货申请
        /// </summary>
        [Repository(typeof(CvCSReponsitory))]
        [PKey(nameof(CvCSReturn), PKeySigner.Mode.Time, 10)]
        CvCSReturn = 1057,

        /// <summary>
        /// 审批退/换货日志
        /// </summary>
        [Repository(typeof(CvCSReponsitory))]
        [PKey(nameof(CvCSLog), PKeySigner.Mode.Time, 10)]
        CvCSLog = 1058,

        /// <summary>
        /// 项目重构CvSso/NewSso注册
        /// </summary>
        [Repository(typeof(CvSsoReponsitory))]
        [PKey(nameof(NUS), PKeySigner.Mode.Time, 10)]
        NUS = 1100,  
           
        /// <summary>
        /// Bom单
        /// </summary>
        [Repository(typeof(BvnErpReponsitory))]
        [PKey(nameof(Bom), PKeySigner.Mode.Time, 10)]
        Bom = 2000,

        /// <summary>
        /// NewBom单
        /// </summary>
        [Repository(typeof(BvnErpReponsitory))]
        [PKey(nameof(NewBom), PKeySigner.Mode.Time, 10)]
        NewBom = 2050,



        /// <summary>
        /// IC360Bom
        /// </summary>
        [Repository(typeof(IC360VAOIPReponsitory))]
        [PKey("Bom", PKeySigner.Mode.Time, 10)]
        IC360Bom = 2060,
        /// <summary>
        /// IC360Inquiry
        /// </summary>
        [Repository(typeof(IC360VAOIPReponsitory))]
        [PKey("Inquiry", PKeySigner.Mode.Time, 10)]
        IC360Inquiry = 2070,



        /// <summary>
        /// Admin登录token
        /// </summary>
        [Repository(typeof(BvnErpReponsitory))]
        [PKey("Login", PKeySigner.Mode.Time, 10)]
        LoginToken = 10000,

        /// <summary>
        /// 管理员
        /// </summary>
        [Repository(typeof(BvnErpReponsitory))]
        [PKey(nameof(Admin), PKeySigner.Mode.Normal, 10)]
        Admin = 40000,

        /// <summary>
        /// 角色
        /// </summary>
        [Repository(typeof(BvnErpReponsitory))]
        [PKey(nameof(Role), PKeySigner.Mode.Normal, 10)]
        Role = 41000,

        /// <summary>
        /// 记账单
        /// </summary>
        [Repository(typeof(VASSOReponsitory))]
        [PKey(nameof(Account),PKeySigner.Mode.Time,10)]
        Account=42000,

        #region Order
        /// <summary>
        /// 订单
        /// </summary>
        [Repository(typeof(IC360CvOssReponsitory))]
        [PKey("D", PKeySigner.Mode.Time, 10)]
        Order = 10000,

        /// <summary>
        /// 发票
        /// </summary>
        [Repository(typeof(IC360CvOssReponsitory))]
        [PKey("IVE", PKeySigner.Mode.Normal, 10)]
        Invoice = 10001,

        /// <summary>
        /// 产品 [销项标识]
        /// </summary>
        [Repository(typeof(IC360CvOssReponsitory))]
        [PKey("SON", PKeySigner.Mode.Normal, 10)]
        ServiceOutput = 10007,
        /// <summary>
        /// 附加价值
        /// </summary>
        [Repository(typeof(IC360CvOssReponsitory))]
        [PKey("PUM", PKeySigner.Mode.Normal, 10)]
        Premium = 10008,

        /// <summary>
        /// 运输条款
        /// </summary>
        [Repository(typeof(IC360CvOssReponsitory))]
        [PKey("STM", PKeySigner.Mode.Normal, 10)]
        ShipmentTerm = 10009,
        /// <summary>
        /// 运单
        /// </summary>
        [Repository(typeof(IC360CvOssReponsitory))]
        [PKey("WBL", PKeySigner.Mode.Normal, 10)]
        Waybill = 10010,
        /// <summary>
        /// 运单项
        /// </summary>
        [Repository(typeof(IC360CvOssReponsitory))]
        [PKey("WBLI", PKeySigner.Mode.Normal, 10)]
        WaybillItem = 10011,
        /// <summary>
        /// 订单
        /// </summary>
        [Repository(typeof(IC360CvOssReponsitory))]
        [PKey("DI", PKeySigner.Mode.Time, 10)]
        OrderItem = 10012,
        /// <summary>
        /// 用户收入
        /// </summary>
        [Repository(typeof(IC360CvOssReponsitory))]
        [PKey("UIP", PKeySigner.Mode.Normal, 10)]
        UserInput = 20001,
        /// <summary>
        /// 用户支出
        /// </summary>
        [Repository(typeof(IC360CvOssReponsitory))]
        [PKey("UOP", PKeySigner.Mode.Normal, 10)]
        UserOutput = 20002,
        /// <summary>
        /// 订单项 Log
        /// </summary>
        [Repository(typeof(IC360CvOssReponsitory))]
        [PKey("OIL", PKeySigner.Mode.Normal, 10)]
        OrderItemLog = 20003,
        /// <summary>
        /// 购物车
        /// </summary>
        [Repository(typeof(IC360CvOssReponsitory))]
        [PKey("SNU", PKeySigner.Mode.Time, 10)]
        Cart = 87000,
        PayExchangeApplyLog = 87001,
        #endregion
    }
}
