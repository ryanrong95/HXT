using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;
using System.Linq.Expressions;

namespace Needs.Ccs.Services.Views.Alls
{
    public class HKOrdersAll : Needs.Linq.Generic.Unique1Classics<HKOrder, ScCustomsReponsitory>
    {
        public HKOrdersAll()
        {
        }

        internal HKOrdersAll(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<HKOrder> GetIQueryable(Expression<Func<HKOrder, bool>> expression, params LambdaExpression[] expressions)
        {
            var clients = new Rolls.ClientsRoll(this.Reponsitory).Where(c => c.Status == Enums.Status.Normal && c.ClientType == Enums.ClientType.External);
            var orders = new Origins.OrdersOrigin(this.Reponsitory);
            var linq = from order in orders
                       join client in clients on order.ClientID equals client.ID
                       where order.Status == Enums.Status.Normal
                       orderby order.CreateDate descending
                       select new HKOrder
                       {
                           ID = order.ID,
                           ClientCode = client.ClientCode,
                           ClientName = client.Company.Name,
                           DeclarePrice = order.DeclarePrice,
                           Currency = order.Currency,
                           OrderStatus = order.OrderStatus,
                           CreateDate = order.CreateDate
                       };

            foreach (var predicate in expressions)
            {
                linq = linq.Where(predicate as Expression<Func<HKOrder, bool>>);
            }

            return linq.Where(expression);
        }

        protected override IEnumerable<HKOrder> OnReadShips(HKOrder[] results)
        {
            var orderIds = results.Select(r => r.ID).ToArray();
            var orderConsignees = new Origins.OrderConsigneesOrigin(this.Reponsitory).Where(oc => orderIds.Contains(oc.OrderID)).ToArray();

            return from result in results
                   join consignee in orderConsignees on result.ID equals consignee.OrderID
                   select new HKOrder
                   {
                       ID = result.ID,
                       ClientCode = result.ClientCode,
                       ClientName = result.ClientName,
                       DeclarePrice = result.DeclarePrice,
                       Currency = result.Currency,
                       OrderStatus = result.OrderStatus,
                       HKDeliveryType = consignee.Type,
                       CreateDate = result.CreateDate
                   };
        }
    }

    /// <summary>
    /// 香港库房报表 -> 报关订单
    /// </summary>
    public class HKOrder : Linq.IUnique
    {
        public string ID { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public decimal DeclarePrice { get; set; }
        public string Currency { get; set; }
        public Enums.HKDeliveryType HKDeliveryType { get; set; }
        public Enums.OrderStatus OrderStatus { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
