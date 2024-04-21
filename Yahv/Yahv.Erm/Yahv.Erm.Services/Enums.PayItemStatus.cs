using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Erm.Services
{
    public enum PayItemStatus
    {
        /// <summary>
        /// 保存
        /// </summary>
        [Description("保存")]
        Save = 100,

        /// <summary>
        /// 提交
        /// </summary>
        [Description("提交")]
        Submit = 200,
    }
}
