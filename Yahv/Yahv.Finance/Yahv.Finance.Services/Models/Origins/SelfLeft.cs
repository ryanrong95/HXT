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
    /// 资金调拨 应调
    /// </summary>
    public class SelfLeft : IUnique
    {
        #region 属性
        public string ID { get; set; }

        /// <summary>
        /// 调入账户ID
        /// </summary>
        public string PayeeAccountID { get; set; }

        /// <summary>
        /// 调出账户ID
        /// </summary>
        public string PayerAccountID { get; set; }

        /// <summary>
        /// 调拨方式（调入、调出）
        /// </summary>
        public AccountMethord AccountMethord { get; set; }

        /// <summary>
        /// 调拨分类
        /// </summary>
        public string AccountCatalogID { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 本位币 币种
        /// </summary>
        public Currency Currency1 { get; set; }

        /// <summary>
        /// 本位币 汇率
        /// </summary>
        public decimal ERate1 { get; set; }

        /// <summary>
        /// 本位币 金额
        /// </summary>
        public decimal Price1 { get; set; }

        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplyID { get; set; }

        /// <summary>
        /// 创建人ID
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
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                //新增
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.SelfLefts>().Any(item => item.ID == this.ID))
                {
                    this.ID = PKeySigner.Pick(PKeyType.SelfLeft);
                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.SelfLefts()
                    {
                        Currency = (int)this.Currency,
                        CreateDate = DateTime.Now,
                        ID = this.ID,
                        Status = (int)this.Status,
                        CreatorID = this.CreatorID,
                        Price = this.Price,
                        Price1 = this.Price1,
                        ApplyID = this.ApplyID,
                        ERate1 = this.ERate1,
                        Currency1 = (int)this.Currency1,
                        PayerAccountID = this.PayerAccountID,
                        PayeeAccountID = this.PayeeAccountID,
                        AccountCatalogID = this.AccountCatalogID,
                        AccountMethord = (int)this.AccountMethord,
                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvFinance.SelfLefts>(new
                    {
                        Currency = (int)this.Currency,
                        Status = (int)this.Status,
                        Price = this.Price,
                        Price1 = this.Price1,
                        ApplyID = this.ApplyID,
                        ERate1 = this.ERate1,
                        Currency1 = (int)this.Currency1,
                        PayerAccountID = this.PayerAccountID,
                        PayeeAccountID = this.PayeeAccountID,
                        AccountCatalogID = this.AccountCatalogID,
                        AccountMethord = (int)this.AccountMethord,
                    }, item => item.ID == this.ID);
                }
            }
        }
        #endregion
    }
}