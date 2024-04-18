using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.DappApi.Services.Enums
{
    /// <summary>
    /// 主键类型
    /// </summary>
    enum PKeyType
    {
        /// <summary>
        /// 通知
        /// </summary>
        [Repository(typeof(PsWmsRepository))]
        [PKey("N", PKeySigner.Mode.Date, 4)]
        Notice = 10000, 
        
        /// <summary>
        /// 通知项
        /// </summary>
        [Repository(typeof(PsWmsRepository))]
        [PKey("NItem", PKeySigner.Mode.Date, 4)]
        NoticeItem = 10001,

        /// <summary>
        /// 通知运输信息
        /// </summary>
        [Repository(typeof(PsWmsRepository))]
        [PKey("NTransport", PKeySigner.Mode.Date, 4)]
        NoticeTransport = 10002,

        /// <summary>
        /// 报告
        /// </summary>
        [Repository(typeof(PsWmsRepository))]
        [PKey("Rpt", PKeySigner.Mode.Date, 4)]
        Report = 10003,

        /// <summary>
        /// 报告项
        /// </summary>
        [Repository(typeof(PsWmsRepository))]
        [PKey("RptItem", PKeySigner.Mode.Date, 4)]
        ReportItem = 10004,

        /// <summary>
        /// 库存
        /// </summary>
        [Repository(typeof(PsWmsRepository))]
        [PKey("ST", PKeySigner.Mode.Date, 4)]
        Storage = 10005,

        /// <summary>
        /// 库位
        /// </summary>
        [Repository(typeof(PsWmsRepository))]
        [PKey("Shelve", PKeySigner.Mode.Date, 4)]
        Shelves = 10006,

        /// <summary>
        /// 应收
        /// </summary>
        [Repository(typeof(PsWmsRepository))]
        [PKey("PayeeLeft", PKeySigner.Mode.Date, 4)]
        PayeeLeft = 10007,

        /// <summary>
        /// 应付
        /// </summary>
        [Repository(typeof(PsWmsRepository))]
        [PKey("PayerLeft", PKeySigner.Mode.Date, 4)]
        PayerLeft = 10008,

        /// <summary>
        /// 产品
        /// </summary>
        [Repository(typeof(PsWmsRepository))]
        [PKey("P", PKeySigner.Mode.Date, 4)]
        Product = 10009,

        /// <summary>
        /// 标准产品
        /// </summary>
        [Repository(typeof(PsWmsRepository))]
        [PKey("SP", PKeySigner.Mode.Date, 4)]
        StandardProduct = 10010,

        /// <summary>
        /// 文件
        /// </summary>
        [Repository(typeof(PsWmsRepository))]
        [PKey("File", PKeySigner.Mode.Date, 4)]
        File = 10011,

        /// <summary>
        /// 特殊要求
        /// </summary>
        [Repository(typeof(PsWmsRepository))]
        [PKey("Require", PKeySigner.Mode.Date, 4)]
        Require = 10012,

        /// <summary>
        /// 进项
        /// </summary>
        [Repository(typeof(PsWmsRepository))]
        [PKey("Input", PKeySigner.Mode.Date, 4)]
        Input = 10013,

        /// <summary>
        /// 拿货人
        /// </summary>
        [Repository(typeof(PsWmsRepository))]
        [PKey("Taker", PKeySigner.Mode.Date, 4)]
        Taker = 10014,
    }
}
