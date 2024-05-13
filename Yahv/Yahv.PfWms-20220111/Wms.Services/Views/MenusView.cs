//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Wms.Services.Models;

//namespace Wms.Services.Views
//{
//    public class MenusView : Needs.Linq.UniqueView<Menus, Layer.Data.Sqls.PvWmsRepository>
//    {
//        protected override IQueryable<Menus> GetIQueryable()
//        {
//            return from entity in Repository.ReadTable<Layer.Data.Sqls.PvWms.Menus>()
//                   select new Menus {
//                       ID = entity.ID,
//                       FatherID = entity.FatherID,
//                       Name = entity.Name,
//                       Url = entity.Url,
//                       Icon = entity.Icon,
//                       Summary = entity.Summary,
//                       Status = (MenuStatus)entity.Status

//                   };
//        }
//    }
//}
