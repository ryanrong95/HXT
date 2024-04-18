using Needs.Ccs.Services.Models.BalanceQueue.CoreStep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestToolAbnormal.Models.ExceptionHandler
{
    /// <summary>
    /// 异常策略
    /// </summary>
    public class ExceptionStrategy
    {
        AbstractHandler handler = null;

        public ExceptionStrategy()
        {
            handler = new PreSendEmail();
        }

        public ExceptionStrategy(AbstractHandler handler)
        {
            this.handler = handler;
        }

        public void Handle(ContextParam contextParam)
        {
            try
            {
                handler.Handle(contextParam);
            }
            catch (Exception ex)
            {
                //this.handler = new OtherException();
                //handler.Handle(msg);
            }
        }
    }
}
