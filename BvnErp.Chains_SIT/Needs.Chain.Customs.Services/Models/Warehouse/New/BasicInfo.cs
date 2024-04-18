using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class BasicInfo
    {
        public string EntryNoticeID { get; set; }
        public DateTime OrderCreateDate { get; set; }
        public EntryNoticeStatus EntryNoticeStatus { get; set; }
        public string EntryNoticeStatusName
        {
            get
            {
                return EntryNoticeStatus.GetDescription();
            }
        }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public ClientRank ClientRank { get; set; }
        public int ClientRankCode
        {
            get
            {
                return (int)ClientRank;
            }
        }
        public string OrderID { get; set; }
        public string MainOrderID { get; set; }
        public string ClientSupplierName { get; set; }
        public SupplierGrade ClientSupplierGrade { get; set; }
        public int ClientSupplierGradeCode
        {
            get
            {
                return (int)ClientSupplierGrade;
            }
        }

        public HKDeliveryType HKDeliveryType { get; set; }
        public DeliveryNoticeStatus DeliveryNoticeStatus { get; set; }
        public string HKDeliveryTypeName
        {
            get
            {
                return HKDeliveryType.GetDescription();
            }
        }
        public Admin Merchandiser { get; set; }
        public string MerchandiserName
        {
            get
            {
                return Merchandiser?.RealName;
            }
        }
        public String PickUpTime { get; set; }
        public string PickUpAddress { get; set; }
        public string PickUpContactName { get; set; }
        public string PickUpContactPhone { get; set; }
        public string CarrierID { get; set; }
        public string DriverID { get; set; }
        public string CarNumber { get; set; }
        public string WayBillCode { get; set; }
        public List<PickupFile> PickupFiles { get; set; }
        public List<PickupFile> StorageFiles { get; set; }
        public ChargeWHType ChargeWH { get; set; }

        public string ChargeType { get; set; }

        public decimal? AmountWH { get; set; }
    }

    public class PickupFile
    {
        public string ID { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public int FileType { get; set; }
    }
}
