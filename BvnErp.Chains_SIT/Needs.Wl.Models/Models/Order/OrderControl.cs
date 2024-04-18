using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models
{
    public class OrderControl : ModelBase<Layer.Data.Sqls.ScCustoms.Orders, ScCustomsReponsitory>, IUnique, IPersistence
    {
        #region 属性

        public string OrderID { get; set; }

        public string OrderItemID { get; set; }

        public Enums.OrderControlType ControlType { get; set; }

        #endregion

        public OrderControl()
        {

        }


    }
}
