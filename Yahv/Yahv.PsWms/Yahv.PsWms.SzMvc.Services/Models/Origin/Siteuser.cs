using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PsWms.SzMvc.Services.Models.Origin
{
    public class Siteuser : IUnique
    {
        public string ID { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }

        public DateTime? LoginDate { get; set; }
    }
}
