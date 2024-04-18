using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.XdtData.Import.Connections;
using Yahv.XdtData.Import.Enums;
using Yahv.XdtData.Import.Extends;

namespace Yahv.XdtData.Import.Services
{
    /// <summary>
    /// 深圳出库运单数据导入
    /// </summary>
    public sealed class SzOutWaybillService : IDataService
    {
        #region 从芯达通查询的老数据

        Layers.Data.Sqls.PvCenter.XdtDecHeadsTopView[] decHeads;
        Layers.Data.Sqls.PvCenter.XdtSzOutWaybillsTopView[] szOutWaybills;
        Layers.Data.Sqls.PvCenter.XdtSzOutConsigneesTopView[] szOutConsignees = null;
        Layers.Data.Sqls.PvCenter.XdtSzOutDeliversTopView[] szOutDelivers = null;
        Layers.Data.Sqls.PvCenter.XdtSzOutExpressagesTopView[] szOutExpressages = null;

        string clientID;

        #endregion

        #region 需要保存的数据

        List<Layers.Data.Sqls.PvCenter.Waybills> szWaybills = new List<Layers.Data.Sqls.PvCenter.Waybills>();
        Dictionary<string, Layers.Data.Sqls.PvCenter.WayParters> szWayParters = new Dictionary<string, Layers.Data.Sqls.PvCenter.WayParters>();
        List<Layers.Data.Sqls.PvCenter.WayLoadings> szWayLoadings = new List<Layers.Data.Sqls.PvCenter.WayLoadings>();
        List<Layers.Data.Sqls.PvCenter.WayExpress> szWayExpress = new List<Layers.Data.Sqls.PvCenter.WayExpress>();
        List<Layers.Data.Sqls.PvCenter.Logs_Waybills> szLogsWaybills = new List<Layers.Data.Sqls.PvCenter.Logs_Waybills>();

        #endregion

        #region 扩展属性

        //芯达通出库单与PvCenter运单的映射关系
        Dictionary<string, string> mapsWaybill = new Dictionary<string, string>();
        public Dictionary<string, string> MapsWaybill
        {
            get { return this.mapsWaybill; }
        }

        #endregion

        private string[] xdtMainOrderIDs;

        public SzOutWaybillService(params string[] mainOrderIDs)
        {
            this.xdtMainOrderIDs = mainOrderIDs;
        }

        /// <summary>
        /// 数据查询
        /// </summary>
        /// <returns></returns>
        public IDataService Query()
        {
            using (var orderContext = new PvWsOrderReponsitory())
            using (var context = new PvCenterReponsitory())
            {
                //芯达通主订单
                var xdtMainOrder = orderContext.ReadTable<Layers.Data.Sqls.PvWsOrder.XdtMainOrdersTopView>()
                    .FirstOrDefault(item => this.xdtMainOrderIDs.Contains(item.ID));
                clientID = xdtMainOrder.ClientID;

                decHeads = context.ReadTable<Layers.Data.Sqls.PvCenter.XdtDecHeadsTopView>()
                    .Where(item => xdtMainOrderIDs.Contains(item.MainOrderID)).ToArray();

                szOutWaybills = context.ReadTable<Layers.Data.Sqls.PvCenter.XdtSzOutWaybillsTopView>()
                    .Where(item => xdtMainOrderIDs.Contains(item.OrderID)).ToArray();

                if (szOutWaybills.Length > 0)
                {
                    var consigneeIDs = szOutWaybills.Where(item => item.ConsigneeID != null).Select(item => item.ConsigneeID).Distinct().ToArray();
                    if (consigneeIDs.Length > 0)
                    {
                        szOutConsignees = context.ReadTable<Layers.Data.Sqls.PvCenter.XdtSzOutConsigneesTopView>()
                            .Where(item => consigneeIDs.Contains(item.ID)).ToArray();
                    }
                    var deliverIDs = szOutWaybills.Where(item => item.DeliverID != null).Select(item => item.DeliverID).Distinct().ToArray();
                    if (deliverIDs.Length > 0)
                    {
                        szOutDelivers = context.ReadTable<Layers.Data.Sqls.PvCenter.XdtSzOutDeliversTopView>()
                            .Where(item => deliverIDs.Contains(item.ID)).ToArray();
                    }
                    var expressageIDs = szOutWaybills.Where(item => item.ExpressageID != null).Select(item => item.ExpressageID).Distinct().ToArray();
                    if (expressageIDs.Length > 0)
                    {
                        szOutExpressages = context.ReadTable<Layers.Data.Sqls.PvCenter.XdtSzOutExpressagesTopView>()
                            .Where(item => expressageIDs.Contains(item.ID)).ToArray();
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// 数据封装
        /// </summary>
        /// <returns></returns>
        public IDataService Encapsule()
        {
            foreach (var xdtMainOrderID in this.xdtMainOrderIDs)
            {
                var currWaybills = this.szOutWaybills.Where(item => item.OrderID == xdtMainOrderID).ToArray();
                if (currWaybills.Length == 0)
                {
                    EncapsuleByOrder(xdtMainOrderID);
                }
                else
                {
                    EncapsuleByXdtWaybills(currWaybills);
                }
            }

            return this;
        }

        /// <summary>
        /// 数据持久化
        /// </summary>
        public void Enter()
        {
            Layers.Data.Sqls.PvCenter.WayParters[] wayParterArr;
            using (var reponsitory = new PvCenterReponsitory())
            {
                var existedIDs = reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayParters>().Where(item => szWayParters.Keys.Contains(item.ID))
                                            .Select(item => item.ID).ToArray();
                wayParterArr = szWayParters.Where(item => !existedIDs.Contains(item.Key)).Select(item => item.Value).ToArray();
            }

            using (var conn = ConnManager.Current.PvCenter)
            {
                if (wayParterArr.Length > 0)
                    conn.BulkInsert(wayParterArr);
                if (szWaybills.Count > 0)
                    conn.BulkInsert(szWaybills);
                if (szWayLoadings.Count > 0)
                    conn.BulkInsert(szWayLoadings);
                if (szWayExpress.Count > 0)
                    conn.BulkInsert(szWayExpress);
                if (szLogsWaybills.Count > 0)
                    conn.BulkInsert(szLogsWaybills);
            }
        }

        /// <summary>
        /// 芯达通没有实际深圳出库运单数据，则以大订单为单位自动生成
        /// </summary>
        private void EncapsuleByOrder(string orderID)
        {
            WsClient client;
            Consignee clientConsignee;
            string adminID;
            string carrierID;
            Purchaser purchaser = PurchaserContext.Current;

            using (var reponsitory = new PvbCrmReponsitory())
            {
                string clientName = System.Configuration.ConfigurationManager.AppSettings[this.clientID];
                client = new Yahv.Services.Views.WsClientsTopView<PvbCrmReponsitory>(reponsitory).FirstOrDefault(item => item.Name == clientName);
                clientConsignee = new Yahv.Services.Views.WsConsigneesTopView<PvbCrmReponsitory>(reponsitory).FirstOrDefault(item => item.EnterpriseID == client.ID);

                var originID = System.Configuration.ConfigurationManager.AppSettings["XdtAdminID"];
                adminID = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.AdminsTopView>().Single(item => item.OriginID == originID).ID;

                string carrierCode = System.Configuration.ConfigurationManager.AppSettings["Carrier"];
                var carrier = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Carriers>().FirstOrDefault(item => item.Code == carrierCode);
                carrierID = carrier?.ID ?? "";
            }

            //报关信息
            var currDecHeads = decHeads.Where(item => item.MainOrderID == orderID).ToArray();
            var declareDate = currDecHeads.First().DDate ?? currDecHeads.First().CreateTime;
            int packNos = currDecHeads.Sum(item => item.PackNo);

            //收货人
            var szConsignee = new Layers.Data.Sqls.PvCenter.WayParters()
            {
                Company = client.Name,
                Place = Origin.CHN.ToString(),
                Address = clientConsignee?.Address ?? "",
                Contact = clientConsignee?.Name ?? "",
                Phone = clientConsignee?.Mobile ?? clientConsignee?.Tel ?? "",
                CreateDate = declareDate
            };
            szConsignee.ID = szConsignee.GenID();
            if (!szWayParters.ContainsKey(szConsignee.ID))
            {
                szWayParters.Add(szConsignee.ID, szConsignee);
            }

            //交货人
            var szConsignor = new Layers.Data.Sqls.PvCenter.WayParters()
            {
                Company = purchaser.CompanyName,
                Place = Origin.CHN.ToString(),
                Address = purchaser.Address,
                Contact = purchaser.Contact,
                Phone = purchaser.Tel,
                Email = string.Empty,
                CreateDate = declareDate
            };
            szConsignor.ID = szConsignor.GenID();
            if (!szWayParters.ContainsKey(szConsignor.ID))
            {
                szWayParters.Add(szConsignor.ID, szConsignor);
            }

            //运单
            var szWaybill = new Layers.Data.Sqls.PvCenter.Waybills()
            {
                ID = Layers.Data.PKeySigner.Pick(PKeyType.Waybill),
                Code = orderID,
                Type = (int)WaybillType.DeliveryToWarehouse,
                CarrierID = carrierID,
                ConsignorID = szConsignor.ID,
                ConsigneeID = szConsignee.ID,
                FreightPayer = (int)WaybillPayer.Consignor,
                TotalParts = packNos,
                Condition = new WayCondition().Json(),
                CreateDate = declareDate,
                ModifyDate = declareDate,
                EnterCode = client.EnterCode,
                Status = (int)GeneralStatus.Normal,
                CreatorID = adminID,
                IsClearance = false,
                ExcuteStatus = (int)PickingExcuteStatus.OutStock,
                AppointTime = declareDate.AddDays(1),
                OrderID = orderID,
                Source = (int)CgNoticeSource.AgentBreakCustomsForIns,
                NoticeType = (int)CgNoticeType.Out,
            };
            szWaybills.Add(szWaybill);

            var szWayLoading = new Layers.Data.Sqls.PvCenter.WayLoadings()
            {
                ID = szWaybill.ID,
                CarNumber1 = System.Configuration.ConfigurationManager.AppSettings["Vehicle"],
                Driver = System.Configuration.ConfigurationManager.AppSettings["Driver"],
                CreateDate = declareDate,
                ModifyDate = declareDate,
                CreatorID = adminID,
                ModifierID = adminID,
            };
            szWayLoadings.Add(szWayLoading);

            //日志
            var szLogWaybill = new Layers.Data.Sqls.PvCenter.Logs_Waybills()
            {
                ID = Guid.NewGuid().ToString(),
                MainID = szWaybill.ID,
                Type = (int)CgPickingExcuteStatus.Completed,
                Status = szWaybill.ExcuteStatus.Value,
                CreateDate = szWaybill.CreateDate,
                CreatorID = szWaybill.CreatorID,
                IsCurrent = true,
            };
            szLogsWaybills.Add(szLogWaybill);

            mapsWaybill.Add(orderID, szWaybill.ID);
        }

        /// <summary>
        /// 芯达通有实际深圳出库运单数据，则根据芯达通数据生成
        /// </summary>
        /// <param name="currWaybills"></param>
        private void EncapsuleByXdtWaybills(Layers.Data.Sqls.PvCenter.XdtSzOutWaybillsTopView[] currWaybills)
        {
            Layers.Data.Sqls.PvbCrm.Carriers[] carriers = null;
            Layers.Data.Sqls.PvbCrm.AdminsTopView[] admins;
            Yahv.Services.Models.WsClient client;
            Purchaser purchaser = PurchaserContext.Current;

            using (var reponsitory = new PvbCrmReponsitory())
            {
                if (szOutDelivers != null)
                {
                    var carrierCodes = szOutDelivers.Select(item => item.CarrierCode).Distinct().ToArray();
                    carriers = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Carriers>().Where(item => carrierCodes.Contains(item.Code)).ToArray();
                }

                string clientName = System.Configuration.ConfigurationManager.AppSettings[this.clientID];
                client = new Yahv.Services.Views.WsClientsTopView<PvbCrmReponsitory>(reponsitory).FirstOrDefault(item => item.Name == clientName);

                var adminIDs = currWaybills.Select(item => item.AdminID).Distinct().ToArray();
                admins = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.AdminsTopView>().Where(item => adminIDs.Contains(item.OriginID)).ToArray();
            }

            var serirs = PKeyType.Waybill.Pick(currWaybills.Length);

            for (int index = 0; index < currWaybills.Length; index++)
            {
                var currWaybill = currWaybills[index];
                var szOutConsignee = szOutConsignees?.FirstOrDefault(item => item.ID == currWaybill.ConsigneeID);
                var szOutDeliver = szOutDelivers?.FirstOrDefault(item => item.ID == currWaybill.DeliverID);
                var szOutExpressage = szOutExpressages?.FirstOrDefault(item => item.ID == currWaybill.ExpressageID);
                string adminID = admins.SingleOrDefault(item => item.OriginID == currWaybill.AdminID).ID;

                //收货人
                var szConsignee = new Layers.Data.Sqls.PvCenter.WayParters()
                {
                    Company = currWaybill.Name,
                    Place = Origin.CHN.ToString(),
                    Address = currWaybill.ConsigneeID != null ? purchaser.Address : currWaybill.DeliverID != null ? szOutDeliver.Address : szOutExpressage.Address,
                    Contact = currWaybill.ConsigneeID != null ? szOutConsignee.Name : currWaybill.DeliverID != null ? szOutDeliver.Contact : szOutExpressage.Contact,
                    Phone = currWaybill.ConsigneeID != null ? szOutConsignee.Mobile : currWaybill.DeliverID != null ? szOutDeliver.Mobile : szOutExpressage.Mobile,
                    IDType = currWaybill.ConsigneeID != null ? (int?)szOutConsignee.IDType : null,
                    IDNumber = currWaybill.ConsigneeID != null ? szOutConsignee.IDNumber : null,
                    CreateDate = currWaybill.CreateDate
                };
                szConsignee.ID = szConsignee.GenID();
                if (!szWayParters.ContainsKey(szConsignee.ID))
                {
                    szWayParters.Add(szConsignee.ID, szConsignee);
                }

                //交货人
                var szConsignor = new Layers.Data.Sqls.PvCenter.WayParters()
                {
                    Company = purchaser.CompanyName,
                    Place = Origin.CHN.ToString(),
                    Address = purchaser.Address,
                    Contact = purchaser.Contact,
                    Phone = purchaser.Tel,
                    Email = string.Empty,
                    CreateDate = currWaybill.CreateDate
                };
                szConsignor.ID = szConsignor.GenID();
                if (!szWayParters.ContainsKey(szConsignor.ID))
                {
                    szWayParters.Add(szConsignor.ID, szConsignor);
                }

                //运单
                var szWaybill = new Layers.Data.Sqls.PvCenter.Waybills()
                {
                    ID = serirs[index],
                    Code = currWaybill.Code,
                    Type = currWaybill.ConsigneeID != null ? (int)WaybillType.PickUp : currWaybill.DeliverID != null ? (int)WaybillType.DeliveryToWarehouse : (int)WaybillType.LocalExpress,
                    CarrierID = currWaybill.DeliverID != null ? carriers.FirstOrDefault(item => item.Code == szOutDeliver.CarrierCode)?.ID : null,
                    ConsignorID = szConsignor.ID,
                    ConsigneeID = szConsignee.ID,
                    FreightPayer = currWaybill.ConsigneeID != null ? (int)WaybillPayer.Consignee : (int)WaybillPayer.Consignor,
                    TotalParts = currWaybill.PackNo,
                    Condition = new WayCondition().Json(),
                    CreateDate = currWaybill.CreateDate,
                    ModifyDate = currWaybill.UpdateDate,
                    EnterCode = client.EnterCode,
                    Status = (int)GeneralStatus.Normal,
                    CreatorID = adminID,
                    IsClearance = false,
                    ExcuteStatus = (int)PickingExcuteStatus.OutStock,
                    AppointTime = currWaybill.DeliverDate,
                    OrderID = currWaybill.OrderID,
                    Source = (int)CgNoticeSource.AgentBreakCustomsForIns,
                    NoticeType = (int)CgNoticeType.Out,
                };
                szWaybills.Add(szWaybill);

                //日志
                var szLogWaybill = new Layers.Data.Sqls.PvCenter.Logs_Waybills()
                {
                    ID = Guid.NewGuid().ToString(),
                    MainID = szWaybill.ID,
                    Type = (int)CgPickingExcuteStatus.Completed,
                    Status = szWaybill.ExcuteStatus.Value,
                    CreateDate = szWaybill.CreateDate,
                    CreatorID = szWaybill.CreatorID,
                    IsCurrent = true,
                };
                szLogsWaybills.Add(szLogWaybill);

                //送货单
                if (currWaybill.DeliverID != null)
                {
                    var szWayLoading = new Layers.Data.Sqls.PvCenter.WayLoadings()
                    {
                        ID = szWaybill.ID,
                        CarNumber1 = szOutDeliver.VehicleLicense,
                        Driver = szOutDeliver.DriverName,
                        CreateDate = szOutDeliver.CreateDate,
                        ModifyDate = szOutDeliver.UpdateDate,
                        CreatorID = adminID,
                        ModifierID = adminID,
                        ExcuteStatus = (int)CgPickingExcuteStatus.Completed,
                    };
                    szWayLoadings.Add(szWayLoading);
                }

                //快递单
                if (currWaybill.ExpressageID != null)
                {
                    var szWayExp = new Layers.Data.Sqls.PvCenter.WayExpress()
                    {
                        ID = szWaybill.ID,
                        ExType = szOutExpressage.TypeValue,
                        ExPayType = szOutExpressage.PayType,
                    };
                    szWayExpress.Add(szWayExp);
                }

                mapsWaybill.Add(currWaybill.ExitNoticeID, szWaybill.ID);
            }
        }
    }
}
