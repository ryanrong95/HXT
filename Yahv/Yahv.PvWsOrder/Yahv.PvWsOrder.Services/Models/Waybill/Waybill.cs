using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 运单
    /// </summary>
    public class Waybill : Yahv.Services.Models.Waybill
    {
        #region 扩展属性

        /// <summary>
        /// 操作人
        /// </summary>
        public string OperatorID { get; internal set; }

        public WayCondition WayCondition
        {
            get
            {
                return this.Condition.JsonTo<WayCondition>();
            }
            set
            {
                this.Condition = value.Json();
            }
        }

        public string PackageDec
        {
            get
            {
                if (string.IsNullOrEmpty(this.Packaging))
                {
                    return Package.纸制或纤维板制盒.GetDescription();
                }
                else
                {
                    return ((Package)Enum.Parse(typeof(Package), this.Packaging)).GetDescription();
                }
            }
        }

        public string WaybillLoadStatus
        {
            get
            {
                return this.WayLoading.LoadingExcuteStatus == null ? "--" : this.WayLoading.LoadingExcuteStatus.GetDescription();
            }
        }

        #endregion

        public Waybill()
        {
            this.EnterSuccess += Waybill_EnterSuccess;
            this.AbandonSuccess += Waybill_AbandonSuccess;
        }

        #region 事件

        public virtual event SuccessHanlder EnterSuccess;
        public virtual event SuccessHanlder AbandonSuccess;

        public virtual void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }
        public virtual void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }

        private void Waybill_EnterSuccess(object sender, SuccessEventArgs e)
        {
            var waybill = (Waybill)e.Object;
            //保存运单状态日志
            SaveLogs_Waybills(waybill);
            //保存提货信息
            SaveWayLoading(waybill);
            //保存货物条款
            SaveWayCharge(waybill);
        }
        private void Waybill_AbandonSuccess(object sender, SuccessEventArgs e)
        {
            var waybill = (Waybill)e.Object;
            //保存运单状态日志
            SaveLogs_Waybills(waybill);
        }

        #endregion

        #region 持久化

        public virtual void Enter()
        {
            using (Layers.Data.Sqls.PvCenterReponsitory Reponsitory = new Layers.Data.Sqls.PvCenterReponsitory())
            {
                #region 保存交货人、收货人
                if (this.Consignor != null && Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayParters>().Count(item => item.ID == this.Consignor.ID) == 0)
                {
                    Reponsitory.Insert(this.Consignor.ToLinq());
                }
                if (this.Consignee != null && Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayParters>().Count(item => item.ID == this.Consignee.ID) == 0)
                {
                    Reponsitory.Insert(this.Consignee.ToLinq());
                }
                this.ConsignorID = this.Consignor?.ID;
                this.ConsigneeID = this.Consignee?.ID;
                #endregion

                #region 保存运单
                int count = Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.Waybills>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    this.ID = Layers.Data.PKeySigner.Pick(PKeyType.Waybill);
                    Reponsitory.Insert(this.ToLinq());
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
                        TotalVolume = this.TotalVolume,
                        CarrierAccount = this.CarrierAccount,
                        VoyageNumber = this.VoyageNumber,
                        Condition = this.Condition,
                        EnterCode = this.EnterCode,
                        Status = (int)this.Status,
                        ModifierID = this.ModifierID,
                        TransferID = this.TransferID,
                        IsClearance = this.IsClearance,
                        ExcuteStatus = this.ExcuteStatus,
                        OrderID = this.OrderID,
                        Source = (int)this.Source,
                        NoticeType = (int)this.NoticeType,
                        ModifyDate = DateTime.Now,
                    }, item => item.ID == this.ID);
                }
                #endregion
            }
            this.OnEnterSuccess();
        }

        public virtual void Abandon()
        {
            using (Layers.Data.Sqls.PvCenterReponsitory Reponsitory = new Layers.Data.Sqls.PvCenterReponsitory())
            {
                Reponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    ModifyDate = DateTime.Now,
                    Status = (int)GeneralStatus.Closed,
                    ModifierID = this.OperatorID,
                    ExcuteStatus = this.ExcuteStatus,
                }, item => item.ID == this.ID);
            }
            this.OnAbandonSuccess();
        }

        /// <summary>
        /// 仅仅修改运单的执行状态
        /// </summary>
        public void UpdateStatus()
        {
            using (Layers.Data.Sqls.PvCenterReponsitory Reponsitory = new Layers.Data.Sqls.PvCenterReponsitory())
            {
                Reponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                {
                    ModifyDate = DateTime.Now,
                    ModifierID = this.OperatorID,
                    ExcuteStatus = this.ExcuteStatus,
                }, item => item.ID == this.ID);
            }
            //保存运单状态日志
            SaveLogs_Waybills(this);
        }

        /// <summary>
        /// 保存提货信息
        /// </summary>
        /// <param name="waybill"></param>
        private void SaveWayLoading(Waybill waybill)
        {
            using (Layers.Data.Sqls.PvCenterReponsitory Reponsitory = new Layers.Data.Sqls.PvCenterReponsitory())
            {
                var wayload = waybill.WayLoading;
                if (wayload == null)
                {
                    //删除提货信息
                    Reponsitory.Delete<Layers.Data.Sqls.PvCenter.WayLoadings>(item => item.ID == waybill.ID);
                }
                else
                {
                    if (waybill.Type == WaybillType.PickUp)
                    {
                        wayload.ID = waybill.ID;
                        var count = Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayLoadings>().Count(item => item.ID == wayload.ID);
                        if (count == 0)
                        {
                            Reponsitory.Insert(wayload.ToLinq());
                        }
                        else
                        {
                            Reponsitory.Update<Layers.Data.Sqls.PvCenter.WayLoadings>(new
                            {
                                TakingDate = (DateTime)wayload.TakingDate,
                                TakingAddress = wayload.TakingAddress,
                                TakingContact = wayload.TakingContact,
                                TakingPhone = wayload.TakingPhone,
                                CarNumber1 = wayload.CarNumber1,
                                Driver = wayload.Driver,
                                Carload = wayload.Carload,
                                ModifyDate = DateTime.Now,
                                ModifierID = waybill.OperatorID,
                                ExcuteStatus = (int)wayload.LoadingExcuteStatus,
                            }, item => item.ID == waybill.ID);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 保存货物条款
        /// </summary>
        /// <param name="waybill"></param>
        private void SaveWayCharge(Waybill waybill)
        {
            using (Layers.Data.Sqls.PvCenterReponsitory Reponsitory = new Layers.Data.Sqls.PvCenterReponsitory())
            {
                var wayCharge = waybill.WayCharge;
                if (wayCharge == null)
                {
                    //删除货物条款
                    Reponsitory.Delete<Layers.Data.Sqls.PvCenter.WayCharges>(item => item.ID == waybill.ID);
                }
                else
                {
                    var count = Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayCharges>().Count(item => item.ID == wayCharge.ID);
                    if (count == 0)
                    {
                        wayCharge.ID = waybill.ID;
                        Reponsitory.Insert(wayCharge.ToLinq());
                    }
                    else
                    {
                        Reponsitory.Update<Layers.Data.Sqls.PvCenter.WayCharges>(new
                        {
                            Payer = (int)wayCharge.Payer,
                            PayMethod = (int)wayCharge.PayMethod,
                            Currency = wayCharge.Currency,
                            TotalPrice = wayCharge.TotalPrice,
                        }, item => item.ID == waybill.ID);
                    }
                }
            }
        }

        /// <summary>
        /// 保存状态日志
        /// </summary>
        /// <param name="waybill"></param>
        private void SaveLogs_Waybills(Waybill waybill)
        {
            Logs_Waybills log = new Logs_Waybills();
            log.MainID = waybill.ID;
            log.Type = 3;
            log.CreateDate = DateTime.Now;
            log.CreatorID = waybill.OperatorID;
            log.Status = waybill.ExcuteStatus.GetValueOrDefault();
            log.IsCurrent = true;
            log.Enter();
        }

        #endregion
    }
}
