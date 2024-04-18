using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Client.Services.Views
{
    /// <summary>
    /// 客户的预归类产品
    /// 不包含归类信息
    /// </summary>
    public class ClientPreProductsView : View<Needs.Wl.Models.PreProduct, ScCustomsReponsitory>
    {
        private string ClientID;

        public ClientPreProductsView(string clientID)
        {
            this.ClientID = clientID;
        }

        protected override IQueryable<Needs.Wl.Models.PreProduct> GetIQueryable()
        {
            return from product in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PreProducts>()
                   join clients in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on product.ClientID equals clients.ID
                   where product.ClientID == this.ClientID && product.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                   select new Needs.Wl.Models.PreProduct
                   {
                       ID = product.ID,
                       ClientID = product.ClientID,
                       ProductUnionCode = product.ProductUnionCode,
                       Model = product.Model,
                       Manufacturer = product.Manufacturer,
                       BatchNo = product.BatchNo,
                       Price = product.Price,
                       Currency = product.Currency,
                       Supplier = product.Supplier,
                       Status = (int)product.Status,
                       CreateDate = product.CreateDate,
                       UpdateDate = product.UpdateDate
                   };
        }

        /// <summary>
        /// 根据物料号查询
        /// </summary>
        /// <param name="unionCode"></param>
        /// <returns></returns>
        public Needs.Wl.Models.PreProduct FindByUnionCode(string unionCode)
        {
            return this.GetIQueryable().Where(s => s.ProductUnionCode == unionCode).FirstOrDefault();
        }
    }
}