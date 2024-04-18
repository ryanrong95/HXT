using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 计算一个订单的逾期时间
    /// </summary>
    public class CalcDueDate
    {
        /// <summary>
        /// 小订单号
        /// </summary>
        private string TinyOrderID { get; set; }

        public CalcDueDate(string tinyOrderID)
        {
            this.TinyOrderID = tinyOrderID;
        }

        public void Execute()
        {
            Task.Run(() =>
            {
                try
                {
                    using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        var order = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Where(t => t.ID == this.TinyOrderID)
                            .Select(item => new
                            {
                                TinyOrderID = item.ID,
                                ClientAgreementID = item.ClientAgreementID,
                            }).FirstOrDefault();

                        var clientFeeSettlements = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientFeeSettlements>()
                            .Where(t => t.AgreementID == order.ClientAgreementID
                                     && t.Status == (int)Enums.Status.Normal)
                            .Select(item => new Models.ClientFeeSettlement
                            {
                                ID = item.ID,
                                FeeType = (Enums.FeeType)item.FeeType,
                                PeriodType = (Enums.PeriodType)item.PeriodType,
                                CreateDate = item.CreateDate,
                                DaysLimit = item.DaysLimit,
                                MonthlyDay = item.MonthlyDay,
                            }).ToList();

                        DateTime ProductFeeDueDate = GetFeeDueDate(Enums.FeeType.Product, clientFeeSettlements);
                        DateTime TaxFeeDueDate = GetFeeDueDate(Enums.FeeType.Tax, clientFeeSettlements);
                        DateTime AgencyFeeDueDate = GetFeeDueDate(Enums.FeeType.AgencyFee, clientFeeSettlements);
                        DateTime IncidentalFeeDueDate = GetFeeDueDate(Enums.FeeType.Incidental, clientFeeSettlements);

                        DateTime dueDate = GetDueDate(ProductFeeDueDate, TaxFeeDueDate, AgencyFeeDueDate, IncidentalFeeDueDate);

                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                        {
                            DueDate = dueDate,
                        }, item => item.ID == this.TinyOrderID);
                    }
                }
                catch (Exception ex)
                {
                    ex.CcsLog("CalcDueDate|" + this.TinyOrderID + "|");
                }
            });

        }

        private DateTime GetFeeDueDate(Enums.FeeType feeType, List<Models.ClientFeeSettlement> clientFeeSettlements)
        {
            Models.ClientFeeSettlement clientFeeSettlement = clientFeeSettlements.Where(t => t.FeeType == feeType).OrderByDescending(t => t.CreateDate).FirstOrDefault();

            DateTime productFeeDueDate = DateTime.Now;

            var periodType = clientFeeSettlement.PeriodType;
            switch (periodType)
            {
                case Enums.PeriodType.PrePaid:
                    productFeeDueDate = DateTime.Now;
                    break;
                case Enums.PeriodType.AgreedPeriod:
                    var daysLimit = clientFeeSettlement.DaysLimit.Value;
                    productFeeDueDate = DateTime.Now.AddDays(daysLimit);
                    break;
                case Enums.PeriodType.Monthly:
                    var monthlyDay = clientFeeSettlement.MonthlyDay.Value;
                    productFeeDueDate = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, monthlyDay);
                    break;
                default:
                    productFeeDueDate = DateTime.Now;
                    break;
            }

            return productFeeDueDate;
        }

        /// <summary>
        /// 对账单应付款结算日期
        /// </summary>
        /// <returns></returns>
        public DateTime GetDueDate(DateTime ProductFeeDueDate, DateTime TaxFeeDueDate, DateTime AgencyFeeDueDate, DateTime IncidentalFeeDueDate)
        {
            var dueDate = ProductFeeDueDate > TaxFeeDueDate ? TaxFeeDueDate : ProductFeeDueDate;
            dueDate = dueDate > AgencyFeeDueDate ? AgencyFeeDueDate : dueDate;
            dueDate = dueDate > IncidentalFeeDueDate ? IncidentalFeeDueDate : dueDate;

            return dueDate;
        }

    }
}
