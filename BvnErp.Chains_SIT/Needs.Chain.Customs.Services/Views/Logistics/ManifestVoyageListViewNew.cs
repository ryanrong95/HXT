using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ManifestVoyageListViewNew : QueryView<ManifestVoyageViewModel, ScCustomsReponsitory>
    {
        public ManifestVoyageListViewNew()
        {
        }

        internal ManifestVoyageListViewNew(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected ManifestVoyageListViewNew(ScCustomsReponsitory reponsitory, IQueryable<ManifestVoyageViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<ManifestVoyageViewModel> GetIQueryable()
        {

            var voyages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>();
            var carriers = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Carriers>();

            var common = from voyage in voyages
                         join carrier in carriers
                                on new
                                {
                                    CarrierCode = voyage.CarrierCode,
                                    VoyageStatus = voyage.Status,
                                    CarrierType = (int)CarrierType.InteLogistics,
                                }
                                equals new
                                {
                                    CarrierCode = carrier.Code,
                                    VoyageStatus = (int)Status.Normal,
                                    CarrierType = carrier.CarrierType,
                                }
                                into carriers2
                         from carrier in carriers2.DefaultIfEmpty()
                         orderby voyage.CutStatus ascending, voyage.CreateTime descending
                         select new ManifestVoyageViewModel
                         {
                             VoyageNo = voyage.ID,
                             Carrier = carrier.Name,
                             HKLicense = voyage.HKLicense,
                             VehicleLicence = voyage.VehicleLicence,
                             TransportTime = voyage.TransportTime,
                             DriverName = voyage.DriverName,
                             VoyageType = (VoyageType)voyage.Type,
                             CutStatus = (CutStatus)voyage.CutStatus,
                             CreateTime = voyage.CreateTime,
                             HKSealNumber = voyage.HKSealNumber,
                         };

            return common;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Models.ManifestVoyageViewModel> iquery = this.IQueryable.Cast<Models.ManifestVoyageViewModel>().OrderByDescending(item => item.CreateTime);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var ienum_EntryNotices = iquery.ToArray();

            var results = ienum_EntryNotices;

            var res = results.Select(
                        item => new
                        {
                            ID = item.VoyageNo,
                            VoyageNo = item.VoyageNo,
                            Carrier = item.Carrier,
                            DriverName = item.DriverName,
                            HKLicense = item.HKLicense + " "+item.VehicleLicence,
                            CreateTime = item.CreateTime.ToString("yyyy-MM-dd"),
                            CutStatusValue = item.CutStatus,
                            CutStatus = item.CutStatus.GetDescription(),
                            TransportTime = item.TransportTime?.ToString("yyyy-MM-dd") ?? string.Empty,
                            VoyageType = item.VoyageType.GetDescription(),
                            HKSealNumber = item.HKSealNumber,
                        }
                     ).ToArray();


            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(item =>
                {
                    object o = item;
                    return o;
                }).ToArray();
            }

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = res.ToArray(),
            };
        }

        public ManifestVoyageListViewNew SearchByVoyageNo(string voyageNo)
        {
            var linq = from query in this.IQueryable
                       where query.VoyageNo == voyageNo
                       select query;

            var view = new ManifestVoyageListViewNew(this.Reponsitory, linq);
            return view;
        }

        public ManifestVoyageListViewNew SearchByCarrier(string carrier)
        {
            var linq = from query in this.IQueryable
                       where query.Carrier == carrier
                       select query;

            var view = new ManifestVoyageListViewNew(this.Reponsitory, linq);
            return view;
        }

        public ManifestVoyageListViewNew SearchByCutStatus(CutStatus cutStatus)
        {
            var linq = from query in this.IQueryable
                       where query.CutStatus == cutStatus
                       select query;

            var view = new ManifestVoyageListViewNew(this.Reponsitory, linq);
            return view;
        }

        public ManifestVoyageListViewNew SearchByStartDate(DateTime dtStart)
        {
            var linq = from query in this.IQueryable
                       where query.CreateTime > dtStart
                       select query;

            var view = new ManifestVoyageListViewNew(this.Reponsitory, linq);
            return view;
        }

        public ManifestVoyageListViewNew SearchByEndDate(DateTime dtEnd)
        {
            dtEnd = dtEnd.AddDays(1);
            var linq = from query in this.IQueryable
                       where query.CreateTime < dtEnd
                       select query;

            var view = new ManifestVoyageListViewNew(this.Reponsitory, linq);
            return view;
        }
    }
}
