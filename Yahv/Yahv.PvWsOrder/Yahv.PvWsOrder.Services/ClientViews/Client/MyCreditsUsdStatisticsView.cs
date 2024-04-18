using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Services.Models;
using Yahv.Services.Views;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class MyCreditsUsdStatisticsView : CreditsUsdStatisticsView<PvWsOrderReponsitory>
    {
        private string clientid;

        /// <summary>
        /// 无参构造函数
        /// </summary>
        private MyCreditsUsdStatisticsView()
        {

        }

        /// <summary>
        /// 当前客户的信用统计
        /// </summary>
        /// <param name="EnterpriseID"></param>
        public MyCreditsUsdStatisticsView(string EnterpriseID)
        {
            this.clientid = EnterpriseID;
        }

        /// <summary>
        /// 查询结果集
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<CreditStatistic> GetIQueryable()
        {
            //获取当前平台公司ID
            string CompanyID = PvClientConfig.CYCompanyID;

            //按照需要，这一期先做人民币的信用批复
            var linq = base.GetIQueryable().Where(item => item.Payer == this.clientid && item.Payee == CompanyID);

            return linq;
        }
    }
}
