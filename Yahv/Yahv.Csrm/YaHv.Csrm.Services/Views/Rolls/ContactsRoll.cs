using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    /// <summary>
    /// 没有MapBEnter关系和不区分业务的使用，承运商联系人，内部公司联系人
    /// </summary>
    public class ContactsRoll : Origins.ContactsOrigin
    {
        Enterprise enterprise;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public ContactsRoll(Enterprise enterprise = null)
        {
            this.enterprise = enterprise;
        }
        protected override IQueryable<Models.Origins.Contact> GetIQueryable()
        {
            if (enterprise == null)
            {
                return base.GetIQueryable();
            }
            else
            {
                return from item in base.GetIQueryable()
                       where item.EnterpriseID == this.enterprise.ID
                       select item;
            }
        }
    }
    /// <summary>
    /// 代仓储客户的联系人
    /// </summary>
    public class WsContactsRoll : Origins.WsContactsOrigin
    {
        Enterprise enterprise;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public WsContactsRoll(Enterprise enterprise)
        {
            this.enterprise = enterprise;
        }
        protected override IQueryable<Models.Origins.WsContact> GetIQueryable()
        {
            return from item in base.GetIQueryable()
                   where item.EnterpriseID == this.enterprise.ID
                   select item;
        }
    }

    /// <summary>
    /// 传统贸易联系人
    /// </summary>
    public class TradingContactsRoll : Origins.ServiceContactsOrigin
    {
        Enterprise enterprise;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public TradingContactsRoll(Enterprise enterprise)
        {
            this.enterprise = enterprise;
        }
        protected override IQueryable<Models.Origins.TradingContact> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.EnterpriseID == this.enterprise.ID).GroupBy(item => item.ID).Select(g => g.First());
        }
    }
}
