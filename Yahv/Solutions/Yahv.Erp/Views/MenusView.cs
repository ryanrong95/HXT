using System.Linq;
using Layers.Data.Sqls;
using Yahv.Linq;

namespace Yahv.Views
{
    public class MenusView : QueryView<string, PvbErmReponsitory>
    {
        public MenusView()
        {
        }

        public MenusView(PvbErmReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<string> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Menus>()
                   where entity.Status == 200 && entity.RightUrl != null
                   select entity.RightUrl;
        }
    }
}