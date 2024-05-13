using Kdn.Library;
using Kdn.Library.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using WinApp.Printers;
using WinApp.Services;
using Yahv.PsWms.PvRoute.Services;
using Yahv.Utils.Extends;
using Yahv.Utils.Kdn;
using Yahv.Utils.Serializers;

namespace WinApp.Services
{
    /// <summary>
    /// 模版打印参数
    /// </summary>
    public class TemplatePrintParameter
    {
        public PrinterConfig Setting { get; set; }

        public object[] Data { get; set; }
    }

    /// <summary>
    /// 照片关系
    /// </summary>
    public class PhotoMap
    {
        public class MyData
        {
            //public string WaybillID { get; set; }
            //public string WsOrderID { get; set; }
            //public string NoticeID { get; set; }
            //public string InputID { get; set; }
            //public string PayID { get; set; }

            ///// <summary>
            ///// 暂时用于lotnumber
            ///// </summary>
            //public string ShipID { get; set; }
            //public int Type { get; set; }

            public string docId { get; set; }
            public string docType { get; set; }
            public string businessType { get; set; }
        }

        //public string SessionID { get; set; }
        //public string AdminID { get; set; }


        /// <summary>
        /// 最后把类型改为object
        /// </summary>
        public MyData Data { get; set; }

    }

    /// <summary>
    /// 照片关系(用于运单批量上传图片)
    /// </summary>
    public class PhotoMaps
    {
        public PhotoMap[] PhotoMapes { get; set; }

    }

    public class PictureUrl
    {
        public string Url { get; set; }
    }

    /// <summary>
    /// 拍照结果
    /// </summary>
    public class PhotoResult
    {
        public string SessionID { get; set; }
        public string FileID { get; set; }
        public string Url { get; set; }
        public string FileName { get; set; }
    }

    public class BasePrint
    {
        /// <summary>
        /// 使用的打印机名称
        /// </summary>
        public string PrinterName { get; set; }

        string _shipperCode;
        /// <summary>
        /// 承运商编码
        /// </summary>
        public string ShipperCode
        {
            get
            {
                return _shipperCode;
            }
            set
            {
                if (value.Contains("顺丰"))
                {
                    value = Kdn.Library.ShipperCode.SF;
                }
                else if (value.Contains("跨越速运"))
                {
                    value = Kdn.Library.ShipperCode.KYSY;
                }
                else
                {
                    throw new NotSupportedException("不支持该面单的打印！！");
                }

                _shipperCode = value;
            }
        }

        /// <summary>
        /// 快递类型
        /// </summary>
        public int ExpType { get; set; }




        public Sender Sender { get; set; }

        public Receiver Receiver { get; set; }

        public int Quantity { get; set; }

        public string Remark { get; set; }

        public double? Volume { get; set; }
        public double? Weight { get; set; }

        public Commodity[] Commodity { get; set; }
    }

    /// <summary>
    /// 快递鸟打印参数
    /// </summary>
    public class KdnPrint : BasePrint
    {
        int exPayType;

        /// <summary>
        /// 快递支付类型
        /// </summary>
        public int ExPayType
        {
            get
            {
                return this.exPayType;
            }
            set
            {
                if (Enum.GetValues(typeof(PayType)).Cast<int>().Contains(value))
                {
                    this.exPayType = value;
                }
                else
                {
                    throw new NotSupportedException($"不支持该面单:支付类型{value}的打印！！");
                }
            }
        }
    }

    /// <summary>
    /// 顺丰/跨越打印所需参数
    /// </summary>
    public class NewPrint : BasePrint
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }
        int exPayType;

        /// <summary>
        /// 快递类型
        /// </summary>
        public int ExPayType
        {
            get
            {
                return this.exPayType;
            }
            set
            {
                if (Enum.GetValues(typeof(PayMethod)).Cast<int>().Contains(value))
                {
                    this.exPayType = value;
                }
                else
                {
                    throw new NotSupportedException($"不支持该面单:支付类型{value}的打印！！");
                }
            }
        }
    }


    public class FacePrint
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        int exPayType;

        /// <summary>
        /// 支付类型
        /// </summary>
        public int ExPayType
        {
            get
            {
                return this.exPayType;
            }
            set
            {
                if (Enum.GetValues(typeof(SFPayType)).Cast<int>().Contains(value) || Enum.GetValues(typeof(KYPayType)).Cast<int>().Contains(value))
                {
                    this.exPayType = value;
                }
                else
                {
                    throw new NotSupportedException($"不支持该面单:支付类型{value}的打印！！");
                }
            }
        }

        /// <summary>
        /// 使用的打印机名称
        /// </summary>
        public string PrinterName { get; set; }

        string _shipperCode;
        /// <summary>
        /// 承运商编码
        /// </summary>
        public string ShipperCode
        {
            get
            {
                return _shipperCode;
            }
            set
            {
                if (value.Contains("顺丰"))
                {
                    value = Yahv.PsWms.PvRoute.Services.ShipperCode.SF;
                }
                else if (value.Contains("跨越速运"))
                {
                    value = Yahv.PsWms.PvRoute.Services.ShipperCode.KY;
                }
                //增加EMS
                else if (value.Contains("EMS"))
                {
                    value = Yahv.PsWms.PvRoute.Services.ShipperCode.EMS;
                }
                else
                {
                    throw new NotSupportedException("不支持该面单的打印！！");
                }

                _shipperCode = value;
            }
        }

        /// <summary>
        /// 从外部传入的月结账号（目前第三方付的支付方式需要传月结账号，不排除未来月结的时候传入）
        /// </summary>
        public string MonthlyCard { get; set; }


        //{
        //    get
        //    {
        //        return _shipperCode;
        //    }
        //    set
        //    {
        //        if (value.Contains("顺丰"))
        //        {
        //            value = Yahv.PsWms.PvRoute.Services.ShipperCode.SF;
        //        }
        //        else if (value.Contains("跨越速运"))
        //        {
        //            value = Yahv.PsWms.PvRoute.Services.ShipperCode.KYSY;
        //        }
        //        else
        //        {
        //            throw new NotSupportedException("不支持该面单的打印！！");
        //        }

        //        _shipperCode = value;
        //    }
        //}

        /// <summary>
        /// 快递类型
        /// </summary>
        public int ExpType { get; set; }

        public Yahv.PsWms.PvRoute.Services.Models.Sender Sender { get; set; }

        public Yahv.PsWms.PvRoute.Services.Models.Receiver Receiver { get; set; }

        public int Quantity { get; set; }

        public string Remark { get; set; }

        public double? Volume { get; set; }
        public double? Weight { get; set; }

        /// <summary>
        /// 是否打印签回单，默认false
        /// </summary>
        public bool IsSignBack { get; set; }

        /// <summary>
        /// 货物信息
        /// </summary>
        public Yahv.PsWms.PvRoute.Services.Models.Commodity[] Commodity { get; set; }
    }

    /// <summary>
    /// 国际快递针式打印所需参数
    /// </summary>
    public class InternationalAirWaybillPrint : BasePrint
    {
        /// <summary>
        /// 快递类型
        /// </summary>
        public string expType { get; set; }
        /// <summary>
        /// 托运人的账号
        /// </summary>
        public string ShipperAccountNo { get; set; }
        /// <summary>
        /// 海关托运人身份证号
        /// </summary>
        public string ShipperCustomNo { get; set; }
        /// <summary>
        /// 发件人电话
        /// </summary>
        public string SenderTel { get; set; }
        /// <summary>
        /// 发件人姓名
        /// </summary>
        public string SenderName { get; set; }
        /// <summary>
        /// 发件人公司
        /// </summary>
        public string SenderCompany { get; set; }
        /// <summary>
        /// 发件人地址
        /// </summary>
        public string SenderAddress { get; set; }
        /// <summary>
        /// 发件人邮编
        /// </summary>
        public string SenderPostalCode { get; set; }
        /// <summary>
        /// 发件人国家
        /// </summary>
        public string SenderCountry { get; set; }

        /// <summary>
        /// 收件人的账号
        /// </summary>
        public string ReceiverAccountNo { get; set; }
        /// <summary>
        /// 海关收件人身份证号
        /// </summary>
        public string ReceiverCustomNo { get; set; }
        /// <summary>
        /// 联系人电话
        /// </summary>
        public string TelOfContactPerson { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string NameOfContactPerson { get; set; }
        /// <summary>
        /// 收件人公司
        /// </summary>
        public string ReceiverCompany { get; set; }
        /// <summary>
        /// 收件人地址
        /// </summary>
        public string ReceiverAddress { get; set; }
        /// <summary>
        /// 收件人邮编
        /// </summary>
        public string ReceiverPostalCode { get; set; }
        /// <summary>
        ///收件人国家
        /// </summary>
        public string ReceiverCountry { get; set; }


    }
    /// <summary>
    /// 获取服务器的面单接结果
    /// </summary>
    public class FaceSheetResult
    {
        public string ID { get; set; }
        public string LogisticCode { get; set; }
        public string ShipperCode { get; set; }

        public string Html { get; set; }
    }

    /// <summary>
    /// 获取新的顺丰/跨越面单结果
    /// </summary>
    public class FaceOrderResult
    {
        public string ID { get; set; }
        public string SendJson { get; set; }
        public string MainID { get; set; }
        public string MyID { get; set; }
        public string Html { get; set; }
        public int Source { get; set; }
    }
}
