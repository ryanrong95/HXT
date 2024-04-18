using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Services.Views
{
    public class vBrandsTopView<TReponsitory> : UniqueView<vBrand, TReponsitory>
          where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public vBrandsTopView()
        {

        }
        public vBrandsTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }
        protected override IQueryable<vBrand> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.vBrandsTopView>()
                   select new vBrand
                   {
                       ID = entity.ID,
                       BrandID = entity.BrandID,
                       BrandName = entity.BrandName,
                       AdminID =entity.AdminID
                   };
        }
    }
}
