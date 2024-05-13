using Layers.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Yahv.Payments;
using Yahv.Services;
using Yahv.Services.Extends;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Underly.Erps;
using Yahv.Utils.Serializers;

namespace Wms.Services.Models
{


    public class DataPaymentWaybill : Wms.Services.Models.Waybills
    {
        public object Payments { get; set; }

        public string OrderID { get; set; }
        public string TinyOrderID { get; set; }

        public string PayeeID { get; set; }
        public string ThirdID { get; set; }

        // <summary>
        /// 收款人
        /// </summary>
        public string PayeeName { get; set; }
        // <summary>
        /// 承运商
        /// </summary>
        public string ThirdName { get; set; }


        public static implicit operator PaymentWaybill(DataPaymentWaybill entity)
        {
            return new PaymentWaybill
            {
                WaybillID = entity.WaybillID,
                WaybillType = entity.WaybillType,
                EnterCode = entity.EnterCode,
                ClientName = entity.ClientName,
                Payments = entity.Payments,
                OrderID = entity.OrderID,
                TinyOrderID = entity.TinyOrderID,
                fee = new Fee { WaybillID = entity.WaybillID, OrderID = entity.OrderID },
                CarrierID = entity.CarrierID,
                ClientID = entity.ClientID,
                PayeeID = entity.PayeeID,
                ThirdID = entity.ThirdID,
                ThirdName = entity.ThirdName,
                PayeeName = entity.PayeeName,
                CarrierName = entity.CarrierName,
            };
        }
    }


    public class PaymentWaybill : Wms.Services.Models.Waybills
    {
        public Input Input { get; set; }
        public string OrderID { get; set; }
        public string TinyOrderID { get; set; }
        public string PayeeID { get; set; }
        public string ThirdID { get; set; }
        // <summary>
        /// 收款人
        /// </summary>
        public string PayeeName { get; set; }
        // <summary>
        /// 承运商
        /// </summary>
        public string ThirdName { get; set; }


        public string WaybillTypeDescription
        {
            get
            {
                return this.WaybillType.GetDescription();
            }
        }
        public object Payments { get; set; }


        public Fee fee { get; set; }


    }


    public class Fee
    {
        public Fee()
        {

        }
        IErpAdmin admin;
        public Fee(IErpAdmin admin)
        {
            this.admin = admin;
        }

        public string WaybillID { get; set; }
        public string OrderID { get; set; }
        public string TinyOrderID { get; set; }
        /// <summary>
        /// 业务
        /// </summary>
        public string Conduct { get; set; }
        /// <summary>
        /// 付款人
        /// </summary>
        public string Payer { get; set; }
        /// <summary>
        /// 收款人
        /// </summary>
        public string Payee { get; set; }
        /// <summary>
        /// 匿名
        /// </summary>
        public string Anonymity { get; set; }
        /// <summary>
        /// 科目
        /// </summary>
        public string Subject { get; set; }
        public Currency Currency { get; set; }

        public string CurrencyDescription
        {
            get
            {

                return this.Currency.GetDescription();

            }
        }


        public string CurrencyShortSymbol
        {
            get
            {
                return this.Currency.GetCurrency().ShortSymbol;
            }
        }


        public Currency OriginCurrency { get; set; }

        public string OriginDescription
        {
            get
            {

                return this.OriginCurrency.GetDescription();

            }
        }


        public string OriginCurrencyShortSymbol
        {
            get
            {
                return this.OriginCurrency.GetCurrency().ShortSymbol;
            }
        }

        /// <summary>
        /// 应付（应收）
        /// </summary>
        public decimal LeftPrice { get; set; }
        /// <summary>
        /// 实付（实收）
        /// </summary>
        public decimal? RightPrice { get; set; }
        /// <summary>
        /// 费用类型：in 收，out 支
        /// </summary>
        public string FeeType { get; set; }
        /// <summary>
        /// 到货批次（时间+运单号）
        /// </summary>
        public string ArrivalBatch { get; set; }
        /// <summary>
        /// 文件
        /// </summary>
        public CenterFileDescription[] Files { get; set; } = new CenterFileDescription[] { };
        /// <summary>
        /// 数量
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// 来源（香港库房、深圳库房）
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        public string TrackingNumber { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string Catalog { get; set; }

        /// <summary>
        /// json Data
        /// </summary>
        public JsonData Data { get; set; }

        public void Enter()
        {
            using (var rep = new Layers.Data.Sqls.PvWmsRepository())
            {
                string id = "";
                var tempId = "";
                var payid = "";
                if (FeeType == "in")
                {
                    id = PKeySigner.Pick(Yahv.Underly.PKeyType.Receiveds);
                    tempId = PKeySigner.Pick(PKeyType.Receivables);
                    payid = tempId;
                }
                else
                {
                    id = PKeySigner.Pick(Yahv.Underly.PKeyType.Payments);
                    tempId = PKeySigner.Pick(PKeyType.Payables);

                    payid = tempId;
                }

                if (FeeType == "in")
                {
                    //匿名收入
                    if (this.Payer == AnonymousEnterprise.Current.ID)
                    {
                        PaymentManager.Erp(admin.ID).AnonymPayer(Anonymity, this.Payee)[this.Conduct].Receivable[
                            this.Catalog, this.Subject].Record(this.Currency, this.LeftPrice, this.OrderID, this.WaybillID,
                                id: tempId, rightPrice: this.RightPrice,
                                quantity: this.Quantity <= 0 ? null : this.Quantity, source: this.Source, trackingNum: this.TrackingNumber, data: string.IsNullOrEmpty(this.Data.Value) ? null : this.Data.Json());
                    }
                    else
                    {
                        PaymentManager.Erp(admin.ID)[this.Payer, this.Payee][this.Conduct].Receivable[this.Catalog, this.Subject].Record(this.Currency, this.LeftPrice, this.OrderID, this.WaybillID, id: tempId, rightPrice: this.RightPrice, quantity: this.Quantity <= 0 ? null : this.Quantity, source: this.Source, trackingNum: this.TrackingNumber, data: string.IsNullOrEmpty(this.Data.Value) ? null : this.Data.Json());
                    }

                }

                if (FeeType == "out")
                {
                    //匿名支付
                    if (this.Payee == AnonymousEnterprise.Current.ID)
                    {
                        PaymentManager.Erp(admin.ID).AnonymPayee(Anonymity, this.Payer)[this.Conduct].Payable[this.Catalog, this.Subject]
                      .Record(this.Currency, this.LeftPrice, orderID: this.OrderID, waybillID: this.WaybillID, id: tempId, rightPrice: this.RightPrice, source: this.Source, trackingNum: this.TrackingNumber);
                    }
                    else
                    {
                        PaymentManager.Erp(admin.ID)[this.Payer, this.Payee][this.Conduct].Payable[this.Catalog, this.Subject]
                       .Record(this.Currency, this.LeftPrice, orderID: this.OrderID, waybillID: this.WaybillID, id: tempId, rightPrice: this.RightPrice, source: this.Source, trackingNum: this.TrackingNumber);
                    }
                }

                //保存文件
                if (this.Files != null && this.Files.Length > 0)
                {
                    this.Files.Select(item => { item.PayID = payid; item.WsOrderID = this.OrderID; item.WaybillID = this.WaybillID; return item; }).ToArray().Update(rep);
                }
            }
        }
    }

    public class JsonData
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public JsonData Children { get; set; }
    }
}
