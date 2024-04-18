using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Usually;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 申请中客户的付款人
    /// </summary>
    public class ApplicationPayer : IUnique
    {
        #region 属性
        public string ID { get; set; }

        public string ApplicationID { get; set; }

        /// <summary>
        /// 付款人ID（来自Crm的Payer表）
        /// </summary>
        public string PayerID { get; set; }

        public string EnterpriseID { get; set; }

        /// <summary>
        /// 企业名称
        /// </summary>
        public string EnterpriseName { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public Methord? Method { get; set; }

        /// <summary>
        /// 支付币种
        /// </summary>
        public Currency? Currency { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal? Amount { get; set; }

        #endregion

        #region 扩展属性
        
        public string Contact { get; set; }
        
        #endregion

        public ApplicationPayer()
        {
        }

        #region 持久化

        public void Enter()
        {
            using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
            {
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ApplicationPayers>().Any(item => item.ID == this.ID))
                {
                    this.ID = Guid.NewGuid().ToString();
                    reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.ApplicationPayers
                    {
                        ID = this.ID,
                        ApplicationID = this.ApplicationID,
                        PayerID = this.PayerID,
                        EnterpriseID = this.EnterpriseID,
                        EnterpriseName = this.EnterpriseName,
                        BankName = this.BankName,
                        BankAccount = this.BankAccount,
                        Method = (int)this.Method,
                        Currency = (int)this.Currency,
                        Amount = this.Amount
                    });
                }
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.ApplicationPayers>(new
                    {
                        PayerID = this.PayerID,
                        EnterpriseID = this.EnterpriseID,
                        EnterpriseName = this.EnterpriseName,
                        BankName = this.BankName,
                        BankAccount = this.BankAccount,
                        Method = (int)this.Method,
                        Currency = (int)this.Currency,
                        Amount = this.Amount
                    }, item => item.ID == this.ID);
                }
            };
        }

        public void Abandon()
        {
            using (PvWsOrderReponsitory Reponsitory = new PvWsOrderReponsitory())
            {
                Reponsitory.Delete<Layers.Data.Sqls.PvWsOrder.ApplicationPayers>(item => item.ID == this.ID);
            }
        }

        #endregion
    }
}
