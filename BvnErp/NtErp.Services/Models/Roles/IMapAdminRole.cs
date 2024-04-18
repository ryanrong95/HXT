using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using Needs.Erp.Generic;

namespace NtErp.Services.Models
{
    /// <summary>
    /// 角色菜单关系
    /// </summary>
    public interface IMapAdminRole : IPersistence,IFulSuccess,IFulError
    {
        string RoleID { get; set; }
        string AdminID { get; set; }
    }
}
