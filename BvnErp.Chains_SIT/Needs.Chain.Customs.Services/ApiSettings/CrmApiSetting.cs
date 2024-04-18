using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.ApiSettings
{
    public class CrmApiSetting
    {
        public string ApiName { get; internal set; }

        public string UpdataAgreement { get; set; }

        public CrmApiSetting()
        {
            this.ApiName = "CrmUrl";
            this.UpdataAgreement = "/CrmUnify/Contract";
        } 
    }
}
