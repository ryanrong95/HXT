using Needs.Ccs.Services;
using Needs.Ccs.Services.Models.BalanceQueue.CoreStep;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using TestToolAbnormal.Enums;

namespace TestToolAbnormal.Models.ExceptionHandler
{
    public class ExceptionHandlerThread
    {
        System.Timers.Timer handlerTimer = new System.Timers.Timer();
        private bool IsHandlerTimerRun = false;

        private bool IsHandlerTimerBusy = false;

        private int IntervalNumForDo = 0;
        private static int MaxIntervalNumForDo = 10 * 1000 / 5000;

        //初始化 ExceptionHandlerThread Begin
        private string _macAddr = string.Empty;
        private string _processName = string.Empty;
        private Needs.Ccs.Services.Enums.BalanceQueueBusinessType _businessType;

        public ExceptionHandlerThread(string macAddr, string processName, Needs.Ccs.Services.Enums.BalanceQueueBusinessType businessType)
        {
            this._macAddr = macAddr;
            this._processName = processName;
            this._businessType = businessType;
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
                DoSendEmail();
                DoRestartCustoms();
            }

            Needs.Ccs.Services.Models.BalanceQueue.BalanceQueue balanceQueue =
                    new Needs.Ccs.Services.Models.BalanceQueue.BalanceQueue(ExceptionHandler, 10, new TimeSpan(hours: 0, minutes: 1, seconds: 0))
                    {
                        Info = new Needs.Ccs.Services.Models.BalanceQueue.BalanceQueueInfo()
                        {
                            MacAddr = this._macAddr,
                            ProcessName = this._processName,
                            BusinessType = this._businessType,
                        },
                    };
            balanceQueue.CoreHandler();

            this.IsHandlerTimerBusy = false;
        }

        /// <summary>
        /// 异常处理策略
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExceptionHandler(object sender, PairedOrNotNeedPairEventArgs e)
        {
            bool IsSuccessPaired = e.ContextParam.IsSuccessPaired;
            if (!IsSuccessPaired)
            {
                return;
            }

            var minProcessIDModelInCircle = e.ContextParam.MinProcessIDModelInCircle;
            var pairModelInCircleAndQueue = e.ContextParam.PairModelInCircleAndQueue;

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

            Statement statement = new Statement(fileContent);
            ExceptionHandlerEnum exceptionHandlerEnum = statement.Analyse();
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

            exceptionStrategy.Handle(e.ContextParam);
        }

        /// <summary>
        /// 真正发送邮件操作
        /// </summary>
        private void DoSendEmail()
        {
            //查出所有状态为"准备发送邮件"的Info
            var sendEmailInfos = new Needs.Ccs.Services.Views.BalanceQueue.BalanceQueueView().GetBalanceQueueInfo()
                .Where(t => t.Status == Needs.Ccs.Services.Enums.Status.Normal
                        && t.ProcessStatus == Needs.Ccs.Services.Enums.BalanceQueueProcessStatus.PreSendEmail
                        && t.MacAddr == this._macAddr
                        && t.ProcessName == this._processName
                        && t.BusinessType == this._businessType)
                .OrderBy(t => t.ProcessID).ToList();

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

            SmtpContext.Current.Send(@"1181397978@qq.com", "报关辅助工具-异常提醒", sb.ToString());  //发送邮件//379511484@qq.com


            //修改状态为"已经发送过邮件"
            Needs.Ccs.Services.Models.BalanceQueue.BalanceQueue balanceQueue = new Needs.Ccs.Services.Models.BalanceQueue.BalanceQueue()
            {
                Infos = sendEmailInfos,
            };
            balanceQueue.BatchSetProcessStatus(Needs.Ccs.Services.Enums.BalanceQueueProcessStatus.SentEmail);
        }

        /// <summary>
        /// 真正重启海关软件
        /// </summary>
        private void DoRestartCustoms()
        {
            //查出所有状态为"准备重启海关软件"的Info
            var sendEmailInfos = new Needs.Ccs.Services.Views.BalanceQueue.BalanceQueueView().GetBalanceQueueInfo()
                .Where(t => t.Status == Needs.Ccs.Services.Enums.Status.Normal
                        && t.ProcessStatus == Needs.Ccs.Services.Enums.BalanceQueueProcessStatus.PreRestartCustoms
                        && t.MacAddr == this._macAddr
                        && t.ProcessName == this._processName
                        && t.BusinessType == this._businessType)
                .OrderBy(t => t.ProcessID).ToList();

            if (sendEmailInfos == null || !sendEmailInfos.Any())
            {
                return;
            }

            //重启海关软件
            var customsProcess = Process.GetProcessesByName("SimulateCustoms");
            foreach (var item in customsProcess)
            {
                item.Kill();
            }

            Thread.Sleep(2000);

            Process.Start(@"C:\Users\cmb1b\Desktop\SimulateCustoms.exe");


            //修改状态为"已经重启海关软件"
            Needs.Ccs.Services.Models.BalanceQueue.BalanceQueue balanceQueue = new Needs.Ccs.Services.Models.BalanceQueue.BalanceQueue()
            {
                Infos = sendEmailInfos,
            };
            balanceQueue.BatchSetProcessStatus(Needs.Ccs.Services.Enums.BalanceQueueProcessStatus.RestartedCustoms);
        }
    }
}
