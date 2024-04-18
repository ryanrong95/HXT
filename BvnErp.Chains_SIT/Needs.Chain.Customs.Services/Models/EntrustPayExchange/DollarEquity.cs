using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DollarEquity : IUnique
    {
        #region 属性
        public string ID { get; set; }
        public string ClientID { get; set; }
        public string OrderID { get; set; }
        public string FinanceReceiptID { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AvailableAmount { get; set; }
        public string Currency { get; set; }
        public Status Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Summary { get; set; }
        #endregion

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;


        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DollarEquity>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.DollarEquity
                    {
                        ID = this.ID,
                        ClientID = this.ClientID,
                        OrderID = this.OrderID,
                        FinanceReceiptID = this.FinanceReceiptID,
                        Amount = this.Amount,
                        AvailableAmount = this.AvailableAmount,
                        Currency = this.Currency,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary
                    });
                }

                this.OnEnter();
            }
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
