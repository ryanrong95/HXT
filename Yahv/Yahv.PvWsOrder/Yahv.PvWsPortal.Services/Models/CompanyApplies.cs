using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Usually;
using Layers.Data.Sqls;

namespace Yahv.PvWsPortal.Services.Models
{
    public class CompanyApplies : IUnique
    {
        #region 属性
        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 企业名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 跟单员
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ServiceApplyHandleStatus Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Summary { get; set; }
        #endregion

        public CompanyApplies()
        {
            this.Status = ServiceApplyHandleStatus.Pending;
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        public virtual event SuccessHanlder EnterSuccess;

        #region 持久化
        /// <summary>
        /// 数据插入
        /// </summary>
        public virtual void Enter()
        {
            using (ScCustomReponsitory reponsitory = new ScCustomReponsitory())
            {
                this.ID = Guid.NewGuid().ToString();
                reponsitory.Insert(new Layers.Data.Sqls.ScCustoms.ServiceApplies
                {
                    ID = this.ID,
                    Email = this.Email ??string.Empty,
                    CompanyName = this.CompanyName ?? string.Empty,
                    Address = this.Address ?? string.Empty,
                    Contact = this.Contact ?? string.Empty,
                    Mobile = this.Mobile ?? string.Empty,
                    Tel = this.Tel ?? string.Empty,
                    AdminID = this.AdminID,
                    Status = (int)this.Status,
                    CreateDate = this.CreateDate,
                    UpdateDate = this.UpdateDate,
                    Summary = this.Summary,
                });
            }
            this.OnEnterSuccess();
        }

        /// <summary>
        /// 成功触发事件
        /// </summary>
        public virtual void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }
        #endregion
    }
}
