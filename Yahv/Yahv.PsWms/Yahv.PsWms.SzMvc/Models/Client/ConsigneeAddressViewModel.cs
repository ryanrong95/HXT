using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PsWms.SzMvc.Models
{
    public class ConsigneeAddressViewModel
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string[] ClientAddress { get; set; }
        public string AddressDetail { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsDefault { get; set; }
        //页面show
        public string IsDefaultVal { get; set; }
    }
}