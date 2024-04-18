using Needs.Wl.Models.Views;
using System.Collections.Generic;
using System.Linq;

namespace Needs.Wl.Models
{
    public static class MainOrderExtends
    {
        
        public static MainOrderFilesView Files(this MainOrder order)
        {
            return new Views.MainOrderFilesView(order.ID);
        }      
    }
}