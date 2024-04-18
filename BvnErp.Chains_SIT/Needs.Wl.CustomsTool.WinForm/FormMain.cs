using Microsoft.Win32;
using Needs.Ccs.Services.Models.BalanceQueueRedis.CoreStep;
using Needs.Utils.Serializers;
using Needs.Wl.CustomsTool.WinForm;
using Needs.Wl.CustomsTool.WinForm.Models.ExceptionHandler;
using Needs.Wl.CustomsTool.WinForm.Models.ExceptionSync;
using Needs.Wl.CustomsTool.WinForm.Services;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace Needs.Wl.CustomsTool.WinForm
{
    public partial class FormMain : Form
    {
        private Icon blank = Resource.blank;
        private Icon normal = Resource.baoguan;
        private Color mouseoverColor = Color.White;
        delegate void MyDelegate();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        DecFailBoxWatcher decFailBoxWatcher;
        ManifestFailBoxWatcher manifestFailBoxWatcher;
        private string date = string.Empty;
        ExceptionInfo ucExceptionInfo;

        #region 异常处理相关

        ExceptionHandlerThread decExceptionHandlerThread;
        ExceptionHandlerThread manifestExceptionHandlerThread;
        ExceptionSyncThread decExceptionSyncThread;
        ExceptionSyncThread manifestExceptionSyncThread;

        #endregion

        public FormMain()
        {
            InitializeComponent();
            Init();
            button1_Click(this, null);
            FileListen();
            ExceptionInit();
            TimerInit();
            //报关单回执处理
            DecReceiptQueue decReceiptQueue = new DecReceiptQueue();
            ThreadStart threadStartDec = new ThreadStart(decReceiptQueue.ReadQueue);
            Thread threadDec = new Thread(threadStartDec);
            threadDec.Start();

            //舱单回执处理
            ManiReceiptQueue maniReceiptQueue = new ManiReceiptQueue();
            ThreadStart threadStartMani = new ThreadStart(maniReceiptQueue.ReadQueue);
            Thread threadMani = new Thread(threadStartMani);
            threadMani.Start();

            //订阅报文处理
            DecSubQueue decsubQueue = new DecSubQueue();
            ThreadStart threadStartDecSub = new ThreadStart(decsubQueue.ReadQueue);
            Thread threadDecSub = new Thread(threadStartDecSub);
            threadDecSub.Start();
        }

        #region Timer 初始化

        private void TimerInit()
        {
            var timerThread = new Thread(() =>
            {
                System.Timers.Timer timer = new System.Timers.Timer();
                timer.Enabled = true;
                timer.Interval = 1000;
                timer.Elapsed += new System.Timers.ElapsedEventHandler(FunInTimer);
            });
            timerThread.IsBackground = true;
            timerThread.Start();
        }

        private void FunInTimer(object source, ElapsedEventArgs e)
        {
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

            if (date != currentDate)
            {
                if (decFailBoxWatcher != null)
                {
                    decFailBoxWatcher.Dispose();
                }
                if (manifestFailBoxWatcher != null)
                {
                    manifestFailBoxWatcher.Dispose();
                }

                date = currentDate;

                string decFailBoxPath = System.IO.Path.Combine(Tool.Current.Folder.DecMainFolder, ConstConfig.FailBox + @"\" + date);
                if (!Directory.Exists(decFailBoxPath))
                {
                    Directory.CreateDirectory(decFailBoxPath);
                }
                decFailBoxWatcher = new DecFailBoxWatcher(decFailBoxPath);

                string manifestFailBoxPath = System.IO.Path.Combine(Tool.Current.Folder.RmftMainFolder, ConstConfig.FailBox + @"\" + date);
                if (!Directory.Exists(manifestFailBoxPath))
                {
                    Directory.CreateDirectory(manifestFailBoxPath);
                }
                manifestFailBoxWatcher = new ManifestFailBoxWatcher(manifestFailBoxPath);
            }
        }

        #endregion

        private void TimerPreProduct_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 监听回执
        /// </summary>
        private void FileListen()
        {
            //添加监听
            DecMessageWatcher decMessageWatcher = new DecMessageWatcher();
            ManifestMessageWatcher manifestMessageWatcher = new ManifestMessageWatcher();

            DecWaitFailWatcher decWaitFailWatcher = new DecWaitFailWatcher();
            ManifestWaitFailWatcher manifestWaitFailWatcher = new ManifestWaitFailWatcher();

            DecSubMessageWatcher decSubMessageWatcher = new DecSubMessageWatcher();
        }

        /// <summary>
        /// 初始页
        /// </summary>
        private void Init()
        {
            var name = Tool.Current.Company.Name;
            this.Text = name + "-报关辅助工具";
            this.myIcon.Text = name + "-报关辅助工具";
            mouseoverColor = button1.FlatAppearance.MouseOverBackColor;
            this.button1.Click += button_Click;
            this.button2.Click += button_Click;
            this.button3.Click += button_Click;
            this.button4.Click += button_Click;
            this.button5.Click += button_Click;

        }

        /// <summary>
        /// 点击图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void myIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                myMenu.Show();
            }

            if (e.Button == MouseButtons.Left)
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //点击"是(YES)"退出程序
            if (MessageBox.Show("确定要退出程序?", "安全提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                SendMail();
                myIcon.Visible = false;   //设置图标不可见
                this.Close();                  //关闭窗体
                this.Dispose();                //释放资源
                Application.Exit();            //关闭应用程序窗体
                KillProcess();
            }
        }
        /// <summary>
        /// 关闭指定名称的进程
        /// </summary>
        /// <param name="processname"></param>
        void KillProcess()
        {
            Process[] allProcess = Process.GetProcesses();
            foreach (Process p in allProcess)
            {
                var name = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                if (p.ProcessName.ToLower() == name.ToLower())
                {
                    for (int i = 0; i < p.Threads.Count; i++)      
                        p.Threads[i].Dispose();
                    p.Kill();

                    break;
                }
            }

        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)//当用户点击窗体右上角X按钮或(Alt + F4)时 发生          
            {
                e.Cancel = true;
                myIcon.Visible = true;
                this.ShowInTaskbar = false;
                this.myIcon.Icon = this.Icon;
                this.Hide();
            }
        }

        /// <summary>
        /// 程序退出发送邮件
        /// </summary>
        private void SendMail()
        {

        }

        /// <summary>
        /// 按钮点击时更改样式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Click(object sender, EventArgs e)
        {
            invit();
            ((Button)sender).BackColor = BaseStyleSetting.Button_Click_BackColor;
            ((Button)sender).ForeColor = Color.White;
            ((Button)sender).Font = new Font("宋体", 9, FontStyle.Bold);
            ((Button)sender).FlatAppearance.MouseOverBackColor = BaseStyleSetting.Button_Click_BackColor;
        }

        /// <summary>
        /// 按钮字体颜色初始化
        /// </summary>
        public void invit()
        {
            var init_color = BaseStyleSetting.Button_Init_BackColor;
            var init_font = this.Font;
            var init_fontcolor = this.ForeColor;
            var init_MouseOverBackColor = mouseoverColor;
            button1.BackColor = init_color;
            button1.Font = init_font;
            button1.ForeColor = init_fontcolor;
            button1.FlatAppearance.MouseOverBackColor = init_MouseOverBackColor;
            button2.BackColor = init_color;
            button2.Font = init_font;
            button2.ForeColor = init_fontcolor;
            button2.FlatAppearance.MouseOverBackColor = init_MouseOverBackColor;
            button3.BackColor = init_color;
            button3.Font = init_font;
            button3.ForeColor = init_fontcolor;
            button3.FlatAppearance.MouseOverBackColor = init_MouseOverBackColor;
            button4.BackColor = init_color;
            button4.Font = init_font;
            button4.ForeColor = init_fontcolor;
            button4.FlatAppearance.MouseOverBackColor = init_MouseOverBackColor;
            button5.BackColor = init_color;
            button5.Font = init_font;
            button5.ForeColor = init_fontcolor;
            button5.FlatAppearance.MouseOverBackColor = init_MouseOverBackColor;
        }

        /// <summary>
        /// 待申报
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            UnDeclare uc = new UnDeclare();
            uc.Dock = DockStyle.Fill;
            this.splitContainer1.Panel2.Controls.Clear();
            this.splitContainer1.Panel2.Controls.Add(uc);
        }

        /// <summary>
        /// 已申报
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            Declared uc = new Declared();
            uc.Dock = DockStyle.Fill;
            this.splitContainer1.Panel2.Controls.Clear();
            this.splitContainer1.Panel2.Controls.Add(uc);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (ucExceptionInfo == null || ucExceptionInfo.IsDisposed)
            {
                ucExceptionInfo = new ExceptionInfo();
            }

            ucExceptionInfo.InitDeclareGrid();
            ucExceptionInfo.InitManifestGrid();

            ucExceptionInfo.Dock = DockStyle.Fill;
            this.splitContainer1.Panel2.Controls.Clear();
            this.splitContainer1.Panel2.Controls.Add(ucExceptionInfo);
        }

        private void myIcon_MouseMove(object sender, MouseEventArgs e)
        {
            this.myIcon.ShowBalloonTip(30, "消息提醒", "现在是托盘状态!", ToolTipIcon.Info);
        }

        /// <summary>
        /// 点击消息提示气泡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            timer2.Stop();
        }

        /// <summary>
        /// 点击查看消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 查看消息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer2.Stop();
            this.myIcon.Icon = normal;
        }

        private bool time2IsOpen = false;
        /// <summary>
        /// 消息提示
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        public void ShowMessage(string title, string content)
        {
            timer2.Start(); //图像跳动
            time2IsOpen = true;
            this.myIcon.ShowBalloonTip(30, title, content, ToolTipIcon.Info);  //显示消息提示
        }

        /// <summary>
        /// 双击图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myIcon_DoubleClick(object sender, EventArgs e)
        {
            if (time2IsOpen)
            {
                timer2.Stop();
                time2IsOpen = false;
                this.myIcon.Icon = normal;
                this.TopMost = true;
                this.Show();
                this.button4_Click(this, null);
                this.TopMost = false;
                this.button_Click(this.button4, null);
            }
            else
            {
                this.Show();
            }
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {

        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            invit();
            button1.BackColor = BaseStyleSetting.Button_Click_BackColor;
            button1.ForeColor = Color.White;
            button1.Font = new Font("宋体", 9, FontStyle.Bold);
            button1.FlatAppearance.MouseOverBackColor = BaseStyleSetting.Button_Click_BackColor;
            UploadControl uc = new UploadControl();
            uc.Dock = DockStyle.Fill;
            this.splitContainer1.Panel2.Controls.Clear();
            this.splitContainer1.Panel2.Controls.Add(uc);
        }

        /// <summary>
        /// 定时申报报关单和舱单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                //报关单申报
                var decHead = Tool.Current.Customs.DecHeads.AsQueryable().Where(t => t.CusDecStatus == Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusDecStatus>(Needs.Ccs.Services.Enums.CusDecStatus.Make));
                foreach (var item in decHead)
                {
                    item.Declare();
                }

                //舱单申报
                var manifestBill = Tool.Current.Customs.Manifests.AsQueryable().Where(t => t.CusMftStatus == Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusMftStatus>(Needs.Ccs.Services.Enums.CusMftStatus.Make)
                                                      || t.CusMftStatus == Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusMftStatus>(Needs.Ccs.Services.Enums.CusMftStatus.Deleting));
                foreach (var item in manifestBill)
                {
                    item.Apply();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
            }
        }

        bool isBling = false;
        /// <summary>
        /// 头像闪烁
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (isBling)
            {
                this.myIcon.Icon = normal;
                isBling = false;
            }
            else
            {
                this.myIcon.Icon = blank;
                isBling = true;
            }
        }

        #region 异常处理相关

        private void ExceptionInit()
        {
            string macAddress = MacService.GetMacAddress();

            decExceptionHandlerThread = new ExceptionHandlerThread(macAddress, "FailBox", Needs.Ccs.Services.Enums.BalanceQueueBusinessType.DecHead, RemindHandler, 
                LogManager.GetLogger("Dec_Handler_Logger"));
            decExceptionHandlerThread.StartHandlerThread();
            manifestExceptionHandlerThread = new ExceptionHandlerThread(macAddress, "FailBox", Needs.Ccs.Services.Enums.BalanceQueueBusinessType.Manifest, RemindHandler,
                LogManager.GetLogger("Manifest_Handler_Logger"));
            manifestExceptionHandlerThread.StartHandlerThread();

            decExceptionSyncThread = new ExceptionSyncThread(macAddress, "FailBox", Needs.Ccs.Services.Enums.BalanceQueueBusinessType.DecHead,
                LogManager.GetLogger("Dec_Sync_Logger"));
            decExceptionSyncThread.StartSyncThread();
            manifestExceptionSyncThread = new ExceptionSyncThread(macAddress, "FailBox", Needs.Ccs.Services.Enums.BalanceQueueBusinessType.Manifest,
                LogManager.GetLogger("Manifest_Sync_Logger"));
            manifestExceptionSyncThread.StartSyncThread();
        }

        /// <summary>
        /// 提醒要执行的方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RemindHandler(object sender, RemindNotifyEventArgs e)
        {
            var remindInfo = e.BalanceQueueInfo;
            this.ShowMessage(remindInfo.BusinessID + " 异常", remindInfo.Brief);
        }

        #endregion
    }
}
