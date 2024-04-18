using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Models.ExceptionHandler
{
    public class ErrDecTrace : Needs.Ccs.Services.Models.DecTrace
    {

    }

    public class ErrDecHead : Needs.Ccs.Services.Models.DecHead
    {
        public void UpdateCusDecStatus()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { this.CusDecStatus }, item => item.ID == this.ID);
            }
        }
    }
}
