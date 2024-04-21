using System.Linq;
using Layers.Data.Sqls;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Erm.Services.Views.Rolls
{
    /// <summary>
    /// 联系方式
    /// </summary>
    public class ContactsRoll : UniqueView<Contact, PvbErmReponsitory>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ContactsRoll()
        {
        }

        protected override IQueryable<Contact> GetIQueryable()
        {
            return new Origins.ContactsOrigin();
        }
    }
}