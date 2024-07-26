using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views.Alls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Needs.Wl.PlanningService
{
    /// <summary>
    /// 华芯通接口计划任务服务
    /// </summary>
    public partial class PlanningService : ServiceBase
    {
        //站点配置：华芯通/创新恒远
        private static string site;
        public PlanningService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            site = System.Configuration.ConfigurationManager.AppSettings["Site"];

            //日志记录
            System.Timers.Timer timerPreProduct = new System.Timers.Timer();//预归类
            timerPreProduct.Interval = 10 * 60 * 1000;// 10分钟执行一次
            timerPreProduct.Elapsed += TimerPreProduct_Elapsed;
            timerPreProduct.Enabled = true;
            timerPreProduct.Start();

            System.Timers.Timer timerConsultProduct = new System.Timers.Timer();//咨询产品
            timerConsultProduct.Interval = 3 * 60 * 1000; // 10分钟执行一次
            timerConsultProduct.Elapsed += TimerConsultProduct_Elapsed;
            timerConsultProduct.Enabled = true;
            timerConsultProduct.Start();

            System.Timers.Timer timerOrder = new System.Timers.Timer();//大赢家订单
            timerOrder.Interval = 10 * 60 * 1000; // 5分钟执行一次
            timerOrder.Elapsed += TimerOrder_Elapsed;
            timerOrder.Enabled = true;
            timerOrder.Start();

            System.Timers.Timer timerPush = new System.Timers.Timer();//信息推送
            timerPush.Interval = 2 * 60 * 1000; // 5分钟执行一次
            timerPush.Elapsed += TimerPush_Elapsed;
            timerPush.Enabled = true;
            timerPush.Start();

            System.Timers.Timer timerPushDec = new System.Timers.Timer();//信息推送
            timerPushDec.Interval = 10 * 60 * 1000; // 5分钟执行一次
            timerPushDec.Elapsed += TimerPushDec_Elapsed;
            timerPushDec.Enabled = true;
            timerPushDec.Start();

            System.Timers.Timer timerClientIDUpdate = new System.Timers.Timer();//信息推送
            timerClientIDUpdate.Interval = 5 * 60 * 1000; // 5分钟执行一次
            timerClientIDUpdate.Elapsed += TimerClientIDUpdate_Elapsed;
            timerClientIDUpdate.Enabled = true;
            timerClientIDUpdate.Start();


            //System.Timers.Timer DBSAccountFlow = new System.Timers.Timer();//信息推送
            //DBSAccountFlow.Interval = 30 * 60 * 1000; // 5分钟执行一次
            //DBSAccountFlow.Elapsed += TimerPush_Elapsed;
            //DBSAccountFlow.Enabled = true;
            //DBSAccountFlow.Start();

            //System.Timers.Timer timerWarnOneHour = new System.Timers.Timer();//信息推送
            //timerPush.Interval = 60*60*1000; // 60分钟执行一次
            //timerPush.Elapsed += TimerNoticeOneHour_Elapsed;
            //timerPush.Enabled = true;
            //timerPush.Start();

            //System.Timers.Timer timerWarnHalfHour = new System.Timers.Timer();//信息推送
            //timerPush.Interval = 30*60*1000; // 30分钟执行一次
            //timerPush.Elapsed += TimerNoticeHalfHour_Elapsed;
            //timerPush.Enabled = true;
            //timerPush.Start();

            ThreadStart threadStart = new ThreadStart(CreateOrder);
            Thread thread = new Thread(threadStart);
            thread.IsBackground = true;
            thread.Start();
            
            //推送Icgoo订单 预归类关税和下单关税不一致
            ThreadStart threadTariffDiffStart = new ThreadStart(PushTariffDiff);
            Thread threadTaiffDiff = new Thread(threadTariffDiffStart);
            threadTaiffDiff.IsBackground = true;
            threadTaiffDiff.Start();

            SendMail("开始");
        }


        //推送产品归类结果、产品完税价格
        private void TimerPush_Elapsed(object sender, ElapsedEventArgs e)
        {         
            if (DateTime.Now.Hour >= 3)
            {
                var currentClient = ApiService.Current.Clients.Where(t => t.Site == site).FirstOrDefault();

                //获取要推送的信息
                IEnumerable<ApiNotice> list = new ApiNoticesAll().Where(item => item.PushStatus == PushStatus.Unpush 
                && item.ClientID == currentClient.ID 
                && item.PushType == PushType.ClassifyResult).ToArray().Take(10);

                List<string> apiNoticeIds = list.Select(t => t.ID).ToList();
                //状态改为推送中
                using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    responsitory.Update<Layer.Data.Sqls.ScCustoms.ApiNotices>(
                                new
                                {
                                    UpdateDate = DateTime.Now,
                                    PushStatus = (int)PushStatus.Pushing
                                }, item => apiNoticeIds.Contains(item.ID));
                }

                //推送
                foreach (var notice in list)
                {
                    try
                    {
                        IApiCallBack callBack = ApiCallBackFactory.Create(notice.Client);
                        callBack?.SetNotice(notice);
                        callBack?.CallBack();
                    }
                    catch (Exception ex)
                    {
                        using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                        {
                            //记录错误
                            responsitory.Insert(new Layer.Data.Sqls.ScCustoms.ApiNoticeLogs
                            {
                                ID = Ccs.Services.ChainsGuid.NewGuidUp(),
                                ApiNoticeID = notice.ID,
                                PushMsg = "",
                                ResponseMsg = ex.ToString(),
                                CreateDate = DateTime.Now,
                            });

                            if (ex.Message.Contains("远程服务器返回错误"))
                            {
                                //推送异常，更新ApiNotice状态，
                                responsitory.Update<Layer.Data.Sqls.ScCustoms.ApiNotices>(
                                new
                                {
                                    PushStatus = PushStatus.Unpush,
                                    UpdateDate = DateTime.Now
                                }, item => item.ID == notice.ID);
                            }
                            else
                            {
                                //推送异常，更新ApiNotice状态，
                                responsitory.Update<Layer.Data.Sqls.ScCustoms.ApiNotices>(
                                new
                                {
                                    PushStatus = PushStatus.PushFailure,
                                    UpdateDate = DateTime.Now
                                }, item => item.ID == notice.ID);

                                //邮件发送
                                string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                                SmtpContext.Current.Send(receivers, "推送归类或完税价格失败", ex.Message);
                            }
                        }

                        System.Threading.Thread.Sleep(2000);
                        continue;
                    }
                }
            }
        }

        private void TimerPushDec_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (DateTime.Now.Hour >= 3)
            {
                var currentClient = ApiService.Current.Clients.Where(t => t.Site == site).FirstOrDefault();

                //获取要推送的信息
                IEnumerable<ApiNotice> list = new ApiNoticesAll().Where(item => item.PushStatus == PushStatus.Unpush 
                && item.ClientID == currentClient.ID&& item.PushType != PushType.ClassifyResult).ToArray().Take(5);

                List<string> apiNoticeIds = list.Select(t => t.ID).ToList();

                //状态改为推送中
                using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    responsitory.Update<Layer.Data.Sqls.ScCustoms.ApiNotices>(
                                new
                                {
                                    UpdateDate = DateTime.Now,
                                    PushStatus = (int)PushStatus.Pushing
                                }, item => apiNoticeIds.Contains(item.ID));
                }

                //推送
                foreach (var notice in list)
                {
                    try
                    {
                        IApiCallBack callBack = ApiCallBackFactory.Create(notice.Client);
                        callBack?.SetNotice(notice);
                        callBack?.CallBack();
                    }
                    catch (Exception ex)
                    {
                        using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                        {
                            //记录错误
                            responsitory.Insert(new Layer.Data.Sqls.ScCustoms.ApiNoticeLogs
                            {
                                ID = Ccs.Services.ChainsGuid.NewGuidUp(),
                                ApiNoticeID = notice.ID,
                                PushMsg = "",
                                ResponseMsg = ex.ToString(),
                                CreateDate = DateTime.Now,
                            });

                            if (ex.Message.Contains("远程服务器返回错误"))
                            {
                                //推送异常，更新ApiNotice状态，
                                responsitory.Update<Layer.Data.Sqls.ScCustoms.ApiNotices>(
                                new
                                {
                                    PushStatus = PushStatus.Unpush,
                                    UpdateDate = DateTime.Now
                                }, item => item.ID == notice.ID);
                            }
                            else
                            {
                                //推送异常，更新ApiNotice状态，
                                responsitory.Update<Layer.Data.Sqls.ScCustoms.ApiNotices>(
                                new
                                {
                                    PushStatus = PushStatus.PushFailure,
                                    UpdateDate = DateTime.Now
                                }, item => item.ID == notice.ID);

                                //邮件发送
                                string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                                SmtpContext.Current.Send(receivers, "推送归类或完税价格失败", ex.Message);
                            }
                        }

                        System.Threading.Thread.Sleep(2000);
                        continue;
                    }
                }
            }
        }

        //获取订单
        private void TimerOrder_Elapsed(object sender, ElapsedEventArgs e)
        {          
            if (DateTime.Now.Hour >= 3)
            {
                // 订单只主动获取大赢家的，其他公司都是主动推送给我们
                var client = ApiService.Current.Clients["dyj"];
                if (client.Site.ToLower() == site.ToLower())
                {
                    IOrderRequest OrderRequest = OrderRequestFactory.Create(client.Key);
                    OrderRequest.Process();
                }
            }              
        }

        //获取预归类产品
        private void TimerPreProduct_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {              
                if (DateTime.Now.Hour >= 3)
                {
                    foreach (ApiClient client in ApiService.Current.Clients)
                    {
                        if (client.Site.ToLower() == site.ToLower())
                        {
                            //获取预归类产品
                            IPreProductRequest preProductRequest = PreProductRequestFactory.Create(client.Key.ToLower());
                            preProductRequest.Process();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteTxt write = new WriteTxt(ex.ToString(), "log.txt");
                write.Write();
            }
                        
        }

        private void TimerConsultProduct_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (DateTime.Now.Hour >= 3)
                {
                    foreach (ApiClient client in ApiService.Current.Clients)
                    {
                        if (site.ToLower().Equals("icgooinxdt"))
                        {
                            IcgooConsultRequest icgooConsultRequest = new IcgooConsultRequest();
                            icgooConsultRequest.Process();
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                WriteTxt write = new WriteTxt(ex.ToString(), "log.txt");
                write.Write();
            }
        }

        protected override void OnStop()
        {
            SendMail("停止");
            //改状态
            using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                responsitory.Update<Layer.Data.Sqls.ScCustoms.ApiNotices>(
                          new
                          {
                              UpdateDate = DateTime.Now,
                              PushStatus = (int)PushStatus.Unpush
                          }, item => item.PushStatus == (int)PushStatus.Pushing);
            }
        }

        private static void CreateOrder()
        {
            if (site == "foric" || site == "eznet")
            {
                while (true)
                {
                    try
                    {
                        DyjOrderCreate dyjOrder = new DyjOrderCreate();
                        dyjOrder.Process();
                    }
                    catch (Exception ex)
                    {
                        string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                        SmtpContext.Current.Send(receivers, "生成大赢家订单异常", ex.Message);
                        continue;
                    }
                }
            }
            else if (site.ToLower().Equals("HXT"))
            {
                while (true)
                {
                    try
                    {
                        IcgooOrderCreate icgooOrder = new IcgooOrderCreate();
                        icgooOrder.Process();
                    }
                    catch (Exception ex)
                    {
                        string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                        SmtpContext.Current.Send(receivers, "生成Icgoo订单异常", ex.Message);
                        continue;
                    }
                }
            }
            else
            {
                while (true)
                {
                    try
                    {
                        IcgooInXDTOrderCreate icgooOrder = new IcgooInXDTOrderCreate();
                        icgooOrder.Process();
                    }
                    catch (Exception ex)
                    {
                        string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                        SmtpContext.Current.Send(receivers, "生成Icgoo订单异常", ex.Message);
                        continue;
                    }
                }
            }
        }

        private void SendMail(string startOrStop)
        {
            string SendTitle = "华芯通计划任务程序" + startOrStop;
            if (site.ToLower() == "HXT")
            {
                SendTitle = "创新恒远计划任务程序" + startOrStop;
            }
            else if (site.ToLower() == "eznet")
            {
                SendTitle = "科睿计划任务程序" + startOrStop;
            }
            else if (site.ToLower() == "icgooinxdt")
            {
                SendTitle = "Icgoo在华芯通计划任务程序" + startOrStop;
            }
            //邮件发送
            string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
            SmtpContext.Current.Send(receivers, SendTitle, SendTitle + "," + startOrStop + "时间：" + DateTime.Now);
        }

        private static void PushTariffDiff()
        {
            while (true)
            {
                try
                {
                    IcgooTariffDiffCheck icgooTariffDiffCheck = new IcgooTariffDiffCheck();
                    icgooTariffDiffCheck.Process();
                }
                catch (Exception ex)
                {
                    //string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                    //SmtpContext.Current.Send(receivers, "推送归类关税和下单关税不一致失败", ex.Message);
                    //System.Threading.Thread.Sleep(2000);
                    WriteTxt write = new WriteTxt(ex.ToString(), "log.txt");
                    write.Write();
                    continue;
                }
            }
            
        }

        private static void TimerDBSAccouontFlow_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (DateTime.Now.Hour == 1)
            {
                string requestUrl = System.Configuration.ConfigurationManager.AppSettings["DBSAREUrl"];
                string OrgId = System.Configuration.ConfigurationManager.AppSettings["Api_OrgId"];
                string CNYAccountNo = System.Configuration.ConfigurationManager.AppSettings["CNYAccountNo"];
                string USDAccountNo = System.Configuration.ConfigurationManager.AppSettings["USDAccountNo"];

                string fromDate = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
                string toDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                Services.DBS.DBSAccountFlow CNYAccountFlow = new Services.DBS.DBSAccountFlow();
                CNYAccountFlow.RequestUrl = requestUrl;
                CNYAccountFlow.AccountNo = CNYAccountNo;
                CNYAccountFlow.ccy = "CNY";
                CNYAccountFlow.fromdate = fromDate;
                CNYAccountFlow.todate = toDate;

                CNYAccountFlow.GetAccountFlow();

                Services.DBS.DBSAccountFlow USDAccountFlow = new Services.DBS.DBSAccountFlow();
                USDAccountFlow.RequestUrl = requestUrl;
                USDAccountFlow.AccountNo = USDAccountNo;
                USDAccountFlow.ccy = "USD";
                USDAccountFlow.fromdate = fromDate;
                USDAccountFlow.todate = toDate;

                USDAccountFlow.GetAccountFlow();
            }
           
        }

        private static void TimerClientIDUpdate_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var currentClient = ApiService.Current.Clients.Where(t => t.Site == site).FirstOrDefault();
                if (currentClient.Code.ToUpper() == "XL002")
                {
                    List<string> clientIDS = new List<string>();
                    string CXZXBJID = System.Configuration.ConfigurationManager.AppSettings["CXZXBJID"];
                    string CXZXSZID = System.Configuration.ConfigurationManager.AppSettings["CXZXSZID"];
                    string SZCXZXID = System.Configuration.ConfigurationManager.AppSettings["SZCXZXID"];
                    string CXZXSDID = System.Configuration.ConfigurationManager.AppSettings["CXZXSDID"];
                    string BJXDNID = System.Configuration.ConfigurationManager.AppSettings["BJXDNID"];
                    string FCGYLID = System.Configuration.ConfigurationManager.AppSettings["FCGYLID"];
                    clientIDS.Add(CXZXBJID);
                    clientIDS.Add(CXZXSZID);
                    clientIDS.Add(SZCXZXID);
                    clientIDS.Add(CXZXSDID);
                    clientIDS.Add(BJXDNID);
                    clientIDS.Add(FCGYLID);

                    using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        responsitory.Update<Layer.Data.Sqls.ScCustoms.ApiNotices>(
                                    new
                                    {
                                        ClientID = currentClient.ID
                                    }, item => clientIDS.Contains(item.ClientID));
                    }
                }
            }
            catch(Exception ex)
            {
                System.Threading.Thread.Sleep(2000);               
            }
        }
    }
}