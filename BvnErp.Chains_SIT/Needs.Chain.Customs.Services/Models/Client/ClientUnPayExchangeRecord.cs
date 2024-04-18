using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 超期付汇日志
    /// </summary>
    public class ClientUnPayExchangeRecord : IUnique, IPersist
    {
        public string ID { get; set; }

        public string ClientID { get; set; }

        public int Type { get; set; }

        public string AdminID { get; set; }

        public decimal? Amount { get; set; }

        public string Currency { get; set; }

        public decimal? UnPayExchangeAmount { get; set; }

        public decimal? PayExchangeAmount { get; set; }

        public decimal? DeclareAmount { get; set; }

        public decimal? PayExchangeAmountMonth { get; set; }

        public decimal? DeclareAmountMonth { get; set; }

        public DateTime CalculateDate { get; set; }

        public string Summary { get; set; }


        public ClientUnPayExchangeRecord()
        {
            this.CalculateDate = DateTime.Now;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientUnPayExchangeRecord>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    this.ID = Guid.NewGuid().ToString("N").ToUpper();
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClientUnPayExchangeRecord
                    {
                        ID = this.ID,
                        AdminID = this.AdminID,
                        ClientID = this.ClientID,
                        Type = this.Type,
                        Amount = this.Amount,
                        Currency = this.Currency,
                        UnPayExchangeAmount = this.UnPayExchangeAmount,
                        DeclareAmount = this.DeclareAmount,
                        PayExchangeAmount = this.PayExchangeAmount,
                        DeclareAmountMonth = this.DeclareAmountMonth,
                        PayExchangeAmountMonth = this.PayExchangeAmountMonth,
                        CalculateDate = this.CalculateDate,
                        Summary = this.Summary
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.ClientUnPayExchangeRecord
                    {
                        ID = this.ID,
                        AdminID = this.AdminID,
                        ClientID = this.ClientID,
                        Type = this.Type,
                        Amount = this.Amount,
                        Currency = this.Currency,
                        UnPayExchangeAmount = this.UnPayExchangeAmount,
                        DeclareAmount = this.DeclareAmount,
                        PayExchangeAmount = this.PayExchangeAmount,
                        DeclareAmountMonth = this.DeclareAmountMonth,
                        PayExchangeAmountMonth = this.PayExchangeAmountMonth,
                        CalculateDate = this.CalculateDate,
                        Summary = this.Summary
                    }, item => item.ID == this.ID);
                }
            }
        }
    }
}