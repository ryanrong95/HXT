using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using NtErp.Crm.Services.Extends;
using Needs.Overall;
using NtErp.Crm.Services.Enums;

namespace NtErp.Crm.Services.Models
{
    [Needs.Underly.FactoryView(typeof(Views.BeneficiariesAlls))]
    public class Beneficiaries : IUnique, IPersistence
    {
        public string Address
        {
            get;set;
        }

        public string Bank
        {
            get; set;
        }

        public string BankCode
        {
            get; set;
        }
        public Company Company
        {
            get; set;
        }
        public string CompanyID
        { 
            get
            {
                return this.Company.ID;
            }
            set
            {
                this.Company.ID = value;
            }
        }

        public DateTime CreateDate
        {
            get; set;
        }

        public string ID
        {
            get; set;
        }

        public Status Status
        {
            get; set;
        }

        public DateTime UpdateDate
        {
            get; set;
        }
        public Beneficiaries()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = Status.Normal;
        }

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        protected void OnAbandon()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.BvCrm.Beneficiaries>(new
                {
                    Status = Status.Delete
                }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                if (this != null && this.AbandonError != null)
                {
                    this.AbandonError(this, new ErrorEventArgs(this.ID));
                }
            }

            this.OnAbandon();

            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
        protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.Beneficiaries
                    {
                        ID = PKeySigner.Pick(PKeyType.Beneficiaries),
                        Bank=this.Bank,
                        BankCode=this.BankCode,
                        Address=this.Address,
                        CompanyID=this.CompanyID,
                        CreateDate = this.CreateDate,
                        Status = (int)this.Status,
                        UpdateDate = this.UpdateDate
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.BvCrm.Beneficiaries
                    {
                        ID = this.ID,
                        Bank = this.Bank,
                        BankCode = this.BankCode,
                        Address = this.Address,
                        CompanyID = this.CompanyID,
                        CreateDate = this.CreateDate,
                        Status = (int)this.Status,
                        UpdateDate = DateTime.Now
                    }, item => item.ID == this.ID);
                }
            }
        }

        /// <summary>
        /// 数据插入
        /// </summary>
        public void Enter()
        {
            this.OnEnter();
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}
