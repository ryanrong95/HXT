using System.Linq;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Models;

namespace Yahv.PvWsOrder.Services.Views.Origins
{
    /// <summary>
    /// 分类原始视图
    /// </summary>
    public class vCatalogsOrigin : UniqueView<vCatalog, PvWsOrderReponsitory>
    {
        public vCatalogsOrigin()
        {

        }

        public vCatalogsOrigin(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<vCatalog> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.vCatalogs>()
                   select new vCatalog()
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       FatherID = entity.FatherID,
                   };
        }
    }
}