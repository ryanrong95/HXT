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
using NtErp.Crm.Services.Extends;
using NtErp.Crm.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Converters;
using Needs.Overall;

namespace NtErp.Crm.Services.Models
{
    [Needs.Underly.FactoryView(typeof(Views.PlanAlls))]
    public partial class Plan:  IUnique, IPersistence
    {
        #region 属性

        string id;
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

        public string Name { get; set; }
        public string ClientID { get; set; }      
        public Client client { get; set; }
        public string CompanyID { get; set; }      
        public string CatalogueID { get; set; }
        public ActionTarget Target { get; set; }    
        public ActionMethord Methord { get; set; }

        public AdminTop Admin { get; set; }
        public string AdminID
        {
            get;set;
            //get
            //{
            //    return this.Admin.ID;
            //}
            //set
            //{
            //    this.AdminID = value;
            //}
        }  
        public DateTime PlanDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ActionStatus Status { get; set; }   
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }
        public string SaleID { get; set; }
        public string SaleManagerID { get; set; }

        public Catelogue Catelogues { get; set; }
      
        public Company Companys { get; set; }

        #endregion

        public Plan()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = ActionStatus.Normal;
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

        virtual protected void OnEnter()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Actions>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    this.UpdateDate = DateTime.Now;
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }
        }

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
        virtual protected void OnAbandon()
        {
            using (Layer.Data.Sqls.BvCrmReponsitory reponsitory = new Layer.Data.Sqls.BvCrmReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.BvCrm.Actions>(new
                {
                    Status = ActionStatus.Delete
                }, item => item.ID == this.ID);
            }
        }
    }
}
