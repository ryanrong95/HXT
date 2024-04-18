using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class PayExchangeApplyFileView : UniqueView<Models.PayExchangeApplyFile, ScCustomsReponsitory>
    {
        public PayExchangeApplyFileView()
        {

        }

        internal PayExchangeApplyFileView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.PayExchangeApplyFile> GetIQueryable()
        {
            return from payApplyFile in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CenterApplyFilesTopView>()
                   where payApplyFile.Status != (int)Enums.Status.Delete
                   select new Models.PayExchangeApplyFile
                   {
                       ID = payApplyFile.ID,
                       PayExchangeApplyID = payApplyFile.PayExchangeApplyID,
                       FileName = payApplyFile.Name,
                       FileFormat = "",
                       FileType = (Enums.FileType)payApplyFile.FileType,
                       Url = payApplyFile.Url,
                       Status = (Enums.Status)payApplyFile.Status,
                       CreateDate = payApplyFile.CreateDate,
                   };

            //return from payApplyFile in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyFiles>()
            //       where payApplyFile.Status == (int)Enums.Status.Normal
            //       select new Models.PayExchangeApplyFile
            //       {
            //           ID = payApplyFile.ID,
            //           PayExchangeApplyID = payApplyFile.PayExchangeApplyID,
            //           UserID = payApplyFile.UserID,
            //           AdminID = payApplyFile.AdminID,
            //           FileName = payApplyFile.Name,
            //           FileFormat = payApplyFile.FileFormat,
            //           FileType = (Enums.FileType)payApplyFile.FileType,
            //           Url = payApplyFile.Url,
            //           Status = (Enums.Status)payApplyFile.Status,
            //           CreateDate = payApplyFile.CreateDate,
            //           Summary = payApplyFile.Summary
            //       };
        }
    }
}