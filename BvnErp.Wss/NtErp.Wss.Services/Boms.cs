using Layer.Data.Sqls.Boms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Services
{
  public static   class BomExtends
    {
      
            public static Layer.Data.Sqls.Boms.Boms ToLinq(this Boms ss)
            {
                return new Layer.Data.Sqls.Boms.Boms
                {
                    ID = ss.ID,
                    CreateDate = ss.CreateDate,
                    ClientID = ss.ClientID,
                    Uri = ss.Uri,
                    Contact = ss.Contact,
                    Email = ss.Email,
                };
            }
        }
}
