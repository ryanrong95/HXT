using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DeclarationNoticeItems : BaseItems<DeclarationNoticeItem>
    {
        internal DeclarationNoticeItems(IEnumerable<DeclarationNoticeItem> enums) : base(enums)
        {
        }

        internal DeclarationNoticeItems(IEnumerable<DeclarationNoticeItem> enums, Action<DeclarationNoticeItem> action) : base(enums, action)
        {
        }

        public override void Add(DeclarationNoticeItem item)
        {
            base.Add(item);
        }
    }
}
