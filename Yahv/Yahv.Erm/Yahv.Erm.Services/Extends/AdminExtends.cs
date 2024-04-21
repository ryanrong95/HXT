using System.Collections.Generic;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Erm.Services.Extends
{
    /// <summary>
    /// 管理员扩展
    /// </summary>
    static public class AdminExtends
    {
        /// <summary>
        /// 停用
        /// </summary>
        /// <param name="admins">管理员集合</param>
        static public void Disable(this IEnumerable<Admin> admins)
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                var arry = admins.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbErm.Admins>(new
                {
                    Status = AdminStatus.Closed
                }, item => arry.Contains(item.ID));
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="admins">管理员集合</param>
        static public void Enable(this IEnumerable<Admin> admins)
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                var arry = admins.Select(item => item.ID);
                repository.Update<Layers.Data.Sqls.PvbErm.Admins>(new
                {
                    Status = AdminStatus.Normal
                }, item => arry.Contains(item.ID));
            }
        }

        /// <summary>
        /// 初始化密码
        /// </summary>
        /// <param name="admins">管理员集合</param>
        static public void InitPassword(this IEnumerable<Admin> admins)
        {
            //初始化的密码
            var pwd = "123456".MD5("x").PasswordOld();

            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                var array = admins.Select(item => item.ID);

                repository.Update<Layers.Data.Sqls.PvbErm.Admins>(new
                {
                    Password = pwd
                }, item => array.Contains(item.ID));
            }
        }
    }
}