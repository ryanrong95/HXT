using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Usually;
using YaHv.CrmPlus.Services.Models.Origins;

namespace Yahv.CrmPlus.Service.Models.Origins
{
    public class BookAccount : Yahv.Linq.IUnique
    {
        public BookAccount()
        {
            this.CreateDate = DateTime.Now;
            this.Status = DataStatus.Normal;
        }
        #region  属性
        public string ID { get; set; }

        public string EnterpriseID { get; set; }

        public Enterprise Enterprise { get; set; }

        /// <summary>
        /// 账户类型
        /// </summary>
        public RelationType RelationType { get; set; }

        /// <summary>
        /// 账户类型
        /// </summary>
        public BookAccountType BookAccountType { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public BookAccountMethord BookAccountMethord { get; set; }
        /// <summary>
        /// 银行
        /// </summary>
        public string Bank { get; set; }
        /// <summary>
        /// 开户行地址
        /// </summary>
        public string BankAddress { get; set; }
        /// <summary>
        /// 行号
        /// </summary>
        public string BankCode { get; set; }
        /// <summary>
        /// 可能是 支付宝、微信、银行帐号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }
        /// <summary>
        /// SwiftCode
        /// </summary>
        public string SwiftCode { get; set; }
        /// <summary>
        /// 中转银行
        /// </summary>
        public string Transfer { get; set; }
        /// <summary>
        /// 是否为个人
        /// </summary>
        public bool IsPersonal { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 所属人
        /// </summary>
        public Admin Admin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DataStatus Status { get; set; }

        //public Nature Nature { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }

        #endregion

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvdCrmReponsitory>.Create())
            {
                //添加
                if (!reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.BookAccounts>().Any(item => item.ID == this.ID))
                {
                    this.ID = PKeySigner.Pick(Yahv.CrmPlus.Service.PKeyType.BookAccounts);
                    reponsitory.Insert(new Layers.Data.Sqls.PvdCrm.BookAccounts()
                    {
                        ID = this.ID,
                        EnterpriseID = this.EnterpriseID,
                        RelationType = (int)this.RelationType,
                        Type = (int)this.BookAccountType,
                        Methord = (int)this.BookAccountMethord,
                        Bank = this.Bank,
                        BankAddress = this.BankAddress,
                        BankCode = this.BankCode,
                        Account = this.Account,
                        Currency = (int)this.Currency,
                        SwiftCode = this.SwiftCode,
                        Transfer = this.Transfer,
                        Status = (int)this.Status,
                        IsPersonal = this.IsPersonal,
                        CreatorID = this.CreatorID,
                        CreateDate = this.CreateDate
                    });
                }
                //修改
                else
                {
                    reponsitory.Update<Layers.Data.Sqls.PvdCrm.BookAccounts>(new
                    {
                        //ID = this.ID,
                        //EnterpriseID = this.EnterpriseID,
                        //RelationType = (int)this.RelationType,
                        Type = (int)this.BookAccountType,
                        Methord = (int)this.BookAccountMethord,
                        Bank = this.Bank,
                        BankAddress = this.BankAddress,
                        BankCode = this.BankCode,
                        Account = this.Account,
                        Currency = (int)this.Currency,
                        SwiftCode = this.SwiftCode,
                        Transfer = this.Transfer,
                        Status = (int)this.Status,
                        IsPersonal = this.IsPersonal,
                        //CreatorID = this.CreatorID
                    }, item => item.ID == this.ID);
                }

                this.EnterSuccess?.Invoke(this, new SuccessEventArgs(this));
            }
        }


        public void Closed()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.BookAccounts>(new
                {
                    Status = DataStatus.Closed
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        public void Enable()
        {
            using (var repository = LinqFactory<PvdCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvdCrm.BookAccounts>(new
                {
                    Status = DataStatus.Normal
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        #endregion

        #region 事件
        public event SuccessHanlder EnterSuccess;

        public event SuccessHanlder AbandonSuccess;
        /// <summary>
        /// EnterError
        /// </summary>
        public event ErrorHanlder EnterError;
        #endregion
    }
}
