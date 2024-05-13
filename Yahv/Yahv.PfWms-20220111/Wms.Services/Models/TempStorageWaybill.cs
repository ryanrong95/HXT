using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Wms.Services.Models
{

    public class DataTempStorageWaybill : Wms.Services.Models.Waybills
    {
        public int? ExcuteStatus { get; set; }

        public TempStorage[] ProductStorages { get; set; }
        public TempStorage[] SummaryStorages { get; set; }

        //public string WaybillTypeDes
        //{
        //    get
        //    {
        //        return this.WaybillType.GetDescription();
        //    }
        //}

        public static implicit operator TempStorageWaybill(DataTempStorageWaybill entity)
        {

            return new TempStorageWaybill
            {
                ExcuteStatus = (TempStockExcuteStatus)entity.ExcuteStatus,
                EnterCode = entity.EnterCode,
                CreateDate = entity.CreateDate,
                DataFiles = entity.DataFiles,
                WaybillType = entity.WaybillType,
                WaybillID = entity.WaybillID,
                Code = entity.Code,
                Supplier = entity.Supplier,
                CarrierID = entity.CarrierID,
                CarrierName = entity.CarrierName,
                Place = entity.Place,
                Condition = entity.Condition,
                ConsignorID = entity.ConsignorID,
                Summary = entity.Summary,
                ProductStorages = entity.ProductStorages,
                SummaryStorages = entity.SummaryStorages,
                Consignor = entity.Consignor,
                TempEnterCode=entity.TempEnterCode//入库单号
            };
        }
    }


    public class TempStorage : Storage
    {
       
        /// <summary>
        /// 分拣信息
        /// </summary>
        internal Sorting Sorting { get; set; }
    }

    public class TempStorageWaybill : Wms.Services.Models.Waybills
    {

        ///// <summary>
        ///// 不为空存进库存信息（库存/分拣/运单/发货人信息），根据描述存储，和根据产品存储是并的关系
        ///// </summary>
        //public string Summary { get; set; }
        /// <summary>
        /// 执行状态
        /// </summary>
        public TempStockExcuteStatus ExcuteStatus { get; set; }

        public TempStorage[] ProductStorages { get; set; }
        public TempStorage[] SummaryStorages { get; set; }

        private CenterFileDescription[] files;
        public CenterFileDescription[] Files
        {
            get
            {
                if (/*files.Count() > 0 && */this.DataFiles != null)
                {
                    files = this.DataFiles/*.Where(item => item.WaybillID == WaybillID).ToArray()*/;
                }
                return files;
            }
            set
            {
                files = value;
            }
        }
    }
}
