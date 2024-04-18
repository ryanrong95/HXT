using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DecTaxGuarantee : IUnique, IPersist
    {
        public string ID { get; set; }

        public string GuaranteeNo { get; set; }

        public string PutOnCustoms { get; set; }

        public decimal GuaranteeAmount { get; set; }

        public decimal RemainAmount { get; set; }

        public string BankName { get; set; }

        public DateTime ApproveDate { get; set; }

        public DateTime ValidDate { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public DecTaxGuarantee()
        { 
        
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxGuarantees>().Where(item => item.ID == this.ID);

                if (count.Count() == 0)
                {
                    this.ID = ChainsGuid.NewGuidUp();
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DecTaxGuarantees
                    {
                        ID = this.ID,
                        GuaranteeNo = this.GuaranteeNo,
                        PutOnCustoms = this.PutOnCustoms,
                        GuaranteeAmount = this.GuaranteeAmount,
                        RemainAmount = this.RemainAmount,
                        BankName = this.BankName,
                        ApproveDate = this.ApproveDate,
                        ValidDate = this.ValidDate,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.DecTaxGuarantees
                    {
                        ID = this.ID,
                        GuaranteeNo = this.GuaranteeNo,
                        PutOnCustoms = this.PutOnCustoms,
                        GuaranteeAmount = this.GuaranteeAmount,
                        RemainAmount = this.RemainAmount,
                        BankName = this.BankName,
                        ApproveDate = this.ApproveDate,
                        ValidDate = this.ValidDate,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = DateTime.Now,
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
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }
}
