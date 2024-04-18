using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 业务员，订单管理，未收款
    /// </summary>
    public class OrderUnReceiveStatsViewNew : QueryView<OrderUnReceiveStatsViewNewModel, ScCustomsReponsitory>
    {
        public OrderUnReceiveStatsViewNew()
        {
        }

        protected OrderUnReceiveStatsViewNew(ScCustomsReponsitory reponsitory, IQueryable<OrderUnReceiveStatsViewNewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<OrderUnReceiveStatsViewNewModel> GetIQueryable()
        {
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>();

            var clientAdmins = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAdmins>();
            //var clientAgreements = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>();
            //var orderConsignees = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>();

            var orderReceipts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>()
                .Where(t => t.Status == (int)Enums.Status.Normal
                         && t.FeeType != (int)Enums.OrderFeeType.Product);

            var iQuery = from order in orders
                         join client in clients on order.ClientID equals client.ID
                         join company in companies on client.CompanyID equals company.ID

                         join salesadmin in clientAdmins.Where(t=>t.Status == (int)Enums.Status.Normal && t.Type == (int)Enums.ClientAdminType.ServiceManager) 
                         on order.ClientID equals salesadmin.ClientID into sale_temp 
                         from salesadmin in sale_temp.DefaultIfEmpty()

                         join meradmin in clientAdmins.Where(t => t.Status == (int)Enums.Status.Normal && t.Type == (int)Enums.ClientAdminType.Merchandiser)
                         on order.ClientID equals meradmin.ClientID into mer_temp
                         from meradmin in mer_temp.DefaultIfEmpty()
                             //join clientAgreement in clientAgreements on order.ClientAgreementID equals clientAgreement.ID
                             //join orderConsignee in orderConsignees on order.ID equals orderConsignee.OrderID

                         join decHead in decHeads on order.ID equals decHead.OrderID into decHeads2

                         join orderReceipt in orderReceipts on order.ID equals orderReceipt.OrderID into orderReceipt2
                         where order.Status == (int)Enums.Status.Normal
                            && order.OrderStatus >= (int)Enums.OrderStatus.Declared
                            && order.OrderStatus <= (int)Enums.OrderStatus.Completed
                            && client.Status == (int)Enums.Status.Normal
                            && company.Status == (int)Enums.Status.Normal

                           // && clientAdmin.Status == (int)Enums.Status.Normal
                            //&& clientAdmin.Type == (int)Enums.ClientAdminType.ServiceManager

                            && orderReceipt2.Any()
                            && orderReceipt2.Sum(r => r.Amount) >= 5
                         select new OrderUnReceiveStatsViewNewModel
                         {
                             ClientCode = client.ClientCode,
                             CompanyName = company.Name,
                             OrderId = order.ID,
                             OrderStatus = (Enums.OrderStatus)order.OrderStatus,
                             DeclareDate = decHeads2.OrderBy(t => t.CreateTime).FirstOrDefault().DDate,
                             DeclarePrice = order.DeclarePrice,

                             ServiceManagerOrginAdminID = salesadmin.AdminID,
                             MerchandiserOriginAdminID = meradmin.AdminID,
                             ClientAgreementID = order.ClientAgreementID,

                             RealExchangeRate = order.RealExchangeRate,
                             CustomsExchangeRate = order.CustomsExchangeRate,
                             DueDate = order.DueDate,
                         };

            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<OrderUnReceiveStatsViewNewModel> iquery = this.IQueryable.Cast<OrderUnReceiveStatsViewNewModel>().OrderByDescending(item => item.DeclareDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_orders = iquery.ToArray();

            //订单ID
            var orderIDs = ienum_orders.Select(item => item.OrderId);

            //ClientAgreementID
            var clientAgreementIDs = ienum_orders.Select(item => item.ClientAgreementID);

            #region 应收款、已收款、欠款

            var orderReceipts = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>();

            var linq_orderReceipt = from orderReceipt in orderReceipts
                                    where orderReceipt.Status == (int)Enums.Status.Normal
                                       && orderReceipt.FeeType != (int)Enums.OrderFeeType.Product
                                       && orderIDs.Contains(orderReceipt.OrderID)
                                    group orderReceipt by new { orderReceipt.OrderID, } into g
                                    select new
                                    {
                                        OrderID = g.Key.OrderID,
                                        //应收款
                                        Receivable = g.Where(r => r.Type == (int)Enums.OrderReceiptType.Receivable).Sum(r => (decimal?)r.Amount).GetValueOrDefault(),
                                        //已收款
                                        Received = g.Where(r => r.Type == (int)Enums.OrderReceiptType.Received).Sum(r => -(decimal?)r.Amount).GetValueOrDefault(),
                                        //欠款
                                        Overdraft = g.Sum(r => (decimal?)r.Amount).GetValueOrDefault(),
                                    };

            var ienums_orderReceipt = linq_orderReceipt.ToArray();

            #endregion

            #region 获取 ProductFeeClause 信息

            var clientAgreements = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>();
            var clientFeeSettlements = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientFeeSettlements>();

            var linq_productFeeSettlement = from clientAgreement in clientAgreements
                                            join clientFeeSettlement in clientFeeSettlements on clientAgreement.ID equals clientFeeSettlement.AgreementID
                                            where //clientAgreement.Status == (int)Enums.Status.Normal
                                               clientFeeSettlement.FeeType == (int)Enums.FeeType.Product
                                               && clientAgreementIDs.Contains(clientAgreement.ID)
                                            group new { clientAgreement, clientFeeSettlement, } by new { ClientAgreementID = clientAgreement.ID, } into g
                                            select new
                                            {
                                                ClientAgreementID = g.Key.ClientAgreementID, //clientAgreement.ID,
                                                ExchangeRateType = (Enums.ExchangeRateType)(g.OrderByDescending(t => t.clientFeeSettlement.CreateDate).FirstOrDefault().clientFeeSettlement.ExchangeRateType), //(Enums.ExchangeRateType)clientFeeSettlement.ExchangeRateType,
                                                ExchangeRateValue = g.OrderByDescending(t => t.clientFeeSettlement.CreateDate).FirstOrDefault().clientFeeSettlement.ExchangeRateValue, //clientFeeSettlement.ExchangeRateValue,
                                            };

            var ienums_productFeeSettlement = linq_productFeeSettlement.ToArray();

            #endregion

            var ienums_linq = from order in ienum_orders
                              join orderReceipt in ienums_orderReceipt on order.OrderId equals orderReceipt.OrderID into ienums_orderReceipt2
                              from orderReceipt in ienums_orderReceipt2.DefaultIfEmpty()
                              join productFeeSettlement in ienums_productFeeSettlement on order.ClientAgreementID equals productFeeSettlement.ClientAgreementID into ienums_productFeeSettlement2
                              from productFeeSettlement in ienums_productFeeSettlement2.DefaultIfEmpty()
                              orderby order.DeclareDate descending
                              select new OrderUnReceiveStatsViewNewModel
                              {
                                  ClientCode = order.ClientCode,
                                  CompanyName = order.CompanyName,
                                  OrderId = order.OrderId,
                                  OrderStatus = order.OrderStatus,
                                  DeclareDate = order.DeclareDate,
                                  DeclarePrice = order.DeclarePrice,

                                  //应收款
                                  Receivable = orderReceipt != null ? orderReceipt.Receivable : 0,
                                  //已收款
                                  Received = orderReceipt != null ? orderReceipt.Received : 0,
                                  //欠款
                                  Overdraft = orderReceipt != null ? orderReceipt.Overdraft : 0,


                                  ExchangeRateType = productFeeSettlement != null ? productFeeSettlement.ExchangeRateType : Enums.ExchangeRateType.Agreed,
                                  ExchangeRateValue = productFeeSettlement != null ? (productFeeSettlement.ExchangeRateValue.HasValue ? productFeeSettlement.ExchangeRateValue.Value : 0) : 0,
                                  RealExchangeRate = order.RealExchangeRate,
                                  CustomsExchangeRate = order.CustomsExchangeRate,
                              };

            var results = ienums_linq;

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            Func<OrderUnReceiveStatsViewNewModel, object> convert = item => new
            {
                ClientCode = item.ClientCode,
                CompanyName = item.CompanyName,
                OrderId = item.OrderId,
                OrderStatus = item.OrderStatus.GetDescription(),
                DeclareDate = item.DeclareDate?.ToShortDateString(),
                DeclarePrice = item.RMBDeclarePrice.ToRound(2).ToString("0.00"),
                Receivable = item.Receivable,
                Received = item.Received,
                Overdraft = item.Overdraft,
            };

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 根据客户编号查询
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public OrderUnReceiveStatsViewNew SearchByClientCode(string clientCode)
        {
            var linq = from query in this.IQueryable
                       where query.ClientCode.Contains(clientCode)
                       select query;

            var view = new OrderUnReceiveStatsViewNew(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据订单编号查询
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public OrderUnReceiveStatsViewNew SearchByOrderID(string orderID)
        {
            var linq = from query in this.IQueryable
                       where query.OrderId.Contains(orderID)
                       select query;

            var view = new OrderUnReceiveStatsViewNew(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据订单状态查询
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public OrderUnReceiveStatsViewNew SearchByOrderStatus(int OrderStatus)
        {
            var linq = from query in this.IQueryable
                       where query.OrderStatus == (Enums.OrderStatus)OrderStatus
                       select query;

            var view = new OrderUnReceiveStatsViewNew(this.Reponsitory, linq);
            return view;
        }
        /// <summary>
        /// 根据报关日期开始时间查询
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public OrderUnReceiveStatsViewNew SearchByDeclareDateBegin(DateTime begin)
        {
            var linq = from query in this.IQueryable
                       where query.DeclareDate >= begin
                       select query;

            var view = new OrderUnReceiveStatsViewNew(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据报关日期结束时间查询
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public OrderUnReceiveStatsViewNew SearchByDeclareDateEnd(DateTime end)
        {
            var linq = from query in this.IQueryable
                       where query.DeclareDate < end
                       select query;

            var view = new OrderUnReceiveStatsViewNew(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据业务员 OrginAdminID 查询
        /// </summary>
        /// <param name="serviceManagerOrginAdminID"></param>
        /// <returns></returns>
        public OrderUnReceiveStatsViewNew SearchByServiceManagerOrginAdminID(string serviceManagerOrginAdminID)
        {
            var linq = from query in this.IQueryable
                       where (query.ServiceManagerOrginAdminID == serviceManagerOrginAdminID
                       || query.MerchandiserOriginAdminID == serviceManagerOrginAdminID)
                       select query;

            var view = new OrderUnReceiveStatsViewNew(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据是否逾期查询
        /// </summary>
        /// <param name="isOverDue">true - 逾期, false - 不逾期</param>
        /// <returns></returns>
        public OrderUnReceiveStatsViewNew SearchByIsOverDue(bool isOverDue)
        {
            var linq = from query in this.IQueryable
                       select query;

            if (isOverDue)
            {
                linq = linq.Where(t => DateTime.Now > t.DueDate);
            }
            else
            {
                linq = linq.Where(t => DateTime.Now <= t.DueDate);
            }

            var view = new OrderUnReceiveStatsViewNew(this.Reponsitory, linq);
            return view;
        }
    }

    public class OrderUnReceiveStatsViewNewModel
    {
        /// <summary>
        /// 客户编号
        /// </summary>
        public string ClientCode { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public Enums.OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DeclareDate { get; set; }

        /// <summary>
        /// 报关货值
        /// </summary>
        public decimal DeclarePrice { get; set; }

        /// <summary>
        /// 应收款
        /// </summary>
        public decimal Receivable { get; set; }

        /// <summary>
        /// 已收款
        /// </summary>
        public decimal Received { get; set; }

        /// <summary>
        /// 欠款
        /// </summary>
        public decimal Overdraft { get; set; }

        /// <summary>
        /// 业务员 OrginAdminID
        /// </summary>
        public string ServiceManagerOrginAdminID { get; set; }

        /// <summary>
        /// 跟单员
        /// </summary>
        public string MerchandiserOriginAdminID { get; set; }


        /// <summary>
        /// ClientAgreementID
        /// </summary>
        public string ClientAgreementID { get; set; }

        /// <summary>
        /// 费用使用的汇率类型
        /// </summary>
        public Enums.ExchangeRateType ExchangeRateType { get; set; }

        /// <summary>
        /// 约定汇率的值
        /// </summary>
        public decimal? ExchangeRateValue { get; set; }

        /// <summary>
        /// 实时汇率
        /// </summary>
        internal decimal? RealExchangeRate { get; set; }

        /// <summary>
        /// 海关汇率
        /// </summary>
        internal decimal? CustomsExchangeRate { get; set; }

        /// <summary>
        /// 货款汇率
        /// </summary>
        private decimal ProductFeeExchangeRate
        {
            get
            {
                decimal productFeeExchangeRate = 0;

                var exchangeRateType = this.ExchangeRateType;
                switch (exchangeRateType)
                {
                    case Enums.ExchangeRateType.RealTime:
                        productFeeExchangeRate = this.RealExchangeRate.HasValue ? this.RealExchangeRate.Value : 0;
                        break;
                    case Enums.ExchangeRateType.Custom:
                        productFeeExchangeRate = this.CustomsExchangeRate.HasValue ? this.CustomsExchangeRate.Value : 0;
                        break;
                    case Enums.ExchangeRateType.Agreed:
                        productFeeExchangeRate = this.ExchangeRateValue.HasValue ? this.ExchangeRateValue.Value : 0;
                        break;
                    default:
                        productFeeExchangeRate = 0;
                        break;
                }

                return productFeeExchangeRate;
            }
        }

        /// <summary>
        /// 报关货值(人民币)
        /// </summary>
        public decimal RMBDeclarePrice
        {
            get
            {
                return this.DeclarePrice * this.ProductFeeExchangeRate;
            }
        }

        /// <summary>
        /// 逾期时间
        /// </summary>
        public DateTime? DueDate { get; set; }

    }

}
