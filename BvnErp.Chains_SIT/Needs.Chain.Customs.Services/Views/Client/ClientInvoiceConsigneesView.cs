using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Needs.Ccs.Services.Views
{
    public class ClientInvoiceConsigneesView : UniqueView<ClientInvoiceConsignee, ScCustomsReponsitory>
    {
        public ClientInvoiceConsigneesView()
        {
        }

        internal ClientInvoiceConsigneesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<ClientInvoiceConsignee> GetIQueryable()
        {
            return from consignee in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientInvoiceConsignees>()
                   select new Models.ClientInvoiceConsignee
                   {
                       ID = consignee.ID,
                       ClientID = consignee.ClientID,
                       Name = consignee.Name,
                       Mobile = consignee.Mobile,
                       Tel = consignee.Tel,
                       Email = consignee.Email,
                       Address = consignee.Address,
                       Status = (Enums.Status)consignee.Status,
                       CreateDate = consignee.CreateDate,
                       UpdateDate = consignee.UpdateDate,
                       Summary = consignee.Summary
                   };
        }
    }
}
