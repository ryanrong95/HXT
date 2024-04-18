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
    /// 准备发送邮件
    /// </summary>
    public class PreSendEmail : AbstractHandler
    {
        public override void Handle(ContextParam contextParam)
        {
            var minProcessIDModelInCircle = contextParam.MinProcessIDModelInCircle;

            contextParam.Redis.ListRightPush<BalanceQueueRecord>(contextParam.RedisKey.SyncDBBalanceRecordList, new BalanceQueueRecord()
            {
                RecordID = Guid.NewGuid().ToString("N"),
                InfoID = minProcessIDModelInCircle.InfoID,
                OldProcessName = minProcessIDModelInCircle.ProcessName,
                NewProcessName = minProcessIDModelInCircle.ProcessName,
                OldProcessStatus = minProcessIDModelInCircle.ProcessStatus,
                NewProcessStatus = Needs.Ccs.Services.Enums.BalanceQueueProcessStatus.PreSendEmail,
                OldProcessID = minProcessIDModelInCircle.ProcessID,
                NewProcessID = minProcessIDModelInCircle.ProcessID,
                Status = Needs.Ccs.Services.Enums.Status.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            });

            minProcessIDModelInCircle.ProcessStatus = Needs.Ccs.Services.Enums.BalanceQueueProcessStatus.PreSendEmail;
            minProcessIDModelInCircle.UpdateDate = DateTime.Now;
            contextParam.Redis.ListRightPush<BalanceQueueInfo>(contextParam.RedisKey.PreSendEmailList, minProcessIDModelInCircle);
        }
    }
}
