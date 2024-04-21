using System;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 承兑调拨左表
    /// </summary>
    public class AcceptanceLeft : IUnique
    {
        #region 数据库属性
        public string ID { get; internal set; }
        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplyID { get; set; }
        /// <summary>
        /// 付款账户ID
        /// </summary>
        public string PayerAccountID { get; set; }
        /// <summary>
        /// 收款账户ID
        /// </summary>
        public string PayeeAccountID { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        public AccountMethord AccountMethord { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (var repository = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //新增
                if (string.IsNullOrEmpty(this.ID))
                {
                    this.ID = PKeySigner.Pick(PKeyType.AcceptanceLefts);
                    repository.Insert(new Layers.Data.Sqls.PvFinance.AcceptanceLefts()
                    {
                        ID = this.ID,
                        ApplyID = this.ApplyID,
                        PayerAccountID = this.PayerAccountID,
                        PayeeAccountID = this.PayeeAccountID,
                        AccountMethord = (int)this.AccountMethord,
                        Currency = (int)this.Currency,
                        Price = this.Price,
                        CreatorID = this.CreatorID,
                        CreateDate = DateTime.Now,
                        Status = (int)this.Status,
                    });
                }
                //修改
                else
                {
                    repository.Update<Layers.Data.Sqls.PvFinance.AcceptanceLefts>(new
                    {
                        ApplyID = this.ApplyID,
                        PayerAccountID = this.PayerAccountID,
                        PayeeAccountID = this.PayeeAccountID,
                        AccountMethord = (int)this.AccountMethord,
                        Currency = (int)this.Currency,
                        Price = this.Price,
                        Status = (int)this.Status,
                    }, item => item.ID == this.ID);
                }
            }
        }
        #endregion
    }
}