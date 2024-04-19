using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Views
{
    /// <summary>
    /// 产品视图
    /// </summary>
    public class StandardProductsView : UniqueView<Models.StandardProduct, CvOssReponsitory>
    {
        internal StandardProductsView()
        {

        }
        internal StandardProductsView(CvOssReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.StandardProduct> GetIQueryable()
        {
            var mfsView = new ManufactruersView(this.Reponsitory);

            var linq = from entity in Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.StandardProducts>()
                       join mf in mfsView on entity.ManufactruerID equals mf.ID
                       select new Models.StandardProduct
                       {
                           ID = entity.ID,
                           SignCode = entity.SignCode,
                           Name = entity.Name,
                           PackageCase = entity.PackageCase,
                           Packaging = entity.Packaging,
                           Batch = entity.Batch,
                           DateCode = entity.DateCode,
                           Description = entity.Description,
                           Manufacturer = mf
                       };

            return linq;
        }

    }
}
