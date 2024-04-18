using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Linq.Extends;
using Yahv.Linq.Generic;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.Underly;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    public class PayExchangeOrderView : UniqueView<UnPayExchangeOrder, ScCustomReponsitory>
    {
        IUser user;

        private PayExchangeOrderView()
        {

        }

        public PayExchangeOrderView(IUser User)
        {
            this.user = User;
        }

        protected override IQueryable<UnPayExchangeOrder> GetIQueryable()
        {
            var orders = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Orders>()
                .Where(item => item.Status == (int)GeneralStatus.Normal && Math.Round(item.PaidExchangeAmount, 2,MidpointRounding.AwayFromZero) < Math.Round(item.DeclarePrice,2,MidpointRounding.AwayFromZero))
                .Where(item => item.OrderStatus >= (int)OrderStatus.QuoteConfirmed && item.OrderStatus <= (int)OrderStatus.Completed);
            var decheads = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecHeads>().Where(item => item.IsSuccess == true);
            if (this.user.IsMain)
            {
                orders = orders.Where(item => item.ClientID == this.user.XDTClientID);
            }
            else
            {
                orders = orders.Where(item => item.UserID == this.user.ID);
            }

            return from order in orders
                   join payexchangesupplier in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.OrderPayExchangeSuppliersView>()
                   on order.ID equals payexchangesupplier.OrderID into suppliers
                   join dechead in decheads on order.ID equals dechead.OrderID into decHeadTemp
                   from dechead in decHeadTemp.DefaultIfEmpty()
                   select new UnPayExchangeOrder
                   {
                       ID = order.ID,
                       Currency = order.Currency,
                       MainOrderID = order.MainOrderId,
                       DeclarePrice = order.DeclarePrice,
                       PaidExchangeAmount = order.PaidExchangeAmount,
                       CreateDate = order.CreateDate,
                       DDate = dechead == null ? null : dechead.DDate,
                       PayExchangeSuppliers = suppliers.Select(item => new OrderPayExchangeSupplier
                       {
                           ID = item.ID,
                           ChineseName = item.ChineseName,
                           Name = item.Name,
                           ClientSupplierID = item.SupplierID,
                       }).ToArray(),
                   };
        }

        /// <summary>
        /// 根据传入参数获取订单数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        virtual protected IQueryable<UnPayExchangeOrder> GetOrders(LambdaExpression[] expressions)
        {
            var orders = this.GetIQueryable();
            if (expressions != null)
            {
                foreach (var expression in expressions)
                {
                    orders = orders.Where(expression as Expression<Func<UnPayExchangeOrder, bool>>);
                }
            }
            return orders;
        }

        /// <summary>
        /// 根据传入参数和查询条件获取数据
        /// </summary>
        /// <param name="expressions"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public PageList<UnPayExchangeOrder> GetPageListData(LambdaExpression[] expressions, int PageSize, int PageIndex, string payExchangeStatus, string[] payExchangeStatuses)
        {
            var data = this.GetOrders(expressions);

            data = data.OrderBy(item => item.CreateDate);

            //小订单分组
            var group_data = from da in data.ToArray()
                             group da by da.MainOrderID into das
                             select new UnPayExchangeOrder
                             {
                                 MainOrderID = das.Key,
                                 CreateDate = das.Min(item => item.CreateDate),
                                 TinyOrders = das.Select(item => new TinyOrder
                                 {
                                     Currency = item.Currency,
                                     ID = item.ID,
                                     suppliers = item.PayExchangeSuppliers,
                                     DeclarePrice = item.DeclarePrice,
                                     PaidExchangeAmount = item.PaidExchangeAmount,
                                     UnPaidAmount = item.DeclarePrice - item.PaidExchangeAmount,
                                     DDate = item.DDate
                                 }).ToArray()
                             };

            if ((!string.IsNullOrWhiteSpace(payExchangeStatus))) //状态
            {
                if ((PayExchangeStatus)int.Parse(payExchangeStatus) == PayExchangeStatus.UnPay)
                {
                    group_data = group_data.Where(item => item.TinyOrders.Sum(a => a.PaidExchangeAmount) == 0);
                }
                else if ((PayExchangeStatus)int.Parse(payExchangeStatus) == PayExchangeStatus.Partial)
                {
                    group_data = group_data.Where(item => item.TinyOrders.Sum(a => a.PaidExchangeAmount) > 0 && item.TinyOrders.Sum(a => a.PaidExchangeAmount) < item.TinyOrders.Sum(a => a.DeclarePrice));
                }
                else
                {
                    group_data = group_data.Where(item => item.TinyOrders.Sum(a => a.PaidExchangeAmount) == item.TinyOrders.Sum(a => a.DeclarePrice));
                }
            }

            if (payExchangeStatuses == null || !payExchangeStatuses.Any())
            {
                payExchangeStatuses = new[] { PayExchangeStatus.UnPay, PayExchangeStatus.Partial, }.Select(t => t.GetHashCode().ToString()).ToArray();
            }
            if (payExchangeStatuses != null && payExchangeStatuses.Any())
            {
                var expPayExchangeStatus = ((Expression<Func<UnPayExchangeOrder, bool>>)null);

                if (payExchangeStatuses.Contains(PayExchangeStatus.UnPay.GetHashCode().ToString()))
                {
                    expPayExchangeStatus = expPayExchangeStatus.Or(item => item.TinyOrders.Sum(a => a.PaidExchangeAmount) == 0);
                }
                if (payExchangeStatuses.Contains(PayExchangeStatus.Partial.GetHashCode().ToString()))
                {
                    expPayExchangeStatus = expPayExchangeStatus.Or(item => item.TinyOrders.Sum(a => a.PaidExchangeAmount) > 0 && item.TinyOrders.Sum(a => a.PaidExchangeAmount) < item.TinyOrders.Sum(a => a.DeclarePrice));
                }

                group_data = group_data.Where(expPayExchangeStatus.Compile());
            }

            var total = group_data.Count();
            var orders = group_data.OrderBy(item => item.CreateDate).Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToArray();

            return new PageList<UnPayExchangeOrder>(PageIndex, PageSize, orders, total);
        }

        /// <summary>
        /// 根据条件查询订单
        /// </summary>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public UnPayExchangeOrder[] GetOrdersByExceptions(LambdaExpression[] expressions)
        {
            var orders = this.GetOrders(expressions).ToArray();
            return this.GetDatailData(orders);
        }

        /// <summary>
        /// 拼接数据
        /// </summary>
        /// <param name="orders"></param>
        /// <returns></returns>
        private UnPayExchangeOrder[] GetDatailData(UnPayExchangeOrder[] orders)
        {
            var paySuppliers = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.OrderPayExchangeSuppliersView>().Where(item => orders.Select(a => a.ID).Contains(item.OrderID)).ToArray();

            var linq = (from order in orders
                        join paySupplier in paySuppliers on order.ID equals paySupplier.OrderID into suppliers

                        select new UnPayExchangeOrder
                        {
                            ID = order.ID,
                            ClientID = order.ClientID,
                            AgreementID = order.AgreementID,
                            Currency = order.Currency,
                            CustomsExchangeRate = order.CustomsExchangeRate,
                            RealExchangeRate = order.RealExchangeRate,
                            DeclarePrice = order.DeclarePrice,
                            PayExchangeSuppliers = suppliers.Select(item => new OrderPayExchangeSupplier()
                            {
                                ID = item.ID,
                                ClientSupplierID = item.SupplierID,
                                Name = item.Name,
                                ChineseName = item.ChineseName
                            }).ToArray(),
                            InvoiceStatus = (OrderInvoiceStatus)order.InvoiceStatus,
                            PaidExchangeAmount = order.PaidExchangeAmount,
                            IsHangUp = order.IsHangUp,
                            MainOrderID = order.MainOrderID,
                            OrderStatus = (OrderStatus)order.OrderStatus,
                            Status = order.Status,
                            CreateDate = order.CreateDate,
                            UpdateDate = order.UpdateDate,
                            Summary = order.Summary,
                            // MainOrderCreateDate = mainorder.CreateDate,
                        }).ToArray();
            return linq;
        }

        /// <summary>
        /// 根据供应商获取
        /// </summary>
        /// <param name="Suppliers"></param>
        /// <returns></returns>
        public SupplierDeclarePrice[] GetSupplierDeclarePrice(string[] Suppliers, string[] orderids)
        {
            var linq = from maps in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.ProductSupplierMap>()
                       join item in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.OrderItems>()
                       on maps.ID equals item.ProductUniqueCode
                       where Suppliers.Contains(maps.SupplierID) && orderids.Contains(item.OrderID)
                       select new
                       {
                           maps.SupplierID,
                           item.OrderID,
                           item.TotalPrice,
                       };

            var data = from entity in linq.ToArray()
                       group entity by new { entity.SupplierID, entity.OrderID } into g_entity
                       select new SupplierDeclarePrice
                       {
                           SupplierID = g_entity.Key.SupplierID,
                           OrderID = g_entity.Key.OrderID,
                           DeclarePrice = g_entity.Sum(a => a.TotalPrice),
                       };

            return data.ToArray();
        }
    }

    /// <summary>
    /// 供应商付汇价格
    /// </summary>
    public class SupplierDeclarePrice
    {
        public string SupplierID { get; set; }

        public string OrderID { get; set; }

        public decimal DeclarePrice { get; set; }
    }

    public class UnPayExchangeOrder : XDTOrder
    {
        #region 属性

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DDate { get; set; }

        /// <summary>
        /// 付汇状态
        /// </summary>
        public PayExchangeStatus PayExchangeStatus
        {
            get
            {
                var tDeclarePrice = this.TinyOrders.Sum(item => item.DeclarePrice); //总报关金额
                var tPaidExchangeAmount = this.TinyOrders.Sum(item => item.PaidExchangeAmount); //总付汇金额
                if (tPaidExchangeAmount == 0)
                {
                    return PayExchangeStatus.UnPay;
                }
                else if (tPaidExchangeAmount < tDeclarePrice)
                {
                    return PayExchangeStatus.Partial;
                }
                else
                {
                    return PayExchangeStatus.All;
                }
            }
        }

        /// <summary>
        /// 获取订单类型
        /// </summary>
        public OrderType OrderType
        {
            get
            {
                using (var orders = new OrderBaseOrigin())
                {
                    return orders[this.MainOrderID]?.Type ?? OrderType.Declare;
                }
            }
        }
        #endregion

        /// <summary>
        /// 小订单数组
        /// </summary>
        public TinyOrder[] TinyOrders { get; set; }
    }

    /// <summary>
    /// 小订单对象
    /// </summary>
    public class TinyOrder
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 报关金额
        /// </summary>
        public decimal DeclarePrice { get; set; }

        public OrderPayExchangeSupplier[] suppliers { get; set; }

        /// <summary>
        /// 已申请付汇金额
        /// </summary>
        public decimal PaidExchangeAmount { get; set; }

        /// <summary>
        /// 交易币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 为付汇金额
        /// </summary>
        public decimal UnPaidAmount { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DDate { get; set; }
    }

    public class OrderPayExchangeSupplier
    {
        public string ID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 付汇供应商ID
        /// </summary>
        public string ClientSupplierID { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 付汇供应商中文名称
        /// </summary>
        public string ChineseName { get; set; }
    }

    /// <summary>
    /// 代理订单状态
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 草稿
        /// </summary>
        [Description("草稿")]
        Draft = 0,

        /// <summary>
        /// 待归类（已下单）
        /// </summary>
        [Description("待归类")]
        Confirmed = 1,

        /// <summary>
        /// 待报价（已归类）
        /// </summary>
        [Description("待报价")]
        Classified = 2,

        /// <summary>
        /// 待客户确认（已报价）
        /// </summary>
        [Description("待客户确认")]
        Quoted = 3,

        /// <summary>
        /// 待报关（已客户确认）
        /// </summary>
        [Description("待报关")]
        QuoteConfirmed = 4,

        /// <summary>
        /// 待出库（已报关）
        /// </summary>
        [Description("待出库")]
        Declared = 5,

        /// <summary>
        /// 待收货（已出库）
        /// </summary>
        [Description("待收货")]
        WarehouseExited = 6,

        /// <summary>
        /// 完成
        /// </summary>
        [Description("完成")]
        Completed = 7,

        /// <summary>
        /// 已退回
        /// </summary>
        [Description("已退回")]
        Returned = 8,

        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Canceled = 9
    }

    /// <summary>
    /// 付汇状态
    /// </summary>
    public enum PayExchangeStatus
    {
        [Description("未付汇")]
        UnPay = 1,

        [Description("部分付汇")]
        Partial = 2,

        [Description("已付汇")]
        All = 4,
    }

    public static class UnPayExchangeOrderExtend
    {
        /// <summary>
        /// 付汇申请页面中，订单本次申请金额
        /// </summary>
        /// <param name="order"></param>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        static public OrderCurrentPayAmountView OrderCurrentPayAmount(this UnPayExchangeOrder order, string supplierID)
        {
            return new OrderCurrentPayAmountView(order.ID, supplierID);
        }
    }
}
