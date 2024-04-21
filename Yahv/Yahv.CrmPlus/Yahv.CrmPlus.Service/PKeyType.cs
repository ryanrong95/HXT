using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;

namespace Yahv.CrmPlus.Service
{
    enum PKeyType
    {
        #region 标准品牌
        /// <summary>
        ///品牌
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Brand", PKeySigner.Mode.Date, 3)]
        Brand = 12001,
        #endregion

        /// <summary>
        ///标准型号
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Spn", PKeySigner.Mode.Date, 3)]
        StandardPartNumber = 12002,
        /// <summary>
        ///企业
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Ep", PKeySigner.Mode.Date, 3)]
        Enterprise = 1,

        /// <summary>
        ///企业基本信息
        /// </summary>
        //[Repository(typeof(PvdCrmReponsitory))]
        //[PKey("ER", PKeySigner.Mode.Date, 3)]
        //EnterpriseRegister = 2,


        /// <summary>
        ///日志
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Log", PKeySigner.Mode.Date, 4)]
        Log = 3,
        /// <summary>
        /// 发票
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("INV", PKeySigner.Mode.Date, 3)]
        Invoices = 4,

        /// <summary>
        /// 可维护枚举
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Enum", PKeySigner.Mode.Normal, 4)]
        Enums = 5,
        /// <summary>
        /// 联系人
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Ct", PKeySigner.Mode.Date, 5)]
        Contacts = 6,
        /// <summary>
        /// 收付款
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Ba", PKeySigner.Mode.Date, 3)]
        BookAccounts = 7,


        /// <summary>
        /// 业务类型
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Cd", PKeySigner.Mode.Date, 3)]
        Conducts = 8,

        //// <summary>
        ///// 供应商
        ///// </summary>
        //[Repository(typeof(PvdCrmReponsitory))]
        //[PKey("Supplier", PKeySigner.Mode.Date, 3)]
        //Supplier = 9,
        // <summary>
        /// 文件
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("F", PKeySigner.Mode.Date, 5)]
        File = 10,

        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Ar", PKeySigner.Mode.Date, 5)]
        Addresse = 11,

        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Tra", PKeySigner.Mode.Date, 8)]
        Trace = 12,
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Com", PKeySigner.Mode.Date, 8)]
        Comment = 13,

        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("pro", PKeySigner.Mode.Date, 6)]
        Project = 14,

        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("proItem", PKeySigner.Mode.Date, 8)]
        ProjectProduct = 14,
        /// <summary>
        ///报备
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Report", PKeySigner.Mode.Date, 6)]
        ProjectReport = 15,
        /// <summary>
        /// 竞品
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Compelete", PKeySigner.Mode.Date, 6)]
        Compelete = 16,
        /// <summary>
        /// 送样
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Sample", PKeySigner.Mode.Date, 5)]
        Sample = 17,
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("SampleItem", PKeySigner.Mode.Date, 6)]
        SampleIt = 18,
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("AgentQuote", PKeySigner.Mode.Date, 8)]
        AgentQuotes = 19,

        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Me", 8)]
        MapsEnterprise = 20,
        /// <summary>
        /// PcFiles
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("PcFile", PKeySigner.Mode.Date, 5)]
        PcFile = 21,
      
        /// <summary>
        /// Commission
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Commission", PKeySigner.Mode.Date, 5)]
        Commission = 22,
        /// <summary>
        /// Control
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Control", PKeySigner.Mode.Date, 5)]
        Control = 23,
        /// <summary>
        /// Credit授信
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Credit", PKeySigner.Mode.Date, 5)]
        Credit = 24,
        /// <summary>
        /// Dishonest失信记录
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Dishonest", PKeySigner.Mode.Date, 5)]
        Dishonest = 25,
        /// <summary>
        /// 信用流水
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Flows", PKeySigner.Mode.Date, 5)]
        Flows = 26,
        /// <summary>
        /// 品牌关系
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("nBrand", PKeySigner.Mode.Date, 5)]
        nBrand = 27,
        /// <summary>
        /// 固定渠道
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("nFixedBrand", PKeySigner.Mode.Date, 5)]
        nFixedBrand = 28,
        /// <summary>
        /// 关联关系
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Relations", PKeySigner.Mode.Date, 5)]
        Relations = 29,
        /// <summary>
        /// ApplyTasks
        /// </summary>
        [Repository(typeof(PvdCrmReponsitory))]
        [PKey("Apply", PKeySigner.Mode.Date, 5)]
        ApplyTasks = 30,


    }
}
