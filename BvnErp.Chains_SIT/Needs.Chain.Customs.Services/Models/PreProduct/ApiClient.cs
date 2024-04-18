using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ApiClient : IUnique
    {
        public string ID { get; set; }
        public string ClientId { get; set; }
        public String ClientSupplierID { get; set; }
        public string ClientCode { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
        public string CompanyCode { get; set; }
        public Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }

    }
}
