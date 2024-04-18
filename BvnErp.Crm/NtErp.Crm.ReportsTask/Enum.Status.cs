using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.ReportsTask
{
    public enum Status
    {
        /// <summary>
        /// 暂存
        /// </summary>
        Temporary = 100,

        /// <summary>
        /// 正常
        /// </summary>
        Normal = 200,

        /// <summary>
        /// 删除
        /// </summary>
        Delete = 400
    }
}
