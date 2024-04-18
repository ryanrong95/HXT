using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using Needs.Utils.Converters;
using NtErp.Crm.Services.Enums;

namespace NtErp.Crm.Services.Models
{
    public class File : IUnique, IPersistence
    {
        #region 属性
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID
        {
            get; set;
        }

        /// <summary>
        /// 客户
        /// </summary>
        public Client Client
        {
            get; set;
        }

        public string ClientID { get; set; }

        /// <summary>
        /// 销售机会
        /// </summary>
        public Project Project
        {
            get; set;
        }

        public string ProjectID { get; set; }

        /// <summary>
        /// 报告
        /// </summary>
        public Report Report
        {
            get; set;
        }

        public string ReportID { get; set; }

        /// <summary>
        /// 工作计划
        /// </summary>
        public WorksOther WorksOther
        {
            get; set;
        }

        public string NoticeID { get; set; }

        /// <summary>
        /// 公告
        /// </summary>
        public Notice Notice
        {
            get; set;
        }

        public string WorksOtherID { get; set; }

        /// <summary>
        /// 工作周报
        /// </summary>
        public WorksWeekly WorksWeekly
        {
            get; set;
        }

        public string WorksWeeklyID { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public Contact Contact
        {
            get; set;
        }

        public string ContactID { get; set; }

        /// <summary>
        /// 附件类型
        /// </summary>
        public int? Type
        {
            get; set;
        }

        /// <summary>
        /// 附件名称
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate
        {
            get; set;
        }

        /// <summary>
        /// 附件地址
        /// </summary>
        public string Url
        {
            get; set;
        }

        /// <summary>
        /// 上传人
        /// </summary>
        public AdminTop Admin
        {
            get; set;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public Status Status
        {
            get; set;
        }
        #endregion

        public event SuccessHanlder AbandonSuccess;
        //public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;


        #region 持久化
        /// <summary>
        /// 数据删除触发事件
        /// </summary>
        public void Abandon()
        {
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                if (this != null && this.AbandonError != null)
                {
                    this.AbandonError(this, new ErrorEventArgs("主键ID不能为空！"));
                }
            }

            this.OnAbandon();

            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        virtual protected void OnAbandon()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.BvCrm.Files>(new
                {
                    Status = Status.Delete
                }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 数据保存触发事件
        /// </summary>
        public void Enter()
        {
            this.OnEnter();

            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        virtual protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                //判断是否为新增
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.Files
                    {
                        ID = this.ID = Guid.NewGuid().ToString(),
                        ClientID = this.Client?.ID,
                        ProjectID = this.Project?.ID,
                        ReportID = this.Report?.ID,
                        WorksOtherID = this.WorksOther?.ID,
                        WorksWeeklyID = this.WorksWeekly?.ID,
                        NoticeID = this.Notice?.ID,
                        ContactID = this.Contact?.ID,
                        Type = this.Type,
                        Name = this.Name,
                        CreateDate = DateTime.Now,
                        Url = this.Url,
                        AdminID = this.Admin.ID,
                        Status = (int)Status.Normal
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.BvCrm.Files>(new
                    {
                        ID = this.ID,
                        ClientID = this.Client?.ID,
                        ProjectID = this.Project?.ID,
                        ReportID = this.Report?.ID,
                        WorksOtherID = this.WorksOther?.ID,
                        WorksWeeklyID = this.WorksWeekly?.ID,
                        NoticeID = this.Notice?.ID,
                        ContactID = this.Contact?.ID,
                        Type = this.Type,
                        Name = this.Name,
                        CreateDate = DateTime.Now,
                        Url = this.Url,
                        AdminID = this.Admin.ID,
                        Status = (int)this.Status
                    }, item => item.ID == this.ID);
                }
            }
        }
        #endregion
    }
}
