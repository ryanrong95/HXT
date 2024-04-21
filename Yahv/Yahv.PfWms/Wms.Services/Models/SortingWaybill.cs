using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wms.Services.Enums;
using Wms.Services.Views;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;

namespace Wms.Services.Models
{
    public class DataSortingWaybill : Wms.Services.Models.Waybills
    {
        public int? ExcuteStatus { get; set; }

        public NoticeSource Source { get; set; }

        public SortingNotice[] Notices { get; set; }
        
        /// <summary>
        /// 隐式转换到可显示列
        /// </summary>
        /// <param name="entity">源类型对象</param>
        public static implicit operator SortingWaybill(DataSortingWaybill entity)
        {
            return new SortingWaybill
            {
                ClientID = entity.ClientID,
                ClientName = entity.ClientName,
                ExcuteStatus = (SortingExcuteStatus)(entity.ExcuteStatus ?? 100),
                EnterCode = entity.EnterCode,
                CreateDate = entity.CreateDate,
                DataFiles = entity.DataFiles,
                Notices = entity.Notices?.Select(item => { item.CheckValue = string.Concat(item.Product.PartNumber, item.Product.Manufacturer, item.Product.PackageCase, item.Product.Packaging, item.DateCode); return item; }).ToArray(),
                WaybillType = entity.WaybillType,
                WaybillID = entity.WaybillID,
                Code = entity.Code,
                Supplier = entity.Supplier,
                CarrierID = entity.CarrierID,
                CarrierName = entity.CarrierName,
                Place = entity.Place,
                Condition = entity.Condition,
                ConsignorID = entity.ConsignorID,
                Packaging = entity.Packaging,
                TransferID = entity.TransferID,
                FatherID = entity.FatherID,
                WayCharge = entity.WayCharge,
                WayChcd = entity.WayChcd,
                Consignee = entity.Consignee,
                Consignor = entity.Consignor,
                WayLoading = entity.WayLoading,
                ConsigneeID = entity.ConsigneeID,
                Status = entity.Status,
                Summary = entity.Summary,
                TotalParts = entity.TotalParts,
                TotalVolume = entity.TotalVolume,
                TotalWeight = entity.TotalWeight,
                VoyageNumber = entity.VoyageNumber,
                Source = entity.Source
                
                

            };
        }
    }

    public class SortingWaybill : Wms.Services.Models.Waybills
    {

        /// <summary>
        /// 运单冗余执行状态
        /// </summary>
        /// <remarks>
        /// 冗余字段
        /// </remarks>
        public SortingExcuteStatus ExcuteStatus { get; set; }

        public string ExcuteStatusDescription
        {
            get
            {
                return this.ExcuteStatus.GetDescription();
            }
        }



        SortingNotice[] notices;

        public SortingNotice[] Notices
        {
            get { return this.notices; }
            set
            {
                this.notices = value;
                if (value != null)
                {
                    for (int index = 0; index < value.Length; index++)
                    {
                        this.notices[index].Waybill = this;
                    }
                }
            }
        }


        private CenterFileDescription[] files;
        public CenterFileDescription[] Files
        {
            get
            {
                if (files == null && this.DataFiles != null)
                {
                    files = this.DataFiles.Where(item => string.IsNullOrEmpty(item.NoticeID)).ToArray();
                }
                return files;
            }
            set { files = value; }
        }

        public string AdminID { get; set; }

        public string AdminName { get; set; }

        string orderID;

        public string OrderID
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(orderID))
                {
                    return this.orderID;
                }

                if (this.Notices != null)
                {
                    return string.Join(",", this.Notices.Select(item => item.Input?.OrderID).Distinct());
                }
                return this.orderID;
            }
            set
            {
                this.orderID = value;
            }
        }

        public string WaybillTypeDescription
        {
            get
            {
                return this.WaybillType.GetDescription();
            }
        }

        public NoticeSource Source { get; set; }

        public string SourceDescription
        {
            get
            {
                return this.Source.GetDescription();
            }
        }

        public string PlaceID
        {
            get
            {
                if (string.IsNullOrEmpty(this.Place))
                { return ""; }
                var index = (int)Enum.GetValues(typeof(Origin)).Cast<Origin>().Where(item => item.GetOrigin().Code == this.Place).FirstOrDefault();
                return index.ToString();
            }
        }

        public string PlaceDescription
        {
            get
            {
                if (string.IsNullOrEmpty(this.Place))
                { return ""; }
                var origins = Enum.GetValues(typeof(Origin)).Cast<Origin>().Select(item => item.GetOrigin());
                origins = origins.Where(item => item.Code == this.Place);
                if (origins.Count() == 0)
                {
                    try
                    {
                        return ((Origin)int.Parse(this.Place)).GetOrigin().ChineseName;
                    }
                    catch
                    {
                        return "";
                    }
                }

                var name = origins.FirstOrDefault().ChineseName;
                return name;
            }
        }

        public WayCondition Conditions
        {
            get
            {
                if (Condition != null)
                {
                    return this.Condition.JsonTo<WayCondition>();
                }
                else
                {

                    return null;
                }
            }
        }

    }

    public class SortingNotice
    {
        public string ID { get; set; }

        /// <summary>
        /// 和前端的约定
        /// </summary>
        public string PID { get; set; }
        public string ClientID { get; set; }
        public string WareHouseID { get; set; }
        public string WayBillID { get; set; }

        string productID;

        public string ProductID
        {
            get
            {
                return this.productID ?? this.Product?.ID;
            }
            set
            {
                this.productID = value;
            }
        }

        public string Supplier { get; set; }
        public NoticeType Type { get; set; }
        public string TypeDescription
        {
            get { return this.Type.GetDescription(); }
        }
        public NoticesStatus Status { get; set; }
        public string StatusDescription
        { get { return this.Status.GetDescription(); } }
        public NoticesTarget Target { get; set; }
        public string TargetDescription
        {
            get
            {
                return this.Target.GetDescription();
            }
        }

        internal SortingWaybill Waybill { get; set; }
        public string InputID { get; set; }
        public Input Input { get; set; }

        CenterProduct product;

        public CenterProduct Product
        {
            get { return this.product; }
            set
            {
                this.product = value;
                this.productID = value.ID;
            }
        }
        public string DateCode { get; set; }
        public decimal Quantity { get; set; }

        /// <summary>
        /// 本次
        /// </summary>
        public decimal? TruetoQuantity { get; set; }
        public decimal SortedQuantity { get; set; }
        public NoticeSource Source { get; set; }
        public string BoxCode { get; set; }
        public string ShelveID { get; set; }
        public decimal? Weight { get; set; }
        private decimal? netWeight;
        /// <summary>
        /// 净重(默认逻辑NetWeight = Weight * 0.7d)
        /// </summary>
        public decimal? NetWeight
        {
            get
            {

                if (this.netWeight == null)
                {
                    if (this.Weight == null)
                    {
                        this.netWeight = null;
                    }
                    else
                    {
                        this.netWeight = this.Weight * (decimal)0.7d;
                    }
                }
                return this.netWeight;
            }
            set
            {
                this.netWeight = value;

            }
        }

        public decimal? Volume { get; set; }
        public NoticeCondition Condition { get; set; }


        public Sorting Sorting { get; set; }

        /// <summary>
        /// 分拣历史
        /// </summary>
        public object Sorted { get; set; }
        public bool Enabled { get; set; } = true;
        public string CheckValue { get; set; }

        public bool IsChange
        {
            get
            {
                return this.CheckValue.MD5() != string.Concat(this.Product.PartNumber, this.Product.Manufacturer, this.Product.PackageCase, this.Product.Packaging, this.DateCode).MD5();
            }
        }

        internal Storage Storage { get; set; }

        private CenterFileDescription[] files;
        public CenterFileDescription[] Files
        {
            get
            {
                if (this.Waybill != null && this.Waybill.DataFiles != null && this.Waybill.Files.Length > 0)
                {
                    files = this.Waybill.DataFiles.Where(item => item.NoticeID == this.ID).ToArray();
                }
                if (files == null)
                {
                    files = new CenterFileDescription[] { };
                }
                return files;
            }
            set
            {
                files = value;
            }
        }

        public bool Visable { get; set; } = true;

        public bool IsOriginalNotice { get; set; } = true;


    }
}
