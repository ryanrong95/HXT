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
using Yahv.PvData.SpiderService.Utils;

namespace Yahv.PvData.SpiderService
{
    public partial class SpiderService : ServiceBase
    {
        public SpiderService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Timer timerFeroboc = new System.Timers.Timer();
            timerFeroboc.Interval = 10000; // 10秒钟执行一次
            timerFeroboc.Elapsed += Feroboc_Elapsed;
            timerFeroboc.Enabled = true;
            timerFeroboc.Start();

            Timer timerERateCN = new System.Timers.Timer();
            timerERateCN.Interval = 10000; // 10秒钟执行一次
            timerERateCN.Elapsed += ExchangeRateCN_Elapsed;
            timerERateCN.Enabled = true;
            timerERateCN.Start();

            /*
            Timer timerERateHK = new System.Timers.Timer();
            timerERateHK.Interval = 10000; // 10秒钟执行一次
            timerERateHK.Elapsed += ExchangeRateHK_Elapsed;
            timerERateHK.Enabled = true;
            timerERateHK.Start();
            */

            SmtpContext.Current.Send("中心数据计划任务启动", "启动时间：" + DateTime.Now);
        }

        protected override void OnStop()
        {
            SmtpContext.Current.Send("中心数据计划任务停止", "停止时间：" + DateTime.Now);
        }

        /// <summary>
        /// 抓取中国银行外汇牌价
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Feroboc_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Plat.Current.Feroboc.Crawling();
            }
            catch (Exception ex)
            {
                SmtpContext.Current.Send("抓取中国银行外汇牌价异常", $"异常信息: {ex.Message}\r\n 堆栈信息: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 抓取大陆实时汇率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExchangeRateCN_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Plat.Current.ExchangeRateCN.Crawling();
            }
            catch (Exception ex)
            {
                SmtpContext.Current.Send("抓取大陆实时汇率异常", $"异常信息: {ex.Message}\r\n 堆栈信息: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 抓取香港实时汇率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExchangeRateHK_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                Plat.Current.ExchangeRateHK.Crawling();
            }
            catch (Exception ex)
            {
                SmtpContext.Current.Send("抓取香港实时汇率异常", $"异常信息: {ex.Message}\r\n 堆栈信息: {ex.StackTrace}");
            }
        }
    }
}
