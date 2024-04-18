using Needs.Ccs.Services.Enums;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;

namespace Needs.Ccs.Services.Models.ApiModels
{
    /// <summary>
    ///物流管理
    /// </summary>
    public class CarrierModel
    {
        public Carrier Carrier { set; get; }
        public Driver Driver { set; get; }
        public Transport Transport { set; get; }
    }
    public class Carrier
    {
        #region 属性
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Name, this.Code).MD5();
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

        public Enums.CarrierType CarrierType { get; set; }

        public CRMCarrierType Type { get; set; }

        /// <summary>
        /// 国家或地区
        /// </summary>
        public string Place { set; get; }
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

    public class Driver
    {
        #region 属性
        /// <summary>
        /// 承运商名
        /// </summary>
        public string EnterpriseName { set; get; }
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Name, this.IDCard).MD5();
            }
            set
            {
                this.id = value;
            }
        }

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
       /// 是否中港贸易  ，默认false 
       /// </summary>

        public bool? IsChcd { set; get; }
        #region  库房同步的时候使用 ，可为空

        /// <summary>
        /// 简称
        /// </summary>
        public string CarrierCode { get; set; }
        #endregion


        #endregion
    }

    public class Transport
    {
        #region 属性
        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Type, this.CarNumber1).MD5();
            }
            set
            {
                this.id = value;
            }
        }
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
        /// <summary>
        /// 简称
        /// </summary>
        public string CarrierCode { get; set; }

        #endregion
    }


    public class Common
    {

        /// <summary>
        /// 根据承运商类型转化成CRM对应得地区和类型
        /// </summary>
        /// <param name="carrier"></param>
        /// <returns></returns>
     public    static Dictionary<string,int>  GetCarrierInfo(CarrierType carrier)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            switch (carrier)
            {
                case CarrierType.InteExpress:
                    dic.Add("HKG", (int)CRMCarrierType.Express);
                    break;
                case CarrierType.InteLogistics:
                    dic.Add("HKG", (int)CRMCarrierType.Logistics);
                    break;
                case CarrierType.DomesticExpress:
                    dic.Add("CHN", (int)CRMCarrierType.Express);
                    break;
                case CarrierType.DomesticLogistics:
                    dic.Add("CHN", (int)CRMCarrierType.Logistics);
                    break;

            }
            return dic;

        }

    }
}