using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Models.Origins;

namespace Yahv.CrmPlus.Service.Models.Rolls
{
    /// <summary>
    /// 审批记录
    /// </summary>
    public class BaseApplyRecord : Yahv.Linq.IUnique
    {
        public string ID { set; get; }
        public string MainID { set; get; }
        public ApplyTaskType TaskType { set; get; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ApplyDate { set; get; }
        /// <summary>
        /// 审批日期
        /// </summary>
        public DateTime? ApproveDate { set; get; }
        /// <summary>
        /// 注册人
        /// </summary>
        public Admin ApplyAdmin { set; get; }
        /// <summary>
        /// 审批人
        /// </summary>
        public Admin ApproveAdmin { set; get; }
        /// <summary>
        /// 审批结果
        /// </summary>
        public ApplyStatus Status { set; get; }
    }

    
}
