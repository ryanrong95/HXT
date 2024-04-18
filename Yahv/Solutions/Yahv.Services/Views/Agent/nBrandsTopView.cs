using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Services.Views
{
    
    public class nBrandsTopView<TReponsitory> : UniqueView<nBrand, TReponsitory>
          where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public nBrandsTopView()
        {

        }
        public nBrandsTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }
        protected override IQueryable<nBrand> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.nBrandsTopView>() 
                   select new nBrand
                   {
                       ID = entity.ID,
                       BrandID=entity.BrandID,
                       BrandName = entity.BrandName,
                       SupplierID = entity.SupplierID,
                       SupplierName = entity.SupplierName,
                   };
        }
    }
}
