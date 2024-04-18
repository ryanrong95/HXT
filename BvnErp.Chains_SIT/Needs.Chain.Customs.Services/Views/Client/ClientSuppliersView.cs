using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ClientSuppliersView : UniqueView<Models.ClientSupplier, ScCustomsReponsitory>
    {
        public ClientSuppliersView()
        {

        }
        internal ClientSuppliersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.ClientSupplier> GetIQueryable()
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
                       SupplierGrade=supplier.Grade==null?Enums.SupplierGrade.First :(Enums.SupplierGrade)supplier.Grade,
                       Place=supplier.Place,
                      
                   };
        }
    }

    /// <summary>
    /// 客户的供应商
    /// </summary>
    public sealed class ClientSuppliers : ClientSuppliersView
    {
        Models.Client Client;

        public ClientSuppliers(Models.Client client)
        {
            this.Client = client;
        }

        protected override IQueryable<Models.ClientSupplier> GetIQueryable()
        {
            return from supplier in base.GetIQueryable()
                   where supplier.ClientID == this.Client.ID && supplier.Status == Enums.Status.Normal
                   select supplier;
        }
    }
}