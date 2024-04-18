using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Client.Services.Views
{
    /// <summary>
    /// 客户的产品信息
    /// </summary>
    public class ClientProductsView : View<Needs.Wl.Models.ClientProducts, ScCustomsReponsitory>
    {
        private string ClientID;

        public ClientProductsView(string clientID)
        {
            this.ClientID = clientID;
        }

        protected override IQueryable<Needs.Wl.Models.ClientProducts> GetIQueryable()
        {
            return from product in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientProducts>()
                   where product.ClientID == this.ClientID && product.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                   select new Needs.Wl.Models.ClientProducts
                   {
                       ID = product.ID,
                       ClientID = product.ClientID,
                       Name = product.Name,
                       Model = product.Model,
                       Manufacturer = product.Manufacturer,
                       Batch = product.Batch,
                       UpdateDate = product.UpdateDate,
                       CreateDate = product.CreateDate,
                       Status = product.Status
                   };
        }
    }
}