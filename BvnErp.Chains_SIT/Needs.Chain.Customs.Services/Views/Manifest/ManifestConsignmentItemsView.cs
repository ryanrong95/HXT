using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ManifestConsignmentItemsView : UniqueView<Models.ManifestConsignmentItem, ScCustomsReponsitory>
    {
        public ManifestConsignmentItemsView()
        {
        }

        internal ManifestConsignmentItemsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ManifestConsignmentItem> GetIQueryable()
        {
            return from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ManifestConsignmentItems>()
                   select new Models.ManifestConsignmentItem
                   {
                       ID = item.ID,
                       ManifestConsignmentID = item.ManifestConsignmentID,
                       GoodsSeqNo = item.GoodsSeqNo,
                       GoodsPackNum = item.GoodsPackNum,
                       GoodsPackType = item.GoodsPackType,
                       GoodsGrossWt = item.GoodsGrossWt,
                       GoodsBriefDesc = item.GoodsBriefDesc,
                       UndgNo = item.UndgNo,
                       HsCode = item.HsCode,
                       GoodsDetailDesc = item.GoodsDetailDesc
                   };
        }
    }
}
