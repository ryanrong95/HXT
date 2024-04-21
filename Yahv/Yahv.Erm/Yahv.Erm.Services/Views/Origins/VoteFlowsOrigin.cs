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
    /// 审批流视图
    /// </summary>
    internal class VoteFlowsOrigin : UniqueView<VoteFlow, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal VoteFlowsOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        internal VoteFlowsOrigin(PvbErmReponsitory repository) : base(repository) { }

        protected override IQueryable<VoteFlow> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.VoteFlows>()
                   select new VoteFlow()
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Type = (ApplicationType)entity.Type,
                       CreatorID = entity.CreatorID,
                       ModifyID = entity.ModifyID,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate
                   };
        }
    }
}
