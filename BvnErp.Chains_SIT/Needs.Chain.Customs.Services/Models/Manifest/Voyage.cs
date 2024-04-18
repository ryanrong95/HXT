using Needs.Linq;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 舱单车辆关联信息表
    /// </summary>
    [Serializable]
    public class Voyage : IUnique, IPersist
    {
        /// <summary>
        /// 货物运输批次号
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 承运商
        /// </summary>
        public Carrier  Carrier { get; set; }

        ///// <summary>
        ///// 车辆
        ///// </summary>
        public Vehicle Vehicle { get; set; }


        /// <summary>
        /// 驾驶员姓名
        /// </summary>
        public string DriverName { get; set; }

        /// <summary>
        ///香港车牌号
        /// </summary>
        public string HKLicense { get; set; }

        /// <summary>
        /// 驾驶员代码
        /// </summary>
        public string DriverCode { get; set; }

        /// <summary>
        /// 香港清关状态
        /// </summary>
        public bool HKDeclareStatus { get; set; }

        /// <summary>
        /// 运输批次截单状态
        /// </summary>
        public Enums.CutStatus CutStatus { get; set; }

        /// <summary>
        /// 运输方式代码   代码表编号：CN012
        /// </summary>
        public int? TrafMode { get; set; }

        /// <summary>
        /// 进出境口岸海关代码
        /// </summary>
        public string CustomsCode { get; set; }
        /// <summary>
        /// 到达卸货地日期 yyyyMMdd
        /// </summary>
        public DateTime? ArrivalDate { get; set; }
        /// <summary>
        ///  货物装载时间  yyyyMMddHHmmss
        /// </summary>
        public DateTime? LoadingDate { get; set; }

        /// <summary>
        /// 卸货地代码 同 进境口岸代码
        /// </summary>
        public string LoadingLocationCode { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public Enums.Status Status { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 运输时间
        /// </summary>
        public DateTime? TransportTime { get; set; }

        /// <summary>
        /// 运输类型
        /// </summary>
        public Enums.VoyageType Type { get; set; }

        /// <summary>
        /// 是否被分配到报关通知上
        /// </summary>
        public bool IsForDecNotice { get; set; }

        /// <summary>
        /// 分配到的报关通知相关的订单的ID
        /// </summary>
        public string ForOrderID { get; set; }

        /// <summary>
        /// 订单是包车
        /// </summary>
        public bool OrderIsBaoChe { get; set; }

        #region 后来加的相关信息字段

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

        public string CarrierCode { get; set; } = string.Empty;

        /// <summary>
        /// 香港封条号
        /// </summary>
        public string HKSealNumber { get; set; }

        #endregion

        public Voyage()
        {
            this.UpdateDate = this.CreateTime = DateTime.Now;
            this.Status = Enums.Status.Normal;
            this.CutStatus = Enums.CutStatus.UnCutting;
            this.HKDeclareStatus = false;
        }

        public  void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Voyages>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.Voyages
                    {
                        ID = this.ID,
                        HKLicense = this.HKLicense,
                        HKDeclareStatus = this.HKDeclareStatus,
                        CarrierCode = this.Carrier.Code,
                        DriverCode = this.DriverCode,
                        DriverName = this.DriverName,
                        Status = (int)this.Status,
                        CreateTime = this.CreateTime,
                        UpdateDate = this.UpdateDate.Value,
                        Summary = this.Summary,
                        CutStatus=(int)this.CutStatus,
                        TransportTime = this.TransportTime,
                        Type = (int)this.Type,
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.Voyages
                    {
                        ID = this.ID,
                        HKLicense = this.HKLicense,
                        HKDeclareStatus = this.HKDeclareStatus,
                        CarrierCode=this.Carrier.Code,
                        DriverCode = this.DriverCode,
                        DriverName = this.DriverName,
                        Status = (int)this.Status,
                        CreateTime = this.CreateTime,
                        UpdateDate = this.UpdateDate.Value,
                        Summary = this.Summary,
                        CutStatus = (int)this.CutStatus,
                        TransportTime = this.TransportTime,
                        Type = (int)this.Type,
                    }, item => item.ID == this.ID);
                }
            }
            this.OnEnterSuccess();
        }

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 清关
        /// </summary>
        public void Clear()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Voyages>(new { HKDeclareStatus=true }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 截单
        /// </summary>
        public void SureCut()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Voyages>(new { CutStatus = Enums.CutStatus.Cutted }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 截单信息到库房Api
        /// </summary>
        public void SureCutToWmsApi()
        {
            try
            {
                string batchID = Guid.NewGuid().ToString("N");

                var requestModel = new VoyageModelToWmsApi
                {
                    ID = this.ID,
                    HKLicense  = this.HKLicense,
                    DriverName  = this.DriverName,
                    DriverCode  = this.DriverCode,
                    CarrierCode = this.CarrierCode,
                    TransportTime = this.TransportTime,
                    Type = (int)this.Type,
                    CutStatus = (int)Enums.CutStatus.Completed,
                    HKDeclareStatus = this.HKDeclareStatus,
                    Status = (int)this.Status,
                    CreateTime = this.CreateTime,
                    UpdateDate = this.UpdateDate ?? this.CreateTime,
                    Summary = this.Summary,
                    CarrierType = this.CarrierType,
                    CarrierName   = this.CarrierName,
                    CarrierQueryMark = this.CarrierQueryMark,
                    ContactMobile = this.ContactMobile,
                    CarrierAddress = this.CarrierAddress,
                    ContactName = this.ContactName,
                    ContactFax = this.ContactFax,
                    VehicleType = this.VehicleType,
                    VehicleLicence = this.VehicleLicence,
                    VehicleWeight = this.VehicleWeight, //VehicleWeight 传车辆类型了
                    VehicleSize =this.VehicleSize,

                    DriverMobile = this.DriverMobile,
                    DriverHSCode = this.DriverHSCode,
                    DriverHKMobile = this.DriverHKMobile,
                    DriverCardNo = this.DriverCardNo,
                    DriverPortElecNo = this.DriverPortElecNo,
                    DriverLaoPaoCode = this.DriverLaoPaoCode,
                };

                var dechead = new Views.DecHeadsListView().Where(t => t.IsSuccess && t.VoyageID == this.ID).ToList();
                requestModel.TotalPacks = dechead.Sum(t => t.PackNo);
                requestModel.TotalWeight = Math.Round(dechead.Sum(t => t.GrossWeight).Value, 2, MidpointRounding.AwayFromZero);

                //口岸
                requestModel.CustomsPort = dechead.FirstOrDefault().CustomsPort;

                var apisetting = new Needs.Ccs.Services.ApiSettings.PfWmsApiSetting();
                var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.VoyageSureCut;

                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = batchID,
                    Url = apiurl,
                    RequestContent = requestModel.Json(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "修改截单状态",
                };
                apiLog.Enter();

                var result = Needs.Utils.Http.ApiHelper.Current.JPost(apiurl, requestModel);
                apiLog.ResponseContent = result;
                apiLog.Enter();
            }
            catch (Exception ex)
            {
                ex.CcsLog("截单信息到库房Api");
            }
        }

        /// <summary>
        /// 完成
        /// </summary>
        public void Complate()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Voyages>(new { CutStatus = Enums.CutStatus.Completed }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        ///删除
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Voyages>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandonSuccess();
        }

        public void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        public void SetVoyageType()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Voyages>(new { Type = this.Type }, item => item.ID == this.ID);
            }
        }

    }
}


