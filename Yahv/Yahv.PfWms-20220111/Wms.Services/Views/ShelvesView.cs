using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Models;
using Yahv.Linq;

namespace Wms.Services.Views
{
    public class ShelvesView : UniqueView<Models.Shelves, PvWmsRepository>
    {
        protected override IQueryable<Shelves> GetIQueryable()
        {

            return from entity in Reponsitory.ReadTable <Layers.Data.Sqls.PvWms.Shelves>()
                   select new Models.Shelves
                   {
                       ID = entity.ID,
                       //FatherID = entity.FatherID,
                       //Type = (Enums.ShelvesType)entity.Type,
                       //Purpose = (Enums.ShelvesPurpose)entity.Purpose,
                       //Addible=entity.Addible,
                       //CreateDate = entity.CreateDate,
                       //UpdateDate = entity.UpdateDate,
                       //Status = (Enums.ShelvesStatus)entity.Status,
                       //SpecID = entity.SpecID,
                       //Summary = entity.Summary,
                       //ManagerID = entity.ManagerID,
                       //EnterpriseID = entity.EnterpriseID,
                       //ClerkID = entity.ClerkID,
                       LeaseID = entity.LeaseID,
                      
                   };
         }
    }
}
