using Yahv.Underly;

namespace Yahv.Models
{
    /// <summary>
    /// Erp 角色
    /// </summary>
    public class RingRole : Underly.Rings.IRingRole
    {
        internal RingRole() { }

        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; internal set; }
        /// <summary>
        /// 角色名
        /// </summary>
        public string Name { get; internal set; }

        public RoleStatus Status { get; internal set; }

        /// <summary>
        /// 是否是超级角色
        /// </summary>
        public bool IsSuper
        {
            get
            {
                return this.Status == RoleStatus.Super;
            }
        }

    }
}
