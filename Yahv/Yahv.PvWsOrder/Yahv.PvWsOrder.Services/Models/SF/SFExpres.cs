using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Yahv.Payments;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsOrder.Services.Models
{
    public class SFExpres
    {
        public SFRequestPara sfPara { get; set; }

        public SFResponse Order()
        {
            SFResponse checkResult = RequestCheck();
            if (!string.IsNullOrEmpty(checkResult.WaybillNo))
            {
                return checkResult;
            }
            else
            {
                return RequestOrder();
            }
        }

        public SFResponse RequestCheck()
        {
            string waybillNo = "";
            string filePath = "";
            bool success = true;

            SFCall call = new SFCall();
            call.ServiceCode = SFConfig.Current.CheckServiceCode;
            call.RequestID = System.Guid.NewGuid().ToString();
            call.MsgJson = new
            {
                searchType = 1,
                orderId = sfPara.OrderID,
                language = SFConfig.Current.Language,
            }.Json();

            string respJson = call.Call();

            List<cargoDetail> cargo = new List<cargoDetail>();
            sfPara.Commodities.ToList().ForEach(item =>
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
                language = SFConfig.Current.Language,
                monthlyCard = SFConfig.Current.MonthlyCard,
                orderId = sfPara.OrderID,
                expressTypeId = sfPara.ExpType,
                payMethod = sfPara.ExPayType,
                parcelQty = sfPara.Quantity,
                remark = sfPara.Remark,
                volume = sfPara.Volume,
                totalWeight = sfPara.Weight,
                cargoDetails = cargo,
                contactInfoList = new List<contactInfo>
                        {
                            new contactInfo
                            {
                                contactType=1,
                                address= sfPara.Sender.Address.Trim(),
                                contact= sfPara.Sender.Name,
                                province= sfPara.Sender.ProvinceName,
                                city= sfPara.Sender.CityName,
                                county= sfPara.Sender.ExpAreaName,
                                mobile= sfPara.Sender.Mobile,
                                tel="",
                                company= sfPara.Sender.Company
                            },
                            new contactInfo
                            {
                                contactType=2,
                                address= sfPara.Receiver.Address.Trim(),
                                contact= sfPara.Receiver.Name,
                                province= sfPara.Receiver.ProvinceName,
                                city= sfPara.Receiver.CityName,
                                county= sfPara.Receiver.ExpAreaName,
                                mobile= sfPara.Receiver.Mobile,
                                tel="",
                                company= sfPara.Receiver.Company
                            }
                        }
            };
            List<WaybillDto> waybillDtos = ParseJsonResult(order, respJson);
            if (waybillDtos != null)
            {
                waybillNo = waybillDtos.FirstOrDefault().mailNo;
                filePath = RequestImage(waybillDtos);
            }
            else
            {
                success = false;
            }

            SFResponse response = new SFResponse();
            response.WaybillNo = waybillNo;
            response.FilePath = filePath;
            response.Success = success;
            return response;
        }

        public SFResponse RequestOrder()
        {
            string waybillNo = "";
            string filePath = "";
            bool success = true;
            //组装货物信息
            List<cargoDetail> cargo = new List<cargoDetail>();
            sfPara.Commodities.ToList().ForEach(item =>
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
                language = SFConfig.Current.Language,
                monthlyCard = SFConfig.Current.MonthlyCard,
                orderId = sfPara.OrderID,
                expressTypeId = sfPara.ExpType,
                payMethod = sfPara.ExPayType,
                parcelQty = sfPara.Quantity,
                remark = sfPara.Remark,
                volume = sfPara.Volume,
                totalWeight = sfPara.Weight,
                cargoDetails = cargo,
                contactInfoList = new List<contactInfo>
                        {
                            new contactInfo
                            {
                                contactType=1,
                                address= sfPara.Sender.Address.ToKdnFullAngle().Trim(),
                                contact= sfPara.Sender.Name,
                                province= sfPara.Sender.ProvinceName,
                                city= sfPara.Sender.CityName,
                                county= sfPara.Sender.ExpAreaName,
                                mobile= sfPara.Sender.Mobile,
                                company= sfPara.Sender.Company
                            },
                            new contactInfo
                            {
                                contactType=2,
                                address= sfPara.Receiver.Address.ToKdnFullAngle().Trim(),
                                contact= sfPara.Receiver.Name,
                                province= sfPara.Receiver.ProvinceName,
                                city= sfPara.Receiver.CityName,
                                county= sfPara.Receiver.ExpAreaName,
                                mobile= sfPara.Receiver.Mobile,
                                company= sfPara.Receiver.Company
                            }
                        }
            };

            SFCall call = new SFCall();
            call.ServiceCode = SFConfig.Current.OrderServiceCode;
            call.RequestID = System.Guid.NewGuid().ToString();
            call.MsgJson = order.Json();

            string respJson = call.Call();

            List<WaybillDto> waybillDtos = ParseJsonResult(order, respJson);
            if (waybillDtos != null)
            {
                waybillNo = waybillDtos.FirstOrDefault().mailNo;
                filePath = RequestImage(waybillDtos);
            }
            else
            {
                success = false;
            }

            SFResponse response = new SFResponse();
            response.WaybillNo = waybillNo;
            response.FilePath = filePath;
            response.Success = success;
            return response;
        }

        public string RequestImage(List<WaybillDto> waybillDtoList)
        {
            try
            {
                string jsonParam = waybillDtoList.Json();
                var waybillDto = waybillDtoList.FirstOrDefault();
                string fileName = waybillDto.mailNo + ".jpg";
                FileDirectory dic = new FileDirectory(fileName, FileType.SFImages);
                string filePath = dic.DownLoadRoot + fileName;

                string reqURL = SFConfig.Current.ImageURL;

                string result = postJson(reqURL, jsonParam);

               

                JObject rlt = result.JsonTo<JObject>();
                var apiResultCode = rlt["code"].Value<string>();
                JToken apiResultData = rlt["result"];

                

                generateImage(apiResultData.FirstOrDefault().ToString(), filePath);

                return filePath;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private List<WaybillDto> ParseJsonResult(SFOrder order, string respJson)
        {
            if (!string.IsNullOrEmpty(respJson))
            {
                JObject rlt = respJson.JsonTo<JObject>();

                var apiResultCode = rlt["apiResultCode"].Value<string>();
                JToken apiResultData = rlt["apiResultData"];

                apiResultData resultData = apiResultData.ToString().JsonTo<apiResultData>();

                //查询结果成功直接打印
                if (apiResultCode == "A1000" && resultData.success == true)
                {
                    var msg = resultData.msgData;//下单结果数据
                    //waybillNo = msg.waybillNoInfoList.FirstOrDefault().waybillNo;
                    List<WaybillDto> waybillDtos = AssemblyParameters(order, msg);
                    return waybillDtos;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private List<WaybillDto> AssemblyParameters(SFOrder order, msgData msg)
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
                    cargo = item.name,
                    cargoCount = item.count,
                    cargoAmount = item.amount,
                    cargoWeight = item.weight,
                    cargoUnit = item.unit,
                });
            });

            List<WaybillDto> waybillDtoList = new List<WaybillDto>();
            WaybillDto dto = new WaybillDto();

            //电子面单顶部是否需要logo 
            dto.isPrintLogo = "1";//1 需要  0 不需要
            dto.appId = SFConfig.Current.PartnerID;
            dto.appKey = SFConfig.Current.Checkword;

            dto.mailNo = msg.waybillNoInfoList.Where(item => item.waybillType == 1).FirstOrDefault().waybillNo;//主运单号，顺丰默认一件（因顺丰内部规则修改，涉及子母单的费用变动，不够重量的子母单收费会翻倍；所以，修改深圳库房打印顺丰快递面单逻辑，不再使用子母单：请求面单时，件数固定1件）

            //收件人信息  
            dto.consignerProvince = consigner.province;
            dto.consignerCity = consigner.city;
            dto.consignerCounty = consigner.county;
            dto.consignerAddress = consigner.address;
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
            dto.deliverCompany = deliver.company;
            dto.deliverMobile = deliver.mobile;
            dto.deliverName = deliver.contact;
            dto.deliverShipperCode = deliver.postCode;
            dto.deliverTel = deliver.tel;

            dto.destCode = msg.destCode; //目的地代码 参考顺丰地区编号
            dto.zipCode = msg.originCode; //原寄地代码 参考顺丰地区编号
            dto.expressType = order.expressTypeId;

            dto.monthAccount = SFConfig.Current.MonthlyCard; //月结卡号
            dto.orderNo = order.orderId;
            dto.payMethod = order.payMethod;

            dto.mainRemark = order.remark;
            dto.encryptCustName = true;//加密寄件人及收件人名称
            dto.encryptMobile = true;//加密寄件人及收件人联系手机	


            dto.rlsInfoDtoList = rlsInfoDto;
            dto.cargoInfoDtoList = cargoInfoList.ToArray();
            dto.rlsInfoDtoList = rlsInfoDto;


            waybillDtoList.Add(dto);

            return waybillDtoList;
        }

        private string postJson(string reqURL, string jsonParm)
        {

            string httpResult = "";
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(reqURL);
                //req.ContentType = "application/json";
                //req.ContentType = "application/x-www-form-urlencoded";
                req.ContentType = "application/json;charset=utf-8";

                req.Method = "POST";
                req.Timeout = 20000;

                byte[] bs = System.Text.Encoding.UTF8.GetBytes(jsonParm);


                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    //在这里对接收到的页面内容进行处理
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default))
                    {
                        httpResult = sr.ReadToEnd().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return httpResult;
        }

        public Boolean generateImage(string imgStr, string imgFilePath)
        {
            if (imgStr == null)
                return false;
            try
            {
                byte[] bytes = Convert.FromBase64String(imgStr);

                int x = 256;
                byte a = (byte)x;

                for (int i = 0; i < bytes.Length; i++)
                {
                    if (bytes[i] < 0)
                    {
                        bytes[i] += a;
                    }
                }

                using (FileStream fs = new FileStream(imgFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
    }
}
