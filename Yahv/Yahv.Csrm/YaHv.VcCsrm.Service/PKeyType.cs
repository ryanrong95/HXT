using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;

namespace YaHv.VcCsrm.Services
{
    /// <summary>
    /// 主键类型
    /// </summary>
    enum PKeyType
    {
        /// <summary>
        /// AddName:BJ，HK
        /// </summary>
        [Repository(typeof(PvcCrmReponsitory))]
        [PKey("", PKeySigner.Mode.Normal, 2)]
        WareHouse,
        /// <summary>
        /// 企业
        /// </summary>
        //[Repository(typeof(PvbCrmReponsitory))]
        //[PKey("", PKeySigner.Mode.Quarter, 5)]
        //Enterprise = 10000,
        /// <summary>
        /// 网站用户登录账号
        /// </summary>
        [Repository(typeof(PvcCrmReponsitory))]
        [PKey("User", PKeySigner.Mode.Normal, 6)]
        User = 100000,
        /// <summary>
        /// 流水账
        /// </summary>
        [Repository(typeof(PvcCrmReponsitory))]
        [PKey("Flow", PKeySigner.Mode.Date, 6)]
        FlowAccount = 200000,
        /// <summary>
        /// 财务科目
        /// </summary>
        [Repository(typeof(PvcCrmReponsitory))]
        [PKey("Subject", PKeySigner.Mode.Normal, 5)]
        Subject = 300000,

        /// <summary>
        /// 物流
        /// </summary>
        [Repository(typeof(PvcCrmReponsitory))]
        [PKey("WL", PKeySigner.Mode.Normal, 5)]
        WL = 10000,
        /// <summary>
        /// 芯达通
        /// </summary>
        [Repository(typeof(PvcCrmReponsitory))]
        [PKey("XL", PKeySigner.Mode.Normal, 5)]
        XL = 20000,
        /// <summary>
        /// 芯达通
        /// </summary>
        [Repository(typeof(PvcCrmReponsitory))]
        [PKey("ICG", PKeySigner.Mode.Normal, 5)]
        ICGO = 30000,

        /// <summary>
        /// 代仓储协议
        /// </summary>
        [Repository(typeof(PvcCrmReponsitory))]
        [PKey("WsContact", PKeySigner.Mode.Date, 5)]
        WsContact = 30000,

        /// <summary>
        /// 代仓储客户
        /// </summary>
        [Repository(typeof(PvcCrmReponsitory))]
        [PKey("WsClient", PKeySigner.Mode.Date, 5)]
        WsClient = 30000,
    }
}
