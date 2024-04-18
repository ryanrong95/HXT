using Needs.Ccs.Services.Models.BalanceQueue.CoreStep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestToolAbnormal.Models.ExceptionHandler
{
    /// <summary>
    /// 提醒软件使用者(报关员)
    /// </summary>
    public class RemindHim : AbstractHandler
    {
        public override void Handle(ContextParam contextParam)
        {
            Needs.Ccs.Services.Models.BalanceQueue.BalanceQueue balanceQueue = new Needs.Ccs.Services.Models.BalanceQueue.BalanceQueue()
            {
                Info = contextParam.MinProcessIDModelInCircle,
            };

            balanceQueue.SetProcessStatus(Needs.Ccs.Services.Enums.BalanceQueueProcessStatus.NeedRemind);
        }
    }
}
