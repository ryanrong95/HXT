using System.Collections.Generic;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Erm.Services.Models.Origins;

namespace Yahv.Erm.Services.Extends
{
    /// <summary>
    /// 岗位扩展类
    /// </summary>
    static public class PostionExtends
    {
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="roles">角色集合</param>
        static public void Detele(this IEnumerable<Postion> roles)
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                var arry = roles.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbErm.Postions>(new
                {
                    Status = Status.Delete
                }, item => arry.Contains(item.ID));
            }
        }
    }
}
