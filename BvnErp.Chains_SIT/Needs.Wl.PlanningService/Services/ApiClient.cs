using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    public class ApiClient
    {
        public string ID { get; set; }

        public string Code { get; set; }

        public string Key { get; set; }

        public string Name { get; set; }

        public string Site { get; set; }

        public string PayExchangeSupplier { get; set; }

        public string DeclareCompany { get; set; }

        public string MQName { get; set; }
        /// <summary>
        /// 推送给Icgoo时用的区分恒远和芯达通
        /// 93：芯达通
        /// 94：恒远
        /// </summary>
        public string IcgooPartnerNo { get; set; }
    }
}