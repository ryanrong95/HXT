using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Rolls
{
    /// <summary>
    /// 组织视图
    /// </summary>
    public class LeaguesRoll : UniqueView<League, PvbErmReponsitory>
    {
        /// <summary>
        /// 受保护的 原始视图 构造器
        /// </summary>
        public LeaguesRoll() { }

        /// <summary>
        /// 组织视图
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<League> GetIQueryable()
        {
            return new LeaguesOrigin().Where(t => t.Status != Status.Delete);
        }
    }
}