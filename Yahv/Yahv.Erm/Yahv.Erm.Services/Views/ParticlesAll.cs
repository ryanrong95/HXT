using Layers.Data.Sqls;
using Yahv.Linq;
using System.Linq;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Erm.Services.Origins.Views
{
    /// <summary>
    /// 颗粒化 视图
    /// </summary>
    public class ParticlesAll : UniqueView<Models.Origins.Particle, PvbErmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        public ParticlesAll()
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

        /// <summary>
        /// 根据url 的 path 获取颗粒化信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Models.Origins.Particle SingleByUrl(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            string code = path.ToLower().MD5();
            return this.SingleOrDefault(item => item.UrlCode == code);
        }

        public void Delete(string id)
        {
            //this.Reponsitory.Delete

        }

        public void Abandon(string id)
        {

        }

        public void Remove(string id)
        {

        }


    }
}