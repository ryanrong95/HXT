using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Erp.Generic;
using Needs.Linq;
using Needs.Overall;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Extends;

namespace NtErp.Crm.Services.Models
{
    [Needs.Underly.FactoryView(typeof(Views.DeclareProductAlls))]
    public partial class DeclareProduct : Needs.Linq.IUnique, Needs.Linq.IPersistence
    {
        #region 属性
        public string CatelogueID
        {
            get; set;
        }

        public string StandardID
        {
            get
            {
                return this.StandardProduct.ID;
            }
            internal set
            {
                this.StandardProduct.ID = value;
            }
        }

        public StandardProduct StandardProduct
        {
            get; set;
        }

        public string SupplierID
        {
            get
            {
                return this.Supplier?.ID;
            }
            internal set
            {
                this.Supplier.ID = value;
            }
        }

        public Company Supplier
        {
            get; set;
        }

        public CurrencyType Currency
        {
            get; set;
        }

        public int Amount
        {
            get; set;
        }

        public decimal UnitPrice
        {
            get; set;
        }

        public string Delivery
        {
            get; set;
        }

        public int? Count
        {
            get; set;
        }


        public string ID
        {
            get;set;
        }

        public ProductStatus Status
        {
            get; set;
        }

        public string CompeteManu
        {
            get;set;
        }

        public string CompeteModel
        {
            get;set;
        }

        public decimal? CompetePrice
        {
            get;set;
        }

        public string OriginNumber
        {
            get;set;
        }

        public decimal? Expect
        {
            get; set;
        }

        public decimal? TotalPrice
        {
            get; set;
        }

        public decimal? ExpectTotal
        {
            get; set;
        }

        public DateTime? ExpectDate
        {
            get; set;
        }

        public string Name
        {
            get;set;
        }

        public string VendorID
        {
            get;set;
        }


        #endregion

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
                reponsitory.Update<Layer.Data.Sqls.BvCrm.DeclareProducts>(new
                {
                    Status = ProductStatus.DL
                }, item => item.ID == this.ID);
            }
        }


        /// <summary>
        /// 持久化
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
                    this.ID = PKeySigner.Pick(PKeyType.Product);
                    //this.StandardProduct.ID = string.Concat(StandardProduct.Name).MD5();
                    //this.StandardProduct.Enter();
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    //this.StandardProduct.Enter();
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }
        }
    }
}
