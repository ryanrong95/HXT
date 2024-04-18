using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class OrderFiles : BaseItems<OrderFile>
    {
        internal OrderFiles(IEnumerable<OrderFile> enums) : base(enums)
        {
        }

        internal OrderFiles(IEnumerable<OrderFile> enums, Action<OrderFile> action) : base(enums, action)
        {
        }

        public override void Add(OrderFile item)
        {
            base.Add(item);
        }

        public override void RemoveAll()
        {
            base.RemoveAll();
        }
    }
}
