//using Layers.Data.Sqls;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;
//using Yahv.Linq.Generic;
//using Yahv.PvWsOrder.Services.ClientModels;
//using Yahv.Services.Models;
//using Yahv.Services.Models.LsOrder;
//using Yahv.Services.Views;
//using Yahv.Underly;

//namespace Yahv.PvWsOrder.Services.ClientViews
//{
//    /// <summary>
//    /// 我的租赁订单
//    /// </summary>
//    public class MyLsOrders : LsOrderAlls
//    {
//        IUser User;

//        private MyLsOrders(IUser user) : base(LsOrderType.Lease)
//        {
//            this.User = user;
//        }

//        protected override IQueryable<LsOrder> GetIQueryable()
//        {
//            return base.GetIQueryable().Where(item => item.ClientID == User.EnterpriseID).OrderByDescending(item => item.CreateDate);
//        }

//        /// <summary>
//        /// 获取订单分页信息
//        /// </summary>
//        /// <param name="expressions"></param>
//        /// <param name="PageSize"></param>
//        /// <param name="PageIndex"></param>
//        /// <returns></returns>
//        public PageList<LsOrderExtends> GetPageData(LambdaExpression[] expressions, int PageSize, int PageIndex)
//        {
//            //分页
//            var orders = base.GetPageListOrders(expressions, PageSize, PageIndex);
//            //ID数组
//            var ids = orders.Select(item => item.ID).ToArray();
//            //产品视图
//            var productView = new LsProductView(this.Reponsitory).ToList();
//            //订单项视图
//            var orderItemView = new LsOrderItemAlls(this.Reponsitory).Where(item => ids.Contains(item.OrderID)).ToArray();
//            var orderFiles = new CenterFilesView().Where(item=>ids.Contains(item.LsOrderID)).ToArray();
//            //订单项
//            var _orderitem = from entity in orderItemView
//                            join product in productView on entity.ProductID equals product.ID
//                            select new LsOrderItem
//                            {
//                                ID = entity.ID,
//                                OrderID=entity.OrderID,
//                                Quantity = entity.Quantity,
//                                ProductID = entity.ProductID,
//                                Product = product,
//                                UnitPrice = entity.UnitPrice,
//                            };
//            var linq = from entity in orders.OrderByDescending(item => item.CreateDate)
//                       join file in orderFiles on entity.ID equals file.LsOrderID into files
//                       join orderItem in _orderitem on entity.ID equals orderItem.OrderID into orderItems
//                       select new LsOrderExtends
//                       {
//                           ID = entity.ID,
//                           FatherID=entity.FatherID,
//                           Type = entity.Type,
//                           Source = entity.Source,
//                           ClientID = entity.ClientID,
//                           PayeeID = entity.PayeeID,
//                           BeneficiaryID = entity.BeneficiaryID,
//                           Currency = entity.Currency,
//                           InvoiceID = entity.InvoiceID,
//                           IsInvoiced=entity.IsInvoiced, 
//                           InheritStatus=entity.InheritStatus,
//                           StartDate = entity.StartDate,
//                           EndDate = entity.EndDate,
//                           Status = entity.Status,
//                           Creator = entity.Creator,
//                           CreateDate = entity.CreateDate,
//                           ModifyDate = entity.ModifyDate,
//                           Summary = entity.Summary,
//                           OrderItems = orderItems.ToArray(),
//                           OrderFiles=files.ToArray(),
//                       };
//            return new PageList<LsOrderExtends>(PageIndex, PageSize, linq, orders.Total);
//        }

//        /// <summary>
//        /// 获取订单详情
//        /// </summary>
//        /// <param name="ID"></param>
//        /// <returns></returns>
//        public LsOrderExtends GetOrderDetail(string ID)
//        {
//            var order = this[ID];
//            var productView = new LsProductView().ToList();
//            var invoice= new InvoicesTopView<PvWsOrderReponsitory>()[order.InvoiceID];
//            var orderItem = from entity in new LsOrderItemAlls(this.Reponsitory).Where(item => item.OrderID == order.ID).ToList()
//                            join product in productView on entity.ProductID equals product.ID
//                            select new LsOrderItem
//                            {
//                                ID = entity.ID,
//                                OrderID = entity.OrderID,
//                                Quantity = entity.Quantity,
//                                Currency = entity.Currency,
//                                UnitPrice = entity.UnitPrice,
//                                ProductID = entity.ProductID,
//                                Description = entity.Description,
//                                Supplier = entity.Supplier,
//                                Status = entity.Status,
//                                CreateDate = entity.CreateDate,
//                                Lease = entity.Lease,
//                                Product = product,
//                            };
//            var orderFiles = new CenterFilesView().SearchByLsOrderID(order.ID).ToArray();
//            return new LsOrderExtends
//            {
//                ID = order.ID,
//                FatherID=order.FatherID,
//                Type = order.Type,
//                Source = order.Source,
//                ClientID = order.ClientID,
//                PayeeID = order.PayeeID,
//                BeneficiaryID = order.BeneficiaryID,
//                Currency = order.Currency,
//                InvoiceID = order.InvoiceID,
//                IsInvoiced=order.IsInvoiced,
//                InheritStatus=order.InheritStatus,
//                StartDate=order.StartDate,
//                EndDate=order.EndDate,
//                Status = order.Status,
//                Creator = order.Creator,
//                CreateDate = order.CreateDate,
//                ModifyDate = order.ModifyDate,
//                Summary = order.Summary,
//                OrderItems = orderItem.ToArray(),
//                Invoice=invoice,
//                OrderFiles= orderFiles.ToArray(),
//            };
//        }
//    }
//}
