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
    public class SuppliernBrand: Linq.IDataEntity, IUnique
    {
        /// <summary>
        ///公司ID
        /// </summary>
        public string ID { get; set; }
        public string SupplierName { get; set; }

        public string BrandID { get; set; }
        public string nBrandID { get; set; }

        public string BrandName { get; set; }

    }


    public class SuppliernBrandView : UniqueView<SuppliernBrand, PvdCrmReponsitory>
    {
        public SuppliernBrandView()
        {

        }
        internal SuppliernBrandView(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {
        }


        protected override IQueryable<SuppliernBrand> GetIQueryable()
        {
            var supplierView = new Origins.SuppliersOrigin(this.Reponsitory);
            var nbrandView = new Origins.nBrandOrigin(this.Reponsitory);
            return from entity in supplierView
                   join nBrand in nbrandView on entity.ID equals nBrand.EnterpriseID
                   where entity.IsDraft==false
                   select new SuppliernBrand
                   {
                       ID = entity.ID,
                       nBrandID=nBrand.ID,
                       SupplierName = entity.Name,
                       BrandID = nBrand.BrandID,
                       BrandName = entity.Name,
                   };


        }


    }
}
