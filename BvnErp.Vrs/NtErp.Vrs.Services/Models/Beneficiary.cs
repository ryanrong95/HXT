using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Underly;
using NtErp.Vrs.Services.Enums;
using Needs.Linq;
using Needs.Utils.Converters;

namespace NtErp.Vrs.Services.Models
{
    public class Beneficiary : IBeneficiary
    {
        #region 属性

        public string Address { set; get; }
        public string Bank { set; get; }
        public string ContactID { set; get; }
        public  string CompanyID { set; get; }
        public Company Company { set; get; }
        public Contact Contact { set; get; }
        public Currency Currency { set; get; }

        string id;
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Bank, this.Address + this.Currency).MD5();
            }
            set
            {
                id = value;
            }
        }

        public PayMethod Method { set; get; }

        public Status Status { set; get; }
        public string SwiftCode { set; get; }

        #endregion
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder EnterError;

      
        public event SuccessHanlder AbandonSuccess;
        public void Enter()
        {
            //this.Contact.Enter();

            using (Layer.Data.Sqls.BvnVrsReponsitory reponsitory = new Layer.Data.Sqls.BvnVrsReponsitory())
            {
                if (!reponsitory.ReadTable<Layer.Data.Sqls.BvnVrs.Beneficiaries>().Any(t => t.ID == this.ID))
                {
                    //if (string.IsNullOrWhiteSpace(this.ID))
                    //{
                    this.ID = Guid.NewGuid().ToString("N");
                        reponsitory.Insert(new Layer.Data.Sqls.BvnVrs.Beneficiaries
                        {
                            ID = this.ID,
                            Bank = this.Bank,
                            Method = (int)this.Method,
                            Currency = (int)this.Currency,
                            Address = this.Address,
                            SwiftCode = this.SwiftCode,
                            ContactID = this.ContactID,
                            CompanyID=this.CompanyID,
                            Status = (int)this.Status
                        });
                    }
             
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.BvnVrs.Beneficiaries>(new
                    {
                        Bank = this.Bank,
                        Method = (int)this.Method,
                        Currency = (int)this.Currency,
                        Address = this.Address,
                        SwiftCode = this.SwiftCode,
                        ContactID = this.ContactID,
                        Status = (int)this.Status
                    }, item => item.ID == this.ID);
                }
                if (EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this.ID));
                }
            }
        }

        public void Abandon()
        {
            using (Layer.Data.Sqls.BvnVrsReponsitory reponsitory = new Layer.Data.Sqls.BvnVrsReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.BvnVrs.Beneficiaries>
                  (item => item.ID == this.ID);
                if (EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this.ID));
                }
                //reponsitory.Update<Layer.Data.Sqls.BvnVrs.Beneficiaries>(new
                //{
                //    Status = Status.Limited
                //}, item => item.ID == this.ID);
            }
        }
    }
}
