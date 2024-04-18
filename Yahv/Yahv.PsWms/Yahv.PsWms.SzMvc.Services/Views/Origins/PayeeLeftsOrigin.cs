using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;

namespace Yahv.PsWms.SzMvc.Services.Views.Origins
{

    public class PayeeLeftsOrigin : UniqueView<Models.Origin.PayeeLeft, PsOrderRepository>
    {
        #region 构造函数
        public PayeeLeftsOrigin()
        {
        }

        public PayeeLeftsOrigin(PsOrderRepository reponsitory) : base(reponsitory)
        {
        }

        #endregion

        protected override IQueryable<Models.Origin.PayeeLeft> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.PayeeLefts>()
                       select new Models.Origin.PayeeLeft
                       {
                           ID = entity.ID,
                           Source = (AccountSource)entity.Source,
                           PayerID = entity.PayerID,
                           PayeeID = entity.PayeeID,
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

    public class PayeeRightsOrigin : UniqueView<Models.Origin.PayeeRight, PsOrderRepository>
    {
        #region 构造函数
        public PayeeRightsOrigin()
        {
        }

        public PayeeRightsOrigin(PsOrderRepository reponsitory) : base(reponsitory)
        {
        }

        #endregion

        protected override IQueryable<Models.Origin.PayeeRight> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.PayeeRights>()
                       select new Models.Origin.PayeeRight
                       {
                           ID = entity.ID,
                           LeftID = entity.LeftID,
                           Currency = (Underly.Currency)entity.Currency,
                           Price = entity.Price,
                           ReviewerID = entity.ReviewerID,
                           FlowFormCode = entity.FlowFormCode,
                           CreateDate = entity.CreateDate,
                           AdminID = entity.AdminID,
                       };
            return view;
        }
    }
}
