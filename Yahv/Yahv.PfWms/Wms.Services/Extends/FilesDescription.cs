//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Wms.Services.Extends
//{
//    public static class FilesDescription
//    {
//        public static Layers.Data.Sqls.PvWms.FilesDescription ToLinq(this Models.FileDescription entity)
//        {
//            return new Layers.Data.Sqls.PvWms.FilesDescription
//            {
//                ID = entity.ID,
//                WaybillID = entity.WaybillID,
//                NoticeID = entity.NoticeID,
//                StorageID = entity.StorageID,
//                CustomName = entity.CustomName,
//                Type = (int)entity.Type,
//                Url = entity.Url,
//                CreateDate = entity.CreateDate,
//                Status = (int)entity.Status,
//                ClientID = entity.ClientID,
//                AdminID = entity.AdminID,
//                InputID = entity.InputID
//            };
//        }
//    }
//}
