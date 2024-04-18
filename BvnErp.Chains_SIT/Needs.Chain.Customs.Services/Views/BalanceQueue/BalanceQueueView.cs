using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.BalanceQueue
{
    public class BalanceQueueView
    {
        private ScCustomsReponsitory _reponsitory;

        public BalanceQueueView()
        {
            _reponsitory = new ScCustomsReponsitory();
        }

        public BalanceQueueView(ScCustomsReponsitory reponsitory)
        {
            _reponsitory = reponsitory;
        }

        #region 查询 BalanceQueueInfo

        public IQueryable<Models.BalanceQueue.BalanceQueueInfo> GetBalanceQueueInfo(params LambdaExpression[] expressions)
        {
            var balanceQueueInfos = _reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BalanceQueueInfos>();

            var results = from balanceQueueInfo in balanceQueueInfos
                          select new Models.BalanceQueue.BalanceQueueInfo
                          {
                              InfoID = balanceQueueInfo.InfoID,
                              MacAddr = balanceQueueInfo.MacAddr,
                              ProcessName = balanceQueueInfo.ProcessName,
                              BusinessType = (Enums.BalanceQueueBusinessType)balanceQueueInfo.BusinessType,
                              BusinessID = balanceQueueInfo.BusinessID,
                              FilePath = balanceQueueInfo.FilePath,
                              ProcessStatus = (Enums.BalanceQueueProcessStatus)balanceQueueInfo.ProcessStatus,
                              ProcessID = balanceQueueInfo.ProcessID,
                              PairCode = balanceQueueInfo.PairCode,
                              Status = (Enums.Status)balanceQueueInfo.Status,
                              CreateDate = balanceQueueInfo.CreateDate,
                              UpdateDate = balanceQueueInfo.UpdateDate,
                              Summary = balanceQueueInfo.Summary,
                          };

            if (expressions != null && expressions.Any())
            {
                foreach (var expression in expressions)
                {
                    results = results.Where(expression as Expression<Func<Models.BalanceQueue.BalanceQueueInfo, bool>>);
                }
            }

            return results;
        }

        #endregion


    }
}
