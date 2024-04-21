using Layers.Data.Sqls;
using Yahv.Linq;
using System.Linq;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Erm.Services.Views
{
    /// <summary>
    /// 颗粒化 视图
    /// </summary>
    public class ParticleSettingsRoll : QueryView<Models.Origins.ParticleSetting, PvbErmReponsitory>
    {

        Models.Origins.Role role;

        /// <summary>
        /// 默认构造器
        /// </summary>
        public ParticleSettingsRoll(Models.Origins.Role role)
        {
            this.role = role;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        internal ParticleSettingsRoll(PvbErmReponsitory reponsitory) : base(reponsitory)
        {
        }
        /// <summary>
        /// 颗粒化 可查询集
        /// </summary>
        /// <returns>可查询集</returns>
        sealed protected override IQueryable<Models.Origins.ParticleSetting> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.ParticleSettings>()
                   where entity.RoleID == this.role.ID
                   select new Models.Origins.ParticleSetting
                   {
                       RoleID = entity.RoleID,
                       UrlCode = entity.UrlCode,
                       Url = entity.Url,
                       Context = entity.Context,
                       Type = entity.Type,
                   };
        }

        /// <summary>
        /// 获取指定的UrlCode的设置数据
        /// </summary>
        /// <param name="path">地址</param>
        /// <returns>设置数据</returns>
        public Models.Origins.ParticleSetting this[string path]
        {
            get
            {
                string code = path?.ToLower().MD5();
                return this.SingleOrDefault(item => item.UrlCode == code);
            }
        }
    }
}