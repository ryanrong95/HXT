using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.PvData.Services
{
    /// <summary>
    /// 用于生成主键
    /// </summary>
    public enum PKeyType
    {
        /// <summary>
        /// 归类变更日志
        /// </summary>
        [Repository(typeof(PvDataReponsitory))]
        [PKey("CML", PKeySigner.Mode.Date, 6)]
        ClassifyModifiedLog = 10001,

        /// <summary>
        /// 归类操作日志
        /// </summary>
        [Repository(typeof(PvDataReponsitory))]
        [PKey("COL", PKeySigner.Mode.Date, 6)]
        ClassifyOperatingLog = 10002,

        /// <summary>
        /// 归类历史记录
        /// </summary>
        [Repository(typeof(PvDataReponsitory))]
        [PKey("CPNL", PKeySigner.Mode.Date, 6)]
        ClassifiedPartNumberLog = 10003,

        /// <summary>
        /// 归类历史数据变更日志
        /// </summary>
        [Repository(typeof(PvDataReponsitory))]
        [PKey("CMP", PKeySigner.Mode.Date, 6)]
        ClassifiedModifiedPast = 10004,

        /// <summary>
        /// 产品报价
        /// </summary>
        [Repository(typeof(PvDataReponsitory))]
        [PKey("PQ", PKeySigner.Mode.Date, 6)]
        ProductQuote = 20001,

        /// <summary>
        /// 中国银行外汇牌价
        /// </summary>
        [Repository(typeof(PvDataReponsitory))]
        [PKey("Feroboc", PKeySigner.Mode.Date, 8)]
        Feroboc = 30001,

        /// <summary>
        /// 抓取的中国银行外汇牌价页面记录
        /// </summary>
        [Repository(typeof(PvDataReponsitory))]
        [PKey("FerobocLog", PKeySigner.Mode.Date, 8)]
        FerobocLog = 30002,

        /// <summary>
        /// 海关卡控
        /// </summary>
        [Repository(typeof(PvDataReponsitory))]
        [PKey("CC", PKeySigner.Mode.Date, 6)]
        CustomsControl = 40001,

        /// <summary>
        /// 申报要素品牌操作日志
        /// </summary>
        [Repository(typeof(PvDataReponsitory))]
        [PKey("EML", PKeySigner.Mode.Date, 6)]
        ElementsManufacturerLog = 50001,
    }
}
