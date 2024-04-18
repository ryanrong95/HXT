using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.Models;

namespace Yahv.PvWsOrder.Services.Extends
{
    public static class WaybillExtends
    {
        /// <summary>
        /// Waybill To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvCenter.Waybills ToLinq(this Waybill entity)
        {
            return new Layers.Data.Sqls.PvCenter.Waybills
            {
                ID = entity.ID,
                Code = entity.Code,
                Type = (int)entity.Type,
                Subcodes = entity.Subcodes,
                CarrierID = entity.CarrierID,
                ConsignorID = entity.ConsignorID,
                ConsigneeID = entity.ConsigneeID,
                FreightPayer = (int)entity.FreightPayer,
                TotalParts = entity.TotalParts,
                TotalWeight = entity.TotalWeight,
                TotalVolume = entity.TotalVolume,
                CarrierAccount = entity.CarrierAccount,
                VoyageNumber = entity.VoyageNumber,
                Condition = entity.Condition,
                CreateDate = entity.CreateDate,
                ModifyDate = entity.ModifyDate,
                EnterCode = entity.EnterCode,
                Status = (int)entity.Status,
                CreatorID = entity.CreatorID,
                ModifierID = entity.ModifierID,
                TransferID = entity.TransferID,
                IsClearance = entity.IsClearance,
                Packaging = entity.Packaging,
                FatherID = entity.FatherID,
                Supplier = entity.Supplier,
                ExcuteStatus = entity.ExcuteStatus,
                OrderID=entity.OrderID,
                Source=(int)entity.Source,
                NoticeType=(int)entity.NoticeType,
            };
        }

        /// <summary>
        /// WayChargos To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvCenter.WayCharges ToLinq(this Yahv.Services.Models.WayCharge entity)
        {
            return new Layers.Data.Sqls.PvCenter.WayCharges
            {
                ID = entity.ID,
                Payer = (int)entity.Payer,
                PayMethod = (int)entity.PayMethod,
                Currency = (int)entity.Currency,
                TotalPrice = (decimal)entity.TotalPrice,
            };
        }

        /// <summary>
        /// WayChcd To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvCenter.WayChcd ToLinq(this Yahv.Services.Models.WayChcd entity)
        {
            return new Layers.Data.Sqls.PvCenter.WayChcd
            {
                ID = entity.ID,
                LotNumber = entity.LotNumber,
                CarNumber1 = entity.CarNumber1,
                CarNumber2 = entity.CarNumber2,
                Carload = (int)entity.Carload,
                IsOnevehicle = (bool)entity.IsOnevehicle,
                Driver = entity.Driver,
                PlanDate = (DateTime)entity.PlanDate,
                DepartDate = (DateTime)entity.DepartDate,
                TotalQuantity = (int)entity.TotalQuantity,
            };
        }

        /// <summary>
        /// WayLoadings To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvCenter.WayLoadings ToLinq(this Yahv.Services.Models.WayLoading entity)
        {
            return new Layers.Data.Sqls.PvCenter.WayLoadings
            {
                ID = entity.ID,
                TakingDate = (DateTime)entity.TakingDate,
                TakingAddress = entity.TakingAddress != null ? entity.TakingAddress : "",
                TakingContact = entity.TakingContact != null ? entity.TakingContact : "",
                TakingPhone = entity.TakingPhone != null ? entity.TakingPhone : "",
                CarNumber1 = entity.CarNumber1 != null ? entity.CarNumber1 : "",
                Driver = entity.Driver != null ? entity.Driver : "",
                Carload = entity.Carload,
                CreateDate = (DateTime)entity.CreateDate,
                ModifyDate = (DateTime)entity.ModifyDate,
                CreatorID = entity.CreatorID != null ? entity.CreatorID : "",
                ModifierID = entity.ModifierID != null ? entity.ModifierID : "",
                ExcuteStatus = (int)entity.LoadingExcuteStatus,
            };
        }

        /// <summary>
        /// WayParters To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvCenter.WayParters ToLinq(this Yahv.Services.Models.WayParter entity)
        {
            return new Layers.Data.Sqls.PvCenter.WayParters
            {
                ID = entity.ID,
                Company = entity.Company != null ? entity.Company : "",
                Place = entity.Place != null ? entity.Place : "",
                Address = entity.Address != null ? entity.Address : "",
                Contact = entity.Contact != null ? entity.Contact : "",
                Phone = entity.Phone != null ? entity.Phone : "",
                Zipcode = entity.Zipcode != null ? entity.Zipcode : "",
                Email = entity.Email != null ? entity.Email : "",
                CreateDate = (DateTime)entity.CreateDate,
                IDType = entity.IDType != null ? (int)entity.IDType : 1,
                IDNumber = entity.IDNumber,
            };
        }

        /// <summary>
        /// WayCosts To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvCenter.WayCosts ToLinq(this Yahv.Services.Models.WayCost entity)
        {
            return new Layers.Data.Sqls.PvCenter.WayCosts
            {
                ID = entity.ID,
                WaybillID = entity.WaybillID,
                Name = entity.Name,
                Currency = (int)entity.Currency,
                Price = entity.Price,
                CreateDate = entity.CreateDate,
                CreatorID = entity.CreatorID,
            };
        }
    }
}
