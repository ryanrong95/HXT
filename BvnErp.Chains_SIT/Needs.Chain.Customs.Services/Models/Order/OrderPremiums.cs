using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class OrderPremiums : BaseItems<OrderPremium>
    {
        internal OrderPremiums(IEnumerable<OrderPremium> enums): base(enums)
        {
        }

        internal OrderPremiums(IEnumerable<OrderPremium> enums, Action<OrderPremium> action) : base(enums, action)
        {
        }

        public override void Add(OrderPremium item)
        {
            base.Add(item);
        }
    }
}
