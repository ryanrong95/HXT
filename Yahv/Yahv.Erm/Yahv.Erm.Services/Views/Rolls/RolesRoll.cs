using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Erm.Services.Views.Rolls
{
    /// <summary>
    /// 角色
    /// </summary>
    public class RolesRoll : UniqueView<Role, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public RolesRoll()
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
    }
}