using Needs.Ccs.Services.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class SFWaybillPrinter
    {
        //public event SuccessEventHandler Success;

        #region 打印

        //向服务传递参数

        /// <summary>
        /// 发送请求并获得结果
        /// </summary>
        /// <param name="reqURL"></param>
        /// <param name="jsonParm"></param>
        /// <returns></returns>
        private static string postJson(string reqURL, string jsonParm)
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


                if (httpResult.Contains("["))
                {
                    httpResult = httpResult.Substring(httpResult.IndexOf("[") + 1, httpResult.IndexOf("]") - httpResult.IndexOf("[") - 1);
                }

                if (httpResult.StartsWith("\""))
                {

                    httpResult = httpResult.Substring(1, httpResult.Length - 1);
                }
                if (httpResult.EndsWith("\""))
                {
                    httpResult = httpResult.Substring(0, httpResult.Length - 1);
                }

                // 将换行全部替换成空
                httpResult = httpResult.Replace("\\n", "");

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return httpResult;
        }

        //将图片文件写入本地
        public static Boolean generateImage(string imgStr, string imgFilePath)
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

                //FileStream write = File.OpenWrite(imgFilePath);

                using (FileStream fs = new FileStream(imgFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(bytes, 0, bytes.Length);
                    //fs.Flush();

                    fs.Close();
                }


            }
            catch (Exception e)
            {
                return false;
            }
            return true;

        }

        //本地文件转换64位
        public static string getResult(string filePath)
        {
            StreamReader sr = new StreamReader(filePath, Encoding.Default, true);
            int index;
            //实例化一个内存流
            System.IO.MemoryStream tempStream = new MemoryStream();
            //将流转换为字节数组
            while ((index = sr.BaseStream.ReadByte()) != -1)
            {
                tempStream.WriteByte(((byte)index));
            }
            byte[] array = tempStream.ToArray();
            tempStream.Close();
            //将得到的字节数组转换为base64位编码
            string result = Convert.ToBase64String(array);
            return result;
        }

        #endregion

    }

    public class CargoInfoDto
    {

        public string cargo;
        public int parcelQuantity;
        public long cargoCount;
        public string cargoUnit;
        public double cargoWeight;
        public double cargoAmount;
        public double cargoTotalWeight;
        public string remark;
        public string sku;

    }
    public class RlsInfoDto
    {
        public string abFlag;
        public string codingMapping;
        public string codingMappingOut;
        public string destRouteLabel;
        public string destTeamCode;
        public string printIcon;
        public string proCode;
        public string qrcode;
        public string sourceTransferCode;
        public string waybillNo;
        public string xbFlag;

    }
    public class WaybillDto
    {

        public string mailNo;
        public string isPrintLogo;
        public int expressType;
        public int payMethod;
        public string returnTrackingNo;//回签单号
        public string monthAccount;
        public string orderNo;
        public string zipCode;
        public string destCode;
        public string payArea;
        public string deliverCompany;
        public string deliverName;
        public string deliverMobile;
        public string deliverTel;
        public string deliverProvince;
        public string deliverCity;
        public string deliverCounty;
        public string deliverAddress;
        public string deliverShipperCode;
        public string consignerCompany;
        public string consignerName;
        public string consignerMobile;
        public string consignerTel;
        public string consignerProvince;
        public string consignerCity;
        public string consignerCounty;
        public string consignerAddress;
        public string consignerShipperCode;
        public string logo;
        public string sftelLogo;
        public string topLogo;
        public string topsftelLogo;
        public string appId;
        public string appKey;
        public string electric;

        public CargoInfoDto[] cargoInfoDtoList;
        public RlsInfoDto[] rlsInfoDtoList;
        public string insureValue;
        public string codValue;
        public string codMonthAccount;


        public string mainRemark;
        public string returnTrackingRemark;
        public string childRemark;
        public string custLogo;
        public string insureFee;

        public Boolean encryptCustName; //加密寄件人及收件人名称
        public Boolean encryptMobile; //加密寄件人及收件人联系手机

    }

    #region 顺丰请求类
    /// <summary>
    /// 顺丰订单，用于下订单接口
    /// </summary>
    public class SFOrder
    {

        /// <summary>
        /// 响应报文的语言，缺省值为zh-CN，目前支持以下值zh-CN表示中文简体，zh-TW或zh-HK或 zh-MO表示中文繁体，en表示英文，必填
        /// </summary>
        public string language { get; set; }

        /// <summary>
        /// 客户订单号，自定义，必填
        /// </summary>
        public string orderId { get; set; }

        /// <summary>
        /// 顺丰运单号信息，可空
        /// </summary>
        //public waybillNoInfo[] waybillNoInfoList { get; set; }
        public List<waybillNoInfo> waybillNoInfoList { get; set; }

        /// <summary>
        /// 报关信息，可空
        /// </summary>
        public CustomsInfo customsInfo { get; set; }

        /// <summary>
        /// 托寄物信息，必填
        /// </summary>
        public List<cargoDetail> cargoDetails { get; set; }
        //public cargoDetail[] cargoDetails { get; set; }

        /// <summary>
        /// 拖寄物类型描述，可空
        /// </summary>
        public string cargoDesc { get; set; }

        /// <summary>
        /// 扩展属性，可空
        /// </summary>
        //public extraInfo[] extraInfoList { get; set; }
        public List<extraInfo> extraInfoList { get; set; }

        /// <summary>
        /// 增值服务信息，可空
        /// </summary>
        //public service[] serviceList { get; set; }
        public List<service> serviceList { get; set; }

        /// <summary>
        /// 收寄双方信息，必填
        /// </summary>
        //public contactInfo[] contactInfoList { get; set; }
        public List<contactInfo> contactInfoList { get; set; }

        /// <summary>
        /// 顺丰月结卡号
        /// 月结支付时传值，现结不需传值
        /// </summary>
        public string monthlyCard { get; set; }


        int _payMethod;

        /// <summary>
        /// 付款方式，支持以下值：
        /// 1:寄方付
        /// 2:收方付
        /// 3:第三方付
        /// </summary>
        public int payMethod
        {
            get
            {
                return this._payMethod;
            }
            set
            {
                if (Enum.GetValues(typeof(PayType)).Cast<int>().Contains(value))
                {
                    this._payMethod = value;
                }
                else
                {
                    throw new NotSupportedException($"不支持该面单:支付类型{value}的打印！！");
                }
            }
        }


        /// <summary>
        /// 快件产品类别
        /// </summary>
        public int expressTypeId { get; set; }

        /// <summary>
        /// 包裹数，一个包裹对应一个运单号；若包裹数大于1，则返回一个母运单号和N-1个子运单号
        /// </summary>
        public int parcelQty { get; set; }

        /// <summary>
        /// 客户订单货物总长，单位厘米，精确到小数点后3位，包含子母件
        /// </summary>
        public double? totalLength { get; set; }

        /// <summary>
        /// 客户订单货物总宽，单位厘米，精确到小数点后3位，包含子母件
        /// </summary>
        public double? totalWidth { get; set; }

        /// <summary>
        /// 客户订单货物总高，单位厘米，精确到小数点后3位，包含子母件
        /// </summary>
        public double? totalHeight { get; set; }

        /// <summary>
        /// 客户订单货物总体积，单位厘米，精确到小数点后3位，会用于计抛(是否计抛具体商务沟通中双方约定)
        /// </summary>
        public double? volume { get; set; }

        /// <summary>
        /// 订单货物总重量，若为子母件必填，单位千克，精确到小数点后3位，如果提供此值，必须>0 (子母件需>6)
        /// </summary>
        public double? totalWeight { get; set; }

        /// <summary>
        /// 商品总净重
        /// </summary>
        public double? totalNetWeight { get; set; }

        /// <summary>
        /// 要求上门取件开始时间，
        /// 格式：
        /// YYYY-MM-DD HH24:MM:SS，
        /// 示例：
        /// 2012-7-30 09:30:00
        /// </summary>
        public DateTime sendStartTm { get; set; }

        /// <summary>
        /// 是否通过手持终端
        /// 通知顺丰收派员上门收件，支持以下值：1：要求 0：不要求
        /// </summary>
        public bool isDocall { get; set; }

        /// <summary>
        /// 是否返回签回单（签单返还）的运单号，支持以下值：1：要求 0：不要求
        /// </summary>
        public bool isSignBack { get; set; }

        /// <summary>
        /// 客户参考编码：如客户原始订单号
        /// </summary>
        public string custReferenceNo { get; set; }

        /// <summary>
        /// 温度范围类型，当express_type为12医药温控件时必填，支持以下值：1:冷藏 3：冷冻
        /// </summary>
        public int temperatureRange { get; set; }

        /// <summary>
        /// 订单平台类型（对于平台类客户，如果需要在订单中区分订单来源，则可使用此字段） 天猫:tmall，拼多多：pinduoduo，京东 : jd等平台类型编码
        /// </summary>
        public string orderSource { get; set; }

        /// <summary>
        /// 业务配置代码，业务配置代码指BSP针对客户业务需求配置的一套接口处理逻辑，一个接入编码可对应多个业务配置代码
        /// </summary>
        public string bizTemplateCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 快件自取，支持以下值：1：客户同意快件自取0：客户不同意快件自取
        /// </summary>
        public bool isOneselfPickup { get; set; }

        /// <summary>
        /// 筛单特殊字段，用来人工筛单
        /// </summary>
        public string filterField { get; set; }

        /// <summary>
        /// 是否返回用来退货业务的二维码URL，支持以下值：1：返回二维码 0：不返回二维码
        /// </summary>
        public bool isReturnQRCode { get; set; }

        /// <summary>
        /// 特殊派送类型代码 1:身份验证
        /// </summary>
        public string specialDeliveryTypeCode { get; set; }

        /// <summary>
        /// 特殊派件具体表述   证件类型:证件后8位如：1:09296231（1表示身份证，暂不支持其他证件）
        /// </summary>
        public string specialDeliveryValue { get; set; }

        /// <summary>
        /// 寄件实名认证流水号
        /// </summary>
        public string realnameNum { get; set; }

        /// <summary>
        /// 商户支付订单号
        /// </summary>
        public string merchantPayOrderNo { get; set; }

        /// <summary>
        /// 是否返回签回单路由标签：默认0，1：返回路由标签，0：不返回
        /// </summary>
        public bool isReturnSignBackRoutelabel { get; set; }

        /// <summary>
        /// 是否返回路由标签：默认0，1：返回路由标签，0：不返回
        /// </summary>
        public bool isReturnSignBackRoute { get; set; }

        /// <summary>
        /// 	是否使用国家统一面单号 1：是， 0：否（默认）
        /// </summary>
        public bool isUnifiedWaybillNo { get; set; }

        /// <summary>
        /// 签单返还范本地址
        /// </summary>
        public string podModelAddress { get; set; }

        /// <summary>
        /// 揽收员工号
        /// </summary>
        public string collectEmpCode { get; set; }

        /// <summary>
        /// 头程运单号
        /// </summary>
        public string inProcessWaybillNo { get; set; }
    }

    /// <summary>
    /// 顺丰运单号
    /// </summary>
    public class waybillNoInfo
    {

        /// <summary>
        /// 运单号类型1：母单 2 :子单 3 : 签回单
        /// </summary>
        public int? waybillType { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        public string waybillNo { get; set; }
    }

    /// <summary>
    /// 报关信息
    /// </summary>
    public class CustomsInfo
    {

        /// <summary>
        /// 客户订单货物总声明价值，包含子母件，精确到小数点后3位。如果是跨境件，则必填
        /// </summary>
        public decimal declaredValue { get; set; }

        /// <summary>
        /// 货物声明价值币别，跨境件报关需要填写参照附录币别代码附件。中国内地默认CNY，否则默认USD
        /// </summary>
        public string declaredValueCurrency { get; set; }

        /// <summary>
        /// 报关批次
        /// </summary>
        public string customsBatchs { get; set; }

        /// <summary>
        /// 税金付款方式，支持以下值：1:寄付 2：到付
        /// </summary>
        public int taxPayMethod { get; set; }

        /// <summary>
        /// 税金结算账号
        /// </summary>
        public string taxSettleAccounts { get; set; }

        /// <summary>
        /// 支付工具
        /// </summary>
        public string paymentTool { get; set; }

        /// <summary>
        /// 支付号码
        /// </summary>
        public string paymentNumber { get; set; }

        /// <summary>
        /// 客户订单下单人姓名
        /// </summary>
        public string orderName { get; set; }

        /// <summary>
        /// 客户订单下单人证件类型
        /// </summary>
        public string orderCertType { get; set; }

        /// <summary>
        /// 客户订单下单人证件号
        /// </summary>
        public string orderCertNo { get; set; }

        /// <summary>
        /// 税款
        /// </summary>
        public string tax { get; set; }

    }

    /// <summary>
    /// 托寄物信息
    /// </summary>
    public class cargoDetail
    {
        /// <summary>
        /// 货物名称，如果需要生成电子运单，则为必填
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 货物数量 跨境件报关需要填写
        /// </summary>
        public long count { get; set; }

        /// <summary>
        /// 	货物单位，如：个、台、本，跨境件报关需要填写
        /// </summary>
        public string unit { get; set; }

        /// <summary>
        /// 订单货物单位重量，包含子母件，单位千克，精确到小数点后3位，跨境件报关需要填写
        /// </summary>
        public double weight { get; set; }

        /// <summary>
        /// 货物单价，精确到小数点后3位，跨境件报关需要填写
        /// </summary>
        public double amount { get; set; }

        /// <summary>
        /// 货物单价的币别：参照附录币别代码附件
        /// </summary>
        public string currency { get; set; }

        /// <summary>
        /// 原产地国别，跨境件报关需要填写
        /// </summary>
        public string sourceArea { get; set; }

        /// <summary>
        /// 货物产品国检备案编号
        /// </summary>
        public string productRecordNo { get; set; }

        /// <summary>
        /// 商品海关备案号
        /// </summary>
        public string goodPrepardNo { get; set; }

        /// <summary>
        /// 商品行邮税号
        /// </summary>
        public string taxNo { get; set; }
        /// <summary>
        /// 海关编码
        /// </summary>
        public string hsCode { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string goodsCode { get; set; }

        /// <summary>
        /// 货物品牌
        /// </summary>
        public string brand { get; set; }

        /// <summary>
        /// 货物规格型号
        /// </summary>
        public string specifications { get; set; }

        /// <summary>
        /// 生产厂家
        /// </summary>
        public string manufacturer { get; set; }

        /// <summary>
        /// 托寄物毛重
        /// </summary>
        public double shipmentWeight { get; set; }

        /// <summary>
        /// 托寄物长
        /// </summary>
        public double length { get; set; }

        /// <summary>
        /// 托寄物宽
        /// </summary>
        public double width { get; set; }

        /// <summary>
        /// 托寄物高
        /// </summary>
        public double height { get; set; }

        /// <summary>
        /// 托寄物体积
        /// </summary>
        public double volume { get; set; }

        /// <summary>
        /// 托寄物声明价值
        /// </summary>
        public double cargoDeclaredValue { get; set; }

        /// <summary>
        /// 托寄物声明价值币别
        /// </summary>
        public string declaredValueDeclaredCurrency { get; set; }

        /// <summary>
        /// 货物id（逆向物流）
        /// </summary>
        public string cargoId { get; set; }

        /// <summary>
        /// 智能验货标识(1-是,0-否)（逆向物流）
        /// </summary>
        public bool intelligentInspection { get; set; }

        /// <summary>
        /// 货物标识码（逆向物流）
        /// </summary>
        public string snCode { get; set; }

        /// <summary>
        /// 国条码
        /// </summary>
        public string stateBarCode { get; set; }

    }

    /*
      增值服务传值说明举例：

         */

    /// <summary>
    /// 增值服务信息
    /// </summary>
    public class service
    {
        /// <summary>
        /// 增值服务名，如COD等
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 增值服务扩展属性，参考增值服务传值说明
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// 增值服务扩展属性
        /// </summary>
        public string value1 { get; set; }

        /// <summary>
        /// 增值服务扩展属性2
        /// </summary>
        public string value2 { get; set; }

        /// <summary>
        /// 增值服务扩展属性3
        /// </summary>
        public string value3 { get; set; }

        /// <summary>
        /// 增值服务扩展属性4
        /// </summary>
        public string value4 { get; set; }

    }

    /// <summary>
    /// 扩展属性
    /// </summary>
    public class extraInfo
    {
        /// <summary>
        /// 扩展字段说明：attrName为字段定义，
        /// </summary>
        public string attrName { get; set; }

        /// <summary>
        /// 扩展字段值
        /// </summary>
        public string attrVal { get; set; }
    }

    /// <summary>
    /// 收寄双方信息
    /// </summary>
    public class contactInfo
    {
        /// <summary>
        /// 地址类型：1，寄件方信息 2，到件方信息
        /// </summary>
        public int? contactType { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string company { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string contact { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string tel { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string mobile { get; set; }

        /// <summary>
        /// 城市代码或国家代码，如果是跨境件，则此字段为必填
        /// </summary>
        public string zoneCode { get; set; }

        /// <summary>
        /// 国家或地区 2位代码参照附录国家代码附件
        /// </summary>
        public string country { get; set; }

        /// <summary>
        /// 所在省级行政区名称，必须是标准的省级行政区名称如：北京、广东省、广西壮族自治区等；此字段影响原寄地代码识别，建议尽可能传该字段的值
        /// </summary>
        public string province { get; set; }

        /// <summary>
        /// 所在地级行政区名称，必须是标准的城市称谓 如：北京市、深圳市、大理白族自治州等；此字段影响原寄地代码识别，建议尽可能传该字段的值
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// 所在县/区级行政区名称，必须是标准的县/区称谓，如：福田区，南涧彝族自治县、准格尔旗等
        /// </summary>
        public string county { get; set; }

        /// <summary>
        /// 详细地址，若province/city字段的值不传，此字段必须包含省市信息，避免影响原寄地代码识别，如：广东省深圳市        福田区新洲十一街万基商务大厦10楼；若需要生成电子运单，则为必填
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// 邮编，跨境件必填（中国内地，港澳台互寄除外）
        /// </summary>
        public string postCode { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// 税号
        /// </summary>
        public string taxNo { get; set; }

    }

    #endregion

    #region 顺丰结果类

    /*
        A1000	统一接入平台校验成功，调用后端服务成功;
       注意：
       不代表后端业务处理成功，实际业务处理结果，
       需要查看响应属性apiResultData中的详细结果
       A1001   必传参数不可为空
       A1002   请求时效已过期
       A1003   IP无效
       A1004   无对应服务权限
       A1005   流量受控
       A1006   数字签名无效
       A1007   重复请求
       A1008   数据解密失败
       A1009   目标服务异常或不可达
       A1099   系统异常
            */

    public class SFResult
    {

        /// <summary>
        /// 错误信息
        /// </summary>
        public string apiErrorMsg { get; set; }

        /// <summary>
        /// 响应编号
        /// </summary>
        public string apiResponseID { get; set; }

        /// <summary>
        /// 返回编号
        /// </summary>
        public string apiResultCode { get; set; }

        /// <summary>
        /// 详细信息
        /// </summary>
        public apiResultData apiResultData { get; set; }

    }

    public class apiResultData
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// 错误编号
        /// </summary>
        public string errorCode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string errorMsg { get; set; }

        /// <summary>
        /// 详细信息
        /// </summary>
        public msgData msgData { get; set; }
    }

    public class msgData
    {
        /// <summary>
        /// 客户订单号，必返
        /// </summary>
        public string orderId { get; set; }

        /// <summary>
        /// 原寄地区域代码，可用于顺丰电子运单标签打印
        /// </summary>
        public string originCode { get; set; }

        /// <summary>
        /// 目的地区域代码，可用于顺丰电子运单标签打印
        /// </summary>
        public string destCode { get; set; }

        /// <summary>
        /// 筛单结果：
        /// 1：人工确认
        /// 2：可收派
        /// 3：不可以收派
        /// </summary>
        public int? filterResult { get; set; }

        /// <summary>
        /// 如果filter_result=3时为必填，
        /// 不可以收派的原因代码：
        /// 1：收方超范围
        /// 2：派方超范围
        /// 3：其它原因
        /// 高峰管控提示信息
        /// 【数字】：【高峰管控提示信息】
        /// (如 4：温馨提示 ，1：春运延时)
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 二维码URL（用于CX退货操作的URL）
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 用于第三方支付运费的URL
        /// </summary>
        public string paymentLink { get; set; }

        /// <summary>
        /// 是否送货上楼 1:是
        /// </summary>
        public bool? isUpstairs { get; set; }

        /// <summary>
        /// true 包含特殊仓库增值服务
        /// </summary>
        public bool? isSpecialWarehouseService { get; set; }

        /// <summary>
        /// 地图标记（顺丰结果类没有的属性）
        /// </summary>
        public string mappingMark { get; set; }

        /// <summary>
        /// 代理订单编号（顺丰结果类没有的属性）
        /// </summary>
        public string agentMailno { get; set; }

        /// <summary>
        /// 下单补充的增值服务信息
        /// </summary>
        public List<service> serviceList { get; set; }

        //extraInfo[] _returnExtraInfoList;

        /// <summary>
        /// 返回信息扩展属性
        /// </summary>
        public List<extraInfo> returnExtraInfoList { get; set; }
        //{
        //    //get
        //    //{
        //    //    return this._returnExtraInfoList;
        //    //}
        //    //set
        //    //{
        //    //    if (value == null)
        //    //    {
        //    //        _returnExtraInfoList = new extraInfo[] { };
        //    //    }
        //    //    else
        //    //    {
        //    //        _returnExtraInfoList = value;
        //    //    }
        //    //}
        //}

        /// <summary>
        /// 顺丰运单号
        /// </summary>
        public List<waybillNoInfo> waybillNoInfoList { get; set; }

        /// <summary>
        /// 路由标签
        /// </summary>
        public List<routeLabelInfo> routeLabelInfo { get; set; }

        //contactInfo[] _contactInfoList;
        public List<contactInfo> contactInfoList { get; set; }
        //{
        //    get
        //    {
        //        return this._contactInfoList;
        //    }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            _contactInfoList = new contactInfo[] { };
        //        }
        //        else
        //        {
        //            _contactInfoList = value;
        //        }
        //    }
        //}
    }

    /// <summary>
    /// 路由标签信息
    /// </summary>
    public class routeLabelInfo
    {
        /// <summary>
        /// 返回调用结果，1000：调用成功；其他调用失败
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 路由标签数据详细数据
        /// </summary>
        public routeLabelData routeLabelData { get; set; }

        /// <summary>
        /// 失败异常描述
        /// </summary>
        public string message { get; set; }

    }

    /// <summary>
    /// 路由标签数据
    /// </summary>
    public class routeLabelData
    {

        /// <summary>
        /// 运单号
        /// </summary>
        public string waybillNo { get; set; }

        /// <summary>
        /// 原寄地中转场
        /// </summary>
        public string sourceTransferCode { get; set; }

        /// <summary>
        /// 原寄地城市代码
        /// </summary>
        public string sourceCityCode { get; set; }

        /// <summary>
        /// 原寄地网点代码
        /// </summary>
        public string sourceDeptCode { get; set; }

        /// <summary>
        /// 原寄地单元区域
        /// </summary>
        public string sourceTeamCode { get; set; }

        /// <summary>
        /// 目的地城市代码,eg:755
        /// </summary>
        public string destCityCode { get; set; }

        /// <summary>
        /// 目的地网点代码,eg:755AQ
        /// </summary>
        public string destDeptCode { get; set; }

        /// <summary>
        /// 目的地网点代码映射码
        /// </summary>
        public string destDeptCodeMapping { get; set; }

        /// <summary>
        /// 目的地单元区域,eg:001
        /// </summary>
        public string destTeamCode { get; set; }

        /// <summary>
        /// 目的地单元区域映射码
        /// </summary>
        public string destTeamCodeMapping { get; set; }

        /// <summary>
        /// 目的地中转场
        /// </summary>
        public string destTransferCode { get; set; }

        /// <summary>
        ///打单时的路由标签信息如果是大网的路由标签,这里的值是目的地网点代码, 如果是同城配的路由标签,这里的值是根据同城配的设置映射出来的值,不同的配置结果会不一样,不能根据-符号切分(如:上海同城配, 可能是:集散点-目的地网点-接驳点, 也有可能是目的地网点代码-集散点-接驳点)
        /// </summary>
        public string destRouteLabel { get; set; }

        /// <summary>
        /// 产品名称，对应RLS:pro_name
        /// </summary>
        public string proName { get; set; }

        /// <summary>
        /// 快件内容:如:C816、SP601
        /// </summary>
        public string cargoTypeCode { get; set; }

        /// <summary>
        /// 时效代码, 如:T4
        /// </summary>
        public string limitTypeCode { get; set; }

        /// <summary>
        /// 产品类型,如:B1
        /// </summary>
        public string expressTypeCode { get; set; }

        /// <summary>
        /// 入港映射码 eg:S10
        /// </summary>
        public string codingMapping { get; set; }

        /// <summary>
        /// 出港映射码
        /// </summary>
        public string codingMappingOut { get; set; }

        /// <summary>
        /// XB标志 0:不需要打印XB 1:需要打印XB
        /// </summary>
        public string xbFlag { get; set; }

        /// <summary>
        /// 打印标志：返回值总共有9位,每位只有0和1两种,0表示按丰密面单默认的规则,1是显示,顺序如下,如111110000表示打印寄方姓名、寄方电话、寄方公司名、寄方地址和重量, 收方姓名、收方电话、收方公司和收方地址按丰密面单默认规则1:寄方姓名 2:寄方电话 3:寄方公司名 4:寄方地址 5:重量 6:收方姓名 7:收方电话 8:收方公司名 9:收方地址
        /// </summary>
        public string printFlag { get; set; }

        /// <summary>
        /// 二维码根据规则生成字符串信息,格式为MMM={'k1':'(目的地中转场代码)','k2':'(目的地原始网点代码)','k3':'(目的地单元区域)','k4':'(附件通过三维码(express_type_code、 limit_type_code、 cargo_type_code)映射时效类型)','k5':'(运单号)','k6':'(AB标识)','k7':'(校验码)'}
        /// </summary>
        public string twoDimensionCode { get; set; }

        /// <summary>
        /// 时效类型:值为二维码中的K4
        /// </summary>
        public string proCode { get; set; }

        /// <summary>
        /// 打印图标,根据托寄物判断需要打印的图标(重货, 蟹类, 生鲜, 易碎，Z标)。返回值有8位，每一位只有0和1两种，0表示按运单默认的规则，1表示显示。后面两位默认0备用。顺序如下：重货,蟹类,生鲜,易碎,医药类,Z标,0,0如：00000000表示不需要打印重货，蟹类，生鲜，易碎,医药,Z标,备用,备用
        /// </summary>
        public string printIcon { get; set; }

        /// <summary>
        /// AB标
        /// </summary>
        public string abFlag { get; set; }

        /// <summary>
        /// 查询出现异常时返回信息。返回代码:0 系统异常 1 未找到面单
        /// </summary>
        public string errMsg { get; set; }

        /// <summary>
        /// 目的地口岸代码
        /// </summary>

        public string destPortCode { get; set; }

        /// <summary>
        /// 目的国别(国别代码如:JP)
        /// </summary>
        public string destCountry { get; set; }

        /// <summary>
        /// 目的地邮编
        /// </summary>
        public string destPostCode { get; set; }

        /// <summary>
        /// 总价值(保留两位小数,数字类型,可补位)
        /// </summary>
        public string goodsValueTotal { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string currencySymbol { get; set; }

        /// <summary>
        /// ？未知属性（顺丰结果类没有的属性）
        /// </summary>
        public string cusBatch { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public string goodsNumber { get; set; }

        /// <summary>
        /// ？未知属性（顺丰结果类没有的属性）翻译查询：checkCode支票代码
        /// </summary>
        public string checkCode { get; set; }

        /// <summary>
        ///  ？未知属性（顺丰结果类没有的属性）翻译查询：proIcon 省份图标
        /// </summary>
        public string proIcon { get; set; }

        /// <summary>
        /// ？未知属性（顺丰结果类没有的属性）翻译查询：fileIcon 文件图标
        /// </summary>
        public string fileIcon { get; set; }

        /// <summary>
        /// ？未知属性（顺丰结果类没有的属性）fba图标
        /// </summary>
        public string fbaIcon { get; set; }

        /// <summary>
        /// ？未知属性（顺丰结果类没有的属性）icsm图标
        /// </summary>
        public string icsmIcon { get; set; }

        /// <summary>
        /// ？未知属性（顺丰结果类没有的属性）
        /// </summary>
        public string destGisDeptCode { get; set; }

        /// <summary>
        /// ？未知属性（顺丰结果类没有的属性）新图标
        /// </summary>
        public string newIcon { get; set; }

    }

    #endregion
}
