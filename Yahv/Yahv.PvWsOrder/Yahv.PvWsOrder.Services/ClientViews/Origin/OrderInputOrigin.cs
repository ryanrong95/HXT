using System.Linq;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.ClientModels;
using Layers.Data.Sqls;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class OrderInputOrigin : UniqueView<OrderInput, PvWsOrderReponsitory>
    {
        protected OrderInputOrigin()
        {

        }

        public OrderInputOrigin(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<OrderInput> GetIQueryable()
        {
            return from input in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderInputs>()
                   select new OrderInput
                   {
                       ID = input.ID,
                       IsPayCharge = input.IsPayCharge,
                       BeneficiaryID = input.BeneficiaryID,
                       WayBillID = input.WayBillID,
                       Currency = (Currency)input.Currency,
                       Conditions = input.Conditions,
                   };
        }
    }
}
