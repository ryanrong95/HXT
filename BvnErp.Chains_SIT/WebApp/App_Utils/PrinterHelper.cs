using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils.Serializers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebApp.App_Utils
{
    public class PrinterHelper
    {
        #region 逻辑流程
        //1.先请求顺丰/跨越下单接口
        //2.请求打印接口
        #endregion

        
        static public void FacePrint(string orderID, string shipperCode, int expType, int exPayType, Sender sender, Receiver receiver, int quantity, string remark
            , double? volume, double? weight
            , params Commodity[] commodities)
        {
           

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
                        monthlyCard = "7550205279",
                        orderId = orderID,
                        expressTypeId = expType,
                        payMethod = 1,//暂时固定为1：寄付 exPayType,
                        //parcelQty = quantity,
                        parcelQty = 1,//固定值为1
                        remark = remark,
                        volume = volume,
                        totalWeight = weight,
                        cargoDetails = cargo,
                        contactInfoList = new List<contactInfo>
                        {
                            new contactInfo
                            {
                                contactType=1,
                                address=sender.Address.Trim(),
                                contact=sender.Name,
                                province=sender.ProvinceName,
                                city=sender.CityName,
                                county=sender.ExpAreaName,
                                mobile=sender.Mobile,
                                tel="",
                                company=sender.Company
                            },
                            new contactInfo
                            {
                                contactType=2,
                                address=receiver.Address.Trim(),
                                contact=receiver.Name,
                                province=receiver.ProvinceName,
                                city=receiver.CityName,
                                county=receiver.ExpAreaName,
                                mobile=receiver.Mobile,
                                tel="",
                                company=receiver.Company
                            }
                        }
                    };

                    string partnerID = "YDCXKg";//此处替换为您在丰桥平台获取的顾客编码          
                    string checkword = "XKuM6bAGYZ2FvHmTb3FOyssM03LBquav";//此处替换为您在丰桥平台获取的校验码     
                    string reqURL = "https://sfapi-sbox.sf-express.com/std/service";
                    //string reqURL = "https://sfapi.sf-express.com/std/service"; //生产环境

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

                                string jsonParam = waybillDtoList.Json();

                                SFWaybillPrinter printer = new SFWaybillPrinter();

                               

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

                                   
                                    rlt = respJson.JsonTo<JObject>();

                                    apiResultCode = rlt["apiResultCode"].Value<string>();
                                    apiResultData = rlt["apiResultData"];

                                    resultData = apiResultData.ToString().JsonTo<apiResultData>();

                                   

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


                                        //List<WaybillDto> waybillDtoList = AssemblyParameters(order, msg, true);

                                        SFWaybillPrinter printer = new SFWaybillPrinter();

                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                           
                            //throw new Exception("未知错误");
                        }
                    }

                    #endregion

                    //
                    //waybillDtoList.Json(); 
                }
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

        /// <summary>
        /// 组装打印参数
        /// </summary>
        /// <param name="order">订单信息</param>
        /// <param name="msg">返回结果数据</param>
        /// <param name="isFengMi">是否采用封密打印方式</param>
        /// <returns></returns>
        private static List<WaybillDto> AssemblyParameters(SFOrder order, msgData msg, Boolean isFengMi)
        {

            //取出订单的到件方（收件人）信息
            var consigner = order.contactInfoList.Where(item => item.contactType == 2).FirstOrDefault();
            //取出订单的寄件方（寄件人）信息
            var deliver = order.contactInfoList.Where(item => item.contactType == 1).FirstOrDefault();

            var routeinfo = msg.routeLabelInfo.FirstOrDefault().routeLabelData;//取出路由标签
            var rlsInfoDto = new RlsInfoDto[]
                       {
                              new RlsInfoDto
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
                             }
                       };

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

            List<WaybillDto> waybillDtoList = new List<WaybillDto>();
            WaybillDto dto = new WaybillDto();

            //电子面单顶部是否需要logo 
            dto.isPrintLogo = "1";//1 需要  0 不需要
                                  //这个必填 
                                  //dto.appId = "SLKJ2019"; //对应丰桥平台获取的clientCode
                                  //dto.appKey = "FBIqMkZjzxbsZgo7jTpeq7PD8CVzLT4Q"; //对应丰桥平台获取的checkWord

            dto.appId = "YDCXKg"; //对应丰桥平台获取的clientCode
            dto.appKey = "XKuM6bAGYZ2FvHmTb3FOyssM03LBquav"; //对应丰桥平台获取的checkWord

            //var msg = result.apiResultData.msgData;//下单结果数据

            dto.mailNo = msg.waybillNoInfoList.Where(item => item.waybillType == 1).FirstOrDefault().waybillNo;//主运单号，顺丰默认一件（因顺丰内部规则修改，涉及子母单的费用变动，不够重量的子母单收费会翻倍；所以，修改深圳库房打印顺丰快递面单逻辑，不再使用子母单：请求面单时，件数固定1件）

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

            dto.monthAccount = "7550205279"; //月结卡号
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


            dto.rlsInfoDtoList = rlsInfoDto;
            dto.cargoInfoDtoList = cargoInfoList.ToArray();


            if (isFengMi)
            {
                dto.rlsInfoDtoList = rlsInfoDto;

            }

            waybillDtoList.Add(dto);

            return waybillDtoList;
        }
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

        
    }
}
