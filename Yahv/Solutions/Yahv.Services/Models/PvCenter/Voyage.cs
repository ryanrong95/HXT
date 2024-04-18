using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Underly.Enums;

namespace Yahv.Services.Models.PvCenter
{
    /// <summary>
    /// 航次信息
    /// </summary>
    public class Voyage
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
        public int Type { get; set; }

        /// <summary>
        /// 【Voyage】截单状态
        /// </summary>
        public int CutStatus { get; set; }

        /// <summary>
        /// 【Voyage】香港清关状态
        /// </summary>
        public bool HKDeclareStatus { get; set; }

        /// <summary>
        /// 【Voyage】Status
        /// </summary>
        public int Status { get; set; }

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
        public int VehicleWeight { get; set; }

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

        public void Enter()
        {

            var waybills = new Yahv.Services.Views.WaybillsTopView<PvCenterReponsitory>().Where(item => item.WayChcd.LotNumber == this.ID);
            string carrierid = null;
            if (!string.IsNullOrEmpty(this.CarrierName))
            {
  
                var carrier = new Yahv.Services.Models.Carrier();
                carrier.Enter(this.CarrierName, this.DriverName,this.HKLicense, (VehicleType)VehicleWeight, this.ContactMobile,"");

                carrierid = carrier.ID;


            }


            foreach (var item in waybills)
            {
                new Yahv.Services.Models.Waybills()
                {
                    ID = item.ID,
                    Code = item.Code,
                    FatherID = item.FatherID,
                    Type = item.Type,
                    Subcodes = item.Subcodes,
                    CarrierID = carrierid,
                    ConsignorID = item.ConsignorID,
                    ConsigneeID = item.ConsigneeID,
                    FreightPayer = item.FreightPayer,
                    TotalParts = item.TotalParts,
                    TotalWeight = item.TotalWeight,
                    CarrierAccount = item.CarrierAccount,
                    VoyageNumber = item.VoyageNumber,
                    Condition = item.Condition ?? string.Empty,
                    CreateDate = item.CreateDate,
                    ModifyDate = item.ModifyDate,
                    EnterCode = item.EnterCode ?? string.Empty,
                    Status = item.Status,
                    CreatorID = item.CreatorID,
                    ModifierID = item.ModifierID,
                    TransferID = item.TransferID,
                    IsClearance = item.IsClearance,
                    Packaging = item.Packaging,
                    Supplier = item.Supplier,
                    ExcuteStatus = item.ExcuteStatus,
                    Summary = item.Summary,
                    TotalVolume = item.TotalVolume,
                    CuttingOrderStatus = this.CutStatus,
                    InitExType = item.InitExType,
                    InitExPayType = item.InitExPayType,
                   
                    


                }.Enter();

            }

            using (var repository = new PvCenterReponsitory())
            {
                var count = repository.ReadTable<Layers.Data.Sqls.PvCenter.WayChcd>().Count(tem => tem.LotNumber == this.ID);
                if (count <= 0)
                {
                    repository.Insert(new Layers.Data.Sqls.PvCenter.WayChcd
                    {
                        ID = this.ID,
                        CarNumber1 = this.HKLicense,
                        CarNumber2 = "",
                        Carload = VehicleWeight,
                        Driver = this.DriverName,
                        LotNumber = this.ID,
                        Phone = ContactMobile,
                        DepartDate = this.TransportTime ?? DateTime.Now,


                    });
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PvCenter.WayChcd>(new 
                    {
                        CarNumber1 = this.HKLicense,
                        CarNumber2 = "",
                        Carload = VehicleWeight,
                        Driver = this.DriverName,
                        Phone = ContactMobile,
                        DepartDate = this.TransportTime ?? DateTime.Now,
                    }, tem => tem.LotNumber == this.ID);
                }
            }


        }

    }
}
