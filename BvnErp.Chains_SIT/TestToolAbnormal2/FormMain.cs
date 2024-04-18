using Needs.Ccs.Services.Models.BalanceQueueRedis;
using Needs.Utils.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using TestToolAbnormal2.Models.ExceptionHandler;
using TestToolAbnormal2.Models.ExceptionSync;

namespace TestToolAbnormal2
{
    public partial class FormMain : Form
    {
        private bool IsFunInTimerBusy = false;

        ExceptionHandlerThread exceptionHandlerThread;
        ExceptionSyncThread exceptionSyncThread;

        public FormMain()
        {
            InitializeComponent();

            var backgroundThread = new Thread(() =>
            {
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Enabled = true;
                timer.Interval = 1000;
                timer.Elapsed += new System.Timers.ElapsedEventHandler(FunInTimer);
            });
            backgroundThread.IsBackground = true;
            backgroundThread.Start();


            string macAddress = GetMacAddress();

            exceptionHandlerThread = new ExceptionHandlerThread(macAddress, "FailBox", Needs.Ccs.Services.Enums.BalanceQueueBusinessType.DecHead);
            exceptionHandlerThread.StartHandlerThread();

            exceptionSyncThread = new ExceptionSyncThread(macAddress, "FailBox", Needs.Ccs.Services.Enums.BalanceQueueBusinessType.DecHead);
            exceptionSyncThread.StartSyncThread();
        }

        private void FunInTimer(object source, ElapsedEventArgs e)
        {
            if (this.IsFunInTimerBusy)
            {
                return;
            }

            this.IsFunInTimerBusy = true;

            if (this.IsHandleCreated)
            {
                this.BeginInvoke(new MethodInvoker(() =>
                {
                    this.label1.Text = DateTime.Now.ToString();
                    this.label2.Text = "IsHandlerTimerBusy = " + this.exceptionHandlerThread.ShowIsHandlerTimerBusy()
                                        + "  |  " + "IntervalNumForDo = " + this.exceptionHandlerThread.ShowIntervalNumForDo();
                }));
            }

            this.IsFunInTimerBusy = false;
        }

        /// <summary>  
        /// 获取本机MAC地址  
        /// </summary>  
        /// <returns>本机MAC地址</returns>  
        private static string GetMacAddress()
        {
            try
            {
                string strMac = string.Empty;
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        strMac = mo["MacAddress"].ToString();
                    }
                }
                moc = null;
                mc = null;
                return strMac;
            }
            catch
            {
                return "unknown";
            }
        }

        /// <summary>
        /// 进入队列按钮(Xml)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnEnterQueueXml_Click(object sender, EventArgs e)
        {
            this.btnEnterQueueXml.Enabled = false;

            Thread threadEnterQueue = new Thread(() =>
            {
                Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueue balanceQueue = new Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueue()
                {
                    Info = new Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueInfo()
                    {
                        MacAddr = GetMacAddress(),
                        ProcessName = "FailBox",
                        BusinessType = Needs.Ccs.Services.Enums.BalanceQueueBusinessType.DecHead,
                        BusinessID = "HYCDO201907090000001",
                        FilePath = @"C:\Users\cmb1b\Desktop\海关回复错误信息\Failed_530300000004SW0000202894f2bce3ba62f84c75936f983aa225f5fb_201907081746320210241.xml",
                        Brief = "01RPC_报关导入服务失败：该票报关单为重发数据，其客户端报关单编号为：HYCDO201907080000004，",
                    },
                };

                RedisHelper redis = new RedisHelper();
                RedisKey redisKey = new RedisKey(balanceQueue.Info.BusinessType.ToString());

                balanceQueue.EnterQueue(redis, redisKey);


                this.BeginInvoke(new MethodInvoker(() =>
                {
                    this.btnEnterQueueXml.Enabled = true;
                }));
            });

            threadEnterQueue.IsBackground = true;
            threadEnterQueue.Start();
        }

        /// <summary>
        /// 进入队列按钮(FailBox)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnterQueueFailBox_Click(object sender, EventArgs e)
        {
            this.btnEnterQueueFailBox.Enabled = false;

            Thread threadEnterQueue = new Thread(() =>
            {
                Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueue balanceQueue = new Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueue()
                {
                    Info = new Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueInfo()
                    {
                        MacAddr = GetMacAddress(),
                        ProcessName = "FailBox",
                        BusinessType = Needs.Ccs.Services.Enums.BalanceQueueBusinessType.DecHead,
                        BusinessID = "HYCDO201907090000001",
                        FilePath = @"C:\Users\cmb1b\Desktop\海关回复错误信息\Failed_530300000004SW0000202894f2bce3ba62f84c75936f983aa225f5fb_201907081746320210241.zip",
                        Brief = "",
                    },
                };

                RedisHelper redis = new RedisHelper();
                RedisKey redisKey = new RedisKey(balanceQueue.Info.BusinessType.ToString());

                balanceQueue.EnterQueue(redis, redisKey);


                this.BeginInvoke(new MethodInvoker(() =>
                {
                    this.btnEnterQueueFailBox.Enabled = true;
                }));
            });

            threadEnterQueue.IsBackground = true;
            threadEnterQueue.Start();
        }

        /// <summary>
        /// 执行一次处理步骤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRunStep_Click(object sender, EventArgs e)
        {
            this.btnRunStep.Enabled = false;

            //Thread threadRunStep = new Thread(() =>
            //{
            //    Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueue balanceQueue =
            //        new Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueue(10, new TimeSpan(hours: 0, minutes: 1, seconds: 0), 
            //                                                                        exceptionHandlerThread.ExceptionHandler, exceptionHandlerThread.RemindHandler)
            //        {
            //            Info = new Needs.Ccs.Services.Models.BalanceQueueRedis.BalanceQueueInfo()
            //            {
            //                MacAddr = GetMacAddress(),
            //                ProcessName = "FailBox",
            //                BusinessType = Needs.Ccs.Services.Enums.BalanceQueueBusinessType.DecHead,
            //            },
            //        };
            //    balanceQueue.CoreHandler(new RedisHelper());


            //    this.BeginInvoke(new MethodInvoker(() =>
            //    {
            //        this.btnRunStep.Enabled = true;
            //    }));
            //});

            //threadRunStep.IsBackground = true;
            //threadRunStep.Start();
        }

        /// <summary>
        /// 启动 HandlerTimer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartHandlerTimer_Click(object sender, EventArgs e)
        {
            this.exceptionHandlerThread.StartHandlerTimer();
        }

        /// <summary>
        /// 停止 HandlerTimer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStopHandlerTimer_Click(object sender, EventArgs e)
        {
            this.exceptionHandlerThread.StopHandlerTimer();
        }

    }
}
