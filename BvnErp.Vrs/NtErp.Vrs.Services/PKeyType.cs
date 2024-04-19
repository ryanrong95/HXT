using Layer.Data.Sqls;
using Layer.Linq;
using Needs.Overall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Vrs.Services
{
    /// <summary>
    /// 主键类型
    /// </summary>
    public enum PKeyType
    {
        /// <summary>
        /// 发票
        /// </summary>
        [Repository(typeof(BvnVrsReponsitory))]
        [PKey("IVE", PKeySigner.Mode.Normal, 10)]
        Invoice = 10001,
        
    }
}
