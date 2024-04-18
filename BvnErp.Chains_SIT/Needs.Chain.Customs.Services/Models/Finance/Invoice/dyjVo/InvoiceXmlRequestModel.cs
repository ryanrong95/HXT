using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InvoiceXmlRequestModel
    {
        public string request_service { get; set; }
        public string request_item { get; set; }
        public object data { get; set; }
        public string token { get; set; } 

        public InvoiceXmlRequestModel()
        {
            this.token =  System.Configuration.ConfigurationManager.AppSettings["dyjtoken"];
        }
    }
}
