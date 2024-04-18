using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wl.IcgooBatch
{
    public class IcgooRequest
    {
        public event IcgooGetRequestHanlder IcgooGetRequest;
        public event IcgooSendSMSHanlder IcgooSendSMS;

        public IcgooRequest()
        {
            IcgooGetRequest += WriteRequestLog;
            IcgooSendSMS += SendMessage;
        }


        public void Process()
        {
            string day = System.Configuration.ConfigurationManager.AppSettings["daypara"];
            string pagesize = System.Configuration.ConfigurationManager.AppSettings["pagesizepara"];
            string IcgooRequestUrl = System.Configuration.ConfigurationManager.AppSettings["IcgooPreProductUrl"];
            string IcgooClientID = System.Configuration.ConfigurationManager.AppSettings["IcgooClientID"];

            string FastBuyRequestUrl = System.Configuration.ConfigurationManager.AppSettings["FastBuyPreProductUrl"];
            string FastBuyClientID = System.Configuration.ConfigurationManager.AppSettings["FastBuyClientID"];

            Console.WriteLine("Icgoo开始请求");
            ProcessRequest(IcgooRequestUrl, IcgooClientID, CompanyTypeEnums.Icgoo, pagesize, day);
            Console.WriteLine("Icgoo请求结束");

            Console.WriteLine("FastBuy开始请求");
            ProcessRequest(FastBuyRequestUrl, FastBuyClientID, CompanyTypeEnums.FastBuy, pagesize,day);
            Console.WriteLine("FastBuy请求结束");
        }

        private void ProcessRequest(string url,string clientID, CompanyTypeEnums companyType,string pagesize,string daypara)
        {
            int page = 0;
            bool ifcontinue = true;
       
            while (ifcontinue)
            {
                page++;
                string requesturl = url+ "?days="+ daypara + "&page="+page.ToString()+"&size="+ pagesize;
                bool requeststatus = true;

                HttpRequest httpRequest = new HttpRequest();
                string result = httpRequest.GetRequest(requesturl, ref requeststatus);
                if (requeststatus)
                {
                    List<IcgooPreProduct> partnos = JsonConvert.DeserializeObject<List<IcgooPreProduct>>(result);
                    foreach (IcgooPreProduct p in partnos)
                    {
                        IcgooPreProduct t = new IcgooPreProduct(companyType);
                        t.sale_orderline_id = p.sale_orderline_id;
                        t.partno = p.partno;
                        t.mfr = p.mfr;
                        t.price = p.price;
                        t.currency_code = p.currency_code;
                        t.ClientID = clientID;
                        t.UpdateTime = t.CreateTime = DateTime.Now;
                        t.Updater = t.Creater = Icgoo.DefaultCreator;
                        t.Status = (int)Status.Normal;
                        t.CompanyType = companyType;
                        t.supplier = p.supplier;
                        t.Enter();
                    }
                    if (partnos.Count != Convert.ToInt16(pagesize))
                    {
                        ifcontinue = false;
                    }
                }
                //写日志
                OnGetDone(new IcgooGetRequestEventArgs("", 0, requesturl, "", requeststatus, companyType));
                //发短信
                if (!requeststatus)
                {
                    OnError(new IcgooGetRequestEventArgs("", 0, requesturl, "", requeststatus, companyType));
                }
            }
        }


        public virtual void OnGetDone(IcgooGetRequestEventArgs args)
        {
            this.IcgooGetRequest?.Invoke(this, args);
        }


        public static void WriteRequestLog(object sender, IcgooGetRequestEventArgs e)
        {
            IcgooRequestLog log = new IcgooRequestLog();
            log.ID = Guid.NewGuid().ToString("N").ToUpper();
            log.Supplier = e.Supplier;
            log.Days = e.Days;
            log.RunPara = e.Url;
            log.Info = e.Info;
            log.IsSend = false;
            log.IsSuccess = e.IsSuccess;
            log.Updatetime = log.Createtime = DateTime.Now;
            log.CompanyType = e.CompanyType;
            log.Enter();
        }

        public virtual void OnError(IcgooGetRequestEventArgs args)
        {
            this.IcgooSendSMS?.Invoke(this, args);
        }

        public static void SendMessage(object sender, IcgooGetRequestEventArgs e)
        {
            using (var view = new Needs.Ccs.Services.Views.SMSContactView())
            {
                foreach (SMSContact s in view.ToList())
                {
                    string sendContent = e.Supplier + "供应商数据获取失败，请查看";
                    SendSMS.SendMessage(s.Phone, sendContent);
                }
            }
        }
    }
}
