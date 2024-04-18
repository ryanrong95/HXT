using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PsWms.PvRoute.Services.Express;
//using Yahv.Utils.Kdn;

namespace Yahv.PsWms.PvRoute.Services.Models
{
    /// <summary>
    /// 发件人
    /// </summary>
    public class Sender : KdAddress, IKdAddress
    {
        public string Company { get; set; }

        public string Contact { get; set; }

        public string Mobile { get; set; }

        public string Tel { get; set; }

        public Sender()
        {
        }

    }
}
