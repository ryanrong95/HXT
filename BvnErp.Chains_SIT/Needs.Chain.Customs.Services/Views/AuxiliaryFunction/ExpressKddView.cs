using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ExpressKddView : UniqueView<Models.ExpressKdd, ScCustomsReponsitory>
    {
        public ExpressKddView()
        {
        }

        internal ExpressKddView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ExpressKdd> GetIQueryable()
        {
            var expressCompanyView = new ExpressCompanyView(this.Reponsitory);
            var expressTypeView = new ExpressTypeView(this.Reponsitory);

            return from expressage in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExpressKdds>()
                   join expressCompany in expressCompanyView on expressage.ExpressCompanyID equals expressCompany.ID
                   join expressType in expressTypeView on expressage.ExpressTypeID equals expressType.ID
                   where expressage.Status == (int)Enums.Status.Normal
                   select new Models.ExpressKdd
                   {
                       ID = expressage.ID,
                       SenderComp = expressage.SenderComp,
                       Sender = expressage.Sender,
                       SenderMobile = expressage.SenderMobile,
                       SenderAddress = expressage.SenderAddress,
                       ReceiverComp = expressage.ReceiverComp,
                       Receiver = expressage.Receiver,
                       ReveiveMobile = expressage.ReveiveMobile,
                       ReveiveAddress = expressage.ReveiveAddress,
                       ExpressCompany = expressCompany,
                       ExpressType = expressType,
                       PayType = (Enums.PayType)expressage.PayType,
                       WaybillCode = expressage.WaybillCode,
                       Status = (Enums.Status)expressage.Status,
                       CreateDate = expressage.CreateDate,
                       UpdateDate = expressage.UpdateDate,
                   };
        }
    }
}
