using Needs.Ccs.Services.Views;
using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
   
    [Serializable]
    public class MainDetailViewModel 
    {
        public string OrderID { get; set; }

        public List<OrderDetailOrderItemsModel> OrderItems { get; set; }
     
    }
}
