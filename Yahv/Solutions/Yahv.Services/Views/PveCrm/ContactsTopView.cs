using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Services.Views.PveCrm
{
    /// <summary>
    /// 联系人通用视图
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class ContactsTopView<TReponsitory> : UniqueView<Contact, TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ContactsTopView()
        {

        }

        public ContactsTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Contact> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.PveCrm.ContactsTopView>()
                   where entity.Status == (int)Underly.DataStatus.Normal
                   select new Contact
                   {
                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       //Type = (ContactType)entity.Type,
                       //Status = (ApprovalStatus)entity.Status,
                       Name = entity.Name,
                       Tel = entity.Tel,
                       Mobile = entity.Mobile,
                       Email = entity.Email,
                       Fax = entity.Fax
                   };
        }
    }
}
