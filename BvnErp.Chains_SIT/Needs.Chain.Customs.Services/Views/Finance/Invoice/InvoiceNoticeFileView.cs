using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class InvoiceNoticeFileView : UniqueView<Models.InvoiceNoticeFile, ScCustomsReponsitory>
    {
        public InvoiceNoticeFileView()
        {
        }

        internal InvoiceNoticeFileView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.InvoiceNoticeFile> GetIQueryable()
        {
            var invoiceNoticeFiles = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeFiles>();

            return from invoiceNoticeFile in invoiceNoticeFiles
                   where invoiceNoticeFile.Status == (int)Enums.Status.Normal
                   select new Models.InvoiceNoticeFile
                   {
                       ID = invoiceNoticeFile.ID,
                       InvoiceNoticeID = invoiceNoticeFile.InvoiceNoticeID,
                       AdminID = invoiceNoticeFile.AdminID,
                       Name = invoiceNoticeFile.Name,
                       FileType = (Enums.InvoiceNoticeFileType)invoiceNoticeFile.FileType,
                       FileFormat = invoiceNoticeFile.FileFormat,
                       Url = invoiceNoticeFile.Url,
                       Status = (Enums.Status)invoiceNoticeFile.Status,
                       CreateDate = invoiceNoticeFile.CreateDate,
                       Summary = invoiceNoticeFile.Summary,
                   };
        }
    }
}
