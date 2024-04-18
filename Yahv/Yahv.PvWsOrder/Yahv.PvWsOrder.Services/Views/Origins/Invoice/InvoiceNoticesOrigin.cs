using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views.Origins
{
    /// <summary>
    /// 开票通知项
    /// </summary>
    public class InvoiceNoticesOrigin : UniqueView<InvoiceNotice, PvWsOrderReponsitory>
    {
        public InvoiceNoticesOrigin()
        {

        }

        internal InvoiceNoticesOrigin(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<InvoiceNotice> GetIQueryable()
        {
            var carrierView = this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Carriers>();
            var linq = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.InvoiceNotices>()
                       join carrier in carrierView on entity.Carrier equals carrier.ID into carriers
                       from carrier in carriers.DefaultIfEmpty()
                       select new InvoiceNotice
                       {
                           ID = entity.ID,
                           ClientID = entity.ClientID,
                           IsPersonal = entity.IsPersonal,
                           FromType = (InvoiceFromType)entity.FromType,
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
                           DeliveryType = (InvoiceDeliveryType)entity.DeliveryType,
                           WayBillCode = entity.WayBillCode,
                           Status = (InvoiceEnum)entity.Status,
                           InvoiceDate = entity.InvoiceDate,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           AdminID = entity.AdminID,
                           Summary = entity.Summary,

                           Carrier = carrier == null ? "" : carrier.Name,
                       };
            return linq;
        }
    }
}
