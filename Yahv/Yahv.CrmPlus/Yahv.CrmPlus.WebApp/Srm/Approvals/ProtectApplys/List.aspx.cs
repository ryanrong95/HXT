using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Srm.Approvals.ProtectApplys
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }
        protected object data()
        {
            var query = Service.ApplyTasks.SupplierProtectApplys();
            return this.Paging(query.OrderByDescending(item => item.CreateDate).ToArray().Select(item => new
            {
                item.ID,
                item.MainID,
                item.SupplierName,
                item.ApplyerName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
            }));

        }
        bool success = false;
        /// <summary>
        /// 审批
        /// </summary>
        protected bool Approve()
        {
            bool result = bool.Parse(Request["result"]);
            string id = Request["id"];
            var entity = Service.ApplyTasks.SupplierProtectApplys().FirstOrDefault(item=>item.ID==id);
           ApplyStatus status = result ?ApplyStatus.Allowed : ApplyStatus.Voted;
            entity.Status = status;
            entity.EnterSuccess += Entity_EnterSuccess;
            entity.Approve( Erp.Current.ID);
            return success;
        }

        private void Entity_EnterSuccess(object sender, Usually.SuccessEventArgs e)
        {
            success = true;
        }
    }
}