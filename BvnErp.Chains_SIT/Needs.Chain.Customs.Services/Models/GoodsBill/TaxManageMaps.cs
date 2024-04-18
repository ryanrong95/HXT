using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class TaxManageMapsModel
    {
        private TaxManageMap[] TaxManageMaps { get; set; }

        public TaxManageMapsModel(TaxManageMap[] taxManageMaps)
        {
            this.TaxManageMaps = taxManageMaps;
        }

        public void BatchInsert()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var layerTaxManageMaps = this.TaxManageMaps.Select(item => new Layer.Data.Sqls.ScCustoms.TaxManageMap
                {
                    ID = item.ID,
                    TaxManageID = item.TaxManageID,
                    InvoiceNoticeID = item.InvoiceNoticeID,
                }).ToArray();
                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.TaxManageMap>(layerTaxManageMaps);
            }
        }
    }
}
