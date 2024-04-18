using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wl.IcgooBatch
{
    public class InsideOrderNoRequest
    {
        public event InsideOrderNoGetRequestHanlder InsideGetRequest;
        public event InsideOrderNoSendSMSHanlder InsideSendSMS;
        public InsideOrderNoRequest()
        {
            InsideGetRequest += Log;
            InsideSendSMS += SendMessage;
        }

        public void Process()
        {
            getOrder(getOrderNo());
        }

        private List<InsideOrderNoItem> getOrderNo()
        {
            List<InsideOrderNoItem> orderNoList = new List<InsideOrderNoItem>();
            try
            {
                string DateNow = DateTime.Now.ToString("yyyy-MM-dd");               
                string requestUrl = InsideOrder.GetInsideOrderNoUrl.Replace("timepara", DateNow);
                
                bool requeststatus = true;
                HttpRequest httpRequest = new HttpRequest();
                string result = httpRequest.GetRequest(requestUrl, ref requeststatus);

                string pendingStatus = System.Configuration.ConfigurationManager.AppSettings["InsideOrderStatus"];

                if (requeststatus)
                {
                    List<InsideOrderNoJsonItem> partnos = JsonConvert.DeserializeObject<List<InsideOrderNoJsonItem>>(result);
                    foreach (var item in partnos)
                    {
                        if (item.状态 == pendingStatus)
                        {
                            InsideOrderNoItem m = new InsideOrderNoItem();
                            m.Createtime = item.时间;
                            m.OrderNo = item.报关单号;
                            m.Operator = item.操作人;
                            orderNoList.Add(m);
                            Console.WriteLine(m.OrderNo);
                        }                        
                    }
                }

                return orderNoList;
            }
            catch (Exception ex)
            {
                return orderNoList;
            }
        }

        private void getOrder(List<InsideOrderNoItem> OrderNos)
        {           
            foreach (var p in OrderNos)
            {
                string datenow = DateTime.Now.ToString("yyyy-MM-dd");
                Console.WriteLine("Loop");
                Console.WriteLine(p.OrderNo);
                var message = new Needs.Ccs.Services.Views.MessageView().Where(item => item.Summary == p.OrderNo).FirstOrDefault();
                if (message == null)
                {                 
                    string url = Needs.Ccs.Services.InsideOrder.GetUrl.Replace("idpara", p.OrderNo);
                    bool bSuccess = true;
                    string result = Needs.Ccs.Services.HttpRequest.GetRequest(url, ref bSuccess);
                    Console.WriteLine(bSuccess);
                    if (bSuccess)
                    {
                        try
                        {
                            List<InsideOrderJsonItem> Items = JsonConvert.DeserializeObject<List<InsideOrderJsonItem>>(result);
                            //TODO:向后确认这个值怎么取
                            string AdditionWeight = "0";
                            //创建InsidePost记录，并持久化
                            IcgooMQ mq = new IcgooMQ();
                            mq.ID = ChainsGuid.NewGuidUp();
                            mq.PostData = result;
                            mq.IsAnalyzed = true;
                            mq.Status = Needs.Ccs.Services.Enums.Status.Normal;
                            mq.UpdateDate = mq.CreateDate = DateTime.Now;
                            mq.CompanyType = Needs.Ccs.Services.Enums.CompanyTypeEnums.Inside;
                            mq.Summary = p.OrderNo;
                            mq.AdditionWeight = Convert.ToInt16(AdditionWeight);

                            mq.Enter();
                            Console.WriteLine("信息持久化");
                            string UserName = System.Configuration.ConfigurationManager.AppSettings["UserName"];
                            string Password = System.Configuration.ConfigurationManager.AppSettings["Password"];
                            string HostName = System.Configuration.ConfigurationManager.AppSettings["HostName"];
                            string Port = System.Configuration.ConfigurationManager.AppSettings["Port"];
                            string VirtualHost = System.Configuration.ConfigurationManager.AppSettings["VirtualHost"];
                            //加入队列
                            MQMethod mqMethod = new MQMethod(UserName, Password, HostName, Convert.ToInt16(Port), VirtualHost);
                            string returnmsg = "";
                            bool isSuccess = mqMethod.ProduceInside(mq.ID, ref returnmsg);
                            Console.WriteLine("加入消息队列");
                            //写日志
                            OnGetDone(new InsideGetRequestEventArgs(datenow +" "+p.OrderNo,  url, "", isSuccess));
                            //发短信
                            if (!isSuccess)
                            {
                                OnError(new InsideGetRequestEventArgs(datenow + " " + p.OrderNo,  url, "", isSuccess));
                            }
                        }
                        catch (Exception ex)
                        {
                            OnError(new InsideGetRequestEventArgs(datenow + " " + p.OrderNo, url, "", false));
                        }
                    }
                }     
            }
        }      

        private void OnGetDone(InsideGetRequestEventArgs args)
        {
            this.InsideGetRequest?.Invoke(this, args);
        }

        private void OnError(InsideGetRequestEventArgs args)
        {
            this.InsideSendSMS?.Invoke(this, args);
        }
        private void Log(object sender, InsideGetRequestEventArgs e)
        {
            try
            {
                IcgooRequestLog log = new IcgooRequestLog();
                log.ID = Guid.NewGuid().ToString("N").ToUpper();
                log.Supplier = e.DateNow;
                log.RunPara = e.Url;
                log.Info = e.Info;
                log.IsSend = false;
                log.IsSuccess = e.IsSuccess;
                log.Updatetime = log.Createtime = DateTime.Now;
                log.CompanyType = CompanyTypeEnums.Inside;
                log.Enter();
            }
            catch(Exception ex)
            {

            }
        }

        private void SendMessage(object sender, InsideGetRequestEventArgs e)
        {
            using (var view = new Needs.Ccs.Services.Views.SMSContactView())
            {
                foreach (SMSContact s in view.ToList())
                {
                    string sendContent = e.DateNow + "内单获取失败，请查看";
                    SendSMS.SendMessage(s.Phone, sendContent);
                }
            }
        }
    }
}
