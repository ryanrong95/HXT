//using Layers.Data.Sqls;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Wms.Services.Models;
//using Yahv.Linq;
//using Yahv.Underly;

//namespace Wms.Services.Views
//{
//    public class ShelvesStockView : QueryView<Models.ShelvesStock, PvWmsRepository>
//    {
//        public ShelvesStockView()
//        {

//        }
//        protected override IQueryable<ShelvesStock> GetIQueryable()
//        {
          

//            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ShelvesStockTopView>()
//                   select new Models.ShelvesStock
//                   {
//                       ID = entity.ID,
//                       LeaseID = entity.LeaseID,
//                       EnterpriseID = entity.EnterpriseID,
//                       Quantity = entity.Quantity,
//                   };
//        }
//    }
//}
