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
   /// 出库通知文件视图
   /// </summary>
    public class ExitNoticeFileView : UniqueView<Models.ExitNoticeFile, ScCustomsReponsitory>
    {
        public ExitNoticeFileView()
        {
        }

        internal ExitNoticeFileView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ExitNoticeFile> GetIQueryable()
        {
            var adminView = new AdminsTopView(this.Reponsitory);

            return from file in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeFiles>()
                   join admin in adminView on file.AdminID equals admin.ID
                   where file.Status == (int)Enums.Status.Normal
                   select new Models.ExitNoticeFile
                   {
                       ID = file.ID,
                       ExitNoticeID = file.ExitNoticeID,
                       Admin = admin,
                       Name = file.Name,
                       FileType = (Enums.FileType)file.FileType,
                       FileFormat = file.FileFormat,
                       URL = file.URL,
                       Status = (Enums.Status)file.Status,
                       CreateDate = file.CreateDate,
                       Summary = file.Summary
                   };
        }
    }
}
