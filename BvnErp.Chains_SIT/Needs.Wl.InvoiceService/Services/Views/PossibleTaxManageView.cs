using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.InvoiceService.Services.Views
{
    public class PossibleTaxManageView : QueryView<PossibleTaxManageViewModel, ScCustomsReponsitory>
    {
        private string InvoiceNoticeID { get; set; }

        public PossibleTaxManageView(string invoiceNoticeID)
        {
            this.InvoiceNoticeID = invoiceNoticeID;
        }

        protected PossibleTaxManageView(ScCustomsReponsitory reponsitory, IQueryable<PossibleTaxManageViewModel> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<PossibleTaxManageViewModel> GetIQueryable()
        {
            var taxManages = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TaxManage>();
            var taxManageMaps = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TaxManageMap>();

            var iQuery = from taxManage in taxManages
                         join taxManageMap in taxManageMaps on taxManage.ID equals taxManageMap.TaxManageID
                         where taxManage.Status == (int)Enums.Status.Normal
                            && taxManage.IsVaild == (int)Enums.InvoiceVaildStatus.UnChecked
                            && taxManageMap.InvoiceNoticeID == this.InvoiceNoticeID
                         select new PossibleTaxManageViewModel
                         {
                             ID = taxManage.ID,
                             InvoiceCode = taxManage.InvoiceCode,
                             InvoiceNo = taxManage.InvoiceNo,
                             InvoiceDate = taxManage.InvoiceDate,
                             SellsName = taxManage.SellsName,
                             Amount = taxManage.Amount,
                             VaildAmount = taxManage.VaildAmount,
                             ConfrimDate = taxManage.ConfrimDate,
                             AuthenticationMonth = taxManage.AuthenticationMonth,
                             IsVaild = (Enums.InvoiceVaildStatus)taxManage.IsVaild,
                             InvoiceDetailID = taxManage.InvoiceDetailID,
                             InvoiceType = (Enums.InvoiceType)taxManage.InvoiceType,
                             BusinessType = (Enums.BusinessType)taxManage.BusinessType,
                             Status = (Enums.Status)taxManage.Status,
                             CreateDate = taxManage.CreateDate,
                             UpdateDate = taxManage.UpdateDate,
                             Summary = taxManage.Summary,
                         };

            return iQuery;
        }

        public PossibleTaxManageViewModel[] GetDatas()
        {
            IQueryable<PossibleTaxManageViewModel> iquery = this.IQueryable.Cast<PossibleTaxManageViewModel>().OrderByDescending(item => item.CreateDate);

            return iquery.ToArray();
        }
    }

    public class PossibleTaxManageViewModel
    {
        public string ID { get; set; }

        public string InvoiceCode { get; set; }

        public string InvoiceNo { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string SellsName { get; set; }

        public decimal Amount { get; set; }

        public decimal? VaildAmount { get; set; }

        public DateTime? ConfrimDate { get; set; }

        public DateTime? AuthenticationMonth { get; set; }

        public Enums.InvoiceVaildStatus IsVaild { get; set; }

        public string InvoiceDetailID { get; set; }

        public Enums.InvoiceType InvoiceType { get; set; }

        public Enums.BusinessType BusinessType { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }
    }
}
