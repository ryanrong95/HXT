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
    /// 没有MapsBEnter关系，不区分业务的到货地址（库房门牌在使用，内部公司在使用）
    /// </summary>
    public class ConsigneesRoll : Origins.ConsigneesOrigin
    {
        Enterprise enterprise;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public ConsigneesRoll(Enterprise enterprise = null)
        {
            this.enterprise = enterprise;
        }
        protected override IQueryable<Consignee> GetIQueryable()
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
    ///代仓储客户的到货地址
    /// </summary>
    public class WsConsigneesRoll : Origins.WsConsigneesOrigin
    {
        Enterprise enterprise;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public WsConsigneesRoll(Enterprise enterprise)
        {
            this.enterprise = enterprise;
        }
        protected override IQueryable<WsConsignee> GetIQueryable()
        {
            return from item in base.GetIQueryable()
                   where item.EnterpriseID == this.enterprise.ID
                   select item;

        }
    }


    ///根据业务和企业查询到货地址
    ///
    public class TradingConsigneesRoll : Origins.ServiceConsigneesOrigin
    {
        Enterprise enterprise;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public TradingConsigneesRoll(Enterprise enterprise)
        {
            this.enterprise = enterprise;
        }
        protected override IQueryable<TradingConsignee> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.EnterpriseID == this.enterprise.ID).GroupBy(item => item.ID).Select(g => g.First());
        }
    }
}
