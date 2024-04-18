using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Enums;

namespace Needs.Ccs.Services.Views.Origins
{
    internal class ClientSuppliersOrigin : UniqueView<Models.ClientSupplier, ScCustomsReponsitory>
    {
        public ClientSuppliersOrigin()
        {

        }
        internal ClientSuppliersOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<ClientSupplier> GetIQueryable()
        {
            return from supplier in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>()
                   select new Models.ClientSupplier
                   {
                       ID = supplier.ID,
                       ClientID = supplier.ClientID,
                       Name = supplier.Name,
                       ChineseName = supplier.ChineseName,
                       Status = (Enums.Status)supplier.Status,
                       CreateDate = supplier.CreateDate,
                       UpdateDate = supplier.UpdateDate,
                       Summary = supplier.Summary,
                       SupplierGrade=(SupplierGrade)supplier.Grade,
                       Place=supplier.Place,
                   };
        }
    }
}
