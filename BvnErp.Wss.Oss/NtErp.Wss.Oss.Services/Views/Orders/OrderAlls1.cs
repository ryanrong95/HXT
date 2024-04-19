using NtErp.Wss.Oss.Services;
using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Views
{
    /// <summary>
    /// 订单总视图
    /// </summary>
    public class OrderAlls1 : UniqueView<Models.Order, CvOssReponsitory>
    {
        public OrderAlls1()
        {

        }

        internal OrderAlls1(CvOssReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected OrderAlls1(CvOssReponsitory reponsitory, IQueryable<Models.Order> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        public OrderAlls1 SearchByProductName(string name)
        {
            var linq = from prodcut in this.Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.StandardProducts>()
                       join item in this.Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.OrderItems>() on prodcut.ID equals item.ProductID
                       join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.Orders>() on item.OrderID equals order.ID
                       where prodcut.Name.StartsWith(name)
                       select order;

            return new OrderAlls1(this.Reponsitory, this.GetIQueryable(linq));
        }

        IQueryable<Models.Order> GetIQueryable(IQueryable<Layer.Data.Sqls.CvOss.Orders> ordersView)
        {
            var clientTopsView = new ClientsTopView(this.Reponsitory);
            var partiesView = new PartiesView(this.Reponsitory);
            var invoicesView = new InvoicesView(this.Reponsitory);
            var beneficiariesView = new BeneficiariesView(this.Reponsitory);
            var itemViews = new OrderItemsAlls(this.Reponsitory);
            var transportTermsView = new TransportTermsView(this.Reponsitory);
            var premiumsView = new PremiumsView(this.Reponsitory);


            var linq = from entity in ordersView
                       join client in clientTopsView on entity.ClientID equals client.ID
                       join consignee in partiesView on entity.ConsigneeID equals consignee.ID
                       join deliverer in partiesView on entity.DelivererID equals deliverer.ID
                       join invoice in invoicesView on entity.InvoiceID equals invoice.ID
                       join beneficiary in beneficiariesView on entity.BeneficiaryID equals beneficiary.ID
                       join transportTerm in transportTermsView on entity.ID equals transportTerm.ID
                       //一对一这样做没有问题

                       // 外键开发
                       join item in itemViews on entity.ID equals item.OrderID into items
                       join premium in premiumsView on entity.ID equals premium.OrderID into premiums

                       select new Models.Order
                       {
                           ID = entity.ID,
                           Type = (OrderType)entity.Type,
                           Status = (OrderStatus)entity.Status,
                           Summary = entity.Summary,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           Invoice = invoice,
                           Consignee = consignee,
                           Deliverer = deliverer,
                           Beneficiary = beneficiary,
                           Client = client,
                           TransportTerm = transportTerm,

                           //Premiums = new Models.Premiums(premiums),
                           Premiums = null,

                           Items = new Models.OrderItems(items),
                           //Items = null,

                           SendRate = entity.SendRate,

                           Paid = entity.Paid ?? 0m,
                           Total = entity.Total ?? 0m,
                       };

            return linq;
        }

        protected override IQueryable<Models.Order> GetIQueryable()
        {
            return this.GetIQueryable(this.Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.Orders>());
        }
    }

   
}
