using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    /// <summary>
    /// 管控
    /// </summary>
    public class ControlsRoll : Origins.ControlsOrigin
    {
        public ControlsRoll()
        {

        }

        protected override IQueryable<Control> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }
}
