using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    public class PartNumbersRoll : Origins.PartNumbersOrigin
    {
        public PartNumbersRoll()
        {

        }
        protected override IQueryable<PartNumber> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }
}
