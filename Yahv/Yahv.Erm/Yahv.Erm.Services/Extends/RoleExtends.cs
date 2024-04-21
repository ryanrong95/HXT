using System.Collections.Generic;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Erm.Services.Models.Origins;

namespace Yahv.Erm.Services.Extends
{
    /// <summary>
    /// 角色扩展
    /// </summary>
    static public class RoleExtends
    {
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="roles">角色集合</param>
        static public void Detele(this IEnumerable<Role> roles)
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                var arry = roles.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbErm.Roles>(new
                {
                    Status = Status.Delete
                }, item => arry.Contains(item.ID));
            }
        }
    }
}