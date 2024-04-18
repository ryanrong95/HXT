using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yahv.PvWsOrder.Services.Warehouse;
using Yahv.Services.Models;
using Yahv.Utils.Serializers;
using Yahv;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {

        /// <summary>
        /// 发送Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        static public string HttpGet(string url)
        {
            Stream instream = null;
            StreamReader sr = null;
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Encoding encoding = Encoding.UTF8;
            // 准备请求...
            try
            {
                // 设置参数
                request = WebRequest.Create(url) as HttpWebRequest;
                CookieContainer cookieContainer = new CookieContainer();
                request.CookieContainer = cookieContainer;
                request.AllowAutoRedirect = true;
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                //发送请求并获取相应回应数据
                response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                instream = response.GetResponseStream();
                sr = new StreamReader(instream, encoding);
                //返回结果网页（html）代码
                string content = sr.ReadToEnd();
                string err = string.Empty;
                return content;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                return string.Empty;
            }
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="mobile">接收方手机号码</param>
        /// <param name="message">短信内容</param>
        public static void Send(string mobile, string message)
        {
            string url = string.Format("http://cf.51welink.com/submitdata/Service.asmx/g_Submit?sname=dlydcx00&spwd=rYfl76qL&scorpid=&sprdid=1012818&sdst={0}&smsg={1}", mobile, message);
            string xml = HttpGet(url);
            //XmlDocument doc = new XmlDocument();
            //doc.LoadXml(xml);
            //string state;
            //XmlNodeList nodeList = doc.GetElementsByTagName("CSubmitState");
            //if (nodeList != null && nodeList.Count > 0)
            //{
            //    foreach (XmlNode item in nodeList[0].ChildNodes)
            //    {
            //        if (item.Name == "State")
            //        {
            //            state = item.InnerText;
            //            break;
            //        }
            //    }
            //}

            //if (state == "")
            //{

            //}
        }

        public const string Register = "您好，您注册芯达通账号的校验码为{0},请不要把校验码泄露给其他人！该校验码3分钟内有效，非本人操作，请忽略本条消息。【芯达通】";

        [TestMethod]
        public void TestMethod1()
        {
            Random ran = new Random();
            string messageCode = ran.Next(0, 999999).ToString("000000"); //随机生成的验证码
            string msg = string.Format(Register, messageCode);

            Send("13601053250", msg);

            //var current = Erp.Current.Leagues.Current;

            //foreach (var item in Erp.Current.Leagues)
            //{

            //}




            //string str = "{\"Notices\":[{\"Product\":null,\"Picking\":null,\"Output\":{\"ID\":\"Opt2019111500000013\",\"InputID\":null,\"OrderID\":\"Order201911150004\",\"ItemID\":\"OrderItem20191115000133\",\"OwnerID\":\"SA01\",\"SalerID\":null,\"CustomerServiceID\":null,\"PurchaserID\":null,\"Currency\":2,\"Price\":100.0,\"StorageID\":\"STOR20191115000002\",\"CreateDate\":\"2019-11-15T09:49:39.2423548+08:00\"},\"ID\":null,\"Type\":305,\"WareHouseID\":null,\"WaybillID\":\"Waybill201911150004\",\"InputID\":null,\"OutputID\":\"Opt2019111500000013\",\"ProductID\":\"F403C1EDCA449C362985D7AF108E2291\",\"Supplier\":null,\"DateCode\":null,\"Quantity\":10,\"Conditions\":{\"DevanningCheck\":false,\"Weigh\":false,\"CheckNumber\":false,\"OnlineDetection\":false,\"AttachLabel\":false,\"PaintLabel\":false,\"Repacking\":false,\"PickByValue\":false},\"CreateDate\":\"2019-11-15T09:49:39.2423548+08:00\",\"Status\":100,\"Source\":20,\"Target\":400,\"BoxCode\":null,\"Weight\":0.02,\"Volume\":0.01,\"ShelveID\":null,\"Files\":[]},{\"Product\":null,\"Picking\":null,\"Output\":{\"ID\":null,\"InputID\":null,\"OrderID\":\"Order201911150004\",\"ItemID\":\"OrderItem20191115000134\",\"OwnerID\":\"SA01\",\"SalerID\":null,\"CustomerServiceID\":null,\"PurchaserID\":null,\"Currency\":2,\"Price\":100.0,\"StorageID\":\"STOR20191115000003\",\"CreateDate\":\"2019-11-15T09:49:39.2423548+08:00\"},\"ID\":null,\"Type\":305,\"WareHouseID\":null,\"WaybillID\":\"Waybill201911150004\",\"InputID\":null,\"OutputID\":null,\"ProductID\":\"7051DC4E0E95757F1F64E317287E0050\",\"Supplier\":null,\"DateCode\":null,\"Quantity\":10,\"Conditions\":{\"DevanningCheck\":false,\"Weigh\":false,\"CheckNumber\":false,\"OnlineDetection\":false,\"AttachLabel\":false,\"PaintLabel\":false,\"Repacking\":false,\"PickByValue\":false},\"CreateDate\":\"2019-11-15T09:49:39.2423548+08:00\",\"Status\":100,\"Source\":20,\"Target\":400,\"BoxCode\":null,\"Weight\":0.02,\"Volume\":0.01,\"ShelveID\":null,\"Files\":[]}]}";
            //string str = "{\"Notices\":[{\"Product\":{\"ID\":\"7D24EABB91459865D26444F3EE2C9787\",\"PartNumber\":\" RSH070N05TB1 \",\"Manufacturer\":\"ROHM\",\"PackageCase\":null,\"Packaging\":null,\"CreateDate\":\"0001-01-01T00:00:00\"},\"Picking\":null,\"Output\":{\"ID\":null,\"InputID\":\"Ipt2020010200000019\",\"OrderID\":\"XL00120191207015\",\"TinyOrderID\":\"XL00120191207015\",\"ItemID\":\"OrderItem20200102000012\",\"OwnerID\":\"Admin0000000333\",\"SalerID\":null,\"CustomerServiceID\":null,\"PurchaserID\":null,\"Currency\":2,\"Price\":25.00000000000000,\"StorageID\":\"STOR20200102000038\",\"CreateDate\":\"0001-01-01T00:00:00\",\"Checker\":null},\"ID\":null,\"Type\":315,\"WareHouseID\":\"HK01_WLT\",\"WaybillID\":null,\"InputID\":\"Ipt2020010200000019\",\"OutputID\":null,\"ProductID\":\"7D24EABB91459865D26444F3EE2C9787\",\"Supplier\":null,\"DateCode\":\"001\",\"Quantity\":1.0000000,\"StockQuantity\":0.0,\"SurplusQuantity\":-1.0000000,\"Conditions\":null,\"CreateDate\":\"2020-01-03T16:03:53.0810376+08:00\",\"Status\":100,\"Source\":30,\"Target\":200,\"BoxCode\":\"WL01\",\"Weight\":null,\"NetWeight\":null,\"Volume\":null,\"ShelveID\":null,\"Files\":null,\"Visable\":true,\"Checked\":false,\"Input\":null},{\"Product\":{\"ID\":\"7D24EABB91459865D26444F3EE2C9787\",\"PartNumber\":\" RSH070N05TB1 \",\"Manufacturer\":\"ROHM\",\"PackageCase\":null,\"Packaging\":null,\"CreateDate\":\"0001-01-01T00:00:00\"},\"Picking\":null,\"Output\":{\"ID\":null,\"InputID\":\"Ipt2020010200000019\",\"OrderID\":\"XL00120191207015\",\"TinyOrderID\":\"XL00120191207015\",\"ItemID\":\"OrderItem20200102000012\",\"OwnerID\":\"Admin0000000333\",\"SalerID\":null,\"CustomerServiceID\":null,\"PurchaserID\":null,\"Currency\":2,\"Price\":25.00000000000000,\"StorageID\":\"STOR20200102000040\",\"CreateDate\":\"0001-01-01T00:00:00\",\"Checker\":null},\"ID\":null,\"Type\":315,\"WareHouseID\":\"HK01_WLT\",\"WaybillID\":null,\"InputID\":\"Ipt2020010200000019\",\"OutputID\":null,\"ProductID\":\"7D24EABB91459865D26444F3EE2C9787\",\"Supplier\":null,\"DateCode\":\"001\",\"Quantity\":1.0000000,\"StockQuantity\":0.0,\"SurplusQuantity\":-1.0000000,\"Conditions\":null,\"CreateDate\":\"2020-01-03T16:03:53.0820349+08:00\",\"Status\":100,\"Source\":30,\"Target\":200,\"BoxCode\":null,\"Weight\":null,\"NetWeight\":null,\"Volume\":null,\"ShelveID\":null,\"Files\":null,\"Visable\":true,\"Checked\":false,\"Input\":null},{\"Product\":{\"ID\":\"58FE5E3D2C13C24FA74136FB61836D61\",\"PartNumber\":\" S-LBAS21LT1G\",\"Manufacturer\":\"LRC\",\"PackageCase\":null,\"Packaging\":null,\"CreateDate\":\"0001-01-01T00:00:00\"},\"Picking\":null,\"Output\":{\"ID\":null,\"InputID\":\"Ipt2020010200000020\",\"OrderID\":\"XL00120191207015\",\"TinyOrderID\":\"XL00120191207015\",\"ItemID\":\"OrderItem20200102000013\",\"OwnerID\":\"Admin0000000333\",\"SalerID\":null,\"CustomerServiceID\":null,\"PurchaserID\":null,\"Currency\":2,\"Price\":10.00000000000000,\"StorageID\":\"STOR20200102000039\",\"CreateDate\":\"0001-01-01T00:00:00\",\"Checker\":null},\"ID\":null,\"Type\":315,\"WareHouseID\":\"HK01_WLT\",\"WaybillID\":null,\"InputID\":\"Ipt2020010200000020\",\"OutputID\":null,\"ProductID\":\"58FE5E3D2C13C24FA74136FB61836D61\",\"Supplier\":null,\"DateCode\":\"001\",\"Quantity\":1.0000000,\"StockQuantity\":0.0,\"SurplusQuantity\":-1.0000000,\"Conditions\":null,\"CreateDate\":\"2020-01-03T16:03:53.0820349+08:00\",\"Status\":100,\"Source\":30,\"Target\":200,\"BoxCode\":null,\"Weight\":null,\"NetWeight\":null,\"Volume\":null,\"ShelveID\":null,\"Files\":null,\"Visable\":true,\"Checked\":false,\"Input\":null},{\"Product\":{\"ID\":\"58FE5E3D2C13C24FA74136FB61836D61\",\"PartNumber\":\" S-LBAS21LT1G\",\"Manufacturer\":\"LRC\",\"PackageCase\":null,\"Packaging\":null,\"CreateDate\":\"0001-01-01T00:00:00\"},\"Picking\":null,\"Output\":{\"ID\":null,\"InputID\":\"Ipt2020010200000020\",\"OrderID\":\"XL00120191207015\",\"TinyOrderID\":\"XL00120191207015\",\"ItemID\":\"OrderItem20200102000013\",\"OwnerID\":\"Admin0000000333\",\"SalerID\":null,\"CustomerServiceID\":null,\"PurchaserID\":null,\"Currency\":2,\"Price\":10.00000000000000,\"StorageID\":\"STOR20200102000042\",\"CreateDate\":\"0001-01-01T00:00:00\",\"Checker\":null},\"ID\":null,\"Type\":315,\"WareHouseID\":\"HK01_WLT\",\"WaybillID\":null,\"InputID\":\"Ipt2020010200000020\",\"OutputID\":null,\"ProductID\":\"58FE5E3D2C13C24FA74136FB61836D61\",\"Supplier\":null,\"DateCode\":\"001\",\"Quantity\":1.0000000,\"StockQuantity\":0.0,\"SurplusQuantity\":-1.0000000,\"Conditions\":null,\"CreateDate\":\"2020-01-03T16:03:53.0820349+08:00\",\"Status\":100,\"Source\":30,\"Target\":200,\"BoxCode\":null,\"Weight\":null,\"NetWeight\":null,\"Volume\":null,\"ShelveID\":null,\"Files\":null,\"Visable\":true,\"Checked\":false,\"Input\":null}],\"ExcuteStatus\":200,\"ExcuteStatusDescription\":\"待处理\",\"TotalGoodsValue\":null,\"TotalPieces\":1.0,\"Files\":[],\"OrderID\":\"XL00120191207015\",\"WaybillTypeDescription\":\"送货上门\",\"Source\":30,\"SourceDescription\":\"代报关\",\"PlaceID\":\"9\",\"PlaceDescription\":\"中国\",\"Conditions\":null,\"WaybillID\":null,\"Code\":null,\"CreateDate\":\"2020-01-03T16:03:38.7363545+08:00\",\"EnterCode\":null,\"ClientName\":\"杭州比一比电子科技有限公司\",\"ClientID\":null,\"WaybillType\":2,\"CarrierID\":\"运达国际物流\",\"CarrierName\":\"运达国际物流有限公司\",\"Supplier\":null,\"Place\":\"CHN\",\"ConsignorID\":null,\"ConsigneeID\":null,\"Consignee\":{\"ID\":\"397472D8F8E104920F5AE5C21B63F991\",\"Company\":\"深圳市芯达通供应链管理有限公司\",\"Place\":\"CHN\",\"Address\":\"深圳市龙岗区吉华路393号英达丰科技园1号楼\",\"Contact\":\"张庆永\",\"Phone\":\"0755-83988698\",\"Zipcode\":null,\"Email\":null,\"CreateDate\":\"2020-01-03T16:03:38.7493201+08:00\",\"IDType\":1,\"IDNumber\":null},\"Consignor\":{\"ID\":\"15945E55F580F7FDBA4A8A87997E2C3C\",\"Company\":\"香港万路通国际物流有限公司\",\"Place\":\"CHN\",\"Address\":\"九龙观塘成业街16号 怡生工业大厦11/F B2单元\",\"Contact\":\"林团裕\",\"Phone\":\"（852）31019258\",\"Zipcode\":null,\"Email\":null,\"CreateDate\":\"2020-01-03T16:03:38.7543068+08:00\",\"IDType\":1,\"IDNumber\":null},\"WayLoading\":null,\"WayChcd\":{\"ID\":null,\"LotNumber\":\"1100325928928\",\"CarNumber1\":\"VT3330\",\"CarNumber2\":\"粵Z.HB88港\",\"Carload\":4,\"IsOnevehicle\":true,\"Driver\":\"李永銘\",\"PlanDate\":\"2019-12-07T00:00:00\",\"DepartDate\":null,\"TotalQuantity\":0},\"WayCharge\":null,\"TotalParts\":1,\"TotalWeight\":2.00000,\"TotalVolume\":null,\"VoyageNumber\":\"1100325928928\",\"Summary\":null,\"Status\":200,\"Packaging\":null,\"TransferID\":null,\"FatherID\":null}";

            //try
            //{
            //    var result = Yahv.Utils.Http.ApiHelper.Current.PostData("http://hv.warehouse.b1b.com/wmsapi/pickings/Submit", str.JsonTo<PickingWaybill>());
            //}
            //catch (Exception ex)
            //{
            //    string mess = ex.Message;
            //}

            //Yahv.Utils.Http.ApiHelper.Current.PostData("http://hv.warehouse.b1b.com/wmsapi/sortings/Submit", str.JsonTo<SortingWaybill>());

        }
    }
}
