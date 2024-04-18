using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 运输批次附件的视图
    /// </summary>
    public class VoyageFilesView : UniqueView<Models.VoyageFile, ScCustomsReponsitory>
    {
        public VoyageFilesView()
        {
        }

        internal VoyageFilesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<VoyageFile> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);

            return from voyageFile in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.VoyageFiles>()
                   join admin in adminsView on voyageFile.AdminID equals admin.ID
                   where voyageFile.Status == (int)Enums.Status.Normal
                   select new Models.VoyageFile
                   {
                       ID = voyageFile.ID,
                       VoyageID = voyageFile.VoyageID,
                       Admin = admin,
                       Name = voyageFile.Name,
                       FileType = (Enums.FileType)voyageFile.FileType,
                       FileFormat = voyageFile.FileFormat,
                       Url = voyageFile.Url,
                       Status = (Enums.Status)voyageFile.Status,
                       CreateDate = voyageFile.CreateDate,
                       Summary = voyageFile.Summary
                   };
        }
    }
}
