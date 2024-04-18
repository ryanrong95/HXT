using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models.Statistics
{
    /// <summary>
    /// 新增客户
    /// </summary>
    public class NewClient : IUnique
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime ApplyDate { get; set; }
    }
}
