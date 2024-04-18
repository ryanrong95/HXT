using Layers.Data.Sqls;
using Yahv.Linq;
using System.Linq;
namespace Yahv.Web.Erp.Views
{
    /// <summary>
    /// 颗粒化 视图
    /// </summary>
    class ParticlesAll : UniqueView<Models.Particle, PvbErmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal ParticlesAll()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        internal ParticlesAll(PvbErmReponsitory reponsitory) : base(reponsitory)
        {
        }
        /// <summary>
        /// 颗粒化 可查询集
        /// </summary>
        /// <returns>可查询集</returns>
        sealed protected override IQueryable<Models.Particle> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Particles>()
                   select new Models.Particle
                   {
                       ID = entity.ID,
                       UrlCode = entity.UrlCode,
                       Url = entity.Url,
                       Context = entity.Context,
                       Type = entity.Type,
                   };
        }

        public void Enter(Models.Particle entity)
        {
            if (this.Any(item => item.ID == entity.ID))
            {
                this.Reponsitory.Update(new Layers.Data.Sqls.PvbErm.Particles
                {
                    ID = entity.ID,
                    UrlCode = entity.UrlCode,
                    Url = entity.Url,
                    Context = entity.Context,
                    Type = entity.Type,
                }, item => item.ID == entity.ID);
            }
            else
            {
                this.Reponsitory.Insert(new Layers.Data.Sqls.PvbErm.Particles
                {
                    ID = entity.ID,
                    UrlCode = entity.UrlCode,
                    Url = entity.Url,
                    Context = entity.Context,
                    Type = entity.Type,
                });
            }
        }
    }
}