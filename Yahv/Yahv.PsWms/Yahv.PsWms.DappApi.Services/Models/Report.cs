using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.DappApi.Services.Enums;

namespace Yahv.PsWms.DappApi.Services.Models
{
    public class Report: IUnique
    {
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 报告类型
        /// </summary>
        public ReportType ReportType { get; set; }

        /// <summary>
        /// 复核时间
        /// </summary>
        public DateTime ReviewDateTime { get; set; }

        /// <summary>
        /// 复核人ID
        /// </summary>
        public string ReviewerID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }
        
        /// <summary>
        /// 状态, 保留设计
        /// </summary>
        public int? Status { get; set; }
    }
}
