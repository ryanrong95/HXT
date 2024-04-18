using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 运输批次
    /// </summary>
    [Serializable]
    public class Voyage : IUnique, IPersist
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// 【车辆】香港车牌号
        /// </summary>
        public string HKLicense { get; set; } = string.Empty;

        /// <summary>
        /// 【司机】司机姓名
        /// </summary>
        public string DriverName { get; set; } = string.Empty;

        /// <summary>
        /// 【司机】司机证件编码 Drivers.Licence
        /// </summary>
        public string DriverCode { get; set; } = string.Empty;

        /// <summary>
        /// 【承运商】承运商简称
        /// </summary>
        public string CarrierCode { get; set; } = string.Empty;

        /// <summary>
        /// 【Voyage】运输时间
        /// </summary>
        public DateTime? TransportTime { get; set; }

        /// <summary>
        /// 【Voyage】运输类型
        /// </summary>
        public Enums.VoyageType Type { get; set; }

        /// <summary>
        /// 【Voyage】截单状态
        /// </summary>
        public Enums.CutStatus CutStatus { get; set; }

        /// <summary>
        /// 【Voyage】香港清关状态
        /// </summary>
        public bool HKDeclareStatus { get; set; }

        /// <summary>
        /// 【Voyage】Status
        /// </summary>
        public Enums.Status Status { get; set; }

        /// <summary>
        /// 【Voyage】CreateTime
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 【Voyage】UpdateDate
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 【Voyage】Summary
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// 【承运商】承运商类型
        /// </summary>
        public int? CarrierType { get; set; }

        /// <summary>
        /// 【承运商】名称
        /// </summary>
        public string CarrierName { get; set; } = string.Empty;

        /// <summary>
        /// 【承运商】查询标记
        /// </summary>
        public string CarrierQueryMark { get; set; } = string.Empty;

        /// <summary>
        /// 【承运商】联系电话
        /// </summary>
        public string ContactMobile { get; set; } = string.Empty;

        /// <summary>
        /// 【承运商】承运商地址
        /// </summary>
        public string CarrierAddress { get; set; } = string.Empty;

        /// <summary>
        /// 【承运商】联系人
        /// </summary>
        public string ContactName { get; set; } = string.Empty;

        /// <summary>
        /// 【承运商】传真
        /// </summary>
        public string ContactFax { get; set; } = string.Empty;

        /// <summary>
        /// 【车辆】车辆类型
        /// </summary>
        public int? VehicleType { get; set; }

        /// <summary>
        /// 【车辆】车牌号
        /// </summary>
        public string VehicleLicence { get; set; } = string.Empty;

        /// <summary>
        /// 【车辆】车重
        /// </summary>
        public string VehicleWeight { get; set; } = string.Empty;

        /// <summary>
        /// 【车辆】尺寸
        /// </summary>
        public string VehicleSize { get; set; } = string.Empty;

        /// <summary>
        /// 【司机】大陆手机号
        /// </summary>
        public string DriverMobile { get; set; } = string.Empty;

        /// <summary>
        /// 【司机】司机海关编号
        /// </summary>
        public string DriverHSCode { get; set; } = string.Empty;

        /// <summary>
        /// 【司机】香港手机号
        /// </summary>
        public string DriverHKMobile { get; set; } = string.Empty;

        /// <summary>
        /// 【司机】司机卡号
        /// </summary>
        public string DriverCardNo { get; set; } = string.Empty;

        /// <summary>
        /// 【司机】口岸电子编号
        /// </summary>
        public string DriverPortElecNo { get; set; } = string.Empty;

        /// <summary>
        /// 【司机】寮步密码
        /// </summary>
        public string DriverLaoPaoCode { get; set; } = string.Empty;

        public event SuccessHanlder AbandonSuccess;
        //public event ErrorHanlder AbandonError;
        public event SuccessHanlder EnterSuccess;
        //public event ErrorHanlder EnterError;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.Voyages>(new Layer.Data.Sqls.ScCustoms.Voyages
                    {
                        ID = this.ID,
                        HKLicense = this.HKLicense,
                        DriverName = this.DriverName,
                        DriverCode = this.DriverCode,
                        CarrierCode = this.CarrierCode,
                        TransportTime = this.TransportTime,
                        Type = (int)this.Type,
                        CutStatus = (int)this.CutStatus,
                        HKDeclareStatus = this.HKDeclareStatus,
                        Status = (int)this.Status,
                        CreateTime = this.CreateTime,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                        CarrierType = this.CarrierType,
                        CarrierName = this.CarrierName,
                        CarrierQueryMark = this.CarrierQueryMark,
                        ContactMobile = this.ContactMobile,
                        CarrierAddress = this.CarrierAddress,
                        ContactName = this.ContactName,
                        ContactFax = this.ContactFax,
                        VehicleType = this.VehicleType,
                        VehicleLicence = this.VehicleLicence,
                        VehicleWeight = this.VehicleWeight,
                        VehicleSize = this.VehicleSize,
                        DriverMobile = this.DriverMobile,
                        DriverHSCode = this.DriverHSCode,
                        DriverHKMobile = this.DriverHKMobile,
                        DriverCardNo = this.DriverCardNo,
                        DriverPortElecNo = this.DriverPortElecNo,
                        DriverLaoPaoCode = this.DriverLaoPaoCode,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Voyages>(new
                    {
                        HKLicense = this.HKLicense,
                        DriverName = this.DriverName,
                        DriverCode = this.DriverCode,
                        CarrierCode = this.CarrierCode,
                        TransportTime = this.TransportTime,
                        Type = (int)this.Type,
                        CutStatus = (int)this.CutStatus,
                        HKDeclareStatus = this.HKDeclareStatus,
                        Status = (int)this.Status,
                        CreateTime = this.CreateTime,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                        CarrierType = this.CarrierType,
                        CarrierName = this.CarrierName,
                        CarrierQueryMark = this.CarrierQueryMark,
                        ContactMobile = this.ContactMobile,
                        CarrierAddress = this.CarrierAddress,
                        ContactName = this.ContactName,
                        ContactFax = this.ContactFax,
                        VehicleType = this.VehicleType,
                        VehicleLicence = this.VehicleLicence,
                        VehicleWeight = this.VehicleWeight,
                        VehicleSize = this.VehicleSize,
                        DriverMobile = this.DriverMobile,
                        DriverHSCode = this.DriverHSCode,
                        DriverHKMobile = this.DriverHKMobile,
                        DriverCardNo = this.DriverCardNo,
                        DriverPortElecNo = this.DriverPortElecNo,
                        DriverLaoPaoCode = this.DriverLaoPaoCode,
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnter();
        }

        virtual public void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Voyages>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandon();
        }

        virtual protected void OnAbandon()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }

    }

    [Serializable]
    public class Voyage1 : Voyage
    {

    }
}
