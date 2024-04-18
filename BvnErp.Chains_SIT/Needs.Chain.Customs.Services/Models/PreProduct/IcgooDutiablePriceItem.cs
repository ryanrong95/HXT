using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class IcgooDutiablePriceItem 
    {
        public string id { get; set; }     
        public string sale_orderline_id { get; set; }       
        public string partno { get; set; }              
        public string supplier { get; set; }       
        public string mfr { get; set; }       
        public string brand { get; set; }       
        public string origin { get; set; }            
        public decimal customs_rate { get; set; }      
        public decimal origin_tax { get; set; }        
        public decimal add_rate { get; set; }
        public string product_name { get; set; }
        public string category { get; set; }
        public string hs_code { get; set; }
        public string tax_code { get; set; }
        public int qty { get; set; }

    }
}
