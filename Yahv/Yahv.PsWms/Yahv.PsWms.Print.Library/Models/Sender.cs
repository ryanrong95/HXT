using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Utils.Kdn;

namespace Yahv.PsWms.Print.Library.Models
{
    /// <summary>
    /// 发件人
    /// </summary>
    public class Sender : KdnAddress, IKdnAddress
    {
        public string Company { get; set; }

        public string Name { get; set; }

        public string Mobile { get; set; }

        public string Tel { get; set; }

        public Sender()
        {
        }

    }
}
