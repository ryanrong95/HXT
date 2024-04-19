
using NtErp.Wss.Sales.Services.Models.SsoUsers;
using NtErp.Wss.Sales.Services.Underly;
using NtErp.Wss.Sales.Services.Underly.Collections;
using NtErp.Wss.Sales.Services.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Model.SsoUsers
{
    /// <summary>
    /// 资产集合
    /// </summary>
    public class Assets : CumulateList<Asset>
    {
        public Assets() : base()
        {

        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <param name="type">账户类型 1现金余额 2 信用</param>
        public Assets(string userid, UserAccountType type)
        {
            var inputs = new UserInputsView(userid, type).ToArray().GroupBy(item => item.Currency);
            var outputs = new UserOutputsView(userid, type).ToArray().GroupBy(item => item.Currency);
            var assets = new List<Asset>();
            foreach (var input in inputs) // 多币种
            {
                var used = outputs.Count(item => item.Key == input.Key) > 0 ? outputs.SingleOrDefault(item => item.Key == input.Key).Sum(t => t.Amount as decimal?).GetValueOrDefault() : 0;
                //outputs.SingleOrDefault(item => item.Key == input.Key).Sum(t => t.Amount as decimal?).GetValueOrDefault(0);
                //if (type == UserAccountType.Credit)
                //{
                //    // 信用的已用金额和余额，则还需要计算还款
                //    var repay = new UserAccountsView().Where(item => item.UserID == userid && item.Currency == input.Key && item.Type == UserAccountType.Credit && item.Source == InputSource.CachToCredit)
                //        .Sum(item => item.Amount as decimal?).GetValueOrDefault();
                //    used = used - repay;
                //}
                assets.Add(new Asset
                {
                    Currency = input.Key,
                    Total = input.Sum(item => item.Amount),
                    Used = used
                });
            }

            this.source = assets;
        }

        /// <summary>
        /// 根据币种得到资产
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public Asset this[Currency currency]
        {
            get
            {
                return this.source.Single(item => item.Currency == currency);
            }
        }
    }
}
