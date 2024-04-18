using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 代理订单的对账单
    /// </summary>
    public class OrderBillsView2 : UniqueView<Models.OrderBill, ScCustomsReponsitory>
    {
        private string OrderID;
        public OrderBillsView2(string orderID)
        {
            this.OrderID = orderID;
        }

        protected override IQueryable<OrderBill> GetIQueryable()
        {

            var Orders = Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var Clients = Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var Companies = Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var Contacts = Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Contacts>();
            var ClientAgreements = Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>();
            var ClientFeeSettlements = Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientFeeSettlements>();


            var linq = from order in Orders
                       join clientAgr in ClientAgreements on order.ClientAgreementID equals clientAgr.ID
                       join agencyFee in ClientFeeSettlements on
                       new
                       {
                           AgreementID = order.ClientAgreementID,
                           FeeType = (int)Enums.FeeType.AgencyFee,
                           Status = (int)Enums.Status.Normal,
                       }
                       equals new
                       {
                           AgreementID = agencyFee.AgreementID,
                           FeeType = agencyFee.FeeType,
                           Status = agencyFee.Status,
                       }
                       join productFee in ClientFeeSettlements on
                        new
                        {
                            AgreementID = order.ClientAgreementID,
                            FeeType = (int)Enums.FeeType.Product,
                            Status = (int)Enums.Status.Normal,
                        }
                       equals new
                       {
                           AgreementID = productFee.AgreementID,
                           FeeType = productFee.FeeType,
                           Status = productFee.Status,
                       }
                       join taxFee in ClientFeeSettlements on
                       new
                       {
                           AgreementID = order.ClientAgreementID,
                           FeeType = (int)Enums.FeeType.Tax,
                           Status = (int)Enums.Status.Normal,
                       }
                      equals new
                      {
                          AgreementID = taxFee.AgreementID,
                          FeeType = taxFee.FeeType,
                          Status = taxFee.Status,
                      }
                       join IncidentalFee in ClientFeeSettlements on
                       new
                       {
                           AgreementID = order.ClientAgreementID,
                           FeeType = (int)Enums.FeeType.Incidental,
                           Status = (int)Enums.Status.Normal,
                       }
                      equals new
                      {
                          AgreementID = IncidentalFee.AgreementID,
                          FeeType = IncidentalFee.FeeType,
                          Status = IncidentalFee.Status,
                      }
                       join client in Clients on order.ClientID equals client.ID
                       join company in Companies on client.CompanyID equals company.ID
                       join contact in Contacts on company.ContactID equals contact.ID
                       where order.Status == (int)Enums.Status.Normal
                            && order.ID == this.OrderID
                            && order.OrderStatus != (int)Enums.OrderStatus.Canceled
                            && order.OrderStatus != (int)Enums.OrderStatus.Returned
                       select new Models.OrderBill()
                       {
                           ID = order.ID,
                           Client = new Client
                           {
                               Company = new Company
                               {
                                   Name = company.Name,
                                   Contact = new Contact
                                   {
                                       Tel = contact.Tel,
                                   }
                               },
                               ClientType = (Enums.ClientType)client.ClientType,
                           },
                           Agreement = new ClientAgreement
                           {
                               InvoiceTaxRate = clientAgr.InvoiceTaxRate,
                               AgencyRate = clientAgr.AgencyRate,
                               MinAgencyFee = clientAgr.MinAgencyFee,
                               AgencyFeeClause = new ClientFeeSettlement
                               {
                                   ExchangeRateType = (Enums.ExchangeRateType)agencyFee.ExchangeRateType,
                                   ExchangeRateValue = agencyFee.ExchangeRateValue,
                                   FeeType = Enums.FeeType.AgencyFee,
                                   PeriodType = (Enums.PeriodType)agencyFee.PeriodType,
                                   DaysLimit = agencyFee.DaysLimit,
                                   MonthlyDay = agencyFee.MonthlyDay,
                               },
                               ProductFeeClause = new ClientFeeSettlement
                               {
                                   ExchangeRateType = (Enums.ExchangeRateType)productFee.ExchangeRateType,
                                   ExchangeRateValue = productFee.ExchangeRateValue,
                                   FeeType = Enums.FeeType.Product,
                                   PeriodType = (Enums.PeriodType)productFee.PeriodType,
                                   DaysLimit = productFee.DaysLimit,
                                   MonthlyDay = productFee.MonthlyDay,
                               },
                               TaxFeeClause = new ClientFeeSettlement
                               {
                                   ExchangeRateType = (Enums.ExchangeRateType)taxFee.ExchangeRateType,
                                   ExchangeRateValue = taxFee.ExchangeRateValue,
                                   FeeType = Enums.FeeType.Tax,
                                   PeriodType = (Enums.PeriodType)taxFee.PeriodType,
                                   DaysLimit = taxFee.DaysLimit,
                                   MonthlyDay = taxFee.MonthlyDay,
                               },
                               IncidentalFeeClause = new ClientFeeSettlement
                               {
                                   ExchangeRateType = (Enums.ExchangeRateType)IncidentalFee.ExchangeRateType,
                                   ExchangeRateValue = IncidentalFee.ExchangeRateValue,
                                   FeeType = Enums.FeeType.Incidental,
                                   PeriodType = (Enums.PeriodType)IncidentalFee.PeriodType,
                                   DaysLimit = IncidentalFee.DaysLimit,
                                   MonthlyDay = IncidentalFee.MonthlyDay,
                               }
                           },
                           Currency = order.Currency,
                           CustomsExchangeRate = order.CustomsExchangeRate ?? 0,
                           RealExchangeRate = order.RealExchangeRate ?? 0,
                           DeclarePrice = order.DeclarePrice,
                           IsLoan = order.IsLoan,
                           CreateDate = order.CreateDate,
                           OrderType = (Enums.OrderType)order.Type,
                           OrderStatus = (Enums.OrderStatus)order.OrderStatus,
                           Order = new Order
                           {
                               ID = order.ID,
                               OrderBillType = (Enums.OrderBillType)order.OrderBillType,
                               Type = (Enums.OrderType)order.Type
                           }
                       };
            return linq;
        }
    }
}