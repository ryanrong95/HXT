using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.InvoiceService.Services.Models
{
    public class TaxManage : IUnique, IPersist
    {
        #region 属性

        public string ID { get; set; }

        public string InvoiceCode { get; set; }

        public string InvoiceNo { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public string SellsName { get; set; }

        public decimal Amount { get; set; }

        public decimal? VaildAmount { get; set; }

        public DateTime? ConfrimDate { get; set; }

        public DateTime? AuthenticationMonth { get; set; }

        public Enums.InvoiceVaildStatus IsVaild { get; set; }

        public string InvoiceDetailID { get; set; }

        public Enums.InvoiceType InvoiceType { get; set; }

        public Enums.BusinessType BusinessType { get; set; }

        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        #endregion

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.TaxManage>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.TaxManage>(new Layer.Data.Sqls.ScCustoms.TaxManage()
                    {
                        ID = this.ID,
                        InvoiceCode = this.InvoiceCode,
                        InvoiceNo = this.InvoiceNo,
                        InvoiceDate = this.InvoiceDate,
                        SellsName = this.SellsName,
                        Amount = this.Amount,
                        VaildAmount = this.VaildAmount,
                        ConfrimDate = this.ConfrimDate,
                        AuthenticationMonth = this.AuthenticationMonth,
                        IsVaild = (int)this.IsVaild,
                        InvoiceDetailID = this.InvoiceDetailID,
                        InvoiceType = (int)this.InvoiceType,
                        BusinessType = (int)this.BusinessType,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    });
                }
                else
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.TaxManage>(new
                    {
                        InvoiceCode = this.InvoiceCode,
                        InvoiceNo = this.InvoiceNo,
                        InvoiceDate = this.InvoiceDate,
                        SellsName = this.SellsName,
                        Amount = this.Amount,
                        VaildAmount = this.VaildAmount,
                        ConfrimDate = this.ConfrimDate,
                        AuthenticationMonth = this.AuthenticationMonth,
                        IsVaild = (int)this.IsVaild,
                        InvoiceDetailID = this.InvoiceDetailID,
                        InvoiceType = (int)this.InvoiceType,
                        BusinessType = (int)this.BusinessType,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                    }, item => item.ID == this.ID);
                }
            }
        }

        public static void SetIsVaild(string id, Enums.InvoiceVaildStatus apiStatus)
        {
            using (var reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.TaxManage>(new
                {
                    IsVaild = (int)apiStatus,
                }, item => item.ID == id);
            }
        }
    }
}
