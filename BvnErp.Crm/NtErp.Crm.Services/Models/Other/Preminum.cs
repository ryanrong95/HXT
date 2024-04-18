using Needs.Erp.Generic;
using Needs.Linq;
using Needs.Overall;
using Needs.Utils.Converters;
using NtErp.Crm.Services.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public class Preminum : IUnique, IPersistence
    {
        #region 属性
        public string ID
        {
            get;set;
        }

        public string CatalogueID
        {
            get; set;
        }

        public string DeclareID
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public decimal Price
        {
            get; set;
        }

        public Status Status
        {
            get; set;
        }

        public DateTime CreateDate
        {
            get; set;
        }

        public DateTime UpdateDate
        {
            get; set;
        }
        #endregion

        public Preminum()
        {
            this.Status = Status.Normal;
            CreateDate = UpdateDate = DateTime.Now;
        }

        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;
        public event SuccessHanlder AbandonSuccess;

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            if (string.IsNullOrWhiteSpace(this.ID))
            {
                if (this != null && this.AbandonError != null)
                {
                    this.AbandonError(this, new ErrorEventArgs(this.ID));
                }
            }

            this.OnAbandon();

            if (this != null && this.AbandonSuccess != null)
            {
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }


        protected void OnAbandon()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.BvCrm.Preminums>(new
                {
                    Status = Status.Delete
                }, item => item.ID == this.ID);
            }
        }

        /// <summary>
        /// 新增
        /// </summary>
        public void Enter()
        {
            this.OnEnter();

            if (this != null && this.EnterSuccess != null)
            {
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }


        protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                if (string.IsNullOrWhiteSpace(this.ID))
                {
                    this.ID = string.Concat(this.Name).MD5();
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    UpdateDate = DateTime.Now;
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }
        }
    }
}
