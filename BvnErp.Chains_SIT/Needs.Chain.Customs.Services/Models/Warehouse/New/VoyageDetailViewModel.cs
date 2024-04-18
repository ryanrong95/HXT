using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class VoyageDetailViewModel:IUnique
    {
        public string ID { get; set; }
        public string OrderID { get; set; }
        public string ClientID { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public ClientType ClientType { get; set; }
        public string CompanyID { get; set; }
        public int ItemCount { get; set; }
        public string BoxIndex { get; set; }
        public DateTime PackingDate { get; set; }
        public string VoyageNo { get; set; }
    }

    public class GroupVoyageDetailViewModel
    {
        public string ID { get; set; }
        public string OrderID { get; set; }
        public string ClientID { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public ClientType ClientType { get; set; }
        public int ItemCount { get; set; }
        public string VoyageNo { get; set; }
        public List<GroupBox> BoxInfo { get; set; }
    }

    public class GroupBox
    {
        public string BoxIndex { get; set; }
        public string PackingDate { get; set; }
    }
}
