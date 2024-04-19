using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NtErp.Wss.Sales.Services.Underly.Collections
{
    /// <summary>
    /// 修改接口
    /// </summary>
    public interface IAlter
    {
        /// <summary>
        /// 变更时间
        /// </summary>
        DateTime AlterDate { get; set; }

        /// <summary>
        /// 变更状态
        /// </summary>
        AlterStatus Status { get; set; }
    }
}
