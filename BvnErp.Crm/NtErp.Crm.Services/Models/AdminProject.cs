using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NtErp.Crm.Services.Enums;
using Needs.Linq;
using NtErp.Crm.Services.Extends;

namespace NtErp.Crm.Services.Models
{
    public partial class AdminProject : IPersist
    {
        #region 属性
        /// <summary>
        /// 用户ID
        /// </summary>
        public string AdminID
        {
            get;set;
        }

        /// <summary>
        /// 公司
        /// </summary>
        public Company Company
        {
            get;set;
        }

        /// <summary>
        /// 职位
        /// </summary>
        public JobType JobType
        {
            get;set;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate
        {
            get;set;
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate
        {
            get;set;
        }

        /// <summary>
        /// 微信ID
        /// </summary>
        public string WXID
        {
            get; set;
        }

        /// <summary>
        /// 是否授权微信
        /// </summary>
        public bool IsAgree
        {
            get; set;
        }

        /// <summary>
        /// 登录Token
        /// </summary>
        public string Token
        {
            get; set;
        }

        /// <summary>
        /// 考核类型
        /// </summary>
        public ScoreType? ScoreType
        {
            get;set;
        }

        /// <summary>
        /// 绩效基数
        /// </summary>
        public decimal? SalaryBase
        {
            get;set;
        }

        /// <summary>
        /// 大赢家ID
        /// </summary>
        public string DyjID
        {
            get; set;
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Summary
        {
            get;set;
        }
        #endregion

        public AdminProject()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            //校验ID是否为空
            if (string.IsNullOrWhiteSpace(this.AdminID))
            {
                if (this != null && this.EnterError != null)
                {
                    //失败触发事件
                    this.EnterError(this, new ErrorEventArgs("管理员ID不能为空！"));
                }
            }

            this.OnEnter();

            if (this != null && this.EnterSuccess != null)
            {
                //成功触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.AdminID));
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                var dyjcount = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.AdminsProject>().Count(item => item.DyjID == this.DyjID && item.AdminID != this.AdminID);

                if(dyjcount > 0)
                {
                    this.EnterError(this, new ErrorEventArgs("大赢家ID不能重复！"));
                }

                //判定数据是否存在
                var count = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.AdminsProject>().Count(item => item.AdminID == this.AdminID);

                if (count > 0)
                {
                    reponsitory.Update<Layer.Data.Sqls.BvCrm.AdminsProject>(new
                    {
                        this.AdminID,
                        JobType = (int)this.JobType,
                        this.CreateDate,
                        this.UpdateDate,
                        CompanyID = this.Company.ID,
                        this.IsAgree,
                        this.Token,
                        this.WXID,
                        ScoreType = (int?)this.ScoreType,
                        this.SalaryBase,
                        this.DyjID,
                        this.Summary,
                    }, item => item.AdminID == this.AdminID);
                }
                else
                {
                    reponsitory.Insert(this.ToLinq());
                }
            }
        }

        /// <summary>
        /// 微信授权更新
        /// </summary>
        public void IsAgreeUpdate()
        {
            //校验ID是否为空
            if (string.IsNullOrWhiteSpace(this.AdminID))
            {
                if (this != null && this.EnterError != null)
                {
                    //失败触发事件
                    this.EnterError(this, new ErrorEventArgs("管理员ID不能为空！"));
                    return;
                }
            }

            this.OnIsAgreeUpdate();

            if (this != null && this.EnterSuccess != null)
            {
                //成功触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.AdminID));
            }
        }

        /// <summary>
        /// 微信授权更新
        /// </summary>
        protected void OnIsAgreeUpdate()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                //判定数据是否存在
                reponsitory.Update<Layer.Data.Sqls.BvCrm.AdminsProject>(new {
                    IsAgree = this.IsAgree,
                    UpdateDate = DateTime.Now,
                 }, item => item.AdminID == this.AdminID);
            }
        }
    }
}
