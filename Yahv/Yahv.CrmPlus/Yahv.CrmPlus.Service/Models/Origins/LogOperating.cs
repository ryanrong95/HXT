using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Erps;
using YaHv.CrmPlus.Services.Models.Origins;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    public class LogOperating : Linq.IUnique
    {
        public string ID { get; set; }

        public string MainID { get; set; }

        public string SubID { get; set; }

        public string Context { get; set; }

        public DateTime CreateDate { get; set; }
        public Admin Admin { get; set; }

    }
}
