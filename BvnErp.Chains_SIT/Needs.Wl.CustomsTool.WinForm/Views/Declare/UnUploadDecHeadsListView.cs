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
    /// <summary>
    /// 报关单--未上传
    /// </summary>
    public class UnUploadDecHeadsListView : UniqueView<UploadDecHead, ScCustomsReponsitory>
    {
        public UnUploadDecHeadsListView()
        {
        }

        internal UnUploadDecHeadsListView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<UploadDecHead> GetIQueryable()
        {
            var fileView = new DecHeadFilesView(this.Reponsitory);
            var decListView = new DecOriginListsView(this.Reponsitory);
            return from head in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                   join file in fileView on head.ID equals file.DecHeadID into files
                   join declist in decListView on head.ID equals declist.DeclarationID into declists
                   orderby head.CreateTime descending
                   select new UploadDecHead
                   {
                       ID = head.ID,
                       ContrNo = head.ContrNo,
                       OrderID = head.OrderID,
                       BillNo = head.BillNo,
                       EntryId = head.EntryId,
                       PreEntryId = head.PreEntryId,
                       AgentName = head.AgentName,
                       ConsignorName = head.ConsignorName,
                       ConsigneeName = head.ConsigneeName,
                       IsInspection = head.IsInspection,
                       IsSuccess = head.IsSuccess,
                       DDate = head.DDate,
                       CusDecStatus = head.CusDecStatus,
                       files = files,
                       Currency = declists.First() == null ? "" : declists.First().TradeCurr,
                       DecAmount = declists.Sum(t => t.DeclTotal),

                   };
        }
    }
}
