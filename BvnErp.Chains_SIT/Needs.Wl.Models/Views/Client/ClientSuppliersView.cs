using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    public class ClientSuppliersView : View<Models.ClientSupplier, ScCustomsReponsitory>
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
                       Status = supplier.Status,
                       CreateDate = supplier.CreateDate,
                       UpdateDate = supplier.UpdateDate,
                       Summary = supplier.Summary
                   };
        }
    }

    public class MySuppliersView : ClientSuppliersView
    {
        private string ClientID;

        public MySuppliersView(string clientID)
        {
            this.ClientID = clientID;
        }

        protected override IQueryable<Models.ClientSupplier> GetIQueryable()
        {
            return from supplier in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>()
                   where supplier.ClientID == this.ClientID && supplier.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                   select new Models.ClientSupplier
                   {
                       ID = supplier.ID,
                       ClientID = supplier.ClientID,
                       Name = supplier.Name,
                       ChineseName = supplier.ChineseName,
                       Status = supplier.Status,
                       CreateDate = supplier.CreateDate,
                       UpdateDate = supplier.UpdateDate,
                       Summary = supplier.Summary
                   };
        }
    }
}