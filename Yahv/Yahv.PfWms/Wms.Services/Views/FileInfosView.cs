using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Models;
using Yahv.Linq;
using Yahv.Services.Enums;
using Yahv.Services.Models.WH;
using Yahv.Underly;

namespace Wms.Services.Views
{
    public class FileInfosView : UniqueView<FileDescription, PvWmsRepository>
    {
        public FileInfosView()
        {

        }
        protected override IQueryable<FileDescription> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable <Layers.Data.Sqls.PvWms.FilesDescription>()
                   select new FileDescription
                   {
                       ID = entity.ID,
                       WaybillID = entity.WaybillID,
                       StorageID = entity.StorageID,
                       CustomName = entity.CustomName,
                       Type = (FileType)entity.Type,
                       Url = entity.Url,
                       CreateDate = entity.CreateDate,
                       Status = (FileStatus)entity.Status,
                       ClientID = entity.ClientID,
                       AdminID = entity.AdminID,
                       InputID = entity.InputID,  
                       LocalFile="",
                       FileBase64Code="",
                       NoticeID=entity.NoticeID
                       
                   };
        }
    }
}
