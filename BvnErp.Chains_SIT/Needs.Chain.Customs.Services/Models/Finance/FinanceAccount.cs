using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 金库账户
    /// </summary>
    public class FinanceAccount : IUnique, IPersistence, IFulError, IFulSuccess
    {
        #region

        public string ID { get; set; }

        /// <summary>
        /// 金库ID
        /// </summary>
        public string FinanceVaultID { get; set; }

        /// <summary>
        /// 金库名称
        /// </summary>
        public string FinanceVaultName { get; set; }

        /// <summary>
        /// 账户名称
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 银行地址
        /// </summary>
        public string BankAddress { get; set; }

        /// <summary>
        /// 银行编码
        /// </summary>
        public string SwiftCode { get; set; }

        /// <summary>
        /// 自定义代码
        /// </summary>
        public string CustomizedCode { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public decimal Balance { get; set; }


        public string AdminID { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public Admin Admin { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        /// <summary>
        /// 是否现金账户
        /// </summary>
        public bool IsCash { get; set; }
        /// <summary>
        /// 账户类型 一般户 基本户
        /// </summary>
        public AccountType AccountType { get; set; }
        /// <summary>
        /// 管理人
        /// </summary>
        public string AdminInchargeID { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 大赢家ID
        /// </summary>
        public string BigWinAccountID { get; set; }
        /// <summary>
        /// 国家地区
        /// </summary>
        public string Region { get; set; }

        public AccountSource AccountSource { get; set; }
        #endregion

        public FinanceAccount()
        {
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceAccounts>().Count(item => item.ID == this.ID);
                    if (count == 0)
                    {
                        //主键ID（FinanceAccount +8位年月日+6位流水号）
                        this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.FinanceAccount);
                        reponsitory.Insert(this.ToLinq());
                    }
                    else
                    {
                        reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                    }
                }
                this.OnEnterSuccess();
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }
        }
        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceAccounts>(
                        new
                        {
                            Status = Enums.Status.Delete
                        }, item => item.ID == this.ID);
                }
                this.OnAbandonSuccess(); ;
            }
            catch (Exception ex)
            {
                this.AbandonError(this, new ErrorEventArgs(ex.Message));
            }
        }
        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 更新账户余额
        /// </summary>
        public void UpdateBalance(decimal amount)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceAccounts>(new
                {
                    UpdateDate = DateTime.Now,
                    Balance = amount,
                }, item => item.ID == this.ID);
            }
        }
    }
}
