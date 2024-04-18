using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Finance.Services.Models
{
    public class DeleteDecFromSwapNoticeHandler
    {
        private string SwapNoticeID { get; set; } = string.Empty;

        private string DeleteDecHeadID { get; set; } = string.Empty;

        public DeleteDecFromSwapNoticeHandler(string swapNoticeID, string deleteDecHeadID)
        {
            this.SwapNoticeID = swapNoticeID;
            this.DeleteDecHeadID = deleteDecHeadID;
        }

        public void Execute()
        {
            //查出当前 SwapNoticeID 中，除了 DeleteDecHeadID 之外其他的 DecHead，
            //就是当前 CleanDecHeadID_Array

            string[] CleanDecHeadID_Array = null;

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                CleanDecHeadID_Array = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.SwapNoticeItems>()
                    .Where(t => t.SwapNoticeID == this.SwapNoticeID
                             && t.DecHeadID != this.DeleteDecHeadID
                             && t.Status == (int)Needs.Wl.Models.Enums.Status.Normal)
                    .Select(t => t.DecHeadID)
                    .ToArray();
            }

            Needs.Wl.Finance.Services.Models.EditBankHandler editBankHandler =
                new EditBankHandler(this.SwapNoticeID, string.Join(",", CleanDecHeadID_Array));
            editBankHandler.Execute();
        }

    }
}
