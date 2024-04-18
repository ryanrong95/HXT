using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otr.Suppliers
{
    public interface IProduct : IStandardProduct, IProductDetail, IInventory
    {
        ICatalogues Catalogue { get; set; }

        //IInventory[] Inventories { get; set; }
    }
}
