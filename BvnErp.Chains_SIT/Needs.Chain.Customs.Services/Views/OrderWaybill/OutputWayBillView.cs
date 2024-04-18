using Layer.Data.Sqls;
using Layer.Data.Sqls.ScCustoms;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// /// <summary>
    /// 香港出口清关视图
    /// </summary>
    /// </summary>
    public class OutputWayBillView : UniqueView<Models.OutputWayBill, ScCustomsReponsitory>
    {
        public OutputWayBillView()
        {
        }

        internal OutputWayBillView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.OutputWayBill> GetIQueryable()
        {
            var voyagesView = new VoyagesView(this.Reponsitory);
            return from bill in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ManifestConsignments>()
                   join manifest in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Manifests>() on bill.ManifestID equals manifest.ID
                   join voyage in voyagesView on bill.ManifestID equals voyage.ID into vogages
                   from voyage in vogages.DefaultIfEmpty()

                   select new Models.OutputWayBill
                   {
                       ID = bill.ID,
                       PackNum = bill.PackNum,
                       GoodsValue = bill.GoodsValue,
                       Currency = bill.Currency,
                       Voyage = new Voyage {
                           ID = voyage.ID,
                           HKLicense = voyage.HKLicense,
                           LoadingDate = manifest.LoadingDate,
                           Carrier = voyage.Carrier,
                           HKDeclareStatus = voyage.HKDeclareStatus
                       },
                      
    };


        }
    }
}
