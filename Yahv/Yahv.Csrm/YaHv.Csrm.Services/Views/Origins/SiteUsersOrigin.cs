using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Origins
{
    public class SiteUsersXdtOrigin : Yahv.Linq.UniqueView<Models.Origins.SiteUserXdt, PvbCrmReponsitory>
    {
        internal SiteUsersXdtOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal SiteUsersXdtOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<SiteUserXdt> GetIQueryable()
        {
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            return from xdt in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.SiteUsersXdt>()
                   join siteuser in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.SiteUsers>() on xdt.ID equals siteuser.ID
                   where xdt.Status == (int)ApprovalStatus.Normal
                   select new SiteUserXdt
                   {
                       ID = xdt.ID,
                       EnterpriseID = xdt.EnterpriseID,
                       UserName = siteuser.UserName,
                       RealName = siteuser.RealName,
                       Password = siteuser.Password,
                       Mobile = siteuser.Mobile,
                       Email = siteuser.Email,
                       QQ = siteuser.QQ,
                       Wx = siteuser.Wx,
                       IsMain = xdt.IsMain,
                       CreateDate = xdt.CreateDate,
                       UpdateDate = xdt.UpdateDate,
                       Summary = siteuser.Summary,
                       Status = (ApprovalStatus)xdt.Status
                   };
        }
    }
    public class SiteUsersOrigin : Yahv.Linq.UniqueView<Models.Origins.SiteUser, PvbCrmReponsitory>
    {
        internal SiteUsersOrigin()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库连接</param>
        internal SiteUsersOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<SiteUser> GetIQueryable()
        {
            var adminsView = new Rolls.AdminsAllRoll(this.Reponsitory);
            return from siteuser in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.SiteUsers>()
                   select new SiteUser
                   {
                       ID = siteuser.ID,
                       UserName = siteuser.UserName,
                       RealName = siteuser.RealName,
                       Password = siteuser.Password,
                       Mobile = siteuser.Mobile,
                       Email = siteuser.Email,
                       QQ = siteuser.QQ,
                       Wx = siteuser.Wx,
                       Summary = siteuser.Summary
                   };
        }
    }
}
