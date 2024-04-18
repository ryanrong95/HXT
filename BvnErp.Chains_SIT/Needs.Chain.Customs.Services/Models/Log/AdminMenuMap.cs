using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class AdminMenuMap : IUnique
    {
        public string ID { get; set; }
        public string AdminID { get; set; }
        public string MenuID { get; set; }
    }

    public class PvbErmAdminMap : IUnique
    {
        public string ID { get; set; }
        public string OriginID { get; set; }
        public string UserName { get; set; }
        public string RealName { get; set; }
        public string RoleID { get; set; }
    }
}
