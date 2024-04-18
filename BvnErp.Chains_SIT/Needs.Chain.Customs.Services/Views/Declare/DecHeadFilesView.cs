using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DecHeadFilesView : UniqueView<Models.DecHeadFile, ScCustomsReponsitory>
    {
        public DecHeadFilesView()
        {
        }

        internal DecHeadFilesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DecHeadFile> GetIQueryable()
        {
            var adminView = new AdminsTopView(this.Reponsitory);

            return from file in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadFiles>()
                   join admin in adminView on file.AdminID equals admin.ID
                   select new Models.DecHeadFile
                   {
                       ID = file.ID,
                       DecHeadID = file.DecHeadID,
                       Admin = admin,
                       Name = file.Name,
                       FileType = (Enums.FileType)file.FileType,
                       FileFormat = file.FileFormat,
                       Url = file.Url,
                       Status = (Enums.Status)file.Status,
                       CreateDate = file.CreateDate,
                       Summary = file.Summary
                   };
        }
    }
}
