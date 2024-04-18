using Yahv.Settings;
using Yahv.Underly.Erps;
using Layers.Data.Sqls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Plats.Services.Views.Roll;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.Views;

namespace Yahv.Models
{
    /// <summary>
    /// Erp系统管理员
    /// </summary>
    public partial class ErpAdmin : IAdmin, Linq.IUnique
    {
        /// <summary>
        /// 构造函数，初始化对象
        /// </summary>
        internal ErpAdmin()
        {

        }

        #region 属性

        /// <summary>
        /// Erp 管理员 只读 ID
        /// </summary>
        public string ID { get; internal set; }
        /// <summary>
        /// Erp 管理员 只读 登录名
        /// </summary>
        public string UserName { get; internal set; }
        /// <summary>
        /// Erp 管理员 只读 真实名
        /// </summary>
        public string RealName { get; internal set; }
        /// <summary>
        /// 员工ID
        /// </summary>
        public string StaffID { get; internal set; }

        /// <summary>
        /// 管理员类型
        /// </summary>
        public ErpAdminType Type
        {
            get
            {
                if (this.IsSuper)
                //  || 属于超级管理员权限的一般管理员
                {
                    return ErpAdminType.Supper;
                }

                if (this.ID.Contains(nameof(ErpAdminType.Npc))
                    //|| this.UserName.Contains(nameof(ErpAdminType.Npc))
                    //|| this.RealName.Contains(nameof(ErpAdminType.Npc))
                    )
                {
                    return ErpAdminType.Npc;
                }

                return ErpAdminType.Normal;
            }
        }

        /// <summary>
        /// 是否系统超级管理员
        /// </summary>
        public bool IsSuper
        {
            get
            {
                if (this.ID == SettingsManager<IAdminSettings>.Current.SuperID
                    || this.UserName == SettingsManager<IAdminSettings>.Current.SuperName)
                //  || 属于超级管理员权限的一般管理员
                {
                    return true;
                }
                else
                {
                    return this.Status == Underly.AdminStatus.Super || this.Role.IsSuper;
                }
            }
        }


        Role role;

        public IRole Role
        {
            get
            {
                return role;
            }

            set
            {
                role = value as Role;
            }
        }

        /// <summary>
        /// 角色
        /// </summary>
        internal AdminStatus Status { get; set; }

        ///// <summary>
        ///// 角色类型
        ///// </summary>
        //public RoleType RoleType { get; set; }



        #endregion

        #region 扩展属性
        ///// <summary>
        ///// 我的菜单
        ///// </summary>
        //public Yahv.Erp.Services.Views.Rolls.MenusRoll Menus
        //{
        //    get
        //    {
        //        return new Services.Views.Rolls.MenusRoll(this);
        //    }
        //}
        ///// <summary>
        ///// 我的角色
        ///// </summary>
        //public IRole Role
        //{
        //    get
        //    {
        //        return new Services.Views.Rolls.RolesRoll(this).SingleOrDefault();
        //    }
        //}


        PersonalLeagues pleagues;

        /// <summary>
        /// 我的组织机构
        /// </summary>
        public PersonalLeagues Leagues
        {
            get
            {
                if (this.pleagues == null)
                {
                    lock (this)
                    {
                        if (this.pleagues == null)
                        {
                            var view = new Yahv.Views.PersonalLeaguesView(this).Select(item => new League()
                            {
                                ID = item.ID,
                                Name = item.Name
                            });

                            this.pleagues = new PersonalLeagues(view.ToArray());
                        }
                    }
                }

                return this.pleagues;
            }
        }

        private PermissionSetting _permissionSetting;
        private static object lockPermission = new object();
        /// <summary>
        /// 权限（菜单、颗粒化）
        /// </summary>
        public PermissionSetting PermissionSettings
        {
            get
            {
                if (this._permissionSetting == null)
                {
                    lock (lockPermission)
                    {
                        if (this._permissionSetting == null)
                        {
                            this._permissionSetting = new PermissionSetting()
                            {
                                MyMenes = new PersonalMenusView(this).ToArray(),
                                ParticleSettings = new PersonParticleSettingsView(this).ToArray(),
                                MenesAll = new MenusView().ToArray()
                            };
                        }
                    }
                }

                return this._permissionSetting;
            }
        }

        bool staffReaded;
        ErmStaff staff;
        /// <summary>
        /// Staff信息
        /// </summary>
        public ErmStaff Staff
        {
            get
            {
                if (!staffReaded)
                {
                    lock (this)
                    {
                        if (!staffReaded)
                        {
                            using (var alls = new Views.StaffsAlls())
                            {
                                staff = alls.SingleOrDefault(item => item.ID == this.StaffID);
                                staffReaded = true;
                            }

                        }
                    }
                }

                return staff;
            }
        }


        #endregion

        #region 业务函数

        /// <summary>
        /// 更改密码
        /// </summary>
        public void ChangePassword(string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                throw new Exception("密码不能为空!");
            }

            //需要进行判断如果有任何不对的地方，直接抛出异常即可
            string adminid = this.ID;
            newPassword = newPassword.MD5("x").PasswordOld();

            using (var repository = new PvbErmReponsitory(false))
            {
                //密码修改记录
                var pasts = repository.ReadTable<Layers.Data.Sqls.PvbErm.Pasts_AdminPassword>()
                        .Where(item => item.AdminID == adminid)
                        .OrderByDescending(item => item.CreateDate)
                        .Take(3);

                if (pasts != null && pasts.Any(item => item.Password == newPassword))
                {
                    throw new Exception("不能与近三次密码一致!");
                }

                //修改密码
                repository.Update<Layers.Data.Sqls.PvbErm.Admins>(new
                {
                    Password = newPassword,
                    UpdateDate = DateTime.Now,
                    PwdModifyDate = DateTime.Now,
                }, item => item.ID == adminid);

                //添加密码修改历史
                repository.Insert(new Pasts_AdminPassword()
                {
                    ID = PKeySigner.Pick(Yahv.Erm.Services.PKeyType.PastsAdminPassword),
                    AdminID = adminid,
                    CreateDate = DateTime.Now,
                    Password = newPassword,
                });

                repository.Submit();
            }
        }

        /// <summary>
        /// 固定角色包涵验证
        /// </summary>
        /// <param name="role">固定角色枚举</param>
        public bool Contains(FixedRole role)
        {
            return this.role.Contains(role);
        }

        #endregion

        /// <summary>
        /// 获取固定角色
        /// </summary>
        public FixedRole? Fixed
        {
            get
            {
                return Enum.GetValues(typeof(FixedRole)).Cast<FixedRole>().SingleOrDefault(item => item.GetFixedID() == this.Role.ID);
            }
        }


    }
}
