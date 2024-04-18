using Needs.Ccs.Services.Models.BalanceQueue.CoreStep;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestToolAbnormal.Models.ExceptionHandler
{
    /// <summary>
    /// 准备重启海关软件
    /// </summary>
    public class PreRestartCustoms : AbstractHandler
    {
        public override void Handle(ContextParam contextParam)
        {
            Needs.Ccs.Services.Models.BalanceQueue.BalanceQueue balanceQueue = new Needs.Ccs.Services.Models.BalanceQueue.BalanceQueue()
            {
                Info = contextParam.MinProcessIDModelInCircle,
            };

            balanceQueue.SetProcessStatus(Needs.Ccs.Services.Enums.BalanceQueueProcessStatus.PreRestartCustoms);
        }
    }
}
