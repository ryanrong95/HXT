using Layers.Data.Sqls;
using Layers.Linq;
using System.Linq;
using Yahv.Underly;
using Yahv.Usually;
using YaHv.Csrm.Services.Models.Origins;

namespace YaHv.Csrm.Services.Views.Rolls
{
    /// <summary>
    /// 内部公司联系人，
    /// </summary>
    public class InvoicesRoll : Origins.InvoicesOrigin
    {
        Enterprise enterprise;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public InvoicesRoll(Enterprise enterprise = null)
        {
            this.enterprise = enterprise;
        }
        protected override IQueryable<Invoice> GetIQueryable()
        {
            if (enterprise == null)
            {
                return from item in base.GetIQueryable()
                       select item;
            }
            else
            {
                return from item in base.GetIQueryable()
                       where item.EnterpriseID == this.enterprise.ID
                       select item;
            }

        }
    }

    public class WsInvoicesRoll : Origins.WsInvoicesOrigin
    {
        Enterprise Enterprise;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public WsInvoicesRoll(Enterprise enterprise)
        {
            this.Enterprise = enterprise;
        }
        protected override IQueryable<WsInvoice> GetIQueryable()
        {
            return from item in base.GetIQueryable()
                   where item.EnterpriseID == this.Enterprise.ID
                   select item;
        }
    }

    /// <summary>
    /// 传统贸易下的发票
    /// </summary>
    public class TradingInvoicesRoll : Origins.TradingInvoiceOrigin
    {
        Enterprise enterprise;
        /// <summary>
        /// 默认构造器
        /// </summary>
        public TradingInvoicesRoll(Enterprise enterprise)
        {
            this.enterprise = enterprise;
        }
        protected override IQueryable<TradingInvoice> GetIQueryable()
        {
            return base.GetIQueryable().Where(item => item.Enterprise.ID == this.enterprise.ID).GroupBy(item => item.ID).Select(g => g.First());

        }
    }
}
