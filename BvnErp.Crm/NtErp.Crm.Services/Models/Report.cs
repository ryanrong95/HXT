using Needs.Linq;
using System;
using NtErp.Crm.Services.Extends;
using Needs.Overall;
using System.Linq;
using NtErp.Crm.Services.Enums;
using Needs.Utils.Descriptions;
using Layer.Data.Sqls;

namespace NtErp.Crm.Services.Models
{
    [Needs.Underly.FactoryView(typeof(Views.ReportsAlls))]
    public partial class Report : IUnique, IPersistence
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
        /// 行动对象
        /// </summary>
        public Plan Action
        {
            get; set;
        }

        /// <summary>
        /// 项目对象
        /// </summary>
        public Project Project
        {
            get; set;
        }

        /// <summary>
        /// 客户对象
        /// </summary>
        public Client Client
        {
            get; set;
        }

        /// <summary>
        /// 当前人员对象
        /// </summary>
        public AdminTop Admin
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
        /// 内容
        /// </summary>
        public string Context
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
        /// 跟进方式
        /// </summary>
        public ActionMethord? Type
        {
            get; set;
        }
        /// <summary>
        /// 跟进日期
        /// </summary>
        public DateTime? Date
        {
            get; set;
        }

        /// <summary>
        /// 下次更新日期
        /// </summary>
        public DateTime? NextDate
        {
            get; set;
        }

        /// <summary>
        /// 下次跟进方式
        /// </summary>
        public ActionMethord NextType
        {
            get; set;
        }

        /// <summary>
        /// 跟进内容
        /// </summary>
        public string Content
        {
            get; set;
        }

        /// <summary>
        /// 行动计划
        /// </summary>
        public string Plan
        {
            get; set;
        }

        /// <summary>
        /// 原厂陪同人员
        /// </summary>
        public string OriginalStaffs
        {
            get; set;
        }

        /// <summary>
        /// 指定阅读人
        /// </summary>
        public string[] Readers
        {
            get
            {
                using (BvCrmReponsitory reponsitory = new BvCrmReponsitory())
                {
                    return reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.MapsReport>().Where(item => item.ReportID == this.ID).
                         Select(item => item.ReadAdminID).ToArray();
                }
            }
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public Report()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
        }

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder EnterSuccess;
        public event SuccessHanlder AbandonSuccess;

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

        /// <summary>
        /// 数据保存到数据库
        /// </summary>
        virtual protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                if (string.IsNullOrEmpty(this.ID))
                {
                    this.ID = PKeySigner.Pick(PKeyType.Report);
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    this.UpdateDate = DateTime.Now;
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }
        }


        /// <summary>
        /// 逻辑删除
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
        /// 执行逻辑删除数据
        /// </summary>
        virtual protected void OnAbandon()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.BvCrm.Reports>(new
                {
                    Status = Status.Delete
                }, item => item.ID == this.ID);
            }
        }
    }

}
