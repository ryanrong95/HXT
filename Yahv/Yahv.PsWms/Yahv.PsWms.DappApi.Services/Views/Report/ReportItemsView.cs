using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.DappApi.Services.Models;
using Yahv.PsWms.DappApi.Services.Enums;
using Yahv.Underly;

namespace Yahv.PsWms.DappApi.Services.Views
{
    public class ReportItemsView : UniqueView<ReportItem, PsWmsRepository>
    {
        #region 构造函数

        public ReportItemsView()
        {
        }

        public ReportItemsView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        public ReportItemsView(PsWmsRepository reponsitory, IQueryable<ReportItem> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        #endregion

        protected override IQueryable<ReportItem> GetIQueryable()
        {
            var products = new ProductsView(this.Reponsitory);

            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.ReportItems>()
                       join product in products on entity.ProductID equals product.ID
                       select new ReportItem
                       {
                           ID = entity.ID,
                           ReportID = entity.ReportID,
                           NoticeID = entity.NoticeID,
                           NoticeItemID = entity.NoticeItemID,
                           ProductID = entity.ProductID,
                           InputID = entity.InputID,
                           NoticeMpq = entity.NoticeMpq,
                           NoticePackageNumber = entity.NoticePackageNumber,
                           NoticeTotal = entity.NoticeTotal,
                           StorageMpq = entity.StorageMpq,
                           StoragePackageNumber = entity.StoragePackageNumber,
                           StorageTotal = entity.StorageTotal,
                           AdminID = entity.AdminID,
                           FormID = entity.FormID,
                           FormItemID = entity.FormItemID,
                           InCurrency = (Currency)entity.InCurrency,
                           InUnitPrice = entity.InUnitPrice,
                           OutCurrency = (Currency)entity.OutCurrency,
                           OutUnitPrice = entity.OutUnitPrice,
                           ClientID = entity.ClientID,
                           CreateDate = entity.CreateDate,
                           Exception = entity.Exception,

                           Product = product,
                       };
            return view;
        }
    }
}
