using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using Needs.Wl.Models.Hanlders;

namespace Needs.Wl.Models
{
    public class MainOrder : ModelBase<Layer.Data.Sqls.ScCustoms.MainOrders, ScCustomsReponsitory>, IUnique, IPersistence
    {
        #region 属性
          
        public string ClientID { get; set; }

        #endregion

        public MainOrder()
        {
          
        }

      
    }
}