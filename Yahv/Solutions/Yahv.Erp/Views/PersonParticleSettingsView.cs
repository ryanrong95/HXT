using System.Linq;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Erps;

namespace Yahv.Views
{
    /// <summary>
    /// 个人颗粒化设置
    /// </summary>
    public class PersonParticleSettingsView : QueryView<string, PvbErmReponsitory>
    {
        private IErpAdmin _erpAdmin;

        public PersonParticleSettingsView(IErpAdmin erpAdmin)
        {
            _erpAdmin = erpAdmin;
        }

        protected override IQueryable<string> GetIQueryable()
        {
            if (_erpAdmin.Role.Type != RoleType.Compose)
            {
                return from settings in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.ParticleSettings>()
                       where settings.RoleID == _erpAdmin.Role.ID && settings.Url != null
                       select settings.Url;
            }
            else
            {
                var childRoleIds = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsRoleCompose>()
                    .Where(item => item.RoleID == _erpAdmin.Role.ID).Select(item => item.ChildID).ToArray();

                return from settings in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.ParticleSettings>()
                       where childRoleIds.Contains(settings.RoleID) && settings.Url != null
                       select settings.Url;
            }
        }
    }
}