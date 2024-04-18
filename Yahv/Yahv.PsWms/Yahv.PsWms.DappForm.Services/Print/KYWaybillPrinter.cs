using kyeSDK;
using kyeSDK.model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PsWms.PvRoute.Services;
using Yahv.Utils.Serializers;

namespace Yahv.PsWms.DappForm.Services.Print
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
                                            Source = (int)PrintSource.KY,
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
}
