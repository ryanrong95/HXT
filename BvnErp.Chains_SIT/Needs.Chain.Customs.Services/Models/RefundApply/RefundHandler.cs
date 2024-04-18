using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public abstract class RefundHandler
    {
        protected RefundHandler successor;

        protected void setSuccessor(RefundHandler s)
        {
            this.successor = s;
        }
        public abstract void HandleRequest(RefundApply apply);

    }   
}
