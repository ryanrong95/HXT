using Needs.Linq;
using Needs.Overall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NtErp.Vrs.Services.Extends;
using NtErp.Vrs.Services.Enums;

namespace NtErp.Vrs.Services.Models
{
    public class Invoice : IInvoice
    {
        #region 属性
        public string ID { set; get; }
        public bool Required { set; get; }
        public InvoiceType Type { set; get; }
        public string Account { set; get; }
        public string Address { set; get; }
        public string Bank { set; get; }
        public string BankAddress { set; get; }
        public string CompanyID { set; get; }
        public string ContactID { set; get; }
        public string Postzip { set; get; }
        public string SwiftCode { set; get; }
        public Company Company { set; get; }
        public Contact Contact { set; get; }
        #endregion
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder EnterError;
        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.BvnVrsReponsitory reponsitory = new Layer.Data.Sqls.BvnVrsReponsitory())
                {
                    if (string.IsNullOrEmpty(this.ID))
                    {
                        this.ID = System.Guid.NewGuid().ToString("N");
                        //this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.Invoice);
                        reponsitory.Insert(this.ToLinq());
                    }
                    else
                    {
                        reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                    }
                    if (this != null && this.EnterSuccess != null)
                    {
                        this.EnterSuccess(this, new SuccessEventArgs(this.ID));
                    }
                }
            }
            catch (Exception ex)
            {
                if (this != null && this.EnterError != null)
                {
                    this.EnterError(this, new ErrorEventArgs(ex.Message, ErrorType.System));
                }
            }
        }
        public void Abandon()
        {
            using (Layer.Data.Sqls.BvnVrsReponsitory reponsitory = new Layer.Data.Sqls.BvnVrsReponsitory())
            {
                reponsitory.Delete<Layer.Data.Sqls.BvnVrs.Invoices>(item => item.ID == this.ID);
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
                }
            }
        }


    }
}
