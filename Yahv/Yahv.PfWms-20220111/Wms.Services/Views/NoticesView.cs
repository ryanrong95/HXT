//using Layers.Data.Sqls;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Wms.Services.Models;
//using Yahv.Linq;
//using Yahv.Services.Enums;
//using Yahv.Services.Models;
//using Yahv.Utils.Serializers;

//namespace Wms.Services.Views
//{
//    public class NoticesView : QueryView<Notice, PvWmsRepository>
//    {
//        protected override IQueryable<Notice> GetIQueryable()
//        {

//            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.Notices>()
//                   select new Notice
//                   {
//                       ID = entity.ID,
//                       Type = (Yahv.Services.Enums.NoticeType)entity.Type,
//                       WareHouseID = entity.WareHouseID,
//                       WaybillID = entity.WaybillID,
//                       InputID = entity.InputID,
//                       OutputID = entity.OutputID,
//                       ProductID = entity.ProductID,
//                       Supplier = entity.Supplier,
//                       Quantity = entity.Quantity,
//                       Conditions = entity.Conditions.JsonTo<NoticeCondition>(),
//                       CreateDate = entity.CreateDate,
//                       ShelveID = entity.ShelveID,
//                       Status =  (NoticesStatus)entity.Status,
//                       Source =  (NoticeSource)entity.Source,
//                       Target = (NoticesTarget)entity.Target,
//                       BoxCode = entity.BoxCode,
//                       DateCode = entity.DateCode,
//                       Weight = entity.Weight,
//                       Volume = entity.Volume,
//                       BoxingSpecs=entity.BoxingSpecs,
//                       NetWeight=entity.NetWeight,
                       
//                   };
//        }
//    }
//}
