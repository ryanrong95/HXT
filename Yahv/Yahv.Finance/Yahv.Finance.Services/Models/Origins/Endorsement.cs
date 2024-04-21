using System;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 背书转让
    /// </summary>
    public class Endorsement : IUnique, IDataEntity
    {
        #region 数据库属性
        public string ID { get; set; }

        /// <summary>
        /// 汇票ID
        /// </summary>
        public string MoneyOrderID { get; set; }

        /// <summary>
        /// 背书人账户ID
        /// </summary>
        public string PayerAccountID { get; set; }

        /// <summary>
        /// 被背书人账户ID
        /// </summary>
        public string PayeeAccountID { get; set; }

        /// <summary>
        /// 背书日期
        /// </summary>
        public DateTime EndorseDate { get; set; }

        /// <summary>
        /// 是否允许转让
        /// </summary>
        public bool IsTransfer { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var repository = LinqFactory<PvFinanceReponsitory>.Create())
            {
                this.ID = PKeySigner.Pick(PKeyType.Endorsements);

                repository.Insert(new Layers.Data.Sqls.PvFinance.Endorsements()
                {
                    CreateDate = DateTime.Now,
                    CreatorID = this.CreatorID,
                    EndorseDate = this.EndorseDate,
                    ID = this.ID,
                    PayerAccountID = this.PayerAccountID,
                    Summary = this.Summary,
                    IsTransfer = this.IsTransfer,
                    MoneyOrderID = this.MoneyOrderID,
                    PayeeAccountID = this.PayeeAccountID,
                });
            }
        }
        #endregion
    }
}