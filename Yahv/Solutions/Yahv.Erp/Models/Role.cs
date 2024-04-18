using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Underly;
using Yahv.Underly.Erps;

namespace Yahv.Models
{
    /// <summary>
    /// Erp 角色
    /// </summary>
    public sealed class Role : Yahv.Underly.Erps.IRole
    {
        internal Role() { }

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

        /// <summary>
        /// 角色类型
        /// </summary>
        public RoleType Type { get; set; }

        /// <summary>
        /// 包涵验证
        /// </summary>
        /// <param name="id">RoleID</param>
        bool Contains(string id)
        {

            if (this.IsSuper)
            {
                return true;
            }

            switch (this.Type)
            {
                case RoleType.Customer:
                    return this.ID == id;
                case RoleType.Compose:
                    using (var reponsitory = new PvbErmReponsitory())
                    {
                        var linq_composes = from map in reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsRoleCompose>()
                                            where map.RoleID == this.ID
                                            select map.RoleID;

                        return this.ID == id || linq_composes.Any(item => item == id);
                    }
                default:
                    throw new NotSupportedException($"The specified type :{this.ToString()} is not supported ");
            }
        }

        /// <summary>
        /// 固定角色包涵验证
        /// </summary>
        /// <param name="role">固定角色枚举</param>
        public bool Contains(FixedRole role)
        {
            return this.Contains(role.GetFixedID());
        }

        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="d"></param>
        public static implicit operator FixedRole(Role role)
        {
            return Enum.GetValues(typeof(FixedRole)).Cast<FixedRole>().SingleOrDefault(item => item.GetFixedID() == role.ID);
        }
    }

    ///// <summary>
    ///// 角色扩展类型
    ///// </summary>
    //static public class RoleExtends
    //{
    //    /// <summary>
    //    /// 固定角色包涵验证
    //    /// </summary>
    //    /// <param name="role">输入对象 角色对象</param>
    //    /// <param name="fixedRole">固定角色枚举</param>
    //    /// <returns>包涵</returns>
    //    /// <remarks>
    //    /// 为了避免修改接口先这样开发
    //    /// </remarks>
    //    static public bool Contains(this Role role, FixedRole fixedRole)
    //    {
    //        return role.Contains(fixedRole);
    //    }
    //}
}
