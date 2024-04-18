using System.Linq;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Models;
using Yahv.PvWsOrder.Services.Views.Origins;

namespace Yahv.PvWsOrder.Services.Views.Rolls.Document
{
    /// <summary>
    /// 文档视图
    /// </summary>
    public class vDocumentsRoll : UniqueView<vDocument, PvWsOrderReponsitory>
    {
        public vDocumentsRoll()
        {

        }

        public vDocumentsRoll(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }


        protected override IQueryable<vDocument> GetIQueryable()
        {
            var catalogs = new vCatalogsOrigin(this.Reponsitory);
            var documents = new vDocumentsOrigin(this.Reponsitory);
            var admins = new AdminsAll(this.Reponsitory);

            return from doc in documents
                   join catalog in catalogs on doc.CatalogID equals catalog.ID
                   join admin in admins on doc.CreatorID equals admin.ID
                   select new vDocument()
                   {
                       ID = doc.ID,
                       CatalogID = doc.CatalogID,
                       Title = doc.Title,
                       ModifyDate = doc.ModifyDate,
                       CreatorID = doc.CreatorID,
                       Context = doc.Context,
                       CreateDate = doc.CreateDate,
                       CatalogName = catalog.Name,
                       CreatorName = admin.RealName,
                   };
        }
    }
}