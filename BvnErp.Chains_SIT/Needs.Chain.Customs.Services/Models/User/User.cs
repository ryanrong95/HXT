using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 会员用户信息
    /// </summary>
    [Serializable]
    public class User : IUnique, IPersist, IPersistence
    {
        public string ID { get; set; }

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

        public string ClientID { get; set; }

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
        /// openid
        /// </summary>
        public string OpenID { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 账号状态
        /// </summary>
        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public event StatusChangedEventHanlder StatusChanged;

        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        public User()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>().Count(item => item.ID == this.ID);

                //主账号
                if (this.IsMain)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Users>(new { IsMain = false }, item => item.ClientID == this.ClientID);
                }

                if (count == 0)
                {
                    this.ID = Needs.Overall.PKeySigner.Pick(PKeyType.User);
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Users>(new
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
            }

            this.OnEnter();
        }

        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Users>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }
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
