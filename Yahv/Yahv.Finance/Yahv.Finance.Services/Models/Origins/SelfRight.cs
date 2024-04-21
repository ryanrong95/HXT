using System;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 资金调拨 实调
    /// </summary>
    public class SelfRight : IUnique
    {
        #region 属性
        public string ID { get; set; }

        /// <summary>
        /// 应调ID
        /// </summary>
        public string SelfLeftID { get; set; }

        /// <summary>
        /// 来源币种
        /// </summary>
        public Currency OriginCurrency { get; set; }

        /// <summary>
        /// 目标币种
        /// </summary>
        public Currency TargetCurrency { get; set; }

        /// <summary>
        /// 来源币种到目标币种的汇率
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// 来源金额
        /// </summary>
        public decimal OriginPrice { get; set; }

        /// <summary>
        /// 目标金额
        /// </summary>
        public decimal TargetPrice { get; set; }

        /// <summary>
        /// 来源本位币种1
        /// </summary>
        public Currency OriginCurrency1 { get; set; }

        /// <summary>
        /// 来源本位金额1
        /// </summary>
        public decimal OriginPrice1 { get; set; }

        /// <summary>
        /// 来源本位币种汇率1
        /// </summary>
        public decimal OriginERate1 { get; set; }

        /// <summary>
        /// 目标本位币种1
        /// </summary>
        public Currency TargetCurrency1 { get; set; }

        /// <summary>
        /// 目标本位金额1
        /// </summary>
        public decimal TargetPrice1 { get; set; }

        /// <summary>
        /// 目标本位币种汇率1
        /// </summary>
        public decimal TargetERate1 { get; set; }

        /// <summary>
        /// 流水ID
        /// </summary>
        public string FlowID { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        #endregion

        #region 持久化

        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvFinance.SelfRights()
                {
                    CreateDate = DateTime.Now,
                    ID = PKeySigner.Pick(PKeyType.SelfRight),
                    CreatorID = this.CreatorID,
                    FlowID = this.FlowID,
                    OriginCurrency = (int)this.OriginCurrency,
                    OriginCurrency1 = (int)this.OriginCurrency1,
                    OriginERate1 = this.OriginERate1,
                    OriginPrice = this.OriginPrice,
                    OriginPrice1 = this.OriginPrice1,
                    Rate = this.Rate,
                    SelfLeftID = this.SelfLeftID,
                    TargetCurrency = (int)this.TargetCurrency,
                    TargetCurrency1 = (int)this.TargetCurrency1,
                    TargetERate1 = this.TargetERate1,
                    TargetPrice = this.TargetPrice,
                    TargetPrice1 = this.TargetPrice1,
                });
            }
        }
        #endregion
    }
}