using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PsWms.DappApi.Services.Models;

namespace Yahv.PsWms.DappApi.Services.Views
{
    /// <summary>
    /// 库位视图
    /// </summary>
    public class ShelvesView : Linq.UniqueView<Shelve, PsWmsRepository>
    {
        public ShelvesView()
        {
        }

        internal ShelvesView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        internal ShelvesView(PsWmsRepository reponsitory, IQueryable<Shelve> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<Shelve> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Shelves>()
                       select new Shelve
                       {
                           ID = entity.ID,
                           Code = entity.Code,
                           Size = entity.Size,
                           Company = entity.Company,
                           Summary = entity.Summary,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate
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
            var iquery = this.IQueryable.Cast<Shelve>();
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var ienum_iquery = iquery.ToArray();

            //库存信息
            var shelveIDs = ienum_iquery.Select(item => item.ID);
            var storagesView = from storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Storages>()
                               where shelveIDs.Contains(storage.ShelveID) && storage.Total > 0
                               select new
                               {
                                   storage.ID,
                                   storage.ClientID,
                                   storage.ShelveID,
                                   storage.ProductID,
                                   storage.FormID,
                                   storage.Mpq,
                                   storage.PackageNumber,
                                   storage.Total,
                                   storage.Exception,
                                   storage.Summary,
                                   storage.Unique
                               };
            var ienum_storages = storagesView.ToArray();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                #region 视图补全

                var linq = from shelve in ienum_iquery
                           join storage in ienum_storages on shelve.ID equals storage.ShelveID into storages
                           let totalPN = storages.Sum(item => item.PackageNumber)
                           let totalCount = storages.Sum(item => item.Total)
                           select new
                           {
                               shelve.ID,
                               ShelveCode = shelve.Code,
                               shelve.Company,
                               CurrentStorage = $"{totalPN}/{totalCount}",
                               shelve.Size,
                               shelve.Summary,
                               IsCanDelete = storages.Count() == 0, //库位中没有库存数据时才可以删除
                           };

                #endregion

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
                #region 补充完整对象

                //客户信息
                var clientIDs = ienum_storages.Select(item => item.ClientID).Distinct();
                var clientsView = from client in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.ClientsTopView>()
                                  where clientIDs.Contains(client.ID)
                                  select new
                                  {
                                      client.ID,
                                      client.Name
                                  };
                var ienum_clients = clientsView.ToArray();

                //产品信息
                var productIDs = ienum_storages.Select(item => item.ProductID);
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

                //汇总需要展示的库存信息
                var itemsView = from storage in ienum_storages
                                join product in ienum_products on storage.ProductID equals product.ID
                                join client in ienum_clients on storage.ClientID equals client.ID
                                join shelve in ienum_iquery on storage.ShelveID equals shelve.ID
                                select new
                                {
                                    storage.ID,
                                    storage.ShelveID,
                                    ShelveCode = shelve.Code,
                                    OrderID = storage.FormID,
                                    ClientName = client.Name,
                                    product.Partnumber,
                                    product.Brand,
                                    product.Package,
                                    product.DateCode,
                                    storage.Mpq,
                                    storage.PackageNumber,
                                    storage.Total,
                                    storage.Exception,
                                    storage.Summary,
                                    storage.Unique
                                };
                var ienum_items = itemsView.ToArray();

                var linq = from shelve in ienum_iquery
                           join item in ienum_items on shelve.ID equals item.ShelveID into items
                           select new
                           {
                               shelve.ID,
                               ShelveCode = shelve.Code,
                               shelve.Size,
                               shelve.Company,
                               Storages = items
                           };

                #endregion

                return linq.ToArray();
            }
        }

        #region 搜索方法
        /// <summary>
        /// 根据库位ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ShelvesView SearchByID(string id)
        {
            var shelvesView = this.IQueryable.Cast<Shelve>();
            var linq = from shelve in shelvesView
                       where shelve.ID == id
                       select shelve;

            var view = new ShelvesView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据订单号查询
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public ShelvesView SearchByOrderID(string orderID)
        {
            //根据库存查出该入库订单所在的库位
            var shelveIDs = (from storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Storages>()
                             where storage.FormID == orderID && storage.Total > 0
                             select storage.ShelveID).Distinct().ToArray();

            var shelvesView = this.IQueryable.Cast<Shelve>();
            var linq = from shelve in shelvesView
                       where shelveIDs.Contains(shelve.ID)
                       select shelve;

            var view = new ShelvesView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据所属公司查询
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public ShelvesView SearchByCompany(string company)
        {
            var shelvesView = this.IQueryable.Cast<Shelve>();
            var linq = from shelve in shelvesView
                       where shelve.Company.Contains(company)
                       select shelve;

            var view = new ShelvesView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据库位查询
        /// </summary>
        /// <param name="shelveCode"></param>
        /// <returns></returns>
        public ShelvesView SearchByShelveCode(string shelveCode)
        {
            var shelvesView = this.IQueryable.Cast<Shelve>();
            var linq = from shelve in shelvesView
                       where shelve.Code == shelveCode
                       select shelve;

            var view = new ShelvesView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据型号查询
        /// </summary>
        /// <param name="partnumber"></param>
        /// <returns></returns>
        public ShelvesView SearchByPartnumber(string partnumber)
        {
            //根据库存查出该型号所在的库位
            var shelveIDs = (from storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Storages>()
                             join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Products>()
                             on storage.ProductID equals product.ID
                             where storage.Total > 0 && product.Partnumber.StartsWith(partnumber)
                             select storage.ShelveID).Distinct().ToArray();

            var shelvesView = this.IQueryable.Cast<Shelve>();
            var linq = from shelve in shelvesView
                       where shelveIDs.Contains(shelve.ID)
                       select shelve;

            var view = new ShelvesView(this.Reponsitory, linq);
            return view;
        }
        #endregion

        /// <summary>
        /// 新增库位
        /// </summary>
        /// <param name="jobject"></param>
        public void Enter(JObject jobject)
        {
            var args = new
            {
                Code = jobject["Code"]?.Value<string>().Trim(),
                Size = jobject["Size"]?.Value<string>() ?? "",
                Company = jobject["Company"]?.Value<string>() ?? "",
                Summary = jobject["Summary"]?.Value<string>() ?? "",
            };

            if (string.IsNullOrEmpty(args.Code))
                throw new ArgumentNullException("库位号不能为空");

            //库位暂时只做新增，不做编辑
            if (!Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Shelves>().Any(item => item.Code == args.Code))
            {
                Reponsitory.Insert(new Layers.Data.Sqls.PsWms.Shelves()
                {
                    ID = args.Code, //库位ID暂时与库位号保持一致
                    Code = args.Code,
                    Size = args.Size,
                    Company = args.Company,
                    Summary = args.Summary,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                });
            }
            else
            {
                throw new Exception($"该库位【{args.Code}】已存在");
            }
        }

        /// <summary>
        /// 删除库位
        /// </summary>
        /// <param name="id"></param>
        public void Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("库位id不能为空");

            if (Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Storages>().Any(item => item.ShelveID == id))
                throw new NotSupportedException("该库位正在使用中，不能删除");

            Reponsitory.Delete<Layers.Data.Sqls.PsWms.Shelves>(item => item.ID == id);
        }
    }
}
