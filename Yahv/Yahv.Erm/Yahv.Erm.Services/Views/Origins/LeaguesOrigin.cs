using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Erm.Services.Views.Origins
{
    internal class LeaguesOrigin : UniqueView<League, PvbErmReponsitory>
    {
        internal LeaguesOrigin()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal LeaguesOrigin(PvbErmReponsitory reponsitory) : base(reponsitory)
        {
        }

        /// <summary>
        ///  可查询集
        /// </summary>
        /// <returns>可查询集</returns>
        protected override IQueryable<League> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Leagues>()
                   select new League()
                   {
                       ID = entity.ID,
                       Status = (Status)entity.Status,
                       FatherID = entity.FatherID,
                       Name = entity.Name,
                       RoleID = entity.RoleID,
                       Type = (LeagueType)entity.Type,
                       Category = (Category)entity.Category,
                       EnterpriseID = entity.EnterpriseID,
                   };
        }
    }
}