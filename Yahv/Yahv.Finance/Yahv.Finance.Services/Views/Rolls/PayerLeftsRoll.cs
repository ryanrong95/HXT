using System.Linq;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 付款应付 视图
    /// </summary>
    public class PayerLeftsRoll : UniqueView<PayerLeft, PvFinanceReponsitory>
    {
        public PayerLeftsRoll()
        {
        }

        public PayerLeftsRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<PayerLeft> GetIQueryable()
        {
            return new PayerLeftsOrigin(this.Reponsitory);
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon(string id)
        {
            new PayerLeft() { ID = id }.Abandon();
        }
    }
}