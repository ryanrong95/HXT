using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace WebApp
{
    public class Global : Needs.Web.Faces.ErpGlobals
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            Timer timer = new Timer();
            timer.Enabled = true;
            timer.Interval = 600000;//十分钟
            timer.Start();
           // timer.Elapsed += new System.Timers.ElapsedEventHandler(GlobalTenMinuteTimer);
        }

        private void GlobalTenMinuteTimer(object sender, ElapsedEventArgs e)
        {
            Needs.Ccs.Services.Models.GlobalTimer.Do();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}