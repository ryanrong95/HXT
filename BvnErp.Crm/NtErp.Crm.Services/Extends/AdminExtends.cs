using NtErp.Crm.Services.Models;
using NtErp.Crm.Services.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Extends
{
    public static class AdminExtends
    {
        /// <summary>
        /// model数据映射数据库对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        static internal Layer.Data.Sqls.BvCrm.AdminsProject ToLinq(this Models.AdminProject entity)
        {
            return new Layer.Data.Sqls.BvCrm.AdminsProject
            {
                AdminID = entity.AdminID,
                JobType = (int)entity.JobType,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                CompanyID = entity.Company.ID,
                IsAgree = entity.IsAgree,
                Token = entity.Token,
                WXID = entity.WXID,
                ScoreType = (int?)entity.ScoreType,
                SalaryBase = entity.SalaryBase,
                DyjID = entity.DyjID,
                Summary = entity.Summary,
            };
        }


        /// <summary>
        /// 使用 外部扩展方法开发或是使用强类型开发
        /// </summary>
        /// <returns></returns>
        public static AdminTop GetTop(string id)
        {
            using (var alls = new Views.AdminTopView())
            {
                return alls[id];
            }
        }

        /// <summary>
        /// 跨表连接
        /// </summary>
        /// <param name="view"></param>
        /// <param name="predicate1">Admin表的过滤条件</param>
        /// <param name="predicate2">AdminProject表的过滤条件</param>
        /// <returns></returns>
        //static public AdminTop[] CrmWhere(this MyStaffsView view, Expression<Func<Admin, bool>> predicate1, Expression<Func<AdminProject, bool>> predicate2)
        //{
        //    Layer.Data.Sqls.BvnErpReponsitory reponsitory = new Layer.Data.Sqls.BvnErpReponsitory();

        //    var admin = from entity in reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.Admins>()
        //                select new Admin
        //                {
        //                    ID = entity.ID,
        //                    Name = entity.UserName,
        //                    RealName = entity.RealName
        //                };
        //    if (predicate1 != null)
        //    {
        //        admin = admin.Where(predicate1);
        //    }
        //    string[] adminID = admin.Select(item => item.ID).ToArray();


        //    var adminproject = from entity in view
        //                       where adminID.Contains(entity.AdminID)
        //                       select new AdminProject
        //                       {
        //                           AdminID = entity.AdminID,
        //                           CompanyID = entity.CompanyID,
        //                           JobType = entity.JobType,
        //                           CreateDate = entity.CreateDate,
        //                           UpdateDate = entity.UpdateDate,
        //                           Summary = entity.Summary
        //                       };
        //    if (predicate2 != null)
        //    {
        //        adminproject = adminproject.Where(predicate2);
        //    }

        //    string[] projectID = adminproject.Select(item => item.AdminID).ToArray();

        //    admin = from entity in admin
        //            where projectID.Contains(entity.ID)
        //            select new Admin
        //            {
        //                ID = entity.ID,
        //                Name = entity.Name,
        //                RealName = entity.RealName
        //            };
        //    var resultP = adminproject.ToArray();
        //    var resultA = admin.ToArray();

        //    var admintop = from a in resultA
        //                   join b in resultP
        //                   on a.ID equals b.AdminID
        //                   select new AdminTop
        //                   {
        //                       ID = a.ID,
        //                       Name = a.Name,
        //                       RealName = a.RealName,
        //                       JobType = b.JobType,
        //                       AdminID = b.AdminID,
        //                       CompanyID = b.CompanyID,
        //                       CreateDate = b.CreateDate,
        //                       UpdateDate = b.UpdateDate,
        //                       Summary = b.Summary
        //                   };
        //    reponsitory.Dispose();
        //    return admintop.ToArray();
        //}        


        //static public AdminTop[] CrmWhere(this AdminsProjectAll view, Expression<Func<Admin, bool>> predicate1, Expression<Func<AdminProject, bool>> predicate2)
        //{
        //    Layer.Data.Sqls.BvnErpReponsitory reponsitory = new Layer.Data.Sqls.BvnErpReponsitory();

        //    var admin = from entity in reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.Admins>()
        //                select new Admin
        //                {
        //                    ID = entity.ID,
        //                    Name = entity.UserName,
        //                    RealName = entity.RealName
        //                };
        //    if (predicate1 != null)
        //    {
        //        admin = admin.Where(predicate1);
        //    }
        //    string[] adminID = admin.Select(item => item.ID).ToArray();


        //    var adminproject = from entity in view
        //                       where adminID.Contains(entity.AdminID)
        //                       select new AdminProject
        //                       {
        //                           AdminID = entity.AdminID,
        //                           CompanyID = entity.CompanyID,
        //                           JobType = entity.JobType,
        //                           CreateDate = entity.CreateDate,
        //                           UpdateDate = entity.UpdateDate,
        //                           Summary = entity.Summary
        //                       };
        //    if (predicate2 != null)
        //    {
        //        adminproject = adminproject.Where(predicate2);
        //    }
        //    var resultP = adminproject.ToArray();
        //    var resultA = admin.ToArray();

        //    var admintop = from a in resultA
        //                   join b in resultP
        //                   on a.ID equals b.AdminID into item
        //                   from c in item.DefaultIfEmpty()
        //                   select new AdminTop
        //                   {
        //                       ID = a.ID,
        //                       Name = a.Name,
        //                       RealName = a.RealName,
        //                       AdminID = c?.AdminID ?? string.Empty,
        //                       JobType = c?.JobType ?? 0,
        //                       CompanyID = c?.CompanyID ?? string.Empty,
        //                       Summary = c?.Summary ?? string.Empty,
        //                   };

        //    reponsitory.Dispose();
        //    return admintop.ToArray();
        //}


        //static public Admin[] CrmWhere(this AdminsProjectAll view, Expression<Func<AdminProject, bool>> predicate)
        //{
        //    Layer.Data.Sqls.BvnErpReponsitory reponsitory = new Layer.Data.Sqls.BvnErpReponsitory();

        //    var adminproject = from entity in view
        //                       select new AdminProject
        //                       {
        //                           AdminID = entity.AdminID,
        //                           CompanyID = entity.CompanyID,
        //                           JobType = entity.JobType,
        //                           CreateDate = entity.CreateDate,
        //                           UpdateDate = entity.UpdateDate,
        //                           Summary = entity.Summary
        //                       };
        //    if (predicate != null)
        //    {
        //        adminproject = adminproject.Where(predicate);
        //    }

        //    string[] projectID = adminproject.Select(item => item.AdminID).ToArray();

        //    var admin = from entity in reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.Admins>()
        //                where projectID.Contains(entity.ID)
        //                select new Admin
        //                {
        //                    ID = entity.ID,
        //                    Name = entity.UserName,
        //                    RealName = entity.RealName
        //                };
        //    var result = admin.ToArray();
        //    reponsitory.Dispose();
        //    return result;
        //}
    }
}
