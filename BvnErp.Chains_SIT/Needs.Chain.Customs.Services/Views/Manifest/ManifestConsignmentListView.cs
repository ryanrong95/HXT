using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ManifestConsignmentListView : UniqueView<Models.ManifestConsignmentList, ScCustomsReponsitory>
    {
        public ManifestConsignmentListView()
        {
        }

        internal ManifestConsignmentListView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ManifestConsignmentList> GetIQueryable()
        {
            var adminView = new AdminsTopView(this.Reponsitory);

            return from consignment in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ManifestConsignments>()
                   join manifest in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Manifests>() on consignment.ManifestID equals manifest.ID
                   join dec in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on consignment.ID equals dec.BillNo into decs
                   from d in decs.DefaultIfEmpty()
                   join admin in adminView on consignment.AdminID equals admin.ID
                   join doubleChecker in adminView on consignment.DoubleCheckerAdminID equals doubleChecker.ID into g
                   from doubleC in g.DefaultIfEmpty()
                   select new Models.ManifestConsignmentList
                   {
                       ID = manifest.ID,
                       VoyageNo = manifest.ID,
                       BillNo = consignment.ID,
                       ContrNO = d.ContrNo,
                       Port = d.CustomMaster,
                       PackNo = d.PackNo,
                       ConsigneeName = d.ConsigneeName,
                       AdminName = admin.ByName,
                       CreateTime = manifest.CreateTime,
                       CusMftStatus = consignment.CusMftStatus,
                       DoubleCheckerName = doubleC == null ? "" : doubleC.ByName,
                       DoubleCheckerAdminID = doubleC == null ? "" : doubleC.ID,
                   };

        }
    }
}
