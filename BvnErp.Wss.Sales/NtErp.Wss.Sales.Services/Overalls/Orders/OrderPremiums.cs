using NtErp.Wss.Sales.Services.Underly;
using NtErp.Wss.Sales.Services.Underly.Orders;
using NtErp.Wss.Sales.Services.Underly.Premiums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Overalls.Orders
{
    public class OrderPremiums : UnifyPremium<OrderPremium>
    {
        IEnumerable<OrderPremium> source;

        public IEnumerable<OrderPremium> Where(Source source, District district, Currency currency)
        {
            return this.source.Where(item =>
            //item.Source == source && 
                item.District == district
                && item.Currency == currency);
        }

        internal OrderPremiums()
        {
            using (var responsitory = new Layer.Data.Sqls.BvOtherReponsitory())
            {
                var linqs = from entity in responsitory.ReadTable<Layer.Data.Sqls.BvOthers.Premiums>()
                            select new OrderPremium
                            {
                                // Source = entity
                                Name = entity.Name,
                                Price = entity.Price,
                                District = (District)entity.District,
                                Currency = (Currency)entity.Currency,
                                Summary = entity.Summary
                            };

                this.source = linqs.ToArray();
            }
        }

        public override IEnumerator<OrderPremium> GetEnumerator()
        {
            return this.source.GetEnumerator();

        }
    }
}
