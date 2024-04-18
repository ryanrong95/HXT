using Needs.Wl.Models.Views;

namespace Needs.Wl.Models
{
    public static partial class ClientSuppliertExtends
    {
        /// <summary>
        /// 供应商的银行账号
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public static SupplierBanksView Banks(this Needs.Wl.Models.ClientSupplier supplier)
        {
            return new Views.SupplierBanksView(supplier.ID);
        }

        /// <summary>
        /// 供应商的提货地址
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        public static SupplierAddressesView Addresses(this Needs.Wl.Models.ClientSupplier supplier)
        {
            return new Views.SupplierAddressesView(supplier.ID);
        }
    }
}