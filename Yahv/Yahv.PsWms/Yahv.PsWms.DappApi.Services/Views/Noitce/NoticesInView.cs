using Layers.Data;
using Layers.Data.Sqls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.DappApi.Services.Enums;
using Yahv.PsWms.DappApi.Services.Models;
using Yahv.PsWms.DappApi.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Http;
using PKeyType = Yahv.PsWms.DappApi.Services.Enums.PKeyType;

namespace Yahv.PsWms.DappApi.Services.Views
{
    abstract public class _NoticesView : DepthView<MyNotice, object, PsWmsRepository>
    {
        public _NoticesView()
        {
        }

        protected _NoticesView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        protected _NoticesView(PsWmsRepository reponsitory, IQueryable<MyNotice> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        /// <summary>
        /// NoticeTransport: Json对象 转 LinqToSql对象
        /// </summary>
        /// <param name="jtoken"></param>
        /// <returns></returns>
        protected Layers.Data.Sqls.PsWms.NoticeTransports EnterNoticeTransport(JToken jtoken)
        {
            if (jtoken == null || jtoken.Count() == 0)
            {
                return null;
            }

            return new Layers.Data.Sqls.PsWms.NoticeTransports
            {
                //ID = jtoken["ID"]?.Value<string>(),
                TransportMode = jtoken["TransportMode"].Value<int>(),
                Carrier = jtoken["Carrier"].Value<string>(),
                WaybillCode = jtoken["WaybillCode"].Value<string>(),
                ExpressPayer = jtoken["ExpressPayer"].Value<int?>(),
                ExpressTransport = jtoken["ExpressTransport"]?.Value<string>(), // 如果没有存什么值
                ExpressEscrow = jtoken["ExpressEscrow"]?.Value<string>(), //运费账户, 只在FreightPayer.ThirdParty时候起作用
                ExpressFreight = jtoken["ExpressFreight"].Value<decimal?>() ?? 0,
                TakingTime = jtoken["TakingTime"].Value<DateTime?>(),
                TakerName = jtoken["TakerName"].Value<string>(),
                TakerLicense = jtoken["TakerLicense"].Value<string>(),
                TakerPhone = jtoken["TakerPhone"].Value<string>(),
                TakerIDType = jtoken["TakerIDType"].Value<int?>(),
                TakerIDCode = jtoken["TakerIDCode"].Value<string>(),
                Address = jtoken["Address"].Value<string>(),
                Contact = jtoken["Contact"].Value<string>(),
                Phone = jtoken["Phone"].Value<string>(),
                Email = jtoken["Email"].Value<string>(),
                Summary = jtoken["Summary"].Value<string>(),
            };

        }

        /// <summary>
        /// 保存NoticeTransports对象
        /// </summary>
        /// <param name="transports"></param>
        /// <returns></returns>
        protected string EnterTransport(Layers.Data.Sqls.PsWms.NoticeTransports transports, string id)
        {
            if (transports == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                transports.ID = id = PKeySigner.Pick(PKeyType.NoticeTransport);
                transports.CreateDate = DateTime.Now;
                transports.ModifyDate = DateTime.Now;
                this.Reponsitory.Insert(transports);

            }
            else
            {
                transports.ID = id;
                transports.CreateDate = DateTime.Now;
                transports.ModifyDate = DateTime.Now;
                this.Reponsitory.Update<Layers.Data.Sqls.PsWms.NoticeTransports>(transports, item => item.ID == id);
            }

            return id;
        }

        /// <summary>
        /// NoticeItems: Json对象 转 LinqToSql对象
        /// </summary>
        /// <param name="jtoken"></param>
        /// <returns></returns>
        protected Layers.Data.Sqls.PsWms.NoticeItems ToNoticeItems(JToken jtoken)
        {
            var source = jtoken["Source"]?.Value<int?>();
            var inputID = jtoken["InputID"].Value<string>()?.Trim();

            return new Layers.Data.Sqls.PsWms.NoticeItems
            {
                Source = source.HasValue ? source.Value : (int)NoticeSource.Tracker,
                InputID = string.IsNullOrEmpty(inputID) ? PKeySigner.Pick(PKeyType.Input) : inputID,
                CustomCode = jtoken["CustomCode"].Value<string>(),
                StocktakingType = jtoken["StocktakingType"].Value<int>(),
                Mpq = jtoken["Mpq"].Value<int>(),
                PackageNumber = jtoken["PackageNumber"].Value<int>(),
                Total = jtoken["Total"].Value<int>(),
                Currency = jtoken["Currency"]?.Value<int?>() ?? 1,
                UnitPrice = jtoken["UnitPrice"].Value<decimal?>() ?? 0,
                Supplier = jtoken["Supplier"]?.Value<string>(),
                ClientID = jtoken["ClientID"]?.Value<string>(),
                FormItemID = jtoken["OrderItemID"]?.Value<string>(),
            };
        }

        /// <summary>
        /// 保存 NoticeItems对象
        /// </summary>
        /// <param name="noticeItem"></param>
        /// <param name="noticeID"></param>
        /// <param name="productID"></param>
        /// <param name="noticeFormID"></param>
        /// <returns></returns>
        protected string EnterNoticeItems(Layers.Data.Sqls.PsWms.NoticeItems noticeItem, string noticeID, string productID, string noticeFormID)
        {
            string id;
            var item = new NoticeItemsView(this.Reponsitory).
                SingleOrDefault(nitem => nitem.FormItemID == noticeItem.FormItemID || nitem.InputID == noticeItem.InputID);
            if (item == null)
            {
                noticeItem.ID = id = PKeySigner.Pick(PKeyType.NoticeItem);
                noticeItem.NoticeID = noticeID;
                noticeItem.ProductID = productID;
                noticeItem.FormID = noticeFormID;
                noticeItem.CreateDate = DateTime.Now;
                this.Reponsitory.Insert(noticeItem);
            }
            else
            {
                id = item.ID;
                this.Reponsitory.Update<Layers.Data.Sqls.PsWms.NoticeItems>(new
                {
                    NoticeID = noticeID,
                    ProductID = productID,
                    FormID = noticeFormID,
                    FormItemID = noticeItem.FormItemID,
                    CreateDate = DateTime.Now,

                }, nitem => nitem.ID == item.ID);
            }

            return id;
        }

        /// <summary>
        /// Product : Json对象转LinqToSql对象
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        protected Layers.Data.Sqls.PsWms.Products SaveProduct(JToken product)
        {
            return new Layers.Data.Sqls.PsWms.Products
            {
                Partnumber = product["Partnumber"].Value<string>(),
                Brand = product["Brand"].Value<string>(),
                Package = product["Package"].Value<string>(),
                DateCode = product["DateCode"].Value<string>(),
                Mpq = product["Mpq"].Value<int?>(),
                Moq = product["Moq"].Value<int?>(),
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
            };
        }


        /// <summary>
        /// Product : Json对象转LinqToSql对象
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        protected Layers.Data.Sqls.PsWms.Products ToProduct(JToken product)
        {
            return new Layers.Data.Sqls.PsWms.Products
            {
                Partnumber = product["Partnumber"].Value<string>(),
                Brand = product["Brand"].Value<string>(),
                Package = product["Package"].Value<string>(),
                DateCode = product["DateCode"].Value<string>(),
                Mpq = product["Mpq"].Value<int?>(),
                Moq = product["Moq"]?.Value<int?>() ?? 1,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
            };
        }

        /// <summary>
        /// 保存 Product对象
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        protected string EnterProduct(Layers.Data.Sqls.PsWms.Products product)
        {
            var productsView = new ProductsView(this.Reponsitory);
            //var productID = productsView.SingleOrDefault(item => item.Partnumber == product.Partnumber && item.Brand == product.Brand && item.Package == product.Package && item.DateCode == product.DateCode && item.Mpq == product.Mpq && item.Moq == product.Moq)?.ID;
            //经过咨询陈经理, NoticeItem中的ProudctID 和 Sorting中的ProductID 没有关系, 每次都重新生成
            string productID = null;
            product.ID = productID = PKeySigner.Pick(PKeyType.Product);
            this.Reponsitory.Insert(product);
            return productID;
        }

        protected Layers.Data.Sqls.PsWms.Products[] EnterProduct(params Layers.Data.Sqls.PsWms.Products[] products)
        {
            var productsView = new ProductsView(this.Reponsitory);
            var ids = PKeySigner.Series(PKeyType.Product, products.Length);
            for (int index = 0; index < products.Length; index++)
            {
                products[index].ID = ids[index];
            }


            //var productID = productsView.SingleOrDefault(item => item.Partnumber == product.Partnumber && item.Brand == product.Brand && item.Package == product.Package && item.DateCode == product.DateCode && item.Mpq == product.Mpq && item.Moq == product.Moq)?.ID;
            //经过咨询陈经理, NoticeItem中的ProudctID 和 Sorting中的ProductID 没有关系, 每次都重新生成

            this.Reponsitory.Insert(products);
            return products;
        }

    }

    /// <summary>
    /// 入库分拣视图
    /// </summary>
    public class NoticesInView : _NoticesView
    {
        #region 构造函数
        public NoticesInView()
        {
        }

        protected NoticesInView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        protected NoticesInView(PsWmsRepository reponsitory, IQueryable<MyNotice> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        #endregion

        protected override IEnumerable<object> OnMyPage(IQueryable<MyNotice> iquery, bool isPaging)
        {
            var ienum_iquery = iquery.ToArray();
            int total = iquery.Count();

            var noticeTransportsView = new NoticeTransportsView(this.Reponsitory);

            if (isPaging)
            {
                #region 视图补全

                var transportsID = ienum_iquery.Select(item => item.ConsignorID);
                var ienum_transports = noticeTransportsView.Where(item => transportsID.Contains(item.ID));

                var clientsID = ienum_iquery.Select(item => item.ClientID);
                var clientView = from client in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.ClientsTopView>()
                                 where clientsID.Contains(client.ID)
                                 select new
                                 {
                                     client.ID,
                                     client.Name
                                 };

                var ienum_clients = clientView.ToArray();
                var linq = from notice in ienum_iquery
                           join _client in ienum_clients on notice.ClientID equals _client.ID into clients
                           from client in clients.DefaultIfEmpty()
                           join consignor in ienum_transports on notice.ConsignorID equals consignor.ID into consignors
                           join _admin in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.AdminsTopView>() on notice.TrackerID equals _admin.ID into admins
                           from admin in admins.DefaultIfEmpty()
                           select new
                           {
                               notice.ClientID,
                               ClientName = client?.Name, //客户
                               notice.FormID, //订单ID
                               notice.CreateDate, //通知时间
                               consignors.SingleOrDefault()?.TransportMode, // 货运类型
                               notice.Status,
                               notice.TrackerID,
                               notice.ID,
                               TrackerName = admin?.RealName,
                           };
                var ienum_linq = linq.ToArray();

                var result = ienum_linq.Select(item => new
                {
                    item.ClientID,
                    item.ClientName,
                    item.FormID,
                    item.CreateDate,
                    item.TransportMode,
                    TransportModeDes = item.TransportMode?.GetDescription(),
                    item.Status,
                    StatusDes = item.Status.GetDescription(),
                    item.TrackerID,
                    item.TrackerName,
                    item.ID,
                });

                #endregion

                return result;

                //return new MyPage<object>()
                //{
                //    Total = total,
                //    Size = pageSize.Value,
                //    Index = pageIndex.Value,
                //    Data = result,
                //};
            }
            else
            {

                #region 补充完整对象

                //获取运输信息

                var transportsID = ienum_iquery.Select(item => item.ConsigneeID).
                    Concat(ienum_iquery.Select(item => item.ConsignorID));
                var ienum_transports = noticeTransportsView.Where(item => transportsID.Contains(item.ID));

                var requireView = new RequiresView(this.Reponsitory);

                var clientsID = ienum_iquery.Select(item => item.ClientID);
                var clientView = from client in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.ClientsTopView>()
                                 where clientsID.Contains(client.ID)
                                 select new
                                 {
                                     client.ID,
                                     client.Name
                                 };
                var ienum_clients = clientView.ToArray();

                var linq = from notice in ienum_iquery
                           join _client in ienum_clients on notice.ClientID equals _client.ID into clients
                           from client in clients.DefaultIfEmpty()
                           join consignee in ienum_transports on notice.ConsigneeID equals consignee.ID into consignees
                           join consignor in ienum_transports on notice.ConsignorID equals consignor.ID into consignors
                           join require in requireView on notice.ID equals require.NoticeID into requires
                           select new
                           {
                               ID = notice.ID,
                               ClientID = notice.ClientID,
                               CompanyID = notice.CompanyID,
                               ConsigneeID = notice.ConsigneeID,
                               ConsignorID = notice.ConsignorID,
                               FormID = notice.FormID,
                               NoticeType = notice.NoticeType,
                               Status = notice.Status,
                               WarehouseID = notice.WarehouseID,
                               WaybillID = notice.WaybillID,
                               CreateDate = notice.CreateDate,
                               ModifyDate = notice.ModifyDate,
                               Summary = notice.Summary,
                               Exception = notice.Exception,
                               Consignee = consignees.SingleOrDefault(),
                               Consignor = consignors.SingleOrDefault(),
                               ClientName = client?.Name,
                               TrackerID = notice.TrackerID,
                               requires = requires.ToArray(),
                           };

                #endregion

                var result = from notice in linq
                             select new
                             {
                                 notice.ClientID,
                                 notice.ID,
                                 notice.FormID, //订单号
                                 notice.NoticeType,//通知类型
                                 notice.Status,
                                 StatusDes = notice.Status.GetDescription(),
                                 NoticeTypeDes = notice.NoticeType.GetDescription(),
                                 TransportMode = notice.Consignor?.TransportMode, //运输方式
                                 TransportModeDes = notice.Consignor?.TransportMode.GetDescription(),
                                 notice.ClientName, //客户名
                                 ConsignorID = notice.Consignor?.ID,
                                 notice.Consignor?.Carrier, //承运商
                                 notice.WaybillID,// 运单号  // 此处有疑问?????
                                 notice.Consignor?.Address, //提货地址
                                 notice.Consignor?.TakingTime, // 提货时间
                                 notice.Consignor?.TakerName,// 提货人
                                 notice.Consignor?.TakerLicense, // 车牌
                                 notice.Consignor?.TakerPhone, //提送货人电话
                                 notice.Consignor?.Phone, //提送货联系电话
                                 notice.Consignor?.Contact, //联系人
                                 notice.Consignor?.WaybillCode, // 运单号
                                 notice.Summary,
                                 notice.Exception,
                                 notice.TrackerID,
                                 Requires = notice.requires.Select(item => new
                                 {
                                     Contents = item.Contents != null ? item.Contents : item.Name
                                 }).ToArray(),
                             };
                return result;

                //return new MyPage<object>()
                //{
                //    Total = total,
                //    Size = pageSize.Value,
                //    Index = pageIndex,
                //    Data = result.ToArray(),
                //};
            }
        }

        protected override IQueryable<MyNotice> GetIQueryable()
        {
            var view = from notice in new NoticesView(this.Reponsitory)
                       where notice.NoticeType == NoticeType.Inbound && notice.Status != NoticeStatus.Closed
                       orderby notice.CreateDate descending
                       select new MyNotice
                       {
                           ID = notice.ID,
                           ClientID = notice.ClientID,
                           CompanyID = notice.CompanyID,
                           ConsigneeID = notice.ConsigneeID,
                           ConsignorID = notice.ConsignorID,
                           FormID = notice.FormID,
                           NoticeType = notice.NoticeType,
                           Status = notice.Status,
                           WarehouseID = notice.WarehouseID,
                           WaybillID = notice.WaybillID,
                           TrackerID = notice.TrackerID,
                           CreateDate = notice.CreateDate,
                           ModifyDate = notice.ModifyDate,
                           Summary = notice.Summary,
                           Exception = notice.Exception,
                       };

            return view;
        }

        #region 搜索方法
        /// <summary>
        /// 根据WarehouseID搜索
        /// </summary>
        /// <param name="whid"></param>
        /// <returns></returns>
        public NoticesInView SearchByWarehouseID(string whid)
        {
            var noticeInView = this.IQueryable.Cast<MyNotice>();
            var linq = from notice in noticeInView
                       where notice.WarehouseID.ToLower().StartsWith(whid)
                       select notice;
            var view = new NoticesInView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据Notice状态搜索
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public NoticesInView SearchByStatus(NoticeStatus[] status)
        {
            var noticeInView = this.IQueryable.Cast<MyNotice>();
            var linq = from notice in noticeInView
                       where status.Contains(notice.Status)
                       select notice;
            var view = new NoticesInView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据型号搜索
        /// </summary>
        /// <param name="partnumber"></param>
        /// <returns></returns>
        public NoticesInView SearchByPartnumber(string partnumber)
        {
            var noticeView = this.IQueryable.Cast<MyNotice>();
            //var noticesIDView = from noticeItem in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.NoticeItems>()
            //                    join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Products>() on noticeItem.ProductID equals product.ID
            //                    where product.Partnumber.StartsWith(partnumber)
            //                    select noticeItem.NoticeID;

            var formsID = noticeView.Select(item => item.FormID).Distinct().ToArray();
            var storagesView = new StoragesView(this.Reponsitory).Where(item => formsID.Contains(item.FormID));

            var productIdsNoticeItem = new NoticeItemsView(this.Reponsitory).Where(item => formsID.Contains(item.FormID)).Select(item => item.ProductID).Distinct().ToArray();
            var productIdsStorage = storagesView.Select(item => item.ProductID).Distinct().ToArray();

            var productsView = new ProductsView(this.Reponsitory).Where(item => productIdsNoticeItem.Contains(item.ID) || productIdsStorage.Contains(item.ID));


            //var noticesIDView = from noticeItem in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.NoticeItems>()
            //                    join productNoticeItem in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Products>() on noticeItem.ProductID equals productNoticeItem.ID
            //                    join _storage in storagesView on noticeItem.FormID equals _storage.FormID into storages
            //                    from storage in storages.DefaultIfEmpty()
            //                    join productStorage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Products>() on storage.ProductID equals productStorage.ID                                
            //                    where productNoticeItem.Partnumber.StartsWith(partnumber) || productStorage.Partnumber.StartsWith(partnumber)
            //                    select noticeItem.NoticeID;

            //var noticesIDView = from noticeItem in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.NoticeItems>()
            //                    join productNoticeItem in productsView on noticeItem.ProductID equals productNoticeItem.ID
            //                    join _storage in storagesView on noticeItem.FormID equals _storage.FormID into storages
            //                    from storage in storages.DefaultIfEmpty()
            //                    join productStorage in productsView on storage.ProductID equals productStorage.ID
            //                    where productNoticeItem.Partnumber.StartsWith(partnumber) || productStorage.Partnumber.StartsWith(partnumber)
            //                    select noticeItem.NoticeID;

            var noticeItem_noticesIDView = from noticeItem in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.NoticeItems>()
                                           join productNoticeItem in productsView on noticeItem.ProductID equals productNoticeItem.ID
                                           where productNoticeItem.Partnumber.StartsWith(partnumber)
                                           select noticeItem.NoticeID;

            var storage_noticesIDView = from storage in storagesView
                                        join productStorage in productsView on storage.ProductID equals productStorage.ID
                                        where productStorage.Partnumber.StartsWith(partnumber)
                                        select storage.NoticeID;

            var ienum_noticeItem_noticesID = noticeItem_noticesIDView.ToArray();
            var ienum_storage_noticesID = storage_noticesIDView.ToArray();

            var noticesID = ienum_noticeItem_noticesID.Concat(ienum_storage_noticesID).Distinct().ToArray();


            //var linq = from noticeID in noticesIDView.Distinct()
            //           join notice in noticeView on noticeID equals notice.ID
            //           orderby noticeID descending
            //           select notice;

            var linq = from noticeID in noticesID
                       join notice in noticeView on noticeID equals notice.ID
                       orderby notice.ID descending
                       select notice;                       

            //注意索引
            var view = new NoticesInView(this.Reponsitory, linq.AsQueryable());
            return view;
        }

        /// <summary>
        /// 根据订单ID搜索
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public NoticesInView SearchByOrderID(string orderID)
        {
            var noticeInView = this.IQueryable.Cast<MyNotice>();
            var linq = from notice in noticeInView
                       where notice.FormID == orderID
                       select notice;
            var view = new NoticesInView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据通知ID搜索
        /// </summary>
        /// <param name="noticeID"></param>
        /// <returns></returns>
        public NoticesInView SearchByNoticeID(string noticeID)
        {
            var noticeInView = this.IQueryable.Cast<MyNotice>();
            var linq = from notice in noticeInView
                       where notice.ID == noticeID
                       select notice;
            var view = new NoticesInView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据起止时间搜索
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public NoticesInView SearchByDate(DateTime? start, DateTime? end)
        {
            Expression<Func<MyNotice, bool>> predicate = notice => (start.HasValue ? notice.CreateDate >= start.Value : true)
                && (end.HasValue ? notice.CreateDate < end.Value.AddDays(1) : true);

            var noticeInView = this.IQueryable.Cast<MyNotice>();
            var linq = noticeInView.Where(predicate);

            var view = new NoticesInView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 搜索运输信息条件
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns>入库基本视图</returns>
        public NoticesInView SearchByTransport(Expression<Func<NoticeTransport, bool>> predicate)
        {
            var noticeTransportsView = new NoticeTransportsView(this.Reponsitory);

            var linq = from transport in noticeTransportsView
                       where transport.TransportMode == TransportMode.Express
                       join notice in this.IQueryable on transport.ID equals notice.ConsigneeID
                       select notice;

            linq = from transport in noticeTransportsView.Where(predicate)
                   where transport.TransportMode == TransportMode.Express
                   join notice in this.IQueryable on transport.ID equals notice.ConsigneeID
                   select notice;


            var view = new NoticesInView(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 通过MyNotice数据对象获取数据
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns>入库视图</returns>
        public NoticesInView SearchByNotice(Expression<Func<MyNotice, bool>> predicate)
        {
            var linq = this.IQueryable.Where(predicate);
            var view = new NoticesInView(this.Reponsitory, linq);
            return view;
        }

        #endregion

        /// <summary>
        /// 入库通知
        /// </summary>
        /// <param name="jobject"></param>
        public string Notice(JObject jobject)
        {
            #region 保存更新通知
            string orderID = jobject["OrderID"].Value<string>();
            string noticeid = null;
            var noticeItems = jobject["Items"];
            var consignor = jobject["Consignor"];
            var consignee = jobject["Consignee"];
            var requires = jobject["Requires"];
            var noticesView = new NoticesView(this.Reponsitory);

            var notice = noticesView.SingleOrDefault(item => item.FormID == orderID);
            noticeid = notice == null ? null : notice.ID;

            if (consignor == null || consignor.Count() == 0)
            {
                throw new ArgumentException("Consignor 相关参数不能为 null");
            }
            
            //Enum.Parse
            TransportMode transportMode = (TransportMode)consignor["TransportMode"].Value<int>();
                        
            string consigneeID = null;
            string consignorID = null;
            if (noticeid == null)
            {
                //增加判断逻辑
                consigneeID = EnterTransport(EnterNoticeTransport(consignee), notice?.ConsigneeID);
                consignorID = EnterTransport(EnterNoticeTransport(consignor), notice?.ConsignorID);
            }
            else
            {
                consigneeID = notice.ConsigneeID;
                consignorID = notice.ConsignorID;
            }            

            var noticeFormID = orderID;
            if (string.IsNullOrWhiteSpace(noticeid))
            {
                noticeid = PKeySigner.Pick(PKeyType.Notice);

                this.Reponsitory.Insert(new Layers.Data.Sqls.PsWms.Notices
                {
                    ID = noticeid,
                    ClientID = jobject["ClientID"].Value<string>(),
                    CompanyID = jobject["CompanyID"].Value<string>(),
                    NoticeType = jobject["NoticeType"].Value<int>(),
                    FormID = noticeFormID,
                    TrackerID = jobject["TrackerID"].Value<string>(),
                    ConsigneeID = consigneeID,
                    ConsignorID = consignorID,
                    Summary = jobject["Summary"].Value<string>(),
                    WarehouseID = jobject["WarehouseID"].Value<string>(),
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    Status = transportMode == TransportMode.PickUp ? (int)NoticeStatus.Arranging : (int)NoticeStatus.Processing //入库等待安排提货, 或者是入库分拣
                });
            }
            else
            {
                this.Reponsitory.Update<Layers.Data.Sqls.PsWms.Notices>(new
                {
                    ClientID = jobject["ClientID"].Value<string>(),
                    CompanyID = jobject["CompanyID"].Value<string>(),

                    NoticeType = jobject["NoticeType"].Value<int>(),
                    //NoticeType = (int)Enum.Parse(typeof(NoticeType), jobject["NoticeType"].Value<string>()), //建议
                    FormID = noticeFormID,
                    Summary = jobject["Summary"].Value<string>(),
                    WarehouseID = jobject["WarehouseID"].Value<string>() ?? "SZ",
                    ModifyDate = DateTime.Now,
                }, item => item.ID == noticeid);
            }
            #endregion

            #region 保存Requires
            var requiresView = new RequiresView(this.Reponsitory).Where(item => item.NoticeID == noticeid);
            var ienum_requires = requiresView.ToArray();

            foreach (var require in requires)
            {
                // 思路, 如果是重新进来的需求, 那么原来的需求怎么处理 -- 已解决
                //string id = require["ID"]?.Value<string>()?.Trim();
                //string formID = require["FormID"]?.Value<string>()?.Trim();
                string name = require["Name"].Value<string>().Trim();
                string contents = require["Contents"].Value<string>()?.Trim();
                int? type = require["Type"]?.Value<int?>(); //1 入库， 2 出库 。建议后期修该
                string noticeTransportID = null;

                if (!type.HasValue || type.Value == 1)
                {
                    noticeTransportID = consignorID;
                }
                else if (type.HasValue && type.Value == 2)
                {
                    noticeTransportID = consigneeID;
                }
                else
                {
                    throw new Exception("不知如何处理：require[Type]==" + type);
                }

                var requiry = ienum_requires.FirstOrDefault(item => item.Name == name && item.Contents == contents);

                if (ienum_requires.Count() == 0 || requiry == null)
                {
                    this.Reponsitory.Insert(new Layers.Data.Sqls.PsWms.Requires
                    {
                        ID = PKeySigner.Pick(PKeyType.Require),
                        NoticeID = noticeid,
                        NoticeTransportID = noticeTransportID,
                        Contents = contents,
                        CreateDate = DateTime.Now,
                        Name = name,
                    });
                }
                else
                {
                    this.Reponsitory.Update<Layers.Data.Sqls.PsWms.Requires>(new
                    {
                        NoticeID = noticeid,
                        NoticeTransportID = noticeTransportID,
                        Contents = contents,
                        Name = name,
                    }, item => item.ID == requiry.ID);
                }
            }
            #endregion

            #region 保存更新NoticeItem
            // 思路, 如果noticeitem 是重新过来的情况的话, 可能需要删除以前的某些对应的noticeItem, 因为入库通知端已经删除            

            // 删除时存在疑问, 可以直接删除NoticeItems中不包含的FormItemID , 
            // 
            // 但此FormItemID 是否可能已经被某些Storage已经用过, 如果此时删除FormItemID对应的NoticeItem, 则导致Storage对应的 FormItemID 没有了
            // 现有已分拣的NoticeItemID
            var noticeItemsIDArr = (from item in new NoticeItemsView(this.Reponsitory)
                                    join reportItem in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.ReportItems>()
                                    on item.ID equals reportItem.NoticeItemID
                                    where item.NoticeID == noticeid
                                    select item.ID).Distinct().ToArray();

            // 总体的NoticeItemID
            var noticeItemsIDTotal = (from noticeItem in new NoticeItemsView(this.Reponsitory)
                                      where noticeItem.FormID == orderID
                                      select noticeItem.ID).Distinct().ToArray();

            // 同时更新(storage 和  reportItem )中formitemid为 null的值
            //var reportItemsView = from reportitem in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.ReportItems>()
            //                     where noticeItemsIDArr.Contains(reportitem.NoticeItemID)
            //                     select new
            //                     {
            //                         reportitem.ID,
            //                         reportitem.NoticeItemID,
            //                         reportitem.InputID,
            //                         reportitem.FormItemID
            //                     };
            //var ienum_reportItems = reportItemsView.ToArray();

            //var storagesView = from storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Storages>()
            //                   where noticeItemsIDArr.Contains(storage.NoticeItemID)
            //                   select new
            //                   {
            //                       storage.ID,
            //                       storage.NoticeItemID,
            //                       storage.InputID,
            //                       storage.FormItemID,
            //                   };
            //var ienum_storages = storagesView.ToArray();

            List<string> currentNoticeItemsList = new List<string>();
            foreach (var noticeItem in noticeItems)
            {
                var product = noticeItem["Product"];
                var productID = EnterProduct(SaveProduct(product));
                var inputId = noticeItem["InputID"].Value<string>();
                var formItemID = noticeItem["OrderItemID"].Value<string>();
                string noticeItemID = EnterNoticeItems(ToNoticeItems(noticeItem), noticeid, productID, noticeFormID);
                currentNoticeItemsList.Add(noticeItemID);

                // 当formItemID 不为空时, 而且针对新增的通知项进行更新其formItemID
                if (!string.IsNullOrEmpty(formItemID))
                {
                    // 同时更新(storage 和  reportItem )中formitemid为 null的值
                    this.Reponsitory.Update<Layers.Data.Sqls.PsWms.ReportItems>(new
                    {
                        FormItemID = formItemID,
                    }, item => item.InputID == inputId && item.NoticeItemID == noticeItemID && item.FormItemID == null);


                    this.Reponsitory.Update<Layers.Data.Sqls.PsWms.Storages>(new
                    {
                        FormItemID = formItemID,
                    }, item => item.InputID == inputId && item.NoticeItemID == noticeItemID && item.FormItemID == null);
                }
            }

            // 当前的NoticeItemsID和 已分拣的NoticeItemsID的合并集合
            var noticeItemsID = noticeItemsIDArr.Concat(currentNoticeItemsList).Distinct().ToArray();

            var noticeItemsDelIDArr = noticeItemsIDTotal.Where(item => !noticeItemsID.Contains(item)).Distinct().ToArray();

            // 删除已经不再包含的FormItemID的NoticeItem
            this.Reponsitory.Delete<Layers.Data.Sqls.PsWms.NoticeItems>(item => noticeItemsDelIDArr.Contains(item.ID));

            #endregion

            return noticeid;
        }

        /// <summary>
        /// 分拣入库
        /// </summary>
        public void Sorting(JObject jobjects)
        {
            var sortings = jobjects["Sortings"];

            //这里如果传输错误，但是这里的报错是我们要的
            var noticesID = sortings.Select(item => item["NoticeID"].Value<string>()).Distinct();
            var notices = new NoticesView(this.Reponsitory).Where(item => noticesID.Contains(item.ID));

            //可能会有sql rpc  2100 以上的错误 
            var noticeItemsID = sortings.Select(item => item["NoticeItemID"].Value<string>()?.Trim())
                .Where(item => item != null).Distinct();
            var noticesItems = new NoticeItemsView(this.Reponsitory).Where(item => noticeItemsID.Contains(item.ID));

            var storagesID = PKeySigner.Series(PKeyType.Storage, sortings.Count());
            //var newNoticeItemCount = sortings.Count(item => string.IsNullOrWhiteSpace(item["NoticeItemID"].Value<string>()?.Trim()));
            //var newItemsID = newNoticeItemCount == 0 ? null : PKeySigner.Series(PKeyType.NoticeItem, newNoticeItemCount);

            var products = EnterProduct(sortings.Select(item => this.ToProduct(item["Product"])).ToArray());

            var index_storage = 0;
            //var index_newItem = 0;
            foreach (var sorting in sortings)
            {
                string noticeID = sorting["NoticeID"].Value<string>();
                string noticeItemID = sorting["NoticeItemID"].Value<string>()?.Trim();
                bool isNew = sorting["IsNew"].Value<bool>();

                var noticeItem = noticesItems.SingleOrDefault(item => item.ID == noticeItemID);

                string inputID = noticeItem?.InputID ?? PKeySigner.Pick(PKeyType.Input);
                string formItemID = noticeItem?.FormItemID;

                var product = products[index_storage];
                var productID = product.ID;
                var notice = notices.SingleOrDefault(item => item.ID == noticeID);

                //if (noticeItem != null)
                //{
                //    inputID = noticeItem.InputID;
                //    formItemID = noticeItem.FormItemID;
                //}
                //else
                //{
                //    inputID = PKeySigner.Pick(PKeyType.Input);
                //}

                // 如果NoticeItem不存在, 则新增NoticeItem
                if (isNew)
                {
                    //noticeItemID = newItemsID[index_newItem];

                    this.Reponsitory.Insert(new Layers.Data.Sqls.PsWms.NoticeItems
                    {
                        ID = noticeItemID,
                        NoticeID = noticeID,
                        Source = (int)NoticeSource.Keeper,
                        ProductID = productID,
                        InputID = inputID,
                        CustomCode = sorting["CustomCode"]?.Value<string>(), //当前陈经理不传CustomCode,则默认null
                        StocktakingType = sorting["StocktakingType"].Value<int>(),
                        Mpq = sorting["Mpq"].Value<int>(),
                        PackageNumber = sorting["PackageNumber"].Value<int>(),
                        Total = sorting["Total"].Value<int>(),
                        Currency = 1,
                        UnitPrice = 0,
                        Supplier = sorting["Supplier"]?.Value<string>(),
                        ClientID = notice.ClientID,
                        FormID = notice.FormID,
                        FormItemID = formItemID,
                        CreateDate = DateTime.Now,
                        SorterID = sorting["SorterID"].Value<string>(),
                        ShelveID = sorting["ShelveID"]?.Value<string>(),
                    });
                }



                // 这里的思路： 通过上层（Orders）获取到的数据
                // 本地存储数据与哪个订单，或是通知
                // 在开发中需要同步考虑拆分情况：同属一个通知项或是一个订单项 硬作如何区分

                // 创建及保存Storage
                this.Reponsitory.Insert(new Layers.Data.Sqls.PsWms.Storages
                {
                    ID = storagesID[index_storage],
                    ClientID = notice.ClientID,
                    NoticeID = noticeID,
                    NoticeItemID = noticeItemID,
                    ProductID = productID,
                    InputID = inputID, // 分批入库要再产生新的InputID吗， 怎样判断是否是分批入库或者拆分
                    Type = (int)StorageType.Store,
                    Islock = true, // 只有入库复核完后，才不对库存进行锁定
                    StocktakingType = sorting["StocktakingType"].Value<int>(),
                    Mpq = sorting["Mpq"].Value<int>(),
                    PackageNumber = sorting["DeliveryCount"].Value<int>(), //本次到货数量用哪个值
                    Total = sorting["Total"].Value<int>(),
                    SorterID = sorting["SorterID"].Value<string>(),
                    FormID = notice.FormID,
                    FormItemID = formItemID,  // 如果新增的NoticeItem则对应的FormItemID为null
                    Currency = noticeItem == null ? 1 : (int)noticeItem.Currency, // 如果没有
                    UnitPrice = noticeItem == null ? 0 : noticeItem.UnitPrice, // 如果没有提供怎么处理
                    ShelveID = sorting["ShelveID"]?.Value<string>(), // 分拣时给货架ID吗
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    Exception = sorting["Exception"]?.Value<string>(),
                    Summary = sorting["Summary"]?.Value<string>(),
                    Unique = "ST:" + storagesID[index_storage],
                });

                index_storage++;
                //index_newItem++;
            }
        }

        /// <summary>
        /// 入库复核
        /// </summary>
        /// <param name="noticeID"></param>
        public void Review(string noticeID, string reviewerID)
        {
            var notice = new NoticesView(this.Reponsitory).SingleOrDefault(item => item.ID == noticeID);

            if (notice == null)
            {
                throw new Exception($"没有找到NoticeID: {noticeID}对应的通知");
            }

            var noticeItemsView = new NoticeItemsView(this.Reponsitory).Where(item => item.NoticeID == noticeID);
            var storagesView = new StoragesView(this.Reponsitory).Where(item => item.NoticeID == noticeID);

            string formID = notice.FormID;
            var ienum_storageIDs = storagesView.Select(item => item.ID).ToArray();


            //var deliveriesView = from storage in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Storages>()
            //                     join noticeItem in noticeItemsView on storage.NoticeItemID equals noticeItem.ID
            //                     where storage.NoticeID == noticeID
            //                     select new
            //                     {
            //                         storage,
            //                         noticeItem,
            //                     };

            var deliveriesView = from storage in storagesView
                                 join noticeItem in noticeItemsView on storage.NoticeItemID equals noticeItem.ID                                 
                                 select new
                                 {
                                     storage.ProductID,
                                     storage.InputID,
                                     StorageMpq = storage.Mpq,
                                     StoragePackageNumber = storage.PackageNumber,
                                     StorageTotal = storage.Total,
                                     FormID = storage.FormID,
                                     FormItemID = storage.FormItemID,
                                     InCurrency = storage.Currency,
                                     InUnitPrice = storage.UnitPrice,
                                     storage.ClientID,
                                     storage.ShelveID,
                                     Exception = storage.Exception,

                                     noticeItem,
                                     NoticeID = noticeItem.NoticeID,
                                     NoticeItemID = noticeItem.ID,
                                     NoticeMpq = noticeItem.Mpq,
                                     NoticePackageNumber = noticeItem.PackageNumber,
                                     NoticeTotal = noticeItem.Total,
                                 };

            var ienum_deliveries = deliveriesView.ToArray();

            string reportID = PKeySigner.Pick(PKeyType.Report);

            // 生成对应的Report
            this.Reponsitory.Insert(new Layers.Data.Sqls.PsWms.Reports
            {
                ID = reportID,
                ReportType = (int)ReportType.Inbound,
                ReviewDateTime = DateTime.Now,
                ReviewerID = reviewerID,
                Status = null,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                FormID = formID,
            });

            foreach (var view in ienum_deliveries)
            {
                // 生成ReportItems
                this.Reponsitory.Insert(new Layers.Data.Sqls.PsWms.ReportItems
                {
                    ID = PKeySigner.Pick(PKeyType.ReportItem),
                    ReportID = reportID,
                    NoticeID = view.noticeItem.NoticeID,
                    NoticeItemID = view.noticeItem.ID,
                    ProductID = view.ProductID,
                    InputID = view.InputID,
                    NoticeMpq = view.noticeItem.Mpq,
                    NoticePackageNumber = view.noticeItem.PackageNumber,
                    NoticeTotal = view.noticeItem.Total,
                    StorageMpq = view.StorageMpq,
                    StoragePackageNumber = view.StoragePackageNumber,
                    StorageTotal = view.StorageTotal,
                    AdminID = reviewerID,
                    FormID = view.FormID,
                    FormItemID = view.FormItemID,
                    InCurrency = (int)view.InCurrency,
                    InUnitPrice = view.InUnitPrice,
                    OutCurrency = (int)Yahv.Underly.Currency.CNY,
                    OutUnitPrice = 0,
                    ClientID = view.ClientID,
                    ShelveID = view.ShelveID, // 该货架ID 只是保留设计，不是上架的ID
                    CreateDate = DateTime.Now,
                    Exception = view.Exception,
                    ReviewDate = DateTime.Now,
                    ReviewerID = reviewerID,
                });
            }

            // 更新Storages锁定的状态
            this.Reponsitory.Update<Layers.Data.Sqls.PsWms.Storages>(new
            {
                Islock = false,
            }, item => ienum_storageIDs.Contains(item.ID));

            // 更新通知复核后的状态--- Completed完成
            this.Reponsitory.Update<Layers.Data.Sqls.PsWms.Notices>(new
            {
                Status = (int)NoticeStatus.Completed,
            }, item => item.ID == noticeID);

            // 通知入库通知的对应订单检查更新入库到货视图
            // ToDo:

            string url = Services.Enums.FromType.ChangeOrder.GetDescription();

            ApiHelper.Current.JPost(url, new OrderChange
            {
                OrderID = formID,
                OrderStatus = 200,
            });
        }

        /// <summary>
        /// 更新修改入库通知详情中的承运商信息，运单号,以及异常备注
        /// </summary>
        /// <param name="jobject"></param>
        public void Update(JObject jobject)
        {
            var noticeID = jobject["NoticeID"].Value<string>();
            var consignorID = jobject["ConsignorID"].Value<string>();
            var carrier = jobject["Carrier"].Value<string>();
            var waybillCode = jobject["WaybillCode"].Value<string>();
            var exception = jobject["Exception"].Value<string>();
            //var takerName = jobject["TakerName"].Value<string>();
            //var takerLicense = jobject["TakerLicense"].Value<string>();
            //var transportMode = jobject["TransportMode"].Value<int>();

            if (!string.IsNullOrEmpty(exception))
            {
                this.Reponsitory.Update<Layers.Data.Sqls.PsWms.Notices>(new
                {
                    Exception = exception,
                }, item => item.ID == noticeID);
            }

            this.Reponsitory.Update<Layers.Data.Sqls.PsWms.NoticeTransports>(new
            {
                WaybillCode = waybillCode,
                Carrier = carrier,
            }, item => item.ID == consignorID);

            //if (transportMode == 1)
            //{
            //    this.Reponsitory.Update<Layers.Data.Sqls.PsWms.NoticeTransports>(new
            //    {
            //        TakerName = takerName,
            //        _TakerLicense = takerLicense,                    
            //    }, item => item.ID == consignorID);
            //}

            //if (transportMode == 2)
            //{
            //    this.Reponsitory.Update<Layers.Data.Sqls.PsWms.NoticeTransports>(new
            //    {
            //        WaybillCode = waybillCode,
            //        Carrier = carrier,
            //    }, item => item.ID == consignorID);
            //}
        }

        /// <summary>
        /// 入库复核后通知订单状态更新
        /// </summary>
        private class OrderChange
        {
            public string OrderID { get; set; }

            public int OrderStatus { get; set; }
        }

        /// <summary>
        /// 生成新的NoticeItemID
        /// </summary>
        /// <returns></returns>
        public string GetNoticeItemID()
        {
            return PKeySigner.Pick(PKeyType.NoticeItem);
        }
    }

    public class MyNotice : Linq.IUnique
    {
        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 库房ID
        /// </summary>
        public string WarehouseID { get; set; }

        /// <summary>
        /// 所属客户
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 内部公司，所属公司
        /// </summary>
        public string CompanyID { get; set; }

        /// <summary>
        /// 通知类型: Inbound 入库 1, Outbound 出库 2, InAndOut 即入即出 3,
        /// </summary>
        public NoticeType NoticeType { get; set; }

        /// <summary>
        /// 来自的订单ID
        /// </summary>
        public string FormID { get; set; }

        /// <summary>
        /// 交货人信息ID
        /// </summary>
        public string ConsignorID { get; set; }

        /// <summary>
        /// 收货人信息ID
        /// </summary>
        public string ConsigneeID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 通知状态
        /// </summary>
        public NoticeStatus Status { get; set; }

        /// <summary>
        /// 运单ID
        /// </summary>
        public string WaybillID { get; set; }

        /// <summary>
        /// 异常备注
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// 跟单，客服
        /// </summary>
        public string TrackerID { get; set; }

        /// <summary>
        /// 信息备注
        /// </summary>
        public string Summary { get; set; }
        #endregion
    }


}
