using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using Needs.Overall;
using NtErp.Crm.Services.Extends;
using Needs.Utils.Converters;
using NtErp.Crm.Services.Enums;

namespace NtErp.Crm.Services.Models
{
    [Needs.Underly.FactoryView(typeof(Views.CompanyAlls))]
    public partial class Company : IUnique, IPersistence, IFulError, IFulSuccess
    {
        #region 属性
        string id;
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Name).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 公司类型
        /// </summary>
        public CompanyType Type
        {
            get; set;
        }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string Code
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
        /// 创建时间
        /// </summary>
        virtual public DateTime CreateDate
        {
            get; set;
        }

        /// <summary>
        /// 更新时间
        /// </summary>
        virtual public DateTime UpdateDate
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
        public Company()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = Status.Normal;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        

        /// <summary>
        /// 删除触发方法
        /// </summary>
        public void Abandon()
        {
            //判定ID不能为空
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                if (this != null && this.AbandonError != null)
                {
                    //失败触发事件
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
        /// 执行数据逻辑删除
        /// </summary>
        virtual protected void OnAbandon()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.BvCrm.Companies>(new
                {
                    Status = Status.Delete
                }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 执行数据保存
        /// </summary>
        virtual protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Companies>().Count(item => item.Name == this.Name && item.Type == (int)this.Type
                    && item.ID != this.ID);
                if (count > 0)
                {
                    if (this != null && this.EnterError != null)
                    {
                        this.EnterError(this, new ErrorEventArgs("名称不能重复！"));
                    }
                }

                //判断数据是否存在
                count = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Companies>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.Companies
                    {
                        ID = this.ID,
                        Type = (int)this.Type,
                        Name = this.Name,
                        Code = this.Code,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary,
                        Status = (int)this.Status,
                        UpdateDate = this.UpdateDate,
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.BvCrm.Companies
                    {
                        ID = this.ID,
                        Type = (int)this.Type,
                        Name = this.Name,
                        Code = this.Code,
                        CreateDate = this.CreateDate,
                        Summary = this.Summary,
                        Status = (int)this.Status,
                        UpdateDate = this.UpdateDate,
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
