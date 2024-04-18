using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.Enums;
using System.Linq.Expressions;
using Yahv.Linq.Generic;
using Yahv.PvWsOrder.Services.Models;
using Layers.Data.Sqls;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views
{
    /// <summary>
    /// 我的仓储收货—订单视图
    /// </summary>
    public class MyRecievedOrderView : MyBaseOrderView
    {
        public MyRecievedOrderView(Underly.Erps.IErpAdmin admin, string companyID) : base(admin, companyID)
        {

        }

        protected override IQueryable<Order_Show> GetIQueryable(Expression<Func<Order_Show, bool>> expression, params LambdaExpression[] expressions)
        {
            var linq = base.GetIQueryable(expression, expressions).Where(item => item.Type == OrderType.Recieved);
            return linq;
        }

        protected override IEnumerable<Order_Show> OnReadShips(Order_Show[] results)
        {
            var linq = base.OnReadShips(results);
            return linq;
        }
    }

    /// <summary>
    /// 我的即收即发—订单视图
    /// </summary>
    public class MyTransportOrderView : MyBaseOrderView
    {
        public MyTransportOrderView(Underly.Erps.IErpAdmin admin, string companyID) : base(admin, companyID)
        {

        }

        protected override IQueryable<Order_Show> GetIQueryable(Expression<Func<Order_Show, bool>> expression, params LambdaExpression[] expressions)
        {
            var linq = base.GetIQueryable(expression, expressions).Where(item => item.Type == OrderType.Transport);
            return linq;
        }

        protected override IEnumerable<Order_Show> OnReadShips(Order_Show[] results)
        {
            var linq = base.OnReadShips(results);
            return linq;
        }
    }

    /// <summary>
    /// 我的仓储发货—订单视图
    /// </summary>
    public class MyDeliveryOrderView : MyBaseOrderView
    {
        public MyDeliveryOrderView(Underly.Erps.IErpAdmin admin, string companyID) : base(admin, companyID)
        {

        }

        protected override IQueryable<Order_Show> GetIQueryable(Expression<Func<Order_Show, bool>> expression, params LambdaExpression[] expressions)
        {
            var linq = base.GetIQueryable(expression, expressions).Where(item => item.Type == OrderType.Delivery);
            return linq;
        }

        protected override IEnumerable<Order_Show> OnReadShips(Order_Show[] results)
        {
            var linq = base.OnReadShips(results);
            return linq;
        }
    }

    /// <summary>
    /// 我的仓储代报关—订单视图
    /// </summary>
    public class MyDeclareOrderView : MyBaseOrderView
    {
        public MyDeclareOrderView(Underly.Erps.IErpAdmin admin, string companyID) : base(admin, companyID)
        {

        }

        protected override IQueryable<Order_Show> GetIQueryable(Expression<Func<Order_Show, bool>> expression, params LambdaExpression[] expressions)
        {
            var linq = base.GetIQueryable(expression, expressions).Where(item => item.Type == OrderType.Declare);
            return linq;
        }

        protected override IEnumerable<Order_Show> OnReadShips(Order_Show[] results)
        {
            var linq = base.OnReadShips(results);
            return linq;
        }
    }

    /// <summary>
    /// 我的仓储转报关—订单视图
    /// </summary>
    public class MyTurnDeclareOrderView : MyBaseOrderView
    {
        public MyTurnDeclareOrderView(Underly.Erps.IErpAdmin admin, string companyID) : base(admin, companyID)
        {

        }

        protected override IQueryable<Order_Show> GetIQueryable(Expression<Func<Order_Show, bool>> expression, params LambdaExpression[] expressions)
        {
            var linq = base.GetIQueryable(expression, expressions).Where(item => item.Type == OrderType.TransferDeclare);
            return linq;
        }

        protected override IEnumerable<Order_Show> OnReadShips(Order_Show[] results)
        {
            var linq = base.OnReadShips(results);
            return linq;
        }
    }

    /// <summary>
    /// 已挂起 - 订单视图
    /// </summary>
    public class SuspendedOrderView : BaseOrderView
    {
        public SuspendedOrderView()
        {

        }
        protected override IQueryable<Order_Show> GetIQueryable(Expression<Func<Order_Show, bool>> expression, params LambdaExpression[] expressions)
        {
            var linq = base.GetIQueryable(expression, expressions)
                .Where(item => item.MainStatus == CgOrderStatus.挂起);
            return linq;
        }

        protected override IEnumerable<Order_Show> OnReadShips(Order_Show[] results)
        {
            var linq = base.OnReadShips(results);
            return linq;
        }
    }

    /// <summary>
    /// 我的本位币应收——订单视图
    /// </summary>
    public class MyCnyOrderView : Linq.UniqueView<Order_Show_AddBill, PvWsOrderReponsitory>
    {
        //系统管理员
        private Underly.Erps.IErpAdmin admin;

        //主体公司
        private string CompanyID;

        public MyCnyOrderView(Underly.Erps.IErpAdmin admin, string companyID)
        {
            this.admin = admin;
            this.CompanyID = companyID;
        }

        protected override IQueryable<Order_Show_AddBill> GetIQueryable()
        {
            var ordersView = new Origins.OrderOrigin(this.Reponsitory)
                .Where(item => item.PayeeID == CompanyID)
                .OrderByDescending(item => item.CreateDate).AsQueryable();
            var clientView = new Yahv.Services.Views.WsClientsTopView<PvWsOrderReponsitory>(this.Reponsitory).Where(item => item.Status == Underly.ApprovalStatus.Normal);

            if (!admin.IsSuper)
            {
                //我的客户
                var clientIds = new Yahv.Services.Views.TrackerWsClients<PvWsOrderReponsitory>(this.Reponsitory, this.admin, this.CompanyID)
                    .Select(item => item.ID).ToArray();
                //客户的订单
                ordersView = ordersView.Where(item => clientIds.Contains(item.ClientID));
                clientView = clientView.Where(item => clientIds.Contains(item.ID));
            }
            //本位币视图
            var statistics1 = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.VouchersCnyStatisticsView>()
                .Where(t => t.Status == (int)GeneralStatus.Normal &&
                t.Business == Payments.ConductConsts.供应链 &&
                t.Subject != Payments.SubjectConsts.代付货款 && t.Subject != Payments.SubjectConsts.代收货款 &&
                t.Payee == CompanyID);
            var statistics2 = (from entity in statistics1
                               group entity by entity.OrderID into newt
                               select new
                               {
                                   OrderID = newt.Key,
                                   TotalPrice = newt.Sum(t => t.LeftPrice),
                               }).Where(t => t.TotalPrice > 0);

            var linq = from entity in ordersView
                       join client in clientView on entity.ClientID equals client.ID
                       join cny in statistics2 on entity.ID equals cny.OrderID
                       select new Order_Show_AddBill
                       {
                           ID = entity.ID,
                           Type = entity.Type,
                           MainStatus = entity.MainStatus,
                           PaymentStatus = entity.PaymentStatus,
                           CreateDate = entity.CreateDate,

                           ClientID = client.ID,
                           ClientName = client.Name,
                           EnterCode = client.EnterCode,
                           Currency = Currency.CNY,
                           TotalPrice = cny.TotalPrice,
                       };
            return linq.ToArray().AsQueryable();
        }
    }

    public class Order_Show_AddBill : Linq.IUnique
    {
        public string ID { get; set; }
        public string ClientID { get; set; }

        public DateTime CreateDate { get; set; }

        public string ClientName { get; set; }

        public string EnterCode { get; set; }

        public OrderType Type { get; set; }

        public CgOrderStatus MainStatus { get; set; }

        public OrderPaymentStatus PaymentStatus { get; set; }

        public Currency Currency { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
