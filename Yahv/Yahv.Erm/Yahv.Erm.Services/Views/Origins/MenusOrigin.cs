using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 菜单视图
    /// </summary>
    internal class MenusOrigin : UniqueView<Models.Origins.Menu, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal MenusOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        internal MenusOrigin(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Models.Origins.Menu> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Menus>()
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
                   };
        }
    }
}
