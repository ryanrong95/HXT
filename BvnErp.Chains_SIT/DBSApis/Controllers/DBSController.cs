using DBSApis.Models;
using DBSApis.Services;
using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace DBSApis.Controllers
{
    public class DBSController : System.Web.Http.ApiController
    {
       

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("ACKStage2")]
        public void ACK2([FromBody] string msg)
        {
            try
            {
                ACKService aCK2Service = new ACKService(msg);
                aCK2Service.DecryptMsg();
            }
            catch (Exception ex)
            {
                string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                Services.SmtpContext.Current.Send(receivers, "星展ACK2：", ex.Message+"报文内容:"+msg);
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("ACKStage3")]
        public void ACK3([FromBody] string msg)
        {
            try
            {
                ACKService aCK3Service = new ACKService(msg);
                aCK3Service.DecryptMsg();               
            }
            catch (Exception ex)
            {
                string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                Services.SmtpContext.Current.Send(receivers, "星展ACK3：", ex.Message + "报文内容:" + msg);
            }
        }

        
    }
}