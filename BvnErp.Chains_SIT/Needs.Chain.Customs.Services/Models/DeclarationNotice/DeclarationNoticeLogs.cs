using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DeclarationNoticeLogs : BaseItems<DeclarationNoticeLog>
    {
        internal DeclarationNoticeLogs(IEnumerable<DeclarationNoticeLog> enums) : base(enums)
        {
        }

        internal DeclarationNoticeLogs(IEnumerable<DeclarationNoticeLog> enums, Action<DeclarationNoticeLog> action) : base(enums, action)
        {
        }

        public override void Add(DeclarationNoticeLog item)
        {
            base.Add(item);
        }
    }
}
