using NtErp.Wss.Oss.Services.Models;
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
    /// 运单总视图
    /// </summary>
    public class WaybillAlls : UniqueView<Waybill, CvOssReponsitory>
    {
        internal WaybillAlls()
        {

        }
        internal WaybillAlls(CvOssReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Waybill> GetIQueryable()
        {
            var linq = from entity in Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.Waybills>()
                       select new Waybill
                       {
                          ID = entity.ID,
                          Carrier = entity.Carrier,
                          Weight = entity.Weight
                       };

            return linq;
        }

    }
}
