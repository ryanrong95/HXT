using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Erm.Services.Views
{
    /// <summary>
    /// 角色
    /// </summary>
    public class RolesAll : UniqueView<Role, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public RolesAll()
        {
        }

        /// <summary>
        /// 数据集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Role> GetIQueryable()
        {
            return new RolesOrigin(this.Reponsitory).Where(t => t.Status != RoleStatus.Delete);
        }

        /// <summary>
        /// 已重写 索引器
        /// </summary>
        /// <param name="id">唯一码</param>
        /// <returns>Partner</returns>
        public override Role this[string id]
        {
            get
            {
                return this.SingleOrDefault(item => item.ID == id);
            }
        }

        /// <summary>
        /// 根据组合角色ID获取角色列表
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public Role[] GetChildRoles(string roleId)
        {
            var maps = Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsRoleCompose>();
            var roles = this;

            return (from m in maps
                    join r in roles on m.ChildID equals r.ID
                    where m.RoleID == roleId
                    select new Role()
                    {
                        ID = m.ChildID,
                        Name = r.Name,
                        CreateDate = r.CreateDate,
                        Status = r.Status,
                        Type = r.Type,
                    }).ToArray();
        }

        /// <summary>
        /// 添加合并角色关系
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="roleIds">合成角色数组</param>
        public void Compose(string roleId, string[] roleIds)
        {
            if (string.IsNullOrWhiteSpace(roleId) || roleIds.Length <= 0)
            {
                return;
            }

            //清空角色合并关系
            Reponsitory.Delete<Layers.Data.Sqls.PvbErm.MapsRoleCompose>(item => item.RoleID == roleId);

            //添加角色合并关系
            foreach (var id in roleIds)
            {
                Reponsitory.Insert<Layers.Data.Sqls.PvbErm.MapsRoleCompose>(new MapsRoleCompose()
                {
                    RoleID = roleId,
                    ChildID = id,
                });
            }
        }
    }
}