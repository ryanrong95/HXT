using Kdn.Library.Models;
using kyeSDK;
using kyeSDK.model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinApp.Services.Print;
using Yahv.Utils.Serializers;

namespace WinApp.Services
{
    public class KYWaybillPrinter
    {

        public event SuccessEventHandler Success;
        public KYWaybillPrinter()
        {

        }

        /// <summary>
        /// 提交电子运单接口并打印
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="shipperCode"></param>
        /// <param name="expType">快递类型</param>
        /// <param name="exPayType">支付方式</param>
        /// <param name="sender"></param>
        /// <param name="receiver"></param>
        /// <param name="quantity"></param>
        /// <param name="remark"></param>
        /// <param name="volume"></param>
        /// <param name="weight"></param>
        /// <param name="commodities"></param>

        public void WayBillPrinterTools(string resJson, string printerName, string orderID)
        {
            try
            {
                //System.Windows.Forms.MessageBox.Show(resJson);
                string appkey = "80631";
                string appsecret = "6C4E0274101AEF1BA4AD31EF866D572D";
                DefaultKyeClient kyeClient = new DefaultKyeClient(appkey, appsecret);
                JObject rlt = resJson.JsonTo<JObject>();
                var success = rlt["success"].Value<bool>();
                //Console.WriteLine(resJson2);

                if (success == true)
                {
                    JToken message = rlt["data"];
                    foreach (var data in message)
                    {
                        string waybillNumber = data["waybillNumber"].Value<string>();
                        string myID = data["orderId"].Value<string>();//获取传到快递鸟的订单编号（根据订单号生成的编号）

                        //打印参数
                        string printInfo = new
                        {
#if DEBUG ||TEST
                            customerCode = "075525131031",//客户编码（寄件公司账号），必须是我们公司的账号，不同于paymentCustomer
                            //customerCode = "075517225569",//客户编码（寄件公司账号），必须是我们公司的账号，不同于paymentCustomer

                            //customerCode = "075568610585",//客户编码（寄件公司账号），必须是我们公司的账号，不同于paymentCustomer，2021.4.1启用

                            //platformFlag = "9C59E00421A331E2EFD807518A995305",//生产版本
                            platformFlag = "6820933B508C2977269F78B2DCD329CB",//测试版本
#else
                            //customerCode = "075517225569",//客户编码（寄件公司账号），必须是我们公司的账号，不同于paymentCustomer
                             customerCode = "075568610585",//客户编码（寄件公司账号），必须是我们公司的账号，不同于paymentCustomer，2021.4.1启用
                            platformFlag = "9C59E00421A331E2EFD807518A995305",
#endif
                            printerNo = printerName,//"Y1000018150604" printerName
                            templateSizeType = "1",//模板尺寸类型 ：（0 -两联100*136模版，1-三联100*210模版）
                            waybillNumberInfos = new[]
                            {
                                new
                                {
                                   waybillNumber = waybillNumber
                                }
                            }
                        }.Json();

#if DEBUG || TEST

                        //打印
                        string resJso3 = kyeClient.Execute(new KyeRequest
                        {
                            contentType = "application/json",
                            methodCode = "open.api.openCommon.cloudPrint",
                            paramInfo = printInfo,
                            responseFormat = "json"
                        }, false);  //沙盒环境执行下单 生产不传false 默认 为生产

#else
                        //打印
                        string resJso3 = kyeClient.Execute(new KyeRequest
                        {
                            contentType = "application/json",
                            methodCode = "open.api.openCommon.cloudPrint",
                            paramInfo = printInfo,
                            responseFormat = "json"
                        });  //沙盒环境执行下单 生产不传false 默认 为生产
#endif

                        JObject rlt3 = resJso3.JsonTo<JObject>();
                        var code3 = rlt3["code"].Value<int>();
                        var msg3 = rlt3["msg"].Value<string>();

                        //成功打印请求数据库接口存入我方数据
                        if (code3 == 10000)
                        {
                            //跨越不用考虑回签单的问题
                            var success3 = rlt3["success"].Value<bool>();
                            if (success3 == true)
                            {
                                //调用接口存入数据库
                                if (this != null && this.Success != null)
                                {
                                    this.Success(null, new SuccessEventArgs
                                    {
                                        Result = new List<Result>() {
                                        new Result
                                        {
                                            Code = waybillNumber,
                                            Type = 1,
                                            CreatorID = "NPC",
                                            Html = null,
                                            SendJson = resJson,
                                            ReceiveJson = "",
                                            Source = (int)Yahv.PsWms.PvRoute.Services.PrintSource.KY,
                                            MainID = orderID,//原订单号
                                            MyID = myID
                                        }

                                    }
                                    });
                                }
                            }

                            else
                            {

                                System.Windows.Forms.MessageBox.Show(msg3);
                            }
                        }

                        //处理一下打印不成功的情况
                        else
                        {
                            System.Windows.Forms.MessageBox.Show(msg3);
                        }

                    }
                }
                else
                {
                    var msg = rlt["msg"].Value<string>();
                    System.Windows.Forms.MessageBox.Show(msg);
                }
            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show((ex.InnerException ?? ex).Message);
            }
        }
    }

    #region 以前
    //public class KYWaybillPrinter
    //{

    //    public event SuccessEventHandler Success;
    //    public KYWaybillPrinter()
    //    {

    //    }

    //    /// <summary>
    //    /// 提交电子运单接口并打印
    //    /// </summary>
    //    /// <param name="orderID"></param>
    //    /// <param name="shipperCode"></param>
    //    /// <param name="expType">快递类型</param>
    //    /// <param name="exPayType">支付方式</param>
    //    /// <param name="sender"></param>
    //    /// <param name="receiver"></param>
    //    /// <param name="quantity"></param>
    //    /// <param name="remark"></param>
    //    /// <param name="volume"></param>
    //    /// <param name="weight"></param>
    //    /// <param name="commodities"></param>

    //    public void WayBillPrinterTools(string resJson, string printerName)
    //    {
    //        try
    //        {
    //            string appkey = "80631";
    //            string appsecret = "6C4E0274101AEF1BA4AD31EF866D572D";
    //            DefaultKyeClient kyeClient = new DefaultKyeClient(appkey, appsecret);
    //            JObject rlt = resJson.JsonTo<JObject>();
    //            var success = rlt["success"].Value<bool>();
    //            //Console.WriteLine(resJson2);

    //            if (success == true)
    //            {
    //                JToken message = rlt["data"];
    //                foreach (var data in message)
    //                {
    //                    string waybillNumber = data["waybillNumber"].Value<string>();

    //                    //打印参数
    //                    string printInfo = new
    //                    {
    //                        customerCode = "075525131031",
    //                        platformFlag = "6820933B508C2977269F78B2DCD329CB",
    //                        printerNo = printerName,//"Y1000018150604" printerName
    //                        templateSizeType = "1",//模板尺寸类型 ：（0 -两联100*136模版，1-三联100*210模版）
    //                        waybillNumberInfos = new[]
    //                        {
    //                            new
    //                            {
    //                               waybillNumber = waybillNumber
    //                            }
    //                        }
    //                    }.Json();

    //                    //调用接口存入数据库
    //                    if (this != null && this.Success != null)
    //                    {
    //                        this.Success(null, new SuccessEventArgs
    //                        {
    //                            Result = new Result
    //                            {
    //                                Code = waybillNumber,
    //                                CreatorID = "NPC",
    //                                Html = null,
    //                                SendJson = resJson,
    //                                ReceiveJson = "",
    //                                Source = (int)PrintSource.KYSY,
    //                            }
    //                        });
    //                    }

    //                    //打印
    //                    string resJso3 = kyeClient.Execute(new KyeRequest
    //                    {
    //                        contentType = "application/json",
    //                        methodCode = "open.api.openCommon.cloudPrint",
    //                        paramInfo = printInfo,
    //                        responseFormat = "json"
    //                    }, false);  //沙盒环境执行下单 生产不传false 默认 为生产


    //                }
    //            }
    //            else
    //            {
    //                var msg = rlt["msg"].Value<string>();
    //                System.Windows.Forms.MessageBox.Show(msg);
    //            }
    //        }
    //        catch (Exception ex)
    //        {

    //            throw;
    //        }
    //    }
    //}

    //#region 跨越需要的类
    ////public class KyRequest
    ////{
    ////    /// <summary>
    ////    /// 月结账号
    ////    /// </summary>
    ////    public string customerCode { get; set; }

    ////    /// <summary>
    ////    /// 客户/平台标识
    ////    /// </summary>
    ////    public string platformFlag { get; set; }

    ////    /// <summary>
    ////    /// 订单信息
    ////    /// </summary>
    ////    public OrderInfo[] orderInfos { get; set; }

    ////    /// <summary>
    ////    /// 客户编码和订单号校验：10-校验；20-不校验，可空
    ////    /// </summary>
    ////    public string repeatCheck { get; set; }

    ////}

    ////public class OrderInfo
    ////{

    ////    /// <summary>
    ////    /// 运单号，可空
    ////    /// </summary>
    ////    public string waybillNumber { get; set; }

    ////    /// <summary>
    ////    /// 发件人
    ////    /// </summary>
    ////    public Delivery preWaybillDelivery { get; set; }

    ////    /// <summary>
    ////    /// 收件人
    ////    /// </summary>
    ////    public WaybillPickup preWaybillPickup { get; set; }

    ////    /// <summary> 
    ////    /// 快递方式，必填
    ////    /// </summary>
    ////    /// <remarks>
    ////    /// serviceMode=10:表示当天达;serviceMode=20:表示次日达;serviceMode=30:表示隔日达;serviceMode=40:表示陆运件;serviceMode=50:表示同城次日;serviceMode=60:表示次晨达;serviceMode=70:表示同城即日;serviceMode=80：表示航空件;serviceMode=160:表示省内次日;serviceMode=170:表示省内即日;serviceMode=210:表示空运;serviceMode=220:表示专运（传代码）
    ////    /// </remarks>
    ////    public int serviceMode { get; set; }

    ////    /// <summary>
    ////    /// 支付方式，必填
    ////    /// </summary>
    ////    /// <remarks>
    ////    /// 10-寄方付 ，20-收方付 ，30-第三方付  （传代码）
    ////    /// </remarks>
    ////    public int payMode { get; set; }

    ////    /// <summary>
    ////    /// 付款账号，可空
    ////    /// </summary>
    ////    /// <remarks>
    ////    /// 当付款方式=10-寄方付 或者 30-第三方付 时，付款账号必填
    ////    /// </remarks>
    ////    public string paymentCustomer { get; set; }

    ////    /// <summary>
    ////    /// 最大支持200个汉字/数字/英文/符号等,必传参数
    ////    /// </summary>
    ////    public string goodsType { get; set; }

    ////    /// <summary>
    ////    /// 总件数（方便安排车型取货，最好提供件数、重量），可空
    ////    /// </summary>
    ////    public int? count { get; set; }

    ////    /// <summary>
    ////    /// 重量要么不传值，要么值必须大于0、单位：kg，可空
    ////    /// </summary>
    ////    public double? actualWeight { get; set; }

    ////    /// <summary>
    ////    /// 10-打卡板， 20-打木架 ，30-打木箱   （传代码），可空
    ////    /// </summary>
    ////    public int? woodenFrame { get; set; }

    ////    /// <summary>
    ////    /// 客户系统的订单号，字段长度控制在32个字符之内
    ////    /// </summary>
    ////    public string orderId { get; set; }

    ////    /// <summary>
    ////    /// 收件公司包含“唯品会”三个字为必传，字段长度控制在32个字符之内，可空
    ////    /// </summary>
    ////    public string productCode { get; set; }

    ////    /// <summary>
    ////    /// 代收金额，可空
    ////    /// </summary>
    ////    public double? collectionAmount { get; set; }

    ////    /// <summary>
    ////    /// 保价值，可空
    ////    /// </summary>
    ////    public double? insuranceValue { get; set; }

    ////    /// <summary>
    ////    /// 收货标志，必填
    ////    /// </summary>
    ////    /// <remarks>
    ////    /// 10：要求返回签回单原件（含电子回单图片），20：不要求返回签回单，30：要求返回签电子回单图片（传代码）
    ////    /// </remarks>
    ////    public int receiptFlag { get; set; }

    ////    /// <summary>
    ////    /// 回单份数，可空
    ////    /// </summary>
    ////    public int? receiptCount { get; set; }

    ////    /// <summary>
    ////    /// 字段长度最大限制200字符，可空
    ////    /// </summary>
    ////    public string waybillRemark { get; set; }

    ////    /// <summary>
    ////    /// 规格，可空
    ////    /// </summary>
    ////    public string specification { get; set; }

    ////    /// <summary>
    ////    /// 客户系统的装运单号，可空
    ////    /// </summary>
    ////    public string shippingCode { get; set; }

    ////    /// <summary>
    ////    /// 客户系统的发运单号，可空
    ////    /// </summary>
    ////    public string[] taskNum { get; set; }

    ////    /// <summary>
    ////    /// 是否预约取货，可空
    ////    /// </summary>
    ////    /// <remarks>
    ////    /// 10-是，表示根据货好时间，预约上门揽收，20-否，表示线下自主联系揽收  （传代码，是否预约取货为“10”时，货好时间字段必填，同时根据货好时间安排司机上门揽收）
    ////    /// </remarks>
    ////    public int? dismantling { get; set; }

    ////    /// <summary>
    ////    /// 是否自动下单为“10”时，货好时间才有效，且必填 ，时间格为: yyyy-MM-dd HH:mm，可空
    ////    /// </summary>
    ////    public string goodsTime { get; set; }

    ////    /// <summary>
    ////    /// 10-是，20-否  （传代码），可空
    ////    /// </summary>
    ////    public string subscriptionService { get; set; }

    ////    /// <summary>
    ////    /// 10-是，20-否 （是否自动下单为“10”时，需维护。如送往唯品会仓库的货物且为大货，就传10 ），可空
    ////    /// </summary>
    ////    public string bulkWarehous { get; set; }

    ////    /// <summary>
    ////    /// 货物尺寸，可空
    ////    /// </summary>
    ////    public WaybillGoodsSize[] preWaybillGoodsSizeList { get; set; }

    ////    /// <summary>
    ////    /// 10-打印子母件，20-打印取货标签  （传代码，根据件数生产子单号或者取货标签）,30-扫描发运单，40-扫描发运单及尺寸，可空
    ////    /// </summary>
    ////    public string additionalService { get; set; }

    ////    /// <summary>
    ////    /// 日期格式：yyyy-MM-dd HH:mm 且检验必须大于当前时间，可空
    ////    /// </summary>
    ////    public string deliveryTime { get; set; }

    ////    /// <summary>
    ////    /// 订单尺寸，可空
    ////    /// </summary>
    ////    public OrderNoSizeRelation[] orderNoSizeRelations { get; set; }

    ////    /// <summary>
    ////    /// 图片订阅：10-回单
    ////    /// </summary>
    ////    public string[] pictureSubscription { get; set; }
    ////}

    /////// <summary>
    /////// 发件人
    /////// </summary>
    ////public class Delivery
    ////{
    ////    /// <summary>
    ////    /// 省，可空
    ////    /// </summary>
    ////    public string provinceName { get; set; }
    ////    /// <summary>
    ////    /// 市，可空
    ////    /// </summary>
    ////    public string cityName { get; set; }


    ////    /// <summary>
    ////    /// 区，可空
    ////    /// </summary>
    ////    public string countyName { get; set; }

    ////    /// <summary>
    ////    /// 详细地址
    ////    /// </summary>
    ////    public string address { get; set; }
    ////    public string companyName { get; set; }

    ////    public string person { get; set; }

    ////    public string mobile { get; set; }

    ////    public string phone { get; set; }

    ////    public Delivery()
    ////    {
    ////    }

    ////}

    /////// <summary>
    /////// 收件人
    /////// </summary>
    ////public class WaybillPickup : Delivery
    ////{

    ////    public WaybillPickup()
    ////    {
    ////    }
    ////}

    /////// <summary>
    /////// 货物尺寸
    /////// </summary>
    ////public class WaybillGoodsSize
    ////{
    ////    /// <summary>
    ////    /// 长，可空
    ////    /// </summary>
    ////    /// <remarks>
    ////    /// 1.长*宽*高*数量要么都有值，要么都没值
    ////    /// 2.单位：cm 长、宽、高任意一边不能大于700CM
    ////    /// </remarks>
    ////    public string length { get; set; }

    ////    /// <summary>
    ////    /// 宽，可空
    ////    /// </summary>
    ////    /// <remarks>
    ////    /// 1.长*宽*高*数量要么都有值，要么都没值
    ////    /// 2.单位：cm 长、宽、高任意一边不能大于700CM
    ////    /// </remarks>
    ////    public string width { get; set; }

    ////    /// <summary>
    ////    /// 高，可空
    ////    /// </summary>
    ////    /// <remarks>
    ////    /// 1.长*宽*高*数量要么都有值，要么都没值
    ////    /// 2.单位：cm 长、宽、高任意一边不能大于700CM
    ////    /// </remarks> 
    ////    public string height { get; set; }

    ////    /// <summary>
    ////    ///数量，可空
    ////    /// </summary>
    ////    public string count { get; set; }

    ////    /// <summary>
    ////    /// 货物编码，可空
    ////    /// </summary>
    ////    public string goodsCode { get; set; }

    ////    /// <summary>
    ////    /// 货物名称，可空
    ////    /// </summary>
    ////    public string goodsName { get; set; }

    ////    /// <summary>
    ////    /// 货物重量，单位：kg，可空
    ////    /// </summary>
    ////    public string goodsWeight { get; set; }

    ////    /// <summary>
    ////    /// 货物体积，单位：m³，可空
    ////    /// </summary>
    ////    public string goodsVolume { get; set; }


    ////}

    /////// <summary>
    /////// 订单尺寸
    /////// </summary>
    ////public class OrderNoSizeRelation
    ////{
    ////    /// <summary>
    ////    /// 订单号，可空
    ////    /// </summary>
    ////    public string orderNo { get; set; }

    ////    /// <summary>
    ////    /// 订单尺寸，可空
    ////    /// </summary>
    ////    public OrderNoSize orderNoSize { get; set; }

    ////    /// <summary>
    ////    /// 重量，可空
    ////    /// </summary>
    ////    public decimal? weight { get; set; }
    ////}

    ////public class OrderNoSize
    ////{
    ////    /// <summary>
    ////    /// 长，可空
    ////    /// </summary>
    ////    /// <remarks>
    ////    /// 单位：cm 长、宽、高任意一边不能大于700CM
    ////    /// </remarks>
    ////    public string length { get; set; }

    ////    /// <summary>
    ////    /// 宽，可空
    ////    /// </summary>
    ////    /// <remarks>
    ////    /// 单位：cm 长、宽、高任意一边不能大于700CM
    ////    /// </remarks>
    ////    public string width { get; set; }

    ////    /// <summary>
    ////    /// 高，可空
    ////    /// </summary>
    ////    /// <remarks>
    ////    /// 单位：cm 长、宽、高任意一边不能大于700CM
    ////    /// </remarks> 
    ////    public string height { get; set; }
    ////}

    //#endregion

    #endregion
}
