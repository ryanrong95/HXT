using System.Linq;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Models;
using Yahv.Underly;
using Yahv.Underly.Erps;

namespace Yahv.Views
{
    /// <summary>
    /// 我的组织机构
    /// </summary>
    public class PersonalLeaguesView : QueryView<Models.League, PvbErmReponsitory>
    {
        private IErpAdmin _erpAdmin;

        public PersonalLeaguesView(IErpAdmin erpAdmin)
        {
            _erpAdmin = erpAdmin;
        }

        protected override IQueryable<League> GetIQueryable()
        {
            return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsLeague>()
                   join league in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Leagues>() on map.LeagueID equals
                       league.ID
                   where map.AdminID == _erpAdmin.ID && league.Status == (int)GeneralStatus.Normal
                   select new League()
                   {
                       ID = map.LeagueID,
                       Name = league.Name,
                   };
        }
    }
}
