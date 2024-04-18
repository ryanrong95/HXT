using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Views
{
    public class ExitNoticeFilesView : UniqueView<Models.ExitNoticeFile, ScCustomsReponsitory>
    {
        public ExitNoticeFilesView()
        {

        }

        public ExitNoticeFilesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.ExitNoticeFile> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeFiles>()
                   select new Models.ExitNoticeFile
                   {
                       ID = entity.ID,
                       ExitNoticeID = entity.ExitNoticeID,
                       AdminID = entity.AdminID,
                       Name = entity.Name,
                       FileType = (Enums.FileType)entity.FileType,
                       FileFormat = entity.FileFormat,
                       URL = entity.URL,
                       Status = (Enums.Status)entity.Status,
                       CreateDate = entity.CreateDate,
                       Summary = entity.Summary,
                   };
        }
    }
}
