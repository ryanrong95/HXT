using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Underly.Erps;

namespace Yahv.Services.Models
{
    public class Waybills : Waybill
    {
        public Waybills()
        {

        }

        IErpAdmin admin;
        public Waybills(IErpAdmin admin)
        {
            this.admin = admin;
        }
        public void Enter()
        {
            using (PvCenterReponsitory Reponsitory = new PvCenterReponsitory())
            {
                #region 保存交货人、收货人
                var count = 0;
                if (this.Consignor != null)
                {
                    count = Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayParters>().Count(item => item.ID == this.Consignor.ID);
                    if (count == 0)
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
                    else if (this.Consignor != null && count > 0)
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

                if (this.Consignee != null)
                {

                    count = Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayParters>().Count(item => item.ID == this.Consignee.ID);
                    if (this.Consignee != null && count == 0)
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
                    else if (this.Consignee != null && count > 0)
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
                }

                if (string.IsNullOrEmpty(this.ConsignorID))
                {
                    this.ConsignorID = this.Consignor?.ID;
                }
                if (string.IsNullOrEmpty(this.ConsigneeID))
                {
                    this.ConsigneeID = this.Consignee?.ID;
                }
                #endregion

                #region 保存运单
                count = Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.Waybills>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    this.ID = Layers.Data.PKeySigner.Pick(PKeyType.Waybill);
                    Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Waybills
                    {
                        ID = this.ID,
                        Code = this.Code,
                        FatherID = this.FatherID,
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
                        Summary = this.Summary,
                        TotalVolume = this.TotalVolume,
                        CuttingOrderStatus=this.CuttingOrderStatus,
                        //InitExType=this.InitExType,
                        //InitExPayType=this.InitExPayType,
                        
           

                    });
                }
                else
                {
                    Reponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                    {
                        Code = this.Code,
                        FatherID = this.FatherID,
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
                        this.Summary,
                    }, item => item.ID == this.ID);
                }
                #endregion

                #region 保存运单状态
                count = Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.Logs_Waybills>().Count(item => item.MainID == this.ID && item.Status == this.ExcuteStatus);
                if (count > 0)
                {
                    Reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_Waybills>(new
                    {
                        IsCurrent = false,
                        CreatorID = this.CreatorID
                    }, item => item.MainID == this.ID && item.Type == (int)OrderStatusType.MainStatus && item.Status == this.ExcuteStatus);
                }
                Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_Waybills()
                {
                    ID = Guid.NewGuid().ToString(),
                    MainID = this.ID,
                    Type = 0,
                    Status = (int)this.ExcuteStatus,
                    CreateDate = DateTime.Now,
                    CreatorID = this.CreatorID,
                    IsCurrent = true,
                });
                #endregion




                //保存提货信息
                if (this.WayLoading != null)
                {
                    try
                    {
                        count = Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayLoadings>().Count(item => item.ID == this.ID);


                        if (count <= 0)
                        {
                            Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayLoadings
                            {
                                ID = this.ID,
                                TakingDate = this.WayLoading.TakingDate ?? DateTime.Now,
                                TakingAddress = this.WayLoading.TakingAddress ?? "",
                                TakingContact = this.WayLoading.TakingContact ?? "",
                                TakingPhone = this.WayLoading.TakingPhone ?? "",
                                CarNumber1 = this.WayLoading.CarNumber1,
                                Driver = this.WayLoading.Driver ?? "",
                                Carload = this.WayLoading.Carload ?? 0,
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                CreatorID = this.WayLoading.CreatorID ?? this.admin.ID,
                                ModifierID = this.WayLoading.ModifierID ?? this.admin.ID,
                            });
                        }
                        else
                        {

                            Reponsitory.Update<Layers.Data.Sqls.PvCenter.WayLoadings>(new
                            {
                                TakingDate = this.WayLoading.TakingDate ?? DateTime.Now,
                                TakingAddress = this.WayLoading.TakingAddress ?? "",
                                TakingContact = this.WayLoading.TakingContact ?? "",
                                TakingPhone = this.WayLoading.TakingPhone ?? "",
                                CarNumber1 = this.WayLoading.CarNumber1,
                                Driver = this.WayLoading.Driver ?? "",
                                Carload = this.WayLoading.Carload ?? 0,
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                CreatorID = this.WayLoading.CreatorID ?? this.admin.ID,
                                ModifierID = this.WayLoading.ModifierID ?? this.admin.ID,
                            }, item => item.ID == this.WayLoading.ID);
                        }
                    }
                    catch
                    { }

                }


                if (this.WayCharge != null)
                {
                    try
                    {
                        count = Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayCharges>().Count(item => item.ID == this.ID);
                        if (count <= 0)
                        {
                            Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayCharges
                            {
                                ID = this.ID,
                                PayMethod = (int)this.WayCharge.PayMethod,
                                Payer = (int)this.WayCharge.Payer,
                                Currency = (int)this.WayCharge.Currency,
                                TotalPrice = (decimal)this.WayCharge.TotalPrice,

                            });
                        }
                        else
                        {
                            Reponsitory.Update<Layers.Data.Sqls.PvCenter.WayCharges>(new
                            {
                                PayMethod = (int)this.WayCharge.PayMethod,
                                Payer = (int)this.WayCharge.Payer,
                                Currency = (int)this.WayCharge.Currency,
                                TotalPrice = (decimal)this.WayCharge.TotalPrice,
                            }, item => item.ID == this.ID);
                        }
                    }
                    catch
                    { }
                }


                //if (this.WayChcd != null)
                //{

                //    count = Reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayChcd>().Count(item => item.ID == this.ID);
                //    if (count <= 0)
                //    {
                //        Reponsitory.Insert(new Layers.Data.Sqls.PvCenter.WayChcd
                //        {
                //            ID = this.ID,
                //            LotNumber = this.WayChcd.LotNumber,
                //            CarNumber1 = this.WayChcd.CarNumber1,
                //            CarNumber2 = this.WayChcd.CarNumber2,
                //            Carload = this.WayChcd.Carload ?? 0,
                //            IsOnevehicle = this.WayChcd.IsOnevehicle ?? false,
                //            Driver = this.WayChcd.Driver,
                //            PlanDate = DateTime.Now,

                //        });
                //    }
                //    else
                //    {
                //        Reponsitory.Update(new Layers.Data.Sqls.PvCenter.WayChcd
                //        {
                //            LotNumber = this.WayChcd.LotNumber,
                //            CarNumber1 = this.WayChcd.CarNumber1,
                //            CarNumber2 = this.WayChcd.CarNumber2,
                //            Carload = this.WayChcd.Carload ?? 0,
                //            IsOnevehicle = this.WayChcd.IsOnevehicle ?? false,
                //            Driver = this.WayChcd.Driver,
                //            PlanDate = DateTime.Now,
                //        }, item => item.ID == this.ID);
                //    }
                //}
            }
        }
    }
}
