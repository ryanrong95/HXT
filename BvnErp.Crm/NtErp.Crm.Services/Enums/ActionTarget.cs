using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NtErp.Crm.Services.Enums
{
    public  enum ActionTarget
    {
        [Description("推广")]
        Extension = 10,
        [Description("项目")]
        Projects = 20,
        [Description("公关")]
        Relation = 30,
        [Description("其他")]
        Other = 40
     }
}
