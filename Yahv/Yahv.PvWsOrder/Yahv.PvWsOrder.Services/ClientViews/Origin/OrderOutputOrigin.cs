using System.Linq;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.ClientModels;
using Layers.Data.Sqls;


namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class OrderOutputOrigin : UniqueView<OrderOutput, PvWsOrderReponsitory>
    {
        protected OrderOutputOrigin()
        {

        }

        public OrderOutputOrigin(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<OrderOutput> GetIQueryable()
        {
            return from output in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderOutputs>()
                   select new OrderOutput
                   {
                       ID = output.ID,
                       IsReciveCharge = output.IsReciveCharge,
                       BeneficiaryID = output.BeneficiaryID,
                       WayBillID = output.WayBillID,
                       Conditions = output.Conditions,
                   };
        }
    }
}
