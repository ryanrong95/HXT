using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.User.Plat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.User.Plat.Views
{
    public class UnConfirmedOrdersView : View<UnConfirmedOrdersViewModel, ScCustomsReponsitory>
    {
        private LambdaExpression UserOrClientExp;

        public UnConfirmedOrdersView(IPlatUser user)
        {
            if (user.IsMain)
            {
                //return base.GetIQueryable(expression, expressions).Where(item => item.ClientID == this.User.ClientID).OrderByDescending(item => item.CreateDate);
                this.UserOrClientExp = (Expression<Func<UnConfirmedOrdersViewModel, bool>>)(t => t.ClientID == user.ClientID);
            }
            else
            {
                //return base.GetIQueryable(expression, expressions).Where(item => item.UserID == this.User.ID).OrderByDescending(item => item.CreateDate);
                this.UserOrClientExp = (Expression<Func<UnConfirmedOrdersViewModel, bool>>)(t => t.UserID == user.ID);
            }
        }

        protected override IQueryable<UnConfirmedOrdersViewModel> GetIQueryable()
        {
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var orderConsignees = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>();
            var clientSuppliers = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>();
            var users = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>();
            var orderControls = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>();
            var orderControlSteps = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControlSteps>();
            var orderConsignors = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignors>();

            //因 删除型号/修改数量 挂起的订单
            var controlOrderInfos = from orderControl in orderControls
                                    join orderControlStep in orderControlSteps on orderControl.ID equals orderControlStep.OrderControlID
                                    join order in orders on orderControl.OrderID equals order.ID
                                    where (orderControl.ControlType == (int)Needs.Wl.Models.Enums.OrderControlType.DeleteModel
                                        || orderControl.ControlType == (int)Needs.Wl.Models.Enums.OrderControlType.ChangeQuantity)
                                        && orderControlStep.ControlStatus == (int)Needs.Wl.Models.Enums.OrderControlStatus.Auditing
                                    where order.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                                       && order.IsHangUp == true
                                    select new UnConfirmedOrdersViewModel
                                    {
                                        OrderID = order.ID,

                                        UserID = order.UserID,
                                        ClientID = order.ClientID,
                                    };

            controlOrderInfos = controlOrderInfos.Where(this.UserOrClientExp as Expression<Func<UnConfirmedOrdersViewModel, bool>>);

            var controlOrderIDs = controlOrderInfos.Distinct().Select(t => t.OrderID);

            //未确认的订单信息
            var unConfirmedOrdersTabs = from order in orders
                                        join orderConsignee in orderConsignees on order.ID equals orderConsignee.OrderID
                                        join clientSupplier in clientSuppliers on orderConsignee.ClientSupplierID equals clientSupplier.ID
                                        join user in users
                                            on new
                                            {
                                                UserID = order.UserID,
                                                UsersDataStatus = (int)Needs.Wl.Models.Enums.Status.Normal,
                                            }
                                            equals new
                                            {
                                                UserID = user.ID,
                                                UsersDataStatus = user.Status,
                                            }
                                            into users2
                                        from user in users2.DefaultIfEmpty()
                                        where order.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                                            && order.OrderStatus == (int)Needs.Wl.Models.Enums.OrderStatus.Quoted
                                            && order.IsHangUp == false
                                            //&& order.ClientID == "68248e7e4c6f95240c93e5eae81a64ea"
                                        select new UnConfirmedOrdersViewModel
                                        {
                                            OrderID = order.ID,
                                            CreateDate = order.CreateDate,
                                            Currency = order.Currency,
                                            DeclarePrice = order.DeclarePrice,
                                            OrderStatus = (Needs.Wl.Models.Enums.OrderStatus)order.OrderStatus,
                                            ClientSupplierChineseName = clientSupplier.ChineseName,
                                            OrderMaker = user != null ? user.RealName : "跟单员",
                                            IsBecauseModified = false,

                                            UserID = order.UserID,
                                            ClientID = order.ClientID,
                                        };

            unConfirmedOrdersTabs = unConfirmedOrdersTabs.Where(this.UserOrClientExp as Expression<Func<UnConfirmedOrdersViewModel, bool>>);

            var controlOrdersTabs = from order in orders
                                    join orderConsignee in orderConsignees on order.ID equals orderConsignee.OrderID
                                    join clientSupplier in clientSuppliers on orderConsignee.ClientSupplierID equals clientSupplier.ID
                                    join user in users
                                        on new
                                        {
                                            UserID = order.UserID,
                                            UsersDataStatus = (int)Needs.Wl.Models.Enums.Status.Normal,
                                        }
                                        equals new
                                        {
                                            UserID = user.ID,
                                            UsersDataStatus = user.Status,
                                        }
                                        into users2
                                    from user in users2.DefaultIfEmpty()
                                    where controlOrderIDs.Contains(order.ID)
                                    select new UnConfirmedOrdersViewModel
                                    {
                                        OrderID = order.ID,
                                        CreateDate = order.CreateDate,
                                        Currency = order.Currency,
                                        DeclarePrice = order.DeclarePrice,
                                        OrderStatus = (Needs.Wl.Models.Enums.OrderStatus)order.OrderStatus,
                                        ClientSupplierChineseName = clientSupplier.ChineseName,
                                        OrderMaker = user != null ? user.RealName : "跟单员",
                                        IsBecauseModified = true,

                                        UserID = order.UserID,
                                        ClientID = order.ClientID,
                                    };

            var ordersTabs = (from unConfirmedOrder in unConfirmedOrdersTabs
                              select unConfirmedOrder
                            ).Union(
                                from controlOrder in controlOrdersTabs
                                select controlOrder
                            );

            //OrderConsignors 中的数据
            //var ordersTabIDs = ordersTabs.Select(t => t.OrderID);
            string[] ordersTabIDs = ordersTabs.Select(t => t.OrderID).ToArray();

            //var qwe2 = from orderConsignor in orderConsignors
            //          where ordersTabIDs.Contains(orderConsignor.OrderID)
            //          //group orderConsignor by new { orderConsignor.OrderID } into g
            //          select new UnConfirmedOrdersViewModel
            //          {
            //              OrderID = orderConsignor.OrderID,
            //              OrderConsignorContact = orderConsignor.Contact,
            //          };

            var orderConsignorsTabs = from orderConsignor in orderConsignors
                                      where ordersTabIDs.Contains(orderConsignor.OrderID)
                                      group orderConsignor by new { orderConsignor.OrderID } into g
                                      select new UnConfirmedOrdersViewModel
                                      {
                                          OrderID = g.Key.OrderID,
                                          OrderConsignorContact = g.FirstOrDefault().Contact,
                                      };

            var result = from ordersTab in ordersTabs
                         join orderConsignorsTab in orderConsignorsTabs on ordersTab.OrderID equals orderConsignorsTab.OrderID into orderConsignorsTabs2
                         from orderConsignorsTab in orderConsignorsTabs2.DefaultIfEmpty()
                         orderby ordersTab.CreateDate descending
                         select new UnConfirmedOrdersViewModel
                         {
                             OrderID = ordersTab.OrderID,
                             CreateDate = ordersTab.CreateDate,
                             Currency = ordersTab.Currency,
                             DeclarePrice = ordersTab.DeclarePrice,
                             OrderStatus = ordersTab.OrderStatus,
                             ClientSupplierChineseName = ordersTab.ClientSupplierChineseName,
                             OrderMaker = ordersTab.OrderMaker,
                             OrderConsignorContact = orderConsignorsTab != null ? orderConsignorsTab.OrderConsignorContact : string.Empty,
                             IsBecauseModified = ordersTab.IsBecauseModified,
                         };

            return result;
        }
    }

    public class UnConfirmedOrdersViewModel : IUnique
    {
        /// <summary>
        /// 
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderID { get; set; } = string.Empty;

        /// <summary>
        /// 订单创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 交易币种
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// 报关总价
        /// </summary>
        public decimal DeclarePrice { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public Needs.Wl.Models.Enums.OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string ClientSupplierChineseName { get; set; } = string.Empty;

        /// <summary>
        /// 下单人
        /// </summary>
        public string OrderMaker { get; set; } = string.Empty;

        /// <summary>
        /// 收件人
        /// </summary>
        public string OrderConsignorContact { get; set; } = string.Empty;

        /// <summary>
        /// 是否是因修改（删除型号/修改数量）引起的确认订单
        /// </summary>
        public bool IsBecauseModified { get; set; }



        public string UserID { get; set; } = string.Empty;

        public string ClientID { get; set; } = string.Empty;
    }


}
