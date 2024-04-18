using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Underly;

namespace Needs.Ccs.Services.Views
{
    public class DeliveriesTopView : UniqueView<DeliveriesTopModel, ScCustomsReponsitory>
    {
        protected override IQueryable<DeliveriesTopModel> GetIQueryable()
        {
            var deliveriesTopViews = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeliveriesTopView>();

            var linq = from deliveriesTopView in deliveriesTopViews
                       select new DeliveriesTopModel
                       {
                           ID = deliveriesTopView.stoID,
                           StoInputID = deliveriesTopView.stoInputID,
                           StoProductID = deliveriesTopView.stoProductID,
                           IptOrderID = deliveriesTopView.iptOrderID,
                           IptTinyOrderID = deliveriesTopView.iptTinyOrderID,
                           IptItemID = deliveriesTopView.iptItemID,
                           PtvPartNumber = deliveriesTopView.ptvPartNumber,
                           PtvManufacturer = deliveriesTopView.ptvManufacturer,
                           IptOrigin = deliveriesTopView.iptOrigin,
                           IptDateCode = deliveriesTopView.iptDateCode,
                           StoQuantity = deliveriesTopView.stoQuantity,
                           WareHouseID = deliveriesTopView.WareHouseID,
                           SortBoxCode = deliveriesTopView.sortBoxCode,
                           SortWeight = deliveriesTopView.sortWeight,
                           SortVolume = deliveriesTopView.sortVolume,
                           //StoShelveID = deliveriesTopView.stoShelveID
                           UnitPrice = deliveriesTopView.UnitPrice,
                       };

            return linq;
        }

        public IEnumerable<DeliveriesTopModelOriginName> GetDataByOrderID(string orderID)
        {
            var deliveriesTopViews = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeliveriesTopView>();
            var adminsTopView2 = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView2>();

            var linq = from deliveriesTopView in deliveriesTopViews
                       join admin in adminsTopView2
                            on new { AdminID = deliveriesTopView.sortAdminID, AdminDataStatus = (int)Enums.Status.Normal, }
                            equals new { AdminID = admin.ID, AdminDataStatus = admin.Status, }
                            into adminsTopView222
                       from admin in adminsTopView222.DefaultIfEmpty()
                       where deliveriesTopView.iptOrderID == orderID
                       select new DeliveriesTopModel
                       {
                           IptOrderID = deliveriesTopView.iptOrderID,
                           IptTinyOrderID = deliveriesTopView.iptTinyOrderID,
                           IptItemID = deliveriesTopView.iptItemID,
                           PtvPartNumber = deliveriesTopView.ptvPartNumber,
                           PtvManufacturer = deliveriesTopView.ptvManufacturer,
                           IptOrigin = deliveriesTopView.iptOrigin,
                           IptDateCode = deliveriesTopView.iptDateCode,
                           StoQuantity = deliveriesTopView.stoQuantity,
                           SorterAdmin = new Admin()
                           {
                               ID = admin.OriginID,
                               UserName = admin.UserName,
                               RealName = admin.RealName,
                           },
                           StoInputID = deliveriesTopView.stoInputID,
                       };

            List<DeliveriesTopModel> deliveriesTopModels = linq.ToList();

            return from deliveriesTopModel in deliveriesTopModels
                   select new DeliveriesTopModelOriginName()
                   {
                       IptOrderID = deliveriesTopModel.IptOrderID,
                       IptTinyOrderID = deliveriesTopModel.IptTinyOrderID,
                       IptItemID = deliveriesTopModel.IptItemID,
                       PtvPartNumber = deliveriesTopModel.PtvPartNumber,
                       PtvManufacturer = deliveriesTopModel.PtvManufacturer,
                       IptOrigin = deliveriesTopModel.IptOrigin,
                       OriginCode = deliveriesTopModel.IptOrigin, //((Needs.Underly.Origin)Convert.ToInt32(deliveriesTopModel.IptOrigin)).GetOrigin().Code,
                       IptDateCode = deliveriesTopModel.IptDateCode,
                       StoQuantity = deliveriesTopModel.StoQuantity,
                       SorterAdmin = deliveriesTopModel.SorterAdmin,
                       StoInputID = deliveriesTopModel.StoInputID,
                   };
        }
    }

    public class DeliveriesTopModel : IUnique
    {

        public string ID { get; set; }
        public string StoSortingID { get; set; }
        public string StoInputID { get; set; }
        public string StoProductID { get; set; }
        public decimal StoQuantity { get; set; }
        public string SortBoxCode { get; set; }

        public string SortAdminID { get; set; }
        public string IptOrderID { get; set; }
        public string IptItemID { get; set; }

        public string IptProductID { get; set; }
        public string IptDateCode { get; set; }
        public string IptOrigin { get; set; }
        public string PtvPartNumber { get; set; }

        public string PtvManufacturer { get; set; }
        public string IptTinyOrderID { get; set; }
        public decimal? SortWeight { get; set; }
        public decimal? SortVolume { get; set; }
        public string OriginInputID { get; set; }

        public DateTime? SortingDate { get; set; }
        public decimal? NetWeight { get; set; }
        public string WareHouseID { get; set; }

        public Admin SorterAdmin { get; set; }
        public decimal? UnitPrice { get; set; }

    }

    public class DeliveriesTopModelOriginName : DeliveriesTopModel
    {
        /// <summary>
        /// 原产地 Code
        /// </summary>
        public string OriginCode { get; set; } = string.Empty;

        /// <summary>
        /// 合并后 InputIDs
        /// </summary>
        public List<string> InputIDs { get; set; } = new List<string>();
    }
}
