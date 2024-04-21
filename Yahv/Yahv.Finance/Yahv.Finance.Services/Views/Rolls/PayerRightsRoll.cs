using System;
using System.Collections.Generic;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Rolls
{
    /// <summary>
    /// 付款核销记录
    /// </summary>
    public class PayerRightsRoll : UniqueView<PayerRight, PvFinanceReponsitory>
    {
        public PayerRightsRoll()
        {
        }

        public PayerRightsRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<PayerRight> GetIQueryable()
        {
            return new PayerRightsOrigin(this.Reponsitory);
        }

        public void Abandon(string id)
        {
            new PayerRight() { ID = id }.Abandon();
        }

        public void AddRange(IEnumerable<PayerRight> list)
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                var data = list.Select(item => new Layers.Data.Sqls.PvFinance.PayerRights()
                {
                    ID = item.ID ?? PKeySigner.Pick(PKeyType.PayerRight),
                    Currency = (int)item.Currency,
                    CreateDate = item.CreateDate,
                    CreatorID = item.CreatorID,
                    Price = item.Price,
                    FlowID = item.FlowID,
                    Currency1 = (int)item.Currency1,
                    ERate1 = item.ERate1,
                    Price1 = item.Price1,
                    PayerLeftID = item.PayerLeftID,
                });
                reponsitory.Insert(data);
            }
        }
    }
}