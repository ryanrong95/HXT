using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.ClassifyService
{
    public partial class Service1 : ServiceBase
    {
        static object locker = new object();
        static System.Timers.Timer timer;
        static System.Timers.Timer timer2;
        //static System.Timers.Timer timer3;

        public Service1()
        {
            InitializeComponent();
            timer = new System.Timers.Timer();
            timer.Interval = 6000;
            timer.Elapsed += Timer_Elapsed;

            timer2 = new System.Timers.Timer();
            timer2.Interval = 6000;
            timer2.Elapsed += Timer2_Elapsed;

            //timer3 = new System.Timers.Timer();
            //timer3.Interval = 6000;
            //timer3.Elapsed += Timer3_Elapsed;
        }

        /// <summary>
        /// 执行归类方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (locker)
            {
                var a = new ProductClassify().RecievedClassify;
            }
        }

        private void Timer2_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (locker)
            {
                var a = new ProductClassify().TransportClassify;
            }
        }

        //private void Timer3_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    lock (locker)
        //    {
        //        //new test().test1();
        //    }
        //}

        protected override void OnStart(string[] args)
        {
            timer.Start();
            timer2.Start();
            //timer3.Start();
        }

        protected override void OnStop()
        {
            timer.Stop();
            timer2.Stop();
            //timer3.Stop();
        }
    }
}
