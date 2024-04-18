using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.PsWms.SzMvc.Services.Models.Origin;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Services.Views.Origins
{
    /// <summary>
    /// 开票通知项
    /// </summary>
    public class InvoiceNoticesOrigin : UniqueView<InvoiceNotice,Layers.Data.Sqls.PsOrderRepository>
    {
        public InvoiceNoticesOrigin()
        {

        }

        internal InvoiceNoticesOrigin(Layers.Data.Sqls.PsOrderRepository reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<InvoiceNotice> GetIQueryable()
        {
            var linq = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsOrder.SzInvoiceNoticesTopView>()
                       select new InvoiceNotice
                       {
                           ID = entity.ID,
                           ClientID = entity.ClientID,
                           IsPersonal = entity.IsPersonal,
                           Type = (InvoiceType)entity.Type,
                           Title = entity.Title,
                           TaxNumber = entity.TaxNumber,
                           RegAddress = entity.RegAddress,
                           Tel = entity.Tel,
                           BankName = entity.BankName,
                           BankAccount = entity.BankAccount,
                           PostAddress = entity.PostAddress,
                           PostRecipient = entity.PostRecipient,
                           PostTel = entity.PostTel,
                           DeliveryType = (Underly.InvoiceDeliveryType)entity.DeliveryType,
                           WayBillCode = entity.WayBillCode,
                           Status = (InvoiceEnum)entity.Status,
                           InvoiceDate = entity.InvoiceDate,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           AdminID = entity.AdminID,
                           Summary = entity.Summary,

                           CarrierName=entity.Name,
                       };
            return linq;
        }
    }
}
