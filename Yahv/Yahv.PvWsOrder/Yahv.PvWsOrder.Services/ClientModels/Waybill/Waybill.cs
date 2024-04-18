using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsOrder.Services.ClientModels
{
    public class Waybill : Yahv.Services.Models.Waybill
    {
        public Waybill()
        {
            this.EnterSuccess += Waybill_EnterSuccess;
        }

        #region 拓展属性
        /// <summary>
        /// 入库条件
        /// </summary>
        public Yahv.Services.Models.WayCondition WayCondition
        {
            get
            {
                return this.Condition.JsonTo<Yahv.Services.Models.WayCondition>();
            }
        }
        #endregion

        #region 事件

        public virtual event SuccessHanlder EnterSuccess;

        private void Waybill_EnterSuccess(object sender, SuccessEventArgs e)
        {
            var waybill = (Waybill)e.Object;
            #region 保存自提
            using (PvCenterReponsitory Reponsitory = LinqFactory<PvCenterReponsitory>.Create())
            {
                var count = Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayLoadings>().Count(item => item.ID == waybill.ID);
                //保存提货信息
                if (this.WayLoading != null)
                {
                    this.WayLoading.ID = waybill.ID;

                    if (count == 0 && this.WayLoading != null)
                    {
                        Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayLoadings
                        {
                            ID = this.WayLoading.ID,
                            TakingDate = (DateTime)this.WayLoading.TakingDate,
                            TakingAddress = this.WayLoading.TakingAddress,
                            TakingContact = this.WayLoading.TakingContact,
                            TakingPhone = this.WayLoading.TakingPhone,
                            CarNumber1 = this.WayLoading.CarNumber1,
                            Driver = this.WayLoading.Driver,
                            Carload = this.WayLoading.Carload,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            CreatorID = this.WayLoading.CreatorID,
                            ModifierID = this.WayLoading.ModifierID,
                            ExcuteStatus = (int)this.WayLoading.LoadingExcuteStatus,
                        });
                    }
                    else if (count > 0 && this.WayLoading != null)
                    {
                        Reponsitory.Update<Layers.Data.Sqls.PvCenter.WayLoadings>(new
                        {
                            TakingDate = (DateTime)this.WayLoading.TakingDate,
                            TakingAddress = this.WayLoading.TakingAddress,
                            TakingContact = this.WayLoading.TakingContact,
                            TakingPhone = this.WayLoading.TakingPhone,
                            CarNumber1 = this.WayLoading.CarNumber1,
                            Driver = this.WayLoading.Driver,
                            Carload = this.WayLoading.Carload,
                            CreateDate = DateTime.Now,
                            ModifyDate = DateTime.Now,
                            CreatorID = this.WayLoading.CreatorID,
                            ModifierID = this.WayLoading.ModifierID,
                            ExcuteStatus = (int)this.WayLoading.LoadingExcuteStatus,
                        }, item => item.ID == waybill.ID);
                    }
                }
                else if (count > 0)
                {
                    Reponsitory.Delete<Layers.Data.Sqls.PvCenter.WayLoadings>(item => item.ID == waybill.ID);
                }

                //count = Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayCharges>().Count(item => item.ID == waybill.ID);
                //if (this.WayCharge != null)
                //{
                //    this.WayCharge.ID = this.ID;

                //    if (count == 0)
                //    {
                //        Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayCharges
                //        {
                //            ID = this.WayCharge.ID,
                //            PayMethod = (int)this.WayCharge.PayMethod,
                //            Payer = (int)this.WayCharge.Payer,
                //            Currency = (int)this.WayCharge.Currency,
                //            TotalPrice = (decimal)this.WayCharge.TotalPrice,
                //        });
                //    }
                //    else
                //    {
                //        Reponsitory.Update<Layers.Data.Sqls.PvCenter.WayCharges>(new
                //        {
                //            ID = this.WayCharge.ID,
                //            PayMethod = (int)this.WayCharge.PayMethod,
                //            Payer = (int)this.WayCharge.Payer,
                //            Currency = (int)this.WayCharge.Currency,
                //            TotalPrice = (decimal)this.WayCharge.TotalPrice,
                //        }, item => item.ID == this.WayCharge.ID);
                //    }
                //}
                //else if (count > 0)
                //{
                //    Reponsitory.Delete<Layers.Data.Sqls.PvCenter.WayCharges>(item => item.ID == waybill.ID);
                //}
            }
            #endregion
        }

        #endregion

        #region 持久化
        /// <summary>
        /// 运单持久化
        /// </summary>
        public virtual void Enter()
        {
            using (PvCenterReponsitory Reponsitory = LinqFactory<PvCenterReponsitory>.Create())
            {
                #region 保存交货人、收货人
                if (this.Consignor != null)
                {
                    if (!Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayParters>().Any(item => item.ID == this.Consignor.ID))
                    {
                        Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayParters
                        {
                            ID = this.Consignor.ID,
                            Company = this.Consignor.Company,
                            Place = this.Consignor.Place ?? string.Empty,
                            Address = this.Consignor.Address ?? string.Empty,
                            Contact = this.Consignor.Contact ?? string.Empty,
                            Phone = this.Consignor.Phone ?? string.Empty,
                            Zipcode = this.Consignor.Zipcode ?? string.Empty,
                            Email = this.Consignor.Email ?? string.Empty,
                            CreateDate = DateTime.Now,
                            IDType = (int?)this.Consignor.IDType,
                            IDNumber = this.Consignor.IDNumber,
                        });
                    }
                    else
                    {
                        Reponsitory.Update<Layers.Data.Sqls.PvCenter.WayParters>(new
                        {
                            Company = this.Consignor.Company,
                            Place = this.Consignor.Place ?? string.Empty,
                            Address = this.Consignor.Address ?? string.Empty,
                            Contact = this.Consignor.Contact ?? string.Empty,
                            Phone = this.Consignor.Phone ?? string.Empty,
                            Zipcode = this.Consignor.Zipcode ?? string.Empty,
                            Email = this.Consignor.Email ?? string.Empty,
                            CreateDate = DateTime.Now,
                            IDType = (int?)this.Consignor.IDType,
                            IDNumber = this.Consignor.IDNumber,
                        }, item => item.ID == this.Consignor.ID);
                    }
                }

                if (!Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayParters>().Any(item => item.ID == this.Consignee.ID))
                {
                    Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayParters
                    {
                        ID = this.Consignee.ID,
                        Company = this.Consignee.Company,
                        Place = this.Consignee.Place ?? string.Empty,
                        Address = this.Consignee.Address ?? string.Empty,
                        Contact = this.Consignee.Contact ?? string.Empty,
                        Phone = this.Consignee.Phone ?? string.Empty,
                        Zipcode = this.Consignee.Zipcode ?? string.Empty,
                        Email = this.Consignee.Email ?? string.Empty,
                        CreateDate = DateTime.Now,
                        IDType = (int?)this.Consignee.IDType,
                        IDNumber = this.Consignee.IDNumber,
                    });
                }
                else
                {
                    Reponsitory.Update<Layers.Data.Sqls.PvCenter.WayParters>(new
                    {
                        Company = this.Consignee.Company,
                        Place = this.Consignee.Place ?? string.Empty,
                        Address = this.Consignee.Address ?? string.Empty,
                        Contact = this.Consignee.Contact ?? string.Empty,
                        Phone = this.Consignee.Phone ?? string.Empty,
                        Zipcode = this.Consignee.Zipcode ?? string.Empty,
                        Email = this.Consignee.Email ?? string.Empty,
                        CreateDate = DateTime.Now,
                        IDType = (int?)this.Consignee.IDType,
                        IDNumber = this.Consignee.IDNumber,
                    }, item => item.ID == this.Consignee.ID);
                }
                this.ConsignorID = this.Consignor?.ID;
                this.ConsigneeID = this.Consignee?.ID;
                #endregion

                #region 保存运单
                if (!Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.Waybills>().Any(item => item.ID == this.ID))
                {
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
                        CarrierAccount = this.CarrierAccount,
                        VoyageNumber = this.VoyageNumber,
                        Condition = this.Condition ?? string.Empty,
                        CreateDate = this.CreateDate,
                        ModifyDate = this.ModifyDate,
                        EnterCode = this.EnterCode ?? string.Empty,
                        Status = (int)this.Status,
                        CreatorID = this.CreatorID,
                        ModifierID = this.ModifierID,
                        TransferID = this.TransferID,
                        IsClearance = this.IsClearance,
                        Packaging = this.Packaging,
                        Supplier = this.Supplier,
                        ExcuteStatus = this.ExcuteStatus,
                        NoticeType = (int)this.NoticeType,
                        Source = (int)this.Source,
                        OrderID = this.OrderID,
                    });
                }
                else
                {
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
                        Condition = this.Condition ?? string.Empty,
                        CreateDate = this.CreateDate,
                        ModifyDate = DateTime.Now,
                        EnterCode = this.EnterCode ?? string.Empty,
                        Status = (int)this.Status,
                        CreatorID = this.CreatorID,
                        ModifierID = this.ModifierID,
                        TransferID = this.TransferID,
                        IsClearance = this.IsClearance,
                        Packaging = this.Packaging,
                        Supplier = this.Supplier,
                        ExcuteStatus = this.ExcuteStatus,
                        NoticeType = (int)this.NoticeType,
                        Source = (int)this.Source,
                        OrderID = this.OrderID,
                    }, item => item.ID == this.ID);
                }
                #endregion

                #region 保存运单状态
                this.StatusLogEnter(Reponsitory);
                #endregion
            }
            this.OnEnterSuccess();
        }

        public virtual void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 运单状态保存
        /// </summary>
        private void StatusLogEnter(PvCenterReponsitory reponsitory)
        {
            int count = reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.Logs_Waybills>().Count(item => item.MainID == this.ID && item.Status == this.ExcuteStatus);
            if (count > 0)
            {
                reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_Waybills>(new
                {
                    IsCurrent = false,
                    CreatorID = this.CreatorID
                }, item => item.MainID == this.ID && item.Type == (int)OrderStatusType.MainStatus && item.Status == this.ExcuteStatus);
            }
            reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_Waybills()
            {
                ID = Guid.NewGuid().ToString(),
                MainID = this.ID,
                Type = 0,
                Status = (int)this.ExcuteStatus,
                CreateDate = DateTime.Now,
                CreatorID = this.CreatorID,
                IsCurrent = true,
            });
        }

        /// <summary>
        /// 确认收货
        /// </summary>
        public void ConfirmReceipt()
        {
            using (PvCenterReponsitory Reponsitory = LinqFactory<PvCenterReponsitory>.Create())
            {
                Reponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    ConfirmReceiptStatus = (int)ClientViews.ConfirmReceiptStatus.Confirmed,
                    ModifyDate = DateTime.Now,
                }, item => item.ID == this.ID);
            }
        }
        #endregion
    }
}
