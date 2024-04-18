using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PsWms.SzMvc.Services.Models.Origin
{
    /// <summary>
    /// 客户信息
    /// </summary>
    public class Client : IUnique
    {
        public string ID { get; set; }

        public string SiteuserID { get; set; }

        public string Name { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }
    }
}
