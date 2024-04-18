using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wl.CusFileListen
{
    public partial class DeclareResponse : Form
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public DeclareResponse()
        {
            InitializeComponent();

            Logger.Trace("============开启监听============");

            //监听报关单回执文件夹
            DecInWatcher.WatcherStrat();
            DecInWatcher.Watcher.EnableRaisingEvents = true;

            //监听舱单回执文件夹
            MftInWatcher.WatcherStrat();
            MftInWatcher.Watcher.EnableRaisingEvents = true;
            

            //监听失败文件夹
            //FailWatcher.WatcherStrat();
            //FailWatcher.Watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// 启动或者停止监听
        /// </summary>
        /// <param name="IsEnableRaising">True:启用监听,False:关闭监听</param>
        private void WatchStartOrSopt(bool IsEnableRaising)
        {
            DecInWatcher.Watcher.EnableRaisingEvents = IsEnableRaising;
            MftInWatcher.Watcher.EnableRaisingEvents = IsEnableRaising;
            //FailWatcher.Watcher.EnableRaisingEvents = IsEnableRaising;
        }


        //禁用窗体的关闭按钮
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Logger.Trace("============手动开启监听============");
            WatchStartOrSopt(true);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Logger.Trace("============手动关闭监听============");
            WatchStartOrSopt(false);
        }
    }
}
