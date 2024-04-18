using Layer.Data.Sqls;
using Needs.Linq;

using Needs.Settings;
using Needs.Utils.Converters;
using Needs.Utils.Http;
using System;
using System.Linq;

namespace Needs.Wl.Admin.Plat.Models
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
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public string ID { get; internal set; }

        public string RealName { get; internal set; }

        /// <summary>
        /// 账号/用户名/登录名
        /// </summary>
        public string UserName { get; internal set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// OriginID
        /// </summary>
        public string OriginID { get; set; }

        /// <summary>
        /// ErmAdminID
        /// </summary>
        public string ErmAdminID { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public string DepartmentID { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        public string ByName { get; set; }

        /// <summary>
        /// 是否超级管理员
        /// </summary>
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

                if (single.Password != oldPassWord.Password())
                {
                    if (this != null && this.PasswordError != null)
                    {
                        this.PasswordError(this, new ErrorEventArgs());
                    }
                    return;
                }

                repository.Update<Layer.Data.Sqls.BvnErp.Admins>(new { Password = newPassword.Password() }, item => item.ID == adminid);
            }

            if (this != null && this.PasswordSuccess != null)
            {
                this.PasswordSuccess(this, new SuccessEventArgs(this));
            }
        }

        public void Enter()
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminWl>().Count(item => item.AdminID == this.ID);
                    if (count > 0)
                    {
                        reponsitory.Update(new Layer.Data.Sqls.ScCustoms.AdminWl
                        {
                            Tel = this.Tel,
                            Mobile = this.Mobile,
                            Email = this.Email,
                            Summary = this.Summary
                        }, item => item.AdminID == this.ID);
                    }
                }
                //this.EnterError(this, new ErrorEventArgs("222"));
                this.OnEnterSuccess();
            }
            catch (Exception ex)
            {
                this.EnterError(this, new ErrorEventArgs(ex.Message));
            }

        }
        virtual public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}
