using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class RefundDeny : RefundHandler
    {
        public override void HandleRequest(RefundApply apply)
        {
            if (apply.ApplyStatus == Enums.RefundApplyStatus.Rejected)
            {
                apply.Approve(Enums.RefundApplyStatus.Rejected);

            }
            else
            {
                if (this.successor != null)
                    successor.HandleRequest(apply);
            }
        }
    }
}
