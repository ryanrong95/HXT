using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class TaxManageForNoticeView : QueryView<TaxManageForNoticeViewModel, ScCustomsReponsitory>
    {
        private string _invoiceNoticeID { get; set; }

        public TaxManageForNoticeView(string invoiceNoticeID)
        {
            this._invoiceNoticeID = invoiceNoticeID;
        }

        public TaxManageForNoticeView(ScCustomsReponsitory reponsitory, IQueryable<TaxManageForNoticeViewModel> iQueryable) : base(reponsitory, iQueryable)
        {

        }

        protected override IQueryable<TaxManageForNoticeViewModel> GetIQueryable()
        {
            var taxManages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TaxManage>();
            var taxManageMaps = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TaxManageMap>();

            var iQuery = from taxManage in taxManages
                         join taxManageMap in taxManageMaps on taxManage.ID equals taxManageMap.TaxManageID
                         where taxManageMap.InvoiceNoticeID == this._invoiceNoticeID
                            && taxManage.Status == (int)Enums.Status.Normal
                         select new TaxManageForNoticeViewModel
                         {
                             TaxManageID = taxManage.ID,
                             InvoiceCode = taxManage.InvoiceCode,
                             InvoiceNo = taxManage.InvoiceNo,
                             InvoiceDate = taxManage.InvoiceDate,
                             Amount = taxManage.Amount,
                             InvoiceType = (Enums.InvoiceType)taxManage.InvoiceType,
                         };

            return iQuery;
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<TaxManageForNoticeViewModel> iquery = this.IQueryable.Cast<TaxManageForNoticeViewModel>();
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myTaxManages = iquery.ToArray();

            var ienums_linq = from taxManage in ienum_myTaxManages
                              orderby taxManage.InvoiceCode, taxManage.InvoiceNo, taxManage.InvoiceDate
                              select new TaxManageForNoticeViewModel
                              {
                                  TaxManageID = taxManage.TaxManageID,
                                  InvoiceCode = taxManage.InvoiceCode,
                                  InvoiceNo = taxManage.InvoiceNo,
                                  InvoiceDate = taxManage.InvoiceDate,
                                  Amount = taxManage.Amount,
                                  InvoiceType = taxManage.InvoiceType,
                              };

            var results = ienums_linq;

            Func<TaxManageForNoticeViewModel, object> convert = item => new
            {
                TaxManageID = item.TaxManageID,
                InvoiceCode = item.InvoiceCode,
                InvoiceNo = item.InvoiceNo,
                InvoiceDate = item.InvoiceDate?.ToString("yyyy.MM.dd"),
                Amount = item.Amount.ToString("0.00"),
                InvoiceTypeInt = (int)item.InvoiceType,
                InvoiceTypeName = item.InvoiceType.GetDescription(),
            };

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                return results.Select(convert).Select(item =>
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
                rows = results.Select(convert).ToArray(),
            };
        }
    }

    public class TaxManageForNoticeViewModel
    {
        public string TaxManageID { get; set; }

        public string InvoiceCode { get; set; }

        public string InvoiceNo { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public decimal Amount { get; set; }

        public Enums.InvoiceType InvoiceType { get; set; }
    }
}
