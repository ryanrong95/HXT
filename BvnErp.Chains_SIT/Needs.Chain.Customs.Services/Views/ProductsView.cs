//using Layer.Data.Sqls;
//using Needs.Ccs.Services.Models;
//using Needs.Linq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Needs.Ccs.Services.Views
//{
//    public class ProductsViews : UniqueView<Models.Product, ScCustomsReponsitory>
//    {
//        public ProductsViews()
//        {
//        }

//        public ProductsViews(ScCustomsReponsitory reponsitory) : base(reponsitory)
//        {

//        }

//        protected override IQueryable<Product> GetIQueryable()
//        {
//            return from product in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Products>()
//                   select new Product
//                   {
//                       ID = product.ID,
//                       Name = product.Name,
//                       Model = product.Model,
//                       Manufacturer = product.Manufacturer,
//                       Batch = product.Batch,
//                       Description = product.Description
//                   };
//        }

//    }
//}
