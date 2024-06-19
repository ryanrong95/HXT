using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class RiskChanges
    {
        public RiskControlChangeType ChangeType { get; set; }
        public decimal NewValue { get; set; }
        public decimal OldValue { get; set; }
    }

    public enum RiskControlChangeType
    {
        /// <summary>
        /// 等级变更
        /// </summary>
        [Description("等级变更")]
        RankChange = 1,

        /// <summary>
        /// 货款上限变更
        /// </summary>
        [Description("货款上限变更")]
        GoodsUpperLimitChange = 2,

        /// <summary>
        /// 税款上限变更
        /// </summary>
        [Description("税款上限变更")]
        TaxUpperLimitChange =3,

        /// <summary>
        /// 代理费上限变更
        /// </summary>
        [Description("服务费上限变更")]
        AgencyFeeUpperLimitChange =4,

        /// <summary>
        /// 杂费上限变更
        /// </summary>
        [Description("杂费上限变更")]
        IncidentalUpperLimitChange =5,

        /// <summary>
        /// 杂费上限变更
        /// </summary>
        [Description("会员类型变更")]
        NatureChange = 6
    }
}
