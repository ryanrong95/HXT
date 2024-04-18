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
    /// 暂存文件View
    /// </summary>
    public class TemporaryFileView : UniqueView<Models.TemporaryFile, ScCustomsReponsitory>
    {
        public TemporaryFileView()
        {
        }

        internal TemporaryFileView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.TemporaryFile> GetIQueryable()
        {
            //var adminView = new Views.AdminsTopView(this.Reponsitory);

            return from temporaryFile in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TemporaryFiles>()
                   //join admin in adminView on temporaryFile.AdminID equals admin.ID
                   where temporaryFile.Status == (int)Enums.Status.Normal
                   select new Models.TemporaryFile
                   {
                       ID = temporaryFile.ID,
                       TemporaryID = temporaryFile.TemporaryID,
                       AdminID = temporaryFile.AdminID,
                       Name = temporaryFile.Name,
                       FileType = (Enums.FileType)temporaryFile.FileType,
                       FileFormat = temporaryFile.FileFormat,
                       URL = temporaryFile.URL,
                       Status = (Enums.Status)temporaryFile.Status,
                       CreateDate = temporaryFile.CreateDate,
                       Summary = temporaryFile.Summary,
                   };
        }
    }
}
