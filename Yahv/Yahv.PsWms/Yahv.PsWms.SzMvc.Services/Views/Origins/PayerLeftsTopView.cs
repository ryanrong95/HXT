using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;

namespace Yahv.PsWms.SzMvc.Services.Views.Origins
{

    public class PayerLeftsTopView : UniqueView<Models.Origin.PayerLeft, PsOrderRepository>
    {
        #region 构造函数
        public PayerLeftsTopView()
        {
        }

        public PayerLeftsTopView(PsOrderRepository reponsitory) : base(reponsitory)
        {
        }

        #endregion

        protected override IQueryable<Models.Origin.PayerLeft> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.PayerLeftsTopView>()
                       select new Models.Origin.PayerLeft
                       {
                           ID = entity.ID,
                           Source = (AccountSource)entity.Source,
                           PayerID = entity.PayerID,
                           PayeeID = entity.PayeeID,
                           TakerID = entity.TakerID,
                           Conduct = (Conduct)entity.Conduct,
                           Subject = entity.Subject,
                           Currency = (Underly.Currency)entity.Currency,
                           Quantity = entity.Quantity,
                           UnitPrice = entity.UnitPrice,
                           Unit = entity.Unit,
                           Total = entity.Total,
                           CreateDate = entity.CreateDate,
                           CutDate = entity.CutDate,
                           CutDateIndex = entity.CutDateIndex,
                           NoticeID = entity.NoticeID,
                           FormID = entity.FormID,
                           WaybillCode = entity.WaybillCode,
                           AdminID = entity.AdminID,
                       };
            return view;
        }
    }
}
