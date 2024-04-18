using Layers.Data.Sqls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Models
{
    public class InvoiceNotice : IUnique
    {
        #region 属性

        public string ID { get; set; }

        public string ClientID { get; set; }

        public bool IsPersonal { get; set; }

        //发票类型
        public Enums.InvoiceFromType FromType { get; set; }

        //发票类型
        public InvoiceType Type { get; set; }

        /// <summary>
        /// 发票抬头
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxNumber { set; get; }
        /// <summary>
        /// 企业注册地址
        /// </summary>
        public string RegAddress { set; get; }
        /// <summary>
        /// 企业电话
        /// </summary>
        public string Tel { set; get; }
        /// <summary>
        /// 开户银行
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// 银行账号
        /// </summary>
        public string BankAccount { set; get; }
        /// <summary>
        /// 收票地址
        /// </summary>
        public string PostAddress { get; set; }
        /// <summary>
        /// 收票人
        /// </summary>
        public string PostRecipient { set; get; }
        /// <summary>
        /// 收票人联系电话
        /// </summary>
        public string PostTel { set; get; }
        /// <summary>
        /// 交付方式
        /// </summary>
        public InvoiceDeliveryType DeliveryType { set; get; }
        public string Carrier { get; set; }
        public string WayBillCode { get; set; }
        public Enums.InvoiceEnum Status { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string AdminID { get; set; }
        public string Summary { get; set; }
        /// <summary>
        /// 是否已经生成凭证
        /// </summary>

        public bool InvoiceCreSta { get; set; }
        #endregion

        public IEnumerable<InvoiceNoticeItem> InvoiceNoticeItem { get; set; }

        public InvoiceNotice()
        {
            this.FromType = Enums.InvoiceFromType.HKStore;
            this.CreateDate = this.UpdateDate = DateTime.Now;
            this.Status = Enums.InvoiceEnum.UnInvoiced;
            this.InvoiceCreSta = false;

        }

        public void Enter()
        {
            using (var Reponsitory = new PvWsOrderReponsitory())
            {
                if (!Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.InvoiceNotices>().Any(item => item.ID == this.ID))
                {
                    this.ID = Layers.Data.PKeySigner.Pick(PKeyType.invoiceNotice);
                    Reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.InvoiceNotices()
                    {
                        ID = this.ID,
                        ClientID = this.ClientID,
                        IsPersonal = this.IsPersonal,
                        FromType = (int)this.FromType,
                        Type = (int)this.Type,
                        Title = this.Title,
                        TaxNumber = this.TaxNumber,
                        RegAddress = this.RegAddress,
                        Tel = this.Tel,
                        BankName = this.BankName,
                        BankAccount = this.BankAccount,
                        PostAddress = this.PostAddress,
                        PostRecipient = this.PostRecipient,
                        PostTel = this.PostTel,
                        DeliveryType = (int)this.DeliveryType,
                        Status = (int)this.Status,
                        InvoiceDate = this.InvoiceDate,
                        AdminID = this.AdminID,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                        Summary = this.Summary,
                        InvoiceCreSta = this.InvoiceCreSta
                    });
                }

                var billIDs = InvoiceNoticeItem.Select(s => s.BillID).ToArray();
                var invoiceNoticeItems = InvoiceNoticeItem.Select(item => new Layers.Data.Sqls.PvWsOrder.InvoiceNoticeItems()
                {
                    ID = Layers.Data.PKeySigner.Pick(PKeyType.invoiceNoticeItem),
                    InvoiceNoticeID = this.ID,
                    BillID = item.BillID,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    Amount = item.Amount,
                    Difference = item.Difference,
                    InvoiceNo = item.InvoiceNo,
                    Status = (int)GeneralStatus.Normal,
                    CreateDate = DateTime.Now,
                    Summary = item.Summary,
                }).ToArray();

                Reponsitory.Insert(invoiceNoticeItems);

                //更新bill开票状态
                Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.Bills>(new { IsInvoice = true }, t => billIDs.Contains(t.ID));
            }
        }
    }
}
