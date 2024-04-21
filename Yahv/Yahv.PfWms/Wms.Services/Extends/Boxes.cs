using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Extends
{
    public static class Boxes
    {
        public static Layers.Data.Sqls.PvWms.Boxes ToLinq(this Models.Boxes entity)
        {
            throw new Exception();
            //return new Layers.Data.Sqls.PvWms.Boxes
            //{
            //    ID = entity.ID,
            //    AdminID = entity.AdminID,
            //    CreateDate = entity.CreateDate,
            //    Code = entity.Code,
            //    Status = (int)entity.Status,
            //    Summary = entity.Summary,
            //    WarehouseID = entity.WarehouseID
            //};
        }
    }
}
