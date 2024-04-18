using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Services.Models.Origin
{
    public class OrderItem : IUnique
    {
        public string ID { get; set; }

        public string OrderID { get; set; }

        public string ProductID { get; set; }

        public string Supplier { get; set; }

        public string Origin { get; set; }

        public string CustomCode { get; set; }

        public Enums.StocktakingType StocktakingType { get; set; }

        public int Mpq { get; set; }

        public int PackageNumber { get; set; }

        public int Total { get; set; }

        public Currency Currency { get; set; }

        public decimal? UnitPrice { get; set; }

        public string StorageID { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }

        public GeneralStatus Status { get; set; }

        #region 扩展属性

        public Product Product { get; set; }

        #endregion
    }
}
