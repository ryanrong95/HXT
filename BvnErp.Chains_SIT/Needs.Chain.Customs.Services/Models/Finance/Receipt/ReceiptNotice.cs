using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layer.Data.Sqls.ScCustoms;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 收款通知
    /// </summary>
    public class ReceiptNotice : FinanceReceipt
    {
        #region 属性

        public Client Client { get; set; }

        /// <summary>
        /// 已添加到订单收款明细的金额
        /// </summary>
        public decimal ClearAmount { get; set; }
        /// <summary>
        /// 可核销金额
        /// </summary>
        public decimal AvailableAmount { get; set; }

        public DateTime? LastReceiptDate { get; set; }       
        #endregion

        public ReceiptNotice() : base()
        {
            this.ClearAmount = 0;
        }

        public ReceiptNotice(FinanceReceipt financeReceipt) : this()
        {
            this.ID = financeReceipt.ID;
            this.Client = new Views.ClientsView().FirstOrDefault(item => item.Company.Name == financeReceipt.Payer);
            this.Summary = financeReceipt.Summary;
            this.AvailableAmount = financeReceipt.Amount;
        }

        override public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ReceiptNotices>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }
        }
    }
}
