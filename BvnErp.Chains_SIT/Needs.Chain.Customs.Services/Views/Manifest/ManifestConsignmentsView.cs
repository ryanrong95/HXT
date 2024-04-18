using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ManifestConsignmentsView : UniqueView<Models.ManifestConsignment, ScCustomsReponsitory>
    {
        public ManifestConsignmentsView()
        {
        }

        internal ManifestConsignmentsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ManifestConsignment> GetIQueryable()
        {
            var manifestView = new ManifestsView(this.Reponsitory);
            var adminView = new AdminsTopView(this.Reponsitory);

            return from consignment in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ManifestConsignments>() 
                   join manifest in manifestView on consignment.ManifestID equals manifest.ID
                   join admin in adminView on consignment.AdminID equals admin.ID into g
                   from admin in g.DefaultIfEmpty()
                   select new Models.ManifestConsignment {
                       ID = consignment.ID,
                       Manifest = manifest,
                       CusMftStatus = consignment.CusMftStatus,
                       ConditionCode = consignment.ConditionCode,
                       PaymentType = consignment.PaymentType,
                       GovProcedureCode = consignment.GovProcedureCode,
                       TransitDestination = consignment.TransitDestination,
                       PackNum = consignment.PackNum,
                       PackType = consignment.PackType,
                       Cube = consignment.Cube,
                       GrossWt = consignment.GrossWt,
                       GoodsValue = consignment.GoodsValue,
                       Currency = consignment.Currency,
                       GoodsQuantity = consignment.GoodsQuantity,
                       Consolidator = consignment.Consolidator,
                       ConsigneeName = consignment.ConsigneeName,
                       ConsignorName = consignment.ConsignorName,
                       Admin = admin,
                       CreateDate = consignment.CreateDate,
                       MarkingUrl = consignment.MarkingUrl
                   };

        }
    }

    public class ManifestConsignmentInfosView : UniqueView<Models.ManifestConsignment, ScCustomsReponsitory>
    {
        public ManifestConsignmentInfosView()
        {
        }

        internal ManifestConsignmentInfosView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ManifestConsignment> GetIQueryable()
        {
            var manifestView = new ManifestsView(this.Reponsitory);
            var consignmentContainersView = new ManifestConsignmentContainersView(this.Reponsitory);
            var consigmentItemsView = new ManifestConsignmentItemsView(this.Reponsitory);

            return from consignment in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ManifestConsignments>()
                   join manifest in manifestView on consignment.ManifestID equals manifest.ID
                   join consignmentContainer in consignmentContainersView on consignment.ID equals consignmentContainer.ManifestConsignmentID into consignmentContainers
                   join consigmentItem in consigmentItemsView on consignment.ID equals consigmentItem.ManifestConsignmentID into consigmentItems
                   select new Models.ManifestConsignment
                   {
                       ID = consignment.ID,
                       Manifest = manifest,
                       CusMftStatus = consignment.CusMftStatus,
                       ConditionCode = consignment.ConditionCode,
                       PaymentType = consignment.PaymentType,
                       GovProcedureCode = consignment.GovProcedureCode,
                       TransitDestination = consignment.TransitDestination,
                       PackNum = consignment.PackNum,
                       PackType = consignment.PackType,
                       Cube = consignment.Cube,
                       GrossWt = consignment.GrossWt,
                       GoodsValue = consignment.GoodsValue,
                       Currency = consignment.Currency,
                       GoodsQuantity = consignment.GoodsQuantity,
                       Consolidator = consignment.Consolidator,
                       ConsigneeName = consignment.ConsigneeName,
                       ConsignorName = consignment.ConsignorName,
                       CreateDate = consignment.CreateDate,
                       MarkingUrl = consignment.MarkingUrl,

                       Containers= new Models.ManifestConsignmentContainers(consignmentContainers),
                       Items = new Models.ManifestConsignmentItems(consigmentItems)
                   };

        }
    }
}
