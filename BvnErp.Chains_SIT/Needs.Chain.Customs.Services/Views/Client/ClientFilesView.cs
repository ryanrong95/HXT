using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ClientFilesView : UniqueView<Models.ClientFile, ScCustomsReponsitory>
    {
        public ClientFilesView()
        {
        }

        internal ClientFilesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ClientFile> GetIQueryable()
        {
            var adminsView = new AdminsTopView(this.Reponsitory);

            return from clientfile in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientFiles>()
                   join admins in adminsView on clientfile.AdminID equals admins.ID into Admins
                   from admin in Admins.DefaultIfEmpty()
                   select new Models.ClientFile
                   {
                       ID = clientfile.ID,
                       ClientID = clientfile.ClientID,
                       Admin = admin,
                       Name = clientfile.Name,
                       FileType = (Enums.FileType)clientfile.FileType,
                       FileFormat = clientfile.FileFormat,
                       Url = clientfile.Url,
                       Status = (Enums.Status)clientfile.Status,
                       CreateDate = clientfile.CreateDate,
                       Summary = clientfile.Summary
                   };
        }
    }
}
