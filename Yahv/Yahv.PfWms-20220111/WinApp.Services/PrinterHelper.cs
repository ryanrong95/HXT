using Kdn.Library;
using Kdn.Library.Models;
using kyeSDK;
using kyeSDK.model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinApp.Services.Controls;
using Yahv.Utils.Serializers;
using WinApp.Services.Print;
using Yahv.PsWms.PvRoute.Services;
using Yahv.PsWms.PvRoute.Services.Models;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Xml;
using Yahv.Utils.EventExtend;
using AForge.Imaging.Filters;
using Spire.Pdf;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Media.Imaging;
using Yahv.Utils.Extends;
using WinApp.Printers;

namespace WinApp.Services
{

    /// <summary>
    /// 顺丰/跨越打印帮助者
    /// </summary>
    public class PrinterHelper
    {
        #region 逻辑流程
        //1.先请求顺丰/跨越下单接口
        //2.请求打印接口
        #endregion

        //EMS相关
        //测试环境地址
#if DEBUG|| TEST
        private static string ReqURL = "https://211.156.197.233/iwaybillno-web/a/iwaybill/receive";
        private static string partnered = "key123xydJDPT";
        private static string ecCompanyId = "DKH";


#else
        private static string ReqURL = "https://211.156.195.15/iwaybillno-web/a/iwaybill/receive";
        private static string partnered = "i6aaZH1Cnjfy";
        private static string ecCompanyId = "FORIC";

#endif



        private static string msg_type = "ORDERCREATE";



        static public string GetPrinterName(string shipperCode)
        {
            switch (shipperCode)
            {
                case Yahv.PsWms.PvRoute.Services.ShipperCode.SF:
                    return PrinterConfigs.Current[PrinterConfigs.顺丰打印].PrinterName;

                case Yahv.PsWms.PvRoute.Services.ShipperCode.KY:
                    return PrinterConfigs.Current[PrinterConfigs.跨越速运打印].PrinterName;
                case Yahv.PsWms.PvRoute.Services.ShipperCode.EMS:
                    return PrinterConfigs.Current[PrinterConfigs.EMS打印].PrinterName;
                default:
                    return null;
            }
        }
        static public void FacePrint(string orderID, string shipperCode, int expType, int exPayType, Yahv.PsWms.PvRoute.Services.Models.Sender sender, Yahv.PsWms.PvRoute.Services.Models.Receiver receiver, int quantity, string remark
            , double? volume, double? weight, string monthlyCard, bool isSignBack
            , params Yahv.PsWms.PvRoute.Services.Models.Commodity[] commodities)
        {
            string printerName = GetPrinterName(shipperCode);

            if (string.IsNullOrWhiteSpace(printerName))
            {
                MessageBox.Show($"请配置顺丰/跨越/EMS打印机!");
                return;
            }

            if (shipperCode == Yahv.PsWms.PvRoute.Services.ShipperCode.KY)
            {
                var printers = System.Configuration.ConfigurationManager.AppSettings["KysyPrinter"].Split(',');
                if (!printers.Any(item => item == printerName))
                {
                    MessageBox.Show($"跨越打印机编号有误!");
                    return;
                }
            }
            else if (shipperCode == Yahv.PsWms.PvRoute.Services.ShipperCode.SF)
            {
                if (!PrinterConfigs.Connected(printerName))
                {
                    MessageBox.Show($"请配置顺丰打印机!");
                    return;
                }
            }
            else if (shipperCode == Yahv.PsWms.PvRoute.Services.ShipperCode.EMS)
            {
                if (!PrinterConfigs.Connected(printerName))
                {
                    MessageBox.Show($"请配置EMS打印机!");
                    return;
                }
            }
            //顺丰打印
            if (shipperCode == Yahv.PsWms.PvRoute.Services.ShipperCode.SF)
            {

                {
                    #region 逻辑流程
                    /* 请求顺丰接口（获取订单、下订单接口）。
                     1.请求顺丰获取订单接口，查看是否已经下过单了，若下过单了，把订单取出来去进行打印；
                     2.如果没有下单，先请求下单接口，然后根据订单结果去进行打印
                     */
                    #endregion

                    #region 顺丰打印

                    //转换支付方式
                    var sfExpayType = ConvertSFExpayType(exPayType);

                    //var monthlyCard = "";//以后可以传值

                    if (sfExpayType != (int)SFPayType.ThirdPay)
                    {
                        monthlyCard = "";
                    }

                    //第三方月结时月结账号不能为空
                    if (sfExpayType == (int)SFPayType.ThirdPay)
                    {
                        if (string.IsNullOrWhiteSpace(monthlyCard))
                        {
                            MessageBox.Show("第三方月结时月结账号不能为空！！");
                            return;
                        }
                    }

                    //顺丰 月结支付时传值，现结不需传值
                    if (sfExpayType == (int)SFPayType.DeliveryPay)
                    {
                        //monthlyCard = "7550205279";
                        monthlyCard = "7550123478";//2021.10.21 顺丰月结账号更换

                    }


                    //每次产生新的OrderID去请求顺丰接口
                    Random rd = new Random();
                    var newOrderId = string.Concat(orderID, "_", rd.Next());

                    #region 组织对应参数
                    List<cargoDetail> cargo = new List<cargoDetail>();
                    commodities.ToList().ForEach(item =>
                    {
                        cargo.Add(new cargoDetail
                        {
                            goodsCode = item.GoodsCode,
                            name = item.GoodsName,
                            count = item.Goodsquantity,
                            amount = item.GoodsPrice,
                            weight = item.GoodsWeight,
                            volume = item.GoodsVol
                        });
                    });

                    //组织订单对象
                    SFOrder order = new SFOrder
                    {
                        language = "zh-CN",
                        monthlyCard = monthlyCard,
                        //orderId = newOrderId,
                        orderId = orderID, //改为正常订单
                        expressTypeId = expType,
                        payMethod = sfExpayType,//暂时固定为1（已取消）：寄付 exPayType,
                        //parcelQty = quantity,
                        parcelQty = 1,//顺风包裹数固定值为1
                        remark = remark,
                        //isDocall = 0,
                        isSignBack = isSignBack,//签回单前台传值
                        //isReturnRoutelabel = true,
                        isReturnSignBackRoutelabel = true,//返回签回单路由标签
                        volume = volume,
                        totalWeight = weight,
                        cargoDetails = cargo,
                        contactInfoList = new List<contactInfo>
                        {
                            new contactInfo
                            {
                                contactType=1,
                                address=sender.Address.Trim(),
                                contact=sender.Contact,
                                province=sender.Province,
                                city=sender.City,
                                county=sender.Region,
                                mobile=sender.Mobile,
                                tel=sender.Tel,
                                company=sender.Company.ToKdnFullAngle()
                            },
                            new contactInfo
                            {
                                contactType=2,
                                address=receiver.Address.Trim(),
                                contact=receiver.Contact,
                                province=receiver.Province,
                                city=receiver.City,
                                county=receiver.Region,
                                mobile=receiver.Mobile,
                                tel=receiver.Tel,
                                company=receiver.Company.ToKdnFullAngle()
                            }
                        }
                    };
                    #endregion

                    string partnerID = "YDCXKg";//此处替换为您在丰桥平台获取的顾客编码          
                    string checkword = "XKuM6bAGYZ2FvHmTb3FOyssM03LBquav";//此处替换为您在丰桥平台获取的校验码  
#if DEBUG || TEST
                    string reqURL = "https://sfapi-sbox.sf-express.com/std/service";//测试环境
#else
                    string reqURL = "https://sfapi.sf-express.com/std/service"; //生产环境
#endif

                    //请求顺丰订单结果查询
                    string serviceCode = "EXP_RECE_SEARCH_ORDER_RESP";

                    string timestamp = GetTimeStamp(); //获取时间戳  
                    string requestID = System.Guid.NewGuid().ToString(); //获取uuid
                    //云打印下单接口的orderid改为正常的
                    string msgJson = new
                    {
                        searchType = 1,
                        orderId = order.orderId,
                        language = order.language
                    }.Json();

                    string msgData = JsonCompress(msgJson);

                    string msgDigest = MD5ToBase64string(UrlEncode(msgData + timestamp + checkword));

                    string respJson = callSfExpressServiceByCSIM(reqURL, partnerID, requestID, serviceCode, timestamp, msgDigest, msgData);

                    if (respJson != null)
                    {
                        try
                        {
                            JObject rlt = respJson.JsonTo<JObject>();

                            var apiResultCode = rlt["apiResultCode"].Value<string>();
                            JToken apiResultData = rlt["apiResultData"];

                            apiResultData resultData = apiResultData.ToString().JsonTo<apiResultData>();

                            //json对象里面还有对象时反序列化不能成功
                            //SFResult result = respJson.JsonTo<SFResult>();

                            //查询结果成功直接打印
                            if (apiResultCode == "A1000" && resultData.success == true)
                            {
                                var msg = resultData.msgData;//下单结果数据

                                #region 1.图片打印方式
                                ////组装打印参数
                                //List<WaybillDto> waybillDtoList = AssemblyParameters(order, msg, true);

                                //SFWaybillPrinter printer = new SFWaybillPrinter();

                                //printer.Success += Printer_Responsed;
                                ////打印
                                //printer.WayBillPrinterTools(waybillDtoList, printerName, orderID);
                                #endregion

                                #region 2.云打印方式, 通过方法来调用SF的运单功能
                                //主运单号
                                var sfWaybillNo = msg.waybillNoInfoList.Where(item => item.waybillType == 1).FirstOrDefault().waybillNo;
                                //签回单
                                var backWaybillNo = msg.waybillNoInfoList.Where(item => item.waybillType == 3)?.FirstOrDefault()?.waybillNo;

                                SF_ClouldPrintHelper(sfWaybillNo, backWaybillNo, orderID, isSignBack);

                                #endregion

                                #region 2.云打印
                                /*
                                //主运单号路径，签回单号路径
                                string filePath = "", backFilePath = "";
                                //主运单号
                                var sfWaybillNo = msg.waybillNoInfoList.Where(item => item.waybillType == 1).FirstOrDefault().waybillNo;
                                //签回单
                                var backWaybillNo = msg.waybillNoInfoList.Where(item => item.waybillType == 3)?.FirstOrDefault()?.waybillNo;

                                //判断顺丰单子是否已经产生pdf文件
                                if (!IsExistWaybillNo(sfWaybillNo, ref filePath) || (!string.IsNullOrWhiteSpace(backWaybillNo) && !IsExistWaybillNo(backWaybillNo, ref backFilePath)))
                                {
                                    //采取最新云打印
                                    serviceCode = "COM_RECE_CLOUD_PRINT_WAYBILLS";

                                    #region 组装云打印参数
                                    //云打印参数组装
                                    var cloudPriterAssmblyParams = new
                                    {
                                        templateCode = "fm_210_standard_YDCXKg",
                                        fileType = "pdf",
                                        sync = "true",
                                        version = "2.0",
                                        documents = new dynamic[]
                                        {
                                                    new
                                                    {
                                                        //主运单
                                                         masterWaybillNo =sfWaybillNo,

                                                    }
                                        }
                                    };


                                    if (isSignBack == true && !string.IsNullOrWhiteSpace(backWaybillNo))
                                    {
                                        //涉及签回单重新生成参数
                                        cloudPriterAssmblyParams = new
                                        {
                                            templateCode = "fm_210_standard_YDCXKg",
                                            fileType = "pdf",
                                            sync = "true",
                                            version = "2.0",
                                            documents = new dynamic[]
                                            {
                                                        new
                                                        {
                                                            //主运单号
                                                             masterWaybillNo =sfWaybillNo,
                                                        },
                                                        new
                                                        {
                                                            //签回单
                                                             backWaybillNo =backWaybillNo
                                                        }
                                            }
                                        };
                                    }
                                    #endregion

                                    msgJson = cloudPriterAssmblyParams.Json();
                                    msgData = JsonCompress(msgJson);
                                    timestamp = GetTimeStamp(); //获取时间戳       
                                    requestID = System.Guid.NewGuid().ToString(); //获取uuid
                                    msgDigest = MD5ToBase64string(UrlEncode(msgData + timestamp + checkword));
                                    //云打印
                                    respJson = callSfExpressServiceByCSIM(reqURL, partnerID, requestID, serviceCode, timestamp, msgDigest, msgData);

                                    if (respJson != null)
                                    {
                                        rlt = respJson.JsonTo<JObject>();
                                        apiResultCode = rlt["apiResultCode"].Value<string>();
                                        apiResultData = rlt["apiResultData"];
                                        //调用云打印返回apiResultData的结果
                                        var cloudResultData = apiResultData.ToString().JsonTo<cloudPrintResultData>();
                                        //如果返回成功需要通过pdf地址+token进行下载pdf
                                        if (apiResultCode == "A1000" && cloudResultData.success == true)
                                        {
                                            var url = cloudResultData.obj.files[0].url;
                                            var token = cloudResultData.obj.files[0].token;
                                            Download(url, token, ref filePath);

                                            if (isSignBack == true && !string.IsNullOrWhiteSpace(backWaybillNo))
                                            {
                                                url = cloudResultData.obj.files[1].url;
                                                token = cloudResultData.obj.files[1].token;
                                                Download(url, token, ref backFilePath);
                                            }
                                        }
                                    }
                                }

                                //对pdf文件进行spire.pdf静默打印
                                List<cloudPrint> cloudPrints = new List<cloudPrint>();
                                if (!string.IsNullOrWhiteSpace(filePath))
                                {
                                    cloudPrints.Add(new cloudPrint
                                    {
                                        WabillNo = sfWaybillNo,
                                        FilePath = filePath,
                                        Type = 1
                                    });
                                }
                                if (!string.IsNullOrWhiteSpace(backFilePath))
                                {

                                    cloudPrints.Add(new cloudPrint
                                    {
                                        WabillNo = backWaybillNo,
                                        FilePath = backFilePath,
                                        Type = 3
                                    });
                                }

                                if (cloudPrints.Count() > 0)
                                {
                                    SFWaybillPrinter printer = new SFWaybillPrinter();

                                    printer.Success += Printer_Responsed;
                                    //打印
                                    printer.CloudWayBillPrinter(cloudPrints.Json(), orderID, printerName, newOrderId);
                                }

                                #region pdf转图片base64编码进行打印（打印太模糊，放弃）
                                ////filepath不为空或backFilePath不为空进行打印处理
                                //if (filePath != "" || backFilePath != "")
                                //{
                                //    List<cloudPrint> cloudPrints = new List<cloudPrint>();
                                //    if (filePath != "")
                                //    {
                                //        string imgFilePath = "";

                                //        Image img = null;
                                //        //判断顺丰是否存在img
                                //        if (!IsExistWaybillNo(sfWaybillNo, ref imgFilePath, "*.jpg"))
                                //        {
                                //            img = PdfToJpg(filePath, ref imgFilePath);
                                //        }
                                //        else
                                //        {
                                //            img = Image.FromFile(imgFilePath);
                                //        }
                                //        //BitmapImage bitmap = new BitmapImage(new Uri(imgFilePath, UriKind.Absolute));   //以绝对路径形式设置Uri

                                //        string base64FilePath = ImageToBase64(img);

                                //        cloudPrints.Add(new cloudPrint
                                //        {
                                //            WabillNo = sfWaybillNo,
                                //            FilePath = base64FilePath,
                                //            Type = 1
                                //        });
                                //    }
                                //    if (backFilePath != "" && isSignBack == true && !string.IsNullOrWhiteSpace(backWaybillNo))
                                //    {
                                //        string imgFilePath = "";

                                //        Image img = null;
                                //        //判断顺丰是否存在img
                                //        if (!IsExistWaybillNo(backWaybillNo, ref imgFilePath, "*.jpg"))
                                //        {
                                //            img = PdfToJpg(backFilePath, ref imgFilePath);
                                //        }
                                //        else
                                //        {
                                //            img = Image.FromFile(imgFilePath);
                                //        }
                                //        var base64FilePath = ImageToBase64(img);

                                //        cloudPrints.Add(new cloudPrint
                                //        {
                                //            WabillNo = backWaybillNo,
                                //            FilePath = base64FilePath,
                                //            Type = 3
                                //        });
                                //    }
                                //    if (cloudPrints.Count() > 0)
                                //    {
                                //        SFWaybillPrinter printer = new SFWaybillPrinter();

                                //        printer.Success += Printer_Responsed;
                                //        //打印
                                //        printer.CloudWayBillPrinter(cloudPrints, orderID, printerName, newOrderId);
                                //    }
                                //}
                                #endregion
                                */
                                #endregion
                            }

                            //未成功请求下单接口进行打印
                            else
                            {
                                serviceCode = "EXP_RECE_CREATE_ORDER";

                                msgJson = order.Json();
                                msgData = JsonCompress(msgJson);

                                timestamp = GetTimeStamp(); //获取时间戳       

                                requestID = System.Guid.NewGuid().ToString(); //获取uuid

                                msgDigest = MD5ToBase64string(UrlEncode(msgData + timestamp + checkword));

                                respJson = callSfExpressServiceByCSIM(reqURL, partnerID, requestID, serviceCode, timestamp, msgDigest, msgData);

                                if (respJson != null)
                                {

                                    //JObject obj = (JObject)JsonConvert.DeserializeObject(respJson);
                                    rlt = respJson.JsonTo<JObject>();

                                    apiResultCode = rlt["apiResultCode"].Value<string>();
                                    apiResultData = rlt["apiResultData"];

                                    resultData = apiResultData.ToString().JsonTo<apiResultData>();

                                    //json对象里面还有对象时反序列化不能成功
                                    //SFResult result = respJson.JsonTo<SFResult>();

                                    //如果返回成功去进行打印
                                    if (apiResultCode == "A1000" && resultData.success == true)
                                    {

                                        var msg = resultData.msgData;//下单结果数据

                                        #region 1.图片打印方式
                                        ////组装打印参数
                                        //List<WaybillDto> waybillDtoList = AssemblyParameters(order, msg, true);

                                        //SFWaybillPrinter printer = new SFWaybillPrinter();

                                        //printer.Success += Printer_Responsed;
                                        ////打印
                                        //printer.WayBillPrinterTools(waybillDtoList, printerName, orderID);
                                        #endregion

                                        #region 2.云打印方式，通过方法来调用SF的运单功能
                                        //主运单号
                                        var sfWaybillNo = msg.waybillNoInfoList.Where(item => item.waybillType == 1).FirstOrDefault().waybillNo;
                                        //签回单
                                        var backWaybillNo = msg.waybillNoInfoList.Where(item => item.waybillType == 3)?.FirstOrDefault()?.waybillNo;

                                        SF_ClouldPrintHelper(sfWaybillNo, backWaybillNo, orderID, isSignBack);
                                        #endregion

                                        #region 2.云打印
                                        /*
                                        //主运单号路径，签回单号路径
                                        string filePath = "", backFilePath = "";
                                        //主运单号
                                        var sfWaybillNo = msg.waybillNoInfoList.Where(item => item.waybillType == 1).FirstOrDefault().waybillNo;
                                        //签回单
                                        var backWaybillNo = msg.waybillNoInfoList.Where(item => item.waybillType == 3)?.FirstOrDefault()?.waybillNo;

                                        //判断顺丰单子是否已经产生pdf文件
                                        if (!IsExistWaybillNo(sfWaybillNo, ref filePath) || (!string.IsNullOrWhiteSpace(backWaybillNo) && !IsExistWaybillNo(backWaybillNo, ref backFilePath)))
                                        {
                                            //采取最新云打印
                                            serviceCode = "COM_RECE_CLOUD_PRINT_WAYBILLS";

                                            #region 组装云打印参数
                                            //云打印参数组装
                                            var cloudPriterAssmblyParams = new
                                            {
                                                templateCode = "fm_210_standard_YDCXKg",
                                                fileType = "pdf",
                                                sync = "true",
                                                version = "2.0",
                                                documents = new dynamic[]
                                                {
                                                    new
                                                    {
                                                        //主运单
                                                         masterWaybillNo =sfWaybillNo,

                                                    }
                                                }
                                            };


                                            if (isSignBack == true && !string.IsNullOrWhiteSpace(backWaybillNo))
                                            {
                                                //涉及签回单重新生成参数
                                                cloudPriterAssmblyParams = new
                                                {
                                                    templateCode = "fm_210_standard_YDCXKg",
                                                    fileType = "pdf",
                                                    sync = "true",
                                                    version = "2.0",
                                                    documents = new dynamic[]
                                                    {
                                                        new
                                                        {
                                                            //主运单号
                                                             masterWaybillNo =sfWaybillNo,
                                                        },
                                                        new
                                                        {
                                                            //签回单
                                                             backWaybillNo =backWaybillNo
                                                        }
                                                    }
                                                };
                                            }
                                            #endregion

                                            msgJson = cloudPriterAssmblyParams.Json();
                                            msgData = JsonCompress(msgJson);
                                            timestamp = GetTimeStamp(); //获取时间戳       
                                            requestID = System.Guid.NewGuid().ToString(); //获取uuid
                                            msgDigest = MD5ToBase64string(UrlEncode(msgData + timestamp + checkword));
                                            //云打印
                                            respJson = callSfExpressServiceByCSIM(reqURL, partnerID, requestID, serviceCode, timestamp, msgDigest, msgData);

                                            if (respJson != null)
                                            {
                                                rlt = respJson.JsonTo<JObject>();
                                                apiResultCode = rlt["apiResultCode"].Value<string>();
                                                apiResultData = rlt["apiResultData"];
                                                //调用云打印返回apiResultData的结果
                                                var cloudResultData = apiResultData.ToString().JsonTo<cloudPrintResultData>();
                                                //如果返回成功需要通过pdf地址+token进行下载pdf
                                                if (apiResultCode == "A1000" && cloudResultData.success == true)
                                                {
                                                    var url = cloudResultData.obj.files[0].url;
                                                    var token = cloudResultData.obj.files[0].token;
                                                    Download(url, token, ref filePath);

                                                    //如果签回单有值
                                                    if (isSignBack == true && !string.IsNullOrWhiteSpace(backWaybillNo))
                                                    {
                                                        url = cloudResultData.obj.files[1].url;
                                                        token = cloudResultData.obj.files[1].token;
                                                        Download(url, token, ref backFilePath);
                                                    }
                                                }
                                            }
                                        }

                                        //对pdf文件进行spire.pdf静默打印
                                        List<cloudPrint> cloudPrints = new List<cloudPrint>();
                                        if (!string.IsNullOrWhiteSpace(filePath))
                                        {
                                            cloudPrints.Add(new cloudPrint
                                            {
                                                WabillNo = sfWaybillNo,
                                                FilePath = filePath,
                                                Type = 1
                                            });
                                        }
                                        if (!string.IsNullOrWhiteSpace(backFilePath))
                                        {

                                            cloudPrints.Add(new cloudPrint
                                            {
                                                WabillNo = backWaybillNo,
                                                FilePath = backFilePath,
                                                Type = 3
                                            });
                                        }

                                        if (cloudPrints.Count() > 0)
                                        {
                                            SFWaybillPrinter printer = new SFWaybillPrinter();

                                            printer.Success += Printer_Responsed;
                                            //打印
                                            printer.CloudWayBillPrinter(cloudPrints.Json(), orderID, printerName, newOrderId);
                                        }

                                        #region pdf转图片base64编码进行打印（打印太模糊，放弃）
                                        ////filepath不为空或backFilePath不为空进行打印处理
                                        //if (filePath != "" || backFilePath != "")
                                        //{
                                        //    List<cloudPrint> cloudPrints = new List<cloudPrint>();
                                        //    if (filePath != "")
                                        //    {
                                        //        string imgFilePath = "";

                                        //        Image img = null;
                                        //        //判断顺丰是否存在img
                                        //        if (!IsExistWaybillNo(sfWaybillNo, ref imgFilePath, "*.jpg"))
                                        //        {
                                        //            img = PdfToJpg(filePath, ref imgFilePath);
                                        //        }
                                        //        else
                                        //        {
                                        //            img = Image.FromFile(imgFilePath);
                                        //        }
                                        //        //BitmapImage bitmap = new BitmapImage(new Uri(imgFilePath, UriKind.Absolute));   //以绝对路径形式设置Uri

                                        //        string base64FilePath = ImageToBase64(img);

                                        //        cloudPrints.Add(new cloudPrint
                                        //        {
                                        //            WabillNo = sfWaybillNo,
                                        //            FilePath = base64FilePath,
                                        //            Type = 1
                                        //        });
                                        //    }
                                        //    if (backFilePath != "" && isSignBack == true && !string.IsNullOrWhiteSpace(backWaybillNo))
                                        //    {
                                        //        string imgFilePath = "";

                                        //        Image img = null;
                                        //        //判断顺丰是否存在img
                                        //        if (!IsExistWaybillNo(backWaybillNo, ref imgFilePath, "*.jpg"))
                                        //        {
                                        //            img = PdfToJpg(backFilePath, ref imgFilePath);
                                        //        }
                                        //        else
                                        //        {
                                        //            img = Image.FromFile(imgFilePath);
                                        //        }
                                        //        var base64FilePath = ImageToBase64(img);

                                        //        cloudPrints.Add(new cloudPrint
                                        //        {
                                        //            WabillNo = backWaybillNo,
                                        //            FilePath = base64FilePath,
                                        //            Type = 3
                                        //        });
                                        //    }
                                        //    if (cloudPrints.Count() > 0)
                                        //    {
                                        //        SFWaybillPrinter printer = new SFWaybillPrinter();

                                        //        printer.Success += Printer_Responsed;
                                        //        //打印
                                        //        printer.CloudWayBillPrinter(cloudPrints, orderID, printerName, newOrderId);
                                        //    }
                                        //}
                                        #endregion
                                        */
                                        #endregion

                                    }
                                    else
                                    {
                                        MessageBox.Show(resultData.errorMsg);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            //throw new Exception("未知错误");
                        }
                    }

                    #endregion

                    //
                    //waybillDtoList.Json(); 
                }
            }
            //跨越打印
            else if (shipperCode == Yahv.PsWms.PvRoute.Services.ShipperCode.KY)
            {
                //KYWaybillPrinter.OrderManage(orderID, shipperCode, expType, exPayType, sender, receiver, quantity, remark, volume, weight, commodities);
                #region 跨越打印
                try
                {
                    #region 组装打印参数

                    var kyExpayType = ConvertKYExpayType(exPayType);

                    var kyExpType = ConvertKYExpType(expType);

                    var paymentCustomer = "";//后续改为可以传值的

                    //第三方月结时月结账号不能为空
                    if (kyExpayType == (int)KYPayType.ThirdPay)
                    {
                        if (string.IsNullOrWhiteSpace(monthlyCard))
                        {
                            MessageBox.Show("第三方月结时月结账号不能为空！！");
                            return;
                        }
                        paymentCustomer = monthlyCard;
                    }

                    //跨越的付款方式为10和30时必须传值
                    if (kyExpayType == (int)KYPayType.DeliveryPay)
                    {
#if DEBUG || TEST
                        paymentCustomer = "075525131031";//付款账号用第三方的或者我司的月结账号
                        //paymentCustomer = "075517225569";//付款账号用第三方的或者我司的月结账号
                        //paymentCustomer = "075568610585";//付款账号用第三方的或者我司的月结账号
#else
                        //paymentCustomer = "075517225569";
                        //paymentCustomer = "075568610585";
                        paymentCustomer = "01010658268";  //2022.6.8更改新的月结卡号
#endif
                    }

                    //每次产生新的OrderID去请求跨越接口
                    Random rd = new Random();
                    var newOrderId = string.Concat(orderID, "_", rd.Next());

                    //跨越人员：如果没有尺寸，preWaybillGoodsSizeList不用传参
                    //ArrayList size = new ArrayList();
                    ////List<WaybillGoodsSize> size = new List<WaybillGoodsSize>();
                    //commodities.ToList().ForEach(item =>
                    //{
                    //    size.Add(new
                    //    {
                    //        goodsCode = item.GoodsCode,
                    //        goodsName = item.GoodsName,
                    //        count = item.Goodsquantity == 0 ? null : item.Goodsquantity.ToString(),
                    //        //amount = item.GoodsPrice,
                    //        goodsWeight = item.GoodsWeight == 0 ? null : item.GoodsWeight.ToString(),
                    //        goodsVolume = item.GoodsVol == 0.0 ? null : item.GoodsVol.ToString()
                    //    });
                    //});

                    //var goodsTime = string.Concat(DateTime.Now.ToString("yyyy-MM-dd "), "18:30");//会在 一个手机APP上面 把跨越的人招呼过来，不用传 是否预约取货（dismantling） 和货好时间（goodsTime）

                    var request = new
                    {

#if DEBUG || TEST
                        customerCode = "075525131031",//客户编码（寄件公司账号），必须是我们公司的账号，不同于paymentCustomer
                        //customerCode = "075517225569",//客户编码（寄件公司账号），必须是我们公司的账号，不同于paymentCustomer
                        //customerCode = "075568610585",//客户编码（寄件公司账号），必须是我们公司的账号，不同于paymentCustomer，2021.4.1更改新的月结卡号
                        //platformFlag = "9C59E00421A331E2EFD807518A995305",//客户/平台标识（生产环境）
                        platformFlag = "6820933B508C2977269F78B2DCD329CB",//客户/平台标识(测试环境)
#else
                        //customerCode = "075517225569",//客户编码（寄件公司账号），必须是我们公司的账号，不同于paymentCustomer
                        //customerCode = "075568610585",//客户编码（寄件公司账号），必须是我们公司的账号，不同于paymentCustomer，2021.4.1更改新的月结卡号
                        customerCode = "01010658268",//客户编码（寄件公司账号），必须是我们公司的账号，不同于paymentCustomer，2022.6.8更改新的月结卡号
                        platformFlag = "9C59E00421A331E2EFD807518A995305",//客户/平台标识（生产环境）
#endif
                        orderInfos = new[]
                        {
                    new
                    {
                        waybillNumber="",
                    preWaybillDelivery=new //发件人
                    {
                        companyName=sender.Company.ToKdnFullAngle(),
                        person=sender.Contact,
                        phone=sender.Tel,
                        mobile=sender.Mobile,
                        provinceName=sender.Province,
                        cityName=sender.City,
                        countyName= sender.Region,
                        address= sender.Address
                    },
                    preWaybillPickup=new //收件人
                    {
                        companyName = receiver.Company.ToKdnFullAngle(),
                        person = receiver.Contact,
                        phone = receiver.Tel,
                        mobile = receiver.Mobile,
                        provinceName = receiver.Province,
                        cityName =receiver.City,
                        countyName = receiver.Region,
                        address = receiver.Address
                    },
                    receiptFlag=20,
                    serviceMode=kyExpType,
                    payMode=kyExpayType,// exPayType,
                    paymentCustomer= paymentCustomer,
                    goodsType=commodities.FirstOrDefault().GoodsName,
                    count=quantity,
                    actualWeight=weight,
                    orderId= newOrderId,
                    waybillRemark= remark,
                    //dismantling=10,//是否预约取货
                    //goodsTime=goodsTime//货好时间（预约上门取货时间段）去掉这两个值，会在 一个手机APP上面 把跨越的人招呼过来，不用传专门的货好时间

                    //preWaybillGoodsSizeList=size.ToArray()
                    }

                }
                    };
                    #endregion

                    string appkey = "80631";
                    string appsecret = "6C4E0274101AEF1BA4AD31EF866D572D";

                    #region 提交电子运单接口并调取打印方法

                    var paramInfo = request.Json();

                    DefaultKyeClient kyeClient = new DefaultKyeClient(appkey, appsecret);

#if DEBUG || TEST
                    //提交电子运单接口
                    string resJson = kyeClient.Execute(new KyeRequest
                    {
                        contentType = "application/json",
                        methodCode = "open.api.openCommon.planOrder",
                        paramInfo = paramInfo,
                        responseFormat = "json"
                    }, false);  //沙盒环境执行下单 生产不传false 默认为生产

#else
                    //提交电子运单接口
                    string resJson = kyeClient.Execute(new KyeRequest
                    {
                        contentType = "application/json",
                        methodCode = "open.api.openCommon.planOrder",
                        paramInfo = paramInfo,
                        responseFormat = "json"
                    });  //沙盒环境执行下单 生产不传false 默认为生产

#endif
                    KYWaybillPrinter printer = new KYWaybillPrinter();

                    printer.Success += Printer_Responsed;
                    //打印
                    printer.WayBillPrinterTools(resJson, printerName, orderID);

                    #endregion

                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                }

                #endregion

            }
            //EMS 打印
            else if (shipperCode == Yahv.PsWms.PvRoute.Services.ShipperCode.EMS)
            {
                var model = new EmsRequestModel();

                model.CreatedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                model.LogisticsProvider = "B";//速递写死？暂时只有速递，速递对应的基础产品代码是1：标准快递  3：代收/到付（标准快递）我们只有标准快递
#if DEBUG || TEST
                model.EcommerceNo = "DKH";// "FORIC";//渠道来源标识写死？
#else
                model.EcommerceNo = "FORIC";// "FORIC";//渠道来源标识写死？
#endif
                model.EcommerceUserId = Guid.NewGuid().ToString();//电商客户标识必填 写死？
                                                                  //model.SenderType = 1;//协议客户 写死？
                model.SenderNo = "1100099411297";//深圳市华芯通供应链管理有限公司(1100099411297)EMS月结账号 这地方写死
                //model.InnerChannel = string.Empty;
                model.LogisticsOrderNo = orderID;//订单编号 必填
                //model.BatchNo = string.Empty;
                //model.WaybillNo = string.Empty;
                //model.OneBillFlag = string.Empty;
                //model.SubmailNo = string.Empty;
                //model.OneBillFeeType = string.Empty;
                //model.ContentsAttribute = "1";//内件性质。发票默认 1 文件
                model.BaseProductNo = expType.ToString();//基础产品代码(相当于快递类型)。邮寄发票默认 1 标准快递 写死？
                //model.BizProductNo = string.Empty;
                model.Weight = weight.ToString();
                model.Volume = volume.ToString();
                //model.Length = string.Empty;
                //model.Width = string.Empty;
                //model.Height = string.Empty;
                //model.PostageTotal = string.Empty;
                model.PickupNotes = remark;
                //model.InsuranceFlag = string.Empty;
                //model.InsuranceAmount = string.Empty;
                //model.DeliverType = string.Empty;
                //model.DeliverPreDate = string.Empty;
                //model.PickupType = string.Empty;
                //model.PickupPreBeginTime = string.Empty;
                //model.PickupPreEndTime = string.Empty;
                //付款方式
                //1:寄件人 2:收件人 3:第三方 4:收件人集中付费 5:免费 6:寄/收件人 7:预付卡
                //邮寄发票默认 1
                model.PaymentMode = exPayType.ToString();//只保留1:寄件人 2:收件人
                //model.CodFlag = string.Empty;
                //model.CodAmount = string.Empty;
                //model.ReceiptFlag = string.Empty;
                //model.ReceiptWaybillNo = string.Empty;
                //model.ElectronicPreferentialNo = string.Empty;
                //model.ElectronicPreferentialAmount = string.Empty;
                //model.ValuableFlag = string.Empty;
                //model.SenderSafetyCode = string.Empty;
                //model.ReceiverSafetyCode = string.Empty;
                //model.Note = string.Empty;
                //model.ProjectId = string.Empty;
                model.Sender = new EmsSender
                {
                    Name = sender.Contact,
                    PostCode = "",
                    Phone = sender.Tel,
                    Mobile = sender.Mobile,
                    Prov = sender.Province,
                    City = sender.City,
                    County = sender.Region,
                    Address = sender.Address.Substring(6)
                };
                model.Receiver = new EmsSender
                {
                    Name = receiver.Contact,
                    PostCode = "",
                    Phone = receiver.Tel,
                    Mobile = receiver.Mobile,
                    Prov = receiver.Province,
                    City = receiver.City,
                    County = receiver.Region,
                    Address = receiver.Address
                };
                model.Cargos = new Cargos();
                model.Cargos.Cargo = new List<Cargo>();
                commodities.ToList().ForEach(item =>
                {
                    model.Cargos.Cargo.Add(new Cargo
                    {
                        CargoName = item.GoodsName,
                        CargoQuantity = item.Goodsquantity.ToString(),
                        CargoValue = item.GoodsPrice.ToString(),
                        CargoWeight = item.GoodsWeight.ToString(),
                    });
                });
                //转换Xml
                var xmldoc = new System.Xml.XmlDocument();
                xmldoc.LoadXml(model.Xml());
                //xmldoc.Save("D:/a.txt");

                var xmlString = ConvertXmlToString(xmldoc);
                xmlString = xmlString.Substring(xmlString.IndexOf("<OrderNormal>"));

                //调用请求
                var result = orderTracesSubByJson(xmlString);

                var response = XmlSerializerExtend.XmlTo<EmsResponseModel>(result);

                if (response.ResponseItems.Response.Success == true)
                {
                    model.WaybillNo = response.ResponseItems.Response.WaybillNo;

                    var setting = PrinterConfigs.Current["EMS打印"];

                    var settingUrl = Config.SchemeName + "://" + Config.DomainName + setting.Url;


                    EMSWaybillPrinter printer = new EMSWaybillPrinter();
                    printer.Success += Printer_Responsed;

                    printer.Print(new
                    {
                        EmsRequestModel = model,
                        EmsResponseModel = response.ResponseItems.Response
                    }, new PrinterConfig
                    {
                        Url = settingUrl,
                        Name = setting.Name,
                        Summary = setting.Summary,
                        PrinterName = setting.PrinterName
                    });
                    ////转到模板页
                    //PrintHelper.Current.Template.Print(new
                    //{
                    //    EmsRequestModel = model,
                    //    EmsResponseModel = response.ResponseItems.Response
                    //}, setting);
                }
                else
                {
                    MessageBox.Show("EMS接口有误请重试");
                }
            }
            else
            {
                MessageBox.Show($"打印接口目前支持：{string.Join(",", new Yahv.PsWms.PvRoute.Services.ShipperCode())}，请重试!");
                return;
            }

        }

        private static void Printer_Responsed(object sender, SuccessEventArgs e)
        {
            //调用Print接口
            var result = e.Result;
            string url = $"{Config.ApiUrlPrex}/Printer/Print";

            foreach (var rsl in result)
            {
                Yahv.Utils.Http.ApiHelper.Current.JPost(url,
               new FaceOrder
               {
                   Code = rsl.Code,
                   Source = (Yahv.PsWms.PvRoute.Services.PrintSource)(PrintSource)rsl.Source,
                   CreatorID = rsl.CreatorID,
                   Html = rsl.Html,
                   SendJson = rsl.SendJson,
                   ReceiveJson = rsl.ReceiveJson,
                   SessionID = rsl.SessionID,
                   MainID = rsl.MainID,
                   MyID = rsl.MyID
               }
               );
            }


            Gecko.GeckoWebBrowser firefox = SimHelper.Firefox;

            if (firefox == null)
            {
                return;
            }

            using (Gecko.AutoJSContext context = new Gecko.AutoJSContext(firefox.Window))
            {
                string _result;

                //主单号返回
                var waybillCode = result.Where(item => item.Type == 1).FirstOrDefault().Code;

                //var trackingCode = "";
                //if (result.Where(item => item.Type == 3).FirstOrDefault() != null)
                //{
                //    //回签单号
                //    trackingCode = result.Where(item => item.Type == 3).FirstOrDefault().Code;
                //    //context.EvaluateScript($"this['TrackingCode']('{trackingCode}');", firefox.Window.DomWindow, out _result);//暂时用过去的变量值
                //}



                //var message = new
                //{
                //    WaybillCode = waybillCode,
                //    TrackingCode = trackingCode
                //};

                context.EvaluateScript($"this['KdPrinted']('{waybillCode}');", firefox.Window.DomWindow, out _result);//暂时用过去的变量值
                //context.EvaluateScript($"this['KdPrinted']('{message.Json()}');", firefox.Window.DomWindow, out _result);//暂时用过去的变量值


                //context.EvaluateScript($"this['FaceOrderID']('{result.Code}');", firefox.Window.DomWindow, out _result);
            }

        }

        static public void SF_ClouldPrintHelper(string waybillNo, string backwaybillNo, string orderID, bool isSignBack = false)
        {
            string partnerID = "YDCXKg";//此处替换为您在丰桥平台获取的顾客编码
            string checkword = "XKuM6bAGYZ2FvHmTb3FOyssM03LBquav";//此处替换为您在丰桥平台获取的校验码
            string printerName = PrinterHelper.GetPrinterName(nameof(Yahv.PsWms.PvRoute.Services.PrintSource.SF));

            Random rd = new Random();
            var newOrderId = string.Concat(orderID, "_", rd.Next());

#if DEBUG || TEST
            string reqURL = "https://sfapi-sbox.sf-express.com/std/service";//测试环境
#else
            string reqURL = "https://sfapi.sf-express.com/std/service"; //生产环境
#endif

            //暂时不考虑签回单的情况
            //主运单号路径，签回单号路径
            string filePath = "", backFilePath = "";
            //主运单号
            var sfWaybillNo = waybillNo;
            //签回单
            var backWaybillNo = backwaybillNo;

            //判断顺丰单子是否已经产生pdf文件
            if (!IsExistWaybillNo(sfWaybillNo, ref filePath) || (!string.IsNullOrWhiteSpace(backWaybillNo) && !IsExistWaybillNo(backWaybillNo, ref backFilePath)))
            {
                //采取最新云打印
                var serviceCode = "COM_RECE_CLOUD_PRINT_WAYBILLS";

                #region 组装云打印参数
                //云打印参数组装
                var cloudPriterAssmblyParams = new
                {
                    templateCode = "fm_210_standard_YDCXKg",
                    fileType = "pdf",
                    sync = "true",
                    version = "2.0",
                    documents = new dynamic[]
                    {
                                                    new
                                                    {
                                                        //主运单
                                                         masterWaybillNo =sfWaybillNo,

                                                    }
                    }
                };


                if (isSignBack == true && !string.IsNullOrWhiteSpace(backWaybillNo))
                {
                    //涉及签回单重新生成参数
                    cloudPriterAssmblyParams = new
                    {
                        templateCode = "fm_210_standard_YDCXKg",
                        fileType = "pdf",
                        sync = "true",
                        version = "2.0",
                        documents = new dynamic[]
                        {
                                                        new
                                                        {
                                                            //主运单号
                                                             masterWaybillNo =sfWaybillNo,
                                                        },
                                                        new
                                                        {
                                                            //签回单
                                                             backWaybillNo =backWaybillNo
                                                        }
                        }
                    };
                }
                #endregion

                var msgJson = cloudPriterAssmblyParams.Json();
                var msgData = JsonCompress(msgJson);
                var timestamp = GetTimeStamp(); //获取时间戳       
                var requestID = System.Guid.NewGuid().ToString(); //获取uuid
                var msgDigest = MD5ToBase64string(UrlEncode(msgData + timestamp + checkword));
                //云打印
                var respJson = callSfExpressServiceByCSIM(reqURL, partnerID, requestID, serviceCode, timestamp, msgDigest, msgData);

                if (respJson != null)
                {
                    var rlt = respJson.JsonTo<JObject>();
                    var apiResultCode = rlt["apiResultCode"].Value<string>();
                    JToken apiResultData = rlt["apiResultData"];
                    //调用云打印返回apiResultData的结果
                    var cloudResultData = apiResultData.ToString().JsonTo<cloudPrintResultData>();
                    //如果返回成功需要通过pdf地址+token进行下载pdf
                    if (apiResultCode == "A1000" && cloudResultData.success == true)
                    {
                        var url = cloudResultData.obj.files[0].url;
                        var token = cloudResultData.obj.files[0].token;
                        Download(url, token, ref filePath);

                        if (isSignBack == true && !string.IsNullOrWhiteSpace(backWaybillNo))
                        {
                            url = cloudResultData.obj.files[1].url;
                            token = cloudResultData.obj.files[1].token;
                            Download(url, token, ref backFilePath);
                        }
                    }
                }
            }

            //对pdf文件进行spire.pdf静默打印
            List<cloudPrint> cloudPrints = new List<cloudPrint>();
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                cloudPrints.Add(new cloudPrint
                {
                    WabillNo = sfWaybillNo,
                    FilePath = filePath,
                    Type = 1
                });
            }
            if (!string.IsNullOrWhiteSpace(backFilePath))
            {

                cloudPrints.Add(new cloudPrint
                {
                    WabillNo = backWaybillNo,
                    FilePath = backFilePath,
                    Type = 3
                });
            }

            if (cloudPrints.Count() > 0)
            {
                SFWaybillPrinter printer = new SFWaybillPrinter();

                printer.Success += Printer_Responsed;                
                //打印
                printer.CloudWayBillPrinter(cloudPrints, orderID, printerName, newOrderId);
            }

            #region pdf转图片base64编码进行打印（打印太模糊，放弃）
            ////filepath不为空或backFilePath不为空进行打印处理
            //if (filePath != "" || backFilePath != "")
            //{
            //    List<cloudPrint> cloudPrints = new List<cloudPrint>();
            //    if (filePath != "")
            //    {
            //        string imgFilePath = "";

            //        Image img = null;
            //        //判断顺丰是否存在img
            //        if (!IsExistWaybillNo(sfWaybillNo, ref imgFilePath, "*.jpg"))
            //        {
            //            img = PdfToJpg(filePath, ref imgFilePath);
            //        }
            //        else
            //        {
            //            img = Image.FromFile(imgFilePath);
            //        }
            //        //BitmapImage bitmap = new BitmapImage(new Uri(imgFilePath, UriKind.Absolute));   //以绝对路径形式设置Uri

            //        string base64FilePath = ImageToBase64(img);

            //        cloudPrints.Add(new cloudPrint
            //        {
            //            WabillNo = sfWaybillNo,
            //            FilePath = base64FilePath,
            //            Type = 1
            //        });
            //    }
            //    if (backFilePath != "" && isSignBack == true && !string.IsNullOrWhiteSpace(backWaybillNo))
            //    {
            //        string imgFilePath = "";

            //        Image img = null;
            //        //判断顺丰是否存在img
            //        if (!IsExistWaybillNo(backWaybillNo, ref imgFilePath, "*.jpg"))
            //        {
            //            img = PdfToJpg(backFilePath, ref imgFilePath);
            //        }
            //        else
            //        {
            //            img = Image.FromFile(imgFilePath);
            //        }
            //        var base64FilePath = ImageToBase64(img);

            //        cloudPrints.Add(new cloudPrint
            //        {
            //            WabillNo = backWaybillNo,
            //            FilePath = base64FilePath,
            //            Type = 3
            //        });
            //    }
            //    if (cloudPrints.Count() > 0)
            //    {
            //        SFWaybillPrinter printer = new SFWaybillPrinter();

            //        printer.Success += Printer_Responsed;
            //        //打印
            //        printer.CloudWayBillPrinter(cloudPrints, orderID, printerName, newOrderId);
            //    }
            //}
            #endregion
        }

        #region 顺丰打印帮助方法
        private static string JsonCompress(string msgData)
        {
            StringBuilder sb = new StringBuilder();
            using (StringReader reader = new StringReader(msgData))
            {
                int ch = -1;
                int lastch = -1;
                bool isQuoteStart = false;
                while ((ch = reader.Read()) > -1)
                {
                    if ((char)lastch != '\\' && (char)ch == '\"')
                    {
                        if (!isQuoteStart)
                        {
                            isQuoteStart = true;
                        }
                        else
                        {
                            isQuoteStart = false;
                        }
                    }
                    if (!Char.IsWhiteSpace((char)ch) || isQuoteStart)
                    {
                        sb.Append((char)ch);
                    }
                    lastch = ch;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 请求顺丰接口
        /// </summary>
        /// <param name="reqURL">url地址</param>
        /// <param name="partnerID">顾客编码</param>
        /// <param name="requestID">uuid</param>
        /// <param name="serviceCode">确定哪个接口</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="msgDigest">数字签名</param>
        /// <param name="msgData">请求内容</param>
        /// <returns></returns>
        private static string callSfExpressServiceByCSIM(string reqURL, string partnerID, string requestID, string serviceCode, string timestamp, string msgDigest, string msgData)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(reqURL);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";

            Dictionary<string, string> content = new Dictionary<string, string>();
            content["partnerID"] = partnerID;
            content["requestID"] = requestID;
            content["serviceCode"] = serviceCode;
            content["timestamp"] = timestamp;
            content["msgData"] = msgData;
            content["msgDigest"] = msgDigest;

            if (!(content == null || content.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in content.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, content[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, content[key]);
                    }
                    i++;
                }

                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                req.ContentLength = data.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }

            }

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        #region  测试2
        /// <summary>
        /// 组装打印参数
        /// </summary>
        /// <param name="order">订单信息</param>
        /// <param name="msg">返回结果数据</param>
        /// <param name="isFengMi">是否采用封密打印方式</param>
        /// <returns></returns>
        private static List<WaybillDto> AssemblyParameters(SFOrder order, msgData msg, Boolean isFengMi)
        {
            List<WaybillDto> waybillDtoList = new List<WaybillDto>();

            WaybillDto dto = new WaybillDto();

            //电子面单顶部是否需要logo 
            dto.isPrintLogo = "1";//1 需要  0 不需要
                                  //这个必填 
                                  //dto.appId = "SLKJ2019"; //对应丰桥平台获取的clientCode
                                  //dto.appKey = "FBIqMkZjzxbsZgo7jTpeq7PD8CVzLT4Q"; //对应丰桥平台获取的checkWord

            dto.appId = "YDCXKg"; //对应丰桥平台获取的clientCode
            dto.appKey = "XKuM6bAGYZ2FvHmTb3FOyssM03LBquav"; //对应丰桥平台获取的checkWord

            var mailNo = msg.waybillNoInfoList.Where(item => item.waybillType == 1).FirstOrDefault().waybillNo;

            dto.mailNo = mailNo;//主运单号，顺丰默认一件（因顺丰内部规则修改，涉及子母单的费用变动，不够重量的子母单收费会翻倍；所以，修改深圳库房打印顺丰快递面单逻辑，不再使用子母单：请求面单时，件数固定1件）

            //签回单号  签单返回服务POD 会打印两份快单 其中第二份作为返寄的单==如有签回单业务需要传此字段值
            if (msg.waybillNoInfoList.Where(item => item.waybillType == 3).FirstOrDefault() != null)
            {
                dto.returnTrackingNo = msg.waybillNoInfoList.Where(item => item.waybillType == 3).FirstOrDefault().waybillNo;
            }


            //取出订单的到件方（收件人）信息
            var consigner = order.contactInfoList.Where(item => item.contactType == 2).FirstOrDefault();
            //取出订单的寄件方（寄件人）信息
            var deliver = order.contactInfoList.Where(item => item.contactType == 1).FirstOrDefault();

            //var msg = result.apiResultData.msgData;//下单结果数据

            //  dto.setMailNo("SF7551234567890,SF2000601520988,SF2000601520997");//子母单方式

            ////签回单号  签单返回服务POD 会打印两份快单 其中第二份作为返寄的单==如有签回单业务需要传此字段值
            //dto.returnTrackingNo = msg.waybillNoInfoList.Where(item => item.waybillType == 3).FirstOrDefault().waybillNo;

            //收件人信息  
            dto.consignerProvince = consigner.province;
            dto.consignerCity = consigner.city;
            dto.consignerCounty = consigner.county;
            dto.consignerAddress = consigner.address; //详细地址建议最多30个字  字段过长影响打印效果
            dto.consignerCompany = consigner.company;
            dto.consignerMobile = consigner.mobile;
            dto.consignerName = consigner.contact;
            dto.consignerShipperCode = consigner.postCode;
            dto.consignerTel = consigner.tel;


            //寄件人信息
            dto.deliverProvince = deliver.province;
            dto.deliverCity = deliver.city;
            dto.deliverCounty = deliver.county;
            dto.deliverAddress = deliver.address;
            dto.deliverCompany = deliver.company; //详细地址建议最多30个字  字段过长影响打印效果
            dto.deliverMobile = deliver.mobile;
            dto.deliverName = deliver.contact;
            dto.deliverShipperCode = deliver.postCode;
            dto.deliverTel = deliver.tel;

            dto.destCode = msg.destCode; //目的地代码 参考顺丰地区编号
            dto.zipCode = msg.originCode; //原寄地代码 参考顺丰地区编号

            dto.expressType = order.expressTypeId;
            //dto.insureValue = "501";//如果真实存在是在order.serviceList 中取到

            dto.monthAccount = order.monthlyCard; //月结卡号：寄方默认我们公司的月结，收方默认没有值，第三方支付时第三方公司的月结账号
            dto.orderNo = order.orderId;
            dto.payMethod = order.payMethod; // 1、寄付月结：显示寄付月结，不显示运费 2、寄付转第三方：显示寄付转第三方，不显示运费 3、寄付现结：按实际运费显示 4、到付：按实际需收取费用显示总计费用（含到付增值服务费，但不含COD费用)

            //dto.childRemark = "";//子单号备注
            dto.mainRemark = order.remark;
            //dto.returnTrackingRemark = "";//迁回单备注
            //dto.custLogo = "";
            //dto.logo = "";
            //dto.insureFee = "";
            //dto.payArea = "";
            //加密项
            dto.encryptCustName = true;//加密寄件人及收件人名称
            dto.encryptMobile = true;//加密寄件人及收件人联系手机	

            var routeinfo = msg.routeLabelInfo.Where(item => item.routeLabelData.waybillNo == mailNo).FirstOrDefault().routeLabelData;//取出路由标签

            List<RlsInfoDto> rlsInfoDtoList = new List<RlsInfoDto>();

            rlsInfoDtoList.Add(new RlsInfoDto
            {
                abFlag = routeinfo.abFlag,
                codingMapping = routeinfo.codingMapping,
                codingMappingOut = routeinfo.codingMappingOut,
                destRouteLabel = routeinfo.destRouteLabel,
                destTeamCode = routeinfo.destTeamCode,
                printIcon = routeinfo.printIcon,
                proCode = routeinfo.proCode,
                qrcode = routeinfo.twoDimensionCode,
                sourceTransferCode = routeinfo.sourceTransferCode,
                waybillNo = routeinfo.waybillNo,
                xbFlag = routeinfo.xbFlag

            });


            //签回单号  签单返回服务POD 会打印两份快单 其中第二份作为返寄的单==如有签回单业务需要传此字段值
            if (dto.returnTrackingNo != null)
            {
                //签回单号
                var backWaybillNo = msg.waybillNoInfoList.Where(item => item.waybillType == 3).FirstOrDefault().waybillNo;

                //签回单路由信息
                var backRoutelabelinfo = msg.routeLabelInfo.Where(item => item.routeLabelData.waybillNo == backWaybillNo).FirstOrDefault().routeLabelData;//取出路由标签

                rlsInfoDtoList.Add(new RlsInfoDto
                {
                    abFlag = backRoutelabelinfo.abFlag,
                    codingMapping = backRoutelabelinfo.codingMapping,
                    codingMappingOut = backRoutelabelinfo.codingMappingOut,
                    destRouteLabel = backRoutelabelinfo.destRouteLabel,
                    destTeamCode = backRoutelabelinfo.destTeamCode,
                    printIcon = backRoutelabelinfo.printIcon,
                    proCode = backRoutelabelinfo.proCode,
                    qrcode = backRoutelabelinfo.twoDimensionCode,
                    sourceTransferCode = backRoutelabelinfo.sourceTransferCode,
                    waybillNo = backRoutelabelinfo.waybillNo,
                    xbFlag = backRoutelabelinfo.xbFlag

                });

                //签回单号  签单返回服务POD 会打印两份快单 其中第二份作为返寄的单==如有签回单业务需要传此字段值

            }

            List<CargoInfoDto> cargoInfoList = new List<CargoInfoDto>();

            //托寄物信息
            order.cargoDetails.ToList().ForEach(item =>
            {
                cargoInfoList.Add(new CargoInfoDto
                {
                    //sku=item.goodsCode,//不理解sku是什么意思
                    cargo = item.name,
                    cargoCount = item.count,
                    //remark =item.// order.cargoDetails并无对应remark字段
                    cargoAmount = item.amount,
                    cargoWeight = item.weight,
                    cargoUnit = item.unit,
                });
            });


            //dto.rlsInfoDtoList = rlsInfoDtoList.ToArray();
            //dto.cargoInfoDtoList = cargoInfoList.ToArray();

            dto.rlsInfoDtoList = rlsInfoDtoList;
            dto.cargoInfoDtoList = cargoInfoList;

            if (isFengMi)
            {
                dto.rlsInfoDtoList = rlsInfoDtoList;

            }

            waybillDtoList.Add(dto);

            return waybillDtoList;
        }
        #endregion

        private static string UrlEncode(string str)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in str)
            {
                if (System.Web.HttpUtility.UrlEncode(c.ToString()).Length > 1)
                {
                    builder.Append(System.Web.HttpUtility.UrlEncode(c.ToString()).ToUpper());
                }
                else
                {
                    builder.Append(c);
                }
            }
            return builder.ToString();
        }

        private static string MD5ToBase64string(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] MD5 = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(str));//MD5(注意UTF8编码)
            string result = Convert.ToBase64String(MD5);//Base64
            return result;
        }

        private static string Read(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.UTF8);

            StringBuilder builder = new StringBuilder();
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                builder.Append(line);
            }
            return builder.ToString();
        }

        private static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// 判断顺丰订单号是否已经生成pdf文件
        /// </summary>
        /// <param name="waybillNo"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static bool IsExistWaybillNo(string waybillNo, ref string filePath, string extension = "*.pdf")
        {
            //顺丰文件存储目录
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SFFiles", DateTime.Now.ToString("yyyyMMdd"));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //获取已经存在的文件名
            string[] filenames = Directory.GetFiles(path, extension, SearchOption.AllDirectories);
            foreach (var existname in filenames)
            {
                if (existname.Contains(waybillNo))
                {
                    filePath = existname;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 下载文件函数
        /// </summary>
        /// <param name="url">下载链接</param>
        /// <param name="filePath">返回文件路径</param>
        /// <returns>bool</returns>
        private static bool Download(string url, string token, ref string filePath)
        {
            //顺丰文件pdf存储目录
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SFFiles", DateTime.Now.ToString("yyyyMMdd"));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Uri uri = new Uri(url);
            var filename = HttpUtility.UrlDecode(uri.Segments.Last());
            FileStream outStream;//创建文件
            filePath = Path.Combine(path, filename);
            outStream = File.Create(filePath);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Headers.Add("X-Auth-token", token);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response != null)
            {
                Stream stream = response.GetResponseStream();//获取响应的流
                                                             //判断不是文本形式就下载写入
                if (!response.ContentType.ToLower().StartsWith("text/"))
                {

                    Stream inStream = response.GetResponseStream();
                    byte[] buffer = new byte[1024];
                    int i;
                    do
                    {
                        i = inStream.Read(buffer, 0, buffer.Length);
                        if (i > 0)
                            outStream.Write(buffer, 0, i);
                    } while (i > 0);

                    outStream.Close();
                    inStream.Close();
                }
                return true;

            }
            else
                return false;
        }

        private readonly static PdfDocument doc = new PdfDocument();
        public static Bitmap PdfToJpg(string fileInfo, ref string filePath)
        {
            //顺丰文件image存储目录
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SFFiles", DateTime.Now.ToString("yyyyMMdd"));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            doc.LoadFromFile(fileInfo);
            Image bmp = doc.SaveAsImage(0);//默认第一页
            Bitmap bitmap = new Bitmap(bmp);
            ////经过测试，向上取值越来越模糊，向下取值也是
            bitmap = KiResizeImage(bitmap, 378, 794);
            //将Bitmap图形保存为jpg格式的图片
            filePath = Path.Combine(path, Path.GetFileNameWithoutExtension(fileInfo) + ".jpg");
            bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
            return bitmap;
        }

        ///<summary>
        /// 图片转Base64
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static string ImageToBase64(Image img)
        {
            try
            {
                Bitmap bmp = new Bitmap(img);
                MemoryStream ms = new MemoryStream();
                //bmp = KiResizeImage(bmp, 378, 794);
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 更改图片分辨率
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="newW"></param>
        /// <param name="newH"></param>
        /// <returns></returns>
        public static Bitmap KiResizeImage(Bitmap bmp, int newW, int newH)
        {
            try
            {
                Bitmap b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region 顺丰、跨越支付方式转变

        private static int ConvertSFExpayType(int exPayType)
        {
            switch (exPayType)
            {
                case 1:
                case 3:
                    return (int)SFPayType.DeliveryPay;
                case 2:
                    return (int)SFPayType.CollectPay;
                case 4:   //提供第三方月结的功能
                    return (int)SFPayType.ThirdPay;
                default:
                    throw new NotSupportedException("不支持此类型的快递方式");
            }
        }

        private static int ConvertKYExpayType(int exPayType)
        {
            switch (exPayType)
            {
                case 1:
                case 3:
                    return (int)KYPayType.DeliveryPay;
                case 2:
                    return (int)KYPayType.CollectPay;
                case 4: //第三方月结处理（提供第三方月结的功能）
                    return (int)KYPayType.ThirdPay;
                default:
                    throw new NotSupportedException("不支持此类型的快递方式");
            }
        }

        /// <summary>
        /// 跨越服务方式转变（快递类型的转变）
        /// </summary>
        /// <param name="expType"></param>
        /// <returns></returns>
        private static int ConvertKYExpType(int expType)
        {
            switch (expType)
            {
                case 1:
                    return (int)KYServiceMode.当天达;
                case 2:
                    return (int)KYServiceMode.次日达;
                case 3:
                    return (int)KYServiceMode.隔日达;
                case 5:
                    return (int)KYServiceMode.同城即日;
                case 6:
                    return (int)KYServiceMode.同城次日;
                case 7:
                    return (int)KYServiceMode.陆运件;
                case 8:
                    return (int)KYServiceMode.省内次日;
                case 9:
                    return (int)KYServiceMode.省内即日;
                case 10:
                    return (int)KYServiceMode.空运;
                case 11:
                    return (int)KYServiceMode.专运;
                case 12:
                    return (int)KYServiceMode.次晨达;
                //case 13:
                //    return (int)KYServiceMode.航空件;
                //case 14:
                //    return (int)KYServiceMode.早班件;
                //case 15:
                //    return (int)KYServiceMode.空运;
                //case 3:   //月结暂时做寄付处理（尚未提供第三方月结的功能）
                //    return (int)KYPayType.ThirdPay;
                default:
                    throw new NotSupportedException("不支持此类型的快递方式");
            }
        }

        #endregion

        #region  EMS私有方法
        /// <summary>
        /// Json方式  电子面单
        /// </summary>
        /// <returns></returns>
        public static string orderTracesSubByJson(string requestData)
        {

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("logistics_interface", HttpUtility.UrlEncode(requestData, Encoding.UTF8));
            param.Add("msg_type", msg_type);
            param.Add("ecCompanyId", ecCompanyId);
            var dataSign = Md5Base64.encode(requestData + partnered);
            param.Add("data_digest", HttpUtility.UrlEncode(dataSign, Encoding.UTF8));
            string result = sendPost(ReqURL, param);

            return result;
        }

        /// <summary>
        /// Post方式提交数据，返回网页的源代码
        /// </summary>
        /// <param name="url">发送请求的 URL</param>
        /// <param name="param">请求的参数集合</param>
        /// <returns>远程资源的响应结果</returns>
        private static string sendPost(string url, Dictionary<string, string> param)
        {
            string result = "";
            StringBuilder postData = new StringBuilder();
            if (param != null && param.Count > 0)
            {
                foreach (var p in param)
                {
                    if (postData.Length > 0)
                    {
                        postData.Append("&");
                    }
                    postData.Append(p.Key);
                    postData.Append("=");
                    postData.Append(p.Value);
                }
            }
            byte[] byteData = Encoding.GetEncoding("UTF-8").GetBytes(postData.ToString());
            try
            {

                ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Referer = url;
                request.Accept = "*/*";
                request.Timeout = 30 * 1000;
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729)";
                request.Method = "POST";
                request.ContentLength = byteData.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(byteData, 0, byteData.Length);
                stream.Flush();
                stream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream backStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(backStream, Encoding.GetEncoding("UTF-8"));
                result = sr.ReadToEnd();
                sr.Close();
                backStream.Close();
                response.Close();
                request.Abort();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }


        public static string ConvertXmlToString(XmlDocument xmlDoc)
        {
            MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, null);
            writer.Formatting = System.Xml.Formatting.Indented;
            xmlDoc.Save(writer);
            StreamReader sr = new StreamReader(stream, System.Text.Encoding.UTF8);
            stream.Position = 0;
            string xmlString = sr.ReadToEnd();
            sr.Close();
            stream.Close();
            return xmlString;
        }
        #endregion

    }
}
