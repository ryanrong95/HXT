using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.PdaApi.Services.Models;

namespace Yahv.PsWms.PdaApi.Services.Views
{
    public class ProductsView : UniqueView<Product, PsWmsRepository>
    {
        public ProductsView()
        {
        }

        public ProductsView(PsWmsRepository reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Product> GetIQueryable()
        {
            var view = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PsWms.Products>()
                       select new Product
                       {
                           ID = entity.ID,
                           Partnumber = entity.Partnumber,
                           Brand = entity.Brand,
                           Package = entity.Package,
                           DateCode = entity.DateCode,
                           Mpq = entity.Mpq,
                           Moq = entity.Moq,
                           CreateDate = entity.CreateDate,
                           ModifyDate = entity.ModifyDate,
                       };

            return view;
        }
    }
}
