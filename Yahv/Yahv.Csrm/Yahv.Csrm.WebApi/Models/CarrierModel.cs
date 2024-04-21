using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Yahv.Services;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Csrm.WebApi.Models
{
    public class CarrierModel
    {
        public Carrier Carrier { set; get; }
        public Driver Driver { set; get; }
        public Transport Transport { set; get; }
        public int ConvertType(CarrierType CarrierType, string Place)
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

        public CarrierContact Contact { set; get; }
    }
    public class Carrier
    {
        #region 属性
        string id;
        public string EnterpriseID
        {
            get
            {
                if (this.Name == "个人承运商")
                {
                    return this.id ?? "Personal";
                }
                else if (this.Name == "芯达通物流部")
                {
                    return this.id ?? "XdtPCL";
                }
                else
                {
                    return this.id;
                }

            }
            set
            {
                this.id = value;
            }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 简称
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 国家或地区
        /// </summary>
        public string Place { set; get; }
        /// <summary>
        /// 承运商类型
        /// </summary>
        public CarrierType Type { set; get; }
        /// <summary>
        /// 是否国际
        /// </summary>
        public bool IsInternational { set; get; }
        public int? CarrierType { set; get; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string ContactName { set; get; }
        /// <summary>
        /// 快递的图标
        /// </summary>
        // public string Icon { get; set; }

        /// <summary>
        /// 注册地址
        /// </summary>
        //public string RegAddress { get; set; }

        /// <summary>
        /// 统一社会信用编码
        /// </summary>
        // public string Uscc { get; set; }

        /// <summary>
        /// 公司法人
        /// </summary>
        // public string Corporation { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { set; get; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        public GeneralStatus Status { set; get; }

        #endregion

    }

    public class Driver
    {
        #region 属性
        public string ID { set; get; }
        /// <summary>
        /// 承运商名称=企业名称
        /// </summary>
        public string EnterpriseName { set; get; }
        /// <summary>
        /// 给靳珊珊同步时使用
        /// </summary>
        public string CarrierCode { set; get; }
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
        public GeneralStatus Status { set; get; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public string Creator { set; get; }
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

    public class Transport
    {
        #region 属性
        public string ID { set; get; }
        public string EnterpriseName { set; get; }
        // <summary>
        /// 给靳珊珊同步时使用
        /// </summary>
        public string CarrierCode { set; get; }
        /// <summary>
        /// 车辆类型
        /// </summary>
        public VehicleType Type { set; get; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string CarNumber1 { set; get; }
        /// <summary>
        /// 临时车牌
        /// </summary>
        public string CarNumber2 { set; get; }
        /// <summary>
        /// 载重
        /// </summary>
        public string Weight { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { set; get; }
        public string Creator { set; get; }
        #endregion
    }

    public class CarrierContact
    {
        public string Name { set; get; }
        public int? Type { set; get; }
        public string Tel { set; get; }
        public string Mobile { set; get; }
        public string Email { set; get; }
        public string Creator { set; get; }
    }

}