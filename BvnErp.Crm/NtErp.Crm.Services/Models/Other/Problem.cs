using NtErp.Crm.Services.Extends;
using Needs.Linq;
using Needs.Overall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Erp.Generic;

namespace NtErp.Crm.Services.Models
{
    public class Problem : IUnique,IPersistence
    {
        #region
        public string ID
        {
            get; set;
        }

        public string ActionID
        {
            get; set;
        }

        public string StandardID
        {
            get; set;
        }

        public string ReportID
        {
            get; set;
        }

        public string ContactID
        {
            get; set;
        }

        public string Context
        {
            get; set;
        }

        public string Answer
        {
            get; set;
        }

        public string AdminID
        {
            get; set;
        }
        public DateTime CreateDate
        {
            get; set;
        }

        public DateTime UpdateDate
        {
            get; set;
        }

        public Status Status
        {
            get; set;
        }
        //public string AdminName
        //{
        //    get
        //    {
        //        return new Views.AdminsTopView()[this.AdminID].UserName;
        //    }
        //}
        //public string ContactName
        //{
        //    get
        //    {
        //        return new Views.ContactsView()[this.ContactID].Name;
        //    }
        //}
        public Contact Contact
        {
            get;set;
        }
        #endregion


        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;

        public void Abandon()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                this.Status = Status.Delete;
                reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
            }
            if (this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = PKeySigner.Pick(PKeyType.Problem);
                    this.CreateDate = this.UpdateDate = DateTime.Now;
                    this.Status = Status.Normal;
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    this.UpdateDate = DateTime.Now;
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
                if (this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this.ID));
                }
            }
        }
    }
}
