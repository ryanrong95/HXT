using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class IcgooDeclarationResponse
    {
        public string OrderID { get; set; }
        public string PackNo { get; set; }
        public string OrderStatus { get; set; }
    }
}