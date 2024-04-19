using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApp.Buyer.Services.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Pricebreak
    {
        public Currency Currency { get; set; }
        public int Moq { get; set; }
        public decimal Price { get; set; }
    }
}
