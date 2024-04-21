//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Yahv.Utils.Serializers;

//namespace Wms.Services.Extends
//{
//    public static class Notices
//    {
//        public static Layers.Data.Sqls.PvWms.Notices ToLinq(this Models.Notices entity)
//        {
//            return new Layers.Data.Sqls.PvWms.Notices
//            {
//                ID = entity.ID,
//                Type=(int)entity.Type,
//                WareHouseID=entity.WareHouseID,
//                Supplier=entity.Supplier,
//                WaybillID=entity.WaybillID,
//                InputID=entity.InputID,
//                OutputID=entity.OutputID,
//                ProductID=entity.ProductID,
//                Quantity=entity.Quantity,
//                Conditions=entity.Conditions.Json(),
//                CreateDate=entity.CreateDate,
//                ShelveID=entity.ShelveID,
//                Status=(int)entity.Status,
//                Source=(int)entity.Source,
//                Target=(int)entity.Target,
//                BoxCode=entity.BoxCode,
//                Weight=entity.Weight,
//                Volume=entity.Volume,
//                DateCode =entity.DateCode
//            };
//        }
//    }
//}
