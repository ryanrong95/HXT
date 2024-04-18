using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    public class CgStoragesTopView<TReponsitory> : UniqueView<StoreInventory, TReponsitory>
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public CgStoragesTopView()
        {
        }

        public CgStoragesTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected CgStoragesTopView(TReponsitory reponsitory, IQueryable<StoreInventory> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        protected override IQueryable<StoreInventory> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWms.CgStoragesTopView>()
                   where (entity.WareHouseID.StartsWith("HK") || (entity.WareHouseID.StartsWith("SZ") && entity.ShelveID != null))
                   && entity.Quantity > 0
                   orderby entity.CreateDate descending
                   select new StoreInventory
                   {
                       ID = entity.ID,
                       WareHouseID = entity.WareHouseID,
                       InputID = entity.InputID,
                       Total = entity.Total,
                       Quantity = entity.Quantity,
                       Origin = entity.Origin,
                       IsLock = entity.IsLock,
                       ShelveID = entity.ShelveID,
                       Supplier = entity.Supplier,
                       DateCode = entity.DateCode,
                       Summary = entity.Summary,
                       CustomsName = entity.CustomsName,
                       OrderID = entity.OrderID,
                       TinyOrderID = entity.TinyOrderID,
                       ItemID = entity.ItemID,
                       TrackerID = entity.TrackerID,
                       Currency = (Currency?)entity.Currency,
                       UnitPrice = entity.UnitPrice,
                       PartNumber = entity.PartNumber,
                       Manufacturer = entity.Manufacturer,
                       PackageCase = entity.PackageCase,
                       Packaging = entity.Packaging,
                       ProductID = entity.ProductID,
                       CreateDate = entity.CreateDate,
                       ClientID = entity.ClientID,
                       ClientName = entity.ClientName
                   };
        }

        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            var iquery = this.IQueryable.Cast<StoreInventory>();

            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            var ienums = iquery.ToArray().Select(entity => new
            {
                ID = entity.ID,
                WareHouseID = entity.WareHouseID,
                InputID = entity.InputID,
                Total = entity.Total.HasValue ? (decimal)entity.Total.Value : 0,
                Quantity = entity.Quantity,
                Origin = ((Origin)Enum.Parse(typeof(Origin), entity.Origin ?? nameof(Origin.Unknown))).GetDescription(),
                IsLock = entity.IsLock,
                ShelveID = entity.WareHouseID.StartsWith("SZ")? entity.ShelveID?.Substring(3): entity.ShelveID,
                Supplier = entity.Supplier,
                DateCode = entity.DateCode,
                Summary = entity.Summary,
                CustomsName = entity.CustomsName,
                OrderID = entity.OrderID,
                TinyOrderID = entity.TinyOrderID,
                ItemID = entity.ItemID,
                TrackerID = entity.TrackerID,
                Currency = entity.Currency.HasValue ? (Underly.Currency)entity.Currency.Value : Underly.Currency.Unknown,
                UnitPrice = entity.UnitPrice.HasValue ? (decimal)entity.UnitPrice.Value : 0,
                PartNumber = entity.PartNumber,
                Manufacturer = entity.Manufacturer,
                PackageCase = entity.PackageCase,
                Packaging = entity.Packaging,
                ProductID = entity.ProductID,
                CreateDate = entity.CreateDate,
                ClientID = entity.ClientID,
                ClientName = entity.ClientName
            });


            return new
            {
                Total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                Data = ienums.ToArray(),
            };
        }

        #region 搜索方法
        /// <summary>
        /// 根据型号搜索库存
        /// </summary>
        /// <param name="partNumber"></param>
        /// <returns></returns>
        public CgStoragesTopView<TReponsitory> SearchByPartNumber(string partNumber)
        {
            var storageview = this.IQueryable.Cast<StoreInventory>();

            var linq = from storage in storageview
                       where storage.PartNumber.Contains(partNumber)
                       select storage;

            var view = new CgStoragesTopView<TReponsitory>(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID
            };

            return view;
        }

        /// <summary>
        /// 根据品牌搜索库存
        /// </summary>
        /// <param name="manufacturer"></param>
        /// <returns></returns>
        public CgStoragesTopView<TReponsitory> SearchByManufacturer(string manufacturer)
        {
            var storageview = this.IQueryable.Cast<StoreInventory>();

            var linq = from storage in storageview
                       where storage.Manufacturer == manufacturer
                       select storage;

            var view = new CgStoragesTopView<TReponsitory>(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID
            };

            return view;
        }

        /// <summary>
        /// 根据货架ID搜索库存
        /// </summary>
        /// <param name="shelveID"></param>
        /// <returns></returns>
        public CgStoragesTopView<TReponsitory> SearchByShelveID(string shelveID)
        {
            var storageview = this.IQueryable.Cast<StoreInventory>();

            var linq = from storage in storageview
                       where storage.ShelveID == shelveID
                       select storage;
            var view = new CgStoragesTopView<TReponsitory>(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        /// <summary>
        /// 根据进入库存的起止时间搜索库存
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public CgStoragesTopView<TReponsitory> SearchByDate(DateTime? starttime, DateTime? endtime)
        {
            Expression<Func<StoreInventory, bool>> predicate = store => (starttime == null ? true : store.CreateDate >= starttime)
                && (endtime == null ? true : store.CreateDate < endtime.Value.AddDays(1));

            var storageview = this.IQueryable.Cast<StoreInventory>();

            var linq = storageview.Where(predicate);

            var view = new CgStoragesTopView<TReponsitory>(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        string wareHouseID;
        /// <summary>
        /// 根据WareHouseID 搜索库存
        /// </summary>
        /// <param name="wareHouseID"></param>
        /// <returns></returns>
        public CgStoragesTopView<TReponsitory> SearchByWareHouseID(string wareHouseID)
        {
            this.wareHouseID = wareHouseID;

            var storageview = this.IQueryable.Cast<StoreInventory>();

            if (string.IsNullOrEmpty(this.wareHouseID))
            {
                return new CgStoragesTopView<TReponsitory>(this.Reponsitory, storageview);
            }

            var linq = from storage in storageview
                       where storage.WareHouseID.StartsWith(this.wareHouseID)
                       select storage;

            var view = new CgStoragesTopView<TReponsitory>(this.Reponsitory, linq)
            {
                wareHouseID = this.wareHouseID,
            };

            return view;
        }

        #endregion
    }
}
