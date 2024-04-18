using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    public class ClientInvoiceConsigneesView : View<ClientInvoiceConsignee, ScCustomsReponsitory>
    {
        private string ClientID;

        public ClientInvoiceConsigneesView(string clientID)
        {
            this.ClientID = clientID;
        }

        internal ClientInvoiceConsigneesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<ClientInvoiceConsignee> GetIQueryable()
        {
            return from consignee in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientInvoiceConsignees>()
                   where consignee.ClientID == this.ClientID && consignee.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                   select new Models.ClientInvoiceConsignee
                   {
                       ID = consignee.ID,
                       ClientID = consignee.ClientID,
                       Name = consignee.Name,
                       Mobile = consignee.Mobile,
                       Tel = consignee.Tel,
                       Email = consignee.Email,
                       Address = consignee.Address,
                       Status = consignee.Status,
                       CreateDate = consignee.CreateDate,
                       UpdateDate = consignee.UpdateDate,
                       Summary = consignee.Summary
                   };
        }
    }
}