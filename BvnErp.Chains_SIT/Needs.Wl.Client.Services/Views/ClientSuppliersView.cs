using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.Models;
using System.Linq;

namespace Needs.Wl.Client.Services.Views
{
    /// <summary>
    /// 客户的供应商
    /// </summary>
    public class ClientSuppliersView : View<Needs.Wl.Models.ClientSupplier, ScCustomsReponsitory>
    {
        private string ClientID;

        public ClientSuppliersView(string clientID)
        {
            this.ClientID = clientID;
        }

        protected override IQueryable<ClientSupplier> GetIQueryable()
        {
            return from supplier in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>()
                   where supplier.ClientID == this.ClientID && supplier.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                   select new Needs.Wl.Models.ClientSupplier
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