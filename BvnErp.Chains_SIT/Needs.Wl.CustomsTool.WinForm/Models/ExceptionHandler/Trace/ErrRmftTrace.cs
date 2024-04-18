using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Models.ExceptionHandler
{
    public class ErrRmftTrace : ManifestConsignmentTrace
    {

    }

    public class ErrManifestConsignment : Needs.Ccs.Services.Models.ManifestConsignment
    {
        public void UpdateCusMftStatus()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ManifestConsignments>(new { this.CusMftStatus }, item => item.ID == this.ID);
            }
        }
    }
}
