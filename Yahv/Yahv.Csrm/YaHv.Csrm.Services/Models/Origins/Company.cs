using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using YaHv.Csrm.Services.Extends;

namespace YaHv.Csrm.Services.Models.Origins
{

    /// <summary>
    /// 内部公司
    /// </summary>
    public class Company : Yahv.Linq.IUnique
    {

        #region 构造函数
        public Company()
        {
            this.CompanyStatus = ApprovalStatus.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 唯一码
        /// </summary>
        /// <chenhan>保障全局唯一</chenhan>
        public string ID { set; get; }
        /// <summary>
        /// 公司
        /// </summary>
        public CompanyType Type { set; get; }
        /// <summary>
        /// 范围
        /// </summary>
        public AreaType Range { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus CompanyStatus { set; get; }
        /// <summary>
        /// 基本信息
        /// </summary>
        public Enterprise Enterprise { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }
        public string CreatorID { set; get; }
        /// <summary>
        /// 添加人
        /// </summary>
        public Admin Admin { get; internal set; }
        #endregion



        #region 扩展（发票，到货地址，联系人，受益人）
        Views.Rolls.InvoicesRoll invoices;
        /// <summary>
        /// 客户发票信息
        /// </summary>
        public Views.Rolls.InvoicesRoll Invoices
        {
            get
            {
                if (this.invoices == null || this.invoices.Disposed)
                {
                    this.invoices = new Views.Rolls.InvoicesRoll(this.Enterprise);
                }
                return this.invoices;
            }
        }

        Views.Rolls.ConsigneesRoll consignees;
        /// <summary>
        /// 客户到货地址
        /// </summary>
        public Views.Rolls.ConsigneesRoll Consignees
        {
            get
            {
                if (this.consignees == null || this.consignees.Disposed)
                {
                    this.consignees = new Views.Rolls.ConsigneesRoll(this.Enterprise);
                }
                return this.consignees;
            }
        }

        Views.Rolls.ContactsRoll contacts;
        /// <summary>
        /// 客户联系人信息
        /// </summary>
        public Views.Rolls.ContactsRoll Contacts
        {
            get
            {
                if (this.contacts == null || this.contacts.Disposed)
                {
                    this.contacts = new Views.Rolls.ContactsRoll(this.Enterprise);
                }
                return this.contacts;
            }
        }

        Views.Rolls.BeneficiariesRoll beneficiaries;
        /// <summary>
        /// 客户受益人信息
        /// </summary>
        public Views.Rolls.BeneficiariesRoll Beneficiaries
        {
            get
            {
                if (this.beneficiaries == null || this.beneficiaries.Disposed)
                {
                    this.beneficiaries = new Views.Rolls.BeneficiariesRoll(this.Enterprise);
                }
                return this.beneficiaries;
            }
        }

        Views.Rolls.PayeesRoll payees;
        /// <summary>
        /// 收款人
        /// </summary>
        public Views.Rolls.PayeesRoll Payees
        {
            get
            {
                if (payees == null)
                {
                    using (var view = new Views.Rolls.PayeesRoll(this.Enterprise.ID))
                    {
                        return payees = view;
                    }
                }
                else
                {
                    return payees;
                }
            }
            set
            {

                this.payees = value;
            }
        }
        Views.Rolls.PayersRoll payers;
        /// <summary>
        /// 付款人
        /// </summary>
        public Views.Rolls.PayersRoll Payers
        {
            get
            {
                if (payers == null)
                {
                    using (var view = new Views.Rolls.PayersRoll(this.Enterprise.ID))
                    {
                        return payers = view;
                    }
                }
                else
                {
                    return payers;
                }
            }
            set
            {

                this.payers = value;
            }
        }
        #endregion


        #region 事件
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;
        /// <summary>
        /// 状态异常
        /// </summary>
        public event ErrorHanlder StatusUnnormal;
        #endregion


        #region 持久化
        public void Enter()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                this.Enterprise.Enter();
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    //1.Company存在
                    if (repository.GetTable<Layers.Data.Sqls.PvbCrm.Companies>().Any(item => item.ID == this.Enterprise.ID))
                    {
                        this.CompanyStatus = new Views.Rolls.CompaniesRoll()[this.Enterprise.ID].CompanyStatus;
                        if (this != null && this.StatusUnnormal != null)
                        {
                            this.StatusUnnormal(this, new ErrorEventArgs());
                        }
                    }
                    //2.Company不存在
                    else
                    {
                        this.ID = this.Enterprise.ID;
                        repository.Insert(this.ToLinq());
                    }
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.Companies>(new
                    {
                        Type = (int)this.Type,
                        Range = (int)this.Range,
                        Status = (int)this.CompanyStatus,
                        UpdateDate = this.UpdateDate
                    }, item => item.ID == this.ID);
                }

                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this));
                }
            }
        }
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                repository.Update<Layers.Data.Sqls.PvbCrm.Companies>(new
                {
                    CompanyStatus = ApprovalStatus.Deleted
                }, item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }

        }

        #endregion
    }

    public class InternalWsClients : Company
    {
        internal Views.Rolls.WsClientsRoll wsclients;
        public Views.Rolls.WsClientsRoll WsClients
        {
            get
            {
                if (this.wsclients == null || this.wsclients.Disposed)
                {
                    this.wsclients = new Views.Rolls.WsClientsRoll();
                }
                return this.wsclients;
            }
        }
    }
}
