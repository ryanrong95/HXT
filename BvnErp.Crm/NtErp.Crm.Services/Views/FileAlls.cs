using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NtErp.Crm.Services.Models;

namespace NtErp.Crm.Services.Views
{
    /// <summary>
    /// 获取所有的文件
    /// </summary>
    public class FileAlls : UniqueView<File, BvCrmReponsitory>
    {
        public FileAlls()
        {

        }

        protected override IQueryable<File> GetIQueryable()
        {
            AdminTopView adminTops = new AdminTopView(this.Reponsitory);
            return from file in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Files>()
                   join admin in adminTops on file.AdminID equals admin.ID
                   select new File
                   {
                       ID = file.ID,
                       ClientID = file.ClientID,
                       ProjectID = file.ProjectID,
                       ReportID = file.ReportID,
                       WorksOtherID = file.WorksOtherID,
                       WorksWeeklyID = file.WorksWeeklyID,
                       NoticeID = file.NoticeID,
                       ContactID = file.ContactID,
                       Type = file.Type,
                       Name = file.Name,
                       CreateDate = file.CreateDate,
                       Url = file.Url,
                       Admin = admin,
                       Status = (Enums.Status)file.Status,
                   };
        }
    }
}
