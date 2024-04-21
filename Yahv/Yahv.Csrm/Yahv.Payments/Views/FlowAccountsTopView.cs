using System.Linq;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 流水表通用视图
    /// </summary>
    public class FlowAccountsTopView : UniqueView<FlowAccount, PvbCrmReponsitory>
    {
        public FlowAccountsTopView()
        {

        }

        public FlowAccountsTopView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }
        protected override IQueryable<FlowAccount> GetIQueryable()
        {
            return new Yahv.Services.Views.FlowAccountsTopView<PvbCrmReponsitory>();
        }

        /// <summary>
        /// 根据币种和银行流水获取余额
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public FlowAccount this[Currency currency, string code]
        {
            get
            {
                Services.Models.FlowAccount entity = this.FirstOrDefault(item => item.FormCode == code && item.Currency == currency && item.Price > 0);

                if (entity == null)
                {
                    entity = new Services.Models.FlowAccount();
                }
                else
                {
                    //将金额赋值为总额
                    entity.Price = this.Where(item => item.FormCode == code && item.Currency == currency)
                   ?.Sum(item => item.Price) ?? 0;
                }

                return entity;
            }
        }
    }
}