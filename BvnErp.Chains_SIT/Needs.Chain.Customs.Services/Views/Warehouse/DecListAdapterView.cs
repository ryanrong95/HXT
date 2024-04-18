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
    public class DecListAdapterView : UniqueView<DeliveriesTopModel, ScCustomsReponsitory>
    {
        protected override IQueryable<DeliveriesTopModel> GetIQueryable()
        {
            var deliveriesTopViews = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeliveriesTopView>();

            var linq = from deliveriesTopView in deliveriesTopViews
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
                       };

            return linq;
        }

        public IEnumerable<DecListAdapter> GetDataByDecNoticeID(string DecNoticeID,string tinyOrderID)
        {
            var decNoticeItemsViews = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNoticeItems>().Where(item => item.DeclarationNoticeID == DecNoticeID);
            var deliveriesTopViews = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclaresTopView>().Where(item=>item.TinyOrderID== tinyOrderID);            

            var linq = from decNoticeItems in decNoticeItemsViews
                       join deliveriesTop in deliveriesTopViews on decNoticeItems.SortingID equals deliveriesTop.UnqiueID
                       select new DecListAdapter
                       {
                          ID = deliveriesTop.UnqiueID,
                          IptTinyOrderID = deliveriesTop.TinyOrderID,
                          IptItemID = deliveriesTop.ItemID,
                          StoQuantity = deliveriesTop.Quantity,
                          BoxIndex = deliveriesTop.BoxCode,
                          GrossWeight = deliveriesTop.Weight==null?0: deliveriesTop.Weight,
                          NetWeight = deliveriesTop.NetWeight==null?0: deliveriesTop.NetWeight,
                          IptOrigin = deliveriesTop.Origin,
                          DeclarationNoticeItemID = decNoticeItems.ID,
                          PackageType = deliveriesTop.PackageType,
                          Model = deliveriesTop.PartNumber,
                          InputID = deliveriesTop.InputID
                       };

            List<DecListAdapter> deliveriesTopModels = linq.ToList();

            var data = from deliveriesTopModel in deliveriesTopModels
                       select new DecListAdapter()
                       {
                           IptTinyOrderID = deliveriesTopModel.IptTinyOrderID,
                           IptItemID = deliveriesTopModel.IptItemID,
                           StoQuantity = deliveriesTopModel.StoQuantity,
                           BoxIndex = deliveriesTopModel.BoxIndex,
                           GrossWeight = deliveriesTopModel.GrossWeight,
                           NetWeight = deliveriesTopModel.NetWeight,
                           IptOrigin = deliveriesTopModel.IptOrigin,
                           //OriginCode = ((Needs.Underly.Origin)Convert.ToInt32(deliveriesTopModel.IptOrigin)).GetOrigin().Code,
                           OriginCode = deliveriesTopModel.IptOrigin,
                           DeclarationNoticeItemID = deliveriesTopModel.DeclarationNoticeItemID,
                           PackageType = deliveriesTopModel.PackageType,
                           Model = deliveriesTopModel.Model,
                           InputID = deliveriesTopModel.InputID
                       };

            return data;
        }
    }

    public class DecListAdapter : IUnique
    {
        /// <summary>
        /// 
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// 大订单号
        /// </summary>
        public string IptOrderID { get; set; } = string.Empty;

        /// <summary>
        /// 小订单号
        /// </summary>
        public string IptTinyOrderID { get; set; } = string.Empty;

        /// <summary>
        /// OrderItemID
        /// </summary>
        public string IptItemID { get; set; } = string.Empty;

        
        /// <summary>
        /// 原产地
        /// </summary>
        public string IptOrigin { get; set; } = string.Empty;

       
        /// <summary>
        /// 数量
        /// </summary>
        public decimal StoQuantity { get; set; }

      

       
        /// <summary>
        /// 箱号
        /// </summary>
        public string BoxIndex { get; set; }

        /// <summary>
        /// 净重
        /// </summary>
        public decimal? NetWeight { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal? GrossWeight { get; set; }

        public string OriginCode { get; set; }
        public string DeclarationNoticeItemID { get; set; }

        /// <summary>
        /// 包装类型
        /// </summary>
        public string PackageType { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }
        public string InputID { get; set; }

    }

}
