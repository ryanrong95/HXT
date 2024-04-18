using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public class ReportExtends
    {
        /// <summary>
        /// 跟进方式
        /// </summary>
        public ActionMethord Type
        {
            get; set;
        }
        /// <summary>
        /// 下次跟进方式
        /// </summary>
        public ActionMethord NextType
        {
            get; set;
        }
        /// <summary>
        /// 跟进日期
        /// </summary>
        public DateTime Date
        {
            get; set;
        }

        /// <summary>
        /// 下次更新日期
        /// </summary>
        public DateTime NextDate
        {
            get; set;
        }

        /// <summary>
        /// 跟进内容
        /// </summary>
        public string Content
        {
            get; set;
        }

        /// <summary>
        /// 行动计划
        /// </summary>
        public string Plan
        {
            get; set;
        }

        /// <summary>
        /// 原厂陪同人员
        /// </summary>
        public string OriginalStaffs
        {
            get;set;
        }
    }
}
