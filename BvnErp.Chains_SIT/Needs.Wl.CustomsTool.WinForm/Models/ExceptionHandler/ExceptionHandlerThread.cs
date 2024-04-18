using Needs.Ccs.Services.Models.BalanceQueueRedis.CoreStep;
using Needs.Utils.Redis;
using Needs.Wl.CustomsTool.WinForm.Enums;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Needs.Wl.CustomsTool.WinForm.Models.ExceptionHandler
{
    public class ExceptionHandlerThread
    {
        System.Timers.Timer handlerTimer = new System.Timers.Timer();
        private bool IsHandlerTimerRun = false;

        private bool IsHandlerTimerBusy = false;

        private int IntervalNumForDo = 0;
        private static int MaxIntervalNumForDo = 10 * 1000 / 5000;//在执行 CoreHandler 中插入"执行 发邮件 和 重启海关软件" 的间隔数值

        //初始化 ExceptionHandlerThread Begin
        private RedisHelper _redis;
        private Needs.Ccs.Services.Models.BalanceQueueRedis.RedisKey _redisKey;
        private string _macAddr = string.Empty;
        private string _processName = string.Empty;
        private Needs.Ccs.Services.Enums.BalanceQueueBusinessType _businessType;
        private event RemindNotifyHandler _remindNotify;
        private Logger _logger;

        public ExceptionHandlerThread(
            string macAddr, 
            string processName, 
            Needs.Ccs.Services.Enums.BalanceQueueBusinessType businessType, 
            RemindNotifyHandler remindNotify,
            Logger logger)
        {
            this._redis = new RedisHelper();
            this._redisKey = new Needs.Ccs.Services.Models.BalanceQueueRedis.RedisKey(businessType.ToString());
            this._macAddr = macAddr;
            this._processName = processName;
            this._businessType = businessType;
            this._remindNotify = remindNotify;
            this._logger = logger;
        }
        //初始化 ExceptionHandlerThread End

        /// <summary>
        /// 启动 HandlerThread
        /// </summary>
        public void StartHandlerThread()
        {
            this.IsHandlerTimerRun = true;

            var handlerThread = new Thread(() =>
            {
                handlerTimer.Enabled = true;
                handlerTimer.Interval = 5000;
                handlerTimer.Elapsed += new System.Timers.ElapsedEventHandler(FunInHandlerTimer);
            });
            handlerThread.IsBackground = true;
            handlerThread.Start();
        }

        /// <summary>
        /// 启动 HandlerTimer
        /// </summary>
        public void StartHandlerTimer()
        {
            this.IsHandlerTimerRun = true;
            handlerTimer.Start();
        }

        /// <summary>
        /// 停止 HandlerTimer
        /// </summary>
        public void StopHandlerTimer()
        {
            this.IsHandlerTimerRun = false;
        }

        public bool ShowIsHandlerTimerBusy()
        {
            return this.IsHandlerTimerBusy;
        }

        public int ShowIntervalNumForDo()
        {
            return this.IntervalNumForDo;
        }

        private void FunInHandlerTimer(object source, ElapsedEventArgs e)
        {
            try
            {
                if (!this.IsHandlerTimerRun)
                {
                    //停止 HandlerThread
                    handlerTimer.Enabled = false;
                    return;
                }

                if (this.IsHandlerTimerBusy)
                {
                    return;
                }

                this.IsHandlerTimerBusy = true;

                //执行 发邮件 和 重启海关软件
                this.IntervalNumForDo++;
                if (MaxIntervalNumForDo == this.IntervalNumForDo)
                {
                    IntervalNumForDo = 0;
                    if (this._businessType == Ccs.Services.Enums.BalanceQueueBusinessType.DecHead)
                    {
                        DoSendEmail();
                    }
                    else if (this._businessType == Ccs.Services.Enums.BalanceQueueBusinessType.Manifest)
                    {
                        DoRestartCustoms();
                    }

                }

                Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueue balanceQueue =
                        new Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueue(10, new TimeSpan(hours: 0, minutes: 10, seconds: 0), ExceptionHandler, this._remindNotify, this.CheckIsUnNeedPair)
                        {
                            Info = new Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueInfo()
                            { 
                                MacAddr = this._macAddr,
                                ProcessName = this._processName,
                                BusinessType = this._businessType,
                            },
                        };
                balanceQueue.CoreHandler(this._redis);

                this.IsHandlerTimerBusy = false;
            }
            catch (Exception ex)
            {
                _logger.Error("ExceptionHandlerThread -> FunInHandlerTimer 中异常：" + ex.Message 
                    + "调用堆栈上的桢的字符串表现形式:" + ex.StackTrace 
                    + "\r\n引发当前异常的函数为:" + ex.TargetSite.Name 
                    + "\r\n导致错误的应用程序或对象的名称为:" + ex.Source);
            }
        }

        /// <summary>
        /// 异常处理策略
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ExceptionHandler(object sender, PairedOrNotNeedPairEventArgs e)
        {
            ContextParam contextParam = e.ContextParam;

            bool IsSuccessPaired = contextParam.IsSuccessPaired;
            if (!IsSuccessPaired)
            {
                //false == IsSuccessPaired
                UnPairedExceptionHandler(contextParam);
            }
            else
            {
                //true == IsSuccessPaired
                SuccessPairedExceptionHandler(contextParam);
            }
        }

        public bool CheckIsUnNeedPair(object sender, CheckIsUnNeedPairEventArgs e)
        {
            ContextParam contextParam = e.ContextParam;

            string fileContent = string.Empty;
            if (contextParam.MinProcessIDModelInCircle != null
                && !string.IsNullOrEmpty(contextParam.MinProcessIDModelInCircle.FilePath)
                && contextParam.MinProcessIDModelInCircle.FilePath.ToLower().EndsWith(".xml"))
            {
                fileContent = File.ReadAllText(contextParam.MinProcessIDModelInCircle.FilePath);
            }

            if (string.IsNullOrEmpty(fileContent))
            {
                return false;
            }


            var unPairedStatements = GetLinesFromTxt(ConfigurationManager.AppSettings["ExceptionConfigPath"] + @"\UnPairedConfig.txt");

            foreach (var unPairedStatement in unPairedStatements)
            {
                if (new Models.Statement.Statement(unPairedStatement, string.Empty).Match(fileContent))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 未配对的文件处理
        /// </summary>
        /// <param name="contextParam"></param>
        private void UnPairedExceptionHandler(ContextParam contextParam)
        {
            if (contextParam.IsUnNeedPair)
            {
                new ExceptionStrategy(new RemindHim()).Handle(contextParam);
            }
        }

        /// <summary>
        /// 成功配对的文件处理
        /// </summary>
        /// <param name="contextParam"></param>
        private void SuccessPairedExceptionHandler(ContextParam contextParam)
        {
            var minProcessIDModelInCircle = contextParam.MinProcessIDModelInCircle;
            var pairModelInCircleAndQueue = contextParam.PairModelInCircleAndQueue;

            string xmlFilePath = string.Empty;
            if (minProcessIDModelInCircle != null && minProcessIDModelInCircle.FilePath.EndsWith(".xml"))
            {
                xmlFilePath = minProcessIDModelInCircle.FilePath;
            }
            if (pairModelInCircleAndQueue != null && pairModelInCircleAndQueue.FilePath.EndsWith(".xml"))
            {
                xmlFilePath = pairModelInCircleAndQueue.FilePath;
            }

            if (string.IsNullOrEmpty(xmlFilePath))
            {
                return;
            }

            ExceptionStrategy exceptionStrategy = null;

            string fileContent = File.ReadAllText(xmlFilePath);
            if (!string.IsNullOrEmpty(fileContent))
            {
                fileContent = fileContent.Trim();
            }

            //Statement statement = new Statement();
            Models.Statement.Parser handlerParser = new Models.Statement.Parser();
            ExceptionHandlerEnum exceptionHandlerEnum = handlerParser.Analyse(fileContent);  //statement.Analyse(fileContent);
            switch (exceptionHandlerEnum)
            {
                case ExceptionHandlerEnum.UnTreated:
                    exceptionStrategy = new ExceptionStrategy(new PreSendEmail());
                    break;
                case ExceptionHandlerEnum.RestartCustomsConfig:
                    exceptionStrategy = new ExceptionStrategy(new PreRestartCustoms());
                    break;
                case ExceptionHandlerEnum.ResendMsgConfig:
                    exceptionStrategy = new ExceptionStrategy(new ResendMsg());
                    break;
                case ExceptionHandlerEnum.RemindHimConfig:
                    exceptionStrategy = new ExceptionStrategy(new RemindHim());
                    break;
                case ExceptionHandlerEnum.OtherExceptionConfig:
                    exceptionStrategy = new ExceptionStrategy(new PreSendEmail());
                    break;
                default:
                    exceptionStrategy = new ExceptionStrategy(new PreSendEmail());
                    break;
            }

            exceptionStrategy.Handle(contextParam);
        }

        /// <summary>
        /// 真正发送邮件操作
        /// </summary>
        private void DoSendEmail()
        {
            //查出所有状态为"准备发送邮件"的Info
            int currentLength = (int)this._redis.ListLength(this._redisKey.PreSendEmailList);
            if (currentLength <= 0)
            {
                return;
            }

            List<Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueInfo> sendEmailInfos = new List<Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueInfo>();
            for (int i = 0; i < currentLength; i++)
            {
                var oneSendEmailInfoInfo = this._redis.ListLeftPop<Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueInfo>(this._redisKey.PreSendEmailList);
                sendEmailInfos.Add(oneSendEmailInfoInfo);
            }

            if (sendEmailInfos == null || !sendEmailInfos.Any())
            {
                return;
            }

            //发送邮件
            StringBuilder sb = new StringBuilder();
            sb.Append("<div>");

            foreach (var sendEmailInfo in sendEmailInfos)
            {
                sb.AppendFormat("<h5><strong>{0}</strong></h5>", sendEmailInfo.BusinessID);
                sb.Append("<textarea style='width: 100%; height: 300px; overflow: auto; word-break: break-all;'>");

                if (!string.IsNullOrEmpty(sendEmailInfo.FilePath) && (sendEmailInfo.FilePath.ToLower().EndsWith(".xml")))
                {
                    string fileContent = File.ReadAllText(sendEmailInfo.FilePath);
                    sb.Append(fileContent);
                }

                sb.Append("</textarea>");
            }

            sb.Append("</div>");

            var emails = GetLinesFromTxt(ConfigurationManager.AppSettings["ExceptionConfigPath"] + @"\Emails.txt");
            if (emails != null && emails.Any())
            {
                foreach (var email in emails)
                {
                    SmtpContext.Current.Send(email, "报关辅助工具-异常提醒", sb.ToString());
                }
            }

            foreach (var sendEmailInfo in sendEmailInfos)
            {
                //修改状态为"已经发送过邮件"
                this._redis.ListRightPush<Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueRecord>(this._redisKey.SyncDBBalanceRecordList,
                new Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueRecord()
                {
                    RecordID = Guid.NewGuid().ToString("N"),
                    InfoID = sendEmailInfo.InfoID,
                    OldProcessName = sendEmailInfo.ProcessName,
                    NewProcessName = sendEmailInfo.ProcessName,
                    OldProcessStatus = sendEmailInfo.ProcessStatus,
                    NewProcessStatus = Needs.Ccs.Services.Enums.BalanceQueueProcessStatus.SentEmail,
                    OldProcessID = sendEmailInfo.ProcessID,
                    NewProcessID = sendEmailInfo.ProcessID,
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                });

                sendEmailInfo.ProcessStatus = Needs.Ccs.Services.Enums.BalanceQueueProcessStatus.SentEmail;
                sendEmailInfo.UpdateDate = DateTime.Now;
                this._redis.ListRightPush<Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueInfo>(this._redisKey.SyncDBBalanceInfoList, sendEmailInfo);
            }
        }

        private string[] GetLinesFromTxt(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            List<string> listLines = new List<string>();

            string line = string.Empty;
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        line = line.Trim();
                        if (!string.IsNullOrEmpty(line))
                        {
                            listLines.Add(line.Trim());
                        }
                    }
                }
            }

            return listLines.ToArray();
        }

        /// <summary>
        /// 真正重启海关软件
        /// </summary>
        private void DoRestartCustoms()
        {
            //查出所有状态为"准备重启海关软件"的Info
            int currentLength = (int)this._redis.ListLength(this._redisKey.PreRestartCustomsList);
            if (currentLength <= 0)
            {
                return;
            }

            List<Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueInfo> restartCustomsInfos = new List<Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueInfo>();
            for (int i = 0; i < currentLength; i++)
            {
                var oneRestartCustomsInfo = this._redis.ListLeftPop<Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueInfo>(this._redisKey.PreRestartCustomsList);
                restartCustomsInfos.Add(oneRestartCustomsInfo);
            }

            if (restartCustomsInfos == null || !restartCustomsInfos.Any())
            {
                return;
            }

            //重启海关软件
            var customsProcess = Process.GetProcessesByName(ConfigurationManager.AppSettings["CustomsSoftProcessName"]);
            foreach (var item in customsProcess)
            {
                item.Kill();
            }

            int sleepSeconds = 20;
            string strSetSeconds = ConfigurationManager.AppSettings["RestartCustomsSoftWaitTime"];
            int tempTime = 0;
            if (int.TryParse(strSetSeconds, out tempTime))
            {
                sleepSeconds = tempTime;
            }

            Thread.Sleep(sleepSeconds * 1000);

            Process.Start(ConfigurationManager.AppSettings["CustomsSoftFullPath"]);


            //修改状态为"已经重启海关软件"
            foreach (var restartCustomsInfo in restartCustomsInfos)
            {
                //修改状态为"已经发送过邮件"
                this._redis.ListRightPush<Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueRecord>(this._redisKey.SyncDBBalanceRecordList,
                new Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueRecord()
                {
                    RecordID = Guid.NewGuid().ToString("N"),
                    InfoID = restartCustomsInfo.InfoID,
                    OldProcessName = restartCustomsInfo.ProcessName,
                    NewProcessName = restartCustomsInfo.ProcessName,
                    OldProcessStatus = restartCustomsInfo.ProcessStatus,
                    NewProcessStatus = Needs.Ccs.Services.Enums.BalanceQueueProcessStatus.RestartedCustoms,
                    OldProcessID = restartCustomsInfo.ProcessID,
                    NewProcessID = restartCustomsInfo.ProcessID,
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                });

                restartCustomsInfo.ProcessStatus = Needs.Ccs.Services.Enums.BalanceQueueProcessStatus.RestartedCustoms;
                restartCustomsInfo.UpdateDate = DateTime.Now;
                this._redis.ListRightPush<Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueInfo>(this._redisKey.SyncDBBalanceInfoList, restartCustomsInfo);
            }
        }
    }
}
