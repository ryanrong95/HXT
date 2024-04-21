//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Wms.Services.Extends
//{
//    public static class Storages
//    {
//        public static Layers.Data.Sqls.PvWms.Storages ToLinq(this Models.Storages entity)
//        {
//            return new Layers.Data.Sqls.PvWms.Storages
//            {
//                ID = entity.ID,
//                Type = (int)entity.Type,
//                WareHouseID =entity.WareHouseID,
//                SortingID=entity.SortingID,
//                InputID=entity.InputID,
//                ProductID=entity.ProductID,
//                Quantity = entity.Quantity,
//                NoticeID=entity.NoticeID,
//                OrderID=entity.OrderID,
//                ItemID=entity.ItemID,
//                IsLock=entity.IsLock,
//                CreateDate=entity.CreateDate,
//                Status = (int)entity.Status,
//                ShelveID=entity.ShelveID
//            };
//        }
//    }
//}
