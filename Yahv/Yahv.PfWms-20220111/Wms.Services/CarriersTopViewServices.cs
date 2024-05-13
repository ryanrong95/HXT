using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Models;

namespace Wms.Services
{
    public class CarriersTopViewServices
    {
        public IEnumerable<Carrier> Carriers(string key = null)
        {
            Func<Carrier, bool> ex = item => true;
            if (!string.IsNullOrEmpty(key))
            {
                ex += item => item.Name.Contains(key);
            }



            return new Yahv.Services.Views.CarriersTopView<PvWmsRepository>().Where(ex);

        }

        /// <summary>
        /// 发货方式为送货上门时获得的承运商
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Carrier> GetLocalCarriers(string key = null)
        {
            Func<Carrier, bool> ex = item => true;
            if (!string.IsNullOrEmpty(key))
            {
                ex += item => item.Name.Contains(key);
            }
            var carrierids = new Views.DriversTopView().Select(item => item.EnterpriseID)?.ToArray();

            return new Yahv.Services.Views.CarriersTopView<PvWmsRepository>().Where(ex).Where(item => carrierids.Contains(item.ID));

            //var carriers = new Yahv.Services.Views.CarriersTopView<PvWmsRepository>().Where(ex);
            //var ids = carriers.Select(item => item.ID).ToArray();
            //var enterpriseIDs = new Views.DriversTopView().Where(item => ids.Contains(item.EnterpriseID)).Select(item => item.EnterpriseID);

            //return carriers.Where(item => enterpriseIDs.Contains(item.ID));
        }
    }
}
