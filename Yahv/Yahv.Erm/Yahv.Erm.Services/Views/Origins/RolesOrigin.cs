using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Erm.Services.Views.Origins
{
    internal class RolesOrigin : UniqueView<Role, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal RolesOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        internal RolesOrigin(PvbErmReponsitory repository) : base(repository) { }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Role> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Roles>()
                   select new Role()
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       CreateDate = entity.CreateDate,
                       Status = (RoleStatus)entity.Status,
                       Type = (RoleType)entity.Type,
                   };
        }
    }
}