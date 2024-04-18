using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;

namespace Yahv.PsWms.SzMvc.Services.Models.Origin
{
    /// <summary>
    /// 应收账目
    /// </summary>
    public class PayeeLeft : IUnique
    {
        #region

        public string ID { get; set; }

        public Enums.AccountSource Source { get; set; }

        /// <summary>
        /// 付款公司ID, ClientID
        /// </summary>
        public string PayerID { get; set; }

        /// <summary>
        /// 收款公司ID, 内部来自内部公司 默认：芯达通
        /// </summary>
        public string PayeeID { get; set; }

        public Enums.Conduct Conduct { get; set; }

        /// <summary>
        /// 科目分类, 参考《联创杰国内仓储服务协议2020-12-17》-附件：收费标准
        /// </summary>
        public string Subject { get; set; }

        public Underly.Currency Currency { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        public decimal Total { get; set; }

        /// <summary>
        /// 发生日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 结算日期
        /// </summary>
        public DateTime? CutDate { get; set; }

        /// <summary>
        /// 结算期号, 月结账单提取的依据 [202012]
        /// </summary>
        public int? CutDateIndex { get; set; }

        /// <summary>
        /// 发生通知ID
        /// </summary>
        public string NoticeID { get; set; }

        /// <summary>
        /// 发生单据ID
        /// </summary>
        public string FormID { get; set; }

        /// <summary>
        /// 发生运单ID
        /// </summary>
        public string WaybillCode { get; set; }

        /// <summary>
        /// 录入人
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// VoucherID
        /// </summary>
        public string VoucherID { get; set; }
        #endregion

        public PayeeLeft()
        {
            this.VoucherID = string.Empty;
        }

        public void Enter()
        {
            using (var repository = new Layers.Data.Sqls.PsOrderRepository())
            {
                if (!repository.ReadTable<Layers.Data.Sqls.PsOrder.PayeeLefts>().Any(t => t.ID == this.ID))
                {
                    this.ID = Layers.Data.PKeySigner.Pick(PKeyType.PayeeLeft);
                    repository.Insert(new Layers.Data.Sqls.PsOrder.PayeeLefts()
                    {
                        ID = this.ID,
                        Source = (int)this.Source,
                        PayerID = this.PayerID,
                        PayeeID = this.PayeeID,
                        Conduct = (int)this.Conduct,
                        Subject = this.Subject,
                        Currency = (int)this.Currency,
                        Quantity = this.Quantity,
                        UnitPrice = this.UnitPrice,
                        Unit = this.Unit,
                        Total = this.Total,
                        NoticeID = this.NoticeID,
                        FormID = this.FormID,
                        WaybillCode = this.WaybillCode,
                        AdminID = this.AdminID,
                        VoucherID = this.VoucherID,
                        CreateDate = this.CreateDate,
                        CutDate = this.CutDate,
                        CutDateIndex = this.CutDateIndex,
                    });
                }
                else
                {
                    repository.Update<Layers.Data.Sqls.PsOrder.PayeeLefts>(new
                    {
                        Source = (int)this.Source,
                        this.PayerID,
                        this.PayeeID,
                        Conduct = (int)this.Conduct,
                        this.Subject,
                        Currency = (int)this.Currency,
                        this.Quantity,
                        this.UnitPrice,
                        this.Unit,
                        this.Total,
                        this.NoticeID,
                        this.FormID,
                        this.WaybillCode,
                        this.AdminID,
                        this.VoucherID,
                        this.CutDate,
                        this.CutDateIndex,
                    }, t => t.ID == this.ID);
                }
            }
        }
    }
}
