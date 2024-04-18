using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NtErp.Crm.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Linq;
using NtErp.Crm.Services.Extends;

namespace NtErp.Crm.Services.Models
{
    public class ApplyStep : IPersist
    {
        #region 属性
        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplyID
        {
            get; set;
        }
        /// <summary>
        /// 审批步骤
        /// </summary>
        public int Step
        {
            get; set;
        }
        /// <summary>
        /// 审批人ID
        /// </summary>
        public string AdminID
        {
            get; set;
        }
        public string AdminName
        {
            get;set;
        }

        /// <summary>
        /// 审批状态
        /// </summary>
        public Enums.ApplyStep Status
        {
            get; set;
        }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string Comment
        {
            get;set;
        }

        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime? AprDate
        {
            get;set;
        }
        #endregion

        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder EnterError;


        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            //判定申请ID和操作不能为空
            if (string.IsNullOrWhiteSpace(this.ApplyID) || this.Step == 0)
            {
                if(this!=null && this.EnterError != null)
                {
                    //失败触发事件
                    this.EnterError(this, new ErrorEventArgs("申请ID和操作不能为空！"));
                    return;
                }
            }

            this.OnEnter();

            if (this != null && EnterSuccess != null)
            {
                //成功触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ApplyID + this.Step));
            }
        }

        /// <summary>
        /// 数据保存
        /// </summary>
        protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                reponsitory.Insert(this.ToLinq());
            }
        }
    }
}
