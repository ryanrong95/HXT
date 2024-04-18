//using Layers.Data;
//using Layers.Data.Sqls;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;
//using Yahv.PsWms.DappApi.Services.Enums;
//using Yahv.PsWms.DappApi.Services.Models;
//using Yahv.PsWms.DappApi.Services.Views;
//using Yahv.Linq;

//namespace Yahv.PsWms.DappApi.Services.Notcies_Chenhan.Views
//{
//    abstract public class _NoticesView : DepthView<MyNotice, PsWmsRepository>
//    {
//        protected Layers.Data.Sqls.PsWms.NoticeTransports EnterNoticeTransport(JToken jtoken)
//        {
//            return new Layers.Data.Sqls.PsWms.NoticeTransports
//            {
//                TransportMode = jtoken["TransportMode"].Value<int>(),
//                Carrier = jtoken["Carrier"].Value<string>(),
//                WaybillCode = jtoken["WaybillCode"].Value<string>(),
//                ExpressPayer = jtoken["ExpressPayer"].Value<int>(),
//                ExpressTransport = jtoken["ExpressTransport"]?.Value<string>(), // 如果没有存什么值
//                ExpressEscrow = jtoken["ExpressEscrow"]?.Value<string>(), //运费账户, 只在FreightPayer.ThirdParty时候起作用
//                ExpressFreight = jtoken["ExpressFreight"].Value<decimal?>() ?? 0,
//                TakingTime = jtoken["TakingTime"].Value<DateTime?>(),
//                TakerName = jtoken["TakerName"].Value<string>(),
//                TakerLicense = jtoken["TakerLicense"].Value<string>(),
//                TakerPhone = jtoken["TakerPhone"].Value<string>(),
//                TakerIDType = jtoken["TakerIDType"].Value<int?>(),
//                TakerIDCode = jtoken["TakerIDCode"].Value<string>(),
//                Address = jtoken["Address"].Value<string>(),
//                Contact = jtoken["Contact"].Value<string>(),
//                Phone = jtoken["Phone"].Value<string>(),
//                Email = jtoken["Email"].Value<string>(),
//                Summary = jtoken["Summary"].Value<string>(),
//            };

//        }
//        protected string EnterTransport(Layers.Data.Sqls.PsWms.NoticeTransports transports)
//        {
//            string id = transports.ID;

//            if (string.IsNullOrWhiteSpace(id))
//            {
//                transports.ID = id = PKeySigner.Pick(PKeyType.NoticeTransport);
//                transports.CreateDate = DateTime.Now;
//                transports.ModifyDate = DateTime.Now;
//                this.Reponsitory.Insert(transports);

//            }
//            else
//            {
//                transports.ModifyDate = DateTime.Now;
//                this.Reponsitory.Update<Layers.Data.Sqls.PsWms.NoticeTransports>(transports, item => item.ID == id);
//            }

//            return id;
//        }

//    }

//    public class _NoticesInView : _NoticesView
//    {
//        protected override MyPage<object> OnMyPage(IQueryable<MyNotice> iquery, int? pageIndex = default(int?), int? pageSize = default(int?))
//        {
//            var ienum_iquery = iquery.ToArray();
//            var total = iquery.Count();

//            var noticeTransportsView = new NoticeTransportsView(this.Reponsitory);

//            if (pageIndex.HasValue && pageSize.HasValue)
//            {
//                #region 视图补全

//                var transportsID = ienum_iquery.Select(item => item.ConsigneeID);
//                var ienum_transports = noticeTransportsView.Where(item => transportsID.Contains(item.ID));

//                var clientsID = ienum_iquery.Select(item => item.ClientID);
//                var clientView = from client in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.ClientsTopView>()
//                                 where clientsID.Contains(client.ID)
//                                 select new
//                                 {
//                                     client.ID,
//                                     client.Name
//                                 };
//                var ienum_clients = clientView.ToArray();
//                var linq = from notice in ienum_iquery
//                           join client in ienum_clients on notice.ClientID equals client.ID
//                           join consignee in ienum_transports on notice.ConsigneeID equals consignee.ID into consignees
//                           select new
//                           {
//                               ClientName = client.Name, //客户
//                               notice.FormID, //订单ID
//                               notice.CreateDate, //通知时间
//                               consignees.Single().TransportMode, // 货运类型
//                               notice.Status,
//                               notice.TrackerID,
//                               notice.ID,
//                           };

//                #endregion

//                return new MyPage<object>()
//                {
//                    Total = total,
//                    Size = pageSize.Value,
//                    Index = pageIndex.Value,
//                    Data = linq.ToArray(),
//                };
//            }
//            else
//            {

//                #region 补充完整对象

//                //获取运输信息

//                var transportsID = ienum_iquery.Select(item => item.ConsigneeID).
//                    Concat(ienum_iquery.Select(item => item.ConsignorID));
//                var ienum_transports = noticeTransportsView.Where(item => transportsID.Contains(item.ID));
//                var clientsID = ienum_iquery.Select(item => item.ClientID);
//                var clientView = from client in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.ClientsTopView>()
//                                 where clientsID.Contains(client.ID)
//                                 select new
//                                 {
//                                     client.ID,
//                                     client.Name
//                                 };
//                var ienum_clients = clientView.ToArray();

//                var linq = from notice in ienum_iquery
//                           join client in ienum_clients on notice.ClientID equals client.ID
//                           join consignee in ienum_transports on notice.ConsigneeID equals consignee.ID into consignees
//                           join consignor in ienum_transports on notice.ConsignorID equals consignor.ID into consignors
//                           select new
//                           {
//                               ID = notice.ID,
//                               ClientID = notice.ClientID,
//                               CompanyID = notice.CompanyID,
//                               ConsigneeID = notice.ConsigneeID,
//                               ConsignorID = notice.ConsignorID,
//                               FormID = notice.FormID,
//                               NoticeType = notice.NoticeType,
//                               Status = notice.Status,
//                               WarehouseID = notice.WarehouseID,
//                               WaybillID = notice.WaybillID,
//                               CreateDate = notice.CreateDate,
//                               ModifyDate = notice.ModifyDate,
//                               Summary = notice.Summary,
//                               Exception = notice.Exception,
//                               Consignee = consignees.SingleOrDefault(),
//                               Consignor = consignors.SingleOrDefault(),
//                               ClientName = client.Name,
//                               TrackerID = notice.TrackerID,
//                           };

//                #endregion

//                var result = from notice in linq
//                             select new
//                             {
//                                 notice.ID,
//                                 notice.FormID, //订单号
//                                 notice.NoticeType,//通知类型
//                                 notice.Consignee.TransportMode, //运输方式
//                                 notice.ClientName, //客户名
//                                 notice.Consignee.Carrier, //承运商
//                                 notice.WaybillID,// 运单号  // 此处有疑问?????
//                                 notice.Consignee.Address, //提货地址
//                                 notice.Consignee.TakingTime, // 提货时间
//                                 notice.Consignee.TakerName,// 提货人
//                                 notice.Consignee.TakerLicense, // 车牌
//                                 notice.Consignee.TakerPhone, //提送货人电话
//                                 notice.Consignee.Phone, //提送货联系电话
//                                 notice.Summary,
//                                 notice.Exception,
//                                 notice.TrackerID,
//                             };

//                return new MyPage<object>()
//                {
//                    Total = total,
//                    Size = pageSize.Value,
//                    Index = pageIndex,
//                    Data = result.ToArray(),
//                };
//            }
//        }

//        protected override IQueryable<MyNotice> GetIQueryable()
//        {
//            var view = from notice in new NoticesView(this.Reponsitory)
//                       where notice.NoticeType == NoticeType.Inbound && notice.Status != NoticeStatus.Closed
//                       orderby notice.CreateDate descending
//                       select new MyNotice
//                       {
//                           ID = notice.ID,
//                           ClientID = notice.ClientID,
//                           CompanyID = notice.CompanyID,
//                           ConsigneeID = notice.ConsigneeID,
//                           ConsignorID = notice.ConsignorID,
//                           FormID = notice.FormID,
//                           NoticeType = notice.NoticeType,
//                           Status = notice.Status,
//                           WarehouseID = notice.WarehouseID,
//                           WaybillID = notice.WaybillID,
//                           CreateDate = notice.CreateDate,
//                           ModifyDate = notice.ModifyDate,
//                           Summary = notice.Summary,
//                           Exception = notice.Exception,
//                       };

//            return view;
//        }

//        /// <summary>
//        /// 入库通知
//        /// </summary>
//        /// <param name="jobject"></param>
//        public void Notice(JObject jobject)
//        {
//            #region 保存更新NoticeTransports表
//            Func<JToken, Layers.Data.Sqls.PsWms.NoticeTransports> enterNoticeTransport = new Func<JToken, Layers.Data.Sqls.PsWms.NoticeTransports>((JToken jtoken) =>
//            {
//                return new Layers.Data.Sqls.PsWms.NoticeTransports
//                {
//                    TransportMode = jtoken["TransportMode"].Value<int>(),
//                    Carrier = jtoken["Carrier"].Value<string>(),
//                    WaybillCode = jtoken["WaybillCode"].Value<string>(),
//                    ExpressPayer = jtoken["ExpressPayer"].Value<int>(),
//                    ExpressTransport = jtoken["ExpressTransport"]?.Value<string>(), // 如果没有存什么值
//                    ExpressEscrow = jtoken["ExpressEscrow"]?.Value<string>(), //运费账户, 只在FreightPayer.ThirdParty时候起作用
//                    ExpressFreight = jtoken["ExpressFreight"].Value<decimal?>() ?? 0,
//                    TakingTime = jtoken["TakingTime"].Value<DateTime?>(),
//                    TakerName = jtoken["TakerName"].Value<string>(),
//                    TakerLicense = jtoken["TakerLicense"].Value<string>(),
//                    TakerPhone = jtoken["TakerPhone"].Value<string>(),
//                    TakerIDType = jtoken["TakerIDType"].Value<int?>(),
//                    TakerIDCode = jtoken["TakerIDCode"].Value<string>(),
//                    Address = jtoken["Address"].Value<string>(),
//                    Contact = jtoken["Contact"].Value<string>(),
//                    Phone = jtoken["Phone"].Value<string>(),
//                    Email = jtoken["Email"].Value<string>(),
//                    Summary = jtoken["Summary"].Value<string>(),
//                };
//            });
//            Func<Layers.Data.Sqls.PsWms.NoticeTransports, string> enterTransport = new Func<Layers.Data.Sqls.PsWms.NoticeTransports, string>((Layers.Data.Sqls.PsWms.NoticeTransports transports) =>
//            {
//                string id = transports.ID;

//                if (string.IsNullOrWhiteSpace(id))
//                {
//                    transports.ID = id = PKeySigner.Pick(PKeyType.NoticeTransport);
//                    transports.CreateDate = DateTime.Now;
//                    transports.ModifyDate = DateTime.Now;
//                    this.Reponsitory.Insert(transports);

//                }
//                else
//                {
//                    transports.ModifyDate = DateTime.Now;
//                    this.Reponsitory.Update<Layers.Data.Sqls.PsWms.NoticeTransports>(transports, item => item.ID == id);
//                }

//                return id;
//            });

//            #endregion

//            #region 保存更新Product
//            Func<JToken, string> enterProduct = new Func<JToken, string>((JToken product) =>
//            {
//                string partnumber = product["Partnumber"].Value<string>();
//                string brand = product["Brand"].Value<string>();
//                string package = product["Package"].Value<string>();
//                string dateCode = product["DateCode"].Value<string>();
//                int? mpq = product["Mpq"].Value<int?>();
//                int? moq = product["Moq"].Value<int?>();

//                var productsView = new ProductsView(this.Reponsitory);
//                var productID = productsView.SingleOrDefault(item => item.Partnumber == partnumber && item.Brand == brand && item.Package == package && item.DateCode == dateCode && item.Mpq == mpq && item.Moq == moq)?.ID;
//                if (string.IsNullOrEmpty(productID))
//                {

//                    productID = PKeySigner.Pick(PKeyType.Product);
//                    this.Reponsitory.Insert(new Layers.Data.Sqls.PsWms.Products
//                    {
//                        ID = productID,
//                        Partnumber = partnumber,
//                        Brand = brand,
//                        Package = package,
//                        DateCode = dateCode,
//                        Mpq = mpq,
//                        Moq = moq,
//                        CreateDate = DateTime.Now,
//                        ModifyDate = DateTime.Now,
//                    });
//                }

//                return productID;
//            });
//            #endregion

//            #region 保存更新通知
//            string noticeid = jobject["ID"]?.Value<string>();

//            var noticeItems = jobject["Items"];
//            var consignor = jobject["Consignor"];
//            var consignee = jobject["Consignee"];

//            if (consignee == null)
//            {
//                throw new ArgumentException("Consignee 相关参数不能为 null");
//            }

//            TransportMode transportMode = (TransportMode)consignee["TransportMode"].Value<int>();
//            string consigneeID = null;
//            string consignorID = null;

//            //增加判断逻辑
//            consigneeID = enterTransport(enterNoticeTransport(consignee));
//            consignorID = enterTransport(enterNoticeTransport(consignor));


//            var noticeFormID = jobject["FormID"].Value<string>();
//            if (string.IsNullOrWhiteSpace(noticeid))
//            {
//                noticeid = PKeySigner.Pick(PKeyType.Notice);

//                this.Reponsitory.Insert(new Layers.Data.Sqls.PsWms.Notices
//                {
//                    ID = noticeid,
//                    ClientID = jobject["ClientID"].Value<string>(),
//                    CompanyID = jobject["CompanyID"].Value<string>(),
//                    NoticeType = jobject["NoticeType"].Value<int>(),
//                    FormID = noticeFormID,
//                    Summary = jobject["Summary"].Value<string>(),
//                    WarehouseID = jobject["WarehouseID"].Value<string>(),
//                    CreateDate = DateTime.Now,
//                    ModifyDate = DateTime.Now,
//                    Status = transportMode == TransportMode.PickUp ? (int)NoticeStatus.Arranging : (int)NoticeStatus.Processing
//                });
//            }
//            else
//            {
//                this.Reponsitory.Update<Layers.Data.Sqls.PsWms.Notices>(new
//                {
//                    ClientID = jobject["ClientID"].Value<string>(),
//                    CompanyID = jobject["CompanyID"].Value<string>(),
//                    NoticeType = jobject["NoticeType"].Value<int>(),
//                    FormID = jobject["FormID"].Value<string>(),
//                    Summary = jobject["Summary"].Value<string>(),
//                    WarehouseID = jobject["WarehouseID"].Value<string>(),
//                    ModifyDate = DateTime.Now,
//                }, item => item.ID == noticeid);
//            }
//            #endregion

//            #region 保存更新NoticeItem

//            foreach (var noticeItem in noticeItems)
//            {
//                string id = noticeItem["ID"].Value<string>().Trim();

//                var product = noticeItem["Product"];
//                var productID = enterProduct(product);
//                var source = noticeItem["Source"]?.Value<int?>();
//                var inputID = noticeItem["InputID"].Value<string>().Trim();
//                var formID = noticeItem["FormID"].Value<string>();

//                if (string.IsNullOrEmpty(id))
//                {
//                    id = PKeySigner.Pick(PKeyType.NoticeItem);

//                    this.Reponsitory.Insert(new Layers.Data.Sqls.PsWms.NoticeItems
//                    {
//                        ID = id,
//                        NoticeID = noticeid,
//                        Source = source.HasValue ? source.Value : (int)NoticeSource.Tracker,
//                        ProductID = productID,
//                        InputID = string.IsNullOrEmpty(inputID) ? PKeySigner.Pick(PKeyType.Input) : inputID,
//                        CustomCode = noticeItem["CustomCode"].Value<string>(),
//                        StocktakingType = noticeItem["StocktakingType"].Value<int>(),
//                        Mpq = noticeItem["Mpq"].Value<int>(),
//                        PackageNumber = noticeItem["PackageNumber"].Value<int>(),
//                        Total = noticeItem["Total"].Value<int>(),
//                        Currency = noticeItem["Currency"].Value<int>(),
//                        UnitPrice = noticeItem["UnitPrice"].Value<decimal?>() ?? 0,
//                        Supplier = noticeItem["Supplier"]?.Value<string>(),
//                        ClientID = noticeItem["ClientID"]?.Value<string>(),
//                        FormID = string.IsNullOrEmpty(formID) ? noticeFormID : formID,
//                        FormItemID = noticeItem["FormItemID"]?.Value<string>(),
//                        CreateDate = DateTime.Now,
//                    });
//                }
//                else
//                {
//                    this.Reponsitory.Update<Layers.Data.Sqls.PsWms.NoticeItems>(new
//                    {
//                        NoticeID = noticeid,
//                        Source = source.HasValue ? source.Value : (int)NoticeSource.Tracker,
//                        ProductID = productID,
//                        InputID = string.IsNullOrEmpty(inputID) ? PKeySigner.Pick(PKeyType.Input) : inputID,
//                        CustomCode = noticeItem["CustomCode"].Value<string>(),
//                        StocktakingType = noticeItem["StocktakingType"].Value<int>(),
//                        Mpq = noticeItem["Mpq"].Value<int>(),
//                        PackageNumber = noticeItem["PackageNumber"].Value<int>(),
//                        Total = noticeItem["Total"].Value<int>(),
//                        Currency = noticeItem["Currency"].Value<int>(),
//                        UnitPrice = noticeItem["UnitPrice"].Value<decimal?>() ?? 0,
//                        Supplier = noticeItem["Supplier"]?.Value<string>(),
//                        ClientID = noticeItem["ClientID"]?.Value<string>(),
//                        FormID = string.IsNullOrEmpty(formID) ? noticeFormID : formID,
//                        FormItemID = noticeItem["FormItemID"]?.Value<string>(),
//                    }, item => item.ID == id);
//                }
//            }

//            #endregion
//        }

//        /// <summary>
//        /// 分拣入库
//        /// </summary>
//        public void Sorting(JObject jobject)
//        {
//            //notice
//        }
//    }

//    /// <summary>
//    /// 入库分拣视图
//    /// </summary>
//    public class NoticesInView : Linq.UniqueView<MyNotice, PsWmsRepository>
//    {
//        #region 构造函数
//        public NoticesInView()
//        {
//        }

//        protected NoticesInView(PsWmsRepository reponsitory) : base(reponsitory)
//        {
//        }

//        protected NoticesInView(PsWmsRepository reponsitory, IQueryable<MyNotice> iQueryable) : base(reponsitory, iQueryable)
//        {

//        }

//        #endregion

//        protected override IQueryable<MyNotice> GetIQueryable()
//        {
//            var view = from notice in new NoticesView(this.Reponsitory)
//                       where notice.NoticeType == NoticeType.Inbound && notice.Status != NoticeStatus.Closed
//                       orderby notice.CreateDate descending
//                       select new MyNotice
//                       {
//                           ID = notice.ID,
//                           ClientID = notice.ClientID,
//                           CompanyID = notice.CompanyID,
//                           ConsigneeID = notice.ConsigneeID,
//                           ConsignorID = notice.ConsignorID,
//                           FormID = notice.FormID,
//                           NoticeType = notice.NoticeType,
//                           Status = notice.Status,
//                           WarehouseID = notice.WarehouseID,
//                           WaybillID = notice.WaybillID,
//                           CreateDate = notice.CreateDate,
//                           ModifyDate = notice.ModifyDate,
//                           Summary = notice.Summary,
//                           Exception = notice.Exception,
//                       };

//            return view;
//        }

//        /// <summary>
//        /// 补全数据
//        /// </summary>
//        /// <returns></returns>
//        public object[] ToMyArray()
//        {
//            return this.ToMyPage() as object[];
//        }

//        /// <summary>
//        /// 获取单个的详情信息
//        /// </summary>
//        /// <returns></returns>
//        public object Single()
//        {
//            return (this.ToMyPage(1) as object[]).SingleOrDefault();
//        }

//        /// <summary>
//        /// 分页
//        /// </summary>
//        /// <param name="pageIndex"></param>
//        /// <param name="pageSize"></param>
//        /// <returns></returns>
//        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
//        {
//            var iquery = this.IQueryable.Cast<MyNotice>();
//            int total = iquery.Count();

//            if (pageIndex.HasValue && pageSize.HasValue)
//            {
//                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
//            }

//            var ienum_iquery = iquery.ToArray();

//            var noticeTransportsView = new NoticeTransportsView(this.Reponsitory);

//            if (pageIndex.HasValue && pageSize.HasValue)
//            {
//                #region 视图补全

//                var transportsID = ienum_iquery.Select(item => item.ConsigneeID);
//                var ienum_transports = noticeTransportsView.Where(item => transportsID.Contains(item.ID));

//                var clientsID = ienum_iquery.Select(item => item.ClientID);
//                var clientView = from client in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.ClientsTopView>()
//                                 where clientsID.Contains(client.ID)
//                                 select new
//                                 {
//                                     client.ID,
//                                     client.Name
//                                 };
//                var ienum_clients = clientView.ToArray();
//                var linq = from notice in ienum_iquery
//                           join client in ienum_clients on notice.ClientID equals client.ID
//                           join consignee in ienum_transports on notice.ConsigneeID equals consignee.ID into consignees
//                           select new
//                           {
//                               ClientName = client.Name, //客户
//                               notice.FormID, //订单ID
//                               notice.CreateDate, //通知时间
//                               consignees.Single().TransportMode, // 货运类型
//                               notice.Status,
//                               notice.TrackerID,
//                               notice.ID,
//                           };

//                #endregion

//                return new
//                {
//                    Total = total,
//                    Size = pageSize,
//                    Index = pageIndex,
//                    Data = linq.ToArray(),
//                };
//            }
//            else
//            {

//                #region 补充完整对象

//                //获取运输信息

//                var transportsID = ienum_iquery.Select(item => item.ConsigneeID).
//                    Concat(ienum_iquery.Select(item => item.ConsignorID));
//                var ienum_transports = noticeTransportsView.Where(item => transportsID.Contains(item.ID));
//                var clientsID = ienum_iquery.Select(item => item.ClientID);
//                var clientView = from client in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.ClientsTopView>()
//                                 where clientsID.Contains(client.ID)
//                                 select new
//                                 {
//                                     client.ID,
//                                     client.Name
//                                 };
//                var ienum_clients = clientView.ToArray();

//                var linq = from notice in ienum_iquery
//                           join client in ienum_clients on notice.ClientID equals client.ID
//                           join consignee in ienum_transports on notice.ConsigneeID equals consignee.ID into consignees
//                           join consignor in ienum_transports on notice.ConsignorID equals consignor.ID into consignors
//                           select new
//                           {
//                               ID = notice.ID,
//                               ClientID = notice.ClientID,
//                               CompanyID = notice.CompanyID,
//                               ConsigneeID = notice.ConsigneeID,
//                               ConsignorID = notice.ConsignorID,
//                               FormID = notice.FormID,
//                               NoticeType = notice.NoticeType,
//                               Status = notice.Status,
//                               WarehouseID = notice.WarehouseID,
//                               WaybillID = notice.WaybillID,
//                               CreateDate = notice.CreateDate,
//                               ModifyDate = notice.ModifyDate,
//                               Summary = notice.Summary,
//                               Exception = notice.Exception,
//                               Consignee = consignees.SingleOrDefault(),
//                               Consignor = consignors.SingleOrDefault(),
//                               ClientName = client.Name,
//                               TrackerID = notice.TrackerID,
//                           };

//                #endregion

//                var result = from notice in linq
//                             select new
//                             {
//                                 notice.ID,
//                                 notice.FormID, //订单号
//                                 notice.NoticeType,//通知类型
//                                 notice.Consignee.TransportMode, //运输方式
//                                 notice.ClientName, //客户名
//                                 notice.Consignee.Carrier, //承运商
//                                 notice.WaybillID,// 运单号  // 此处有疑问?????
//                                 notice.Consignee.Address, //提货地址
//                                 notice.Consignee.TakingTime, // 提货时间
//                                 notice.Consignee.TakerName,// 提货人
//                                 notice.Consignee.TakerLicense, // 车牌
//                                 notice.Consignee.TakerPhone, //提送货人电话
//                                 notice.Consignee.Phone, //提送货联系电话
//                                 notice.Summary,
//                                 notice.Exception,
//                                 notice.TrackerID,
//                             };

//                return result.ToArray();
//            }
//        }

//        #region 搜索方法
//        /// <summary>
//        /// 根据WarehouseID搜索
//        /// </summary>
//        /// <param name="whid"></param>
//        /// <returns></returns>
//        public NoticesInView SearchByWarehouseID(string whid)
//        {
//            var noticeInView = this.IQueryable.Cast<MyNotice>();
//            var linq = from notice in noticeInView
//                       where notice.WarehouseID == whid
//                       select notice;
//            var view = new NoticesInView(this.Reponsitory, linq);
//            return view;
//        }

//        /// <summary>
//        /// 根据Notice状态搜索
//        /// </summary>
//        /// <param name="status"></param>
//        /// <returns></returns>
//        public NoticesInView SearchByStatus(NoticeStatus status)
//        {
//            var noticeInView = this.IQueryable.Cast<MyNotice>();
//            var linq = from notice in noticeInView
//                       where notice.Status == status
//                       select notice;
//            var view = new NoticesInView(this.Reponsitory, linq);
//            return view;
//        }

//        /// <summary>
//        /// 根据型号搜索
//        /// </summary>
//        /// <param name="partnumber"></param>
//        /// <returns></returns>
//        public NoticesInView SearchByPartnumber(string partnumber)
//        {
//            var noticeView = this.IQueryable.Cast<MyNotice>();
//            var noticesIDView = from noticeItem in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.NoticeItems>()
//                                join product in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Products>() on noticeItem.ProductID equals product.ID
//                                where product.Partnumber.StartsWith(partnumber)
//                                select noticeItem.NoticeID;

//            var linq = from noticeID in noticesIDView.Distinct()
//                       join notice in noticeView on noticeID equals notice.ID
//                       orderby noticeID descending
//                       select notice;
//            //注意索引
//            var view = new NoticesInView(this.Reponsitory, linq);
//            return view;
//        }

//        /// <summary>
//        /// 根据订单ID搜索
//        /// </summary>
//        /// <param name="orderID"></param>
//        /// <returns></returns>
//        public NoticesInView SearchByOrderID(string orderID)
//        {
//            var noticeInView = this.IQueryable.Cast<MyNotice>();
//            var linq = from notice in noticeInView
//                       where notice.FormID == orderID
//                       select notice;
//            var view = new NoticesInView(this.Reponsitory, linq);
//            return view;
//        }

//        /// <summary>
//        /// 根据通知ID搜索
//        /// </summary>
//        /// <param name="noticeID"></param>
//        /// <returns></returns>
//        public NoticesInView SearchByNoticeID(string noticeID)
//        {
//            var noticeInView = this.IQueryable.Cast<MyNotice>();
//            var linq = from notice in noticeInView
//                       where notice.ID == noticeID
//                       select notice;
//            var view = new NoticesInView(this.Reponsitory, linq);
//            return view;
//        }

//        /// <summary>
//        /// 根据起止时间搜索
//        /// </summary>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <returns></returns>
//        public NoticesInView SearchByDate(DateTime? start, DateTime? end)
//        {
//            Expression<Func<MyNotice, bool>> predicate = notice => (start.HasValue ? notice.CreateDate >= start.Value : true)
//                && (end.HasValue ? notice.CreateDate < end.Value.AddDays(1) : true);

//            var noticeInView = this.IQueryable.Cast<MyNotice>();
//            var linq = noticeInView.Where(predicate);

//            var view = new NoticesInView(this.Reponsitory, linq);
//            return view;
//        }

//        /// <summary>
//        /// 搜索运输信息条件
//        /// </summary>
//        /// <param name="predicate">条件表达式</param>
//        /// <returns>入库基本视图</returns>
//        public NoticesInView SearchByTransport(Expression<Func<NoticeTransport, bool>> predicate)
//        {
//            var noticeTransportsView = new NoticeTransportsView(this.Reponsitory);

//            var linq = from transport in noticeTransportsView
//                       where transport.TransportMode == TransportMode.Express
//                       join notice in this.IQueryable on transport.ID equals notice.ConsigneeID
//                       select notice;

//            linq = from transport in noticeTransportsView.Where(predicate)
//                   where transport.TransportMode == TransportMode.Express
//                   join notice in this.IQueryable on transport.ID equals notice.ConsigneeID
//                   select notice;


//            var view = new NoticesInView(this.Reponsitory, linq);
//            return view;
//        }

//        #endregion

//        /// <summary>
//        /// 入库通知
//        /// </summary>
//        /// <param name="jobject"></param>
//        public void Notice(JObject jobject)
//        {
//            #region 保存更新NoticeTransports表
//            Func<JToken, Layers.Data.Sqls.PsWms.NoticeTransports> enterNoticeTransport = new Func<JToken, Layers.Data.Sqls.PsWms.NoticeTransports>((JToken jtoken) =>
//            {
//                return new Layers.Data.Sqls.PsWms.NoticeTransports
//                {
//                    TransportMode = jtoken["TransportMode"].Value<int>(),
//                    Carrier = jtoken["Carrier"].Value<string>(),
//                    WaybillCode = jtoken["WaybillCode"].Value<string>(),
//                    ExpressPayer = jtoken["ExpressPayer"].Value<int>(),
//                    ExpressTransport = jtoken["ExpressTransport"]?.Value<string>(), // 如果没有存什么值
//                    ExpressEscrow = jtoken["ExpressEscrow"]?.Value<string>(), //运费账户, 只在FreightPayer.ThirdParty时候起作用
//                    ExpressFreight = jtoken["ExpressFreight"].Value<decimal?>() ?? 0,
//                    TakingTime = jtoken["TakingTime"].Value<DateTime?>(),
//                    TakerName = jtoken["TakerName"].Value<string>(),
//                    TakerLicense = jtoken["TakerLicense"].Value<string>(),
//                    TakerPhone = jtoken["TakerPhone"].Value<string>(),
//                    TakerIDType = jtoken["TakerIDType"].Value<int?>(),
//                    TakerIDCode = jtoken["TakerIDCode"].Value<string>(),
//                    Address = jtoken["Address"].Value<string>(),
//                    Contact = jtoken["Contact"].Value<string>(),
//                    Phone = jtoken["Phone"].Value<string>(),
//                    Email = jtoken["Email"].Value<string>(),
//                    Summary = jtoken["Summary"].Value<string>(),
//                };
//            });
//            Func<Layers.Data.Sqls.PsWms.NoticeTransports, string> enterTransport = new Func<Layers.Data.Sqls.PsWms.NoticeTransports, string>((Layers.Data.Sqls.PsWms.NoticeTransports transports) =>
//            {
//                string id = transports.ID;

//                if (string.IsNullOrWhiteSpace(id))
//                {
//                    transports.ID = id = PKeySigner.Pick(PKeyType.NoticeTransport);
//                    transports.CreateDate = DateTime.Now;
//                    transports.ModifyDate = DateTime.Now;
//                    this.Reponsitory.Insert(transports);

//                }
//                else
//                {
//                    transports.ModifyDate = DateTime.Now;
//                    this.Reponsitory.Update<Layers.Data.Sqls.PsWms.NoticeTransports>(transports, item => item.ID == id);
//                }

//                return id;
//            });

//            #endregion

//            #region 保存更新Product
//            Func<JToken, string> enterProduct = new Func<JToken, string>((JToken product) =>
//            {
//                string partnumber = product["Partnumber"].Value<string>();
//                string brand = product["Brand"].Value<string>();
//                string package = product["Package"].Value<string>();
//                string dateCode = product["DateCode"].Value<string>();
//                int? mpq = product["Mpq"].Value<int?>();
//                int? moq = product["Moq"].Value<int?>();

//                var productsView = new ProductsView(this.Reponsitory);
//                var productID = productsView.SingleOrDefault(item => item.Partnumber == partnumber && item.Brand == brand && item.Package == package && item.DateCode == dateCode && item.Mpq == mpq && item.Moq == moq)?.ID;
//                if (string.IsNullOrEmpty(productID))
//                {

//                    productID = PKeySigner.Pick(PKeyType.Product);
//                    this.Reponsitory.Insert(new Layers.Data.Sqls.PsWms.Products
//                    {
//                        ID = productID,
//                        Partnumber = partnumber,
//                        Brand = brand,
//                        Package = package,
//                        DateCode = dateCode,
//                        Mpq = mpq,
//                        Moq = moq,
//                        CreateDate = DateTime.Now,
//                        ModifyDate = DateTime.Now,
//                    });
//                }

//                return productID;
//            });
//            #endregion

//            #region 保存更新通知
//            string noticeid = jobject["ID"]?.Value<string>();

//            var noticeItems = jobject["Items"];
//            var consignor = jobject["Consignor"];
//            var consignee = jobject["Consignee"];

//            if (consignee == null)
//            {
//                throw new ArgumentException("Consignee 相关参数不能为 null");
//            }

//            TransportMode transportMode = (TransportMode)consignee["TransportMode"].Value<int>();
//            string consigneeID = null;
//            string consignorID = null;

//            //增加判断逻辑
//            consigneeID = enterTransport(enterNoticeTransport(consignee));
//            consignorID = enterTransport(enterNoticeTransport(consignor));


//            var noticeFormID = jobject["FormID"].Value<string>();
//            if (string.IsNullOrWhiteSpace(noticeid))
//            {
//                noticeid = PKeySigner.Pick(PKeyType.Notice);

//                this.Reponsitory.Insert(new Layers.Data.Sqls.PsWms.Notices
//                {
//                    ID = noticeid,
//                    ClientID = jobject["ClientID"].Value<string>(),
//                    CompanyID = jobject["CompanyID"].Value<string>(),
//                    NoticeType = jobject["NoticeType"].Value<int>(),
//                    FormID = noticeFormID,
//                    Summary = jobject["Summary"].Value<string>(),
//                    WarehouseID = jobject["WarehouseID"].Value<string>(),
//                    CreateDate = DateTime.Now,
//                    ModifyDate = DateTime.Now,
//                    Status = transportMode == TransportMode.PickUp ? (int)NoticeStatus.Arranging : (int)NoticeStatus.Processing
//                });
//            }
//            else
//            {
//                this.Reponsitory.Update<Layers.Data.Sqls.PsWms.Notices>(new
//                {
//                    ClientID = jobject["ClientID"].Value<string>(),
//                    CompanyID = jobject["CompanyID"].Value<string>(),
//                    NoticeType = jobject["NoticeType"].Value<int>(),
//                    FormID = jobject["FormID"].Value<string>(),
//                    Summary = jobject["Summary"].Value<string>(),
//                    WarehouseID = jobject["WarehouseID"].Value<string>(),
//                    ModifyDate = DateTime.Now,
//                }, item => item.ID == noticeid);
//            }
//            #endregion

//            #region 保存更新NoticeItem

//            foreach (var noticeItem in noticeItems)
//            {
//                string id = noticeItem["ID"].Value<string>().Trim();

//                var product = noticeItem["Product"];
//                var productID = enterProduct(product);
//                var source = noticeItem["Source"]?.Value<int?>();
//                var inputID = noticeItem["InputID"].Value<string>().Trim();
//                var formID = noticeItem["FormID"].Value<string>();

//                if (string.IsNullOrEmpty(id))
//                {
//                    id = PKeySigner.Pick(PKeyType.NoticeItem);

//                    this.Reponsitory.Insert(new Layers.Data.Sqls.PsWms.NoticeItems
//                    {
//                        ID = id,
//                        NoticeID = noticeid,
//                        Source = source.HasValue ? source.Value : (int)NoticeSource.Tracker,
//                        ProductID = productID,
//                        InputID = string.IsNullOrEmpty(inputID) ? PKeySigner.Pick(PKeyType.Input) : inputID,
//                        CustomCode = noticeItem["CustomCode"].Value<string>(),
//                        StocktakingType = noticeItem["StocktakingType"].Value<int>(),
//                        Mpq = noticeItem["Mpq"].Value<int>(),
//                        PackageNumber = noticeItem["PackageNumber"].Value<int>(),
//                        Total = noticeItem["Total"].Value<int>(),
//                        Currency = noticeItem["Currency"].Value<int>(),
//                        UnitPrice = noticeItem["UnitPrice"].Value<decimal?>() ?? 0,
//                        Supplier = noticeItem["Supplier"]?.Value<string>(),
//                        ClientID = noticeItem["ClientID"]?.Value<string>(),
//                        FormID = string.IsNullOrEmpty(formID) ? noticeFormID : formID,
//                        FormItemID = noticeItem["FormItemID"]?.Value<string>(),
//                        CreateDate = DateTime.Now,
//                    });
//                }
//                else
//                {
//                    this.Reponsitory.Update<Layers.Data.Sqls.PsWms.NoticeItems>(new
//                    {
//                        NoticeID = noticeid,
//                        Source = source.HasValue ? source.Value : (int)NoticeSource.Tracker,
//                        ProductID = productID,
//                        InputID = string.IsNullOrEmpty(inputID) ? PKeySigner.Pick(PKeyType.Input) : inputID,
//                        CustomCode = noticeItem["CustomCode"].Value<string>(),
//                        StocktakingType = noticeItem["StocktakingType"].Value<int>(),
//                        Mpq = noticeItem["Mpq"].Value<int>(),
//                        PackageNumber = noticeItem["PackageNumber"].Value<int>(),
//                        Total = noticeItem["Total"].Value<int>(),
//                        Currency = noticeItem["Currency"].Value<int>(),
//                        UnitPrice = noticeItem["UnitPrice"].Value<decimal?>() ?? 0,
//                        Supplier = noticeItem["Supplier"]?.Value<string>(),
//                        ClientID = noticeItem["ClientID"]?.Value<string>(),
//                        FormID = string.IsNullOrEmpty(formID) ? noticeFormID : formID,
//                        FormItemID = noticeItem["FormItemID"]?.Value<string>(),
//                    }, item => item.ID == id);
//                }
//            }

//            #endregion
//        }

//        /// <summary>
//        /// 分拣入库
//        /// </summary>
//        public void Sorting(JObject jobject)
//        {
//            //notice
//        }

//        //单例
//    }

//    public class MyNotice : Linq.IUnique
//    {
//        #region 属性
//        /// <summary>
//        /// 唯一码
//        /// </summary>
//        public string ID { get; set; }

//        /// <summary>
//        /// 库房ID
//        /// </summary>
//        public string WarehouseID { get; set; }

//        /// <summary>
//        /// 所属客户
//        /// </summary>
//        public string ClientID { get; set; }

//        /// <summary>
//        /// 内部公司，所属公司
//        /// </summary>
//        public string CompanyID { get; set; }

//        /// <summary>
//        /// 通知类型: Inbound 入库 1, Outbound 出库 2, InAndOut 即入即出 3,
//        /// </summary>
//        public NoticeType NoticeType { get; set; }

//        /// <summary>
//        /// 来自的订单ID
//        /// </summary>
//        public string FormID { get; set; }

//        /// <summary>
//        /// 交货人信息ID
//        /// </summary>
//        public string ConsignorID { get; set; }

//        /// <summary>
//        /// 收货人信息ID
//        /// </summary>
//        public string ConsigneeID { get; set; }

//        /// <summary>
//        /// 创建时间
//        /// </summary>
//        public DateTime CreateDate { get; set; }

//        /// <summary>
//        /// 修改时间
//        /// </summary>
//        public DateTime ModifyDate { get; set; }

//        /// <summary>
//        /// 通知状态
//        /// </summary>
//        public NoticeStatus Status { get; set; }

//        /// <summary>
//        /// 运单ID
//        /// </summary>
//        public string WaybillID { get; set; }

//        /// <summary>
//        /// 异常备注
//        /// </summary>
//        public string Exception { get; set; }

//        /// <summary>
//        /// 跟单，客服
//        /// </summary>
//        public string TrackerID { get; set; }

//        /// <summary>
//        /// 信息备注
//        /// </summary>
//        public string Summary { get; set; }
//        #endregion
//    }
//}
