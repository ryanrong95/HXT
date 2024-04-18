using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otr.Suppliers
{
    public interface ICatalogues
    {
        string Manufacture { get; set; }
        string Supplier { get; set; }
        string HSCode { get; set; }
        string TaxCode { get; set; }
    }


    public interface ICustoms
    {
        string Embargo { get; set; }
        string Inspection { get; set; }
        string this[string index] { get; set; }
    }
}
