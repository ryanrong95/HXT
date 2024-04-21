using Layers.Data.Sqls;
using Yahv.Linq;
using System.Linq;
namespace Yahv.Erm.Services.Origins.Views
{
    /// <summary>
    /// 颗粒化 视图
    /// </summary>
    public class ParticlesOrigin : UniqueView<Models.Origins.Particle, PvbErmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal ParticlesOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        internal ParticlesOrigin(PvbErmReponsitory reponsitory) : base(reponsitory)
        {
        }
        /// <summary>
        /// 颗粒化 可查询集
        /// </summary>
        /// <returns>可查询集</returns>
        sealed protected override IQueryable<Models.Origins.Particle> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Particles>()
                   select new Models.Origins.Particle
                   {
                       ID = entity.ID,
                       UrlCode = entity.UrlCode,
                       Url = entity.Url,
                       Context = entity.Context,
                       Type = entity.Type,
                   };
        }
    }
}