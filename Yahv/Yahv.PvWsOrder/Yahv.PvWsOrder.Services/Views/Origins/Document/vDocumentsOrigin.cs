using System.Linq;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Models;

namespace Yahv.PvWsOrder.Services.Views.Origins
{
    /// <summary>
    /// 文档原始视图
    /// </summary>
    public class vDocumentsOrigin : UniqueView<vDocument, PvWsOrderReponsitory>
    {
        public vDocumentsOrigin()
        {

        }

        public vDocumentsOrigin(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<vDocument> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.vDocuments>()
                   select new vDocument()
                   {
                       ID = entity.ID,
                       Title = entity.Title,
                       CreatorID = entity.CreatorID,
                       Context = entity.Context,
                       CatalogID = entity.CatalogID,
                       ModifyDate = entity.ModifyDate,
                       CreateDate = entity.CreateDate,
                   };
        }
    }
}