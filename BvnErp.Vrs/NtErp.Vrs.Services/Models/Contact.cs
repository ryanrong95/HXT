using Layer.Data.Sqls.BvnVrs;
using Needs.Linq;
using NtErp.Vrs.Services.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Vrs.Services.Models
{
    public class Contact : IContact
    {
        #region 属性

        public string ID { set; get; }

        public string Name { set; get; }

        public string Email { set; get; }

        public string Birthday { set; get; }
        public string Mobile { set; get; }

        public bool Sex { set; get; }
        public string Tel { set; get; }
        public string CompanyID { set; get; }
        public Company Company { get; set; }
        public Status Status { set; get; }
        public  JobType   Job { get; set; }


        #endregion

        public event ErrorHanlder EnterError;

        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;
       
    
        public void Enter()
        {
            //this.Company.Enter();


            using (Layer.Data.Sqls.BvnVrsReponsitory reponsitory = new Layer.Data.Sqls.BvnVrsReponsitory())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = Guid.NewGuid().ToString("N");
                                 
                    reponsitory.Insert(new Layer.Data.Sqls.BvnVrs.Contacts
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Sex = this.Sex,
                        Birthday = this.Birthday,
                        Tel = this.Tel,
                        Email = this.Email,
                        Mobile = this.Mobile,
                        CompanyID = this.CompanyID,
                        Status = (int)this.Status,
                        Job =(int)this.Job
                    });
                }
                else
                {
                    //reponsitory.Update<Layer.Data.Sqls.BvnVrs.Contacts>(new
                    //{
                    //    Name = this.Name
                    //}, item => item.ID == this.ID);

                    reponsitory.Update<Layer.Data.Sqls.BvnVrs.Contacts>(new
                    {
                        Name = this.Name,
                        Sex = this.Sex,
                        Birthday = this.Birthday,
                        Tel = this.Tel,
                        Email = this.Email,
                        Mobile = this.Mobile,
                        CompanyID = this.CompanyID,
                        Status = (int)this.Status,
                        Job=(int)this.Job
                    }, item => item.ID == this.ID);
                }
                if (EnterSuccess!=null)
                {
                    this.EnterSuccess(this, new SuccessEventArgs(this.ID));
                }
            }
          
        }
        public void Abandon()
        {
            using (Layer.Data.Sqls.BvnVrsReponsitory reponsitory = new Layer.Data.Sqls.BvnVrsReponsitory())
            {

                reponsitory.Delete<Layer.Data.Sqls.BvnVrs.Contacts>
                    (item => item.ID == this.ID);
                if (AbandonSuccess!=null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
                }
                //reponsitory.Update<Layer.Data.Sqls.BvnVrs.Contacts>(new
                //{
                //    Status = Status.Limited
                //}, item => item.CompanyID == this.CompanyID);
                //if (this != null && this.EnterSuccess != null)
                //{
                //    this.EnterSuccess(this, new SuccessEventArgs(this.ID));
                //}
            }
        }
    }
}
