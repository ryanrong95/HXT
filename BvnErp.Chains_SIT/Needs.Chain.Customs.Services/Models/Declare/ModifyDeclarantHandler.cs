using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ModifyDeclarantHandler
    {
        public void ModifyDeclareCreator(string[] decNoticeIDs, string adminID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DeclarationNotices>(new
                {
                    CreateDeclareAdminID = adminID,
                }, item => decNoticeIDs.Contains(item.ID));
            }
        }

        public void ModifyCustomSubmiter(string[] decHeadIDs, string adminID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new
                {
                    SubmitCustomAdminID = adminID,
                }, item => decHeadIDs.Contains(item.ID));
            }
        }

        public void ModifyDoubleChecker(string[] decHeadIDs, string adminID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new
                {
                    DoubleCheckerAdminID = adminID,
                }, item => decHeadIDs.Contains(item.ID));
            }
        }

        public void ModifyManifestDoubleChecker(string[] decHeadIDs, string adminID)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ManifestConsignments>(new
                {
                    DoubleCheckerAdminID = adminID,
                }, item => decHeadIDs.Contains(item.ID));
            }
        }
    }
}
