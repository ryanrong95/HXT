using Layers.Data.Sqls;
using System.Linq;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;

namespace Yahv.PsWms.SzMvc.Services.Views.Origins
{

    public class InvoicesOrigin : UniqueView<Models.Origin.Invoice, PsOrderRepository>
    {
        #region 构造函数
        public InvoicesOrigin()
        {
        }

        public InvoicesOrigin(PsOrderRepository reponsitory) : base(reponsitory)
        {
        }

        #endregion

        protected override IQueryable<Models.Origin.Invoice> GetIQueryable()
        {
            var view = from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.Invoices>()
                       select new Models.Origin.Invoice
                       {
                           ID = entity.ID,
                           ClientID = entity.ClientID,
                           Name = entity.Name,
                           TaxNumber = entity.TaxNumber,
                           RegAddress = entity.RegAddress,
                           Tel = entity.Tel,
                           BankName = entity.BankName,
                           BankAccount = entity.BankAccount,
                           DeliveryType = (Underly.InvoiceDeliveryType)entity.DeliveryType,
                           RevAddress = entity.RevAddress,
                           Contact = entity.Contact,
                           Phone = entity.Phone,
                           Email = entity.Email,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                       };
            return view;
        }
    }
}
