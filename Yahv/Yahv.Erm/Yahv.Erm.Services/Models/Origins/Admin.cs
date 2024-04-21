using System;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using YaHv.Erm.Services.Views.Rolls;

namespace Yahv.Erm.Services.Models.Origins
{
    /// <summary>
    /// 管理员
    /// </summary>
    public class Admin : IUnique
    {
        #region 事件

        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;

        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;

        #endregion

        #region 属性
        /// <summary>
        /// 唯一码:Adm001
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 员工ID
        /// </summary>
        public string StaffID { get; set; }

        /// <summary>
        /// 登入名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 登入密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 默认使用：ID的数字部分。即，001
        /// </summary>
        public string SelCode { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleID { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary> 
        public DateTime? UpdateDate { get; set; }

        /// <summary>
        /// 状态 正常（管理员：在系统中要时刻显示管理员的在职与离职状态）、停用、超级管理员。
        /// </summary>
        public AdminStatus Status { get; set; }

        /// <summary>
        /// 最近登录时间
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 密码修改时间
        /// </summary>
        public DateTime? PwdModifyDate { get; set; }

        /// <summary>
        /// 员工状态
        /// </summary>
        public int StaffStatus { get; set; }

        /// <summary>
        /// 大赢家编码
        /// </summary>
        public string StaffDyjCode { get; set; }

        Advantage advantage;
        public Advantage Advantage
        {
            get
            {
                if (this.advantage == null)
                {
                    this.advantage = new AdvantageRoll(this).FirstOrDefault();
                }
                return advantage;
            }
        }
        #endregion

        public Admin()
        {
            this.CreateDate = DateTime.Now;
            this.UpdateDate = DateTime.Now;
            this.Status = AdminStatus.Closed;
        }

        #region 持久化
        /// <summary>
        /// 添加/修改
        /// </summary>
        public void Enter()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                //添加
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    //判断账号是否存在
                    if (repository.ReadTable<Admins>().Any(a => a.UserName == this.UserName))
                    {
                        if (this != null && EnterError != null)
                        {
                            this.EnterError(this, new ErrorEventArgs("该账号已经存在!"));
                        }
                        return;
                    }

                    var ID = PKeySigner.Pick(PKeyType.Admin);
                    repository.Insert(new Admins()
                    {
                        Status = (int)(AdminStatus.Normal),
                        ID = ID,
                        RoleID = this.RoleID,
                        CreateDate = DateTime.Now,
                        Password = this.Password.MD5("x").PasswordOld(),
                        RealName = this.RealName,
                        SelCode = ID.Replace(nameof(PKeyType.Admin), ""),
                        StaffID = this.StaffID,
                        UserName = this.UserName,
                        OriginID = ID,
                        PwdModifyDate = this.PwdModifyDate,
                    });

                    //添加密码修改历史
                    repository.Insert(new Pasts_AdminPassword()
                    {
                        ID = PKeySigner.Pick(PKeyType.PastsAdminPassword),
                        AdminID = ID,
                        CreateDate = DateTime.Now,
                        Password = this.Password.MD5("x").PasswordOld(),
                    });
                }
                //修改
                else
                {
                    if (string.IsNullOrWhiteSpace(this.Password))
                    {
                        repository.Update<Admins>(new
                        {
                            RoleID = this.RoleID,
                            UpdateDate = DateTime.Now,
                        }, a => a.ID == this.ID);
                    }
                    else
                    {
                        repository.Update<Admins>(new
                        {
                            RoleID = this.RoleID,
                            Password = this.Password.MD5("x").PasswordOld(),
                            UpdateDate = DateTime.Now,
                            PwdModifyDate = DateTime.Now,
                        }, a => a.ID == this.ID);

                        //添加密码修改历史
                        repository.Insert(new Pasts_AdminPassword()
                        {
                            ID = PKeySigner.Pick(PKeyType.PastsAdminPassword),
                            AdminID = this.ID,
                            CreateDate = DateTime.Now,
                            Password = this.Password.MD5("x").PasswordOld(),
                        });
                    }
                }

                //操作成功
                if (this != null && EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        /// <summary>
        /// 废弃
        /// </summary>
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbErm.Admins>(new
                {
                    Status = AdminStatus.Closed
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        /// <summary>
        /// 权限开通
        /// </summary>
        public void PermissionOpen()
        {
            using (var repository = LinqFactory<PvbErmReponsitory>.Create())
            {
                if (string.IsNullOrEmpty(this.RoleID))
                {
                    this.RoleID = FixedRole.NewStaff.GetFixedID();
                }
                repository.Update<Layers.Data.Sqls.PvbErm.Admins>(new
                {
                    RoleID = this.RoleID,
                    Status = AdminStatus.Normal,
                }, item => item.ID == this.ID);
            }
        }

        #endregion
    }
}
