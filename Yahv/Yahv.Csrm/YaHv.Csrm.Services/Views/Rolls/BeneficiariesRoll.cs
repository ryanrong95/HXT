using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Linq;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using Yahv.Usually;
using Yahv.Underly.Erps;
using Yahv.Underly;

namespace YaHv.Csrm.Services.Views.Rolls
{
    /// <summary>
    ///没有MapsBENter关系,不区分业务的受益人（承运商受益人，内部公司受益人在使用）
    /// </summary>
    public class BeneficiariesRoll : Origins.BeneficiariesOrigin
    {
        Enterprise enterprise;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public BeneficiariesRoll(Enterprise enterprise = null)
        {
            this.enterprise = enterprise;
        }
        protected override IQueryable<Beneficiary> GetIQueryable()
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
    /// 代仓储供应商的受益人（客户与供应商有关系）
    /// </summary>
    public class WsBeneficiariesRoll : Origins.WsBeneficiariesOrigin
    {
        Enterprise WsClient;
        Enterprise WsSupplier;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public WsBeneficiariesRoll(Enterprise wsclient, Enterprise wssupplier)
        {
            this.WsClient = wsclient;
            this.WsSupplier = wssupplier;
        }
        protected override IQueryable<WsBeneficiary> GetIQueryable()
        {
            return from entity in base.GetIQueryable()
                   where entity.EnterpriseID == this.WsSupplier.ID && entity.WsClient.ID == this.WsClient.ID
                   select entity;
        }

    }

    //根据传统贸易业务（MapsBenter）查询受益人的关系（传统贸易供应商）
    public class TradeBeneficiariesRoll : Origins.TradingBeneficiariesOrigin
    {
        Enterprise enterprise;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public TradeBeneficiariesRoll(Enterprise enterprise)
        {
            this.enterprise = enterprise;
        }
        protected override IQueryable<TradingBeneficiary> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.EnterpriseID == this.enterprise.ID).GroupBy(item => item.ID).Select(g => g.First());
        }

    }
}
