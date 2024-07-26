using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Interfaces;
using Needs.Ccs.Services.Views;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.SqlClient;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理订单  
    /// </summary>
    [Serializable]
    public class Order : IOrder, IPersist, IFulError, IFulSuccess
    {
        #region 属性

        private string id;

        public string ID
        {
            get
            {
                //主键编码规则：客户编号+日期+当天的序号 如：WL001720180128001
                return this.id ?? this.CreateOrderID();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 订单类型：内单、外单、Icgoo
        /// </summary>
        public Enums.OrderType Type { get; set; }

        /// <summary>
        /// 下单时的跟单员
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 平台用户
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 下单人
        /// </summary>
        private string orderMaker;
        public string OrderMaker
        {
            get
            {
                if (orderMaker == null)
                {
                    if (this.UserID == null)
                    {
                        this.OrderMaker = "跟单员";
                    }
                    else
                    {
                        using (var view = new Views.UsersView())
                        {
                            var user = view.Where(item => item.ID == this.UserID).SingleOrDefault();
                            this.OrderMaker = user.RealName;
                        }
                    }
                }
                return this.orderMaker;
            }
            set
            {
                this.orderMaker = value;
            }
        }

        /// <summary>
        /// 客户
        /// </summary>
        public Client Client { get; set; }

        public string ClientID { get; set; }

        /// <summary>
        /// 下单时的会员补充协议
        /// </summary>
        public ClientAgreement ClientAgreement { get; set; }

        public string ClientAgreementID { get; set; }

        /// <summary>
        /// 交易币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 报价时的海关税率
        /// </summary>
        public decimal? CustomsExchangeRate { get; set; }

        /// <summary>
        /// 报价时的实时汇率
        /// </summary>
        public decimal? RealExchangeRate { get; set; }

        /// <summary>
        /// 是否包车：Yes/No
        /// </summary>
        public bool IsFullVehicle { get; set; }

        /// <summary>
        /// 是否垫款：Yes/No
        /// </summary>
        public bool IsLoan { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public int? PackNo { get; set; }

        /// <summary>
        /// 包装种类
        /// </summary>
        public string WarpType { get; set; }

        /// <summary>
        /// 报关总货值（外币）
        /// </summary>
        private decimal declarePrice;
        public decimal DeclarePrice
        {
            get
            {
                if (this.declarePrice == 0)
                {
                    this.DeclarePrice = this.Items.Sum(item => item.TotalPrice);
                }
                return this.declarePrice;
            }
            set
            {
                this.declarePrice = value;
            }
        }

        /// <summary>
        /// 订单的开票状态
        /// </summary>
        public Enums.InvoiceStatus InvoiceStatus { get; set; }

        /// <summary>
        /// 订单的付汇状态
        /// </summary>
        public Enums.PayExchangeStatus PayExchangeStatus
        {
            get
            {
                if (PaidExchangeAmount == 0)
                    return Enums.PayExchangeStatus.UnPay;
                else if (PaidExchangeAmount < DeclarePrice)
                    return Enums.PayExchangeStatus.Partial;
                else
                    return Enums.PayExchangeStatus.All;
            }
        }

        /// <summary>
        /// 已申请付汇金额
        /// </summary>
        public decimal PaidExchangeAmount { get; set; }

        /// <summary>
        /// 是否挂起：Yes/No
        /// </summary>
        public bool IsHangUp { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public Enums.OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 状态：正常、删除
        /// </summary>
        public Enums.Status Status { get; set; }
        /// <summary>
        /// 代理费收取方式
        /// </summary>
        public Enums.OrderBillType OrderBillType { get; set; }
        /// <summary>
        /// 订单是否报关标记，跟单匹配到货信息用
        /// </summary>
        public Enums.DeclareFlagEnums DeclareFlag { get; set; }
        /// <summary>
        /// 入库通知状态，
        /// </summary>
        public Enums.EntryNoticeStatus EntryNoticeStatus { get; set; }
        /// <summary>
        /// 收款状态
        /// </summary>
        public Enums.CollectStatusEnums CollectStatus { get; set; }

        public decimal? CollectedAmount { get; set; }
        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        /// <summary>
        /// 订单的香港接货方式
        /// </summary>
        private OrderConsignee orderConsignee;

        /// <summary>
        /// 订单的香港接货方式
        /// </summary>
        public OrderConsignee OrderConsignee
        {
            get
            {
                if (orderConsignee == null)
                {
                    using (var view = new Views.OrderConsigneesView())
                    {
                        var query = view.Where(item => item.OrderID == this.ID);
                        this.OrderConsignee = query.SingleOrDefault();
                    }
                }

                return this.orderConsignee;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.orderConsignee = value;
            }
        }

        /// <summary>
        /// 订单的深圳交货方式
        /// </summary>
        private OrderConsignor orderConsignor;

        /// <summary>
        /// 订单的深圳交货方式
        /// </summary>
        public OrderConsignor OrderConsignor
        {
            get
            {
                if (orderConsignor == null)
                {
                    using (var view = new Views.OrderConsignorsView())
                    {
                        var query = view.Where(item => item.OrderID == this.ID);
                        this.OrderConsignor = query.SingleOrDefault();
                    }
                }

                return this.orderConsignor;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.orderConsignor = value;
            }
        }

        private OrderPayExchangeSuppliers payExchangeSuppliers;

        /// <summary>
        /// 付汇供应商
        /// </summary>
        public OrderPayExchangeSuppliers PayExchangeSuppliers
        {
            get
            {
                if (payExchangeSuppliers == null)
                {
                    using (var view = new Views.OrderPayExchangeSuppliersView())
                    {
                        var query = view.Where(item => item.OrderID == this.ID && item.Status == Enums.Status.Normal);
                        this.PayExchangeSuppliers = new OrderPayExchangeSuppliers(query);
                    }
                }
                return this.payExchangeSuppliers;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.payExchangeSuppliers = new OrderPayExchangeSuppliers(value, new Action<OrderPayExchangeSupplier>(delegate (OrderPayExchangeSupplier item)
                {
                    item.OrderID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 订单附件
        /// </summary>
        private OrderFiles files;

        /// <summary>
        /// 订单文件
        /// </summary>
        public OrderFiles Files
        {
            get
            {
                if (files == null)
                {
                    using (var view = new Views.OrderFilesView())
                    {
                        var query = view.Where(item => item.OrderID == this.ID);
                        this.Files = new OrderFiles(query);
                    }
                }
                return this.files;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.files = new OrderFiles(value, new Action<OrderFile>(delegate (OrderFile item)
                {
                    item.OrderID = this.ID;
                }));
            }
        }

        private MainOrderFiles mainOrderFiles { get; set; }
        /// <summary>
        /// 付汇委托书，对账单，PI
        /// </summary>
        public MainOrderFiles MainOrderFiles
        {
            get
            {
                if (mainOrderFiles == null)
                {
                    using (var view = new Views.CenterLinkXDTFilesTopView())
                    {
                        var query = view.Where(item => item.MainOrderID == this.MainOrderID);
                        this.MainOrderFiles = new MainOrderFiles(query);
                    }
                }
                return this.mainOrderFiles;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.mainOrderFiles = new MainOrderFiles(value, new Action<MainOrderFile>(delegate (MainOrderFile item)
                {
                    item.MainOrderID = this.MainOrderID;
                }));
            }
        }

        /// <summary>
        /// 订单日志
        /// </summary>
        private OrderLogs logs;

        /// <summary>
        /// 订单日志
        /// </summary>
        public OrderLogs Logs
        {
            get
            {
                if (logs == null)
                {
                    using (var view = new Views.OrderLogsView())
                    {
                        var query = view.Where(item => item.OrderID == this.ID);
                        this.Logs = new OrderLogs(query);
                    }
                }
                return this.logs;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.logs = new OrderLogs(value, new Action<OrderLog>(delegate (OrderLog item)
                {
                    item.OrderID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 订单项
        /// </summary>
        OrderItems items;
        public OrderItems Items
        {
            get
            {
                if (items == null)
                {
                    using (var view = new Views.OrderItemsView())
                    {
                        var query = view.Where(item => item.OrderID == this.ID && item.Status == Enums.Status.Normal);
                        this.Items = new OrderItems(query);
                    }
                }
                return this.items;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.items = new OrderItems(value, new Action<OrderItem>(delegate (OrderItem item)
                {
                    item.OrderID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 订单附加费用
        /// </summary>
        OrderPremium[] premiums;

        public OrderPremium[] Premiums
        {
            get
            {
                if (premiums == null)
                {
                    var view = new Views.OrderPremiumsView().GetOrderPremiums();
                    var query = view.Where(item => item.OrderID == this.ID && item.Status == Enums.Status.Normal);
                    this.Premiums = query.ToArray();

                }
                return this.premiums;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                this.premiums = value;

                //this.premiums = new OrderPremiums(value, new Action<OrderPremium>(delegate (OrderPremium item)
                //{
                //    item.OrderID = this.ID;
                //}));
            }
        }

        /// <summary>
        /// 订单代理费
        /// </summary>
        decimal agencyFee;
        public decimal AgencyFee
        {
            get
            {
                if (agencyFee == 0)
                {
                    this.AgencyFee = this.Premiums.Where(item => item.Type == Enums.OrderPremiumType.AgencyFee)
                    .Select(f => f.UnitPrice * f.Count * f.Rate).FirstOrDefault();
                }
                return agencyFee;
            }
            set
            {
                this.agencyFee = value;
            }
        }

        /// <summary>
        /// 订单杂费(不含商检费，跟单维护杂费时使用)
        /// </summary>
        OrderPremiums miscFees;
        public OrderPremiums MiscFees
        {
            get
            {
                if (miscFees == null)
                {
                    var query = this.Premiums.Where(item => item.Type != OrderPremiumType.AgencyFee && item.Type != OrderPremiumType.InspectionFee);
                    this.MiscFees = new OrderPremiums(query);
                }
                return miscFees;
            }
            set
            {
                this.miscFees = value;
            }
        }


        /// <summary>
        /// 杂费合计(不含商检费)
        /// </summary>
        public decimal OtherFee
        {
            get
            {
                return this.Premiums.Where(item => item.Type != OrderPremiumType.AgencyFee && item.Type != OrderPremiumType.InspectionFee)
                                    .Sum(item => item.Count * item.UnitPrice * item.Rate);
            }
        }

        /// <summary>
        /// 订单轨迹
        /// </summary>
        private OrderTraces traces;

        /// <summary>
        /// 订单轨迹
        /// </summary>
        public OrderTraces Traces
        {
            get
            {
                if (traces == null)
                {
                    using (var view = new Views.OrderTracesView())
                    {
                        var query = view.Where(item => item.OrderID == this.ID);
                        this.Traces = new OrderTraces(query);
                    }
                }
                return this.traces;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.traces = new OrderTraces(value, new Action<OrderTrace>(delegate (OrderTrace item)
                {
                    item.OrderID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 订单的国际快递
        /// </summary>
        private OrderWaybills waybills;
        public OrderWaybills Waybills
        {
            get
            {
                if (waybills == null)
                {
                    using (var view = new Views.HKClearanceData())
                    {
                        var query = view.Where(item => item.OrderID == this.ID);
                        this.Waybills = new OrderWaybills(query);
                    }
                }
                return this.waybills;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.waybills = new OrderWaybills(value, new Action<OrderWaybill>(delegate (OrderWaybill item)
                {
                    item.OrderID = this.ID;
                }));
            }
        }

        /// <summary>
        /// 订单退回原因
        /// </summary>
        public string ReturnedSummary { get; set; }

        /// <summary>
        /// 货款汇率(计算人民币货值时使用)
        /// </summary>
        private decimal productFeeExchangeRate;
        public decimal ProductFeeExchangeRate
        {
            get
            {
                var exchangeRateType = this.ClientAgreement.ProductFeeClause.ExchangeRateType;
                switch (exchangeRateType)
                {
                    case Enums.ExchangeRateType.RealTime:
                        productFeeExchangeRate = this.RealExchangeRate.GetValueOrDefault();
                        break;
                    case Enums.ExchangeRateType.Custom:
                        productFeeExchangeRate = this.CustomsExchangeRate.GetValueOrDefault();
                        break;
                    case Enums.ExchangeRateType.Agreed:
                        productFeeExchangeRate = this.ClientAgreement.ProductFeeClause.ExchangeRateValue.HasValue ? this.ClientAgreement.ProductFeeClause.ExchangeRateValue.Value : 0;
                        break;
                    default:
                        productFeeExchangeRate = 0;
                        break;
                }

                return this.productFeeExchangeRate;
            }
        }

        /// <summary>
        /// 代理费汇率
        /// </summary>
        private decimal agencyFeeExchangeRate;
        public decimal AgencyFeeExchangeRate
        {
            get
            {
                var exchangeRateType = this.ClientAgreement.AgencyFeeClause.ExchangeRateType;
                switch (exchangeRateType)
                {
                    case Enums.ExchangeRateType.RealTime:
                        agencyFeeExchangeRate = this.RealExchangeRate.GetValueOrDefault();
                        break;
                    case Enums.ExchangeRateType.Custom:
                        agencyFeeExchangeRate = this.CustomsExchangeRate.GetValueOrDefault();
                        break;
                    case Enums.ExchangeRateType.Agreed:
                        agencyFeeExchangeRate = this.ClientAgreement.AgencyFeeClause.ExchangeRateValue.HasValue ? this.ClientAgreement.AgencyFeeClause.ExchangeRateValue.Value : 0;
                        break;
                    default:
                        agencyFeeExchangeRate = 0;
                        break;
                }

                return this.agencyFeeExchangeRate;
            }
        }

        /// <summary>
        /// 归类产品（会员中心选择预归类产品快捷下单或通过接口下单时使用）
        /// </summary>
        public List<ClassifyProduct> ClassifyProducts { get; set; }

        /// <summary>
        /// 订单特殊类型
        /// </summary>
        public List<OrderVoyage> OrderVoyages { get; set; }

        public string MainOrderID { get; set; }

        /// <summary>
        /// 是否到货
        /// </summary>
        public bool IsArrived { get; set; }

        /// <summary>
        /// 指定代理费
        /// </summary>
        public decimal PointedAgencyFee
        {
            get
            {
                if (this.OrderBillType == OrderBillType.Pointed)
                {
                    var agency = new Views.Origins.OrderPremiumsOrigin().Where(t => t.OrderID == this.ID
                                                            && t.Type == OrderPremiumType.AgencyFee
                                                            && t.Status == Status.Normal).FirstOrDefault();
                    return agency?.UnitPrice ?? 0M;
                }

                return 0M;
            }
        }

        /// <summary>
        /// 入仓号
        /// </summary>
        public string EnterCode { get; set; }

        /// <summary>
        /// 垫资金额
        /// </summary>
        public decimal? AdvanceAmount { get; set; }

        #endregion

        #region 操作人

        internal Admin Admin;
        internal User User;
        /// <summary>
        /// 仅供接口下单时使用
        /// </summary>
        internal Admin APIAdmin;

        public void SetAdmin(Admin admin)
        {
            Admin = admin;
        }

        public void SetUser(User user)
        {
            User = user;
        }

        public void SetAPIAdmin(Admin apiAdmin)
        {
            APIAdmin = apiAdmin;
        }

        #endregion

        #region 事件

        public event ErrorHanlder EnterError;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 当生成对账单时发生
        /// </summary>
        public event GenerateOrderBillHanlder BillGenerated;

        /// <summary>
        /// 当订单挂起时发生
        /// </summary>
        public event OrderHangUpedHanlder HangUped;

        /// <summary>
        /// 当取消订单挂起时发生
        /// </summary>
        public event OrderHangUpCanceledHanlder HangUpCanceled;

        /// <summary>
        /// 当订单退回时发生
        /// </summary>
        public event OrderReturnedHanlder Returned;

        /// <summary>
        /// 订单退回，给客户端发送报文
        /// </summary>
        public event OrderReturnedHanlder Return2Client;
        #endregion

        public Order()
        {
            this.InvoiceStatus = Enums.InvoiceStatus.UnInvoiced;
            this.PaidExchangeAmount = 0;
            this.IsHangUp = false;
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.OrderBillType = Enums.OrderBillType.Normal;
            this.DeclareFlag = Enums.DeclareFlagEnums.Unable;

            this.EnterSuccess += Order_EnterSuccess;
            this.BillGenerated += OrderBill_Generated;
            this.HangUped += Order_HangUped;
            this.HangUpCanceled += Order_HangUpCanceled;
            this.Returned += Order_Returned;
            this.Return2Client += Order_Returned2Client;

        }

        /// <summary>
        /// 生成订单编号
        /// </summary>
        /// <returns></returns>
        protected string CreateOrderID()
        {
            //订单编号规则：客户编号+日期+当天的序号 如：WL001720180128001
            string orderIndex = "";
            string orderID = "";
            string mainOrderID = "";
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                if (this.MainOrderID == null)
                {
                    int orderCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>()
                                .Where(item => item.ClientID == this.Client.ID && SqlMethods.DateDiffDay(item.CreateDate, DateTime.Now) == 0).Count();
                    orderCount += 500;
                    orderIndex = (orderCount + 1).ToString();
                    orderIndex = orderIndex.PadLeft(3, '0');

                    mainOrderID = this.Client.ClientCode + DateTime.Now.ToString("yyyyMMdd") + orderIndex;
                }
                else
                {
                    mainOrderID = this.MainOrderID;
                }

                //bug修改：ryan 20220624
                //若先拆分 - 退回修改 - 再拆分会有错误，此时02订单的CreateTime 小于 01订单，ID错误
                //改为根据ID排序
                var mainRelatedOrder = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                                            .Where(item => item.MainOrderId == mainOrderID).OrderByDescending(item => item.ID).FirstOrDefault();
                if (mainRelatedOrder != null)
                {
                    string[] orderIDs = mainRelatedOrder.ID.Split('-');
                    orderID = mainOrderID + "-" + Convert.ToString(Convert.ToInt16(orderIDs[1]) + 1).PadLeft(3, '0');
                }
                else
                {
                    orderID = mainOrderID + "-01";
                }
            }

            this.MainOrderID = mainOrderID;
            this.ID = orderID;
            return this.ID;
        }

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //保存主订单
                int mainCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>().Count(item => item.ID == this.MainOrderID);
                if (mainCount == 0)
                {
                    MainOrder main = new MainOrder();
                    main.ID = this.MainOrderID;
                    main.ClientID = this.Client.ID;
                    main.AdminID = this.AdminID;
                    main.UserID = this.UserID;
                    main.Type = this.Type;
                    main.Enter();
                }

                //保存订单
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                    {
                        ID = this.ID,
                        Type = (int)this.Type,
                        AdminID = this.AdminID,
                        UserID = this.UserID,
                        ClientID = this.Client.ID,
                        ClientAgreementID = this.ClientAgreement.ID,
                        Currency = this.Currency,
                        CustomsExchangeRate = this.CustomsExchangeRate,
                        RealExchangeRate = this.RealExchangeRate,
                        IsFullVehicle = this.IsFullVehicle,
                        IsLoan = this.IsLoan,
                        PackNo = this.PackNo,
                        WarpType = this.WarpType,
                        DeclarePrice = this.DeclarePrice,
                        InvoiceStatus = (int)this.InvoiceStatus,
                        PaidExchangeAmount = this.PaidExchangeAmount,
                        IsHangUp = this.IsHangUp,
                        OrderStatus = (int)this.OrderStatus,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = DateTime.Now,
                        Summary = this.Summary,
                        OrderBillType = (int)this.OrderBillType,
                        DeclareFlag = (int)this.DeclareFlag
                    }, item => item.ID == this.ID);
                }

                //保存香港交货信息
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderConsignees>(item => item.OrderID == this.ID);
                reponsitory.Insert(this.OrderConsignee.ToLinq());

                //保存国内交货信息
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderConsignors>(item => item.OrderID == this.ID);
                reponsitory.Insert(this.OrderConsignor.ToLinq());

                //保存付汇供应商
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderPayExchangeSuppliers>(item => item.OrderID == this.ID);
                foreach (var payExchangeSupplier in this.PayExchangeSuppliers)
                {
                    reponsitory.Insert(payExchangeSupplier.ToLinq());
                }

                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                //保存订单项
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderItems>(item => item.OrderID == this.ID);

                #region 特殊化 ItemID动作，挪到外面进行
                //var prefix = System.Configuration.ConfigurationManager.AppSettings["Purchaser"];
                //foreach (var classifyProduct in this.ClassifyProducts)
                //{
                //    classifyProduct.ID = prefix + Needs.Overall.PKeySigner.Pick(PKeyType.OrderItem);
                //}
                #endregion

                //批量插入
                reponsitory.Insert(this.ClassifyProducts.Select(item => new Layer.Data.Sqls.ScCustoms.OrderItems
                {
                    ID = item.ID,
                    OrderID = item.OrderID,
                    Name = item.Name,
                    Model = item.Model,
                    Manufacturer = item.Manufacturer,
                    Batch = item.Batch,
                    Origin = item.Origin,
                    Quantity = item.Quantity,
                    DeclaredQuantity = item.DeclaredQuantity,
                    Unit = item.Unit,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    GrossWeight = item.GrossWeight,
                    IsSampllingCheck = item.IsSampllingCheck,
                    ClassifyStatus = (int)item.ClassifyStatus,
                    Status = (int)item.Status,
                    CreateDate = item.CreateDate,
                    UpdateDate = item.UpdateDate,
                    Summary = item.Summary,
                    ProductUniqueCode = item.ProductUniqueCode,
                }).ToArray());

                watch.Stop();
                TimeSpan span = watch.Elapsed;

                if (this.OrderStatus != OrderStatus.Draft)
                {
                    Task.Run(() =>
                    {
                        watch.Restart();

                        foreach (var classifyProduct in this.ClassifyProducts)
                        {
                            try
                            {
                                if (classifyProduct.ClassifyStatus == ClassifyStatus.Done)
                                {
                                    classifyProduct.DoPreClassify();
                                }
                                if (classifyProduct.ClassifyStatus == ClassifyStatus.Unclassified)
                                {
                                    var autoCategory = new AutoClassify(classifyProduct);
                                    autoCategory.DoClassify();
                                }
                            }
                            catch (Exception ex)
                            {
                                //记录异常日志
                                var log = new Models.ClassifyApiLogs()
                                {
                                    ClassifyProductID = classifyProduct.ID,
                                    ResponseContent = new { ex.Message, ex.StackTrace }.Json(),
                                    Summary = "产品归类 - 自动归类异常"
                                };
                                log.Enter();
                                continue;
                            }
                        }

                        //同步归类数据到中心数据和代仓储订单
                        var cps = this.ClassifyProducts.Where(cp => cp.ClassifyStatus == ClassifyStatus.Done).ToArray();
                        if (cps.Length > 0)
                            SyncManager.Current.Classify.For(cps).DoSync();

                        watch.Stop();
                        span = watch.Elapsed;
                    });
                }

                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderFiles>(item => item.OrderID == this.ID && item.FileType != (int)FileType.OrderFeeFile);
                foreach (var file in this.Files)
                {
                    file.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderFile);
                    reponsitory.Insert(file.ToLinq());
                }


                ////判断中心库和 老数据是否已存在，
                //var datafiles = new Needs.Ccs.Services.Views.CenterLinkXDTFilesTopView().Where(x => x.MainOrderID == this.MainOrderID && x.FileType != FileType.OrderFeeFile).ToArray();
                //string[] ids = datafiles.Select(a => a.ID.ToString()).ToArray();
                ////先删除 再插入
                //var mainIds = this.MainOrderFiles.Where(x => x.ID != null).Select(x => x.ID.ToString());
                //var deleteIds = ids.Except(mainIds).ToArray();
                //new CenterFilesTopView().Modify(new { Status = FileDescriptionStatus.Delete }, deleteIds);
                //foreach (var file in this.MainOrderFiles)
                //{
                //    //上传或者modify
                //    #region 订单文件保存到中心文件库
                //    var dic = new { CustomName = file.Name, WsOrderID = file.MainOrderID, AdminID = file.ErmAdminID };
                //    //本地文件上传到服务器
                //    if (file.ID == null)
                //    {
                //        var result = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(FileDirectory.Current.FilePath + @"\" + file.Url, file.Type, dic);
                //    }
                //    else
                //    {
                //        if (file.DataType == 1)
                //        {
                //            new CenterFilesTopView().Modify(new { ID = file.ID, WsOrderID = file.MainOrderID, Url = file.Url, AdminID = file.AdminID, CreateDate = file.CreateDate, Status = file.Status, Type = (int)file.FileType }, file.ID);
                //        }
                //    }
                //    #endregion
                //}

            }

            this.OnEnterSuccess();
        }

        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        ///记录订单Log,订单Trace 分开来记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Order_EnterSuccess(object sender, SuccessEventArgs e)
        {
            var order = (Order)e.Object;
            if (order.OrderStatus > Enums.OrderStatus.Draft)
            {
                // 写入日志
                if (order.Admin != null)
                {
                    order.Log(order.Admin, "跟单员[" + order.Admin.RealName + "]新增了订单，等待产品归类。");
                    order.Trace(order.Admin, OrderTraceStep.Submitted, "您提交了订单，系统确认归类中");
                }
                if (order.User != null)
                {
                    order.Log(order.User, "用户[" + order.User.RealName + "]新增了订单，等待产品归类。");
                    order.Trace(order.User, OrderTraceStep.Submitted, "您提交了订单，系统确认归类中");
                }
                if (order.APIAdmin != null)
                {
                    order.Log(order.APIAdmin, "[" + order.APIAdmin.RealName + "]通过接口下单");
                    order.Trace(order.APIAdmin, OrderTraceStep.Submitted, "您提交了订单，系统确认归类中");
                }

                CalcDueDate calcDueDate = new CalcDueDate(order.ID);
                calcDueDate.Execute();
            }
        }


        /// <summary>
        /// 已退回订单重新下单
        /// </summary>
        public void ReEnter()
        {
            #region 删除相关的订单数据

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //删除订单管控信息
                var orderControlIds = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>().Where(x => x.OrderID == this.ID).Select(x => x.ID).ToArray();
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderControlSteps>(ocs => orderControlIds.Contains(ocs.OrderControlID));
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderControls>(oc => orderControlIds.Contains(oc.ID));

                var orderItemIds = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Where(x => x.OrderID == this.ID).Select(x => x.ID).ToArray();
                //删除产品归类信息
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderItemCategories>(oic => orderItemIds.Contains(oic.OrderItemID));
                //删除产品税率信息
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>(oit => orderItemIds.Contains(oit.OrderItemID));
                //删除产品附件信息（3C、原产地证明）
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderFiles>(of => orderItemIds.Contains(of.OrderItemID));
                //删除产品附加费用（商检费）
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderPremiums>(op => orderItemIds.Contains(op.OrderItemID));
                //删除产品归类日志
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.ProductClassifyLogs>(ol => orderItemIds.Contains(ol.ClassifyProductID));

                //将订单货款、关税、增值税、代理费、商检费实收金额写回收款通知
                var receiveds = new Views.OrderReceivedsView().Where(item => item.OrderID == this.ID && item.FeeSourceID == null);
                var receiptNotices = receiveds.GroupBy(item => item.ReceiptNoticeID);
                receiptNotices.ToList().ForEach(item =>
                {
                    var clearAmount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ReceiptNotices>().First(rn => rn.ID == item.Key).ClearAmount;
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ReceiptNotices>(new { ClearAmount = clearAmount - item.Sum(r => r.Amount) }, rn => rn.ID == item.Key);
                });

                //删除货款、关税、增值税、代理费、商检费收款记录
                var receiptIds = new Views.OrderReceiptsAllsView().Where(item => item.OrderID == this.ID && item.FeeSourceID == null).Select(item => item.ID).ToArray();
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderReceipts>(or => receiptIds.Contains(or.ID));
            }

            #endregion

            this.Enter();
        }

        /// <summary>
        /// 生成对账单
        /// </summary>
        /// <param name="IsMinAgency">当传入true时，没有最低代理费 </param>
        public void GenerateBill(Enums.OrderBillType orderBillType = OrderBillType.Normal, decimal PointedAgencyFee = 0)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var order = this;

                //获取海关汇率和实时汇率
                decimal customsExchangeRate, realExchangeRate;
                if (order.CustomsExchangeRate != null && order.RealExchangeRate != null)
                {
                    customsExchangeRate = order.CustomsExchangeRate.Value;
                    realExchangeRate = order.RealExchangeRate.Value;
                }
                else
                {
                    if (order.Currency == MultiEnumUtils.ToCode<Enums.Currency>(Enums.Currency.CNY))   //  "CNY")
                    {
                        customsExchangeRate = 1;
                        realExchangeRate = 1;
                    }
                    else
                    {
                        var customsExchange = new Views.CustomExchangeRatesView(this.Currency).ToRate();
                        var realExchange = new Views.RealTimeExchangeRatesView(this.Currency).ToRate();
                        var nineRealExchange = new Views.NineRealTimeExchangeRatesView(this.Currency).ToRate();

                        if (customsExchange == null || string.IsNullOrEmpty(customsExchange.ID))
                        {
                            throw new Exception("请检查交易币种的海关汇率是否正确配置！");
                        }
                        if (realExchange == null || string.IsNullOrEmpty(realExchange.ID))
                        {
                            throw new Exception("请检查交易币种的实时汇率是否正确配置！");
                        }
                        if (nineRealExchange == null || string.IsNullOrEmpty(nineRealExchange.ID))
                        {
                            throw new Exception("请检查交易币种的九点半汇率是否正确配置！");
                        }

                        customsExchangeRate = customsExchange.Rate;
                        //2023-06-13 九点半付汇客户的账单实时汇率  使用九点半的
                        realExchangeRate = order.ClientAgreement.IsTen == PEIsTen.Ten ? realExchange.Rate : nineRealExchange.Rate;

                        //2023-06-28 税费使用实时汇率的客户,将海关汇率 赋值为 实时汇率
                        if (order.ClientAgreement.TaxFeeClause.ExchangeRateType == ExchangeRateType.RealTime)
                        {
                            customsExchangeRate = realExchangeRate;
                        }
                    }
                }

                //代理费汇率类型
                decimal agentExchangeRate = 0;
                switch (order.ClientAgreement.AgencyFeeClause.ExchangeRateType)
                {
                    case Enums.ExchangeRateType.RealTime:
                        agentExchangeRate = realExchangeRate;
                        break;
                    case Enums.ExchangeRateType.Custom:
                        agentExchangeRate = customsExchangeRate;
                        break;
                    case Enums.ExchangeRateType.Agreed:
                        agentExchangeRate = order.ClientAgreement.AgencyFeeClause.ExchangeRateValue.HasValue ? order.ClientAgreement.AgencyFeeClause.ExchangeRateValue.Value : 0;
                        break;
                    default:
                        break;
                }

                //计算代理费
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderPremiums>(op => op.OrderID == order.ID && op.Type == (int)Enums.OrderPremiumType.AgencyFee);

                decimal agencyRate = agentExchangeRate * order.ClientAgreement.AgencyRate;
                //重新计算订单总价格
                order.DeclarePrice = order.Items.Sum(item => item.TotalPrice);
                decimal orderAgencyFee = order.DeclarePrice * agencyRate;
                decimal minAgencyFee = order.ClientAgreement.MinAgencyFee;
                OrderPremium agency = new OrderPremium();
                agency.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderPremium);
                agency.OrderID = order.ID;
                agency.Type = Enums.OrderPremiumType.AgencyFee;
                agency.Admin = order.Client.Merchandiser;
                agency.Count = 1;
                agency.Currency = MultiEnumUtils.ToCode<Enums.Currency>(Enums.Currency.CNY);
                agency.Rate = 1;
                var preAgency = (order.ClientAgreement.PreAgency.HasValue && order.ClientAgreement.PreAgency > 0M ) ? order.ClientAgreement.PreAgency.Value : 0M;
                //增加基础收费20240606
                //orderAgencyFee += preAgency;
                switch (orderBillType)
                {
                    case Enums.OrderBillType.Normal:
                        agency.UnitPrice = orderAgencyFee < minAgencyFee ? minAgencyFee : (orderAgencyFee + preAgency).ToRound(4);
                        break;

                    case Enums.OrderBillType.MinAgencyFee:
                        agency.UnitPrice = orderAgencyFee.ToRound(4);
                        break;

                    case Enums.OrderBillType.Pointed:
                        agency.UnitPrice = PointedAgencyFee;
                        break;
                }

                reponsitory.Insert(agency.ToLinq());

                /*
                //如果后续允许分批报关，需要结合报关通知计算代理费，如分两次报关则收取两笔代理费
                //已申报货值的代理费
                var declareNotices = new Views.DeclarationNoticesView().Where(item => item.OrderID == order.ID && item.Status != DeclareNoticeStatus.Cancel).ToList();
                declareNotices.ForEach(notice =>
                {
                    var noticeAgency = notice.Items.Sum(item => item.Sorting.Quantity * item.Sorting.OrderItem.UnitPrice) * agencyRate;
                    agency.UnitPrice = noticeAgency < minAgencyFee ? minAgencyFee : noticeAgency.ToRound(4);
                    agency.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderPremium);
                    reponsitory.Insert(agency.ToLinq());
                });
                //未申报货值的代理费
                var totalAgency = order.DeclarePrice * agencyRate;
                var totalNoticeAgency = declareNotices.Sum(notice => notice.Items.Sum(item => item.Sorting.Quantity * item.Sorting.OrderItem.UnitPrice)) * agencyRate;
                var agencyBalance = totalAgency - totalNoticeAgency;
                if (agencyBalance > 0)
                {
                    agency.UnitPrice = agencyBalance < minAgencyFee ? minAgencyFee : agencyBalance.ToRound(4);
                    agency.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderPremium);
                    reponsitory.Insert(agency.ToLinq());
                }
                */

                /*
                //计算关税、增值税
                foreach (var item in order.Items)
                {
                    //2019-01-17 更新关税计算公式：Round(Round(Round(报关总价 * 运保杂, 2) * 海关汇率, 0) * 关税率, 2)
                    var topPrice = (item.TotalPrice * ConstConfig.TransPremiumInsurance).ToRound(2);
                    var total = (topPrice * customsExchangeRate).ToRound(0);
                    var importTaxValue = (total * item.ImportTax.Rate).ToRound(2);

                    item.ImportTax.Value = importTaxValue;
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>(new
                    {
                        Value = importTaxValue,
                        UpdateDate = DateTime.Now,
                    }, t => t.ID == item.ImportTax.ID);

                    //2019-01-17 更新增值税计算公式：Round((Round(Round(报关总价 * 运保杂, 2) * 海关汇率, 0) + 关税) * 增值税率, 2)
                    var addedValueTaxValue = ((total + importTaxValue) * item.AddedValueTax.Rate).ToRound(2);

                    item.AddedValueTax.Value = addedValueTaxValue;
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>(new
                    {
                        Value = addedValueTaxValue,
                        UpdateDate = DateTime.Now,
                    }, t => t.ID == item.AddedValueTax.ID);
                }
                */

                var entityList = new List<OrderItemTax>();

                int decimalFortotal = ConstConfig.CustomsValueOne;
                DateTime dt20210127 = Convert.ToDateTime("2021-01-27");
                if (this.CreateDate >= dt20210127)
                {
                    decimalFortotal = ConstConfig.CustomsValueTwo;
                }
                else
                {
                    var DecHead = new DecHeadsView().Where(item => item.OrderID == this.id).FirstOrDefault();
                    if (DecHead != null)
                    {
                        if (DecHead.isTwoStep)
                        {
                            decimalFortotal = ConstConfig.CustomsValueTwo;
                        }
                    }
                }


                //2023-09-28 更新
                //特殊客户单抬头开票增值税不包含运保杂计算，当前有两个客户webconfig
                var NoTransPremiumInsurance = System.Configuration.ConfigurationManager.AppSettings["NoTransPremiumInsurance"];
                var hasYBZ = !NoTransPremiumInsurance.Split(',').Contains(order.Client.ClientCode);

                //计算关税、增值税、消费税
                foreach (var item in order.Items)
                {
                    //2020-09-09 更新
                    //完税价格计算公式：Round(Round(报关总价 * 运保杂, 2) * 海关汇率, 0)
                    var topPrice = (item.TotalPrice * (hasYBZ ? ConstConfig.TransPremiumInsurance : 1)).ToRound(2);
                    var total = (topPrice * customsExchangeRate).ToRound(decimalFortotal);

                    if (item.ImportTax != null)
                    {
                        //关税计算公式：Round(完税价格 * 关税率, 2)
                        var importTaxValue = (total * item.ImportTax.Rate).ToRound(2);

                        item.ImportTax.Value = importTaxValue;
                        entityList.Add(new OrderItemTax
                        {
                            ID = item.ImportTax.ID,
                            Value = importTaxValue,
                            UpdateDate = DateTime.Now
                        });

                        if (item.ExciseTax != null)
                        {
                            //消费税计算公式：Round((完税价格＋关税)÷(1－消费税税率)×消费税税率, 2）
                            var exciseTaxValue = ((total + importTaxValue) / (1 - item.ExciseTax.Rate) * item.ExciseTax.Rate).ToRound(2);

                            item.ExciseTax.Value = exciseTaxValue;
                            entityList.Add(new OrderItemTax
                            {
                                ID = item.ExciseTax.ID,
                                Value = exciseTaxValue,
                                UpdateDate = DateTime.Now
                            });
                        }

                        if (item.AddedValueTax != null)
                        {
                            var exciseTaxRate = item.ExciseTax?.Rate ?? 0m;
                            //增值税计算公式：Round(((完税价 + 关税) + (完税价 + 关税) / (1-消费税税率) * 消费税税率) * 增值税率, 2)
                            var addedValueTaxValue = (((total + importTaxValue) + (total + importTaxValue) / (1 - exciseTaxRate) * exciseTaxRate) * item.AddedValueTax.Rate).ToRound(2);

                            item.AddedValueTax.Value = addedValueTaxValue;
                            entityList.Add(new OrderItemTax
                            {
                                ID = item.AddedValueTax.ID,
                                Value = addedValueTaxValue,
                                UpdateDate = DateTime.Now
                            });
                        }
                    }
                }

                var ids = order.Items.Select(item => item.ID).ToArray();
                Expression<Func<Layer.Data.Sqls.ScCustoms.OrderItemTaxes, bool>> whereLambda = oit => ids.Contains(oit.OrderItemID);
                Expression<Func<Layer.Data.Sqls.ScCustoms.OrderItemTaxes, string>> orderbyLambda = oit => oit.ID;
                var entities = entityList.OrderBy(oit => oit.ID).Select(oit => new { oit.Value, oit.UpdateDate }).ToArray();
                reponsitory.Update(whereLambda, orderbyLambda, entities);


                //更新报关总价、海关汇率和实时汇率
                order.CustomsExchangeRate = customsExchangeRate;
                order.RealExchangeRate = realExchangeRate;
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                {
                    DeclarePrice = order.DeclarePrice,
                    CustomsExchangeRate = customsExchangeRate,
                    RealExchangeRate = realExchangeRate
                }, o => o.ID == order.ID);

                //更新报关单中的海关汇率 20211104 ryan 特殊币种的海关汇率6小数位
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new
                {
                    CustomsExchangeRate = customsExchangeRate
                }, o => o.OrderID == order.ID);

                //此处调用一下 Yahv 的 dll ，使用 Task 执行, 并记录异常日志 Begin

                PaymentToYahv paymentToYahv = new PaymentToYahv(agency, orderBillType, order.ClientAgreement);
                paymentToYahv.Execute();

                //此处调用了一下 Yahv 的 dll ，其中用了 Task, 并会记录异常日志 End

            }

            this.OnOrderBillGenerated();
        }

        public void OnOrderBillGenerated()
        {
            if (this != null && this.BillGenerated != null)
            {
                this.BillGenerated(this, new GenerateOrderBillEventArgs(this));
            }
        }

        private void OrderBill_Generated(object sender, Hanlders.GenerateOrderBillEventArgs e)
        {
            if (e.Order.OrderStatus > OrderStatus.Quoted)
            {
                Task.Run(() =>
                {
                    e.Order.ToReceivables();
                });
            }
        }

        /// <summary>
        /// 将应收货款、应收关税、消费税、增值税、代理费、商检费写入订单收款(OrderReceipts)表
        /// </summary>
        public void ToReceivables()
        {
            var order = this;
            var taxPoint = 1 + order.ClientAgreement.InvoiceTaxRate;
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var tariffAmount = 0M;
                var exciseAmout = 0M;
                var avtAmount = 0M;
                reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Where(x => x.OrderID == order.ID).ToList()
                        .ForEach(x =>
                        {
                            tariffAmount += reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>().FirstOrDefault(tax => tax.OrderItemID == x.ID && tax.Type == (int)Enums.CustomsRateType.ImportTax).Value.GetValueOrDefault();
                            exciseAmout += reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>().FirstOrDefault(tax => tax.OrderItemID == x.ID && tax.Type == (int)Enums.CustomsRateType.ConsumeTax)?.Value ?? 0M;
                            avtAmount += reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>().FirstOrDefault(tax => tax.OrderItemID == x.ID && tax.Type == (int)Enums.CustomsRateType.AddedValueTax).Value.GetValueOrDefault();
                        });

                //ryan 20210113 外单税费小于50不收 钟苑平
                //订单应收中，小于50 不记录
                tariffAmount = tariffAmount < 50 ? 0M : tariffAmount.ToRound(2);
                exciseAmout = exciseAmout < 50 ? 0M : exciseAmout.ToRound(2);
                avtAmount = avtAmount < 50 ? 0M : avtAmount.ToRound(2);

                var productAmount = (order.DeclarePrice * order.ProductFeeExchangeRate).ToRound(2);
                var agencyAmount = (reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>()
                                              .Where(fee => fee.OrderID == order.ID && fee.Type == (int)Enums.OrderPremiumType.AgencyFee)
                                              .Sum(fee => fee.Count * fee.UnitPrice) * taxPoint).ToRound(2);
                var inspAmount = (reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>()
                                            .Where(fee => fee.OrderID == order.ID && fee.Type == (int)Enums.OrderPremiumType.InspectionFee)
                                            .Sum(fee => (decimal?)(fee.Count * fee.UnitPrice)) * taxPoint).GetValueOrDefault().ToRound(2);
                #region 应收货款
                var receivables = new Views.OrderReceivablesView(reponsitory).Where(item => item.OrderID == order.ID).ToList();
                var productReceivable = receivables.FirstOrDefault(item => item.FeeType == OrderFeeType.Product);
                if (productReceivable != null)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderReceipts>(new { Amount = productAmount }, x => x.ID == productReceivable.ID);
                }
                else
                {
                    productReceivable = new OrderReceivable(order, OrderFeeType.Product);
                    productReceivable.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderReceipt);
                    productReceivable.Amount = productAmount;
                    reponsitory.Insert(productReceivable.ToLinq());
                }
                #endregion

                #region 应收关税
                var tariffReceivable = receivables.FirstOrDefault(item => item.FeeType == OrderFeeType.Tariff);
                if (tariffReceivable != null)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderReceipts>(new { Amount = tariffAmount }, x => x.ID == tariffReceivable.ID);
                }
                else
                {
                    tariffReceivable = new OrderReceivable(order, OrderFeeType.Tariff);
                    tariffReceivable.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderReceipt);
                    tariffReceivable.Amount = tariffAmount;
                    reponsitory.Insert(tariffReceivable.ToLinq());
                }
                #endregion

                #region 应收消费税
                var exciseReceivable = receivables.FirstOrDefault(item => item.FeeType == OrderFeeType.ExciseTax);
                if (exciseReceivable != null)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderReceipts>(new { Amount = exciseAmout }, x => x.ID == exciseReceivable.ID);
                }
                else
                {
                    exciseReceivable = new OrderReceivable(order, OrderFeeType.ExciseTax);
                    exciseReceivable.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderReceipt);
                    exciseReceivable.Amount = exciseAmout;
                    reponsitory.Insert(exciseReceivable.ToLinq());
                }
                #endregion

                #region 应收增值税
                var avtReceivable = receivables.FirstOrDefault(item => item.FeeType == OrderFeeType.AddedValueTax);
                if (avtReceivable != null)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderReceipts>(new { Amount = avtAmount }, x => x.ID == avtReceivable.ID);
                }
                else
                {
                    avtReceivable = new OrderReceivable(order, OrderFeeType.AddedValueTax);
                    avtReceivable.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderReceipt);
                    avtReceivable.Amount = avtAmount;
                    reponsitory.Insert(avtReceivable.ToLinq());
                }
                #endregion

                #region 应收代理费
                var agencyReceivable = receivables.FirstOrDefault(item => item.FeeType == OrderFeeType.AgencyFee);
                if (agencyReceivable != null)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderReceipts>(new { Amount = agencyAmount }, x => x.ID == agencyReceivable.ID);
                }
                else
                {
                    agencyReceivable = new OrderReceivable(order, OrderFeeType.AgencyFee);
                    agencyReceivable.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderReceipt);
                    agencyReceivable.Amount = agencyAmount;
                    reponsitory.Insert(agencyReceivable.ToLinq());
                }
                #endregion

                #region 应收商检费
                if (inspAmount > 0)
                {
                    var inspReceivable = receivables.FirstOrDefault(item => item.FeeType == OrderFeeType.Incidental && item.FeeSourceID == null);
                    if (inspReceivable != null)
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderReceipts>(new { Amount = inspAmount }, x => x.ID == inspReceivable.ID);
                    }
                    else
                    {
                        inspReceivable = new OrderReceivable(order, OrderFeeType.Incidental);
                        inspReceivable.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderReceipt);
                        inspReceivable.Amount = inspAmount;
                        reponsitory.Insert(inspReceivable.ToLinq());
                    }
                }
                else
                {
                    var inspReceivable = receivables.FirstOrDefault(item => item.FeeType == OrderFeeType.Incidental && item.FeeSourceID == null);
                    if (inspReceivable != null)
                    {
                        reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderReceipts>(x => x.ID == inspReceivable.ID);
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// 订单挂起
        /// </summary>
        /// <param name="controlType">管控类型</param>
        /// <param name="summary">管控原因</param>
        public void HangUp(Enums.OrderControlType controlType, string summary = null)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { IsHangUp = true }, item => item.ID == this.ID);
            }

            this.OnHangUped(controlType, summary);
        }

        virtual protected void OnHangUped(Enums.OrderControlType controlType, string summary)
        {
            if (this != null && this.HangUped != null)
            {
                this.HangUped(this, new OrderHangUpedEventArgs(this.ID, controlType, summary));
            }
        }

        private void Order_HangUped(object sender, Hanlders.OrderHangUpedEventArgs e)
        {
            this.GenerateControl(e.OrderID, e.OrderControlType, e.Summary);
            this.SendNotice(e.OrderID, e.OrderControlType, e.Summary);
        }

        /// <summary>
        /// 生成订单管控
        /// </summary>
        /// <param name="type">管控类型</param>
        /// <param name="summary">管控原因</param>
        private void GenerateControl(string orderID, Enums.OrderControlType type, string summary)
        {
            //生成订单管控
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>()
                    .Where(oc => oc.OrderID == orderID
                              && oc.ControlType == (int)type
                              && oc.Status == (int)Enums.Status.Normal).Count();
                if (count > 0)
                    return;

                var control = new OrderControlData();
                control.OrderID = orderID;
                control.ControlType = type;
                control.Summary = summary;
                reponsitory.Insert(control.ToLinq());

                var controlStep = new OrderControlStep();
                controlStep.OrderControlID = control.ID;
                controlStep.Step = type == OrderControlType.ExceedLimit ? Enums.OrderControlStep.Headquarters : Enums.OrderControlStep.Merchandiser;
                reponsitory.Insert(controlStep.ToLinq());
            }
        }

        /// <summary>
        /// 订单挂起，发送通知
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="type"></param>
        /// <param name="summary"></param>
        private void SendNotice(string orderID, Enums.OrderControlType type, string summary)
        {
            //归类异常,分拣异常,抽检异常 这些不用了
            //产地变更 库房
            //删除型号，修改数量 这些是要通知客户
            NoticeLog log = new NoticeLog();
            log.MainID = orderID;

            switch (type)
            {
                //系统自动算出超出垫款上限
                case Enums.OrderControlType.ExceedLimit:
                    log.NoticeType = SendNoticeType.ExceedLimit;
                    log.SendNotice();
                    break;

                //系统自动归类为3C
                case Enums.OrderControlType.CCC:
                    log.NoticeType = SendNoticeType.HQCCC;
                    log.SendNotice();
                    break;

                //系统自动归类为禁运
                case Enums.OrderControlType.Forbid:
                    log.NoticeType = SendNoticeType.Forbid;
                    log.SendNotice();
                    break;

                case Enums.OrderControlType.OriginCertificate:
                    log.NoticeType = SendNoticeType.OriginCertificate;
                    log.SendNotice();
                    break;

                case Enums.OrderControlType.OriginChange:
                    log.NoticeType = SendNoticeType.OriginChange;
                    log.SendNotice();
                    break;
            }
        }

        /// <summary>
        /// 取消订单挂起
        /// </summary>
        public void CancelHangUp()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { IsHangUp = false }, item => item.ID == this.ID);
            }

            this.OnHangUpCanceled();
        }

        virtual protected void OnHangUpCanceled()
        {
            if (this != null && this.HangUpCanceled != null)
            {
                this.HangUpCanceled(this, new OrderHangUpCanceledEventArgs(this));
            }
        }

        private void Order_HangUpCanceled(object sender, Hanlders.OrderHangUpCanceledEventArgs e)
        {
            //写入日志
            e.Order.Log(e.Order.Admin, "跟单员[" + e.Order.Admin.RealName + "]取消了订单挂起");
        }

        /// <summary>
        /// 订单退回
        /// </summary>
        public void Return()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.OrderStatus = Enums.OrderStatus.Returned;
                this.IsHangUp = false;
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                {
                    OrderStatus = Enums.OrderStatus.Returned,
                    IsHangUp = false
                }, item => item.ID == this.ID);
                //2020-08-03 LK 修改
                //小订单退回，这个小订单的订单变更全删除               
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderChangeNotices>(new
                {
                    Status = (int)Enums.Status.Delete
                }, item => item.OderID == this.ID);

                //2020-07-08 LK 修改
                //如果这个大订单下的所有小订单都被退回了，这个大订单的状态置为400，要不然这个订单会一直挂在 对账单待审核 和 代理报关委托书待审核页面
                bool changeMainOrderStatus = true;
                var Orders = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Where(t => t.MainOrderId == this.MainOrderID).ToList();
                foreach (var singleOrder in Orders)
                {
                    if (singleOrder.OrderStatus != (int)Enums.OrderStatus.Returned || singleOrder.OrderStatus != (int)Enums.OrderStatus.Canceled)
                    {
                        changeMainOrderStatus = false;
                    }
                }
                if (changeMainOrderStatus)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.MainOrders>(new
                    {
                        Status = (int)Enums.Status.Delete
                    }, item => item.ID == this.MainOrderID);
                }

                //退回，保存按钮根据付汇申请的状态判断，如果付汇申请状态是大于已审核的状态，则返回的时候需要插入一笔预付汇记录
                //by yeshuangshuang  2020-11-11
                if (this.ID != "")
                {
                    var payExchangeApplyItems = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>().Where(item => item.OrderID == this.ID && item.Status == (int)Enums.Status.Normal).ToList();
                    if (payExchangeApplyItems != null)
                    {
                        foreach (var items in payExchangeApplyItems)
                        {
                            var payExchangeApplies = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>().FirstOrDefault(item => item.ID == items.PayExchangeApplyID && item.Status == (int)Enums.Status.Normal);
                            if (payExchangeApplies.PayExchangeApplyStatus != (int)Enums.PayExchangeApplyStatus.Auditing && payExchangeApplies.PayExchangeApplyStatus != (int)Enums.PayExchangeApplyStatus.Cancled)
                            {
                                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PrePayExchangeApplies>().Count(item => item.ClientID == this.Client.ID && item.PayExchangeApplyID == payExchangeApplies.ID && item.OrderID == this.ID);
                                if (count == 0)
                                {
                                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.PrePayExchangeApplies
                                    {
                                        ID = Guid.NewGuid().ToString("N"),
                                        ClientID = this.Client.ID,
                                        PayExchangeApplyID = payExchangeApplies.ID,
                                        ExchangeRate = payExchangeApplies.ExchangeRate,
                                        PayExchangeApplyStatus = payExchangeApplies.PayExchangeApplyStatus,
                                        Amount = items.Amount,//this.PaidExchangeAmount,
                                        Status = (int)Enums.Status.Normal,
                                        CreateDate = this.CreateDate,
                                        UpdateDate = DateTime.Now,
                                        Summary = this.Summary,
                                        OrderID = this.ID,
                                    });

                                    //reponsitory.Insert<Layer.Data.Sqls.ScCustoms.PrePayExchangeAppliesLogs>(new Layer.Data.Sqls.ScCustoms.PrePayExchangeAppliesLogs
                                    //{
                                    //    ID = Guid.NewGuid().ToString("N"),
                                    //    PayExchangeApplyItemID = payExchangeApplyItems.ID,
                                    //    OrderID = this.ID,
                                    //    AdminID = null,
                                    //    UserID = UserID,
                                    //    CreateDate = DateTime.Now,
                                    //    Summary = "用户[" + RealName + "]生成了预付汇记录",
                                    //});
                                }
                                //else
                                //{
                                //    reponsitory.Update<Layer.Data.Sqls.ScCustoms.PrePaymentApplyItems>(new
                                //    {
                                //        Amount = this.PaidExchangeAmount,
                                //        Summary = this.Summary,
                                //    }, item => item.ClientID == this.Client.ID && item.PayExchangeApplyID == payExchangeApplyItems.PayExchangeApplyID);
                                //}
                            }
                        }
                    }
                }
            }

            this.OnReturned();
        }

        virtual protected void OnReturned()
        {
            //订单退回，给客户端发送报文
            if (this != null && this.Return2Client != null)
            {
                this.Return2Client(this, new OrderReturnedEventArgs(this));
            }
            if (this != null && this.Returned != null)
            {
                this.Returned(this, new OrderReturnedEventArgs(this));
            }
        }

        private void Order_Returned(object sender, OrderReturnedEventArgs e)
        {
            //写入日志
            e.Order.Log(e.Order.Admin, "跟单员[" + e.Order.Admin.RealName + "]退回了订单,订单退回原因：" + e.Order.ReturnedSummary);
            e.Order.Trace(Admin, OrderTraceStep.Anomaly, "您的订单已退回，退回原因：" + e.Order.ReturnedSummary);

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {

                #region 删除报关单数据

                reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(x => x.OrderID == e.Order.ID).ToList()
                .ForEach(x =>
                {
                    reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ManifestConsignments>().Where(c => c.ID == x.BillNo).ToList()
                    .ForEach(m =>
                    {
                        //删除舱单轨迹
                        reponsitory.Delete<Layer.Data.Sqls.ScCustoms.ManifestConsignmentTraces>(mct => mct.ManifestConsignmentID == m.ID);
                        //删除提运单集装箱
                        reponsitory.Delete<Layer.Data.Sqls.ScCustoms.ManifestConsignmentContainers>(mcc => mcc.ManifestConsignmentID == m.ID);
                        //删除提运单表体
                        reponsitory.Delete<Layer.Data.Sqls.ScCustoms.ManifestConsignmentItems>(mci => mci.ManifestConsignmentID == m.ID);
                        //删除提运单
                        reponsitory.Delete<Layer.Data.Sqls.ScCustoms.ManifestConsignments>(mc => mc.ID == m.ID);
                    });

                    //删除报关单缴税流水
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DecTaxFlows>(dtf => dtf.DecTaxID == x.ID);
                    //删除报关单缴税
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DecTaxs>(dt => dt.ID == x.ID);

                    //删除报关单集装箱
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DecContainers>(dc => dc.DeclarationID == x.ID);
                    //删除报关单附件
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DecHeadFiles>(df => df.DecHeadID == x.ID);
                    //删除报关单随附单证
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DecLicenseDocus>(dl => dl.DeclarationID == x.ID);
                    //删除报关单其它包装
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DecOtherPacks>(dop => dop.DeclarationID == x.ID);
                    //删除报关单申请单证信息
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DecRequestCerts>(drc => drc.DeclarationID == x.ID);
                    //删除报关单轨迹
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DecTraces>(dt => dt.DeclarationID == x.ID);
                    //删除报关单电子单据
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.EdocRealations>(er => er.DeclarationID == x.ID);
                    //删除报关单表体
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DecLists>(dl => dl.DeclarationID == x.ID);
                    //删除报关单
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DecHeads>(dh => dh.ID == x.ID);
                    //删除订单特殊类型
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderVoyages>(ov => ov.OrderID == x.OrderID);
                });

                //删除报关通知
                reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNotices>().Where(x => x.OrderID == e.Order.ID).ToList()
                .ForEach(x =>
                {
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DeclarationNoticeLogs>(dnl => dnl.DeclarationNoticeID == x.ID);
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DeclarationNoticeItems>(dni => dni.DeclarationNoticeID == x.ID);
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DeclarationNotices>(dn => dn.ID == x.ID);
                });

                #endregion

                #region 删除香港库房产生的数据

                //删除装箱数据
                reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Packings>().Where(x => x.OrderID == e.Order.ID).ToList()
                    .ForEach(x =>
                    {
                        reponsitory.Delete<Layer.Data.Sqls.ScCustoms.PackingItems>(pi => pi.PackingID == x.ID);
                        reponsitory.Delete<Layer.Data.Sqls.ScCustoms.Packings>(p => p.ID == x.ID);
                    });
                //删除香港清关数据
                reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWaybills>().Where(x => x.OrderID == e.Order.ID).ToList()
                    .ForEach(x =>
                    {
                        reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderWaybillItems>(wi => wi.OrderWaybillID == x.ID);
                        reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderWaybills>(w => w.ID == x.ID);
                    });
                //删除分拣数据
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.Sortings>(s => s.OrderID == e.Order.ID);
                ////删除提货通知数据
                //if (this.OrderConsignee.Type == Enums.HKDeliveryType.PickUp)
                //{
                //    reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeliveryNotices>().Where(x => x.OrderID == e.Order.ID).ToList()
                //    .ForEach(x =>
                //    {
                //        reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DeliveryNoticeLogs>(dnl => dnl.DeliveryNoticeID == x.ID);
                //        reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DeliveryConsignees>(dc => dc.DeliveryNoticeID == x.ID);
                //        reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DeliveryNotices>(dn => dn.ID == x.ID);
                //    });
                //}
                //删除入库通知
                reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>().Where(x => x.OrderID == e.Order.ID).ToList()
                .ForEach(x =>
                {
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.EntryNoticeLogs>(enl => enl.EntryNoticeID == x.ID);
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>(eni => eni.EntryNoticeID == x.ID);
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.EntryNotices>(en => en.ID == x.ID);
                });

                #endregion
            }
        }

        /// <summary>
        /// 生成香港仓库的入库通知/提货通知，
        /// </summary>
        public void ToEntryNotice()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                #region 删除入库通知、提货通知，以防脏数据

                //删除入库通知
                reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNotices>().Where(x => x.OrderID == this.ID).ToList()
                .ForEach(x =>
                {
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.EntryNoticeLogs>(enl => enl.EntryNoticeID == x.ID);
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>(eni => eni.EntryNoticeID == x.ID);
                    reponsitory.Delete<Layer.Data.Sqls.ScCustoms.EntryNotices>(en => en.ID == x.ID);
                });

                //删除提货通知数据
                if (this.OrderConsignee.Type == Enums.HKDeliveryType.PickUp)
                {
                    reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeliveryNotices>().Where(x => x.OrderID == this.ID).ToList()
                    .ForEach(x =>
                    {
                        reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DeliveryNoticeLogs>(dnl => dnl.DeliveryNoticeID == x.ID);
                        reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DeliveryConsignees>(dc => dc.DeliveryNoticeID == x.ID);
                        reponsitory.Delete<Layer.Data.Sqls.ScCustoms.DeliveryNotices>(dn => dn.ID == x.ID);
                    });
                }

                #endregion

                #region 生成香港入库通知、提货通知

                //生成入库通知
                var entrynotice = new Models.EntryNotice();
                entrynotice.Order = this;
                entrynotice.ClientCode = this.Client.ClientCode;
                entrynotice.WarehouseType = Enums.WarehouseType.HongKong;
                //五级客户需要拆箱分拣
                entrynotice.SortingRequire = this.Client.ClientRank == Enums.ClientRank.ClassFive ? Enums.SortingRequire.UnPacking : Enums.SortingRequire.Packed;
                entrynotice.Enter();
                foreach (var item in this.Items)
                {
                    var eitem = new EntryNoticeItem();
                    eitem.EntryNoticeID = entrynotice.ID;
                    eitem.OrderItem = item;
                    eitem.IsSportCheck = item.IsSampllingCheck;
                    eitem.Enter();
                }

                //HK提货通知
                if (this.OrderConsignee.Type == Enums.HKDeliveryType.PickUp)
                {
                    var deliverynotice = new DeliveryNotice();
                    deliverynotice.Order = this;
                    deliverynotice.Admin = this.Client.Merchandiser;
                    deliverynotice.Enter();
                    var deliveryConsignees = new DeliveryConsignee();
                    deliveryConsignees.DeliveryNoticeID = deliverynotice.ID;
                    deliveryConsignees.Supplier = this.OrderConsignee.ClientSupplier.ChineseName;
                    deliveryConsignees.PickUpDate = this.OrderConsignee.PickUpTime.Value;
                    deliveryConsignees.Address = this.OrderConsignee.Address;
                    deliveryConsignees.Contact = this.OrderConsignee.Contact;
                    deliveryConsignees.Tel = this.OrderConsignee.Mobile;
                    deliveryConsignees.Enter();
                }

                #endregion
            }
        }

        public void UpdateOrderStatus()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { OrderStatus = (int)this.OrderStatus, }, item => item.ID == this.ID);
            }
        }

        private void Order_Returned2Client(object sender, OrderReturnedEventArgs e)
        {
            try
            {
                var apisetting = new Needs.Ccs.Services.ApiSettings.PvWsOrderApiSetting();
                string address = apisetting.OrderReturn;

                var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + address;

                var ermAdminID = new Needs.Ccs.Services.Views.AdminsTopView2().FirstOrDefault(x => x.OriginID == this.AdminID)?.ID;

                var PostData = new
                {
                    VastOrderID = this.MainOrderID,
                    TinyOrderID = this.ID,
                    AdminID = ermAdminID
                };

                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    OrderID = this.MainOrderID,
                    TinyOrderID = this.ID,
                    Url = apiurl,
                    RequestContent = PostData.Json(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                };
                apiLog.Enter();


                var result = Needs.Utils.Http.ApiHelper.Current.JPost(apiurl, PostData);
                apiLog.ResponseContent = result;
                apiLog.Enter();
            }
            catch (Exception ex)
            {
                ex.CcsLog("订单退回调用客户端接口失败");
            }
        }

        public void UpdateEnterCode(string enterCode)
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { EnterCode = enterCode, UpdateDate = DateTime.Now }, item => item.ID == this.ID);
                }

                var apisetting = new Needs.Ccs.Services.ApiSettings.PvWsOrderApiSetting();
                string address = apisetting.UpdateEnterCode;

                var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + address;

                var PostData = new
                {
                    VastOrderID = this.MainOrderID,
                    EnterCode = enterCode
                };

                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    OrderID = this.MainOrderID,
                    TinyOrderID = this.ID,
                    Url = apiurl,
                    RequestContent = PostData.Json(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "更改EnterCode"
                };
                apiLog.Enter();

                var result = Needs.Utils.Http.ApiHelper.Current.JPost(apiurl, PostData);
                apiLog.ResponseContent = result;
                apiLog.Enter();
            }
            catch (Exception ex)
            {
                ex.CcsLog("更改EnterCode调用客户端接口失败");
            }
        }

    }
}
