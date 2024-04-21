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
    /// 当前登录人的受益人
    /// </summary>
    public class MyTradingBeneficiariesRoll : Origins.TradingBeneficiariesOrigin
    {
        IErpAdmin admin;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MyTradingBeneficiariesRoll(IErpAdmin admin)
        {
            this.admin = admin;
        }
        protected override IQueryable<TradingBeneficiary> GetIQueryable()
        {
            if (this.admin.IsSuper)
            {
                return base.GetIQueryable();//去重
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
        public IQueryable<TradingBeneficiary> Match(string enterpriseid)
        {
            return this.Where(item => item.EnterpriseID == enterpriseid);
        }
    }
    /// <summary>
    /// 某Admin的受益人
    /// </summary>
    public class AdminBeneficiaries : Origins.TradingBeneficiariesOrigin
    {
        Admin admin;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminBeneficiaries(Admin admin)
        {
            this.admin = admin;
        }
        protected override IQueryable<TradingBeneficiary> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.CreatorID == this.admin.ID);
        }

    }
}
