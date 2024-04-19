using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NtErp.Vrs.Services.Enums;
using Needs.Linq;
using Needs.Underly;

namespace NtErp.Vrs.Services.Models
{
    public partial class Vender : Company, IVender
    {
        #region 属性
        public string CompanyID
        {
            get
            {
                return this.ID;
            }

            private set
            {
                this.ID = value;
            }
        }
        public Grade Grade { set; get; }
        public string Properties { set; get; }
        public Status Status { set; get; }
        public ComapnyType Type { set; get; }

        #endregion
        public event SuccessHanlder AbandonVenderSuccess;
        public event SuccessHanlder EnterVenderSuccess;
        public Vender()
        {
            this.Status = Status.Nomal;
        }

        public Vender(Company company)
        {
            this.ID = company.ID;
            this.Name = company.Name;
            this.Address = company.Address;
            this.RegisteredCapital = company.RegisteredCapital;
            this.CorporateRepresentative = company.CorporateRepresentative;
            //补齐
        }

        override public void Abandon()
        {
            using (Layer.Data.Sqls.BvnVrsReponsitory reponsitory = new Layer.Data.Sqls.BvnVrsReponsitory())
            {
                if (reponsitory.ReadTable<Layer.Data.Sqls.BvnVrs.Companies>().Any(t => t.ID == this.ID))
                {
                    reponsitory.Delete<Layer.Data.Sqls.BvnVrs.Companies>(item => item.ID == this.ID);
                }
                //reponsitory.Update<Layer.Data.Sqls.BvnVrs.Venders>(new
                //{
                //    Status = Status.Limited
                //}, item => item.CompanyID == this.CompanyID);
                if (this != null && this.AbandonVenderSuccess != null)
                {
                    this.AbandonVenderSuccess(this, new SuccessEventArgs(this.ID));
                }
            }
        }
        override public void Enter()
        {
            base.Enter();
            using (Layer.Data.Sqls.BvnVrsReponsitory reponsitory = new Layer.Data.Sqls.BvnVrsReponsitory())
            {
                if (!reponsitory.ReadTable<Layer.Data.Sqls.BvnVrs.Companies>().Any(t => t.ID == this.ID))
                {
                    reponsitory.Insert(new Layer.Data.Sqls.BvnVrs.Companies
                    {
                        //CompanyID = this.CompanyID,
                        //Type = (int)this.Type,
                        //Status = (int)this.Status,
                        //Grade = (int)this.Grade,
                        //Properties = this.Properties.ToString()
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.BvnVrs.Companies>(new
                    {
                        Type = this.Type,
                        Grade = this.Grade,
                        Status = this.Status,
                        Properties = this.Properties,
                    }, item => item.ID == this.CompanyID);
                }
                if (this != null && this.EnterVenderSuccess != null)
                {
                    this.EnterVenderSuccess(this, new SuccessEventArgs(this.ID));
                }
            }
        }
    }
}
