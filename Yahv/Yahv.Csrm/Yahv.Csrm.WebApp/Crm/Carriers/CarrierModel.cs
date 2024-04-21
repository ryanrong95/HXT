using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;

namespace Yahv.Csrm.WebApp.Crm.Carriers
{
    public class Commons
    {
        readonly static public string UnifyApiUrl = ConfigurationManager.AppSettings["UnifyApiUrl"];
        /// <summary>
        /// 把CarrierType装成靳珊珊要的枚举值：100-国际快递,200-国际物流，300-国内快递,400-国内物流
        /// </summary>
        /// <param name="CarrierType"></param>
        /// <returns></returns>
        public static int ConvertType(CarrierType CarrierType, string Place)
        {
            int xdt = 100;
            switch (CarrierType)
            {
                case CarrierType.Logistics:
                    if (Place == Origin.CHN.GetOrigin().Code)
                    {
                        xdt = 400;
                    }
                    else
                    {
                        xdt = 200;
                    }
                    break;
                case CarrierType.Express:
                    if (Place == Origin.CHN.GetOrigin().Code)
                    {
                        xdt = 300;
                    }
                    else
                    {
                        xdt = 100;
                    }
                    break;

            }
            return xdt;
        }
    }
    public class CarrierModel
    {
        public apiCarrier Carrier { set; get; }
        public apiDriver Driver { set; get; }
        public apiTransport Transport { set; get; }
        public object Unify()
        {
            if (!string.IsNullOrWhiteSpace(Commons.UnifyApiUrl))
            {
                var response = HttpClientHelp.HttpPostRaw(Commons.UnifyApiUrl + "/Carriers/Enter", this.Json());
                return response;
            }
            return null;
        }

    }
    public class apiCarrier
    {
        #region 属性
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 简称
        /// </summary>
        public string Code { get; set; }

        public int CarrierType { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { set; get; }


        public int Status { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }

        #endregion

    }

    public class apiDriver
    {
        #region 属性
        /// <summary>
        /// 承运商名
        /// </summary>
        public string EnterpriseName { set; get; }
        /// <summary>
        /// 司机姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IDCard { set; get; }
        /// <summary>
        /// 手机号,大陆
        /// </summary>
        public string Mobile { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { set; get; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { set; get; }
        /// <summary>
        /// 香港手机号
        /// </summary>

        public string Mobile2 { set; get; }
        /// <summary>
        /// 海关编码
        /// </summary>

        public string CustomsCode { set; get; }
        /// <summary>
        /// 司机卡号
        /// </summary>

        public string CardCode { set; get; }
        /// <summary>
        /// 口岸电子编号
        /// </summary>

        public string PortCode { set; get; }
        /// <summary>
        /// 寮步密码
        /// </summary>

        public string LBPassword { set; get; }
        /// <summary>
        /// 是否中港贸易
        /// </summary>
        public bool IsChcd { set; get; }

        #endregion
    }

    public class apiTransport
    {
        #region 属性
        /// <summary>
        /// 承运商名
        /// </summary>
        public string EnterpriseName { set; get; }
        /// <summary>
        /// 车辆类型
        /// </summary>
        public VehicleType Type { set; get; }

        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNumber1 { get; set; }

        /// <summary>
        /// 香港车牌号
        /// </summary>
        public string CarNumber2 { get; set; }

        public int Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// 车重
        /// </summary>

        public string Weight { get; set; }


        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { set; get; }

        #endregion
    }
    //public class apiCarrier
    //{
    //    #region 属性
    //    string id;
    //    public string ID
    //    {
    //        get
    //        {
    //            return this.id ?? this.Name.MD5();

    //        }
    //        set
    //        {
    //            this.id = value;
    //        }
    //    }
    //    /// <summary>
    //    /// 名称
    //    /// </summary>
    //    public string Name { get; set; }
    //    /// <summary>
    //    /// 简称
    //    /// </summary>
    //    public string Code { get; set; }

    //    /// <summary>
    //    /// 快递的图标
    //    /// </summary>
    //   // public string Icon { get; set; }

    //    /// <summary>
    //    /// 注册地址
    //    /// </summary>
    //    //public string RegAddress { get; set; }

    //    /// <summary>
    //    /// 统一社会信用编码
    //    /// </summary>
    //   // public string Uscc { get; set; }

    //    /// <summary>
    //    /// 公司法人
    //    /// </summary>
    //   // public string Corporation { get; set; }
    //    /// <summary>
    //    /// 描述
    //    /// </summary>
    //    public string Summary { set; get; }

    //    /// <summary>
    //    /// 创建人
    //    /// </summary>
    //    public string Creator { get; set; }
    //    public int Status { set; get; }

    //    #endregion

    //}

    //public class apiDriver
    //{
    //    #region 属性
    //    public string ID { set; get; }
    //    /// <summary>
    //    /// 承运商名称=企业名称
    //    /// </summary>
    //    public string EnterpriseName { set; get; }
    //    /// <summary>
    //    /// 司机姓名
    //    /// </summary>
    //    public string Name { set; get; }
    //    /// <summary>
    //    /// 身份证号
    //    /// </summary>
    //    public string IDCard { set; get; }
    //    /// <summary>
    //    /// 手机号,大陆
    //    /// </summary>
    //    public string Mobile { set; get; }
    //    /// <summary>
    //    /// 状态
    //    /// </summary>
    //    public int Status { set; get; }
    //    /// <summary>
    //    /// 创建人ID
    //    /// </summary>
    //    public string CreatorID { set; get; }
    //    /// <summary>
    //    /// 香港手机号
    //    /// </summary>

    //    public string Mobile2 { set; get; }
    //    /// <summary>
    //    /// 海关编码
    //    /// </summary>

    //    public string CustomsCode { set; get; }
    //    /// <summary>
    //    /// 司机卡号
    //    /// </summary>

    //    public string CardCode { set; get; }
    //    /// <summary>
    //    /// 口岸电子编号
    //    /// </summary>

    //    public string PortCode { set; get; }
    //    /// <summary>
    //    /// 寮步密码
    //    /// </summary>

    //    public string LBPassword { set; get; }
    //    #endregion
    //}

    //public class apiTransport
    //{
    //    #region 属性
    //    public string ID { set; get; }
    //    public string EnterpriseName { set; get; }
    //    /// <summary>
    //    /// 车辆类型
    //    /// </summary>
    //    public VehicleType Type { set; get; }
    //    /// <summary>
    //    /// 车牌号
    //    /// </summary>
    //    public string CarNumber1 { set; get; }
    //    /// <summary>
    //    /// 临时车牌
    //    /// </summary>
    //    public string CarNumber2 { set; get; }
    //    /// <summary>
    //    /// 载重
    //    /// </summary>
    //    public string Weight { set; get; }
    //    /// <summary>
    //    /// 状态
    //    /// </summary>
    //    public int Status { set; get; }
    //    public string creator { set; get; }
    //    #endregion
    //}


}