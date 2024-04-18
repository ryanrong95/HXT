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
    [Needs.Underly.FactoryView(typeof(Views.DirectoryAlls))]
    public class Directory : IUnique, IPersistence
    {
        #region 属性
        /// <summary>
        /// 主键
        /// </summary>
        public string ID
        {
            get;set;
        }

        /// <summary>
        /// 父类节点
        /// </summary>
        public string FatherID
        {
            get;set;
        }

        /// <summary>
        /// 目录名称
        /// </summary>
        public string Name
        {
            get;set;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public Status Status
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
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public Directory()
        {
            this.Status = Status.Normal;
            this.CreateDate = this.UpdateDate = DateTime.Now;
        }


        public event SuccessHanlder AbandonSuccess;
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
                reponsitory.Update<Layer.Data.Sqls.BvCrm.Directories>(new
                {
                    Status = Status.Delete,
                }, item => item.ID == this.ID);
                //目录下的文档
                reponsitory.Update<Layer.Data.Sqls.BvCrm.Documents>(new
                {
                    Status = Status.Delete,
                }, item => item.DirectoryID == this.ID);
            }
        }

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
                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.Directories
                    {
                        ID = this.ID = Guid.NewGuid().ToString(),
                        FatherID = this.FatherID,
                        Name = this.Name,
                        Status  = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.BvCrm.Directories
                    {
                        ID = this.ID,
                        FatherID = this.FatherID,
                        Name = this.Name,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                    }, item => item.ID == this.ID);
                }
            }
        }
        #endregion
    }
}
