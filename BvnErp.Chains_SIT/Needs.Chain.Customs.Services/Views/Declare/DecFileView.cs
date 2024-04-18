using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 报关单附件
    /// </summary>
    public class DecFileView : UniqueView<Models.DecFile, ScCustomsReponsitory>
    {
        public DecFileView()
        {
        }

        internal DecFileView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DecFile> GetIQueryable()
        {
            var adminView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();

            return from file in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeadFiles>()
                   join admin in adminView on file.AdminID equals admin.ID
                   select new Models.DecFile
                   {
                       ID = file.ID,
                       DecHeadID = file.DecHeadID,
                       AdminID = admin.RealName,
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
