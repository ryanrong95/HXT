using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Logistics.Services.Views
{
    /// <summary>
    /// 运输批次列表 View
    /// </summary>
    public class ManifestVoyageListView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public ManifestVoyageListView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        internal ManifestVoyageListView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        private IQueryable<PageModels.ManifestVoyageListModel> GetCommon(LambdaExpression[] expressions)
        {
            var voyages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>();
            var carriers = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Carriers>();

            var common = from voyage in voyages
                         join carrier in carriers
                                on new
                                {
                                    CarrierCode = voyage.CarrierCode,
                                    VoyageStatus = voyage.Status,
                                    CarrierType = (int)Needs.Wl.Models.Enums.CarrierType.InteLogistics,
                                }
                                equals new
                                {
                                    CarrierCode = carrier.Code,
                                    VoyageStatus = (int)Needs.Wl.Models.Enums.Status.Normal,
                                    CarrierType = carrier.CarrierType,
                                }
                                into carriers2
                         from carrier in carriers2.DefaultIfEmpty()
                         orderby voyage.CutStatus ascending, voyage.CreateTime descending
                         select new PageModels.ManifestVoyageListModel
                         {
                             VoyageNo = voyage.ID,
                             Carrier = carrier.Name,
                             HKLicense = voyage.HKLicense,
                             TransportTime = voyage.TransportTime,
                             DriverName = voyage.DriverName,
                             VoyageType = (Needs.Wl.Models.Enums.VoyageType)voyage.Type,
                             CutStatus = (Needs.Wl.Models.Enums.CutStatus)voyage.CutStatus,
                             CreateTime = voyage.CreateTime,
                         };

            foreach (var expression in expressions)
            {
                common = common.Where(expression as Expression<Func<PageModels.ManifestVoyageListModel, bool>>);
            }

            return common;
        }

        private IQueryable<PageModels.ManifestVoyageListModel> GetList(int pageIndex, int pageSize, LambdaExpression[] expressions)
        {
            var common = GetCommon(expressions);

            return common.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
        }

        private int GetCount(LambdaExpression[] expressions)
        {
            return GetCommon(expressions).Count();
        }

        public IQueryable<PageModels.ManifestVoyageListModel> GetResult(out int totalCount, int pageIndex, int pageSize, LambdaExpression[] expressions)
        {
            totalCount = GetCount(expressions);

            return GetList(pageIndex, pageSize, expressions);
        }

    }
}
