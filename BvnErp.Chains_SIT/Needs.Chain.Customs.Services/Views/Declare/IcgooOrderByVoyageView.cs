using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class IcgooOrderByVoyageView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public IcgooOrderByVoyageView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public IcgooOrderByVoyageView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        public List<IcgooOrderByVoyageViewModel> GetResults(string voyageNo)
        {
            var decHeads = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.DecHeads>();
            var icgooOrderMap = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>();

            var results = from decHead in decHeads
                          join icgooOrder in icgooOrderMap on decHead.OrderID equals icgooOrder.OrderID
                          where decHead.VoyNo == voyageNo
                             && decHead.CusDecStatus != "04"
                             && icgooOrder.Status == (int)Enums.Status.Normal
                          select new IcgooOrderByVoyageViewModel
                          {
                              IcgooOrder = icgooOrder.IcgooOrder,
                          };

            results = results.DistinctBy(t => t.IcgooOrder);

            return results.ToList();
        }
    }

    public class IcgooOrderByVoyageViewModel
    {
        public string IcgooOrder { get; set; } = string.Empty;
    }

}
