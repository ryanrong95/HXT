using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using Needs.Wl.CustomsTool.WinForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Views
{
   public class DecHeadFilesView : UniqueView<DecHeadFile, ScCustomsReponsitory>
    {
        public DecHeadFilesView()
        {
        }

        internal DecHeadFilesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DecHeadFile> GetIQueryable()
        {
            return from file in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadFiles>()
                   join admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on file.AdminID equals admin.ID into g
                   from temp in g.DefaultIfEmpty()
                   select new Models.DecHeadFile
                   {
                       ID = file.ID,
                       DecHeadID = file.DecHeadID,
                       Admin = new Ccs.Services.Models.Admin { ID = temp.ID, RealName = temp.RealName },
                       Name = file.Name,
                       FileType = (FileType)file.FileType,
                       FileFormat = file.FileFormat,
                       Url = file.Url,
                       Status = (Ccs.Services.Enums.Status)file.Status,
                       CreateDate = file.CreateDate,
                       Summary = file.Summary
                   };
        }
    }
}
