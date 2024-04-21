using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Views.Rolls;
using YaHv.CrmPlus.Services.Models.Origins;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service
{
    static public class GetPmBySpnID
    {
        public static Admin GetPm(string spnId)
        {
            var standardPartNumber = new StandardPartNumbersRoll()[spnId];
           // var brand = new BrandsRoll()[standardPartNumber.BrandID];
            var nbrand = new AgentBrandsRoll().FirstOrDefault(x=>x.BrandID== standardPartNumber.BrandID);
            var admin = new Admin();
            if (nbrand != null)
            {
                var vbrand = new vBrandsRoll().FirstOrDefault(x => x.BrandID == standardPartNumber.BrandID);
                if (vbrand != null)
                {
                 admin = new AdminsAllRoll()[vbrand.AdminID];
                }
            }
               return admin;
        }

    }
}
