using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    public class DecHeadFilesOrigin : UniqueView<Models.DecHeadFile, ScCustomsReponsitory>
    {
        public DecHeadFilesOrigin()
        {
        }

        public DecHeadFilesOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<DecHeadFile> GetIQueryable()
        {
            return from decHeadFile in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadFiles>()
                   select new Models.DecHeadFile
                   {
                       ID = decHeadFile.ID,
                       DecHeadID = decHeadFile.DecHeadID,
                       AdminID = decHeadFile.AdminID,
                       Name = decHeadFile.Name,
                       FileType = (Enums.FileType)decHeadFile.FileType,
                       FileFormat = decHeadFile.FileFormat,
                       Url = decHeadFile.Url,
                       Status = (Enums.Status)decHeadFile.Status,
                       CreateDate = decHeadFile.CreateDate,
                       Summary = decHeadFile.Summary,
                   };
        }
    }
}
