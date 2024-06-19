using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using Needs.Wl.Models.Hanlders;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 会员用户信息
    /// </summary>
    [Serializable]
    public class User : ModelBase<Layer.Data.Sqls.ScCustoms.Users, ScCustomsReponsitory>, IUnique, IPersist, IPersistence
    {
        /// <summary>
        /// 
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 用户名/账号/登录账号
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        public string RealName { get; set; }

        /// <summary>
        /// 是否主账号
        /// </summary>
        public bool IsMain { get; set; }

        /// <summary>
        /// 添加人ID
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 微信 OpenID
        /// </summary>
        public string OpenID { get; set; }

        public event StatusChangedEventHanlder StatusChanged;

        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        public User()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        public override void Enter()
        {
            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>().Count(item => item.ID == this.ID);

            //主账号
            if (this.IsMain)
            {
                this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.Users>(new { IsMain = false }, item => item.ClientID == this.ClientID);
            }

            if (count == 0)
            {
                this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.User);
                this.Reponsitory.Insert(this.ToLinq());
            }
            else
            {
                this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.Users>(new
                {
                    ID = this.ID,
                    AdminID = this.AdminID,
                    RealName = this.RealName,
                    ClientID = this.ClientID,
                    Email = this.Email,
                    Mobile = this.Mobile,
                    Name = this.Name,
                    Password = this.Password,
                    IsMain = this.IsMain,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = DateTime.Now,
                    Summary = this.Summary

                }, item => item.ID == this.ID);
            }

            this.OnEnter();
        }

        public override void Abandon()
        {
            this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.Users>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        public void ResetPassword()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var defaultPd = Needs.Utils.Converters.StringExtend.StrToMD5("HXT123");
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Users>(new { Password = defaultPd }, item => item.ID == this.ID);
            }
        }
    }
}