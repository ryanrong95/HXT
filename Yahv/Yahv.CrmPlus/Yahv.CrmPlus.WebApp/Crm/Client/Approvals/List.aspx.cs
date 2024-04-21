using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.Underly;
using Yahv.Underly.CrmPlus;
using Yahv.Web.Erp;

namespace Yahv.CrmPlus.WebApp.Crm.Client.Approvals
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.entity = ApplyTasks.Waitings();
            }
        }


        protected object data()
        {
            var applies = ApplyTasks.Array();
            Type type = typeof(Underly.ApplyTaskType);
            var enums = Enum.GetValues(type).Cast<Underly.ApplyTaskType>();
            var data = enums.Cast<Underly.ApplyTaskType>().Select(source =>
            {
                if (Enum.GetName(type, source).StartsWith("Supplier", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                var exist = applies.Where(item => item.TaskType == source);
                if (exist.Any())
                {
                    return exist.FirstOrDefault();
                }
                else
                {
                    return new Applies
                    {
                        Count = 0,
                        TaskType = source,
                    };
                }
              
            }).Where(item => item != null);

            return data;
        }
    }
}