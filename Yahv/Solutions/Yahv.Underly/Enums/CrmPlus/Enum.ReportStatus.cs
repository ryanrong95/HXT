using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    public enum ReportStatus
    {

        /// <summary>
        /// 待审批
        /// </summary>
        [Description("待报备")]
        Waiting = 100,
        /// <summary>
        /// 正常
        /// </summary>
        [Description("报备成功")]
        Success = 200,
        /// <summary>
        /// 报备失败
        /// </summary>
        [Description("报备失败")]
        Fail = 300,
      
    }
}
