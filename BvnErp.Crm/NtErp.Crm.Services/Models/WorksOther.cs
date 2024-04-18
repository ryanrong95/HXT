using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using Needs.Overall;
using NtErp.Crm.Services.Extends;
using Needs.Utils.Descriptions;
using NtErp.Crm.Services.Enums;

namespace NtErp.Crm.Services.Models
{
    [Needs.Underly.FactoryView(typeof(Views.WorksOtherAlls))]
    public class WorksOther : IUnique, IPersistence
    {
        #region 属性
        /// <summary>
        /// ID
        /// </summary>
        public string ID
        {
            get; set;
        }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDate
        {
            get; set;
        }
        /// <summary>
        /// 主题
        /// </summary>
        public string Subject
        {
            get; set;
        }
        /// <summary>
        /// 内容
        /// </summary>
        public string Context
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
        /// <summary>
        /// 状态
        /// </summary>
        public Status Status
        {
            get; set;
        }
        /// <summary>
        /// 编写人
        /// </summary>
        public AdminTop Admin
        {
            get; set;
        }
        #endregion

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WorksOther()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        /// <summary>
        /// 逻辑删除数据
        /// </summary>
        protected void OnAbandon()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.BvCrm.WorksOther>(new
                {
                    Status = Status.Delete
                }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 删除
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
        /// 数据保存到数据库
        /// </summary>
        protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                //判定是否为新增数据
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = PKeySigner.Pick(PKeyType.WorksOther);
                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.WorksOther
                    {
                        ID = this.ID,
                        Context = this.Context,
                        StartDate = this.StartDate,
                        CreateDate = this.CreateDate,
                        AdminID = this.Admin.ID,
                        Status = (int)this.Status,
                        UpdateDate = this.UpdateDate,
                        Subject = this.Subject
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.BvCrm.WorksOther
                    {
                        ID = this.ID,
                        Context = this.Context,
                        StartDate = this.StartDate,
                        CreateDate = this.CreateDate,
                        AdminID = this.Admin.ID,
                        Status = (int)this.Status,
                        UpdateDate = DateTime.Now,
                        Subject = this.Subject
                    }, item => item.ID == this.ID);
                }
            }
        }

        /// <summary>
        /// 数据插入
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
    }
}
