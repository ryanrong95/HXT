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
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.Views
{
    /// <summary>
    /// 订单基础视图
    /// </summary>
    public class BaseOrderView : QueryRoll<Order_Show, Order_Show, PvWsOrderReponsitory>
    {
        public BaseOrderView()
        {

        }

        protected override IQueryable<Order_Show> GetIQueryable(Expression<Func<Order_Show, bool>> expression, params LambdaExpression[] expressions)
        {
            var ordersView = new Origins.OrderOrigin(this.Reponsitory).OrderByDescending(item => item.CreateDate).AsQueryable();
            var clients = new Yahv.Services.Views.WsClientsTopView<PvWsOrderReponsitory>(this.Reponsitory).Where(item => item.Status == Underly.ApprovalStatus.Normal);
            var inputView = new OrderInputAlls(this.Reponsitory);
            var outputView = new OrderOutputAlls(this.Reponsitory);
            var supplierView = new WsSupplierAlls(this.Reponsitory);

            var linq = from entity in ordersView
                       join client in clients on entity.ClientID equals client.ID
                       join supplier in supplierView on new { entity.SupplierID, ClientID = client.ID }
                           equals new { SupplierID = supplier.ID, ClientID = supplier.OwnID } into suppliers
                       from supplier in suppliers.DefaultIfEmpty()
                       join input in inputView on entity.ID equals input.ID into inputs
                       from input in inputs.DefaultIfEmpty()
                       join output in outputView on entity.ID equals output.ID into outputs
                       from output in outputs.DefaultIfEmpty()
                       where entity.MainStatus != CgOrderStatus.取消
                       select new Order_Show
                       {
                           ID = entity.ID,
                           Type = entity.Type,
                           ClientID = entity.ClientID,
                           ClientName = client.Name,
                           EnterCode = client.EnterCode,
                           MainStatus = entity.MainStatus,
                           PaymentStatus = entity.PaymentStatus,
                           CreateDate = entity.CreateDate,

                           SupplierName = supplier.Name,
                           IsPayCharge = input.IsPayCharge,
                           IsReciveCharge = output.IsReciveCharge,
                           EnterType = input.Waybill.Type,
                           ExitType = output.Waybill.Type,
                           LoadingExcuteStatus = input.Waybill.WayLoading.LoadingExcuteStatus,
                           Consignee = output.Waybill.Consignee.Company ?? output.Waybill.Consignee.Contact
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<Order_Show, bool>>);
            }
            return linq.Where(expression);
        }

        protected override IEnumerable<Order_Show> OnReadShips(Order_Show[] results)
        {
            var linq = from entity in results
                       select new Order_Show
                       {
                           ID = entity.ID,
                           Type = entity.Type,
                           ClientID = entity.ClientID,
                           ClientName = entity.ClientName,
                           EnterCode = entity.EnterCode,
                           MainStatus = entity.MainStatus,
                           PaymentStatus = entity.PaymentStatus,
                           CreateDate = entity.CreateDate,

                           SupplierName = entity.SupplierName,

                           IsPayCharge = entity.IsPayCharge,
                           IsReciveCharge = entity.IsReciveCharge,
                           EnterType = entity.EnterType,
                           ExitType = entity.ExitType,
                           LoadingExcuteStatus = entity.LoadingExcuteStatus,
                           Consignee = entity.Consignee,
                       };
            return linq;
        }
    }

    /// <summary>
    /// 管理员订单视图
    /// </summary>
    public class MyBaseOrderView : QueryRoll<Order_Show, Order_Show, PvWsOrderReponsitory>
    {
        //系统管理员
        private Underly.Erps.IErpAdmin admin;
        //主体公司
        private string CompanyID;

        public MyBaseOrderView(Underly.Erps.IErpAdmin admin, string companyID)
        {
            this.admin = admin;
            this.CompanyID = companyID;
        }

        protected override IQueryable<Order_Show> GetIQueryable(Expression<Func<Order_Show, bool>> expression, params LambdaExpression[] expressions)
        {
            var ordersView = new Origins.OrderOrigin(this.Reponsitory).Where(item => item.PayeeID == CompanyID).OrderByDescending(item => item.CreateDate).AsQueryable();
            var clients = new Yahv.Services.Views.WsClientsTopView<PvWsOrderReponsitory>(this.Reponsitory).Where(item => item.Status == Underly.ApprovalStatus.Normal);
            var inputView = new OrderInputAlls(this.Reponsitory);
            var outputView = new OrderOutputAlls(this.Reponsitory);
            var supplierView = new WsSupplierAlls(this.Reponsitory);

            if (!admin.IsSuper)
            {
                //管理员的代仓储客户
                var clientIds = new Yahv.Services.Views.TrackerWsClients<PvWsOrderReponsitory>(this.Reponsitory, this.admin, this.CompanyID)
                    .Select(item => item.ID).ToArray();
                ordersView = ordersView.Where(item => clientIds.Contains(item.ClientID));
            }

            var linq = from entity in ordersView
                       join client in clients on entity.ClientID equals client.ID
                       join supplier in supplierView on new { entity.SupplierID, ClientID = client.ID }
                           equals new { SupplierID = supplier.ID, ClientID = supplier.OwnID } into suppliers
                       from supplier in suppliers.DefaultIfEmpty()
                       join input in inputView on entity.ID equals input.ID into inputs
                       from input in inputs.DefaultIfEmpty()
                       join output in outputView on entity.ID equals output.ID into outputs
                       from output in outputs.DefaultIfEmpty()
                       where entity.MainStatus != CgOrderStatus.取消
                       select new Order_Show
                       {
                           ID = entity.ID,
                           Type = entity.Type,
                           ClientID = entity.ClientID,
                           ClientName = client.Name,
                           EnterCode = client.EnterCode,
                           MainStatus = entity.MainStatus,
                           PaymentStatus = entity.PaymentStatus,
                           CreateDate = entity.CreateDate,

                           SupplierName = supplier.Name,
                           IsPayCharge = input.IsPayCharge,
                           IsReciveCharge = output.IsReciveCharge,
                           EnterType = input.Waybill.Type,
                           ExitType = output.Waybill.Type,
                           LoadingExcuteStatus = input.Waybill.WayLoading.LoadingExcuteStatus,
                           Consignee = output.Waybill.Consignee.Company ?? output.Waybill.Consignee.Contact
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<Order_Show, bool>>);
            }
            return linq.Where(expression);
        }

        protected override IEnumerable<Order_Show> OnReadShips(Order_Show[] results)
        {
            var linq = from entity in results
                       select new Order_Show
                       {
                           ID = entity.ID,
                           Type = entity.Type,
                           ClientID = entity.ClientID,
                           ClientName = entity.ClientName,
                           EnterCode = entity.EnterCode,
                           MainStatus = entity.MainStatus,
                           PaymentStatus = entity.PaymentStatus,
                           CreateDate = entity.CreateDate,

                           SupplierName = entity.SupplierName,

                           IsPayCharge = entity.IsPayCharge,
                           IsReciveCharge = entity.IsReciveCharge,
                           EnterType = entity.EnterType,
                           ExitType = entity.ExitType,
                           LoadingExcuteStatus = entity.LoadingExcuteStatus,
                           Consignee = entity.Consignee,
                       };
            return linq;
        }
    }

    public class Order_Show
    {
        public string ID { get; set; }

        public OrderType Type { get; set; }

        public string ClientID { get; set; }

        public string ClientName { get; set; }

        public string EnterCode { get; set; }

        public DateTime CreateDate { get; set; }

        public string SupplierName { get; set; }

        public CgOrderStatus MainStatus { get; set; }

        public OrderPaymentStatus PaymentStatus { get; set; }

        //public OrderInput OrderInput { get; set; }

        //public OrderOutput OrderOutput { get; set; }

        /// <summary>
        /// 交货方式
        /// </summary>
        public WaybillType? EnterType { get; set; }

        /// <summary>
        /// 提货状态
        /// </summary>
        public CgLoadingExcuteStauts? LoadingExcuteStatus { get; set; }

        /// <summary>
        /// 发货方式
        /// </summary>
        public WaybillType? ExitType { get; set; }

        public bool? IsPayCharge { get; set; }

        public bool? IsReciveCharge { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        public string Consignee { get; set; }

    }
}
