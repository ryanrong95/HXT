using Layers.Data.Sqls.PvbCrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 华芯通网站用户视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class SiteUsersXdtTopView<TReponsitory> : UniqueView<SiteUser, TReponsitory>
          where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public SiteUsersXdtTopView()
        {

        }

        public SiteUsersXdtTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<SiteUser> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.SiteUsersXdtTopView>()
                   select new SiteUser
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       EnterpriseName = entity.EnterpriseName,
                       UserName = entity.UserName,
                       Password = entity.Password,
                       RealName = entity.RealName,
                       Mobile = entity.Mobile,
                       QQ = entity.QQ,
                       Wx = entity.Wx,
                       Email = entity.Email,
                       Status = (ApprovalStatus)entity.Status,
                       IsMain = entity.IsMain,
                       Summary = entity.Summary,
                   };
        }
    }
}


