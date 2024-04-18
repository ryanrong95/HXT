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
    public class ReportItems_In_View : UniqueView<ReportItem, PsWmsRepository>
    {
        #region 构造函数

        public ReportItems_In_View()
        {
        }

        public ReportItems_In_View(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        public ReportItems_In_View(PsWmsRepository reponsitory, IQueryable<ReportItem> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        #endregion

        protected override IQueryable<ReportItem> GetIQueryable()
        {
            var products = new ProductsView(this.Reponsitory);
            var reports = new ReportsView(this.Reponsitory).Where(t => t.ReportType == ReportType.Inbound);

            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.ReportItems>()
                       join product in products on entity.ProductID equals product.ID
                       join report in reports on entity.ReportID equals report.ID
                       orderby entity.CreateDate descending
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
                           ShelveID = entity.ShelveID,

                           Product = product,
                       };
            return view;
        }

        /// <summary>
        /// 分页视图
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object ToMyPage(int pageIndex = 1, int pageSize = 20)
        {
            var iquery = this.IQueryable.Cast<ReportItem>();
            int total = iquery.Count();

            iquery = iquery.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            var ienum_iquery = iquery.ToArray();
            var products = new ProductsView(this.Reponsitory);
            var shelvesView = new ShelvesView(this.Reponsitory);

            var linq = from entity in ienum_iquery
                       join product in products on entity.ProductID equals product.ID
                       join _shelve in shelvesView on entity.ShelveID equals _shelve.ID into shelves
                       from shelve in shelves.DefaultIfEmpty()
                       select new ReportItem()
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
                           InCurrency = entity.InCurrency,
                           InUnitPrice = entity.InUnitPrice,
                           OutCurrency = entity.OutCurrency,
                           OutUnitPrice = entity.OutUnitPrice,
                           ClientID = entity.ClientID,
                           CreateDate = entity.CreateDate,
                           Exception = entity.Exception,
                           ShelveCode = shelve?.Code,
                           Product = product,
                       };
            return new
            {
                Total = total,
                PageSize = pageSize,
                PageIndex = pageIndex,
                data = linq.ToArray(),
            };
        }

        #region 查询方法

        /// <summary>
        /// 根据型号搜索
        /// </summary>
        /// <param name="Partnumber">型号</param>
        /// <returns></returns>
        public ReportItems_In_View SearchByPartnumber(string Partnumber)
        {
            var items = this.IQueryable.Cast<ReportItem>();
            var linq = from item in items
                       where item.Product.Partnumber.Contains(Partnumber)
                       select item;

            var view = new ReportItems_In_View(this.Reponsitory, linq)
            {
            };
            return view;
        }

        /// <summary>
        /// 根据品牌搜索
        /// </summary>
        /// <param name="Partnumber">品牌</param>
        /// <returns></returns>
        public ReportItems_In_View SearchByBrand(string Brand)
        {
            var items = this.IQueryable.Cast<ReportItem>();
            var linq = from item in items
                       where item.Product.Brand.Contains(Brand)
                       select item;

            var view = new ReportItems_In_View(this.Reponsitory, linq)
            {
            };
            return view;
        }

        /// <summary>
        /// 根据起止时间搜索
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public ReportItems_In_View SearchByDate(DateTime start, DateTime end)
        {
            var items = this.IQueryable.Cast<ReportItem>();
            var linq = from item in items
                       where item.CreateDate >= start && item.CreateDate < end.AddDays(1)
                       select item;

            var view = new ReportItems_In_View(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据FormID 订单ID搜索
        /// </summary>
        /// <param name="formID"></param>
        /// <returns></returns>
        public ReportItems_In_View SearchByFormID(string formID)
        {
            var items = this.IQueryable.Cast<ReportItem>();
            var linq = from item in items
                       where item.FormID == formID
                       select item;

            var view = new ReportItems_In_View(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根究是否存在异常搜索
        /// </summary>
        /// <param name="isNormal"></param>
        /// <returns></returns>
        public ReportItems_In_View SearchByNormalStatus(bool isNormal = true)
        {
            var items = this.IQueryable.Cast<ReportItem>();

            IQueryable<ReportItem> linq = null;

            if(isNormal)
            {
                linq = from item in items
                       where item.Exception == null
                       select item;

                var normalview = new ReportItems_In_View(this.Reponsitory, linq);
                return normalview;
            }

            linq = from item in items
                   where item.Exception != null
                   select item;

            var view = new ReportItems_In_View(this.Reponsitory, linq);
            return view;
        }

        #endregion
    }
}
