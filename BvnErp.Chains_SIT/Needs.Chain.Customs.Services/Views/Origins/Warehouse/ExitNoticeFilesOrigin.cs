using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    public class ExitNoticeFilesOrigin : UniqueView<Models.ExitNoticeFile, ScCustomsReponsitory>
    {
        internal ExitNoticeFilesOrigin()
        {
        }

        internal ExitNoticeFilesOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ExitNoticeFile> GetIQueryable()
        {
            return from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeFiles>()
                   select new Models.ExitNoticeFile
                   {
                       ID = item.ID,
                       ExitNoticeID = item.ExitNoticeID,
                       AdminID = item.AdminID,
                       Name = item.Name,
                       FileType = (Enums.FileType)item.FileType,
                       FileFormat = item.FileFormat,
                       URL = item.URL,
                       Status = (Enums.Status)item.Status,
                       CreateDate = item.CreateDate,
                       Summary = item.Summary,
                   };
        }
    }
}
