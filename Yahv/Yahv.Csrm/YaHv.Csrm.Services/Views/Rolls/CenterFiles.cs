using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;

namespace YaHv.Csrm.Services.Views.Rolls
{
    /// <summary>
    /// 中心库文件
    /// </summary>
    public class CenterFiles : UniqueView<Yahv.Services.Models.CenterFileDescription, PvCenterReponsitory>
    {
        string Clientid;
        FileType fileType;
        public CenterFiles(FileType filetype, string clientid)
        {
            this.fileType = filetype;
            this.Clientid = clientid;
        }
        public CenterFiles(FileType filetype)
        {
            this.fileType = filetype;
        }
        protected override IQueryable<Yahv.Services.Models.CenterFileDescription> GetIQueryable()
        {
            if (!string.IsNullOrWhiteSpace(this.Clientid))
            {
                return from entity in new Yahv.Services.Views.CenterFilesTopView()
                 .Where(t => t.ClientID == this.Clientid && t.Status == Yahv.Services.Models.FileDescriptionStatus.Normal && t.Type == (int)this.fileType)
                       select new Yahv.Services.Models.CenterFileDescription
                       {
                           ID = entity.ID,
                           WsOrderID = entity.WsOrderID,
                           LsOrderID = entity.LsOrderID,
                           ApplicationID = entity.ApplicationID,
                           WaybillID = entity.WaybillID,
                           NoticeID = entity.NoticeID,
                           StorageID = entity.StorageID,
                           CustomName = entity.CustomName,
                           Type = entity.Type,
                           Url = Yahv.Services.Models.CenterFile.Web + entity.Url,
                           CreateDate = entity.CreateDate,
                           ClientID = entity.ClientID,
                           AdminID = entity.AdminID,
                           InputID = entity.InputID,
                           Status = entity.Status,
                           PayID = entity.PayID,
                           StaffID = entity.StaffID,
                           ErmApplicationID = entity.ErmApplicationID,
                       };
            }
            return from entity in new Yahv.Services.Views.CenterFilesTopView()
                .Where(t => t.Status == Yahv.Services.Models.FileDescriptionStatus.Normal && t.Type == (int)this.fileType)
                   select new Yahv.Services.Models.CenterFileDescription
                   {
                       ID = entity.ID,
                       WsOrderID = entity.WsOrderID,
                       LsOrderID = entity.LsOrderID,
                       ApplicationID = entity.ApplicationID,
                       WaybillID = entity.WaybillID,
                       NoticeID = entity.NoticeID,
                       StorageID = entity.StorageID,
                       CustomName = entity.CustomName,
                       Type = entity.Type,
                       Url = Yahv.Services.Models.CenterFile.Web + entity.Url,
                       CreateDate = entity.CreateDate,
                       ClientID = entity.ClientID,
                       AdminID = entity.AdminID,
                       InputID = entity.InputID,
                       Status = entity.Status,
                       PayID = entity.PayID,
                       StaffID = entity.StaffID,
                       ErmApplicationID = entity.ErmApplicationID,
                   };


        }
    }
}
