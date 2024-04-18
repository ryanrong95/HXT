using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Plats.Services.Views.Roll
{
    public class AdminsRoll : UniqueView<Models.AdminRoll, PvbErmReponsitory>
    {
        /// <summary>
        /// 默认构造器
        /// </summary>
        internal AdminsRoll()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">Linq支持者</param>
        internal AdminsRoll(PvbErmReponsitory reponsitory) : base(reponsitory)
        {
        }
        /// <summary>
        /// 管理员 可查询集
        /// </summary>
        /// <returns>可查询集</returns>
        sealed protected override IQueryable<Models.AdminRoll> GetIQueryable()
        {
            //var origin = new Origins.AdminsOrigin(this.Reponsitory);
            //return from admin in origin
            //       join _role in this.RoleView on admin.RoleID equals _role.ID into roles
            //       from role in roles.DefaultIfEmpty()
            //       select new Models.AdminRoll
            //       {
            //           ID = admin.ID,
            //           StaffID = admin.StaffID,
            //           UserName = admin.UserName,
            //           RealName = admin.RealName,
            //           Password = admin.Password,
            //           SelCode = admin.SelCode,
            //           Role = role,
            //           CreateDate = admin.CreateDate,
            //           UpdateDate = admin.UpdateDate,
            //           Status = admin.Status,
            //           LastLoginDate = admin.LastLoginDate,
            //           PwdModifyDate = admin.PwdModifyDate,
            //       };


            var origin = new Origins.AdminsOrigin(this.Reponsitory);

            //合并角色信息列表
            var linq = from m in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsRoleCompose>()
                       join r in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Roles>()
                        on m.ChildID equals r.ID
                       select new
                       {
                           ParentID = m.RoleID,
                           ID = r.ID,
                           Name = r.Name,
                           RoleType = (RoleType)r.Type,
                           Status = (Underly.RoleStatus)r.Status,
                       };

            return from admin in origin
                   join _role in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Roles>() on admin.RoleID equals _role.ID into roles
                   from role in roles.DefaultIfEmpty()
                   select new Models.AdminRoll
                   {
                       ID = admin.ID,
                       StaffID = admin.StaffID,
                       UserName = admin.UserName,
                       RealName = admin.RealName,
                       Password = admin.Password,
                       SelCode = admin.SelCode,
                       Role = new Models.RoleRoll
                       {
                           ID = role.ID,
                           Name = role.Name,
                           Status = (Underly.RoleStatus)role.Status,
                           Type = (Underly.RoleType)role.Type,
                           ChildRoles = from c in linq
                                        where c.ParentID == role.ID
                                        select new Role()
                                        {
                                            ID = c.ID,
                                            Name = c.Name,
                                            Type = c.RoleType,
                                            Status = c.Status
                                        }
                       },
                       CreateDate = admin.CreateDate,
                       UpdateDate = admin.UpdateDate,
                       Status = admin.Status,
                       LastLoginDate = admin.LastLoginDate,
                       PwdModifyDate = admin.PwdModifyDate,
                   };
        }

        /// <summary>
        /// 是否是芯达通业务员工
        /// </summary>
        /// <param name="admindID">adminID</param>
        /// <param name="business">业务名称</param>
        /// <returns></returns>
        public bool IsXdt(string admindID, string business)
        {
            bool result = false;

            using (var reponsitory = new PvbErmReponsitory())
            {
                var view = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.AdminsBussinessTopView>();

                if (view.Where(item => item.ID == admindID).Any(item => item.bussiness.Contains(business)))
                {
                    result = true;
                }
            }

            return result;
        }


        public IQueryable<Models.RoleRoll> RoleView
        {
            get
            {
                var linq = from m in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.MapsRoleCompose>()
                           join r in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Roles>()
                            on m.ChildID equals r.ID
                           select new
                           {
                               fatherID = m.RoleID,
                               ID = r.ID,
                               Name = r.Name,
                               RoleType = (RoleType)r.Type,
                               Status = (Underly.RoleStatus)r.Status,
                           };

                return from r in Reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Roles>()
                       join l in linq on r.ID equals l.ID into ls
                       select new Models.RoleRoll
                       {
                           ID = r.ID,
                           Name = r.Name,
                           Status = (Underly.RoleStatus)r.Status,
                           ChildRoles = from c in ls
                                        where c.fatherID == r.ID
                                        select new Role()
                                        {
                                            ID = c.ID,
                                            Name = c.Name,
                                            Type = c.RoleType,
                                            Status = c.Status
                                        }
                       };
            }

        }
    }
}