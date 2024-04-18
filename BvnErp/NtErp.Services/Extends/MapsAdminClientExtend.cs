using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NtErp.Services.Models;

namespace NtErp.Services.Extends
{
    public static class MapsAdminClientExtend
    {
        public static Layer.Data.Sqls.BvnErp.MapsAdminClient ToLinq(this MapsAdminClient entity)
        {
            return new Layer.Data.Sqls.BvnErp.MapsAdminClient
            {
                AdminID = entity.AdminID,
                ClientID = entity.ClientID
            };
        }
    }
}
