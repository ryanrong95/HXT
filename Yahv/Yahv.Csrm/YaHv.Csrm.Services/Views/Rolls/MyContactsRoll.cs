using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Underly.Erps;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    /// <summary>
    /// 当前登录人的联系人
    /// </summary>
    public class MyTradingContactsRoll : Origins.ServiceContactsOrigin
    {
        IErpAdmin admin;
        /// <summary>
        /// 构造函数
        /// </summary>
        public MyTradingContactsRoll(IErpAdmin admin)
        {
            this.admin = admin;
        }
        protected override IQueryable<TradingContact> GetIQueryable()
        {
            if (this.admin.IsSuper)
            {
                var group = base.GetIQueryable().GroupBy(item => item.ID).Select(g => g.First());//去重
                return group;
            }
            else
            {
                return base.GetIQueryable().Where(item => item.CreatorID == this.admin.ID);
            }
        }
        /// <summary>
        /// 匹配企业
        /// </summary>
        /// <param name="enterpriseid"></param>
        /// <returns></returns>
        public IQueryable<TradingContact> this[Enterprise enterprise]
        {
            get
            {
                return this.GetIQueryable().Where(item => item.EnterpriseID == enterprise.ID);
            }
        }

    }
    /// <summary>
    /// 某Admin的联系人
    /// </summary>
    public class AdminContactsRoll : Origins.ServiceContactsOrigin
    {
        Admin admin;
        Business business;
        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminContactsRoll(Admin admin, Business business = Business.Trading)
        {
            this.admin = admin;
            this.business = business;
        }
        protected override IQueryable<TradingContact> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.CreatorID == this.admin.ID);
        }
        /// <summary>
        /// 匹配企业
        /// </summary>
        /// <param name="enterpriseid"></param>
        /// <returns></returns>
        public IQueryable<TradingContact> this[Enterprise enterprise]
        {
            get
            {
                return this.Where(item => item.EnterpriseID == enterprise.ID);
            }
        }
    }
}
