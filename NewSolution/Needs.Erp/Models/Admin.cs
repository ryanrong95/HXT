using Layer.Data.Sqls;
using Needs.Linq;

using Needs.Settings;
using Needs.Utils.Converters;
using Needs.Utils.Http;
using System;
using System.Linq;

namespace Needs.Erp.Models
{
    /// <summary>
    /// 管理员
    /// </summary>
    /// <example>
    /// 这是管理员视图
    /// </example>
    public partial class Admin : IAdmin
    {
        public event SuccessHanlder PasswordSuccess;
        public event ErrorHanlder PasswordError;

        //static event EventHandler PreDrawEvent;
        //static public event EventHandler OnDraw
        //{
        //    add
        //    {
        //        lock (PreDrawEvent)
        //        {
        //            if (PreDrawEvent == null)
        //            {
        //                PreDrawEvent += value;
        //            }
        //        }
        //    }
        //    remove
        //    {
        //        lock (PreDrawEvent)
        //        {
        //            PreDrawEvent -= value;
        //        }
        //    }
        //}

        public string ID { get; internal set; }
        public string RealName { get; internal set; }
        public string UserName { get; internal set; }

        public bool IsSa
        {
            get
            {
                return this.UserName.Equals("sa", StringComparison.OrdinalIgnoreCase);
            }
        }

        internal Admin()
        {

        }

       


        public void ChangePassword(string oldPassWord, string newPassword)
        {
            string adminid = this.ID;
            using (var repository = new BvnErpReponsitory())
            {
                var single = repository.ReadTable<Layer.Data.Sqls.BvnErp.Admins>().Single(item => item.ID == adminid);

                if (single.Password != oldPassWord.PasswordOld())
                {
                    if (this != null && this.PasswordError != null)
                    {
                        this.PasswordError(this, new ErrorEventArgs());
                    }
                    return;
                }

                repository.Update<Layer.Data.Sqls.BvnErp.Admins>(new { Password = newPassword.PasswordOld() }, item => item.ID == adminid);
            }

            if (this != null && this.PasswordSuccess != null)
            {
                this.PasswordSuccess(this, new SuccessEventArgs(this));
            }
        }
    }
}
