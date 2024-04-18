using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using Needs.Utils.Converters;

namespace NtErp.Crm.Services.Models
{
    public class Reply : IUnique, IPersist
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
        /// 关联工作计划
        /// </summary>
        public WorksOther WorksOther
        {
            get; set;
        }

        public string WorksOtherID { get; set; }

        /// <summary>
        /// 关联工作周报
        /// </summary>
        public WorksWeekly WorksWeekly
        {
            get; set;
        }

        public string WorksWeeklyID { get; set; }

        /// <summary>
        /// 关联报告
        /// </summary>
        public Report Report
        {
            get; set;
        }

        public string ReportID { get; set; }

        /// <summary>
        /// 点评内容
        /// </summary>
        public string Context
        {
            get; set;
        }

        /// <summary>
        /// 点评人
        /// </summary>
        public AdminTop Admin
        {
            get; set;
        }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate
        {
            get; set;
        }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime UpdateDate
        {
            get; set;
        }
        #endregion 

        public event SuccessHanlder EnterSuccess;


        #region 持久化
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
        /// 保存
        /// </summary>
        virtual protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.Replies
                    {
                        ID = this.ID = Guid.NewGuid().ToString(),
                        WorksOtherID = WorksOther?.ID,
                        WorksWeeklyID = WorksWeekly?.ID,
                        ReportID = Report?.ID,
                        Context = Context,
                        AdminID = Admin.ID,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.BvCrm.Replies
                    {
                        ID = this.ID,
                        WorksOtherID = WorksOther?.ID,
                        WorksWeeklyID = WorksWeekly?.ID,
                        ReportID = Report?.ID,
                        Context = Context,
                        AdminID = Admin.ID,
                        CreateDate = this.CreateDate,
                        UpdateDate = DateTime.Now
                    }, item => item.ID == this.ID);
                }
            }
        }
        #endregion
    }
}
