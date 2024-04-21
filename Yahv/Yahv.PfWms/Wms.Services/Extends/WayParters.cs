using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Extends
{
    public static class WayParters
    {
        public static Layers.Data.Sqls.PvWms.WayParters ToLinq(this Models.WayParters entity)
        {
            return new Layers.Data.Sqls.PvWms.WayParters
            {
                ID=entity.ID,
                Company = entity.Company,
                Place = entity.Place,
                Address = entity.Address,
                Contact = entity.Contact,
                Phone = entity.Phone,
                Zipcode = entity.Zipcode,
                Email = entity.Email,
                CreateDate = entity.CreateDate,
            };
        }
    }
}
