using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.CrmPlus.Service.Views
{
    public class CompanynBrand: Linq.IDataEntity, IUnique
    {
        /// <summary>
        ///公司ID
        /// </summary>
        public string ID { get; set; }
        public string CompanyName { get; set; }
        public string nBrandID { get; set; }

        public string BrandID { get; set; }

        public string BrandName { get; set; }

    }


    public class CompanynBrandView : UniqueView<CompanynBrand, PvdCrmReponsitory>
    {
        public CompanynBrandView()
        {

        }
        internal CompanynBrandView(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }


        protected override IQueryable<CompanynBrand> GetIQueryable()
        {
            var companyView = new Origins.CompaniesOrigin(this.Reponsitory);
            var nbrandView = new Origins.nBrandOrigin(this.Reponsitory);
            return from entity in companyView
                   join nBrand in nbrandView on entity.ID equals nBrand.EnterpriseID
                   select new CompanynBrand
                   {
                       ID = entity.ID,
                       nBrandID=nBrand.ID,
                       CompanyName = entity.Name,
                       BrandID = nBrand.BrandID,
                       BrandName = entity.Name,
                   };

            //using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            //{


            //var companyView = new Origins.CompaniesOrigin(reponsitory);
            //var nbrandView = new  Origins.nBrandOrigin(reponsitory);
            //return from entity in companyView
            //       join nBrand in nbrandView on entity.ID equals nBrand.ID
            //       where nBrand.BrandID.Contains(brandid)
            //       select new CompanynBrand
            //       {
            //           ID = entity.ID,
            //           CompanyName = entity.Name,
            //           BrandID = nBrand.BrandID,
            //           BrandName = entity.Name,
            //       };

            //}

        }


    }
}
