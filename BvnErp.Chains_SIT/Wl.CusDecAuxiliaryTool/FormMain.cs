using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Threading;
using System.Windows.Forms;
using Wl.CusDecAuxiliaryTool.Service;

namespace Wl.CusDecAuxiliaryTool
{
    /// <summary>
    /// 测试代码，可删除
    /// </summary>
    public class myButton : Button
    {
        public new string Text
        {
            get
            {
                return "myText";
            }
        }
    }


    public partial class FormMain : Form
    {
        private Icon blank = Resource.blank;
        private Icon normal = Resource.BAOGUAN;
        private Color mouseoverColor = Color.White;
        delegate void MyDelegate();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        public FormMain()
        {
            InitializeComponent();
            Init();
            button1_Click(this, null);
            FileListen();

            //测试代码，可删除
            myButton myButton = new myButton();
            myButton.Click += Button1_Click;
        }

        //测试代码，可删除
        private void Button1_Click(object sender, EventArgs e)
        {

        }

        private void TimerPreProduct_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 监听回执
        /// </summary>
        private void FileListen()
        {
            Logger.Trace("============开启监听============");

            //监听报关单回执文件夹
            DecInWatcher decWatch = new DecInWatcher();
            decWatch.WatcherStrat();
            decWatch.Watcher.EnableRaisingEvents = true;

            //监听舱单回执文件夹
            //MftInWatcher.WatcherStrat();
            //MftInWatcher.Watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// 初始页
        /// </summary>
        private void Init()
        {
            var name = CompanyContext.Current.CompanySimpleName;
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
            ((Button)sender).BackColor = BaseSetting.Button_Click_BackColor;
            ((Button)sender).ForeColor = Color.White;
            ((Button)sender).Font = new Font("宋体", 9, FontStyle.Bold);
            ((Button)sender).FlatAppearance.MouseOverBackColor = BaseSetting.Button_Click_BackColor;
        }

        /// <summary>
        /// 按钮字体颜色初始化
        /// </summary>
        public void invit()
        {
            var init_color = BaseSetting.Button_Init_BackColor;
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

        /// <summary>
        /// 消息提示
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        public void ShowMessage(string title, string content)
        {
            timer2.Start(); //图像跳动
            this.myIcon.ShowBalloonTip(30, title, content, ToolTipIcon.Info);  //显示消息提示
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            timer2.Start();
        }

        /// <summary>
        /// 双击图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myIcon_DoubleClick(object sender, EventArgs e)
        {
            timer2.Stop();
            this.myIcon.Icon = normal;
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
            button1.BackColor = BaseSetting.Button_Click_BackColor;
            button1.ForeColor = Color.White;
            button1.Font = new Font("宋体", 9, FontStyle.Bold);
            button1.FlatAppearance.MouseOverBackColor = BaseSetting.Button_Click_BackColor;
        }

        private void btn_Click(object sender, EventArgs e)
        {
            //消息队列路径
            string path = ".\\Private$\\myQueue";
            MessageQueue queue;
            //如果存在指定路径的消息队列 
            if (MessageQueue.Exists(path))
            {
                //获取这个消息队列
                queue = new MessageQueue(path);
            }
            else
            {
                //不存在，就创建一个新的，并获取这个消息队列对象
                queue = MessageQueue.Create(path);
            }
            System.Messaging.Message msg = new System.Messaging.Message();
            //内容
            msg.Body = "Hello World";
            //指定格式化程序
            msg.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            queue.Send(msg);

            //接收到的消息对象
            System.Messaging.Message msg1 = queue.Receive();
            //指定格式化程序
            msg1.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            //接收到的内容
            string str = msg1.Body.ToString();
        }

        /// <summary>
        /// 定时申报报关单和舱单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UnDeclare uc = new UnDeclare();
            //报关单申报
            var DecHead = new Needs.Ccs.Services.Views.DecHeadsListView().AsQueryable();
            DecHead = DecHead.Where(t => t.Status == Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusDecStatus>(Needs.Ccs.Services.Enums.CusDecStatus.Make));

            foreach (var item in DecHead)
            {
                var head = new Needs.Ccs.Services.Views.DecHeadsView().First(t => t.ID == item.ID);
                head.ClientDeclare();
            }

            uc.InitDeclareGrid();

            //舱单申报
            var ManifestBill = new Needs.Ccs.Services.Views.ManifestConsignmentsView().Where(t => t.CusMftStatus == Needs.Ccs.Services.MultiEnumUtils.ToCode<Needs.Ccs.Services.Enums.CusMftStatus>(Needs.Ccs.Services.Enums.CusMftStatus.Make));
            foreach (var item in ManifestBill)
            {
                item.Apply();
            }
            uc.InitManifestGrid();
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

        private void button7_Click(object sender, EventArgs e)
        {
            Logger.Info("fewf");
            Logger.Error("dfds");
        }
    }
}
