using System;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.Finance.Services.Models.Origins
{
    /// <summary>
    /// 应付（货款申请）
    /// </summary>
    public class PayerLeft : IUnique
    {
        #region 事件
        /// <summary>
        /// 成功
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// 失败
        /// </summary>
        public event ErrorHanlder EnterError;
        #endregion

        #region 属性
        public string ID { get; set; }
        /// <summary>
        /// 收款账户ID
        /// </summary>
        public string PayeeAccountID { get; set; }

        /// <summary>
        /// 付款账户ID
        /// </summary>
        public string PayerAccountID { get; set; }

        /// <summary>
        /// 付款公司ID
        /// </summary>
        public string PayerID { get; set; }

        /// <summary>
        /// 账款分类（付款类型）
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
        /// 创建时间
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
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.PayerLefts>().Any(item => item.ID == this.ID))
                {
                    this.ID = PKeySigner.Pick(PKeyType.PayerLeft);

                    reponsitory.Insert(new Layers.Data.Sqls.PvFinance.PayerLefts()
                    {
                        ID = this.ID,
                        Status = (int)this.Status,
                        CreateDate = DateTime.Now,
                        CreatorID = this.CreatorID,
                        AccountCatalogID = this.AccountCatalogID,
                        ApplyID = this.ApplyID,
                        Currency = (int)this.Currency,
                        Currency1 = (int)this.Currency1,
                        ERate1 = this.ERate1,
                        PayeeAccountID = this.PayeeAccountID,
                        PayerAccountID = this.PayerAccountID,
                        Price = this.Price,
                        Price1 = this.Price1,
                        PayerID = this.PayerID,
                    });
                }
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvFinance.PayerLefts>(new
                    {
                        Status = (int)this.Status,
                        CreatorID = this.CreatorID,
                        AccountCatalogID = this.AccountCatalogID,
                        ApplyID = this.ApplyID,
                        Currency = (int)this.Currency,
                        Currency1 = (int)this.Currency1,
                        ERate1 = this.ERate1,
                        PayeeAccountID = this.PayeeAccountID,
                        PayerAccountID = this.PayerAccountID,
                        Price = this.Price,
                        Price1 = this.Price1,
                        PayerID = this.PayerID,
                    }, item => item.ID == this.ID);
                }

                EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                reponsitory.Delete<Layers.Data.Sqls.PvFinance.PayerLefts>(item => item.ID == this.ID);
            }
        }
        #endregion
    }
}