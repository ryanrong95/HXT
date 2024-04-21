using System;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Payments.Models.Origins
{
    public class Log_Receivable : IUnique
    {
        #region 属性
        public string ID { get; set; }
        public string OriginID { get; set; }
        public string Payer { get; internal set; }
        public string PayerID { get; set; }
        public string Payee { get; internal set; }
        internal string PayeeID { get; set; }
        public string Business { get; set; }
        public string Catalog { get; set; }
        public string Subject { get; set; }
        public Currency Currency { get; set; }
        public decimal Price { get; set; }
        public Currency Currency1 { get; set; }
        public decimal Price1 { get; set; }
        public decimal Rate1 { get; set; }
        public Currency Currency11 { get; set; }
        public decimal Price11 { get; set; }
        public decimal Rate11 { get; set; }
        public Currency SettlementCurrency { get; set; }
        public decimal SettlementPrice { get; set; }
        public decimal SettlementRate { get; set; }
        public string OrderID { get; set; }
        public string WaybillID { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? OriginalDate { get; set; }
        public DateTime? ChangeDate { get; set; }
        public int? OriginalIndex { get; set; }
        public int? ChangeIndex { get; set; }
        public string AdminID { get; set; }
        public string Summay { get; set; }
        public string AccountCode { get; set; }
        public string ItemID { get; set; }
        /// <summary>
        /// 小订单ID
        /// </summary>
        public string TinyID { get; set; }

        /// <summary>
        /// 付汇申请ID
        /// </summary>
        public string ApplicationID { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        //public decimal UnitPrice { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// 匿名付款人
        /// </summary>
        public string PayerAnonymous { get; set; }

        /// <summary>
        /// 匿名收款人
        /// </summary>
        public string PayeeAnonymous { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        public string TrackingNumber { get; set; }

        /// <summary>
        /// json数据
        /// </summary>
        public string Data { get; set; }
        #endregion

        #region 持久化

        public void Enter(PvbCrmReponsitory reponsitory = null)
        {
            if (reponsitory != null)
            {
                Insert(reponsitory);
            }
            else
            {
                using (reponsitory = LinqFactory<PvbCrmReponsitory>.Create())
                {
                    Insert(reponsitory);
                }
            }
        }

        private void Insert(PvbCrmReponsitory reponsitory)
        {
            reponsitory.Insert(new Layers.Data.Sqls.PvbCrm.Logs_Receivable()
            {
                ID = this.ID ?? PKeySigner.Pick(PKeyTypes.Logs_Receivable),
                OriginID = this.OriginID,
                Payer = this.Payer,
                PayerID = this.PayerID,
                Payee = this.Payee,
                PayeeID = this.PayeeID,
                Business = this.Business,
                Catalog = this.Catalog,
                Subject = this.Subject,
                TinyID = this.TinyID,
                ItemID = this.ItemID,
                ApplicationID = this.ApplicationID,

                Currency = (int)this.Currency,
                Price = this.Price,

                Quantity = this.Quantity,

                Currency1 = (int)Currency.CNY,
                Rate1 = this.Rate1,
                Price1 = this.Price1,

                Currency11 = (int)Currency.USD,
                Rate11 = this.Rate11,
                Price11 = this.Price11.Round(),

                SettlementCurrency = (int)this.SettlementCurrency,
                SettlementPrice = this.SettlementPrice,
                SettlementRate = this.SettlementRate,

                OrderID = this.OrderID,
                WaybillID = this.WaybillID,
                CreateDate = DateTime.Now,
                AdminID = this.AdminID,
                Summay = this.Summay,

                OriginalIndex = this.OriginalIndex,
                OriginalDate = this.OriginalDate,
                ChangeDate = this.ChangeDate,
                ChangeIndex = this.ChangeIndex,
                PayeeAnonymous = this.PayeeAnonymous,
                PayerAnonymous = this.PayerAnonymous,
                Source = this.Source,
                TrackingNumber = this.TrackingNumber,
                Data = this.Data,
            });
        }
        #endregion
    }
}