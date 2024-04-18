using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;

namespace Yahv.PsWms.SzMvc.Services.Views
{
    public class StorageListView : QueryView<StorageListViewModel, PsOrderRepository>
    {
        public StorageListView()
        {
        }

        protected StorageListView(PsOrderRepository reponsitory, IQueryable<StorageListViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<StorageListViewModel> GetIQueryable()
        {
            var storagesTopView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.StoragesTopView>();

            var iQuery = from storage in storagesTopView
                         select new StorageListViewModel
                         {
                             StorageID = storage.ID,
                             ClientID = storage.ClientID,
                             StocktakingType = (StocktakingType)storage.StocktakingType,
                             Mpq = storage.Mpq,
                             PackageNumber = storage.PackageNumber,
                             CreateDate = storage.CreateDate,
                             Partnumber = storage.Partnumber,
                             Brand = storage.Brand,
                             Package = storage.Package,
                             DateCode = storage.DateCode,
                             Code = storage.Code,
                             Ex_PackageNumber = storage.Ex_PackageNumber,
                             CustomCode = storage.CustomCode,
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public Tuple<T[], int> ToMyPage<T>(Func<StorageListViewModel, T> convert, int? pageIndex = null, int? pageSize = null) where T : class
        {
            IQueryable<StorageListViewModel> iquery = this.IQueryable.Cast<StorageListViewModel>()
                .OrderBy(item => item.Code).ThenBy(item => item.Partnumber);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_storages = iquery.ToArray();

            var ienums_linq = from storage in ienum_storages
                              select new StorageListViewModel
                              {
                                  StorageID = storage.StorageID,
                                  ClientID = storage.ClientID,
                                  StocktakingType = storage.StocktakingType,
                                  Mpq = storage.Mpq,
                                  PackageNumber = storage.PackageNumber,
                                  CreateDate = storage.CreateDate,
                                  Partnumber = storage.Partnumber,
                                  Brand = storage.Brand,
                                  Package = storage.Package,
                                  DateCode = storage.DateCode,
                                  Code = storage.Code,
                                  Ex_PackageNumber = storage.Ex_PackageNumber,
                                  CustomCode = storage.CustomCode,
                              };

            var results = ienums_linq;

            return new Tuple<T[], int>(results.Select(convert).ToArray(), total);
        }

        /// <summary>
        /// 根据 ClientID 查询
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public StorageListView SearchByClientID(string clientID)
        {
            var linq = from query in this.IQueryable
                       where query.ClientID == clientID
                       select query;

            var view = new StorageListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据型号查询
        /// </summary>
        /// <param name="partnumber"></param>
        /// <returns></returns>
        public StorageListView SearchByPartnumber(string partnumber)
        {
            var linq = from query in this.IQueryable
                       where query.Partnumber.Contains(partnumber)
                       select query;

            var view = new StorageListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据品牌查询
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        public StorageListView SearchByBrand(string brand)
        {
            var linq = from query in this.IQueryable
                       where query.Brand.Contains(brand)
                       select query;

            var view = new StorageListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据库位号查询
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public StorageListView SearchByCode(string code)
        {
            var linq = from query in this.IQueryable
                       where query.Code.Contains(code)
                       select query;

            var view = new StorageListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 StorageIDs 查询
        /// </summary>
        /// <param name="storageIDs"></param>
        /// <returns></returns>
        public StorageListView SearchByStorageIDs(string[] storageIDs)
        {
            var linq = from query in this.IQueryable
                       where storageIDs.Contains(query.StorageID)
                       select query;

            var view = new StorageListView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据 PackageNumber 不为 0 查询
        /// </summary>
        /// <returns></returns>
        public StorageListView SearchByPackageNumberIsNotZero()
        {
            var linq = from query in this.IQueryable
                       where (query.PackageNumber - (query.Ex_PackageNumber ?? 0)) != 0
                       select query;

            var view = new StorageListView(this.Reponsitory, linq);
            return view;
        }
    }

    public class StorageListViewModel
    {
        /// <summary>
        /// StorageID
        /// </summary>
        public string StorageID { get; set; }

        /// <summary>
        /// ClientID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 包装类型
        /// </summary>
        public StocktakingType StocktakingType { get; set; }

        /// <summary>
        /// 最小包装量
        /// </summary>
        public int Mpq { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int PackageNumber { get; set; }

        /// <summary>
        /// 创建时间(分拣时间、入库时间)
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Partnumber { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 封装
        /// </summary>
        public string Package { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
        public string DateCode { get; set; }

        /// <summary>
        /// 库位号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 已使用的数量
        /// </summary>
        public int? Ex_PackageNumber { get; set; }

        /// <summary>
        /// 自定义编号
        /// </summary>
        public string CustomCode { get; set; }
    }
}
