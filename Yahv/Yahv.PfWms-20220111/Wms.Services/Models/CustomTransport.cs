using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Models;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Wms.Services.Models
{
    public class DataCustomTransport : WayChcd
    {
        /// <summary>
        /// 运单编号
        /// </summary>
        internal string WaybillID { get; set; }

        /// <summary>
        /// 运输类型
        /// </summary>
        public WaybillType WaybillType { get; set; }

        public string WaybillTypeDes
        {
            get
            {
                return this.WaybillType.GetDescription();
            }
        }

        /// <summary>
        /// 截单状态
        /// </summary>
        public CgCuttingOrderStatus? CuttingOrderStatus { get; set; }

        public string CuttingOrderStatusDes
        {
            get
            {
                if (CuttingOrderStatus != null)
                {
                    return this.CuttingOrderStatus.GetDescription();
                }
                else
                {
                    return Yahv.Underly.CgCuttingOrderStatus.Waiting.GetDescription();
                }
            }
        }

        /// <summary>
        /// 承运商编号
        /// </summary>
        public string CarrierID { get; set; }

        /// <summary>
        /// 承运商名字
        /// </summary>
        public string CarrierName { get; set; }

        public GeneralStatus Status { get; set; }

        public PickingExcuteStatus PickingExcuteStatus { get; set; }
        public string PickingExcuteStatusDes
        {
            get
            {
                return this.PickingExcuteStatus.GetDescription();
            }
        }

        public Currency Currency { get; set; }

        public string CurrencyDes
        {
            get
            {
                return this.Currency.GetDescription();
            }
        }
        public string CurrencySymbol
        {
            get
            {
                return this.Currency.GetCurrency().ShortSymbol;
            }
        }


        /// <summary>
        /// 总金额
        /// </summary>
        public decimal? TotalMoney { get; set; }

        /// <summary>
        /// 总毛重
        /// </summary>
        public decimal? TotalWeight { get; set; }

        /// <summary>
        /// 总箱数
        /// </summary>
        public int BoxNumber { get; set; }

        /// <summary>
        /// 已上架箱数
        /// </summary>
        public int UpperNumber { get; set; }

        //public Yahv.Services.Models.PickingNotice[] Notices { get; set; }
        public BoxesPickingNotices[] Notices { get; set; }

        public Yahv.Services.Models.SortingNotice[] SortingNotices { get; set; }

        public static implicit operator CustomTransport(DataCustomTransport entity)
        {
            return new CustomTransport
            {
                WaybillType = entity.WaybillType,
                CarrierID = entity.CarrierID,
                CarrierName = entity.CarrierName,
                Status = entity.Status,
                CuttingOrderStatus = entity.CuttingOrderStatus,
                LotNumber = entity.LotNumber,
                CarNumber1 = entity.CarNumber1,
                CarNumber2 = entity.CarNumber2,
                Carload = entity.Carload,
                IsOnevehicle = entity.IsOnevehicle,
                Driver = entity.Driver,
                Phone = entity.Phone,
                PlanDate = entity.PlanDate,
                DepartDate = entity.DepartDate,
                TotalQuantity = entity.TotalQuantity,
                Notices = entity.Notices,
                TotalMoney = entity.TotalMoney,
                TotlaWeight = entity.TotalWeight,
                BoxNumber = entity.BoxNumber
            };
        }
    }
    public class CustomTransport : WayChcd
    {
        /// <summary>
        /// 运输类型
        /// </summary>
        public WaybillType WaybillType { get; set; }

        /// <summary>
        /// 截单状态
        /// </summary>
        public CgCuttingOrderStatus? CuttingOrderStatus { get; set; }

        public string CuttingOrderStatusDes
        {
            get
            {
                if (CuttingOrderStatus != null)
                {
                    return this.CuttingOrderStatus.GetDescription();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 承运商编号
        /// </summary>
        public string CarrierID { get; set; }

        /// <summary>
        /// 承运商名字
        /// </summary>
        public string CarrierName { get; set; }

        public GeneralStatus Status { get; set; }
        public ExcuteStatus ExcuteStatus { get; set; }
        public string ExcuteStatusDes
        {
            get
            {
                return this.ExcuteStatus.GetDescription();
            }
        }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal? TotalMoney { get; set; }

        /// <summary>
        /// 总毛重
        /// </summary>
        public decimal? TotlaWeight { get; set; }

        /// <summary>
        /// 总箱数
        /// </summary>
        public int BoxNumber { get; set; }
        //public Yahv.Services.Models.PickingNotice[] Notices { get; set; }
        public BoxesPickingNotices[] Notices { get; set; }

    }

}
