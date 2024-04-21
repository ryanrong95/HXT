//using Layers.Data.Sqls;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Wms.Services.Models;
//using Yahv.Linq;

//namespace Wms.Services.Views
//{
//    public class CrmWarehouseView : QueryView<CrmWarehouse, PvWmsRepository>
//    {
//        protected override IQueryable<CrmWarehouse> GetIQueryable()
//        {
//            return from entity in Reponsitory.ReadTable <Layers.Data.Sqls.PvWms.CrmWarehouseView>()
//                   select new CrmWarehouse
//                   {
//                       ID = entity.ID,
//                       Name=entity.Name
//                   };
//        }
//    }
//}
