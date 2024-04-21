using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.CrmPlus.Service;
using Yahv.Web.Erp;
using Yahv.Underly;

namespace Yahv.CrmPlus.WebApp.Srm.Approvals
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        protected object data()
        {
            var applies = ApplyTasks.Array();
            var srmlist = new List<Applies>();
            Type type = typeof(Underly.ApplyTaskType);
            var enums = Enum.GetValues(type).Cast<Underly.ApplyTaskType>();
            var data = enums.Cast<Underly.ApplyTaskType>().Select(source =>
            {
                if (Enum.GetName(type, source).StartsWith("Client", StringComparison.OrdinalIgnoreCase))
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
                //if (applies.Any(item => item.TaskType == source))
                //{
                //    return applies.FirstOrDefault(item => item.TaskType == Underly.CrmPlus.ApplyTaskType.SupplierRegist);
                //}
                //else
                //{
                //    return new Applies
                //    {
                //        Count = 0,
                //        TaskType = source,
                //    };
                //}
            }).Where(item => item != null);

            return data;
            
        }
    }
}