using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.CustomsTool.WinForm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool.WinForm.Views
{
    public class ManifestConsignmentsView : UniqueView<ManifestConsignment, ScCustomsReponsitory>
    {
        internal ManifestConsignmentsView()
        {
        }

        protected override IQueryable<ManifestConsignment> GetIQueryable()
        {
            var manifestView = new ManifestsView(this.Reponsitory);

            return from consignment in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ManifestConsignments>()
                   join manifest in manifestView on consignment.ManifestID equals manifest.ID
                   join admin in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>() on consignment.AdminID equals admin.ID into admins
                   from _admin in admins.DefaultIfEmpty()
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
                       Admin = new Ccs.Services.Models.Admin { ID = _admin.ID, RealName = _admin.RealName },
                       CreateDate = consignment.CreateDate,
                       MarkingUrl = consignment.MarkingUrl
                   };
        }
    }
}
