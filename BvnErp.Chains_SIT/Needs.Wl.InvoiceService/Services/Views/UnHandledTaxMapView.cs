using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.InvoiceService.Services.Views
{
    /// <summary>
    /// 未处理的 TaxMap 视图
    /// </summary>
    public class UnHandledTaxMapView : QueryView<UnHandledTaxMapViewModel, ScCustomsReponsitory>
    {
        protected override IQueryable<UnHandledTaxMapViewModel> GetIQueryable()
        {
            var taxMap = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TaxMap>();

            var iQuery = from map in taxMap
                         where map.ApiStatus == (int)Enums.TaxMapApiStatus.UnHandled
                            || map.ApiStatus == (int)Enums.TaxMapApiStatus.RevUnHandled
                         select new UnHandledTaxMapViewModel
                         {
                             ID = map.ID,
                             InvoiceNoticeID = map.InvoiceNoticeID,
                             InvoiceCode = map.InvoiceCode,
                             InvoiceNo = map.InvoiceNo,
                             InvoiceDate = map.InvoiceDate,
                             IsMapped = map.IsMapped,
                             ApiStatus = (Enums.TaxMapApiStatus)map.ApiStatus,
                             Status = (Enums.Status)map.Status,
                             CreateDate = map.CreateDate,
                             UpdateDate = map.UpdateDate,
                             Summary = map.Summary,
                         };

            return iQuery;
        }

        public UnHandledTaxMapViewModel[] GetUnHandledTaxs()
        {
            IQueryable<UnHandledTaxMapViewModel> iquery = this.IQueryable.Cast<UnHandledTaxMapViewModel>().OrderBy(item => item.CreateDate);

            return iquery.ToArray();
        }
    }

    public class UnHandledTaxMapViewModel
    {
        public string ID { get; set; }

        public string InvoiceNoticeID { get; set; }

        public string InvoiceCode { get; set; }

        public string InvoiceNo { get; set; }

        public DateTime InvoiceDate { get; set; }

        public bool IsMapped { get; set; }

        public Enums.TaxMapApiStatus ApiStatus { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }
    }
}
