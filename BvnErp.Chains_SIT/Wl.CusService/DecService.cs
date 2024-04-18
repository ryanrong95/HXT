using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Wl.CusService
{
    public partial class DecService : ServiceBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        System.Timers.Timer OutTimer;  //发送报关文件计时器
        System.Timers.Timer InTimer;   //接收报关回执文件计时器

        public DecService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Logger.Trace(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "Start.");

            System.Threading.Thread.Sleep(20000);

            //var Deckare = Needs.Wl.WlPlot.Current.Customs.DecHeads.ToList();

            //读取发送错误文件夹
            OutTimer = new System.Timers.Timer();
            OutTimer.Interval = 20000;  //设置计时器事件间隔执行时间
            OutTimer.Elapsed += new System.Timers.ElapsedEventHandler(TMOutTimer_Fail);
            OutTimer.Enabled = true;
        }

        protected override void OnStop()
        {
            Logger.Trace(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "End.");
            OutTimer.Enabled = false;
            //InTimer.Enabled = false;
        }

        /// <summary>
        /// 读取失败文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TMOutTimer_Fail(object sender, ElapsedEventArgs e)
        {
            try
            {
                var files = XmlHelper.GetFailBox();

                foreach (var file in files)
                {
                    Logger.Trace("-------------------");
                    Logger.Trace("获取报关单导入fail回执:" + file.ClientSeqNo);
                    //content
                    Logger.Trace("报关单fail回执内容:" + file.ClientSeqNo + " Code: " + file.ResponseCode + " ErrorMessage: " + file.ErrorMessage);

                    //更新DB
                    var Deckare = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.DecHeads[file.ClientSeqNo];
                    //Deckare.DeclareRecepit(file.ResponseCode);
                    
                    //写入回执轨迹表
                    var decTrace = new Needs.Ccs.Services.Models.DecTrace
                    {
                        DeclarationID = file.ClientSeqNo,
                        Channel = file.ResponseCode,
                        Message = file.ErrorMessage,
                        NoticeDate = file.ResponseTime.Value,
                        FileName = file.FileName,
                        BackupUrl = file.BackupUrl
                    };
                    decTrace.Enter();
                    Logger.Trace("报关单fail回执更新DB:" + file.ClientSeqNo);
                    Logger.Trace("-------------------");
                }

            }
            catch (Exception ex)
            {
                Logger.Error("获取报关单导入fail回执失败：" + ex.Message);
            }
        }

    }
}
