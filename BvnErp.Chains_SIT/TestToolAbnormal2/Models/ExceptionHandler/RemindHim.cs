using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models.BalanceQueueRedis;
using Needs.Ccs.Services.Models.BalanceQueueRedis.CoreStep;

namespace TestToolAbnormal2.Models.ExceptionHandler
{
    /// <summary>
    /// 提醒软件使用者(报关员)
    /// </summary>
    public class RemindHim : AbstractHandler
    {
        public override void Handle(ContextParam contextParam)
        {
            BalanceQueueInfo xmlBalanceQueueInfo = null;
            if (!string.IsNullOrEmpty(contextParam.MinProcessIDModelInCircle.FilePath) && contextParam.MinProcessIDModelInCircle.FilePath.ToLower().EndsWith(".xml"))
            {
                xmlBalanceQueueInfo = contextParam.MinProcessIDModelInCircle;
            }
            else if (!string.IsNullOrEmpty(contextParam.PairModelInCircleAndQueue.FilePath) && contextParam.PairModelInCircleAndQueue.FilePath.ToLower().EndsWith(".xml"))
            {
                xmlBalanceQueueInfo = contextParam.PairModelInCircleAndQueue;
            }

            if (xmlBalanceQueueInfo == null)
            {
                return;
            }

            contextParam.Redis.ListRightPush<BalanceQueueRecord>(contextParam.RedisKey.SyncDBBalanceRecordList, new BalanceQueueRecord()
            {
                RecordID = Guid.NewGuid().ToString("N"),
                InfoID = xmlBalanceQueueInfo.InfoID,
                OldProcessName = xmlBalanceQueueInfo.ProcessName,
                NewProcessName = xmlBalanceQueueInfo.ProcessName,
                OldProcessStatus = xmlBalanceQueueInfo.ProcessStatus,
                NewProcessStatus = Needs.Ccs.Services.Enums.BalanceQueueProcessStatus.NeedRemind,
                OldProcessID = xmlBalanceQueueInfo.ProcessID,
                NewProcessID = xmlBalanceQueueInfo.ProcessID,
                Status = Needs.Ccs.Services.Enums.Status.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            });

            xmlBalanceQueueInfo.ProcessStatus = Needs.Ccs.Services.Enums.BalanceQueueProcessStatus.NeedRemind;
            xmlBalanceQueueInfo.UpdateDate = DateTime.Now;
            contextParam.Redis.ListRightPush<BalanceQueueInfo>(contextParam.RedisKey.RemindHimList, xmlBalanceQueueInfo);

            contextParam.Redis.HashSet<BalanceQueueInfo>(contextParam.RedisKey.RemindDBSet, xmlBalanceQueueInfo.InfoID, xmlBalanceQueueInfo);

            //执行通知动作
            contextParam.RemindInfo = xmlBalanceQueueInfo;
            contextParam.OnRemindNotify();
        }
    }
}
