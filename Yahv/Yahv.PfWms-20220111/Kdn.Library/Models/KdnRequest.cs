using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kdn.Library.Models
{
    public class KdnRequest
    {
        internal string ReqURL;

        /// <summary>
        /// 编号：与快递单号一一对应
        /// </summary>
        public string OrderCode { get; set; }


        string shipperCode;

        /// <summary>
        /// 快递公司编号缩写：SF(顺丰)，KYSY（跨域）
        /// </summary>
        public string ShipperCode
        {
            get { return this.shipperCode; }
            set
            {
                this.shipperCode = value;
                switch (value)
                {
                    case Kdn.Library.ShipperCode.SF:
                        this.CustomerName = "13646211002";
                        this.CustomerPwd = "rong3222518";
                        //this.MonthCode = "7550205279";
                        //this.MonthCode = "7550123478";//2020.10.12 顺丰月结账号的更改
                        this.MonthCode = "7550205279";//2020.12.04 顺丰月结账号的更改（荣检重新确定：芯达通使用子账号）
                        //this.ReqURL = "https://api.kdniao.com/api/Eorderservice";

                        //this.ReqURL = "http://testapi.kdniao.com:8081/api/EOrderService";

                        break;
                    case Kdn.Library.ShipperCode.KYSY:
                        //this.CustomerName = "075517225569";
                        //this.CustomerPwd = "50489100";
                        //this.MonthCode = "075517225569";

                        this.CustomerName = "075568610585";//2021.4.1日启用
                        this.CustomerPwd = "90117189";
                        this.MonthCode = "075568610585";




                        //this.CustomerName = "testkysy";
                        //this.CustomerPwd = "";


                        //this.ReqURL = "http://testapi.kdniao.com:8081/api/EOrderService";

                        //this.ReqURL = "http://sandboxapi.kdniao.com:8080/kdniaosandbox/gateway/exterfaceInvoke.json";

                        break;
                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// 客户名称、客户密码、月结账号，正式环境需要填写，见表ScCustoms.dbo.ExpressCompanys
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 用户名密码
        /// </summary>
        public string CustomerPwd { get; set; }

        /// <summary>
        /// 月结帐号
        /// </summary>
        public string MonthCode { get; set; }

        /// <summary>
        /// 支付方式：寄付、到付、月结
        /// </summary>
        public PayType PayType { get; set; }

        /// <summary>
        /// 快递方式：见表ScCustoms.dbo.ExpressTypes
        /// </summary>
        public int ExpType { get; set; }

        /// <summary>
        /// 运费：默认填0
        /// </summary>
        public double Cost { get; set; }
        /// <summary>
        /// 其它费用：默认填0
        /// </summary>
        public double OtherCost { get; set; }

        /// <summary>
        /// 发货人
        /// </summary>
        public Sender Sender { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        public Receiver Receiver { get; set; }

        ///// <summary>
        ///// 是否通知快递员上门揽件 0-通知，1-不通知，不填则默认为1
        ///// </summary>
        //public int IsNotice { get; set; }

        ///// <summary>
        ///// 上门揽件时间段，格式：YYYY MM DD HH24:MM:SS
        ///// </summary>
        //public string StartDate { get; set; }

        ///// <summary>
        ///// 上门揽件时间段，格式：YYYY MM DD HH24:MM:SS
        ///// </summary>
        //public string EndDate { get; set; }

        /// <summary>
        /// 商品，一般只要填一个描述，例如：“发票”、“电子元器件”等
        /// </summary>
        public Commodity[] Commodity { get; set; }

        public double? Weight { get; set; }

        /// <summary>
        /// 快递件数：顺丰按照实际件数填写、跨域总是填1
        /// </summary>
        public int Quantity { get; set; }

        public double? Volume { get; set; }

        /// <summary>
        /// 备注：都是填:"小心轻放"
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 面单规格：都是填:"210",
        /// 顺丰更改为21001（20200430日确定）
        /// </summary>
        public string TemplateSize { get; set; }
        /// <summary>
        /// 都是填:"1"
        /// </summary>
        public string IsReturnPrintTemplate { get; set; }

        public PackingType PackingType { get; set; }

        public DeliveryMethod DeliveryMethod { get; set; }

        ///// <summary>
        ///// 是否要求签回单
        ///// </summary>
        //public int IsReturnSignBill { get; set; }

        public KdnRequest()
        {
            //不通知
            //this.IsNotice = 1;

            //快递鸟顺丰：21001
            //快递鸟跨域速运：210
            //this.TemplateSize = "210";
            this.DeliveryMethod = DeliveryMethod.Floor;
        }

        //public Receiver GetReceiver(string Address)
        //{
        //    var address = new Dictionary<string, string>();
        //    address = this.HandleAddress(Address);
        //    Receiver receiver = new Receiver();
        //    receiver.ProvinceName = address["Province"];
        //    receiver.CityName = address["City"];
        //    receiver.ExpAreaName = address["Area"];
        //    receiver.Address = address["DetailsAddress"];
        //    return receiver;
        //}

        //public Sender GetSender(string Address)
        //{
        //    var address = new Dictionary<string, string>();
        //    address = this.HandleAddress(Address);
        //    Sender sender = new Sender();
        //    sender.ProvinceName = address["Province"];
        //    sender.CityName = address["City"];
        //    sender.ExpAreaName = address["Area"];
        //    sender.Address = address["DetailsAddress"];
        //    return sender;
        //}

        ///*
        //发货前需要：
        //填写件数(需要填写)
        //选择快递类型(需要选择)
        //快递公司(需要选择)

        //程序需要开发
        //程序返回来填写承运商与快递单号
        //*/

        //public void kkk()
        //{
        //    string regex_text = "((?<province>[^省]+省|.+自治区)|上海|北京|天津|重庆)(?<city>[^市]+市|.+自治州)(?<county>[^县]+县|.+区|.+镇|.+局)?(?<town>[^区]+区|.+镇)?(?<village>.*)";
        //    Regex regex = new Regex(regex_text);
        //    var address = "江苏省苏州市工业园区某陆谋好7号楼某方";

        //    var china = pccAreas.Current["中国"];
        //    var province = china.s.Where(item => address.StartsWith(item.n)).SingleOrDefault();
        //    var _city = china.n.Substring(2);
        //    var city = province.s.Where(item => _city.StartsWith(item.n));

        //}

        //private Dictionary<string, string> HandleAddress(string Address)
        //{
        //    var Province = "";
        //    var City = "";
        //    var Area = "";
        //    var DetailsAddress = "";
        //    if (Address.Split(' ').Length == 3)
        //    {
        //        Province = Address.Split(' ')[0].Trim();
        //        City = Address.Split(' ')[0].Trim() + "市";
        //        Area = Address.Split(' ')[1].Trim();
        //        DetailsAddress = Address.Split(' ')[2].Trim();
        //    }
        //    else
        //    {
        //        Province = Address.Split(' ')[0].Trim();
        //        if (Province == "内蒙古" || Province == "西藏")
        //            Province = Address.Split(' ')[0] + "自治区";
        //        if (Province == "新疆")
        //            Province = Address.Split(' ')[0] + "维吾尔自治区";
        //        if (Province == "广西")
        //            Province = Address.Split(' ')[0] + "壮族自治区";
        //        if (Province == "宁夏")
        //            Province = Address.Split(' ')[0] + "回族自治区";
        //        else
        //        {
        //            Province = Address.Split(' ')[0] + "省";
        //        }
        //        City = Address.Split(' ')[1].Trim();
        //        Area = Address.Split(' ')[2].Trim();
        //        DetailsAddress = Address.Split(' ')[3].Trim();
        //    }
        //    var DicAddres = new Dictionary<string, string>();
        //    DicAddres.Add("Province", Province);
        //    DicAddres.Add("City", City);
        //    DicAddres.Add("Area", Area);
        //    DicAddres.Add("DetailsAddress", DetailsAddress);
        //    return DicAddres;
        //}
    }
}
