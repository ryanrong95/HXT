using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientAgreementView<TReponsitory> : UniqueView<ClientAgreement, TReponsitory>
           where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        private IUser User;

        public ClientAgreementView(IUser user)
        {
            this.User = user;
        }

        public ClientAgreementView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<ClientAgreement> GetIQueryable()
        {
            var linq = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.ClientAgreements>().Where(item => item.Status == (int)GeneralStatus.Normal)
                       join feeSettlement in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.ClientFeeSettlements>().Where(item => item.Status == (int)GeneralStatus.Normal)
                       on entity.ID equals feeSettlement.AgreementID into feeSettlements
                       where entity.ClientID == this.User.XDTClientID
                       select new ClientAgreement
                       {
                           ID = entity.ID,
                           ClientID = entity.ClientID,
                           AgreementCode = entity.AgreementCode,
                           StartDate = entity.StartDate,
                           EndDate = entity.EndDate,
                           PreAgency = entity.PreAgency,
                           AgencyRate = entity.AgencyRate,
                           MinAgencyFee = entity.MinAgencyFee,
                           IsPrePayExchange = entity.IsPrePayExchange,
                           IsLimitNinetyDays = entity.IsLimitNinetyDays,
                           InvoiceType = (Invoice)entity.InvoiceType,
                           InvoiceTaxRate = entity.InvoiceTaxRate,
                           clientFeeSettlements = feeSettlements.Select(item => new ClientFeeSettlement
                           {
                               ID = item.ID,
                               AgreementID = item.AgreementID,
                               FeeType = item.FeeType,
                               PeriodType = (PeriodType)item.PeriodType,
                               ExchangeRateType = (ExchangeRateType)item.ExchangeRateType,
                               ExchangeRateValue = item.ExchangeRateValue,
                               DaysLimit = item.DaysLimit,
                               MonthlyDay = item.MonthlyDay,
                               UpperLimit = item.UpperLimit,
                               Status = item.Status,
                               CreateDate = item.CreateDate,
                               UpdateDate = item.UpdateDate,
                               Summary = item.Summary
                           }).ToArray(),
                           AdminID = entity.AdminID,
                           IsTen = (PEIsTen)entity.IsTen,
                           Status = entity.Status,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           Summary = entity.Summary
                       };

            return linq;
        }

        public IQueryable<ClientAgreement> GetIQueryableNoUser()
        {
            var linq = from entity in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.ClientAgreements>().Where(item => item.Status == (int)GeneralStatus.Normal)
                       join feeSettlement in this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.ClientFeeSettlements>().Where(item => item.Status == (int)GeneralStatus.Normal)
                       on entity.ID equals feeSettlement.AgreementID into feeSettlements
                       select new ClientAgreement
                       {
                           ID = entity.ID,
                           ClientID = entity.ClientID,
                           AgreementCode = entity.AgreementCode,
                           StartDate = entity.StartDate,
                           EndDate = entity.EndDate,
                           PreAgency = entity.PreAgency,
                           AgencyRate = entity.AgencyRate,
                           MinAgencyFee = entity.MinAgencyFee,
                           IsPrePayExchange = entity.IsPrePayExchange,
                           IsLimitNinetyDays = entity.IsLimitNinetyDays,
                           InvoiceType = (Invoice)entity.InvoiceType,
                           InvoiceTaxRate = entity.InvoiceTaxRate,
                           clientFeeSettlements = feeSettlements.Select(item => new ClientFeeSettlement
                           {
                               ID = item.ID,
                               AgreementID = item.AgreementID,
                               FeeType = item.FeeType,
                               PeriodType = (PeriodType)item.PeriodType,
                               ExchangeRateType = (ExchangeRateType)item.ExchangeRateType,
                               ExchangeRateValue = item.ExchangeRateValue,
                               DaysLimit = item.DaysLimit,
                               MonthlyDay = item.MonthlyDay,
                               UpperLimit = item.UpperLimit,
                               Status = item.Status,
                               CreateDate = item.CreateDate,
                               UpdateDate = item.UpdateDate,
                               Summary = item.Summary
                           }).ToArray(),
                           AdminID = entity.AdminID,
                           IsTen = (PEIsTen)entity.IsTen,
                           Status = entity.Status,
                           CreateDate = entity.CreateDate,
                           UpdateDate = entity.UpdateDate,
                           Summary = entity.Summary
                       };

            return linq;
        }

        /// <summary>
        /// 获取结算日期
        /// </summary>
        /// <returns></returns>
        public DateTime GetDueDate()
        {
            //获取当前客户协议
            var agreement = this.GetIQueryable().FirstOrDefault();
            var tempduedate = DateTime.Now;

            #region 货款协议
            //货款协议
            var productagreement = agreement.clientFeeSettlements.FirstOrDefault(item => item.FeeType == 1);
            switch (productagreement.PeriodType)
            {
                case PeriodType.AgreedPeriod:
                    var daysLimit = productagreement.DaysLimit.Value;
                    tempduedate = DateTime.Now.AddDays(daysLimit);
                    break;
                case PeriodType.Monthly:
                    var monthlyDay = productagreement.MonthlyDay.Value;
                    tempduedate = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, monthlyDay);
                    break;
                case PeriodType.PrePaid:
                    tempduedate = DateTime.Now;
                    break;
            }
            var duedate = tempduedate;
            #endregion

            #region 税款协议
            //税款协议
            var taxagreement = agreement.clientFeeSettlements.FirstOrDefault(item => item.FeeType == 2);
            switch (taxagreement.PeriodType)
            {
                case PeriodType.AgreedPeriod:
                    var daysLimit = taxagreement.DaysLimit.Value;
                    tempduedate = DateTime.Now.AddDays(daysLimit);
                    break;
                case PeriodType.Monthly:
                    var monthlyDay = taxagreement.MonthlyDay.Value;
                    tempduedate = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, monthlyDay);
                    break;
                case PeriodType.PrePaid:
                    tempduedate = DateTime.Now;
                    break;
            }

            if (duedate > tempduedate)
            {
                duedate = tempduedate;
            }
            #endregion

            #region 代理费协议
            var agencyagreement = agreement.clientFeeSettlements.FirstOrDefault(item => item.FeeType == 3);
            switch (agencyagreement.PeriodType)
            {
                case PeriodType.AgreedPeriod:
                    var daysLimit = agencyagreement.DaysLimit.Value;
                    tempduedate = DateTime.Now.AddDays(daysLimit);
                    break;
                case PeriodType.Monthly:
                    var monthlyDay = agencyagreement.MonthlyDay.Value;
                    tempduedate = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, monthlyDay);
                    break;
                case PeriodType.PrePaid:
                    tempduedate = DateTime.Now;
                    break;
            }

            if (duedate > tempduedate)
            {
                duedate = tempduedate;
            }
            #endregion

            #region 杂费协议
            var incidental = agreement.clientFeeSettlements.FirstOrDefault(item => item.FeeType == 4);
            switch (incidental.PeriodType)
            {
                case PeriodType.AgreedPeriod:
                    var daysLimit = incidental.DaysLimit.Value;
                    tempduedate = DateTime.Now.AddDays(daysLimit);
                    break;
                case PeriodType.Monthly:
                    var monthlyDay = incidental.MonthlyDay.Value;
                    tempduedate = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, monthlyDay);
                    break;
                case PeriodType.PrePaid:
                    tempduedate = DateTime.Now;
                    break;
            }

            if (duedate > tempduedate)
            {
                duedate = tempduedate;
            }
            #endregion

            return duedate;
        }

        /// <summary>
        /// 获取结算日期计算的基准时间
        /// </summary>
        /// <param name="mainOrderID"></param>
        /// <returns></returns>
        private DateTime GetStandardTime(string mainOrderID)
        {
            bool declareIsSuccess = false;
            DateTime DDate = DateTime.Now;
            DateTime OrderCreateDate = DateTime.Now;

            using (var reponsitory = LinqFactory<ScCustomReponsitory>.Create())
            {
                var Orders = reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Orders>();
                var DecHeads = reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.DecHeads>();

                //var tinyOrderIDs = orders
                //    .Where(t => t.MainOrderId == mainOrderID && t.Status == (int)GeneralStatus.Normal)
                //    .Select(item => item.ID).ToArray();

                //var decHeads = decHeads
                //    .Where(t => tinyOrderIDs.Contains(t.OrderID))
                //    .Select(item => new
                //    {
                //        TinyOrderID = item.OrderID,
                //        IsSuccess = item.IsSuccess,
                //        DDate = item.DDate,
                //    }).ToArray();

                var decHeadInfos = (from order in Orders
                                    join decHead in DecHeads on order.ID equals decHead.OrderID into DecHeads2
                                    from decHead in DecHeads2.DefaultIfEmpty()
                                    where order.MainOrderId == mainOrderID
                                       && order.Status == (int)GeneralStatus.Normal
                                    orderby order.CreateDate ascending
                                    select new
                                    {
                                        OrderID = order.ID,
                                        OrderCreateDate = order.CreateDate,
                                        IsSuccess = decHead != null ? decHead.IsSuccess : false,
                                        DDate = decHead != null ? decHead.DDate : null,
                                    }).ToArray();

                if (decHeadInfos != null && decHeadInfos.Length > 0)
                {
                    var successDecHeads = decHeadInfos.Where(t => t.IsSuccess == true).OrderBy(t => t.DDate).ToArray();

                    if (successDecHeads != null && successDecHeads.Length > 0)
                    {
                        //如果有报关成功的
                        declareIsSuccess = true;
                        DDate = (DateTime)successDecHeads[0].DDate;
                        OrderCreateDate = successDecHeads[0].OrderCreateDate;
                    }
                    else
                    {
                        //如果没有报关成功的
                        decHeadInfos = decHeadInfos.OrderBy(t => t.OrderCreateDate).ToArray();

                        declareIsSuccess = false;
                        DDate = DateTime.Now;
                        OrderCreateDate = decHeadInfos[0].OrderCreateDate;
                    }
                }
            }

            if (declareIsSuccess)
            {
                return DDate;
            }
            else
            {
                return OrderCreateDate;
            }
        }

        private DateTime? GetMinAdvanceTime(string mainOrderID)
        {
            DateTime? minAdvanceTime = null;

            using (var reponsitory = LinqFactory<ScCustomReponsitory>.Create())
            {
                var Orders = reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Orders>();
                var AdvanceRecords = reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.AdvanceRecords>();

                var advanceRecordInfos = (from orders in Orders
                                          join advanceRecords in AdvanceRecords on orders.ID equals advanceRecords.OrderID
                                          where orders.MainOrderId == mainOrderID && orders.Status == (int)GeneralStatus.Normal
                                          select new
                                          {
                                              TinyOrderID = orders.ID,
                                              MainOrderID = orders.MainOrderId,
                                              Amount = advanceRecords.Amount,
                                              AdvanceTime = advanceRecords.AdvanceTime,
                                              LimitDays = advanceRecords.LimitDays
                                          }).ToArray();

                if (advanceRecordInfos != null && advanceRecordInfos.Length > 0)
                {
                    foreach (var advanceRecordInfo in advanceRecordInfos)
                    {
                        var nextTime = advanceRecordInfo.AdvanceTime.AddDays(advanceRecordInfo.LimitDays);

                        if (minAdvanceTime == null)
                        {
                            minAdvanceTime = nextTime;
                        }
                        else if (minAdvanceTime > nextTime)
                        {
                            minAdvanceTime = nextTime;
                        }
                    }
                }
            }

            return minAdvanceTime;
        }

        /// <summary>
        /// 获取结算日期 New
        /// </summary>
        /// <returns></returns>
        public DateTime GetDueDateNew(string mainOrderID)
        {
            DateTime standardTime = GetStandardTime(mainOrderID);

            //获取当前客户协议
            var agreement = this.GetIQueryable().FirstOrDefault();
            var tempduedate = DateTime.Now;

            //#region 货款协议
            ////货款协议
            //var productagreement = agreement.clientFeeSettlements.FirstOrDefault(item => item.FeeType == 1);
            //switch (productagreement.PeriodType)
            //{
            //    case PeriodType.AgreedPeriod:
            //        var daysLimit = productagreement.DaysLimit.Value;
            //        tempduedate = DateTime.Now.AddDays(daysLimit);
            //        break;
            //    case PeriodType.Monthly:
            //        var monthlyDay = productagreement.MonthlyDay.Value;
            //        tempduedate = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, monthlyDay);
            //        break;
            //    case PeriodType.PrePaid:
            //        tempduedate = DateTime.Now;
            //        break;
            //}
            //var duedate = tempduedate;
            //#endregion

            #region 税款协议
            //税款协议
            var taxagreement = agreement.clientFeeSettlements.FirstOrDefault(item => item.FeeType == 2);
            switch (taxagreement.PeriodType)
            {
                case PeriodType.AgreedPeriod:
                    var daysLimit = taxagreement.DaysLimit.Value;
                    tempduedate = standardTime.AddDays(daysLimit);
                    break;
                case PeriodType.Monthly:
                    var monthlyDay = taxagreement.MonthlyDay.Value;
                    tempduedate = new DateTime(standardTime.AddMonths(1).Year, standardTime.AddMonths(1).Month, monthlyDay);
                    break;
                case PeriodType.PrePaid:
                    tempduedate = standardTime;
                    break;
            }

            //if (duedate > tempduedate)
            //{
            //    duedate = tempduedate;
            //}

            var duedate = tempduedate;

            #endregion

            #region 代理费协议
            var agencyagreement = agreement.clientFeeSettlements.FirstOrDefault(item => item.FeeType == 3);
            switch (agencyagreement.PeriodType)
            {
                case PeriodType.AgreedPeriod:
                    var daysLimit = agencyagreement.DaysLimit.Value;
                    tempduedate = standardTime.AddDays(daysLimit);
                    break;
                case PeriodType.Monthly:
                    var monthlyDay = agencyagreement.MonthlyDay.Value;
                    tempduedate = new DateTime(standardTime.AddMonths(1).Year, standardTime.AddMonths(1).Month, monthlyDay);
                    break;
                case PeriodType.PrePaid:
                    tempduedate = standardTime;
                    break;
            }

            if (duedate > tempduedate)
            {
                duedate = tempduedate;
            }
            #endregion

            #region 杂费协议
            var incidental = agreement.clientFeeSettlements.FirstOrDefault(item => item.FeeType == 4);
            switch (incidental.PeriodType)
            {
                case PeriodType.AgreedPeriod:
                    var daysLimit = incidental.DaysLimit.Value;
                    tempduedate = standardTime.AddDays(daysLimit);
                    break;
                case PeriodType.Monthly:
                    var monthlyDay = incidental.MonthlyDay.Value;
                    tempduedate = new DateTime(standardTime.AddMonths(1).Year, standardTime.AddMonths(1).Month, monthlyDay);
                    break;
                case PeriodType.PrePaid:
                    tempduedate = standardTime;
                    break;
            }

            if (duedate > tempduedate)
            {
                duedate = tempduedate;
            }
            #endregion

            DateTime? minAdvanceTime = GetMinAdvanceTime(mainOrderID);

            if (minAdvanceTime != null && minAdvanceTime < duedate)
            {
                duedate = (DateTime)minAdvanceTime;
            }

            return duedate;
        }
    }


    public class ClientAgreement : Yahv.Linq.IUnique
    {
        public string ID { get; set; }

        public string ClientID { get; set; }

        public string AgreementCode { get; set; }

        public decimal? PreAgency { get; set; }

        public decimal AgencyRate { get; set; }

        public decimal MinAgencyFee { get; set; }

        public bool IsPrePayExchange { get; set; }

        public bool IsLimitNinetyDays { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Invoice InvoiceType { get; set; }

        public decimal InvoiceTaxRate { get; set; }

        public string AdminID { get; set; }

        public PEIsTen IsTen { get; set; }

        public int Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public ClientFeeSettlement[] clientFeeSettlements { get; set; }
    }

    public class ClientFeeSettlement
    {
        public string ID { get; set; }

        public string AgreementID { get; set; }

        public int FeeType { get; set; }

        public PeriodType PeriodType { get; set; }

        public ExchangeRateType ExchangeRateType { get; set; }

        public decimal? ExchangeRateValue { get; set; }

        public int? DaysLimit { get; set; }

        public int? MonthlyDay { get; set; }

        public decimal? UpperLimit { get; set; }

        public int Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }
    }

    /// <summary>
    /// 账期类型
    /// </summary>
    public enum PeriodType
    {
        /// <summary>
        /// 预付款
        /// </summary>
        [Description("预付款")]
        PrePaid = 0,

        /// <summary>
        /// 约定期限
        /// </summary>
        [Description("约定期限")]
        AgreedPeriod = 1,

        /// <summary>
        /// 月结
        /// </summary>
        [Description("月结")]
        Monthly = 2,
    }

    /// <summary>
    /// 汇率类型
    /// </summary>
    public enum ExchangeRateType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("无")]
        None = 0,

        /// <summary>
        /// 海关汇率
        /// </summary>
        [Description("海关汇率")]
        Custom = 1,

        /// <summary>
        /// 实时汇率
        /// </summary>
        [Description("实时汇率")]
        RealTime = 2,

        /// <summary>
        /// 约定汇率
        /// </summary>
        [Description("约定汇率")]
        Agreed = 3,
    }

    /// <summary>
    /// 开票类型
    /// </summary>
    public enum Invoice
    {
        /// <summary>
        /// 全额发票
        /// </summary>
        [Description("全额发票")]
        Full = 0,

        /// <summary>
        /// 服务费发票
        /// </summary>
        [Description("服务费发票")]
        Service = 1,
    }

    /// <summary>
    /// 付汇汇率类型
    /// </summary>
    public enum PEIsTen
    {
        /// <summary>
        /// 九点半
        /// </summary>
        [Description("9:30")]
        Nine = 0,

        /// <summary>
        /// 十点
        /// </summary>
        [Description("10:00")]
        Ten = 1,
    }
}
