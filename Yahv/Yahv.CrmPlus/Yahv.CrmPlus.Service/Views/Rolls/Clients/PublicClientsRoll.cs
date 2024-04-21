using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
   
    public class PublicClientsRoll : PublicClientsOrigin
    {

        public PublicClientsRoll() { }
        protected override IQueryable<Models.Origins.PublicClient> GetIQueryable()
        {
            return base.GetIQueryable();
        }

    }


    public class PublicClientsExtendRoll : PublicClientsExtendOrigin
    {

        public PublicClientsExtendRoll() { }
        protected override IQueryable<Models.Origins.PublicClient> GetIQueryable()
        {
            return base.GetIQueryable();
        }

    }

}
