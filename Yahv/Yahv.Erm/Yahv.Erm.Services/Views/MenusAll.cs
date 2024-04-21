using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Linq;
using Yahv.Utils.Serializers;

namespace Yahv.Erm.Services.Views
{
    /// <summary>
    /// 菜单视图 （重新命名）
    /// </summary>
    public class MenusAll : UniqueView<Models.Origins.Menu, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MenusAll()
        {

        }

        protected override IQueryable<Models.Origins.Menu> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Menus>()
                   where entity.Status == (int)Status.Normal
                   select new Models.Origins.Menu()
                   {
                       ID = entity.ID,
                       Status = (Status)entity.Status,
                       Name = entity.Name,
                       OrderIndex = entity.OrderIndex,
                       IconUrl = entity.IconUrl,
                       FirstUrl = entity.FirstUrl,
                       RightUrl = entity.RightUrl,
                       FatherID = entity.FatherID,
                       LogoUrl = entity.LogoUrl,
                       HelpUrl = entity.HelpUrl
                   };
        }

        public override string ToString()
        {
            return this.Json();
        }

        public string Json(string roleID = null)
        {
            var arry = this.ToArray();

            string[] checkeds;
            checkeds = this.Reponsitory.ReadTable<MapsRole>().Where(item => item.RoleID == roleID)
                .Select(item => item.MenuID).ToArray();


            var linq = from business in arry
                       where business.FatherID == null
                       select new
                       {
                           id = business.ID,
                           text = business.Name,
                           children = (from first in arry
                                       where first.FatherID == business.ID
                                       select new
                                       {
                                           id = first.ID,
                                           text = first.Name,
                                           children = (from second in arry
                                                       where second.FatherID == first.ID
                                                       select new
                                                       {
                                                           id = second.ID,
                                                           text = second.Name,
                                                           url = second.RightUrl,
                                                           @checked = checkeds.Contains(second.ID),
                                                           children = (from particle in arry
                                                                       where particle.FatherID == second.ID
                                                                       select new
                                                                       {
                                                                           id = particle.ID,
                                                                           text = particle.Name,
                                                                           url = particle.RightUrl,
                                                                           attributes = new
                                                                           {
                                                                               extension = true
                                                                           }
                                                                       }).ToArray()
                                                       }).ToArray()
                                       }).ToArray()
                       };

            return linq.ToArray().Json();
        }
    }
}