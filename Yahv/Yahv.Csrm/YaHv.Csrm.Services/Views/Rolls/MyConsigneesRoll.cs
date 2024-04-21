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
    /// 当前登录人的到货地址
    /// </summary>
    public class MyTradingConsigneesRoll : Origins.ServiceConsigneesOrigin
    {
        IErpAdmin admin;
        /// <summary>
        /// 构造函数
        /// </summary>
        public MyTradingConsigneesRoll(IErpAdmin admin)
        {
            this.admin = admin;
        }
        protected override IQueryable<TradingConsignee> GetIQueryable()
        {
            if (this.admin.IsSuper)
            {
                var query = base.GetIQueryable().GroupBy(item => item.ID).Select(g => g.First());//去重
                return query;
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
        public IQueryable<TradingConsignee> this[Enterprise enterprise]
        {
            get
            {
                return this.GetIQueryable().Where(item => item.EnterpriseID == enterprise.ID);
            }
        }
    }

    /// <summary>
    /// 某Admin的到货地址
    /// </summary>
    public class AdminConsigneesRoll : Origins.ServiceConsigneesOrigin
    {
        Admin admin;
        Business business;
        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminConsigneesRoll(Admin admin, Business business)
        {
            this.admin = admin;
            this.business = business;
        }
        protected override IQueryable<TradingConsignee> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.CreatorID == this.admin.ID); ;
        }
        // <summary>
        /// 匹配企业
        /// </summary>
        /// <param name="enterpriseid"></param>
        /// <returns></returns>
        public IQueryable<TradingConsignee> this[Enterprise enterprise]
        {
            get
            {
                return this.Where(item => item.EnterpriseID == enterprise.ID);
            }
        }
    }
}
