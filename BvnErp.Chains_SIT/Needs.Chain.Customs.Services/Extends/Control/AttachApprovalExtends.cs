using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class AttachApprovalExtends
    {
        public static void Log(this Models.AttachApproval attachApproval, string orderControlID, string summary, bool isAuto = false)
        {
            AttachApprovalLog log = new AttachApprovalLog()
            {
                ID = Guid.NewGuid().ToString("N"),
                OrderControlID = orderControlID,
                Status = Enums.Status.Normal,
                CreateDate = isAuto ? (DateTime.Now.AddMinutes(30)) : DateTime.Now,
                Summary = summary,
            };

            log.Enter();
        }
    }
}
