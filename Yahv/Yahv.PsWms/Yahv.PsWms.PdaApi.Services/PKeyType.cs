using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PsWms.PdaApi.Services
{
    /// <summary>
    /// 主键类型
    /// </summary>
    public enum PKeyType
    {
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
        /// 文件
        /// </summary>
        [Repository(typeof(PsWmsRepository))]
        [PKey("File", PKeySigner.Mode.Date, 4)]
        File = 10011,
    }
}
