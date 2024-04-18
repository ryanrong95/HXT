using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class PaymentNoticeFileView : UniqueView<Models.PaymentNoticeFile, ScCustomsReponsitory>
    {
        public PaymentNoticeFileView()
        {

        }

        internal PaymentNoticeFileView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.PaymentNoticeFile> GetIQueryable()
        {
            return from file in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNoticeFiles>()
                   where file.Status == (int)Enums.Status.Normal
                   select new Models.PaymentNoticeFile
                   {
                       ID = file.ID,
                       PaymentNoticeID = file.PaymentNoticeID,
                       AdminID = file.AdminID,
                       FileName = file.Name,
                       FileFormat = file.FileFormat,
                       FileType = (Enums.FileType)file.FileType,
                       Url = file.Url,
                       Status = (Enums.Status)file.Status,
                       CreateDate = file.CreateDate,
                       Summary = file.Summary
                   };
        }
    }
}