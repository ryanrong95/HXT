using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PsWms.PvRoute.Services;
using Yahv.PsWms.PvRoute.Services.Models;

namespace Yahv.PsWms.DappForm.Services
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
            public string MainID { get; set; }
          
            public int Type { get; set; }
        }
       
        /// <summary>
        /// 网站上传人
        /// </summary>
        public string SiteuserID { get; set; }

        /// <summary>
        /// 上传人
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 供乔霞使用帮助定位上传和拍照的位置
        /// </summary>
        public string SessionID { get; set; }

        

        /// <summary>
        /// 最后把类型改为object
        /// </summary>
        public MyData Data { get; set; }

    }

    /// <summary>
    /// 照片关系(用于运单批量上传图片)
    /// </summary>

    public class PictureUrl
    {
        public string Url { get; set; }
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
                if (Enum.GetValues(typeof(SFPayType)).Cast<int>().Contains(value)|| Enum.GetValues(typeof(KYPayType)).Cast<int>().Contains(value))
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

        //string _shipperCode;
        /// <summary>
        /// 承运商编码
        /// </summary>
        public string ShipperCode { get; set; }

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

        public Sender Sender { get; set; }

        public Receiver Receiver { get; set; }

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
        public Commodity[] Commodity { get; set; }
    }

    /// <summary>
    /// 获取顺丰/跨越面单结果
    /// </summary>
    public class FaceOrderResult
    {
        public string ID { get; set; }
        public string SendJson { get; set; }
        public string MainID { get; set; }
        public string Html { get; set; }
        public int Source { get; set; }
    }
}

