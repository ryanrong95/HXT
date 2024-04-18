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
using System.Windows.Forms;
using Yahv.PsWms.DappForm.Services.Print;
using Yahv.PsWms.PvRoute.Services;
using Yahv.PsWms.PvRoute.Services.Models;
using Yahv.Utils.Serializers;

namespace Yahv.PsWms.DappForm.Services
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

        static public string GetPrinterName(string shipperCode)
        {
            switch (shipperCode)
            {
                case ShipperCode.SF:
                    return PrinterConfigs.Current[PrinterConfigs.顺丰打印].PrinterName;

                case ShipperCode.KY:
                    return PrinterConfigs.Current[PrinterConfigs.跨越速运打印].PrinterName;
                default:
                    return null;
            }
        }
        static public void FacePrint(string orderID, string shipperCode, int expType, int exPayType, Sender sender, Receiver receiver, int quantity, string remark
            , double? volume, double? weight, string monthlyCard, bool isSignBack
            , params Commodity[] commodities)
        {
            string printerName = GetPrinterName(shipperCode);

            if (string.IsNullOrWhiteSpace(printerName))
            {
                MessageBox.Show($"请配置顺丰/跨越打印机!");
                return;
            }

            if (shipperCode == ShipperCode.KY)
            {
                var printers = System.Configuration.ConfigurationManager.AppSettings["KysyPrinter"].Split(',');
                if (!printers.Any(item => item == printerName))
                {
                    MessageBox.Show($"跨越打印机编号有误!");
                    return;
                }
            }
            else if (shipperCode == ShipperCode.SF)
            {
                if (!PrinterConfigs.Connected(printerName))
                {
                    MessageBox.Show($"请配置顺丰打印机!");
                    return;
                }
            }

            //顺丰打印
            if (shipperCode == ShipperCode.SF)
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
                        monthlyCard = "7550205279";
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
                        orderId = newOrderId,
                        expressTypeId = expType,
                        payMethod = sfExpayType,//暂时固定为1（已取消）：寄付 exPayType,
                        //parcelQty = quantity,
                        parcelQty = 1,//顺风包裹数固定值为1
                        remark = remark,
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
                                company=sender.Company
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
                                company=receiver.Company
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

                                //组装打印参数
                                List<WaybillDto> waybillDtoList = AssemblyParameters(order, msg, true);

                                SFWaybillPrinter printer = new SFWaybillPrinter();

                                printer.Success += Printer_Responsed;
                                //打印
                                printer.WayBillPrinterTools(waybillDtoList, printerName, orderID);

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

                                #region 小实验
                                //try
                                //{
                                //    string path = "../../../WinApp.Services/callExpressRequest/01.order.json";//下订单

                                //    string msgJson = order.Json();
                                //    string msgData = JsonCompress(msgJson);

                                //    string timestamp = GetTimeStamp(); //获取时间戳       

                                //    string requestID = System.Guid.NewGuid().ToString(); //获取uuid

                                //    string msgDigest = MD5ToBase64string(UrlEncode(msgData + timestamp + checkword));

                                //    respJson = callSfExpressServiceByCSIM(reqURL, partnerID, requestID, serviceCode, timestamp, msgDigest, msgData);

                                //    //respJson = Read(@"d:\projects_vs2015\yahv\yahv.pfwms\winapp.services\callexpressrequest\01.order.json");
                                //    respJson = Read(path);
                                //}
                                //catch (Exception ex)
                                //{
                                //    throw;
                                //}
                                #endregion

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

                                        #region 都放到组装参数办法里
                                        //IList<WaybillDto> waybillDtoList = new List<WaybillDto>();

                                        ////取出订单的到件方（收件人）信息
                                        //var consigner = order.contactInfoList.Where(item => item.contactType == 2).FirstOrDefault();
                                        ////取出订单的寄件方（寄件人）信息
                                        //var deliver = order.contactInfoList.Where(item => item.contactType == 1).FirstOrDefault();

                                        //var routeinfo = msg.routeLabelInfo.FirstOrDefault().routeLabelData;//取出路由标签

                                        //var rlsInfoDto = new RlsInfoDto[]
                                        //{
                                        //      new RlsInfoDto
                                        //      {
                                        //         abFlag = routeinfo.abFlag,
                                        //         codingMapping = routeinfo.codingMapping,
                                        //         codingMappingOut = routeinfo.codingMappingOut,
                                        //         destRouteLabel = routeinfo.destRouteLabel,
                                        //         destTeamCode = routeinfo.destTeamCode,
                                        //         printIcon = routeinfo.printIcon,
                                        //         proCode = routeinfo.proCode,
                                        //         qrcode = routeinfo.twoDimensionCode,
                                        //         sourceTransferCode = routeinfo.sourceTransferCode,
                                        //         waybillNo = routeinfo.waybillNo,
                                        //         xbFlag = routeinfo.xbFlag
                                        //     }
                                        //};

                                        //List<CargoInfoDto> cargoInfoList = new List<CargoInfoDto>();

                                        ////托寄物信息
                                        //order.cargoDetails.ToList().ForEach(item =>
                                        //{
                                        //    cargoInfoList.Add(new CargoInfoDto
                                        //    {
                                        //        //sku=item.goodsCode,//不理解sku是什么意思
                                        //        cargo = item.name,
                                        //        cargoCount = item.count,
                                        //        //remark =item.// order.cargoDetails并无对应remark字段
                                        //        cargoAmount = item.amount,
                                        //        cargoWeight = item.weight,
                                        //        cargoUnit = item.unit,
                                        //    });
                                        //});
                                        #endregion

                                        #region 组装打印参数第一种写法


                                        // //取出订单的到件方（收件人）信息
                                        // var consigner = order.contactInfoList.Where(item => item.contactType == 2).FirstOrDefault();
                                        // //取出订单的到件方（收件人）信息
                                        // var deliver = order.contactInfoList.Where(item => item.contactType == 1).FirstOrDefault();

                                        // var routeinfo = msg.routeLabelInfo.FirstOrDefault().routeLabelData;//取出路由标签

                                        // var rlsInfoDto = new RlsInfoDto[]
                                        // {
                                        //       new RlsInfoDto
                                        //       {
                                        //          abFlag = routeinfo.abFlag,
                                        //          codingMapping = routeinfo.codingMapping,
                                        //          codingMappingOut = routeinfo.codingMappingOut,
                                        //          destRouteLabel = routeinfo.destRouteLabel,
                                        //          destTeamCode = routeinfo.destTeamCode,
                                        //          printIcon = routeinfo.printIcon,
                                        //          proCode = routeinfo.proCode,
                                        //          qrcode = routeinfo.twoDimensionCode,
                                        //          sourceTransferCode = routeinfo.sourceTransferCode,
                                        //          waybillNo = routeinfo.waybillNo,
                                        //          xbFlag = routeinfo.xbFlag
                                        //      }
                                        //};

                                        // List<CargoInfoDto> cargoInfoList = new List<CargoInfoDto>();

                                        // //请求顺丰下订单接口
                                        // order.cargoDetails.ToList().ForEach(item =>
                                        // {
                                        //     cargoInfoList.Add(new CargoInfoDto
                                        //     {
                                        //              //sku=item.goodsCode,//不理解sku是什么意思
                                        //              cargo = item.name,
                                        //         cargoCount = item.count,
                                        //              //remark =item.// order.cargoDetails并无对应remark字段
                                        //              cargoAmount = item.amount,
                                        //         cargoWeight = item.weight,
                                        //         cargoUnit = item.unit,
                                        //     });
                                        // });


                                        // IList<WaybillDto> waybillDtoList = new List<WaybillDto>() {
                                        //      new WaybillDto()
                                        //      {
                                        //          isPrintLogo="1",
                                        //          appId = "YDCXKg",//对应丰桥平台获取的clientCode
                                        //          appKey = "XKuM6bAGYZ2FvHmTb3FOyssM03LBquav", //对应丰桥平台获取的checkWord
                                        //          mailNo = msg.waybillNoInfoList.Where(item => item.waybillType == 1).FirstOrDefault().waybillNo,//主运单号，顺丰默认一件（因顺丰内部规则修改，涉及子母单的费用变动，不够重量的子母单收费会翻倍；所以，修改深圳库房打印顺丰快递面单逻辑，不再使用子母单：请求面单时，件数固定1件）
                                        //          consignerProvince = consigner.province,
                                        //          consignerCity = consigner.city,
                                        //          consignerCounty = consigner.county,
                                        //          consignerAddress = consigner.address, //详细地址建议最多30个字  字段过长影响打印效果
                                        //          consignerCompany = consigner.company,
                                        //          consignerMobile = consigner.mobile,
                                        //          consignerName = consigner.contact,
                                        //          consignerShipperCode = consigner.postCode,
                                        //          consignerTel = consigner.tel,
                                        //          deliverProvince = deliver.province,
                                        //          deliverCity = deliver.city,
                                        //          deliverCounty = deliver.county,
                                        //          deliverAddress = deliver.address,
                                        //          deliverCompany = deliver.company, //详细地址建议最多30个字  字段过长影响打印效果
                                        //          deliverMobile = deliver.mobile,
                                        //          deliverName = deliver.contact,
                                        //          deliverShipperCode = deliver.postCode,
                                        //          deliverTel = deliver.tel,
                                        //          destCode = msg.destCode,//目的地代码 参考顺丰地区编号
                                        //          zipCode = msg.originCode, //原寄地代码 参考顺丰地区编号
                                        //          expressType = order.expressTypeId,
                                        //          monthAccount = "7550205279",//月结卡号
                                        //          orderNo = order.orderId,
                                        //          payMethod = order.payMethod, // 1、寄付月结：显示寄付月结，不显示运费 2、寄付转第三方：显示寄付转第三方，不显示运费 3、寄付现结：按实际运费显示 4、到付：按实际需收取费用显示总计费用（含到付增值服务费，但不含COD费用)
                                        //          mainRemark = order.remark,
                                        //          encryptCustName = true,//加密寄件人及收件人名称
                                        //          encryptMobile = true,//加密寄件人及收件人联系手机	
                                        //          rlsInfoDtoList = rlsInfoDto,
                                        //          cargoInfoDtoList = cargoInfoList.ToArray(),
                                        //  }

                                        // };

                                        #endregion

                                        #region 第二种写法
                                        //WaybillDto dto = new WaybillDto();


                                        ////电子面单顶部是否需要logo 
                                        //dto.isPrintLogo = "1";//1 需要  0 不需要
                                        //                      //这个必填 
                                        //                      //dto.appId = "SLKJ2019"; //对应丰桥平台获取的clientCode
                                        //                      //dto.appKey = "FBIqMkZjzxbsZgo7jTpeq7PD8CVzLT4Q"; //对应丰桥平台获取的checkWord

                                        //dto.appId = "YDCXKg"; //对应丰桥平台获取的clientCode
                                        //dto.appKey = "XKuM6bAGYZ2FvHmTb3FOyssM03LBquav"; //对应丰桥平台获取的checkWord

                                        ////var msg = result.apiResultData.msgData;//下单结果数据

                                        //dto.mailNo = msg.waybillNoInfoList.Where(item => item.waybillType == 1).FirstOrDefault().waybillNo;//主运单号，顺丰默认一件（因顺丰内部规则修改，涉及子母单的费用变动，不够重量的子母单收费会翻倍；所以，修改深圳库房打印顺丰快递面单逻辑，不再使用子母单：请求面单时，件数固定1件）

                                        ////收件人信息  
                                        //dto.consignerProvince = consigner.province;
                                        //dto.consignerCity = consigner.city;
                                        //dto.consignerCounty = consigner.county;
                                        //dto.consignerAddress = consigner.address; //详细地址建议最多30个字  字段过长影响打印效果
                                        //dto.consignerCompany = consigner.company;
                                        //dto.consignerMobile = consigner.mobile;
                                        //dto.consignerName = consigner.contact;
                                        //dto.consignerShipperCode = consigner.postCode;
                                        //dto.consignerTel = consigner.tel;


                                        ////寄件人信息
                                        //dto.deliverProvince = deliver.province;
                                        //dto.deliverCity = deliver.city;
                                        //dto.deliverCounty = deliver.county;
                                        //dto.deliverAddress = deliver.address;
                                        //dto.deliverCompany = deliver.company; //详细地址建议最多30个字  字段过长影响打印效果
                                        //dto.deliverMobile = deliver.mobile;
                                        //dto.deliverName = deliver.contact;
                                        //dto.deliverShipperCode = deliver.postCode;
                                        //dto.deliverTel = deliver.tel;

                                        //dto.destCode = msg.destCode; //目的地代码 参考顺丰地区编号
                                        //dto.zipCode = msg.originCode; //原寄地代码 参考顺丰地区编号

                                        //dto.expressType = order.expressTypeId;
                                        ////dto.insureValue = "501";//如果真实存在是在order.serviceList 中取到

                                        //dto.monthAccount = "7550205279"; //月结卡号
                                        //dto.orderNo = order.orderId;
                                        //dto.payMethod = order.payMethod; // 1、寄付月结：显示寄付月结，不显示运费 2、寄付转第三方：显示寄付转第三方，不显示运费 3、寄付现结：按实际运费显示 4、到付：按实际需收取费用显示总计费用（含到付增值服务费，但不含COD费用)

                                        ////dto.childRemark = "";//子单号备注
                                        //dto.mainRemark = order.remark;
                                        ////dto.returnTrackingRemark = "";//迁回单备注
                                        ////dto.custLogo = "";
                                        ////dto.logo = "";
                                        ////dto.insureFee = "";
                                        ////dto.payArea = "";
                                        ////加密项
                                        //dto.encryptCustName = true;//加密寄件人及收件人名称
                                        //dto.encryptMobile = true;//加密寄件人及收件人联系手机	


                                        //dto.rlsInfoDtoList = rlsInfoDto;
                                        //dto.cargoInfoDtoList = cargoInfoList.ToArray();

                                        //var isFengMi = true;
                                        //if (isFengMi)
                                        //{
                                        //    dto.rlsInfoDtoList = rlsInfoDto;

                                        //}


                                        //waybillDtoList.Add(dto);

                                        #endregion

                                        List<WaybillDto> waybillDtoList = AssemblyParameters(order, msg, true);

                                        SFWaybillPrinter printer = new SFWaybillPrinter();

                                        printer.Success += Printer_Responsed;
                                        //打印
                                        printer.WayBillPrinterTools(waybillDtoList, printerName, orderID);

                                        //WayBillPrinterTools(waybillDtoList.Json());
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
            else if (shipperCode == ShipperCode.KY)
            {
                //KYWaybillPrinter.OrderManage(orderID, shipperCode, expType, exPayType, sender, receiver, quantity, remark, volume, weight, commodities);
                #region 跨越打印
                try
                {
                    #region 组装打印参数

                    var kyExpayType = ConvertKYExpayType(exPayType);

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
                        paymentCustomer = "075568610585";
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
                         customerCode = "075568610585",//客户编码（寄件公司账号），必须是我们公司的账号，不同于paymentCustomer，2021.4.1更改新的月结卡号
                        platformFlag = "9C59E00421A331E2EFD807518A995305",//客户/平台标识（生产环境）
#endif
                        orderInfos = new[]
                        {
                    new
                    {
                        waybillNumber="",
                    preWaybillDelivery=new //发件人
                    {
                        companyName=sender.Company,
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
                        companyName = receiver.Company,
                        person = receiver.Contact,
                        phone = receiver.Tel,
                        mobile = receiver.Mobile,
                        provinceName = receiver.Province,
                        cityName =receiver.City,
                        countyName = receiver.Region,
                        address = receiver.Address
                    },
                    receiptFlag=20,
                    serviceMode=expType,
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
            else
            {
                MessageBox.Show($"打印接口目前支持：{string.Join(",", new ShipperCode())}，请重试!");
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
                   Source = (PrintSource)rsl.Source,
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

                var trackingCode = "";
                if (result.Where(item => item.Type == 3).FirstOrDefault() != null)
                {
                    //回签单号
                    trackingCode = result.Where(item => item.Type == 3).FirstOrDefault().Code;
                    //context.EvaluateScript($"this['TrackingCode']('{trackingCode}');", firefox.Window.DomWindow, out _result);//暂时用过去的变量值
                }



                var message = new
                {
                    WaybillCode = waybillCode,
                    TrackingCode = trackingCode
                };

                context.EvaluateScript($"this['KdPrinted']('{message.Json()}');", firefox.Window.DomWindow, out _result);//暂时用过去的变量值

              
                //context.EvaluateScript($"this['FaceOrderID']('{result.Code}');", firefox.Window.DomWindow, out _result);
            }

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

        #region 测试1
        /// <summary>
        /// 组装打印参数
        /// </summary>
        /// <param name="order">订单信息</param>
        /// <param name="msg">返回结果数据</param>
        /// <param name="isFengMi">是否采用封密打印方式</param>
        /// <returns></returns>
        //private static List<WaybillDto> AssemblyParameters(SFOrder order, msgData msg, Boolean isFengMi)
        //{
        //    List<WaybillDto> waybillDtoList = new List<WaybillDto>();
        //    WaybillDto dto = new WaybillDto();
        //    List<CargoInfoDto> cargoInfoList = new List<CargoInfoDto>();

        //    //托寄物信息
        //    order.cargoDetails.ToList().ForEach(item =>
        //    {
        //        cargoInfoList.Add(new CargoInfoDto
        //        {
        //            //sku=item.goodsCode,//不理解sku是什么意思
        //            cargo = item.name,
        //            cargoCount = item.count,
        //            //remark =item.// order.cargoDetails并无对应remark字段
        //            cargoAmount = item.amount,
        //            cargoWeight = item.weight,
        //            cargoUnit = item.unit,
        //        });
        //    });

        //    //签回单号  签单返回服务POD 会打印两份快单 其中第二份作为返寄的单==如有签回单业务需要传此字段值
        //    if (msg.waybillNoInfoList.Where(item => item.waybillType == 3).FirstOrDefault().waybillNo != null)
        //    {
        //        //签回单号
        //        var backWaybillNo = msg.waybillNoInfoList.Where(item => item.waybillType == 3).FirstOrDefault().waybillNo;

        //        //签回单路由信息
        //        var backRoutelabelinfo = msg.routeLabelInfo.Where(item => item.routeLabelData.waybillNo == backWaybillNo).FirstOrDefault().routeLabelData;//取出路由标签

        //        var backRlsInfoDto = new RlsInfoDto[]
        //              {
        //                      new RlsInfoDto
        //                      {
        //                         abFlag = backRoutelabelinfo.abFlag,
        //                         codingMapping = backRoutelabelinfo.codingMapping,
        //                         codingMappingOut = backRoutelabelinfo.codingMappingOut,
        //                         destRouteLabel = backRoutelabelinfo.destRouteLabel,
        //                         destTeamCode = backRoutelabelinfo.destTeamCode,
        //                         printIcon = backRoutelabelinfo.printIcon,
        //                         proCode = backRoutelabelinfo.proCode,
        //                         qrcode = backRoutelabelinfo.twoDimensionCode,
        //                         sourceTransferCode = backRoutelabelinfo.sourceTransferCode,
        //                         waybillNo = backRoutelabelinfo.waybillNo,
        //                         xbFlag = backRoutelabelinfo.xbFlag
        //                     }
        //              };

        //        //签回单的收寄件人是和主订单相反的
        //        //签回单订单的到件方（收件人）信息
        //        var backConsigner = order.contactInfoList.Where(item => item.contactType == 1).FirstOrDefault();
        //        //取出订单的寄件方（寄件人）信息
        //        var backDeliver = order.contactInfoList.Where(item => item.contactType == 2).FirstOrDefault();

        //        //电子面单顶部是否需要logo 
        //        dto.isPrintLogo = "1";//1 需要  0 不需要
        //                              //这个必填 
        //                              //dto.appId = "SLKJ2019"; //对应丰桥平台获取的clientCode
        //                              //dto.appKey = "FBIqMkZjzxbsZgo7jTpeq7PD8CVzLT4Q"; //对应丰桥平台获取的checkWord

        //        dto.appId = "YDCXKg"; //对应丰桥平台获取的clientCode
        //        dto.appKey = "XKuM6bAGYZ2FvHmTb3FOyssM03LBquav"; //对应丰桥平台获取的checkWord

        //        //var msg = result.apiResultData.msgData;//下单结果数据

        //        dto.mailNo = msg.waybillNoInfoList.Where(item => item.waybillType == 1).FirstOrDefault().waybillNo;//取值回签单的运单号不知道设置这个值还是下面的签回单号值??

        //        //  dto.setMailNo("SF7551234567890,SF2000601520988,SF2000601520997");//子母单方式

        //        //签回单号  签单返回服务POD 会打印两份快单 其中第二份作为返寄的单==如有签回单业务需要传此字段值
        //        dto.returnTrackingNo = msg.waybillNoInfoList.Where(item => item.waybillType == 3).FirstOrDefault().waybillNo;

        //        //收件人信息  
        //        dto.consignerProvince = backConsigner.province;
        //        dto.consignerCity = backConsigner.city;
        //        dto.consignerCounty = backConsigner.county;
        //        dto.consignerAddress = backConsigner.address; //详细地址建议最多30个字  字段过长影响打印效果
        //        dto.consignerCompany = backConsigner.company;
        //        dto.consignerMobile = backConsigner.mobile;
        //        dto.consignerName = backConsigner.contact;
        //        dto.consignerShipperCode = backConsigner.postCode;
        //        dto.consignerTel = backConsigner.tel;


        //        //寄件人信息
        //        dto.deliverProvince = backDeliver.province;
        //        dto.deliverCity = backDeliver.city;
        //        dto.deliverCounty = backDeliver.county;
        //        dto.deliverAddress = backDeliver.address;
        //        dto.deliverCompany = backDeliver.company; //详细地址建议最多30个字  字段过长影响打印效果
        //        dto.deliverMobile = backDeliver.mobile;
        //        dto.deliverName = backDeliver.contact;
        //        dto.deliverShipperCode = backDeliver.postCode;
        //        dto.deliverTel = backDeliver.tel;

        //        dto.destCode = msg.destCode; //目的地代码 参考顺丰地区编号
        //        dto.zipCode = msg.originCode; //原寄地代码 参考顺丰地区编号

        //        dto.expressType = order.expressTypeId;
        //        //dto.insureValue = "501";//如果真实存在是在order.serviceList 中取到

        //        dto.monthAccount = order.monthlyCard; //月结卡号：寄方默认我们公司的月结，收方默认没有值，第三方支付时第三方公司的月结账号
        //        dto.orderNo = order.orderId;
        //        dto.payMethod = order.payMethod; // 1、寄付月结：显示寄付月结，不显示运费 2、寄付转第三方：显示寄付转第三方，不显示运费 3、寄付现结：按实际运费显示 4、到付：按实际需收取费用显示总计费用（含到付增值服务费，但不含COD费用)

        //        //dto.childRemark = "";//子单号备注
        //        dto.mainRemark = order.remark;
        //        //dto.returnTrackingRemark = "";//迁回单备注
        //        //dto.custLogo = "";
        //        //dto.logo = "";
        //        //dto.insureFee = "";
        //        //dto.payArea = "";
        //        //加密项
        //        dto.encryptCustName = true;//加密寄件人及收件人名称
        //        dto.encryptMobile = true;//加密寄件人及收件人联系手机	


        //        dto.rlsInfoDtoList = backRlsInfoDto;
        //        dto.cargoInfoDtoList = cargoInfoList.ToArray();


        //        if (isFengMi)
        //        {
        //            dto.rlsInfoDtoList = backRlsInfoDto;

        //        }

        //        waybillDtoList.Add(dto);
        //    }

        //    //加入新一个waybillDto
        //    dto = new WaybillDto();

        //    //取出订单的到件方（收件人）信息
        //    var consigner = order.contactInfoList.Where(item => item.contactType == 2).FirstOrDefault();
        //    //取出订单的寄件方（寄件人）信息
        //    var deliver = order.contactInfoList.Where(item => item.contactType == 1).FirstOrDefault();


        //    var mailNo = msg.waybillNoInfoList.Where(item => item.waybillType == 1).FirstOrDefault().waybillNo;
        //    var routeinfo = msg.routeLabelInfo.Where(item => item.routeLabelData.waybillNo == mailNo).FirstOrDefault().routeLabelData;//取出路由标签
        //    var rlsInfoDto = new RlsInfoDto[]
        //               {
        //                      new RlsInfoDto
        //                      {
        //                         abFlag = routeinfo.abFlag,
        //                         codingMapping = routeinfo.codingMapping,
        //                         codingMappingOut = routeinfo.codingMappingOut,
        //                         destRouteLabel = routeinfo.destRouteLabel,
        //                         destTeamCode = routeinfo.destTeamCode,
        //                         printIcon = routeinfo.printIcon,
        //                         proCode = routeinfo.proCode,
        //                         qrcode = routeinfo.twoDimensionCode,
        //                         sourceTransferCode = routeinfo.sourceTransferCode,
        //                         waybillNo = routeinfo.waybillNo,
        //                         xbFlag = routeinfo.xbFlag
        //                     }
        //               };

        //    //List<CargoInfoDto> cargoInfoList = new List<CargoInfoDto>();

        //    ////托寄物信息
        //    //order.cargoDetails.ToList().ForEach(item =>
        //    //{
        //    //    cargoInfoList.Add(new CargoInfoDto
        //    //    {
        //    //        //sku=item.goodsCode,//不理解sku是什么意思
        //    //        cargo = item.name,
        //    //        cargoCount = item.count,
        //    //        //remark =item.// order.cargoDetails并无对应remark字段
        //    //        cargoAmount = item.amount,
        //    //        cargoWeight = item.weight,
        //    //        cargoUnit = item.unit,
        //    //    });
        //    //});

        //    //List<WaybillDto> waybillDtoList = new List<WaybillDto>();
        //    //WaybillDto dto = new WaybillDto();

        //    //电子面单顶部是否需要logo 
        //    dto.isPrintLogo = "1";//1 需要  0 不需要
        //                          //这个必填 
        //                          //dto.appId = "SLKJ2019"; //对应丰桥平台获取的clientCode
        //                          //dto.appKey = "FBIqMkZjzxbsZgo7jTpeq7PD8CVzLT4Q"; //对应丰桥平台获取的checkWord

        //    dto.appId = "YDCXKg"; //对应丰桥平台获取的clientCode
        //    dto.appKey = "XKuM6bAGYZ2FvHmTb3FOyssM03LBquav"; //对应丰桥平台获取的checkWord

        //    //var msg = result.apiResultData.msgData;//下单结果数据

        //    dto.mailNo = msg.waybillNoInfoList.Where(item => item.waybillType == 1).FirstOrDefault().waybillNo;//主运单号，顺丰默认一件（因顺丰内部规则修改，涉及子母单的费用变动，不够重量的子母单收费会翻倍；所以，修改深圳库房打印顺丰快递面单逻辑，不再使用子母单：请求面单时，件数固定1件）

        //    //  dto.setMailNo("SF7551234567890,SF2000601520988,SF2000601520997");//子母单方式

        //    ////签回单号  签单返回服务POD 会打印两份快单 其中第二份作为返寄的单==如有签回单业务需要传此字段值
        //    //dto.returnTrackingNo = msg.waybillNoInfoList.Where(item => item.waybillType == 3).FirstOrDefault().waybillNo;

        //    //收件人信息  
        //    dto.consignerProvince = consigner.province;
        //    dto.consignerCity = consigner.city;
        //    dto.consignerCounty = consigner.county;
        //    dto.consignerAddress = consigner.address; //详细地址建议最多30个字  字段过长影响打印效果
        //    dto.consignerCompany = consigner.company;
        //    dto.consignerMobile = consigner.mobile;
        //    dto.consignerName = consigner.contact;
        //    dto.consignerShipperCode = consigner.postCode;
        //    dto.consignerTel = consigner.tel;


        //    //寄件人信息
        //    dto.deliverProvince = deliver.province;
        //    dto.deliverCity = deliver.city;
        //    dto.deliverCounty = deliver.county;
        //    dto.deliverAddress = deliver.address;
        //    dto.deliverCompany = deliver.company; //详细地址建议最多30个字  字段过长影响打印效果
        //    dto.deliverMobile = deliver.mobile;
        //    dto.deliverName = deliver.contact;
        //    dto.deliverShipperCode = deliver.postCode;
        //    dto.deliverTel = deliver.tel;

        //    dto.destCode = msg.destCode; //目的地代码 参考顺丰地区编号
        //    dto.zipCode = msg.originCode; //原寄地代码 参考顺丰地区编号

        //    dto.expressType = order.expressTypeId;
        //    //dto.insureValue = "501";//如果真实存在是在order.serviceList 中取到

        //    dto.monthAccount = order.monthlyCard; //月结卡号：寄方默认我们公司的月结，收方默认没有值，第三方支付时第三方公司的月结账号
        //    dto.orderNo = order.orderId;
        //    dto.payMethod = order.payMethod; // 1、寄付月结：显示寄付月结，不显示运费 2、寄付转第三方：显示寄付转第三方，不显示运费 3、寄付现结：按实际运费显示 4、到付：按实际需收取费用显示总计费用（含到付增值服务费，但不含COD费用)

        //    //dto.childRemark = "";//子单号备注
        //    dto.mainRemark = order.remark;
        //    //dto.returnTrackingRemark = "";//迁回单备注
        //    //dto.custLogo = "";
        //    //dto.logo = "";
        //    //dto.insureFee = "";
        //    //dto.payArea = "";
        //    //加密项
        //    dto.encryptCustName = true;//加密寄件人及收件人名称
        //    dto.encryptMobile = true;//加密寄件人及收件人联系手机	


        //    dto.rlsInfoDtoList = rlsInfoDto;
        //    dto.cargoInfoDtoList = cargoInfoList.ToArray();


        //    if (isFengMi)
        //    {
        //        dto.rlsInfoDtoList = rlsInfoDto;

        //    }

        //    waybillDtoList.Add(dto);

        //    return waybillDtoList;
        //}

        #endregion

        #region 原先的
        /// <summary>
        /// 组装打印参数
        /// </summary>
        /// <param name="order">订单信息</param>
        /// <param name="msg">返回结果数据</param>
        /// <param name="isFengMi">是否采用封密打印方式</param>
        /// <returns></returns>
        //private static List<WaybillDto> AssemblyParameters(SFOrder order, msgData msg, Boolean isFengMi)
        //{


        //    //取出订单的到件方（收件人）信息
        //    var consigner = order.contactInfoList.Where(item => item.contactType == 2).FirstOrDefault();
        //    //取出订单的寄件方（寄件人）信息
        //    var deliver = order.contactInfoList.Where(item => item.contactType == 1).FirstOrDefault();

        //    var routeinfo = msg.routeLabelInfo.FirstOrDefault().routeLabelData;//取出路由标签
        //    var rlsInfoDto = new RlsInfoDto[]
        //               {
        //                      new RlsInfoDto
        //                      {
        //                         abFlag = routeinfo.abFlag,
        //                         codingMapping = routeinfo.codingMapping,
        //                         codingMappingOut = routeinfo.codingMappingOut,
        //                         destRouteLabel = routeinfo.destRouteLabel,
        //                         destTeamCode = routeinfo.destTeamCode,
        //                         printIcon = routeinfo.printIcon,
        //                         proCode = routeinfo.proCode,
        //                         qrcode = routeinfo.twoDimensionCode,
        //                         sourceTransferCode = routeinfo.sourceTransferCode,
        //                         waybillNo = routeinfo.waybillNo,
        //                         xbFlag = routeinfo.xbFlag
        //                     }
        //               };

        //    List<CargoInfoDto> cargoInfoList = new List<CargoInfoDto>();

        //    //托寄物信息
        //    order.cargoDetails.ToList().ForEach(item =>
        //    {
        //        cargoInfoList.Add(new CargoInfoDto
        //        {
        //            //sku=item.goodsCode,//不理解sku是什么意思
        //            cargo = item.name,
        //            cargoCount = item.count,
        //            //remark =item.// order.cargoDetails并无对应remark字段
        //            cargoAmount = item.amount,
        //            cargoWeight = item.weight,
        //            cargoUnit = item.unit,
        //        });
        //    });

        //    List<WaybillDto> waybillDtoList = new List<WaybillDto>();
        //    WaybillDto dto = new WaybillDto();

        //    //电子面单顶部是否需要logo 
        //    dto.isPrintLogo = "1";//1 需要  0 不需要
        //                          //这个必填 
        //                          //dto.appId = "SLKJ2019"; //对应丰桥平台获取的clientCode
        //                          //dto.appKey = "FBIqMkZjzxbsZgo7jTpeq7PD8CVzLT4Q"; //对应丰桥平台获取的checkWord

        //    dto.appId = "YDCXKg"; //对应丰桥平台获取的clientCode
        //    dto.appKey = "XKuM6bAGYZ2FvHmTb3FOyssM03LBquav"; //对应丰桥平台获取的checkWord

        //    //var msg = result.apiResultData.msgData;//下单结果数据

        //    dto.mailNo = msg.waybillNoInfoList.Where(item => item.waybillType == 1).FirstOrDefault().waybillNo;//主运单号，顺丰默认一件（因顺丰内部规则修改，涉及子母单的费用变动，不够重量的子母单收费会翻倍；所以，修改深圳库房打印顺丰快递面单逻辑，不再使用子母单：请求面单时，件数固定1件）

        //    //收件人信息  
        //    dto.consignerProvince = consigner.province;
        //    dto.consignerCity = consigner.city;
        //    dto.consignerCounty = consigner.county;
        //    dto.consignerAddress = consigner.address; //详细地址建议最多30个字  字段过长影响打印效果
        //    dto.consignerCompany = consigner.company;
        //    dto.consignerMobile = consigner.mobile;
        //    dto.consignerName = consigner.contact;
        //    dto.consignerShipperCode = consigner.postCode;
        //    dto.consignerTel = consigner.tel;


        //    //寄件人信息
        //    dto.deliverProvince = deliver.province;
        //    dto.deliverCity = deliver.city;
        //    dto.deliverCounty = deliver.county;
        //    dto.deliverAddress = deliver.address;
        //    dto.deliverCompany = deliver.company; //详细地址建议最多30个字  字段过长影响打印效果
        //    dto.deliverMobile = deliver.mobile;
        //    dto.deliverName = deliver.contact;
        //    dto.deliverShipperCode = deliver.postCode;
        //    dto.deliverTel = deliver.tel;

        //    dto.destCode = msg.destCode; //目的地代码 参考顺丰地区编号
        //    dto.zipCode = msg.originCode; //原寄地代码 参考顺丰地区编号

        //    dto.expressType = order.expressTypeId;
        //    //dto.insureValue = "501";//如果真实存在是在order.serviceList 中取到

        //    dto.monthAccount = order.monthlyCard; //月结卡号：寄方默认我们公司的月结，收方默认没有值，第三方支付时第三方公司的月结账号
        //    dto.orderNo = order.orderId;
        //    dto.payMethod = order.payMethod; // 1、寄付月结：显示寄付月结，不显示运费 2、寄付转第三方：显示寄付转第三方，不显示运费 3、寄付现结：按实际运费显示 4、到付：按实际需收取费用显示总计费用（含到付增值服务费，但不含COD费用)

        //    //dto.childRemark = "";//子单号备注
        //    dto.mainRemark = order.remark;
        //    //dto.returnTrackingRemark = "";//迁回单备注
        //    //dto.custLogo = "";
        //    //dto.logo = "";
        //    //dto.insureFee = "";
        //    //dto.payArea = "";
        //    //加密项
        //    dto.encryptCustName = true;//加密寄件人及收件人名称
        //    dto.encryptMobile = true;//加密寄件人及收件人联系手机	


        //    dto.rlsInfoDtoList = rlsInfoDto;
        //    dto.cargoInfoDtoList = cargoInfoList.ToArray();


        //    if (isFengMi)
        //    {
        //        dto.rlsInfoDtoList = rlsInfoDto;

        //    }

        //    waybillDtoList.Add(dto);

        //    return waybillDtoList;
        //}

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

        #endregion

        #region 顺丰、跨越支付方式转变

        private static int ConvertSFExpayType(int exPayType)
        {
            switch (exPayType)
            {
                case 1:
                case 4:
                    return (int)SFPayType.DeliveryPay;
                case 2:
                    return (int)SFPayType.CollectPay;
                case 3:
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
                case 4:
                    return (int)KYPayType.DeliveryPay;
                case 2:
                    return (int)KYPayType.CollectPay;
                case 3:
                    return (int)KYPayType.ThirdPay;
                default:
                    throw new NotSupportedException("不支持此类型的快递方式");
            }
        }


        #endregion

    }
}
