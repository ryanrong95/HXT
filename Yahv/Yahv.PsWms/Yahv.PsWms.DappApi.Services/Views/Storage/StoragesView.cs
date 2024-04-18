using Layers.Data;
using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.PsWms.DappApi.Services.Enums;
using Yahv.PsWms.DappApi.Services.Models;

namespace Yahv.PsWms.DappApi.Services.Views
{
    /// <summary>
    /// 库存视图
    /// </summary>
    public class StoragesView : Linq.UniqueView<Storage, PsWmsRepository>
    {
        public StoragesView()
        {
        }

        internal StoragesView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        internal StoragesView(PsWmsRepository reponsitory, IQueryable<Storage> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Storage> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Storages>()
                       //where entity.PackageNumber > 0
                       //orderby entity.ModifyDate descending
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

            //库位信息
            var shelveIDs = ienum_iquery.Select(item => item.ShelveID).Distinct();
            var shelvesView = from shelve in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Shelves>()
                              where shelveIDs.Contains(shelve.ID)
                              select new
                              {
                                  shelve.ID,
                                  shelve.Code
                              };
            var ienum_shelves = shelvesView.ToArray();

            var linq = from storage in ienum_iquery
                       join client in ienum_clients on storage.ClientID equals client.ID
                       join product in ienum_products on storage.ProductID equals product.ID
                       join shelve in ienum_shelves on storage.ShelveID equals shelve.ID into shelves
                       from shelve in shelves.DefaultIfEmpty()
                       select new
                       {
                           storage.ID,
                           ShelveCode = shelve?.Code,
                           OrderID = storage.FormID,
                           ClientName = client.Name,
                           product.Partnumber,
                           product.Brand,
                           product.Package,
                           product.DateCode,
                           storage.PackageNumber,
                           storage.Mpq,
                           storage.Total,
                           storage.Exception,
                           storage.Summary,
                           storage.Unique,
                           storage.CreateDate,
                           storage.ModifyDate,
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

        /// <summary>
        /// 在库库存拆分
        /// </summary>
        /// <param name="storageID"></param>
        /// <param name="quantity"></param>
        /// <param name="shelveID"></param>
        public void Inventory(string storageID, int quantity, string summary)
        {
            if (quantity < 1)
            {
                throw new Exception("拆库存的数量不能小于1");
            }

            var oldStorage = this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Storages>().SingleOrDefault(item => item.ID == storageID);

            string newStorageID = PKeySigner.Pick(PKeyType.Storage);
            var newStorage = new Layers.Data.Sqls.PsWms.Storages
            {
                ID = newStorageID,
                ClientID = oldStorage.ClientID,
                NoticeID = oldStorage.NoticeID,
                NoticeItemID = oldStorage.NoticeItemID,
                ProductID = oldStorage.ProductID,
                InputID = oldStorage.InputID,
                Type = oldStorage.Type,
                Islock = oldStorage.Islock,
                StocktakingType = oldStorage.StocktakingType,
                Mpq = oldStorage.Mpq,
                PackageNumber = quantity,
                Total = oldStorage.Mpq * quantity,
                SorterID = oldStorage.SorterID,
                FormID = oldStorage.FormID,
                FormItemID = oldStorage.FormItemID,
                Currency = oldStorage.Currency,
                UnitPrice = oldStorage.UnitPrice,
                ShelveID = null,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Exception = null,
                Summary = summary,
                Unique = $"ST:{newStorageID}"
            };

            if (oldStorage.Mpq > 1)
            {
                if ((oldStorage.PackageNumber > 1 && oldStorage.PackageNumber <= quantity) ||(oldStorage.PackageNumber == 1 && oldStorage.Total <= quantity))
                {
                    throw new Exception("拆库存的数量不能大于等于当前库存数量");
                }

                if (oldStorage.PackageNumber > 1 && oldStorage.PackageNumber > quantity)
                {
                    this.Reponsitory.Update<Layers.Data.Sqls.PsWms.Storages>(new
                    {
                        PackageNumber = oldStorage.PackageNumber - quantity,
                        Total = oldStorage.Total - oldStorage.Mpq * quantity,
                        ModifyDate = DateTime.Now,
                    }, item => item.ID == storageID);
                }

                if (oldStorage.PackageNumber == 1 && oldStorage.Total > quantity)
                {
                    this.Reponsitory.Update<Layers.Data.Sqls.PsWms.Storages>(new
                    {
                        Mpq = 1,
                        PackageNumber = oldStorage.Total - quantity,
                        Total = oldStorage.Total - quantity,
                        ModifyDate = DateTime.Now,
                    }, item => item.ID == storageID);

                    newStorage.Mpq = 1;
                }                                               
            }

            if (oldStorage.Mpq == 1)
            {
                if (oldStorage.PackageNumber <= quantity)
                {
                    throw new Exception("拆库存的数量不能大于等于当前库存数量");
                }

                if (oldStorage.PackageNumber > quantity)
                {
                    this.Reponsitory.Update<Layers.Data.Sqls.PsWms.Storages>(new
                    {                        
                        PackageNumber = oldStorage.PackageNumber - quantity,
                        Total = oldStorage.PackageNumber - quantity,
                        ModifyDate = DateTime.Now,
                    }, item => item.ID == storageID);
                }
            }

            // 保存新拆出来的库存
            this.Reponsitory.Insert<Layers.Data.Sqls.PsWms.Storages>(newStorage);
        }

        #region 搜索方法
        /// <summary>
        /// 根据订单ID搜索
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public StoragesView SearchByOrderID(string orderID)
        {
            var storagesView = this.IQueryable.Cast<Storage>();
            var linq = from storage in storagesView
                       where storage.FormID == orderID
                       select storage;

            var view = new StoragesView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据客户名称搜索
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public StoragesView SearchByClientName(string clientName)
        {
            var storagesView = this.IQueryable.Cast<Storage>();
            var linq = from storage in storagesView
                       join client in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.ClientsTopView>() on storage.ClientID equals client.ID
                       where client.Name.Contains(clientName)
                       select storage;

            var view = new StoragesView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据库位搜索
        /// </summary>
        /// <param name="shelveCode"></param>
        /// <returns></returns>
        public StoragesView SearchByShelveCode(string shelveCode)
        {
            var storagesView = this.IQueryable.Cast<Storage>();
            var linq = from storage in storagesView
                       join shelve in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Shelves>() on storage.ShelveID equals shelve.ID
                       where shelve.Code == shelveCode
                       select storage;

            var view = new StoragesView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据型号搜索
        /// </summary>
        /// <param name="partnumber"></param>
        /// <returns></returns>
        public StoragesView SearchByPartnumber(string partnumber)
        {
            var storagesView = this.IQueryable.Cast<Storage>();
            var linq = from storage in storagesView
                       join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Products>() on storage.ProductID equals product.ID
                       where product.Partnumber.Trim().Contains(partnumber.Trim())
                       select storage;

            var view = new StoragesView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public StoragesView SearchByCondition(Expression<Func<Storage, bool>> predicate)
        {
            var linq = this.IQueryable.Where(predicate);
            return new StoragesView(this.Reponsitory, linq);
        }
        #endregion

        #region 排序方法
        /// <summary>
        /// 按时间降序排序
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public StoragesView OrderByDateTimeDesc(Expression<Func<Storage, DateTime>> predicate)
        {
            var linq = this.IQueryable.OrderByDescending(predicate);
            return new StoragesView(this.Reponsitory, linq);
        }
        #endregion
    }
}
