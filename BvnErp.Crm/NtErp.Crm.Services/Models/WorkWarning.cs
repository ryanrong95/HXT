using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Erp.Generic;
using Needs.Linq;
using Needs.Utils.Converters;
using NtErp.Crm.Services.Enums;

namespace NtErp.Crm.Services.Models
{
    [Needs.Underly.FactoryView(typeof(Views.WorkWarningsAlls))]
    public class WorkWarning : IUnique, IPersist
    {
        #region 属性
        public string ID
        {
            get; set;
        }

        /// <summary>
        /// 类型
        /// </summary>
        public WarningType Type
        {
            get; set;
        }

        /// <summary>
        /// 关联ID
        /// </summary>
        public string MainID
        {
            get; set;
        }

        /// <summary>
        /// 当前人员
        /// </summary>
        public AdminTop Admin
        {
            get; set;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public WarningStatus Status
        {
            get; set;
        }

        /// <summary>
        /// 来源
        /// </summary>
        public string Resource
        {
            get; set;
        }

        /// <summary>
        /// 新建日期
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
        /// 备注
        /// </summary>
        public string Summary
        {
            get; set;
        }
        #endregion

        public WorkWarning()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = WarningStatus.unread;
        }

        public event SuccessHanlder EnterSuccess;

        #region 数据库持久化

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
                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.Warnings
                    {
                        ID = this.ID = Guid.NewGuid().ToString(),
                        Type = (int)this.Type,
                        MainID = this.MainID,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                        AdminID = this.Admin.ID,
                        Status = (int)this.Status,
                        Resource = this.Resource,
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.BvCrm.Warnings
                    {
                        ID = this.ID,
                        Type = (int)this.Type,
                        MainID = this.MainID,
                        UpdateDate = this.UpdateDate = DateTime.Now,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary,
                        AdminID = this.Admin.ID,
                        Status = (int)this.Status,
                        Resource = this.Resource,
                    }, item => item.ID == this.ID);
                }
            }
        }
        #endregion
    }
}
