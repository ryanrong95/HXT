using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;

namespace YaHv.Csrm.Services
{
    /// <summary>
    /// 主键类型
    /// </summary>
    enum PKeyType
    {
        /// <summary>
        /// AddName:BJ，HK
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("", PKeySigner.Mode.Normal, 2)]
        WareHouse,
       
        /// <summary>
        /// 网站用户登录账号
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("User", PKeySigner.Mode.Normal, 6)]
        User = 100000,
        /// <summary>
        /// 流水账
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("Flow", PKeySigner.Mode.Date, 6)]
        FlowAccount = 200000,
        /// <summary>
        /// 财务科目
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("Subject", PKeySigner.Mode.Normal, 5)]
        Subject = 300000,

        /// <summary>
        /// 物流
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("WL", PKeySigner.Mode.Normal, 5)]
        WL = 10000,
        /// <summary>
        /// 芯达通
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("XL", PKeySigner.Mode.Normal, 5)]
        XL = 20000,
        /// <summary>
        /// 芯达通
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("ICG", PKeySigner.Mode.Normal, 5)]
        ICGO = 30000,

        /// <summary>
        /// 代仓储协议
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("WsCon", PKeySigner.Mode.Date, 5)]
        WsCon = 30000,



        #region 代理线品牌相关
        /// <summary>
        ///品牌
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("Brand", PKeySigner.Mode.Date, 3)]
        Brand = 12001,

        /// <summary>
        ///品牌同义词
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("BrandDic", PKeySigner.Mode.Date, 3)]
        BrandDic = 12002,

        /// <summary>
        ///品牌
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("nBrand", PKeySigner.Mode.Date, 3)]
        nBrand = 12003,
        /// <summary>
        ///品牌
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("vBrand", PKeySigner.Mode.Date, 3)]
        vBrand = 12004,
        #endregion

        #region 代仓储客户相关

        /// <summary>
        /// 付款人
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("Payer", PKeySigner.Mode.Normal, 4)]
        Payer = 11000,

        /// <summary>
        /// 收款人
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("Payee", PKeySigner.Mode.Normal, 4)]
        Payee = 11001,
        /// <summary>
        /// 私有付款人
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("nPayer", PKeySigner.Mode.Normal, 5)]
        nPayer = 11003,
        /// <summary>
        /// 私有收款人
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("nPayee", PKeySigner.Mode.Normal, 5)]
        nPayee = 11004,
        /// <summary>
        /// 私有供应商
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("nSupplier", PKeySigner.Mode.Normal, 5)]
        nSupplier = 11005,
        /// <summary>
        /// 私有交货地址
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("nConsignor", PKeySigner.Mode.Normal, 5)]
        nConsignor = 11006,
        /// <summary>
        /// 私有联系人
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("nContact", PKeySigner.Mode.Normal, 5)]
        nContact = 11007,
        /// <summary>
        /// 企业
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("Co", PKeySigner.Mode.Date, 3)]
        Enterprise = 11008,
        /// <summary>
        /// 个人发票
        /// </summary>
        [Repository(typeof(PvbCrmReponsitory))]
        [PKey("vInv", PKeySigner.Mode.Date, 3)]
        vInvoice = 11009,
        #endregion
        /// <summary>
        /// 中心库文件
        /// </summary>
        [Repository(typeof(PvCenterReponsitory))]
        [PKey("F", PKeySigner.Mode.Date, 4)]
        FileDecription = 13001,
    }
}
