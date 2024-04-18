using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Linq;
using Needs.Overall;
using NtErp.Crm.Services.Extends;
using System.Linq.Expressions;
using Needs.Utils.Converters;
using NtErp.Crm.Services.Enums;

namespace NtErp.Crm.Services.Models
{
    [Needs.Underly.FactoryView(typeof(Views.IndustryAlls))]
    public class Industry : IUnique, IPersistence
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate
        {
            get; set;
        }


        /// <summary>
        /// 父类ID
        /// </summary>
        public string FatherID
        {
            get; set;
        }

        string id;
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID
        {
            get
            {
                return this.id ?? string.Concat(this.Name+this.FatherID).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 行业名称
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// 行业状态
        /// </summary>
        public Status Status
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

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate
        {
            get; set;
        }

        /// <summary>
        /// 英文名称
        /// </summary>
        public  string EnglishName
        {
            get;set;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Industry()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = Status.Normal;
        }

        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;
        public event SuccessHanlder EnterSuccess;

        /// <summary>
        /// 逻辑删除数据
        /// </summary>
        protected void OnAbandon()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.BvCrm.Industries>(new
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
        /// 执行数据保存
        /// </summary>
        protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Industries>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.BvCrm.Industries
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Summary = this.Summary,
                        FatherID = this.FatherID,
                        EnglishName=this.EnglishName,
                        CreateDate = this.CreateDate,
                        Status = (int)this.Status,
                        UpdateDate = this.UpdateDate
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.BvCrm.Industries
                    {
                        ID = this.ID,
                        Name = this.Name,
                        Summary = this.Summary,
                        FatherID = this.FatherID,
                        EnglishName=this.EnglishName,
                        CreateDate = this.CreateDate,
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
    }
}
