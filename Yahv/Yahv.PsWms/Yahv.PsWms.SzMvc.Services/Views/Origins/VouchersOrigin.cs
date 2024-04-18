using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;

namespace Yahv.PsWms.SzMvc.Services.Views.Origins
{

    public class VouchersOrigin : UniqueView<Models.Origin.Voucher, PsOrderRepository>
    {
        #region 构造函数
        public VouchersOrigin()
        {
        }

        public VouchersOrigin(PsOrderRepository reponsitory) : base(reponsitory)
        {
        }

        #endregion

        protected override IQueryable<Models.Origin.Voucher> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.Vouchers>()
                       select new Models.Origin.Voucher
                       {
                           ID = entity.ID,
                           PayerID = entity.PayerID,
                           PayeeID = entity.PayeeID,
                           Type = (VoucherType)entity.Type,
                           Mode = (VoucherMode)entity.Mode,
                           CreateDate = entity.CreateDate,
                           Summary = entity.Summary,
                           CutDate = entity.CutDate,
                           CutDateIndex = entity.CutDateIndex,
                           IsInvoiced = entity.IsInvoiced,
                       };
            return view;
        }
    }
}
