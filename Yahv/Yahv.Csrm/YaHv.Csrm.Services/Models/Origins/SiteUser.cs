using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Converters.Contents;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Views.Rolls;

namespace YaHv.Csrm.Services.Models.Origins
{
    public class SiteUser : Yahv.Linq.IUnique
    {
        #region 属性
        public string ID { set; get; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { set; get; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { set; get; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { set; get; }
        /// <summary>
        /// 来源
        /// </summary>
        //public string From { set; get; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { set; get; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { set; get; }
        /// <summary>
        /// QQ
        /// </summary>
        public string QQ { set; get; }
        /// <summary>
        /// 微信
        /// </summary>
        public string Wx { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        /// <summary>
        /// 是否仓储审批
        /// </summary>
        public bool IsStorageService { set; get; }
        /// <summary>
        /// 是否报关审批
        /// </summary>
        public bool IsDeclaretion { set; get; }
        #endregion


    }
    public class SiteUserXdt : SiteUser
    {
        public SiteUserXdt()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = ApprovalStatus.Normal;
        }
        /// <summary>
        /// 企业ID
        /// </summary>
        public string EnterpriseID { set; get; }
        /// <summary>
        /// 是否主账号
        /// </summary>
        public bool IsMain { set; get; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { set; get; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime UpdateDate { set; get; }
        /// <summary>
        /// 账号状态
        /// </summary>
        public ApprovalStatus Status { set; get; }
        public Enterprise Enterprise { set; get; }

        #region 事件
        /// <summary>
        /// AbandonSuccess
        /// </summary>
        public event SuccessHanlder AbandonSuccess;
        /// <summary>
        /// EnterSuccess
        /// </summary>
        public event SuccessHanlder EnterSuccess;
        /// <summary>
        /// 用户名已存在
        /// </summary>
        public event ErrorHanlder UserNameRepeat;
        #endregion

        #region 持久化
        public void Enter()
        {
            using (var reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
            {
                //主账号
                if (this.IsMain)
                {
                    reponsitory.Update<Layers.Data.Sqls.PvbCrm.SiteUsersXdt>(new { IsMain = false }, item => item.EnterpriseID == this.EnterpriseID);
                }
                var siterusers = new SiteUsersRoll();//所有的用户

                bool isNewUser = false;//是否新用户，是否添加
                bool flag = true;

                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    if (siterusers.Any(item => item.UserName == this.UserName))
                    {
                        flag = false;
                        //用户名已存在，同一客户的会员账号用户名不能重复，不同客户的可以重复(作废时间：20200611)
                        //用户名全局唯一：20200611

                        if (this != null && this.UserNameRepeat != null)
                        {
                            this.UserNameRepeat(this, new ErrorEventArgs());
                        }
                    }
                    else
                    {
                        isNewUser = true;
                    }
                }
                else
                {
                    //用户名是否修改，
                    if (siterusers.Any(item => item.ID == this.ID && item.UserName != this.UserName))
                    {
                        //改后的用户名是否存在
                        if (siterusers.Any(item => item.UserName == this.UserName))
                        {
                            flag = false;
                            if (this != null && this.UserNameRepeat != null)
                            {
                                this.UserNameRepeat(this, new ErrorEventArgs());
                            }
                        }
                    }
                }
                if (flag)
                {
                    if (isNewUser)
                    {
                        this.ID = PKeySigner.Pick(PKeyType.User);
                        reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.SiteUsers
                        {
                            ID = this.ID,
                            UserName = this.UserName,
                            RealName = this.RealName,
                            Password = this.Password,
                            Mobile = this.Mobile,
                            Email = this.Email,
                            QQ = this.QQ,
                            Wx = this.Wx,
                            Summary = this.Summary,
                            CreateDate = this.CreateDate,
                           
                        });
                        reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.SiteUsersXdt
                        {
                            ID = this.ID,
                            EnterpriseID = this.EnterpriseID,
                            IsMain = this.IsMain,
                            CreateDate = this.CreateDate,
                            UpdateDate = this.UpdateDate,
                            Status = (int)this.Status,
                            IsDeclaretion = this.IsDeclaretion,
                            IsStorageService = this.IsStorageService
                        });
                        if (this != null && this.EnterSuccess != null)
                        {
                            this.EnterSuccess(this, new SuccessEventArgs(this));
                        }
                    }
                    else
                    {
                        reponsitory.Update<Layers.Data.Sqls.PvbCrm.SiteUsers>(new
                        {
                            UserName = this.UserName,
                            RealName = this.RealName,
                            Password = this.Password,
                            Mobile = this.Mobile,
                            Email = this.Email,
                            QQ = this.QQ,
                            Wx = this.Wx,
                            Summary = this.Summary
                        }, item => item.ID == this.ID);
                        reponsitory.Update<Layers.Data.Sqls.PvbCrm.SiteUsersXdt>(new
                        {
                            IsMain = this.IsMain,
                            UpdateDate = this.UpdateDate,
                            IsDeclaretion = this.IsDeclaretion,
                            IsStorageService=this.IsStorageService
                        }, item => item.ID == ID);
                        if (this != null && this.EnterSuccess != null)
                        {
                            this.EnterSuccess(this, new SuccessEventArgs(this));
                        }

                    }
                }


            }
        }
        public void Abandon()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.SiteUsersXdt>().Any(item => item.ID == this.ID))
                {
                    repository.Update<Layers.Data.Sqls.PvbCrm.SiteUsersXdt>(new { Status = ApprovalStatus.Deleted }, item => item.ID == this.ID);
                }
                if (this != null && this.AbandonSuccess != null)
                {
                    this.AbandonSuccess(this, new SuccessEventArgs(this));
                }
            }
        }

        public void ResetPwd()
        {
            using (var repository = LinqFactory<PvbCrmReponsitory>.Create())
            {
                if (repository.ReadTable<Layers.Data.Sqls.PvbCrm.SiteUsers>().Any(item => item.ID == this.ID))
                {
                    var df = "CXHY123".StrToMD5();
                    repository.Update<Layers.Data.Sqls.PvbCrm.SiteUsers>(new { Password = df }, item => item.ID == this.ID);
                }
            }
        }
        #endregion
    }
}
