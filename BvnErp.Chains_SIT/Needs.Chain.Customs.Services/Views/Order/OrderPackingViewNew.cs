using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class OrderPackingViewNew
    {
        ScCustomsReponsitory Reponsitory { get; set; }

        public OrderPackingViewNew()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        /// <summary>
        /// 装箱信息页面列表信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="nCount"></param>
        /// <returns></returns>
        public List<OrderPackingListModel> GetPagedPackingList(bool isSa, string adminID, LambdaExpression[] expressions, int pageIndex, int pageSize, out int nCount)
        {
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();

            var orderConsignees = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>();
            var clientSuppliers = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>();

            var clientAdminsView = new ClientAdminsView(this.Reponsitory);

            var theOrders = from order in orders
                            join client in clients
                                on new
                                {
                                    ClientID = order.ClientID,
                                    OrderDataStatus = order.Status,
                                    ClientDataStatus = (int)Enums.Status.Normal,
                                    OrderStatus = order.OrderStatus,
                                }
                                equals new
                                {
                                    ClientID = client.ID,
                                    OrderDataStatus = (int)Enums.Status.Normal,
                                    ClientDataStatus = client.Status,
                                    OrderStatus = (int)Enums.OrderStatus.QuoteConfirmed,
                                }
                            join company in companies
                                on new
                                {
                                    CompanyID = client.CompanyID,
                                    CompanyDataStatus = (int)Enums.Status.Normal,
                                }
                                equals new
                                {
                                    CompanyID = company.ID,
                                    CompanyDataStatus = company.Status,
                                }
                            join clientAdmin in clientAdminsView.Where(t => t.Type == Enums.ClientAdminType.Merchandiser && t.Status == Enums.Status.Normal) on client.ID equals clientAdmin.ClientID into clientAdmins
                            from cadmin in clientAdmins.DefaultIfEmpty()
                            select new OrderPackingListModel
                            {
                                OrderID = order.ID,
                                ClientType = (Enums.ClientType)((int)client.ClientType),
                                ClientID = client.ID,
                                ClientCode = client.ClientCode,
                                CompanyName = company.Name,
                                DeclarePrice = order.DeclarePrice,
                                Currency = order.Currency,
                                OrderCreateDate = order.CreateDate,

                                MerchandiserID = cadmin.Admin.ID,
                            };

            foreach (var predicate in expressions)
            {
                theOrders = theOrders.Where(predicate as Expression<Func<OrderPackingListModel, bool>>);
            }

            if (!isSa)
            {
                theOrders = theOrders.Where(t => t.MerchandiserID == adminID);
            }

            nCount = theOrders.Count();

            theOrders = theOrders.OrderByDescending(t => t.OrderCreateDate);

            theOrders = theOrders.Skip(pageSize * (pageIndex - 1)).Take(pageSize);

            //orderConsignees   clientSuppliers
            var results = from theOrder in theOrders
                          join orderConsignee in orderConsignees on new { OrderID = theOrder.OrderID, OrderConsigneeDataStatus = (int)Enums.Status.Normal, }
                                equals new { OrderID = orderConsignee.OrderID, OrderConsigneeDataStatus = orderConsignee.Status, }
                                into orderConsignees2
                          from orderConsignee in orderConsignees2.DefaultIfEmpty()

                          join clientSupplier in clientSuppliers on new { ClientSupplierID = orderConsignee.ClientSupplierID, ClientSupplierDataStatus = (int)Enums.Status.Normal, }
                                equals new { ClientSupplierID = clientSupplier.ID, ClientSupplierDataStatus = clientSupplier.Status, }
                                into clientSuppliers2
                          from clientSupplier in clientSuppliers2.DefaultIfEmpty()
                          select new OrderPackingListModel
                          {
                              OrderID = theOrder.OrderID,
                              ClientType = theOrder.ClientType,
                              ClientID = theOrder.ClientID,
                              ClientCode = theOrder.ClientCode,
                              CompanyName = theOrder.CompanyName,
                              DeclarePrice = theOrder.DeclarePrice,
                              Currency = theOrder.Currency,
                              OrderCreateDate = theOrder.OrderCreateDate,
                              ClientSupplierChineseName = clientSupplier.ChineseName,
                              OrderConsigneeType = (Enums.HKDeliveryType)orderConsignee.Type,
                          };

            return results.ToList();
        }

        public List<OrderPackingListModel> GetPackingStatus(string[] orderIDs)
        {
            var deliveriesTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeliveriesTopView>();
            var declarationNotices = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNotices>();

            var viewDatas = (from delivery in deliveriesTopView
                             where orderIDs.Contains(delivery.iptTinyOrderID)
                             group delivery by new { delivery.iptTinyOrderID } into g
                             select new OrderPackingListModel
                             {
                                 OrderID = g.Key.iptTinyOrderID,
                             }).ToArray();

            var decNoticeDatas = (from decNotice in declarationNotices
                                  where orderIDs.Contains(decNotice.OrderID)
                                  group decNotice by new { decNotice.OrderID } into g
                                  select new OrderPackingListModel
                                  {
                                      OrderID = g.Key.OrderID,
                                  }).ToArray();

            var results = (from orderID in orderIDs
                           join viewData in viewDatas on orderID equals viewData.OrderID into viewDatas2
                           from viewData in viewDatas2.DefaultIfEmpty()

                           join decNoticeData in decNoticeDatas on orderID equals decNoticeData.OrderID into decNoticeDatas2
                           from decNoticeData in decNoticeDatas2.DefaultIfEmpty()

                           select new OrderPackingListModel
                           {
                               OrderID = orderID,
                               HasPacking = (viewData != null),
                               PackingStatus = decNoticeData != null ? Enums.PackingStatus.Sealed : Enums.PackingStatus.UnSealed,
                           }).ToList();

            return results;
        }
    }

    public class OrderPackingListModel
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 客户类型
        /// </summary>
        public Enums.ClientType ClientType { get; set; }

        /// <summary>
        /// ClientID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public string ClientCode { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 跟单员ID
        /// </summary>
        public string MerchandiserID { get; set; }

        /// <summary>
        /// 报关总货值
        /// </summary>
        public decimal DeclarePrice { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 下单日期
        /// </summary>
        public DateTime OrderCreateDate { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string ClientSupplierChineseName { get; set; }

        /// <summary>
        /// 香港交货方式
        /// </summary>
        public Enums.HKDeliveryType OrderConsigneeType { get; set; }

        /// <summary>
        /// 是否装箱
        /// </summary>
        public bool HasPacking { get; set; }

        /// <summary>
        /// 装箱状态（这里其实是为了区分"未封箱"、"已封箱"）
        /// </summary>
        public Enums.PackingStatus PackingStatus { get; set; }
    }

}
