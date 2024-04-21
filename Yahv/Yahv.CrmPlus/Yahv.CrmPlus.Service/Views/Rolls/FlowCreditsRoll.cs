using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins.Rolls;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Models.Origins;

namespace YaHv.CrmPlus.Services.Views.Rolls
{
    /// <summary>
    /// 信用流水
    /// </summary>
    public class FlowCreditsRoll : FlowCreditsOrigin
    {
        public FlowCreditsRoll()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public FlowCreditsRoll(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<FlowCredit> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }
}
