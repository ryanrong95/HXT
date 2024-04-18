using Needs.Ccs.Services.Models.BalanceQueue.CoreStep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestToolAbnormal.Models.ExceptionHandler
{
    /// <summary>
    /// 抽象 Handler
    /// </summary>
    public abstract class AbstractHandler
    {
        public abstract void Handle(ContextParam contextParam);
    }
}
