using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Extends
{
    public static class Shelves
    {
            public static Layers.Data.Sqls.PvWms.Shelves ToLinq(this Models.Shelves entity)
            {
                return new Layers.Data.Sqls.PvWms.Shelves
                {
                    ID = entity.ID.ToUpper(),
                    //WarehouseID=entity.WarehouseID,
                    //FatherID = entity.FatherID,
                    //Type = (int)entity.Type,
                    //Purpose=(int)entity.Purpose,
                    //Addible=entity.Addible,
                    //CreateDate = entity.CreateDate,
                    //UpdateDate = entity.UpdateDate,
                    //Status = (int)entity.Status,
                    //SpecID=entity.SpecID,
                    //Summary = entity.Summary,
                    //ManagerID = entity.ManagerID,
                    //EnterpriseID = entity.EnterpriseID,
                    //ClerkID = entity.ClerkID,
                    LeaseID = entity.LeaseID,
                };
        }

    }
}
