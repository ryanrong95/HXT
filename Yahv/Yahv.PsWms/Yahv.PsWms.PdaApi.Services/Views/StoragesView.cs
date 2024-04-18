using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PsWms.PdaApi.Services.Models;

namespace Yahv.PsWms.PdaApi.Services.Views
{
    /// <summary>
    /// 库存视图
    /// </summary>
    public class StoragesView : Linq.UniqueView<Storage, PsWmsRepository>
    {
        public StoragesView()
        {
        }

        protected StoragesView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        protected StoragesView(PsWmsRepository reponsitory, IQueryable<Storage> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Storage> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Storages>()
                       select new Storage
                       {
                           ID = entity.ID,
                           ClientID = entity.ClientID,
                           NoticeID = entity.NoticeID,
                           NoticeItemID = entity.NoticeItemID,
                           ProductID = entity.ProductID,
                           InputID = entity.InputID,
                           Type = (Enums.StorageType)entity.Type,
                           Islock = entity.Islock,
                           StocktakingType = (Enums.StocktakingType)entity.StocktakingType,
                           Mpq = entity.Mpq,
                           PackageNumber = entity.PackageNumber,
                           Total = entity.Total,
                           SorterID = entity.SorterID,
                           FormID = entity.FormID,
                           FormItemID = entity.FormItemID,
                           Currency = (Underly.Currency)entity.Currency,
                           UnitPrice = entity.UnitPrice,
                           ShelveID = entity.ShelveID,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Exception = entity.Exception,
                           Summary = entity.Summary,
                           Unique = entity.Unique
                       };

            return view;
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <returns></returns>
        public object Single()
        {
            return (this.ToMyPage() as object[]).SingleOrDefault();
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            var iquery = this.IQueryable.Cast<Storage>();
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            #region 补充完整对象

            var ienum_iquery = iquery.ToArray();

            //客户信息
            var clientIDs = ienum_iquery.Select(item => item.ClientID).Distinct();
            var clientsView = from client in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.ClientsTopView>()
                              where clientIDs.Contains(client.ID)
                              select new
                              {
                                  client.ID,
                                  client.Name
                              };
            var ienum_clients = clientsView.ToArray();

            //产品信息
            var productIDs = ienum_iquery.Select(item => item.ProductID).Distinct();
            var productsView = from product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Products>()
                               where productIDs.Contains(product.ID)
                               select new
                               {
                                   product.ID,
                                   product.Partnumber,
                                   product.Brand,
                                   product.Package,
                                   product.DateCode
                               };
            var ienum_products = productsView.ToArray();

            var linq = from storage in ienum_iquery
                       join client in ienum_clients on storage.ClientID equals client.ID
                       join product in ienum_products on storage.ProductID equals product.ID
                       select new
                       {
                           storage.ID,
                           ClientName = client.Name,
                           product.Partnumber,
                           product.Brand,
                           product.Package,
                           product.DateCode,
                           storage.PackageNumber,
                           storage.Mpq,
                           storage.Total,
                           storage.Summary
                       };

            #endregion

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                return new
                {
                    Total = total,
                    Size = pageSize,
                    Index = pageIndex,
                    Data = linq.ToArray(),
                };
            }
            else
            {
                return linq.ToArray();
            }
        }

        #region 搜索方法
        /// <summary>
        /// 根据库存ID搜索
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public StoragesView SearchByID(string id)
        {
            var storagesView = this.IQueryable.Cast<Storage>();
            var linq = from storage in storagesView
                       where storage.ID == id
                       select storage;

            var view = new StoragesView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据库存唯一标识搜索
        /// </summary>
        /// <param name="unique"></param>
        /// <returns></returns>
        public StoragesView SearchByUnique(string unique)
        {
            var storagesView = this.IQueryable.Cast<Storage>();
            var linq = from storage in storagesView
                       where storage.Unique == unique
                       select storage;

            var view = new StoragesView(this.Reponsitory, linq);
            return view;
        }
        #endregion

        /// <summary>
        /// 扫码上架
        /// </summary>
        ///<param name="jobject"></param>
        public void Shelving(JObject jobject)
        {
            var args = new
            {
                InCode = jobject["InCode"]?.Value<string>(),
                ShelveCode = jobject["ShelveCode"]?.Value<string>(),
            };

            if (string.IsNullOrEmpty(args.InCode))
                throw new ArgumentNullException("入库标签不能为空");
            if (string.IsNullOrEmpty(args.ShelveCode))
                throw new ArgumentNullException("库位标签不能为空");

            var storage = this.SingleOrDefault(item => item.ID == args.InCode);
            if(storage == null)
                throw new Exception($"未查询到库存记录");
            if (!string.IsNullOrEmpty(storage.ShelveID))
                throw new Exception($"该产品已经上架，库位【{storage.ShelveID}】");

            var shelve = Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Shelves>().SingleOrDefault(item => item.Code == args.ShelveCode);
            if (shelve == null)
                throw new Exception($"未查询到库位号【{args.ShelveCode}】对应的库位信息");

            this.Reponsitory.Update<Layers.Data.Sqls.PsWms.Storages>(new
            {
                ShelveID = shelve.ID,
                ModifyDate = DateTime.Now,
            }, item => item.ID == args.InCode);
        }

        /// <summary>
        /// 库位变更
        /// </summary>
        ///<param name="jobject"></param>
        public void ChangeShelve(JObject jobject)
        {
            var args = new
            {
                InCode = jobject["InCode"]?.Value<string>(),
                ShelveCode = jobject["ShelveCode"]?.Value<string>(),
            };

            if (string.IsNullOrEmpty(args.InCode))
                throw new ArgumentNullException("入库标签不能为空");
            if (string.IsNullOrEmpty(args.ShelveCode))
                throw new ArgumentNullException("库位标签不能为空");

            var storage = this.SingleOrDefault(item => item.ID == args.InCode);
            if (storage == null)
                throw new Exception($"未查询到库存记录");

            var shelve = Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Shelves>().SingleOrDefault(item => item.Code == args.ShelveCode);
            if (shelve == null)
                throw new Exception($"未查询到库位号【{args.ShelveCode}】对应的库位信息");

            if (storage.ShelveID == shelve.ID)
                throw new Exception($"该产品当前已在库位【{args.ShelveCode}】");

            this.Reponsitory.Update<Layers.Data.Sqls.PsWms.Storages>(new
            {
                ShelveID = shelve.ID,
                ModifyDate = DateTime.Now,
            }, item => item.ID == args.InCode);
        }
    }
}
