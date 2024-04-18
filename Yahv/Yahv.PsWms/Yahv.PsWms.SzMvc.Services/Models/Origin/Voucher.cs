using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PsWms.SzMvc.Services.Common;
using Yahv.PsWms.SzMvc.Services.Enums;

namespace Yahv.PsWms.SzMvc.Services.Models.Origin
{
    public class Voucher : Linq.IUnique
    {
        public string ID { get; set; }
        public string PayerID { get; set; }
        public string PayeeID { get; set; }
        public VoucherType Type { get; set; }
        public VoucherMode Mode { get; set; }
        public DateTime CreateDate { get; set; }
        public string Summary { get; set; }
        public DateTime CutDate { get; set; }
        public int CutDateIndex { get; set; }

        public bool IsInvoiced { get; set; }

        public Voucher()
        {
            this.CreateDate = this.CutDate = DateTime.Now;
            this.Summary = string.Empty;
        }

        public void Enter()
        {
            using (var repository = new Layers.Data.Sqls.PsOrderRepository())
            {
                if (!repository.ReadTable<Layers.Data.Sqls.PsOrder.Vouchers>().Any(t => t.ID == this.ID))
                {
                    this.ID = Layers.Data.PKeySigner.Pick(PKeyType.Vourcher);

                    repository.Insert(new Layers.Data.Sqls.PsOrder.Vouchers()
                    {
                        ID = this.ID,
                        PayerID = this.PayerID,
                        PayeeID = this.PayeeID,
                        Type = (int)this.Type,
                        Mode = (int)this.Mode,
                        Summary = this.Summary,
                        CreateDate = this.CreateDate,
                        CutDate = this.CutDate,
                        CutDateIndex = this.CutDateIndex,
                        IsInvoiced = this.IsInvoiced,
                    });
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PsOrder.Vouchers>(new
                    {
                        this.PayerID,
                        this.PayeeID,
                        Type = (int)this.Type,
                        Mode = (int)this.Mode,
                        this.Summary,
                        this.CutDate,
                        this.CutDateIndex,
                        this.IsInvoiced,
                    }, t => t.ID == this.ID);
                }
            }
        }

        public void UpdateInvoiceStatus()
        {
            using (var repository = new Layers.Data.Sqls.PsOrderRepository())
            {
                //更新开票状态
                repository.Update<Layers.Data.Sqls.PsOrder.Vouchers>(new { IsInvoiced = true }, t => t.ID == this.ID);
            }
        }

        public string ToPdf()
        {
            VoucherToPdf pdf = new VoucherToPdf(this);
            string fileName = DateTime.Now.Ticks + ".pdf";

            FileDirectory.CreateDirectory();
            var filePath = FileDirectory.DownLoadRoot + fileName;

            pdf.SaveAs(filePath);

            return filePath;
        }
    }
}
