using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Xml;

namespace WebApp.App_Utils
{
    public class EmsApiHelper
    {
        //测试环境地址
        private string ReqURL = System.Configuration.ConfigurationManager.AppSettings["EMSRequestURL"];

        private string partnered = System.Configuration.ConfigurationManager.AppSettings["EMSPartnered"];

        private string msg_type = "ORDERCREATE";

        private string ecCompanyId = System.Configuration.ConfigurationManager.AppSettings["EMSecCompanyId"];//FORIC



		public EmsResponseModel EmsXmlGenerate(EmsRequestModel emsRequest) 
		{

   //         var kk = new EmsRequestModel();

			//kk.LogisticsOrderNo = "IVNT20211222000004";

			//kk.Sender = new EmsSender();
			//kk.Sender.Name = "撒库拉";
			//kk.Sender.PostCode = "";
			//kk.Sender.Phone = "";
			//kk.Sender.Mobile = "13646211008";
			//kk.Sender.Prov = "广东";
			//kk.Sender.City = "深圳";
			//kk.Sender.County = "龙岗区";
			//kk.Sender.Address = "吉华路应大风科技园";

			//kk.Receiver = new EmsSender();
			//kk.Receiver.Name = "方平";
			//kk.Receiver.PostCode = "";
			//kk.Receiver.Phone = "";
			//kk.Receiver.Mobile = "13646211012";
			//kk.Receiver.Prov = "广东";
			//kk.Receiver.City = "深圳";
			//kk.Receiver.County = "福田区";
			//kk.Receiver.Address = "佳和市场三楼3C013室";

			//kk.Cargos = new Cargos();
			//kk.Cargos.Cargo = new List<Cargo>();
			//kk.Cargos.Cargo.Add(new Cargo()
			//{
			//	CargoName = "文件票据"
			//});


			//转换Xml
			var xmldoc = new System.Xml.XmlDocument();
            xmldoc.LoadXml(emsRequest.Xml());
            //xmldoc.Save("D:/a.txt");

			var xmlString = ConvertXmlToString(xmldoc);
			xmlString = xmlString.Substring(xmlString.IndexOf("<OrderNormal>"));

            //调用请求
            var result = orderTracesSubByJson(xmlString);

			var response = XmlSerializerExtend.XmlTo<EmsResponseModel>(result);

			return response;

        }


		/// <summary>
		/// Json方式  电子面单
		/// </summary>
		/// <returns></returns>
		public string orderTracesSubByJson(string requestData)
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
        private string sendPost(string url, Dictionary<string, string> param)
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

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }


		public string ConvertXmlToString(XmlDocument xmlDoc)
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

		public Dictionary<string, string> HandleAddress(string Address)
		{
			var Province = "";
			var City = "";
			var Area = "";
			var DetailsAddress = "";
			if (Address.Split(' ').Length == 3)
			{
				Province = Address.Split(' ')[0].Trim();
				City = Address.Split(' ')[0].Trim() + "市";
				Area = Address.Split(' ')[1].Trim();
				DetailsAddress = Address.Split(' ')[2].Trim();
			}
			else
			{
				Province = Address.Split(' ')[0].Trim();
				if (Province == "内蒙古" || Province == "西藏")
					Province = Address.Split(' ')[0] + "自治区";
				if (Province == "新疆")
					Province = Address.Split(' ')[0] + "维吾尔自治区";
				if (Province == "广西")
					Province = Address.Split(' ')[0] + "壮族自治区";
				if (Province == "宁夏")
					Province = Address.Split(' ')[0] + "回族自治区";
				else
				{
					Province = Address.Split(' ')[0] + "省";
				}
				City = Address.Split(' ')[1].Trim();
				Area = Address.Split(' ')[2].Trim();
				DetailsAddress = Address.Split(' ')[3].Trim();
			}
			var DicAddres = new Dictionary<string, string>();
			DicAddres.Add("Province", Province);
			DicAddres.Add("City", City);
			DicAddres.Add("Area", Area);
			DicAddres.Add("DetailsAddress", DetailsAddress);
			return DicAddres;
		}


	}

	#region Ems 签名制作


	/// <summary>
	/// Md5Base64 签名制作
	/// </summary>
	public class Md5Base64
	{
		public static string encode(String str)
		{
			Base64Encoder base64 = new Base64Encoder();
			var ss = (byte[])MD5Ems.md5(str, true, Encoding.UTF8);
			return base64.GetEncoded((byte[])MD5Ems.md5(str, true, Encoding.UTF8));

		}

	}
	public class MD5Ems
	{
		// 格式化md5 hash 字节数组所用的格式（两位小写16进制数字） 
		private static readonly string m_strHexFormat = "x2";
		private MD5Ems() { }
		/// <summary> 
		/// 使用当前缺省的字符编码对字符串进行加密 
		/// </summary> 
		/// <param name="str">需要进行md5演算的字符串</param> 
		/// <returns>用小写字母表示的32位16进制数字字符串</returns> 
		public static string md5(string str)
		{
			return (string)md5(str, false, Encoding.Default);
		}
		/// <summary> 
		/// 对字符串进行md5 hash计算 
		/// </summary> 
		/// <param name="str">需要进行md5演算的字符串</param> 
		/// <param name="raw_output"> 
		/// false则返回经过格式化的加密字符串(等同于 string md5(string) ) 
		/// true则返回原始的md5 hash 长度16 的 byte[] 数组 
		/// </param> 
		/// <returns> 
		/// byte[] 数组或者经过格式化的 string 字符串 
		/// </returns> 
		public static object md5(string str, bool raw_output)
		{
			return md5(str, raw_output, Encoding.Default);
		}
		/// <summary> 
		/// 对字符串进行md5 hash计算 
		/// </summary> 
		/// <param name="str">需要进行md5演算的字符串</param> 
		/// <param name="raw_output"> 
		/// false则返回经过格式化的加密字符串(等同于 string md5(string) ) 
		/// true则返回原始的md5 hash 长度16 的 byte[] 数组 
		/// </param> 
		/// <param name="charEncoder"> 
		/// 用来指定对输入字符串进行编解码的 Encoding 类型， 
		/// 当输入字符串中包含多字节文字（比如中文）的时候 
		/// 必须保证进行匹配的 md5 hash 所使用的字符编码相同， 
		/// 否则计算出来的 md5 将不匹配。 
		/// </param> 
		/// <returns> 
		/// byte[] 数组或者经过格式化的 string 字符串 
		/// </returns> 
		public static object md5(string str, bool raw_output,
													Encoding charEncoder)
		{
			if (!raw_output)
				return md5str(str, charEncoder);
			else
				return md5raw(str, charEncoder);
		}

		/// <summary> 
		/// 使用当前缺省的字符编码对字符串进行加密 
		/// </summary> 
		/// <param name="str">需要进行md5演算的字符串</param> 
		/// <returns>用小写字母表示的32位16进制数字字符串</returns> 
		protected static string md5str(string str)
		{
			return md5str(str, Encoding.Default);
		}
		/// <summary> 
		/// 对字符串进行md5加密 
		/// </summary> 
		/// <param name="str">需要进行md5演算的字符串</param> 
		/// <param name="charEncoder"> 
		/// 指定对输入字符串进行编解码的 Encoding 类型 
		/// </param> 
		/// <returns>用小写字母表示的32位16进制数字字符串</returns> 
		protected static string md5str(string str, Encoding charEncoder)
		{
			byte[] bytesOfStr = md5raw(str, charEncoder);
			int bLen = bytesOfStr.Length;
			StringBuilder pwdBuilder = new StringBuilder(32);
			for (int i = 0; i < bLen; i++)
			{
				pwdBuilder.Append(bytesOfStr[i].ToString(m_strHexFormat));
			}
			return pwdBuilder.ToString();
		}
		/// <summary> 
		/// 使用当前缺省的字符编码对字符串进行加密 
		/// </summary> 
		/// <param name="str">需要进行md5演算的字符串</param> 
		/// <returns>长度16 的 byte[] 数组</returns> 
		protected static byte[] md5raw(string str)
		{
			return md5raw(str, Encoding.Default);
		}
		/// <summary> 
		/// 对字符串进行md5加密 
		/// </summary> 
		/// <param name="str">需要进行md5演算的字符串</param> 
		/// <param name="charEncoder"> 
		/// 指定对输入字符串进行编解码的 Encoding 类型 
		/// </param> 
		/// <returns>长度16 的 byte[] 数组</returns> 
		protected static byte[] md5raw(string str, Encoding charEncoder)
		{
			System.Security.Cryptography.MD5 md5 =
				System.Security.Cryptography.MD5.Create();
			return md5.ComputeHash(charEncoder.GetBytes(str));
		}
	}

	/// <summary>
	/// Base64编码类。
	/// 将byte[]类型转换成Base64编码的string类型。
	/// </summary>
	public class Base64Encoder
	{
		byte[] source;
		int length, length2;
		int blockCount;
		int paddingCount;
		public static Base64Encoder Encoder = new Base64Encoder();

		public Base64Encoder()
		{
		}

		private void init(byte[] input)
		{
			source = input;
			length = input.Length;
			if ((length % 3) == 0)
			{
				paddingCount = 0;
				blockCount = length / 3;
			}
			else
			{
				paddingCount = 3 - (length % 3);
				blockCount = (length + paddingCount) / 3;
			}
			length2 = length + paddingCount;
		}

		public string GetEncoded(byte[] input)
		{
			//初始化
			init(input);

			byte[] source2;
			source2 = new byte[length2];

			for (int x = 0; x < length2; x++)
			{
				if (x < length)
				{
					source2[x] = source[x];
				}
				else
				{
					source2[x] = 0;
				}
			}

			byte b1, b2, b3;
			byte temp, temp1, temp2, temp3, temp4;
			byte[] buffer = new byte[blockCount * 4];
			char[] result = new char[blockCount * 4];
			for (int x = 0; x < blockCount; x++)
			{
				b1 = source2[x * 3];
				b2 = source2[x * 3 + 1];
				b3 = source2[x * 3 + 2];

				temp1 = (byte)((b1 & 252) >> 2);

				temp = (byte)((b1 & 3) << 4);
				temp2 = (byte)((b2 & 240) >> 4);
				temp2 += temp;

				temp = (byte)((b2 & 15) << 2);
				temp3 = (byte)((b3 & 192) >> 6);
				temp3 += temp;

				temp4 = (byte)(b3 & 63);

				buffer[x * 4] = temp1;
				buffer[x * 4 + 1] = temp2;
				buffer[x * 4 + 2] = temp3;
				buffer[x * 4 + 3] = temp4;

			}

			for (int x = 0; x < blockCount * 4; x++)
			{
				result[x] = sixbit2char(buffer[x]);
			}


			switch (paddingCount)
			{
				case 0: break;
				case 1: result[blockCount * 4 - 1] = '='; break;
				case 2:
					result[blockCount * 4 - 1] = '=';
					result[blockCount * 4 - 2] = '=';
					break;
				default: break;
			}
			return new string(result);
		}
		private char sixbit2char(byte b)
		{
			char[] lookupTable = new char[64]{
				  'A','B','C','D','E','F','G','H','I','J','K','L','M',
				 'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
				 'a','b','c','d','e','f','g','h','i','j','k','l','m',
				 'n','o','p','q','r','s','t','u','v','w','x','y','z',
				 '0','1','2','3','4','5','6','7','8','9','+','/'};

			if ((b >= 0) && (b <= 63))
			{
				return lookupTable[(int)b];
			}
			else
			{

				return ' ';
			}
		}
	}

	#endregion
}