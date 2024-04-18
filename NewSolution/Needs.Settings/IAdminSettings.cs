using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Settings
{
    public interface IAdminSettings : ISettings
    {
        /// <summary>
        /// 超级权限
        /// </summary>
        string SuperRoleID { get; }
        string SuperID { get; }
        string LoginCookieName { get; }

        string SuperName { get; }
    }
}
