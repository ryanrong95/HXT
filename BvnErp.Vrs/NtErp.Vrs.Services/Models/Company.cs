using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NtErp.Vrs.Services.Enums;
using NtErp.Vrs.Services.Extends;

namespace NtErp.Vrs.Services.Models
{
    public class Company : SubjectBase, ICompany
    {
        #region 属性
        public ComapnyType Type { set; get; }
        public string Code { set; get; }
        public string Address { set; get; }
        public string RegisteredCapital { set; get; }
        public string CorporateRepresentative { set; get; }
        public string Summary { set; get; }
        public DateTime CreateDate { set; get; }
        public DateTime UpdateDate { set; get; }
        public Company()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }
        #endregion
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;
        override public void Enter()
        {
            using (Layer.Data.Sqls.BvnVrsReponsitory reponsitory = new Layer.Data.Sqls.BvnVrsReponsitory())
            {
                if (!reponsitory.ReadTable<Layer.Data.Sqls.BvnVrs.Companies>().Any(t => t.ID == this.ID))
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    this.UpdateDate = DateTime.Now;
                    reponsitory.Update<Layer.Data.Sqls.BvnVrs.Companies>(this.ToLinq(), item => item.ID == this.ID);
                }
                if (this != null && this.EnterSuccess != null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this.ID));
                }
            }
        }
        override public void Abandon()
        {
            var message = string.Empty;
            using (Layer.Data.Sqls.BvnVrsReponsitory reponsitory = new Layer.Data.Sqls.BvnVrsReponsitory())
            {
                if (!reponsitory.ReadTable<Layer.Data.Sqls.BvnVrs.Contacts>().Any(t => t.CompanyID == this.ID) && !reponsitory.ReadTable<Layer.Data.Sqls.BvnVrs.Beneficiaries>().Any(t => t.CompanyID == this.ID))
                {
                    reponsitory.Delete<Layer.Data.Sqls.BvnVrs.Companies>(item => item.ID == this.ID);
                }
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
                }
            }
        }
    }
}
