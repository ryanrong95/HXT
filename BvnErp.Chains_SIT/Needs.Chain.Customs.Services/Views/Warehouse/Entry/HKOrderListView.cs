using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 香港库房 查看订单列表（外单）
    /// </summary>
    public class HKOrderListView
    {
        private ScCustomsReponsitory _reponsitory;

        public HKOrderListView()
        {
            _reponsitory = new ScCustomsReponsitory();
        }

        public HKOrderListView(ScCustomsReponsitory reponsitory)
        {
            _reponsitory = reponsitory;
        }

        public class HKOrderModel
        {
            /// <summary>
            /// 订单编号
            /// </summary>
            public string OrderID { get; set; } = string.Empty;

            /// <summary>
            /// 入仓号
            /// </summary>
            public string ClientCode { get; set; } = string.Empty;

            /// <summary>
            /// 客户名称
            /// </summary>
            public string ClientName { get; set; } = string.Empty;

            /// <summary>
            /// 订单状态
            /// </summary>
            public Enums.OrderStatus OrderStatus { get; set; }

            /// <summary>
            /// 交货方式
            /// </summary>
            public Enums.HKDeliveryType OrderConsigneeType { get; set; }

            /// <summary>
            /// 供应商
            /// </summary>
            public string ClientSupplierName { get; set; } = string.Empty;
        }

        public class ClientModel
        {
            public string ClientID { get; set; } = string.Empty;

            public Enums.Status Status { get; set; }

            public Enums.ClientType ClientType { get; set; }

            public string CompanyID { get; set; } = string.Empty;

            public string ClientCode { get; set; } = string.Empty;
        }

        private IEnumerable<HKOrderModel> GetHKOrderModelBase(params LambdaExpression[] expressions)
        {
            var orders = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var orderConsignees = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>();
            var clientSuppliers = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>();

            //ID    Status    ClientType      CompanyID    ClientCode
            var externalClientView = from client in clients
                                     where client.ClientType != null && client.Status == (int)Enums.Status.Normal
                                     select new ClientModel
                                     {
                                         ClientID = client.ID,
                                         Status = (Enums.Status)client.Status,
                                         ClientType = (Enums.ClientType)client.ClientType,
                                         CompanyID = client.CompanyID,
                                         ClientCode = client.ClientCode,
                                     };

            var baseResults = from order in orders
                              join client in externalClientView on new { ClientID = order.ClientID, ClientStatus = Enums.Status.Normal, ClientType = Enums.ClientType.External, }
                                   equals new { ClientID = client.ClientID, ClientStatus = client.Status, ClientType = client.ClientType, }
                              join company in companies on new { CompanyID = client.CompanyID, CompanyStatus = (int)Enums.Status.Normal, }
                                  equals new { CompanyID = company.ID, CompanyStatus = company.Status, }
                              join orderConsignee in orderConsignees on new { OrderID = order.ID, OrderConsigneeStatus = (int)Enums.Status.Normal, }
                                   equals new { OrderID = orderConsignee.OrderID, OrderConsigneeStatus = orderConsignee.Status, }
                              join clientSupplier in clientSuppliers on new { ClientSupplierID = orderConsignee.ClientSupplierID, ClientSupplierStatus = (int)Enums.Status.Normal, }
                                   equals new { ClientSupplierID = clientSupplier.ID, ClientSupplierStatus = clientSupplier.Status, }
                              orderby order.CreateDate descending
                              select new HKOrderModel
                              {
                                  OrderID = order.ID,
                                  ClientCode = client.ClientCode,
                                  ClientName = company.Name,
                                  OrderStatus = (Enums.OrderStatus)order.OrderStatus,
                                  OrderConsigneeType = (Enums.HKDeliveryType)orderConsignee.Type,
                                  ClientSupplierName = clientSupplier.ChineseName,
                              };

            foreach (var expression in expressions)
            {
                baseResults = baseResults.Where(expression as Expression<Func<HKOrderModel, bool>>);
            }

            return baseResults;
        }

        public IEnumerable<HKOrderModel> GetHKOrderModelResult(out int total, int page, int rows, params LambdaExpression[] expressions)
        {
            var baseView = GetHKOrderModelBase(expressions);

            total = baseView.Count();

            var results = baseView.Skip(rows * (page - 1)).Take(rows).ToList();

            return results;
        }
    }
}
