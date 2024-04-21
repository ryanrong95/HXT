using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class StandardPartNumbersRoll : Origins.StandardPartNumbersOrigin
    {
        public StandardPartNumbersRoll()
        {

        }

        protected override IQueryable<StandardPartNumber> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }
}
