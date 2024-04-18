using Layers.Data.Sqls;
using Layers.Data.Sqls.ScCustoms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.XDTModels;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    /// <summary>
    /// 客户应付款统计视图
    /// </summary>
    public class ClientAccountPayablesView : UniqueView<ClientAccountPayable, ScCustomReponsitory>
    {
        //IUser User;

        public ClientAccountPayablesView()
        {

        }

        //public ClientAccountPayablesView(IUser user)
        //{
        //    this.User = user;
        //}

        protected override IQueryable<ClientAccountPayable> GetIQueryable()
        {
            var companies = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Companies>();
            var clients = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Clients>();
            var orderReceipts = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.OrderReceipts>();
            var orders = this.Reponsitory.ReadTable<Layers.Data.Sqls.ScCustoms.Orders>();

            // 货款应付款
            var productPayableOrderReceipts = from orderReceipt in orderReceipts
                                              join order in orders on orderReceipt.OrderID equals order.ID
                                              where orderReceipt.Status == (int)GeneralStatus.Normal
                                                    && orderReceipt.FeeType == (int)OrderFeeType.Product && order.IsLoan
                                              select orderReceipt;

            // 税款应付款
            var taxPayableOrderReceipts = from orderReceipt in orderReceipts
                                          where orderReceipt.Status == (int)GeneralStatus.Normal
                                                    && (orderReceipt.FeeType == (int)OrderFeeType.Tariff
                                                     || orderReceipt.FeeType == (int)OrderFeeType.AddedValueTax
                                                     || orderReceipt.FeeType == (int)OrderFeeType.ExciseTax)
                                          select orderReceipt;

            // 代理费应付款
            var agencyPayableOrderReceipts = from orderReceipt in orderReceipts
                                             where orderReceipt.Status == (int)GeneralStatus.Normal
                                                    && orderReceipt.FeeType == (int)OrderFeeType.AgencyFee
                                             select orderReceipt;
            // 杂费应付款
            var incidentalPayableOrderReceipts = from orderReceipt in orderReceipts
                                                 where orderReceipt.Status == (int)GeneralStatus.Normal
                                                        && orderReceipt.FeeType == (int)OrderFeeType.Incidental
                                                 select orderReceipt;


            var linq = from company in companies
                       join client in clients on company.ID equals client.CompanyID
                       join productPayable in productPayableOrderReceipts on client.ID equals productPayable.ClientID into productPayables
                       join taxPayable in taxPayableOrderReceipts on client.ID equals taxPayable.ClientID into taxPayables
                       join agencyPayable in agencyPayableOrderReceipts on client.ID equals agencyPayable.ClientID into agencyPayables
                       join incidentalPayable in incidentalPayableOrderReceipts on client.ID equals incidentalPayable.ClientID into incidentalPayables
                       select new ClientAccountPayable
                       {
                           ID = company.ID,
                           ProductPayable = productPayables.Sum(t => (decimal?)t.Amount).GetValueOrDefault() < 0 ?
                                            0 : productPayables.Sum(t => (decimal?)t.Amount).GetValueOrDefault(),
                           TaxPayable = taxPayables.Sum(t => (decimal?)t.Amount).GetValueOrDefault(),
                           AgencyPayable = agencyPayables.Sum(t => (decimal?)t.Amount).GetValueOrDefault(),
                           IncidentalPayable = incidentalPayables.Sum(t => (decimal?)t.Amount).GetValueOrDefault(),
                       };

            return linq;
        }
    }
}
