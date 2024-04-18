using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.DappApi.Services.Enums;
using Yahv.PsWms.DappApi.Services.Models;
using Yahv.Underly;

namespace Yahv.PsWms.DappApi.Services.Views
{
    public class NoticeItemsView : UniqueView<NoticeItem, PsWmsRepository>
    {
        #region 构造函数
        public NoticeItemsView()
        {
        }

        public NoticeItemsView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        public NoticeItemsView(PsWmsRepository reponsitory, IQueryable<NoticeItem> iQueryable) : base(reponsitory, iQueryable)
        {

        }
        #endregion

        protected override IQueryable<NoticeItem> GetIQueryable()
        {
            var view = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.NoticeItems>()
                       select new NoticeItem
                       {
                           ID = entity.ID,
                           NoticeID = entity.NoticeID,
                           Source = (NoticeSource)entity.Source,
                           ProductID = entity.ProductID,
                           InputID = entity.InputID,
                           CustomCode = entity.CustomCode,
                           StocktakingType = (StocktakingType)entity.StocktakingType,
                           Mpq = entity.Mpq,
                           PackageNumber = entity.PackageNumber,
                           Total = entity.Total,
                           Currency = (Currency)entity.Currency,
                           UnitPrice = entity.UnitPrice,
                           Supplier = entity.Supplier,
                           ClientID = entity.ClientID,
                           FormID = entity.FormID,
                           FormItemID = entity.FormItemID,
                           CreateDate = entity.CreateDate,
                           ShelveID = entity.ShelveID,
                           SorterID = entity.SorterID,
                           PickerID = entity.PickerID,
                           Summary = entity.Summary,
                           Exception = entity.Exception,
                       };

            return view;
        }

        /// <summary>
        /// 根据NoticeID 进行搜索
        /// </summary>
        /// <param name="noticeID"></param>
        /// <returns></returns>
        public NoticeItemsView SearchByNoticeID(string noticeID)
        {
            var noticeItemsView = this.IQueryable;
            var linq = from noticeItem in noticeItemsView
                       where noticeItem.NoticeID == noticeID
                       select noticeItem;

            var view = new NoticeItemsView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据NoticeItemID进行搜索
        /// </summary>
        /// <param name="noticeItemID"></param>
        /// <returns></returns>
        public NoticeItemsView SearchByID(string noticeItemID)
        {
            var noticeItemsView = this.IQueryable;
            var linq = from noticeItem in noticeItemsView
                       where noticeItem.ID == noticeItemID
                       select noticeItem;

            var view = new NoticeItemsView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 获取分页数据,或者补充数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = default(int?), int? pageSize = default(int?))
        {
            var iquery = this.IQueryable; //.Cast<NoticeItem>();
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var ienum_iquery = iquery.ToArray();
            var noticeItemsID = ienum_iquery.Select(item => item.ID).Distinct().ToArray();


            var reportItems = this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.ReportItems>().
                Where(item => noticeItemsID.Contains(item.NoticeItemID)).ToArray();

            var ienums_filesID = (from file in new PcFilesView(this.Reponsitory)
                                  where noticeItemsID.Contains(file.MainID)
                                  select file.MainID).ToArray();


            var lins = new StoragesView(this.Reponsitory).Where(item => noticeItemsID.Contains(item.NoticeItemID)).OrderBy(item => item.ID);

            var storages = new StoragesView(this.Reponsitory).Where(item => noticeItemsID.Contains(item.NoticeItemID)).OrderBy(item => item.ID).ToArray();

            var productsID = reportItems.Select(item => item.ProductID)
                .Concat(ienum_iquery.Select(item => item.ProductID))
                .Concat(storages.Select(item => item.ProductID)).Distinct();
            var products = new ProductsView(this.Reponsitory).Where(item => productsID.Contains(item.ID))
                .Select(product => new
                {
                    product.ID,
                    product.Partnumber,
                    product.Brand,
                    product.Package,  //封装
                    product.DateCode
                })
                .ToArray();


            //应该考虑盘库后会去除真实的 storage.quantiy ==0 的情况


            var linq = from noticeItem in ienum_iquery
                       join storage in storages on noticeItem.ID equals storage.NoticeItemID into cstorages
                       join reportItem in reportItems on noticeItem.ID equals reportItem.NoticeItemID into creportItems
                       join file in ienums_filesID on noticeItem.ID equals file into files
                       orderby noticeItem.ID
                       let cstorage = cstorages.FirstOrDefault()
                       let creportItem = creportItems.FirstOrDefault()
                       let sortedReportQuantity = creportItems.Sum(c => c.StoragePackageNumber)
                       let sortedReportTotal = creportItems.Sum(c => c.StorageTotal)
                       let reportInputs = creportItems.Select(item => item.InputID)
                       let sortedStorageQuantity = cstorages.Where(item => !reportInputs.Contains(item.InputID)).Sum(s => s.PackageNumber)
                       let sortedStorageTotal = cstorages.Where(item => !reportInputs.Contains(item.InputID)).Sum(s => s.Total)
                       select new
                       {
                           noticeItem.ID,
                           noticeItem.Exception,
                           noticeItem.Summary,
                           noticeItem.Mpq, //Mpq
                           noticeItem.PackageNumber, //件数或者总量, 应到
                           noticeItem.Total, //Total,
                           sortedQuantity = sortedReportQuantity + sortedStorageQuantity,
                           sortedTotal = sortedReportTotal + sortedStorageTotal,

                           //考虑 显示 notice or storage or reportitem 三种情况

                           ProductID = creportItem?.ProductID ?? cstorage?.ProductID ?? noticeItem.ProductID,
                           FormItemID = creportItem?.FormItemID ?? cstorage?.FormItemID ?? noticeItem.FormItemID,

                           FileExist = files.Count() > 0,
                           FileCount = files.Count(),
                           //IsMultStorage = cstorages.Count() > 0 || creportItems.Count() > 0,//？


                           //未来在Ui上一定考虑：不能把历史与当前分拣的数据做在一个Ui上，否者就有如下问题。
                           //期望在Ui上能够做清楚，是永远坐不清楚的

                           Printers = cstorages.Where(item => item.Total > 0).Select(item => new
                           {
                               item.Mpq,
                               item.PackageNumber,
                               item.Exception,
                               item.ID,
                               item.Unique,
                               item.Summary,
                           }) //正确，如果已经都出库了，就不用打印了。
                       };

            var results = from entity in linq
                          join product in products on entity.ProductID equals product.ID
                          select new
                          {
                              entity.ID,
                              entity.Exception,
                              entity.Summary,
                              entity.Mpq,
                              entity.PackageNumber,
                              entity.Total,
                              entity.FormItemID,
                              IsNew = entity.FormItemID == null ? true : false,
                              product.Partnumber,
                              product.Brand,
                              product.Package,  //封装
                              product.DateCode, //批次                            
                              DeliveryCount = "",
                              StoragePackageNumber = entity.sortedQuantity,
                              StorageTotal = entity.sortedTotal,
                              FileExist = entity.FileExist,
                              FileCount = entity.FileCount,
                              Printer = entity.Printers.Select(item => new
                              {
                                  product.Partnumber,
                                  product.Brand,
                                  product.Package,  //封装
                                  product.DateCode, //批次
                                  item.Mpq,
                                  item.PackageNumber,
                                  item.Exception,
                                  item.Summary,
                                  item.ID,
                                  item.Unique,
                              }),
                          };

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                return new MyPage<object>()
                {
                    Total = total,
                    Size = pageSize.Value,
                    Index = pageIndex.Value,
                    Data = results.ToArray(),
                };
            }
            else
            {
                return results.ToArray();
            }
        }

        /// <summary>
        /// 根据条件查询后获取唯一的详细数据
        /// </summary>
        /// <returns></returns>
        public object Single()
        {
            var results = this.ToMyPage(1) as object[];
            return results?.FirstOrDefault();
        }

        /// <summary>
        /// 根据通知项ID 删除对应的通知项，Storage.
        /// </summary>
        /// <param name="noticeItemID"></param>
        public void delete(string noticeItemID)
        {
            var reportItems = this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.ReportItems>().Where(item => item.NoticeItemID == noticeItemID).ToArray();

            if (reportItems != null && reportItems.Count() > 0)
            {
                throw new Exception("不能删除新增的通知项及分拣已经复核过了");
            }

            this.Reponsitory.Delete<Layers.Data.Sqls.PsWms.Storages>(item => item.NoticeItemID == noticeItemID);
            this.Reponsitory.Delete<Layers.Data.Sqls.PsWms.NoticeItems>(item => item.ID == noticeItemID);
        }
    }
}
