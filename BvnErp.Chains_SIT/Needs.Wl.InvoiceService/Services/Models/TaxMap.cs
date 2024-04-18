using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.InvoiceService.Services.Models
{
    public class TaxMap
    {
        public static void SetApiStatus(string id, Enums.TaxMapApiStatus apiStatus)
        {
            using (var reponsitory = new ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.TaxMap>(new
                {
                    ApiStatus = (int)apiStatus,
                }, item => item.ID == id);
            }
        }

        public static void SetIsMapped(string id, bool isMapped)
        {
            using (var reponsitory = new ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.TaxMap>(new
                {
                    IsMapped = isMapped,
                }, item => item.ID == id);
            }
        }
    }
}
