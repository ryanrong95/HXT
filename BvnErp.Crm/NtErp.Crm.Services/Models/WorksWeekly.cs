using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using Needs.Overall;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Extends;

namespace NtErp.Crm.Services.Models
{
    [Needs.Underly.FactoryView(typeof(Views.WorksWeeklyAlls))]
    public class WorksWeekly : IUnique, IPersist
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
        /// 周数
        /// </summary>
        public int WeekOfYear
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
        /// 当前人员
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
        /// <summary>
        /// 状态
        /// </summary>
        public Status Status
        {
            get; set;
        }
        #endregion

        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 构造函数
        /// </summary>
        public WorksWeekly()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        #region 持久化
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
                    this.ID = PKeySigner.Pick(PKeyType.Weekly);
                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.WorksWeekly
                    {
                        ID = this.ID,
                        Context = this.Context,                        
                        WeekOfYear=this.WeekOfYear,
                        CreateDate = this.CreateDate,
                        AdminID=this.Admin.ID,
                        Status = (int)this.Status,
                        UpdateDate = this.UpdateDate
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.BvCrm.WorksWeekly
                    {
                        ID = this.ID,
                        Context = this.Context,                        
                        WeekOfYear = this.WeekOfYear,
                        CreateDate = this.CreateDate,
                        AdminID = this.Admin.ID,
                        Status = (int)this.Status,
                        UpdateDate = DateTime.Now
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
        #endregion
    }
}
