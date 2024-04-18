using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService
{
    public interface IApiCallBack
    {
        void SetNotice(ApiNotice notice);

        void CallBack();
    }

    public partial class ApiCallBackFactory
    {
        public static IApiCallBack Create(Client client)
        {
            switch (client.ClientCode)
            {
                case "WL888"://ICGOO               
                    return new IcgooCallBack();

                case "XL001"://dyj
                case "XL111":
                case "XL037":
                    return new DyjCallBack();

                case "WL666"://快包
                    return new KbCallBack();

                case "XL002":
                case "XL033":
                case "XL035":
                case "XL008":
                    return new IcgooInXDTCallBack();

                default:
                    return null;
            }
        }
    }

    public abstract class ApiCallBack : IApiCallBack
    {
        protected ApiClient Client { get; set; }
        protected ApiNotice ApiNotice { get; set; }
        protected ApiSetting ApiSetting { get; private set; }

        public ApiCallBack(string key)
        {
            //根据key 加载不同的信息
            this.Client = ApiService.Current.Clients[key];//根据配置 加载当前客户,索引器的使用

            //全局变量中加载ApiSettings
            this.ApiSetting = ApiService.Current.ApiSettings[key];//根据配置 加载当前客户,索引器的使用
        }

        public void SetNotice(ApiNotice notice)
        {
            this.ApiNotice = notice;
        }

        /// <summary>
        /// 信息推送
        /// </summary>
        public void CallBack()
        {
            try
            {
                if (this.ApiNotice.PushType == PushType.ClassifyResult)
                {                   
                    //推送归类结果
                    var cResult = new Needs.Ccs.Services.Views.Alls.PreClassifyProductsAll()[this.ApiNotice.ItemID];
                    this.PushClassifyResult(cResult);
                }
                else if(this.ApiNotice.PushType == PushType.DutiablePrice)
                {
                    //推送完税价格
                    var decHead = new Needs.Ccs.Services.Views.DecHeadsView()[this.ApiNotice.ItemID];
                    this.PushDutiablePrice(decHead);
                }
                else if (this.ApiNotice.PushType == PushType.TariffDiff)
                {
                    var orderItem = new Needs.Ccs.Services.Views.OrderItemsView()[this.ApiNotice.ItemID];
                    if (orderItem != null)
                    {
                        var preProduct = new Needs.Ccs.Services.Views.IcgooPreProductView().Where(t => t.sale_orderline_id == orderItem.ProductUniqueCode).FirstOrDefault();
                        this.PushIcgooTariffDiff(orderItem, preProduct?.supplier);
                    }
                   
                }
            }
            catch (Exception ex)
            {
                //推送异常发送邮件
                //string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                //var message = "信息推送ID：【" + this.ApiNotice.ID + "】, 推送时间：【" + DateTime.Now + "】, 异常原因：【" + ex.Message + "】";
                //SmtpContext.Current.Send(receivers, "信息推送异常", message);
                //WriteTxt write = new WriteTxt(ex.ToString(), "log.txt");
                //write.Write();

                throw new Exception(ex.Message);
            }
        }

        abstract protected void PushClassifyResult(PreClassifyProduct cResult);
        abstract protected void PushDutiablePrice(DecHead decHead);
        abstract protected void PushIcgooTariffDiff(OrderItem cResult,string supplier);

        /// <summary>
        /// 接口提交
        /// </summary>
        /// <param name="url">接口地址</param>
        /// <param name="msg">推送报文</param>
        /// <returns>响应报文</returns>
        protected string PushMsg(string url, string msg)
        {
            HttpRequest request = new HttpRequest();
            request.Timeout = this.ApiSetting.Timeout;
            request.ContentType = this.ApiSetting.ContentType;
            request.Headers = this.ApiSetting.Headers;
            return request.Post(url, msg);
        }

        /// <summary>
        /// 推送完成后，如果全部推送成功，则更新ApiNotice状态，否则发送邮件
        /// </summary>
        /// <param name="isSuccess">推送是否成功</param>
        /// <param name="postdata">推送报文</param>
        /// <param name="result">响应报文</param>
        protected void AfterPush(bool isSuccess, string postdata, string result)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                if (isSuccess)
                {
                    //全部推送成功，更新ApiNotice状态，
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ApiNotices>(
                        new
                        {
                            PushStatus = PushStatus.Pushed,
                            UpdateDate = DateTime.Now
                        }, item => item.ItemID == this.ApiNotice.ItemID);
                }
                else
                {
                    //推送失败，更新ApiNotice状态，
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.ApiNotices>(
                        new
                        {
                            PushStatus = PushStatus.PushFailure,
                            UpdateDate = DateTime.Now
                        }, item => item.ItemID == this.ApiNotice.ItemID);

                    //推送失败，发送邮件
                    //string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                    //var message = "信息推送ID：【" + this.ApiNotice.ID + "】, 推送时间：【" + DateTime.Now + "】, 推送报文：【" + postdata + "】, 响应报文：【" + result + "】";
                    //SmtpContext.Current.Send(receivers, "信息推送失败", message);
                }
            }
        }
    }
}
