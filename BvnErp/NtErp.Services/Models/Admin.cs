using Layer.Data.Sqls;
using Needs.Linq;

using Needs.Settings;
using Needs.Utils.Converters;
using Needs.Utils.Http;
using NtErp.Services.Views;
using System;
using System.Linq;
using NtErp.Services.Extends;

namespace NtErp.Services.Models
{
    /// <summary>
    /// 管理员
    /// </summary>
    /// <example>
    /// 这是管理员视图
    /// </example>
    public partial class Admin : AdminTop, Needs.Linq.IUnique
    {
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder EnterError;

        public event SuccessHanlder AbandonSuccess;

        public string Password { get; set; }


        public string Summary { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public DateTime? LoginDate { get; set; }

        public Needs.Erp.Generic.Status Status { get; set; }


        public Admin()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = Needs.Erp.Generic.Status.Normal;
        }

        public void Abandon()
        {
            using (var repository = new BvnErpReponsitory())
            {
                this.Status = Needs.Erp.Generic.Status.Delete;
                repository.Update(this.ToLinq(), item => item.ID == this.ID);
            }

            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this));
            }
        }

        public void Enter()
        {

            using (var repository = new Layer.Data.Sqls.BvnErpReponsitory())
            using (var alls = new AdminsAlls())
            {

#if true
                this.Password = this.Password.PasswordOld();
#else
                    this.Password = this.Password.Password();                        
#endif

                if (string.IsNullOrEmpty(this.ID))
                {
                    if (alls.Any(item => item.UserName == this.UserName || item.RealName == this.RealName))
                    {
                        if (this != null && this.EnterError != null)
                        {
                            this.EnterError(this, new ErrorEventArgs("Accounts Repeat!!", ErrorType.Repeated));
                        }
                        return;
                    }

                    this.ID = Needs.Overall.PKeySigner.Pick(Needs.Overall.PKeyType.Admin);
                    this.CreateDate = this.UpdateDate = DateTime.Now;
                    this.Status = Needs.Erp.Generic.Status.Normal;
                    repository.Insert(this.ToLinq());
                }
                else
                {
                    this.UpdateDate = DateTime.Now;
                    repository.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }

            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }

        }

        
    }
}
