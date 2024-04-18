using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DecTaxQuota : IUnique, IPersist
    {
        #region 属性
        public string ID { get; set; }
        /// <summary>
        ///报关单ID
        /// </summary>
        public string DeclarationID { get; set; }
        /// <summary>
        /// 关税
        /// </summary>
        public decimal Tariff { get; set; }
        /// <summary>
        /// 增值税
        /// </summary>
        public decimal AddedValueTax { get; set; }
        /// <summary>
        /// 消费税
        /// </summary>

        public decimal ExciseTax { get; set; }
        /// <summary>
        /// 缴税状态
        /// </summary>
        public  Enums.TaxStatus PayStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }
        #endregion

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;
        public DecTaxQuota()
        {
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
            this.ExciseTax = 0M;


        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxQuotas>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DecTaxQuotas {

                        ID = ChainsGuid.NewGuidUp(),
                        DeclarationID = this.DeclarationID,
                        ExciseTax = this.ExciseTax,
                        AddedValueTax = this.AddedValueTax,
                        Tariff = this.Tariff,
                        PayStatus= (int)this.PayStatus,
                        Status=(int)this.Status,
                        CreateDate=this.CreateDate,
                        UpdateDate=this.UpdateDate,
                        Summary=this.Summary
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.DecTaxQuotas {

                        ID =this.ID,
                        DeclarationID = this.DeclarationID,
                        ExciseTax = this.ExciseTax,
                        AddedValueTax = this.AddedValueTax,
                        Tariff = this.Tariff,
                        PayStatus = (int)this.PayStatus,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary

                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnter();
        }

        virtual protected void OnEnter()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }
    }
}
