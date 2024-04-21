using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 收款账户通用视图
    /// </summary>
    public class PayeesTopView : QueryView<wsPayee, PvbCrmReponsitory>
    {
        public PayeesTopView()
        {

        }

        public PayeesTopView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<wsPayee> GetIQueryable()
        {
            return new Yahv.Services.Views.wsPayeesTopView<PvbCrmReponsitory>();
        }

        /// <summary>
        /// 收款账户
        /// </summary>
        /// <param name="enterpriseID">企业ID</param>
        /// <param name="methord">收款方式</param>
        /// <param name="currency">币种</param>
        /// <returns></returns>
        public wsPayee this[string enterpriseID, Methord methord, Currency currency]
        {
            get
            {
                return
                    this.FirstOrDefault(
                        item =>
                            item.EnterpriseID == enterpriseID && item.Methord == methord && item.Currency == currency);
            }
        }

        /// <summary>
        /// 收款账户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public wsPayee this[string id]
        {
            get { return this.Single(item => item.ID == id); }
        }
    }
}
