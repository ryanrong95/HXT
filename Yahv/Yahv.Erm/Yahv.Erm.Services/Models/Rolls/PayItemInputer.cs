using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Erm.Services.Models.Rolls
{
    /// <summary>
    /// 工资项负责人
    /// </summary>
    public class PayItemInputer
    {
        /// <summary>
        /// 工资项名称
        /// </summary>
        public string PayItemName { get; set; }

        /// <summary>
        /// 录入人
        /// </summary>
        public string InputerName { get; set; }

        /// <summary>
        /// 是否导入
        /// </summary>
        public bool IsImport { get; set; }
    }
}
