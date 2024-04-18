using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using System.Collections;
using System.Linq.Expressions;
using Needs.Erp.Generic;
using Needs.Underly;
using Layer.Data.Sqls;
using NtErp.Crm.Services.Enums;
using Needs.Utils.Descriptions;
using NtErp.Crm.Services.Extends;
using Needs.Overall;

namespace NtErp.Crm.Services.Models
{
    [Needs.Underly.FactoryView(typeof(Views.ApplyAlls))]
    public partial class Apply : IUnique, IPersist
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
        /// 申请类型
        /// </summary>
        public ApplyType Type
        {
            get; set;
        }

        /// <summary>
        /// 关联主键ID
        /// </summary>
        public string MainID
        {
            get; set;
        }

        /// <summary>
        /// 申请人ID
        /// </summary>
        public string AdminID
        {
            get
            {
                return this.Admin.ID;
            }
            internal set
            {
                this.Admin.ID = value;
            }
        }

        /// <summary>
        /// 申请人
        /// </summary>
        public AdminTop Admin
        {
            get; set;
        }
        /// <summary>
        /// 申请状态
        /// </summary>
        public ApplyStatus Status
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
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate
        {
            get; set;
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Summary
        {
            get; set;
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public Apply()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }

        public event SuccessHanlder EnterSuccess;


        /// <summary>
        /// 持久化触发方法
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
        /// 数据保存
        /// </summary>
        protected void OnEnter()
        {
            using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
            {
                //判定是否为新增
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Applies>().Count(item => item.MainID == this.MainID && item.Type == (int)this.Type
                        && item.Status == (int)ApplyStatus.Audting);
                    if (count == 0)
                    {
                        this.ID = PKeySigner.Pick(PKeyType.Apply);
                        reponsitory.Insert(this.ToLinq());
                    }
                }
                else
                {
                    this.UpdateDate = DateTime.Now;
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }
        }
    }
}
