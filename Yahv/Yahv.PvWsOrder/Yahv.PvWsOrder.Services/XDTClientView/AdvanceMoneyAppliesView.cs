using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    /// <summary>
    /// 垫款额度
    /// </summary>
    public class AdvanceMoneyAppliesView
    {
        public decimal GetProductAdvanceMoneyApply(string clientID)
        {
            decimal advanceMoney = 0;

            using (Layers.Data.Sqls.ScCustomReponsitory reponsitory = new Layers.Data.Sqls.ScCustomReponsitory())
            {
                var result = (from advanceMoneyApply in reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.AdvanceMoneyApplies>()
                              where advanceMoneyApply.ClientID == clientID && advanceMoneyApply.Status == (int)AdvanceMoneyStatus.Effective
                              select new
                              {
                                  advanceMoneyApply.Amount,
                                  advanceMoneyApply.AmountUsed,
                              }).FirstOrDefault();

                if (result != null && result.Amount >= 0)
                {
                    advanceMoney = result.Amount - result.AmountUsed;
                }
            }

            return advanceMoney;
        }

        public AdvanceMoneyApply GetAdvanceMoneyApplyInfo(string clientID)
        {
            AdvanceMoneyApply model = null;

            using (Layers.Data.Sqls.ScCustomReponsitory reponsitory = new Layers.Data.Sqls.ScCustomReponsitory())
            {
                var result = (from advanceMoneyApply in reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.AdvanceMoneyApplies>()
                              where advanceMoneyApply.ClientID == clientID && advanceMoneyApply.Status == (int)AdvanceMoneyStatus.Effective
                              select new
                              {
                                  advanceMoneyApply.ID,
                                  advanceMoneyApply.ClientID,
                                  advanceMoneyApply.Amount,
                                  advanceMoneyApply.AmountUsed,
                              }).FirstOrDefault();

                if (result != null && result.Amount >= 0)
                {
                    model = new AdvanceMoneyApply
                    {
                        ID = result.ID,
                        ClientID = result.ClientID,
                        Amount = result.Amount,
                        AmountUsed = result.AmountUsed,
                    };
                }
            }

            return model;
        }
    }

    public class AdvanceMoneyApply
    {
        public string ID { get; set; }

        public string ClientID { get; set; }

        public decimal Amount { get; set; }

        public decimal AmountUsed { get; set; }
    }

    /// <summary>
    /// 垫资申请状态
    /// </summary>
    public enum AdvanceMoneyStatus
    {
        //[Description("待风控审核")]
        RiskAuditing = 1,

        //[Description("待审批")]
        Auditing = 2,

        //[Description("已生效")]
        Effective = 3,

        //[Description("作废")]
        Delete = 4
    }
}
