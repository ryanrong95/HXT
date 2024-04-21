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
//    public class SuppliersView : QueryView<Suppliers, PvWmsRepository>
//    {
//        protected override IQueryable<Suppliers> GetIQueryable()
//        {
//            return from entity in Reponsitory.ReadTable <Layers.Data.Sqls.PvWms.WsSuppliersTopView>()
//                   select new Suppliers
//                   {
//                       ID = entity.ID,
//                       Name = entity.Name,
//                       AdminCode = entity.AdminCode,
//                       Corporation = entity.Corporation,
//                       RegAddress=entity.RegAddress,
//                       Uscc=entity.Uscc,
//                       Grade=entity.Grade,
//                       Status=entity.Status,
//                       AdminID=entity.AdminID,
//                       CreateDate=entity.CreateDate,
//                       UpdateDate=entity.UpdateDate,
//                       ChineseName=entity.ChineseName,
//                       EnglishName=entity.EnglishName
//                   };
//        }
//    }
//}
