using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Linq;
using Yahv.Linq.Generic;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views
{
    /// <summary>
    /// 代仓储客户-租赁订单视图
    /// </summary>
    public class LsOrdersAll : UniqueView<LsOrder, PvWsOrderReponsitory>
    {
        public LsOrdersAll()
        {

        }

        public LsOrdersAll(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<LsOrder> GetIQueryable()
        {
            var Clients = new Yahv.Services.Views.WsClientsTopView<PvWsOrderReponsitory>(this.Reponsitory);
            var LsOrders = new Yahv.Services.Views.LsOrderTopView<PvWsOrderReponsitory>(this.Reponsitory)
                .Where(en => en.Type == LsOrderType.Lease && en.Status != LsOrderStatus.Closed);

            var linq = from entity in LsOrders
                       join client in Clients on entity.ClientID equals client.ID
                       select new LsOrder
                       {
                           ID = entity.ID,
                           Type = entity.Type,
                           Source = entity.Source,
                           ClientID = entity.ClientID,
                           PayeeID = entity.PayeeID,
                           BeneficiaryID = entity.BeneficiaryID,
                           Currency = entity.Currency,
                           InvoiceID = entity.InvoiceID,
                           StartDate = entity.StartDate,
                           EndDate = entity.EndDate,
                           Status = entity.Status,
                           Creator = entity.Creator,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Summary = entity.Summary,
                           InvoiceStatus = entity.InvoiceStatus,
                           FatherID = entity.FatherID,
                           InheritStatus = entity.InheritStatus,
                           IsInvoiced = entity.IsInvoiced,
                           wsClient = client,
                       };
            return linq;
        }
    }

    /// <summary>
    /// 租赁订单分页视图
    /// </summary>
    public class BaseLsOrderView : QueryRoll<LsOrder, LsOrder, PvWsOrderReponsitory>
    {
        //系统管理员
        private Underly.Erps.IErpAdmin admin;
        //主体公司
        private string CompanyID;
        public BaseLsOrderView()
        {

        }
        public BaseLsOrderView(Underly.Erps.IErpAdmin admin, string companyID)
        {
            this.admin = admin;
            this.CompanyID = companyID;
        }

        protected override IQueryable<LsOrder> GetIQueryable(Expression<Func<LsOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            var ordersView = new LsOrdersAll(this.Reponsitory).OrderByDescending(item => item.CreateDate).Where(order => order.PayeeID == this.CompanyID);
            if (!admin.IsSuper)
            {
                //管理员的代仓储客户
                var clientIds = new Yahv.Services.Views.TrackerWsClients<PvWsOrderReponsitory>(this.Reponsitory, this.admin, this.CompanyID)
                    .Select(item => item.ID).ToArray();
                ordersView = ordersView.Where(item => clientIds.Contains(item.ClientID));
            }

            var linq = from entity in ordersView
                       select new LsOrder
                       {
                           ID = entity.ID,
                           Type = entity.Type,
                           Source = entity.Source,
                           ClientID = entity.ClientID,
                           PayeeID = entity.PayeeID,
                           BeneficiaryID = entity.BeneficiaryID,
                           Currency = entity.Currency,
                           InvoiceID = entity.InvoiceID,
                           StartDate = entity.StartDate,
                           EndDate = entity.EndDate,
                           Status = entity.Status,
                           Creator = entity.Creator,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Summary = entity.Summary,
                           InvoiceStatus = entity.InvoiceStatus,
                           FatherID = entity.FatherID,
                           InheritStatus = entity.InheritStatus,
                           IsInvoiced = entity.IsInvoiced,
                           wsClient = entity.wsClient,
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<LsOrder, bool>>);
            }
            return linq.Where(expression);
        }

        protected override IEnumerable<LsOrder> OnReadShips(LsOrder[] results)
        {
            var linq = from entity in results
                       select new LsOrder
                       {
                           ID = entity.ID,
                           Type = entity.Type,
                           Source = entity.Source,
                           ClientID = entity.ClientID,
                           PayeeID = entity.PayeeID,
                           BeneficiaryID = entity.BeneficiaryID,
                           Currency = entity.Currency,
                           InvoiceID = entity.InvoiceID,
                           StartDate = entity.StartDate,
                           EndDate = entity.EndDate,
                           Status = entity.Status,
                           Creator = entity.Creator,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                           Summary = entity.Summary,
                           FatherID = entity.FatherID,
                           InheritStatus = entity.InheritStatus,
                           IsInvoiced = entity.IsInvoiced,
                           wsClient = entity.wsClient,
                       };
            return linq;
        }
    }

    /// <summary>
    /// 某客户的租赁订单
    /// </summary>
    public class ClientLsOrders : LsOrdersAll
    {
        string ClientID;

        public ClientLsOrders(string clientID)
        {
            this.ClientID = clientID;
        }

        protected override IQueryable<LsOrder> GetIQueryable()
        {
            var linq = base.GetIQueryable().Where(item => item.ClientID == this.ClientID);
            return linq;
        }
    }
}
