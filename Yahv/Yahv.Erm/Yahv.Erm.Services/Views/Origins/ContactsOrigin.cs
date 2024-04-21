using System.Linq;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvbErm;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Origins
{
    /// <summary>
    /// 联系方式视图
    /// </summary>
    internal class ContactsOrigin : UniqueView<Contact, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        internal ContactsOrigin() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">数据库连接</param>
        internal ContactsOrigin(PvbErmReponsitory repository) : base(repository) { }

        protected override IQueryable<Contact> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Contacts>()
                   select new Contact()
                   {
                       ID = entity.ID,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Wx = entity.Wx,
                       QQ = entity.QQ,
                       Email = entity.Email,
                       Mobile = entity.Mobile,
                       Tel = entity.Tel,
                   };
        }
    }
}