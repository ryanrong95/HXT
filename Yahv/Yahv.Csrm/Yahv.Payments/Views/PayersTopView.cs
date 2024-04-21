using System.Linq;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 付款账户通用视图
    /// </summary>
    public class PayersTopView : QueryView<wsPayer, PvbCrmReponsitory>
    {
        public PayersTopView()
        {

        }

        public PayersTopView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<wsPayer> GetIQueryable()
        {
            return new Yahv.Services.Views.wsPayersTopView<PvbCrmReponsitory>();
        }

        /// <summary>
        /// 付款账户
        /// </summary>
        /// <param name="enterpriseID">企业ID</param>
        /// <param name="methord">付款方式</param>
        /// <param name="currency">币种</param>
        /// <returns></returns>
        public wsPayer this[string enterpriseID, Underly.Methord methord, Underly.Currency currency]
        {
            get
            {
                return this.FirstOrDefault(item => item.EnterpriseID == enterpriseID && item.Methord == methord && item.Currency == currency);
            }
        }
    }
}

namespace Yahv.Payments.Views.Behands
{
    /// <summary>
    /// 付款账户通用视图
    /// </summary>
    public class PayersTopView : QueryView<Yahv.Services.Models.Behands.wsPayer, PvbCrmReponsitory>
    {
        public PayersTopView()
        {

        }

        public PayersTopView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Yahv.Services.Models.Behands.wsPayer> GetIQueryable()
        {
            var linq = from entity in new Yahv.Services.Views.wsPayersTopView<PvbCrmReponsitory>()
                       select new Yahv.Services.Models.Behands.wsPayer
                       {

                       };

            return linq;
        }

        /// <summary>
        /// 付款账户
        /// </summary>
        /// <param name="enterpriseID">企业ID</param>
        /// <param name="methord">付款方式</param>
        /// <param name="currency">币种</param>
        /// <returns></returns>
        public Yahv.Services.Models.Behands.wsPayer this[string enterpriseID, Underly.Methord methord, Underly.Currency currency]
        {
            get
            {
                return this.FirstOrDefault(item => item.EnterpriseID == enterpriseID
                    && item.Methord == methord && item.Currency == currency);
            }
        }
    }
}
