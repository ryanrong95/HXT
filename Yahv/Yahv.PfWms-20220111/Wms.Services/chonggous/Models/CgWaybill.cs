using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Underly.Attributes;

namespace Wms.Services.chonggous.Models
{
    /// <summary>
    /// [重构] 运单状态
    /// </summary>
    public enum CgWaybillStatus
    {
        /// <summary>
        /// 待处理
        /// </summary>
        [Description("待处理")]
        Waiting = 100,
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 200,

        /// <summary>
        /// 关闭
        /// </summary>
        [Description("关闭")]
        Closed = 400,
        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Deleted = 500


    }


    public class CgWaybill : Yahv.Services.Models.Waybill
    {
        #region 扩展属性  

        /// <summary>
        /// 运单操作人
        /// </summary>
        public string OperatorID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 客户企业ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        new public CgWaybillStatus Status { get; set; }

        #endregion

        #region 持久化

        public void Enter()
        {
            //this.Enter((Layers.Data.Sqls.PvCenterReponsitory)null);

            using (var Reponsitory = new Layers.Data.Sqls.PvCenterReponsitory())
            {
                //保存交货人、收货人
                this.SaveWayParters(Reponsitory);
                //保存运单数据
                this.SaveWaybill(Reponsitory);
                //保存提送货信息
                this.SaveWayLoading(Reponsitory);
                //保存货物条款
                this.SaveWayCharge(Reponsitory);
                //保存中港报关
                this.SaveWaychcd(Reponsitory);
            }
        }

        //internal void Enter(Layers.Data.Sqls.PvCenterReponsitory pvcenterReponsitory)
        //{
        //    var Reponsitory = pvcenterReponsitory ?? new Layers.Data.Sqls.PvCenterReponsitory();

        //    //保存交货人、收货人
        //    this.SaveWayParters(Reponsitory);
        //    //保存运单数据
        //    this.SaveWaybill(Reponsitory);
        //    //保存提送货信息
        //    this.SaveWayLoading(Reponsitory);
        //    //保存货物条款
        //    this.SaveWayCharge(Reponsitory);
        //    //保存中港报关
        //    this.SaveWaychcd(Reponsitory);

        //    if (pvcenterReponsitory == null)
        //    {
        //        Reponsitory.Dispose();
        //    }
        //}

        public void Enter(Layers.Data.Sqls.PvCenterReponsitory pvcenterReponsitory)
        {
            //保存交货人、收货人
            this.SaveWayParters(pvcenterReponsitory);
            //保存运单数据
            this.SaveWaybill(pvcenterReponsitory);
            //保存提送货信息
            this.SaveWayLoading(pvcenterReponsitory);
            //保存货物条款
            this.SaveWayCharge(pvcenterReponsitory);
            //保存中港报关
            this.SaveWaychcd(pvcenterReponsitory);
        }

        /// <summary>
        /// 保存交货人、收货人
        /// </summary>
        /// <param name="Reponsitory"></param>
        private void SaveWayParters(Layers.Data.Sqls.PvCenterReponsitory Reponsitory)
        {
            //用any 会快点
            var parterView = Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayParters>();

            if (this.Consignor != null && !parterView.Any(item => item.ID == this.Consignor.ID))
            {
                Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayParters
                {
                    ID = this.Consignor.ID,
                    Company = this.Consignor.Company,
                    Place = this.Consignor.Place,
                    Address = this.Consignor.Address,
                    Contact = this.Consignor.Contact,
                    Phone = this.Consignor.Phone,
                    Zipcode = this.Consignor.Zipcode,
                    Email = this.Consignor.Email,
                    IDType = (int?)this.Consignor.IDType,
                    IDNumber = this.Consignor.IDNumber,
                    CreateDate = DateTime.Now,
                });
            }
            if (this.Consignee != null && !parterView.Any(item => item.ID == this.Consignee.ID))
            {
                Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayParters
                {
                    ID = this.Consignee.ID,
                    Company = this.Consignee.Company,
                    Place = this.Consignee.Place,
                    Address = this.Consignee.Address,
                    Contact = this.Consignee.Contact,
                    Phone = this.Consignee.Phone,
                    Zipcode = this.Consignee.Zipcode,
                    Email = this.Consignee.Email,
                    IDType = (int?)this.Consignee.IDType,
                    IDNumber = this.Consignee.IDNumber,
                    CreateDate = DateTime.Now,
                });
            }
            this.ConsignorID = this.Consignor?.ID;
            this.ConsigneeID = this.Consignee?.ID;
        }

        /// <summary>
        /// 保存运单数据
        /// </summary>
        /// <param name="Reponsitory"></param>
        private void SaveWaybill(Layers.Data.Sqls.PvCenterReponsitory Reponsitory)
        {
            //用any 会快点
            var exsit = Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.Waybills>().Any(item => item.ID == this.ID);
            if (!exsit)
            {
                this.ID = Layers.Data.PKeySigner.Pick(PKeyType.Waybill);
                Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Waybills
                {
                    ID = this.ID,
                    Code = this.Code,
                    Type = (int)this.Type,
                    Subcodes = this.Subcodes,
                    CarrierID = this.CarrierID,
                    ConsignorID = this.ConsignorID,
                    ConsigneeID = this.ConsigneeID,
                    FreightPayer = (int)this.FreightPayer,
                    TotalParts = this.TotalParts,
                    TotalWeight = this.TotalWeight,
                    TotalVolume = this.TotalVolume,
                    CarrierAccount = this.CarrierAccount,
                    VoyageNumber = this.VoyageNumber,
                    Condition = this.Condition,
                    CreateDate = this.CreateDate,
                    ModifyDate = this.ModifyDate,
                    EnterCode = this.EnterCode,
                    Status = (int)this.Status,
                    CreatorID = this.CreatorID,
                    ModifierID = this.ModifierID,
                    TransferID = this.TransferID,
                    IsClearance = this.IsClearance,
                    Packaging = this.Packaging,
                    FatherID = this.FatherID,
                    Supplier = this.Supplier,
                    ExcuteStatus = this.ExcuteStatus,
                    CuttingOrderStatus = this.CuttingOrderStatus,
                    ConfirmReceiptStatus = this.ConfirmReceiptStatus,
                    OrderID = this.OrderID,
                    Source = (int)this.Source,
                    NoticeType = (int)this.NoticeType,
                    TempEnterCode = this.TempEnterCode,
                    AppointTime = this.AppointTime,
                });
            }
            else
            {
                this.ModifyDate = DateTime.Now;
                this.ModifierID = this.OperatorID;
                Reponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    Code = this.Code,
                    Type = (int)this.Type,
                    Subcodes = this.Subcodes,
                    CarrierID = this.CarrierID,
                    ConsignorID = this.ConsignorID,
                    ConsigneeID = this.ConsigneeID,
                    FreightPayer = (int)this.FreightPayer,
                    TotalParts = this.TotalParts,
                    TotalWeight = this.TotalWeight,
                    CarrierAccount = this.CarrierAccount,
                    VoyageNumber = this.VoyageNumber,
                    Condition = this.Condition,
                    CreateDate = this.CreateDate,
                    ModifyDate = DateTime.Now,
                    EnterCode = this.EnterCode,
                    Status = (int)this.Status,
                    CreatorID = this.CreatorID,
                    ModifierID = this.ModifierID,
                    TransferID = this.TransferID,
                    IsClearance = this.IsClearance,
                    ExcuteStatus = this.ExcuteStatus,
                    CuttingOrderStatus = this.CuttingOrderStatus,
                    ConfirmReceiptStatus = this.ConfirmReceiptStatus,
                    OrderID = this.OrderID,
                    Source = (int)this.Source,
                    NoticeType = (int)this.NoticeType,
                    TempEnterCode = this.TempEnterCode,
                    AppointTime = this.AppointTime,
                }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 保存提货信息
        /// </summary>
        private void SaveWayLoading(Layers.Data.Sqls.PvCenterReponsitory Reponsitory)
        {
            if (this.WayLoading != null)
            {
                this.WayLoading.ID = this.ID;
                var count = Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayLoadings>()
                    .Count(item => item.ID == this.WayLoading.ID);
                if (count == 0)
                {
                    Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayLoadings
                    {
                        ID = this.WayLoading.ID,
                        TakingDate = this.WayLoading.TakingDate,
                        TakingAddress = this.WayLoading.TakingAddress,
                        TakingContact = this.WayLoading.TakingContact,
                        TakingPhone = this.WayLoading.TakingPhone,
                        CarNumber1 = this.WayLoading.CarNumber1,
                        Driver = this.WayLoading.Driver,
                        Carload = this.WayLoading.Carload,
                        CreateDate = this.WayLoading.CreateDate ?? DateTime.Now,
                        ModifyDate = this.WayLoading.ModifyDate,
                        CreatorID = this.WayLoading.CreatorID,
                        ModifierID = this.WayLoading.ModifierID,
                    });
                }
                else
                {
                    this.WayLoading.ModifyDate = DateTime.Now;
                    this.WayLoading.ModifierID = this.OperatorID;
                    Reponsitory.Update<Layers.Data.Sqls.PvCenter.WayLoadings>(new
                    {
                        TakingDate = this.WayLoading.TakingDate,
                        TakingAddress = this.WayLoading.TakingAddress,
                        TakingContact = this.WayLoading.TakingContact,
                        TakingPhone = this.WayLoading.TakingPhone,
                        CarNumber1 = this.WayLoading.CarNumber1,
                        Driver = this.WayLoading.Driver,
                        Carload = this.WayLoading.Carload,
                        CreateDate = this.WayLoading.CreateDate ?? DateTime.Now,
                        ModifyDate = this.WayLoading.ModifyDate,
                        CreatorID = this.WayLoading.CreatorID,
                        ModifierID = this.WayLoading.ModifierID,
                    }, item => item.ID == this.ID);
                }
            }
        }

        /// <summary>
        /// 保存货物条款
        /// </summary>
        /// <param name="waybill"></param>
        private void SaveWayCharge(Layers.Data.Sqls.PvCenterReponsitory Reponsitory)
        {
            if (this.WayCharge != null)
            {
                this.WayCharge.ID = this.ID;
                var count = Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayCharges>()
                    .Count(item => item.ID == this.WayCharge.ID);
                if (count == 0)
                {
                    Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayCharges
                    {
                        ID = this.WayCharge.ID,
                        Payer = (int)this.WayCharge.Payer,
                        PayMethod = (int)this.WayCharge.PayMethod,
                        Currency = (int)this.WayCharge.Currency,
                        TotalPrice = (decimal)this.WayCharge.TotalPrice,
                    });
                }
                else
                {
                    Reponsitory.Update<Layers.Data.Sqls.PvCenter.WayCharges>(new
                    {
                        Payer = (int)this.WayCharge.Payer,
                        PayMethod = (int)this.WayCharge.PayMethod,
                        Currency = this.WayCharge.Currency,
                        TotalPrice = this.WayCharge.TotalPrice,
                    }, item => item.ID == this.ID);
                }
            }
        }

        /// <summary>
        /// 保存中港报关
        /// </summary>
        /// <param name="Reponsitory"></param>
        private void SaveWaychcd(Layers.Data.Sqls.PvCenterReponsitory Reponsitory)
        {
            if (this.WayChcd != null)
            {
                this.WayChcd.ID = this.ID;
                var count = Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayChcd>()
                    .Count(item => item.ID == this.WayChcd.ID);
                if (count == 0)
                {
                    Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayChcd
                    {
                        ID = this.WayChcd.ID,
                        LotNumber = this.WayChcd.LotNumber,
                        CarNumber1 = this.WayChcd.CarNumber1,
                        CarNumber2 = this.WayChcd.CarNumber2,
                        Carload = this.WayChcd.Carload,
                        IsOnevehicle = this.WayChcd.IsOnevehicle,
                        Driver = this.WayChcd.Driver,
                        PlanDate = this.WayChcd.PlanDate,
                        DepartDate = this.WayChcd.DepartDate,
                        TotalQuantity = this.WayChcd.TotalQuantity,
                    });
                }
                else
                {
                    Reponsitory.Update<Layers.Data.Sqls.PvCenter.WayCharges>(new
                    {
                        LotNumber = this.WayChcd.LotNumber,
                        CarNumber1 = this.WayChcd.CarNumber1,
                        CarNumber2 = this.WayChcd.CarNumber2,
                        Carload = this.WayChcd.Carload,
                        IsOnevehicle = this.WayChcd.IsOnevehicle,
                        Driver = this.WayChcd.Driver,
                        PlanDate = this.WayChcd.PlanDate,
                        DepartDate = this.WayChcd.DepartDate,
                        TotalQuantity = this.WayChcd.TotalQuantity,
                    }, item => item.ID == this.WayChcd.ID);
                }
            }
        }

        #endregion
    }
}
