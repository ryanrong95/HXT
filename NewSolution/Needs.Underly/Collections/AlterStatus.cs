using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Underly.Collections
{
    public enum AlterStatus
    {
        Unknown = 0,
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 200,
        /// <summary>
        /// 修改
        /// </summary>
        Altered = 300,
        /// <summary>
        /// 移除
        /// </summary>
        Cancel = 400,
    }
}
