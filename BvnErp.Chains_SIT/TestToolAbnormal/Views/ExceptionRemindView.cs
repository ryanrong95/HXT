using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TestToolAbnormal.Views
{
    public class ExceptionRemindView
    {
        /// <summary>
        /// 其他必要输入参数 MacAddr, ProcessName, BusinessType
        /// </summary>
        /// <param name="total"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public IEnumerable<Needs.Ccs.Services.Models.BalanceQueue.BalanceQueueInfo> GetBalanceQueueInfo(
            out int total, int page, int rows, params LambdaExpression[] expressions)
        {
            var baseView = new Needs.Ccs.Services.Views.BalanceQueue.BalanceQueueView().GetBalanceQueueInfo()
                .Where(t => t.Status == Needs.Ccs.Services.Enums.Status.Normal 
                        && t.ProcessStatus == Needs.Ccs.Services.Enums.BalanceQueueProcessStatus.NeedRemind);

            foreach (var expression in expressions)
            {
                baseView = baseView.Where(expression as Expression<Func<Needs.Ccs.Services.Models.BalanceQueue.BalanceQueueInfo, bool>>);
            }

            total = baseView.Count();

            var results = baseView.OrderByDescending(t => t.UpdateDate).Skip(rows * (page - 1)).Take(rows).ToList();
            return results;
        }
    }
}
