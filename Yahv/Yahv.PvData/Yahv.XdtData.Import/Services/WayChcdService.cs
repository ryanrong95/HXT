using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.XdtData.Import.Extends;

namespace Yahv.XdtData.Import.Services
{
    /// <summary>
    /// 中港报关运单数据导入
    /// </summary>
    public sealed class WayChcdService : IDataService
    {
        #region 从芯达通查询的老数据

        List<Layers.Data.Sqls.PvCenter.XdtWaybillsTopView> xdtWaybills;

        #endregion

        #region 需要保存的数据

        Layers.Data.Sqls.PvCenter.WayParters consignee;
        Layers.Data.Sqls.PvCenter.WayParters consignor;
        Dictionary<string, Layers.Data.Sqls.PvCenter.Waybills> waybills = new Dictionary<string, Layers.Data.Sqls.PvCenter.Waybills>();
        List<Layers.Data.Sqls.PvCenter.WayChcd> wayChcds = new List<Layers.Data.Sqls.PvCenter.WayChcd>();
        List<Layers.Data.Sqls.PvCenter.Logs_Waybills> logsWaybill = new List<Layers.Data.Sqls.PvCenter.Logs_Waybills>();

        #endregion

        private string[] xdtMainOrderIDs;

        public WayChcdService(params string[] mainOrderIDs)
        {
            this.xdtMainOrderIDs = mainOrderIDs;
        }

        /// <summary>
        /// 数据查询
        /// </summary>
        /// <returns></returns>
        public IDataService Query()
        {
            using (var reponsitory = new PvCenterReponsitory())
            {
                //芯达通运单
                xdtWaybills = reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.XdtWaybillsTopView>()
                    .Where(item => xdtMainOrderIDs.Contains(item.MainOrderId)).ToList();
            }

            return this;
        }

        /// <summary>
        /// 数据封装
        /// </summary>
        /// <returns></returns>
        public IDataService Encapsule()
        {
            Purchaser purchaser = PurchaserContext.Current;
            Vendor vendor = VendorContext.Current;
            Layers.Data.Sqls.PvbCrm.Carriers[] carriers;

            using (var reponsitory = new PvbCrmReponsitory())
            {
                var carrierCodes = xdtWaybills.Select(item => item.CarrierID).Distinct().ToArray();
                carriers = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Carriers>().Where(item => carrierCodes.Contains(item.Code)).ToArray();
            }

            //收货人
            consignee = new Layers.Data.Sqls.PvCenter.WayParters()
            {
                Company = purchaser.CompanyName,
                Place = Origin.CHN.ToString(),
                Address = purchaser.Address,
                Contact = purchaser.Contact,
                Phone = purchaser.Tel,
                CreateDate = this.xdtWaybills.First().CreateTime,
            };
            consignee.ID = consignee.GenID();

            //发货人
            consignor = new Layers.Data.Sqls.PvCenter.WayParters()
            {
                Company = vendor.OverseasConsignorCname,
                Place = Origin.CHN.ToString(),
                Address = vendor.Address,
                Contact = vendor.Contact,
                Phone = vendor.Tel,
                CreateDate = this.xdtWaybills.First().CreateTime,
            };
            consignor.ID = consignor.GenID();

            foreach (var currWaybill in xdtWaybills)
            {
                if (this.waybills.Keys.Any(item => item.Contains(currWaybill.LotNumber)))
                    continue;

                #region 香港出库单

                //运单
                var hkWaybill = new Layers.Data.Sqls.PvCenter.Waybills()
                {
                    ID = Layers.Data.PKeySigner.Pick(PKeyType.Waybill),
                    Type = (int)WaybillType.DeliveryToWarehouse,
                    CarrierID = carriers.FirstOrDefault(item => item.Code == currWaybill.CarrierID)?.ID,
                    ConsignorID = consignor.ID,
                    ConsigneeID = consignee.ID,
                    FreightPayer = (int)WaybillPayer.Consignor,
                    Condition = new WayCondition().Json(),
                    CreateDate = currWaybill.CreateTime,
                    ModifyDate = currWaybill.UpdateDate,
                    EnterCode = currWaybill.EnterCode,
                    Status = (int)GeneralStatus.Normal,
                    CreatorID = Npc.Robot.Obtain(),
                    IsClearance = false,
                    ExcuteStatus = (int)CgPickingExcuteStatus.Completed,
                    CuttingOrderStatus = (int)Enums.CutStatus.Cutted,
                    Source = (int)CgNoticeSource.AgentBreakCustomsForIns,
                    NoticeType = (int)CgNoticeType.Out,
                };
                this.waybills.Add(currWaybill.LotNumber + "_HK", hkWaybill);

                //中港报关运输批次
                var hkWayChcd = new Layers.Data.Sqls.PvCenter.WayChcd()
                {
                    ID = hkWaybill.ID,
                    LotNumber = currWaybill.LotNumber,
                    CarNumber1 = currWaybill.CarNumber1,
                    CarNumber2 = currWaybill.CarNumber2,
                    Carload = currWaybill.Carload,
                    IsOnevehicle = true,
                    Driver = currWaybill.Driver,
                    PlanDate = currWaybill.PlanDate,
                    DepartDate = currWaybill.DepartDate
                };
                this.wayChcds.Add(hkWayChcd);

                //日志
                var hkLogWaybill = new Layers.Data.Sqls.PvCenter.Logs_Waybills()
                {
                    ID = Guid.NewGuid().ToString(),
                    MainID = hkWaybill.ID,
                    Type = (int)CgPickingExcuteStatus.Completed,
                    Status = hkWaybill.ExcuteStatus.Value,
                    CreateDate = hkWaybill.CreateDate,
                    CreatorID = hkWaybill.CreatorID,
                    IsCurrent = true,
                };
                this.logsWaybill.Add(hkLogWaybill);

                #endregion

                #region 深圳入库单

                //运单
                var szWaybill = new Layers.Data.Sqls.PvCenter.Waybills()
                {
                    ID = Layers.Data.PKeySigner.Pick(PKeyType.Waybill),
                    Type = (int)WaybillType.DeliveryToWarehouse,
                    CarrierID = carriers.FirstOrDefault(item => item.Code == currWaybill.CarrierID)?.ID,
                    ConsignorID = consignor.ID,
                    ConsigneeID = consignee.ID,
                    FreightPayer = (int)WaybillPayer.Consignor,
                    Condition = new WayCondition().Json(),
                    CreateDate = currWaybill.CreateTime,
                    ModifyDate = currWaybill.UpdateDate,
                    EnterCode = currWaybill.EnterCode,
                    Status = (int)GeneralStatus.Normal,
                    CreatorID = Npc.Robot.Obtain(),
                    IsClearance = false,
                    ExcuteStatus = (int)CgSortingExcuteStatus.Completed,
                    Source = (int)CgNoticeSource.AgentBreakCustomsForIns,
                    NoticeType = (int)CgNoticeType.Enter,
                };
                this.waybills.Add(currWaybill.LotNumber + "_SZ", szWaybill);

                //中港报关运输批次
                var szWayChcd = new Layers.Data.Sqls.PvCenter.WayChcd()
                {
                    ID = szWaybill.ID,
                    LotNumber = currWaybill.LotNumber,
                    CarNumber1 = currWaybill.CarNumber1,
                    CarNumber2 = currWaybill.CarNumber2,
                    Carload = currWaybill.Carload,
                    IsOnevehicle = true,
                    Driver = currWaybill.Driver,
                    PlanDate = currWaybill.PlanDate,
                    DepartDate = currWaybill.DepartDate
                };
                this.wayChcds.Add(szWayChcd);

                //日志
                var szLogWaybill = new Layers.Data.Sqls.PvCenter.Logs_Waybills()
                {
                    ID = Guid.NewGuid().ToString(),
                    MainID = szWaybill.ID,
                    Type = (int)CgSortingExcuteStatus.Completed,
                    Status = szWaybill.ExcuteStatus.Value,
                    CreateDate = szWaybill.CreateDate,
                    CreatorID = szWaybill.CreatorID,
                    IsCurrent = true,
                };
                this.logsWaybill.Add(szLogWaybill);

                #endregion
            }

            return this;
        }

        /// <summary>
        /// 数据持久化
        /// </summary>
        public void Enter()
        {
            using (var reponsitory = new PvCenterReponsitory())
            {
                //收发货人
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayParters>().Any(item => item.ID == consignee.ID))
                    reponsitory.Insert(consignee);
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayParters>().Any(item => item.ID == consignor.ID))
                    reponsitory.Insert(consignor);

                //运输批次
                var lotNumbers = this.wayChcds.Select(item => item.LotNumber).Distinct().ToArray();
                foreach (var lotNumber in lotNumbers)
                {
                    if (reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayChcd>().Any(item => item.LotNumber == lotNumber))
                        continue;

                    var wayChcdsInsert = this.wayChcds.Where(item => item.LotNumber == lotNumber).ToArray();
                    var wayChcdIDs = wayChcdsInsert.Select(item => item.ID).ToArray();
                    var waybillsInsert = this.waybills.Values.Where(item => wayChcdIDs.Contains(item.ID)).ToArray();
                    var waybillIDs = wayChcdsInsert.Select(item => item.ID).ToArray();
                    var logsWaybillInsert = this.logsWaybill.Where(item => waybillIDs.Contains(item.MainID)).ToArray();

                    reponsitory.Insert(waybillsInsert);
                    reponsitory.Insert(wayChcdsInsert);
                    reponsitory.Insert(logsWaybillInsert);
                }
            }
        }
    }
}
