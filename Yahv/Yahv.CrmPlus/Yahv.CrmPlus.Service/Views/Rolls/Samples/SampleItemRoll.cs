using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly.Erps;

namespace Yahv.CrmPlus.Service.Views.Rolls.Samples
{
    public class SampleItemRoll : Origins.SampleItemOrigin
    {
        public SampleItemRoll()
        {

        }

        protected override IQueryable<SampleItem> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }
}
