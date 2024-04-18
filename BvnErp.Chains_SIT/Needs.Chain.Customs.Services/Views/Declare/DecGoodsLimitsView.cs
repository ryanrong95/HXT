using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DecGoodsLimitsView : UniqueView<Models.DecGoodsLimit, ScCustomsReponsitory>
    {
        public DecGoodsLimitsView()
        {
        }
        internal DecGoodsLimitsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DecGoodsLimit> GetIQueryable()
        {
            var goodlimits = new BaseGoodsLimitView(this.Reponsitory);

            return from goodlimit in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecGoodsLimits>()
                   join limit in goodlimits on goodlimit.LicTypeCode equals limit.Code
                   select new Models.DecGoodsLimit
                   {
                       ID = goodlimit.ID,
                       DecListID = goodlimit.DecListID,
                       GoodsNo = goodlimit.GoodsNo,
                       LicTypeCode = goodlimit.LicTypeCode,
                       LicenceNo = goodlimit.LicenceNo,
                       LicWrtofDetailNo = goodlimit.LicWrtofDetailNo,
                       LicWrtofQty = goodlimit.LicWrtofQty,
                       LicWrtofQtyUnit = goodlimit.LicWrtofQtyUnit,
                       BaseGoodsLimit = limit,
                       FileUrl = string.IsNullOrEmpty(goodlimit.FileUrl) ? "" : (FileDirectory.Current.FileServerUrl + @"/" + goodlimit.FileUrl.Replace(@"\", @"/")),
                   };
        }
    }
}
