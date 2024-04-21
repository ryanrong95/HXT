//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Net;
//using System.Security.Cryptography;
//using System.Text;

//namespace callExpressRequest
//{
//    //顺丰请求接口例子
//    class TestCallExpressService
//    {
//        static void Main(string[] args)
//        {
//            String partnerID = "SLKJ2019";//此处替换为您在丰桥平台获取的顾客编码          
//            String checkword = "FBIqMkZjzxbsZgo7jTpeq7PD8CVzLT4Q";//此处替换为您在丰桥平台获取的校验码            

//            String reqURL = "https://sfapi-sbox.sf-express.com/std/service";
//            //String reqURL = "https://sfapi.sf-express.com/std/service"; //生产环境

//            //将callExpressRequest文件件放于{program_}/Debug/文件夹下

//            //String serviceCode = "EXP_RECE_CREATE_ORDER";
//            //String path = "./callExpressRequest/01.order.json";//下订单

//            // String serviceCode = "EXP_RECE_SEARCH_ORDER_RESP";
//            // String path = "./callExpressRequest/02.order.query.json";//订单结果查询

//            // String serviceCode = "EXP_RECE_UPDATE_ORDER";
//            //  String path = "./callExpressRequest/03.order.confirm.json";//订单确认取消

//            //   String serviceCode = "EXP_RECE_FILTER_ORDER_BSP";
//            //   String path = "./callExpressRequest/04.order.filter.json";//订单筛选	

//            String serviceCode = "EXP_RECE_SEARCH_ROUTES";
//            String path = "./callExpressRequest/05.route_query_by_MailNo.json";//路由查询-通过运单号
//            //String path = "./callExpressRequest/05.route_query_by_OrderNo.json";//路由查询-通过订单号

//            //String serviceCode = "EXP_RECE_GET_SUB_MAILNO";
//            // String path = "./callExpressRequest/07.sub.mailno.json";//子单号申请

//            //  String serviceCode = "EXP_RECE_QUERY_SFWAYBILL";
//            // String path = "./callExpressRequest/09.waybills_fee.json";//清单运费查询            

//            String msgJson = Read(path);
//            String msgData = JsonCompress(msgJson);

//            String timestamp = GetTimeStamp(); //获取时间戳       

//            String requestID = System.Guid.NewGuid().ToString(); //获取uuid

//            String msgDigest = MD5ToBase64String(UrlEncode(msgData + timestamp + checkword));//获取数字签名

//            Console.WriteLine("partnerID: " + partnerID);
//            Console.WriteLine("--------------------------------------");
//            Console.WriteLine("checkword: " + checkword);
//            Console.WriteLine("--------------------------------------");
//            Console.WriteLine("timestamp: " + timestamp);
//            Console.WriteLine("--------------------------------------");
//            Console.WriteLine("requestID: " + requestID);
//            Console.WriteLine("--------------------------------------");
//            Console.WriteLine("msgDigest: " + msgDigest);
//            Console.WriteLine("--------------------------------------");
//            Console.WriteLine("请求报文: " + (msgData + timestamp + checkword));
//            Console.WriteLine("--------------------------------------");


//            String respJson = callSfExpressServiceByCSIM(reqURL, partnerID, requestID, serviceCode, timestamp, msgDigest, msgData);

//            if (respJson != null)
//            {
//                Console.WriteLine("--------------------------------------");
//                Console.WriteLine("返回报文: " + respJson);
//                Console.WriteLine("--------------------------------------");
//                Console.ReadKey(true);
//            }

//        }

//        private static string JsonCompress(string msgData)
//        {
//            StringBuilder sb = new StringBuilder();
//            using (StringReader reader = new StringReader(msgData))
//            {
//                int ch = -1;
//                int lastch = -1;
//                bool isQuoteStart = false;
//                while ((ch = reader.Read()) > -1)
//                {
//                    if ((char)lastch != '\\' && (char)ch == '\"')
//                    {
//                        if (!isQuoteStart)
//                        {
//                            isQuoteStart = true;
//                        }
//                        else
//                        {
//                            isQuoteStart = false;
//                        }
//                    }
//                    if (!Char.IsWhiteSpace((char)ch) || isQuoteStart)
//                    {
//                        sb.Append((char)ch);
//                    }
//                    lastch = ch;
//                }
//            }
//            return sb.ToString();
//        }

//        private static string callSfExpressServiceByCSIM(string reqURL, string partnerID, string requestID, string serviceCode, string timestamp, string msgDigest, string msgData)
//        {
//            String result = "";
//            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(reqURL);
//            req.Method = "POST";
//            req.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";

//            Dictionary<string, string> content = new Dictionary<string, string>();
//            content["partnerID"] = partnerID;
//            content["requestID"] = requestID;
//            content["serviceCode"] = serviceCode;
//            content["timestamp"] = timestamp;
//            content["msgDigest"] = msgDigest;
//            content["msgData"] = msgData;

//            if (!(content == null || content.Count == 0))
//            {
//                StringBuilder buffer = new StringBuilder();
//                int i = 0;
//                foreach (string key in content.Keys)
//                {
//                    if (i > 0)
//                    {
//                        buffer.AppendFormat("&{0}={1}", key, content[key]);
//                    }
//                    else
//                    {
//                        buffer.AppendFormat("{0}={1}", key, content[key]);
//                    }
//                    i++;
//                }

//                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
//                req.ContentLength = data.Length;
//                using (Stream reqStream = req.GetRequestStream())
//                {
//                    reqStream.Write(data, 0, data.Length);
//                    reqStream.Close();
//                }

//            }

//            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
//            Stream stream = resp.GetResponseStream();
//            //获取响应内容
//            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
//            {
//                result = reader.ReadToEnd();
//            }
//            return result;
//        }

//        private static string UrlEncode(string str)
//        {
//            StringBuilder builder = new StringBuilder();
//            foreach (char c in str)
//            {
//                if (System.Web.HttpUtility.UrlEncode(c.ToString()).Length > 1)
//                {
//                    builder.Append(System.Web.HttpUtility.UrlEncode(c.ToString()).ToUpper());
//                }
//                else
//                {
//                    builder.Append(c);
//                }
//            }
//            return builder.ToString();
//        }

//        private static string MD5ToBase64String(string str)
//        {
//            MD5 md5 = new MD5CryptoServiceProvider();
//            byte[] MD5 = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(str));//MD5(注意UTF8编码)
//            string result = Convert.ToBase64String(MD5);//Base64
//            return result;
//        }

//        private static string Read(string path)
//        {
//            StreamReader sr = new StreamReader(path, Encoding.UTF8);

//            StringBuilder builder = new StringBuilder();
//            String line;
//            while ((line = sr.ReadLine()) != null)
//            {
//                builder.Append(line);
//            }
//            return builder.ToString();
//        }

//        private static string GetTimeStamp()
//        {
//            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
//            return Convert.ToInt64(ts.TotalSeconds).ToString();
//        }
//    }
//}