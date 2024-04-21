using Layers.Data.Sqls;
using System;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Linq.Extends;
using Yahv.Payments.Models;
using Yahv.Payments.Models.Rolls;
using Yahv.Payments.Views;
using Yahv.Payments.Views.Rolls;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.Payments
{
    /// <summary>
    /// 确认单详情
    /// </summary>
    public class VoucherDetails
    {
        /// <summary>
        /// 付款人信息
        /// </summary>
        public Services.Models.Enterprise Payer { get; internal set; }

        /// <summary>
        /// 收款人信息
        /// </summary>
        public Services.Models.Enterprise Payee { get; internal set; }
        //public Services.Models.Beneficiary PayeeAccount { get; internal set; }

        /// <summary>
        /// 财务通知类型
        /// </summary>
        public Underly.VoucherType Type { get; internal set; }

        /// <summary>
        /// 对账单信息
        /// </summary>
        public VoucherStatistic[] Items { get; internal set; }

        /// <summary>
        /// 减免记录
        /// </summary>
        public ReducationDto[] ReducationRecords { get; internal set; }

        /// <summary>
        /// 收款记录
        /// </summary>
        public VoucherDto[] VoucherRecord { get; internal set; }
    }

    public class VoucherFactory
    {
        string applicationID;       //申请ID
        string orderID;         //订单ID
        string voucherID;       //账单ID
        Underly.VoucherType type;       //确认单类型

        VoucherFactory()
        {

        }

        public VoucherDetails Factory()
        {
            VoucherDetails result = null;

            using (var reponsitory = new PvbCrmReponsitory())
            {
                if (this.type == VoucherType.Receipt)
                {
                    result = GetReceiptVouchers(reponsitory);
                }
                else if (this.type == VoucherType.Payment)
                {
                    result = GetPaymentVouchers(reponsitory);
                }
            }

            return result;
        }

        public VoucherDetails CreateByApplictionID(string id, Underly.VoucherType type, string orderId = "")
        {

            this.applicationID = id;
            this.orderID = orderId;
            this.type = type;
            return this.Factory();
        }

        public VoucherDetails CreateByOrderID(string id, Underly.VoucherType type)
        {
            this.orderID = id;
            this.applicationID = null;
            this.type = type;
            return this.Factory();
        }

        public VoucherDetails CreateByVoucherID(string id, Underly.VoucherType type)
        {
            this.orderID = null;
            this.applicationID = null;
            this.voucherID = id;
            this.type = type;
            return this.Factory();
        }

        private VoucherDetails GetReceiptVouchers(PvbCrmReponsitory reponsitory)
        {
            Expression<Func<Layers.Data.Sqls.PvbCrm.Vouchers, bool>> where_voucher = item => item.Status == (int)VoucherStatus.Confirmed;
            Expression<Func<Services.Models.VoucherStatistic, bool>> where_voucherStatistic = null;

            if (!string.IsNullOrWhiteSpace(applicationID))
            {
                where_voucher = item => item.ApplicationID == applicationID && item.Type == (int)this.type;
                where_voucherStatistic = item => item.ApplicationID == applicationID;
            }
            if (!string.IsNullOrWhiteSpace(orderID))
            {
                where_voucher = where_voucher.And(item => item.OrderID == orderID && item.Type == (int)this.type);
                where_voucherStatistic = where_voucherStatistic.And(item => item.OrderID == orderID);

                if (string.IsNullOrWhiteSpace(applicationID))
                {
                    where_voucher = where_voucher.And(item => item.ApplicationID == null);
                    where_voucherStatistic = where_voucherStatistic.And(item => item.ApplicationID == null);
                }
            }

            var data_voucher = reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Vouchers>().SingleOrDefault(where_voucher);

            //仓储费
            if (data_voucher?.IsSettlement ?? false)
            {
                if (!string.IsNullOrWhiteSpace(orderID))
                {
                    where_voucherStatistic = item => item.OrderID == orderID ||
                             (item.OrderID == null && item.Payer == data_voucher.Payer && item.Payee == data_voucher.Payee);
                }

                if (!string.IsNullOrWhiteSpace(applicationID))
                {
                    where_voucherStatistic = item => item.ApplicationID == applicationID ||
                            (item.ApplicationID == null && item.Payer == data_voucher.Payer && item.Payee == data_voucher.Payee);
                }
            }


            var statistics = new Yahv.Services.Views.VouchersStatisticsView<PvbCrmReponsitory>(reponsitory).Where(where_voucherStatistic).ToArray();


            var payeeIds = statistics.Where(item => item.Payee != null).Select(item => item.Payee).Distinct();
            var payerIds = statistics.Where(item => item.Payer != null).Select(item => item.Payer).Distinct();
            var enterIds = payeeIds.Concat(payerIds).ToArray();
            var enterprises = new Yahv.Services.Views.EnterprisesTopView<PvbCrmReponsitory>(reponsitory).Where(item => enterIds.Contains(item.ID)).ToArray();

            var receivableIds = statistics.Select(item => item.ReceivableID).ToArray();
            var receiveds = new ReceivedsView(reponsitory).Where(item => item.AccountType == AccountType.Reduction && receivableIds.Contains(item.ReceivableID)).ToArray();
            var reducationRecords = from rd in receiveds
                                    join rb in statistics on rd.ReceivableID equals rb.ReceivableID
                                    select new ReducationDto()
                                    {
                                        ID = rb.ReceivableID,
                                        Business = rb.Business,
                                        Catalog = rb.Catalog,
                                        Subject = rb.Subject,
                                        Price = rd.Price,
                                        Currency = rb.Currency,
                                        CreateDate = rd.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                    };


            return new VoucherDetails
            {
                Payee = enterprises?.SingleOrDefault(item => item.ID == payeeIds.FirstOrDefault()),
                Payer = enterprises?.SingleOrDefault(item => item.ID == payerIds.FirstOrDefault()),
                Items = statistics,
                Type = this.type,
                VoucherRecord = GetRecords(reponsitory, payeeIds.FirstOrDefault(), payerIds.FirstOrDefault()),
                ReducationRecords = reducationRecords.ToArray(),
            };
        }

        private VoucherDetails GetPaymentVouchers(PvbCrmReponsitory reponsitory)
        {
            Expression<Func<Layers.Data.Sqls.PvbCrm.Vouchers, bool>> where_voucher = item => item.Status == (int)VoucherStatus.Confirmed;
            Expression<Func<Services.Models.PaymentsStatistic, bool>> where_paymentsStatistic = null;

            if (!string.IsNullOrWhiteSpace(applicationID))
            {
                where_voucher = item => item.ApplicationID == applicationID && item.Type == (int)this.type;
                where_paymentsStatistic = item => item.ApplicationID == applicationID;
            }
            if (!string.IsNullOrWhiteSpace(orderID))
            {
                where_voucher = where_voucher.And(item => item.OrderID == orderID && item.Type == (int)this.type);
                where_paymentsStatistic = where_paymentsStatistic.And(item => item.OrderID == orderID);

                if (string.IsNullOrWhiteSpace(applicationID))
                {
                    where_voucher = where_voucher.And(item => item.ApplicationID == null);
                    where_paymentsStatistic = where_paymentsStatistic.And(item => item.ApplicationID == null);
                }
            }
            if (!string.IsNullOrWhiteSpace(voucherID))
            {
                where_voucher = item => item.ID == voucherID && item.Type == (int)this.type;
                where_paymentsStatistic = item => item.VoucherID == voucherID;
            }


            //var data_voucher =
            //    reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Vouchers>().SingleOrDefault(where_voucher);
            var statistics =
                new Yahv.Services.Views.PaymentsStatisticsView<PvbCrmReponsitory>(reponsitory).Where(
                    where_paymentsStatistic).ToArray();


            var payeeIds = statistics.Where(item => item.Payee != null).Select(item => item.Payee).Distinct();
            var payerIds = statistics.Where(item => item.Payer != null).Select(item => item.Payer).Distinct();
            var enterIds = payeeIds.Concat(payerIds).ToArray();
            var enterprises =
                new Yahv.Services.Views.EnterprisesTopView<PvbCrmReponsitory>(reponsitory).Where(
                    item => enterIds.Contains(item.ID)).ToArray();

            var payableIds = statistics.Select(item => item.PayableID).ToArray();
            var payments =
                new PaymentsView(reponsitory).Where(
                    item => item.AccountType == AccountType.Reduction && payableIds.Contains(item.PayableID))
                    .ToArray();
            var reducationRecords = from pmt in payments
                                    join stt in statistics on pmt.PayableID equals stt.PayableID
                                    select new ReducationDto()
                                    {
                                        ID = stt.PayableID,
                                        Business = stt.Business,
                                        Catalog = stt.Catalog,
                                        Subject = stt.Subject,
                                        Price = pmt.Price,
                                        Currency = stt.Currency,
                                        CreateDate = pmt.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                    };


            return new VoucherDetails
            {
                Payee = enterprises?.SingleOrDefault(item => item.ID == payeeIds.FirstOrDefault()),
                Payer = enterprises?.SingleOrDefault(item => item.ID == payerIds.FirstOrDefault()),
                Items = statistics.Select(item => new VoucherStatistic()
                {
                    Currency = item.Currency,
                    OrderID = item.OrderID,
                    Catalog = item.Catalog,
                    Business = item.Business,
                    AdminID = item.AdminID,
                    ReceivableID = item.PayableID,
                    Payer = item.Payer,
                    Payee = item.Payee,
                    WaybillID = item.WaybillID,
                    Subject = item.Subject,
                    ApplicationID = item.ApplicationID,
                    LeftDate = item.LeftDate,
                    LeftPrice = item.LeftPrice,
                    TinyID = item.TinyID,
                    RightDate = item.RightDate,
                    RightPrice = item.RightPrice,
                    Status = item.Status,
                    ReducePrice = item.ReducePrice,
                }).ToArray(),
                Type = this.type,
                VoucherRecord = GetRecords(reponsitory, payeeIds.FirstOrDefault(), payerIds.FirstOrDefault()),
                ReducationRecords = reducationRecords.ToArray(),
            };
        }

        #region bak
        //private VoucherDetails GetPaymentVouchers(PvbCrmReponsitory reponsitory)
        //{
        //    Expression<Func<Layers.Data.Sqls.PvbCrm.Vouchers, bool>> where_voucher = null;
        //    Expression<Func<Services.Models.PaymentsStatistic, bool>> where_paymentsStatistic = null;

        //    if (!string.IsNullOrWhiteSpace(applicationID))
        //    {
        //        where_voucher = item => item.ApplicationID == applicationID && item.Type == (int)this.type;
        //        where_paymentsStatistic = item => item.ApplicationID == applicationID;
        //    }
        //    if (!string.IsNullOrWhiteSpace(orderID))
        //    {
        //        where_voucher = item => item.OrderID == orderID && item.Type == (int)this.type;
        //        where_paymentsStatistic = item => item.OrderID == orderID;
        //    }

        //    //var data_voucher =
        //    //    reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Vouchers>().SingleOrDefault(where_voucher);
        //    var statistics =
        //        new Yahv.Services.Views.PaymentsStatisticsView<PvbCrmReponsitory>(reponsitory).Where(
        //            where_paymentsStatistic).ToArray();


        //    var payeeIds = statistics.Where(item => item.Payee != null).Select(item => item.Payee).Distinct();
        //    var payerIds = statistics.Where(item => item.Payer != null).Select(item => item.Payer).Distinct();
        //    var enterIds = payeeIds.Concat(payerIds).ToArray();
        //    var enterprises =
        //        new Yahv.Services.Views.EnterprisesTopView<PvbCrmReponsitory>(reponsitory).Where(
        //            item => enterIds.Contains(item.ID)).ToArray();

        //    var payableIds = statistics.Select(item => item.PayableID).ToArray();
        //    var payments =
        //        new PaymentsView(reponsitory).Where(
        //            item => item.AccountType == AccountType.Reduction && payableIds.Contains(item.PayableID))
        //            .ToArray();
        //    var reducationRecords = from pmt in payments
        //                            join stt in statistics on pmt.PayableID equals stt.PayableID
        //                            select new ReducationDto()
        //                            {
        //                                ID = stt.PayableID,
        //                                Business = stt.Business,
        //                                Catalog = stt.Catalog,
        //                                Subject = stt.Subject,
        //                                Price = pmt.Price,
        //                                Currency = stt.Currency,
        //                                CreateDate = pmt.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
        //                            };


        //    return new VoucherDetails
        //    {
        //        Payee = enterprises?.SingleOrDefault(item => item.ID == payeeIds.FirstOrDefault()),
        //        Payer = enterprises?.SingleOrDefault(item => item.ID == payerIds.FirstOrDefault()),
        //        Items = statistics.Select(item => new VoucherStatistic()
        //        {
        //            Currency = item.Currency,
        //            OrderID = item.OrderID,
        //            Catalog = item.Catalog,
        //            Business = item.Business,
        //            AdminID = item.AdminID,
        //            ReceivableID = item.PayableID,
        //            Payer = item.Payer,
        //            Payee = item.Payee,
        //            WaybillID = item.WaybillID,
        //            Subject = item.Subject,
        //            ApplicationID = item.ApplicationID,
        //            LeftDate = item.LeftDate,
        //            LeftPrice = item.LeftPrice,
        //            TinyID = item.TinyID,
        //            RightDate = item.RightDate,
        //            RightPrice = item.RightPrice,
        //            Status = item.Status,
        //            ReducePrice = item.ReducePrice,
        //        }).ToArray(),
        //        Type = this.type,
        //        VoucherRecord = GetRecords(reponsitory, payeeIds.FirstOrDefault(), payerIds.FirstOrDefault()),
        //        ReducationRecords = reducationRecords.ToArray(),
        //    };
        //} 
        #endregion

        private VoucherDto[] GetRecords(PvbCrmReponsitory reponsitory, string payee, string payer)
        {
            Expression<Func<Models.Origins.FlowAccount, bool>> predicate =
                item => (item.Type == AccountType.BankStatement || item.Type == AccountType.Cash)
                        && item.Payee == payee
                        && item.Payer == payer
                        && item.Price < 0;

            if (!string.IsNullOrWhiteSpace(orderID))
            {
                predicate = predicate.And(item => item.OrderID == orderID);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(applicationID))
                {
                    string[] flowIds = null;

                    //根据applicationId获取所有流水ID
                    if (this.type == VoucherType.Receipt)
                    {
                        var receivableIds =
                            new ReceivablesView(reponsitory).Where(item => item.ApplicationID == applicationID)
                                .Select(item => item.ID)
                                .ToArray();
                        flowIds =
                            new ReceivedsView(reponsitory).Where(item => receivableIds.Contains(item.ReceivableID))
                                .Select(item => item.FlowID)
                                .ToArray();
                    }
                    else if (this.type == VoucherType.Payment)
                    {
                        var payableIds =
                            new PayablesView(reponsitory).Where(item => item.ApplicationID == applicationID)
                                .Select(item => item.ID)
                                .ToArray();
                        flowIds =
                            new PaymentsView(reponsitory).Where(item => payableIds.Contains(item.PayableID))
                                .Select(item => item.FlowID)
                                .ToArray();
                    }

                    predicate = predicate.And(item => flowIds.Contains(item.ID));
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(voucherID))
                    {
                        string[] flowIds = null;

                        if (this.type == VoucherType.Payment)
                        {
                            var payableIds =
                                new PayablesView(reponsitory).Where(item => item.VoucherID == voucherID)
                                    .Select(item => item.ID)
                                    .ToArray();
                            flowIds =
                                new PaymentsView(reponsitory).Where(item => payableIds.Contains(item.PayableID))
                                    .Select(item => item.FlowID)
                                    .ToArray();
                        }

                        predicate = predicate.And(item => flowIds.Contains(item.ID));
                    }
                }
            }


            return new FlowAccountsRoll(reponsitory).Where(predicate)
                    .Select(item => new VoucherDto
                    {
                        Type = this.type,
                        Currency = item.Currency,
                        Price = item.Price,
                        Payee = item.Payee,
                        Payer = item.Payer,
                        FormCode = item.FormCode,
                        OrderID = item.OrderID,
                        Account = item.Account,
                        Bank = item.Bank,
                        CreateDate = item.CreateDate,
                        CreatorID = item.Admin.RealName,
                    }).ToArray();
        }

        static object locker = new object();
        static private VoucherFactory current;
        static public VoucherFactory Current
        {
            get
            {
                if (current == null)
                {
                    lock (locker)
                    {
                        if (current == null)
                        {
                            current = new VoucherFactory();
                        }
                    }
                }

                return current;
            }
        }

    }
}
