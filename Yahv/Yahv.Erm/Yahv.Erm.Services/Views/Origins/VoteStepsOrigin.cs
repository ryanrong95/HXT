using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 审批步骤视图
    /// </summary>
    internal class VoteStepsOrigin : UniqueView<VoteStep, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal VoteStepsOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        internal VoteStepsOrigin(PvbErmReponsitory repository) : base(repository) { }

        protected override IQueryable<VoteStep> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.VoteSteps>()
                   select new VoteStep()
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       VoteFlowID = entity.VoteFlowID,
                       CreateDate = entity.CreateDate,
                       OrderIndex = entity.OrderIndex,
                       AdminID = entity.AdminID,
                       PositionID = entity.PositionID,
                       Uri = entity.Uri
                   };
        }
    }
}
