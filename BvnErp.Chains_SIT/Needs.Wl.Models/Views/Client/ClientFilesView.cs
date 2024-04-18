using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    public class ClientFilesView : View<ClientFile, ScCustomsReponsitory>
    {
        private string ClientID;

        public ClientFilesView()
        {

        }

        public ClientFilesView(string clientID)
        {
            this.ClientID = clientID;
            this.AllowPaging = false;
        }

        internal ClientFilesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ClientFile> GetIQueryable()
        {
            return from file in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientFiles>()
                   join admins in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on file.AdminID equals admins.ID into Admins
                   from admin in Admins.DefaultIfEmpty()
                   where file.ClientID == this.ClientID && file.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                   select new ClientFile
                   {
                       ID = file.ID,
                       ClientID = file.ClientID,
                       Admin = new Admin()
                       {
                           ID = admin.ID,
                           RealName = admin.RealName,
                           UserName = admin.UserName
                       },
                       Name = file.Name,
                       FileType = (Enums.FileType)file.FileType,
                       FileFormat = file.FileFormat,
                       Url = file.Url,
                       Status = file.Status,
                       CreateDate = file.CreateDate,
                       Summary = file.Summary
                   };
        }
    }
}