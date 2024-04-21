using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Views.Rolls.ApprovalRecords;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Client.ApprovalRecords
{
    public partial class Protecteds : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected object data()
        {
            var applytask = new ProtectedRecordsRoll().Where(x => x.ApplierID == Erp.Current.ID);
            return this.Paging(applytask.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.Name,
                item.MainID,
                item.ApproverID,
                item.ApplyerName,
                item.Status,
                StatusDes = item.Status.GetDescription(),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
            }));

        }
    }
}