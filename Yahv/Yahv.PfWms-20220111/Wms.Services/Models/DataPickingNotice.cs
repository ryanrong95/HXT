using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Models;

namespace Wms.Services.Models
{
    public class DataPickingNotice : Notice
    {
      
        public CenterProduct Product { get; set; }

        public Picking Picking { get; set; }

        public Yahv.Services.Models.Output Output { get; set; }

        /// <summary>
        /// 总件数
        /// </summary>
        public decimal TotalPieces { get; set; }

        public Waybill Waybill { get; set; }
        public int? ExcuteStatus { get; set; }

        public static implicit operator PickingNotice(DataPickingNotice entity)
        {
            return new PickingNotice
            {
                ExcuteStatus = (Yahv.Underly.CgPickingExcuteStatus)entity.ExcuteStatus,
                BoxCode = entity.BoxCode,
                Checked = entity.Checked,
                Conditions = entity.Conditions,
                CreateDate = entity.CreateDate,
                DateCode = entity.DateCode,
                Files = entity.Files,
                ID = entity.ID,
                InputID = entity.InputID,
                NetWeight = entity.NetWeight,
                Output = entity.Output,
                OutputID = entity.OutputID,
                Picking = entity.Picking,
                Product = entity.Product,
                ProductID = entity.ProductID,
                Quantity = entity.Quantity,
                ShelveID = entity.ShelveID,
                Source = entity.Source,
                Status = entity.Status,
                StockQuantity = entity.StockQuantity,
                Supplier = entity.Supplier,
                Target = entity.Target,
                TotalPieces = entity.TotalPieces,
                Type = entity.Type,
                Visable = entity.Visable,
                Volume = entity.Volume,
                WareHouseID = entity.WareHouseID,
                WaybillID = entity.WaybillID,
                Weight = entity.Weight
            };
        }
    }
}
