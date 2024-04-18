using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ModelDetailView : UniqueView<Models.ModelDetail, ScCustomsReponsitory>
    {
        private string CustomsUnionID;

        public ModelDetailView()
        { }

        public ModelDetailView(string CustomsUnionID)
        {
            this.CustomsUnionID = CustomsUnionID;
        }

        internal ModelDetailView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ModelDetail> GetIQueryable()
        {
            throw new NotImplementedException();
        }

        public IQueryable<ModelDetail> GetModelDetailPre()
        {
            var orderitemView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Where(t=>t.ProductUniqueCode == CustomsUnionID);

            var apiNoticesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ApiNotices>().Where(t => t.PushType == (int)Enums.PushType.DutiablePrice);

            var orderitemDetail = from orderitem in orderitemView
                                  join category in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>() on orderitem.ID equals category.OrderItemID
                                  join rate in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemRateView>() on orderitem.ID equals rate.OrderItemID
                                  join declist in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>() on orderitem.ID equals declist.OrderItemID
                                  join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on orderitem.OrderID equals order.ID
                                  //从订单中获取真实的客户名称，不使用报关单中的
                                  join client in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>() on order.ClientID equals client.ID
                                  join company in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>() on client.CompanyID equals company.ID
                                  join dechead in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>() on orderitem.OrderID equals dechead.OrderID
                                  join decTariff in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>().Where(t => t.TaxType == (int)Enums.DecTaxType.Tariff) on dechead.ID equals decTariff.DecTaxID into taxTraffMap
                                  from decTariff in taxTraffMap.DefaultIfEmpty()
                                  join decAddTax in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTaxFlows>().Where(t => t.TaxType == (int)Enums.DecTaxType.AddedValueTax) on dechead.ID equals decAddTax.DecTaxID into taxaddtaxMap
                                  from decAddTax in taxaddtaxMap.DefaultIfEmpty()
                                  join ordermap in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>() on orderitem.OrderID equals ordermap.OrderID into map
                                  from ordermap in map.DefaultIfEmpty()
                                  join apiNotice in apiNoticesView on dechead.ID equals apiNotice.ItemID into dec_apiNotice
                                  from apiNotice in dec_apiNotice.DefaultIfEmpty()
                                  where orderitem.ProductUniqueCode == this.CustomsUnionID && dechead.CusDecStatus != "04" && dechead.IsSuccess
                                  select new ModelDetail
                                  {
                                      ID = orderitem.ID,
                                      CustomsUnionID = CustomsUnionID,
                                      Model = declist.GoodsModel,
                                      ProductName = declist.GName,
                                      Manufacturer = declist.GoodsBrand,
                                      HSCode = declist.CodeTS,
                                      Elements = declist.GModel,
                                      Origin = declist.OriginCountry,
                                      Qty = declist.GQty,
                                      AgentUnitPrice = orderitem.UnitPrice,
                                      AgentTotalPrice = orderitem.TotalPrice,
                                      DeclareUnitPrice = declist.DeclPrice,
                                      DeclareTotalPrice = declist.DeclTotal,
                                      Currency = order.Currency,
                                      CustomsExchangeRate = order.CustomsExchangeRate.Value,
                                      TariffRate = rate.ImportTaxRate.Value,
                                      RealTariffRate = rate.ImportTaxReceiptRate.Value,
                                      AddTaxRate = rate.AddedValueTaxRate.Value,
                                      RealAddTaxRate = rate.AddedValueTaxReceiptRate.Value,
                                      TaxName = category.TaxName,
                                      TaxCode = category.TaxCode,
                                      OrderID = orderitem.OrderID,
                                      DeclareDate = dechead.DDate.Value,
                                      ContractNo = dechead.ContrNo,
                                      EntryID = dechead.EntryId,
                                      CustomsOrderID = ordermap.IcgooOrder,
                                      IsValid = apiNotice == null ? 0 : 1,
                                      DecHeadType = dechead.Type,
                                      ClientName = company.Name,
                                      CusTariffNumber = decTariff == null ? "" : decTariff.TaxNumber,
                                      CusTariffAmount = decTariff == null ? 0M : decTariff.Amount,
                                      CusAddTaxNumber = decAddTax == null ? "" : decAddTax.TaxNumber,
                                      CusAddTaxAmount = decAddTax == null ? 0M : decAddTax.Amount,

                                  };
            return orderitemDetail;
        }

        public IQueryable<ProfitInvoiceInfo> GetInvoiceInfo()
        {
            var result  = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeItems>()
                                    join invoice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNotices>() on item.InvoiceNoticeID equals invoice.ID
                                    select new ProfitInvoiceInfo
                                    {
                                        InvoiceType = (Enums.InvoiceType)invoice.InvoiceType,
                                        InvoiceTaxRate = invoice.InvoiceTaxRate,
                                        InvoiceDate = item.InvoiceDate.HasValue? item.InvoiceDate.Value : DateTime.Parse("2000-01-01T00:00:00"),
                                        InvoiceNo = item.InvoiceNo,
                                        OrderID = item.OrderID
                                    };
            return result;
        }

        public void CalculateTax(ModelDetail detail)
        {
            var orderOrigin = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Where(t => t.ID == detail.OrderID).FirstOrDefault();
            var agreement = new ClientAgreementsView(this.Reponsitory).Where(item => item.ID == orderOrigin.ClientAgreementID).SingleOrDefault();
            //确定单双抬头
            var invoiceType = agreement.InvoiceType;
            //订单项信息
            var orderitemList = new Needs.Ccs.Services.Views.OrderItemsView(this.Reponsitory).Where(t => t.OrderID == orderOrigin.ID).ToArray();
            var count = orderitemList.Count();//条数
             //杂费合计
            var MiscFeesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderPremiums>().Where(item =>
                        item.OrderID == orderOrigin.ID
                        && item.Type != (int)OrderPremiumType.AgencyFee
                        && item.Type != (int)OrderPremiumType.InspectionFee
                        && item.Status == (int)Status.Normal).ToArray();
            var MiscFees = MiscFeesView.Length > 0 ? MiscFeesView.Sum(item => item.Count * item.UnitPrice * item.Rate) : 0M;
            //平摊杂费（不含商检费）
            var miscFee = MiscFees * (1 + agreement.InvoiceTaxRate);
            var aveMiscFee = (miscFee / count);

            var orderitem = orderitemList.FirstOrDefault(t => t.ID == detail.ID);
            //商检费
            decimal InspectionFee = orderitem.InspectionFee.GetValueOrDefault() * (1 + agreement.InvoiceTaxRate);
            //代理费
            var aveAgencyFee = orderitem.TotalPrice * orderOrigin.RealExchangeRate.Value * agreement.AgencyRate * (1 + agreement.InvoiceTaxRate);

            //计算报关价格
            //1、双抬头：完税价格
            if (invoiceType == InvoiceType.Service)
            {
                detail.NoTaxAmount = (detail.DeclareTotalPrice * orderOrigin.CustomsExchangeRate.Value).ToRound(2) + ((detail.DeclareTotalPrice * orderOrigin.CustomsExchangeRate.Value).ToRound(2) * detail.TariffRate).ToRound(2);
                //服务费
                detail.ServicePrice = ((aveAgencyFee + InspectionFee + aveMiscFee) / (1 + agreement.InvoiceTaxRate)).ToRound(2);
            }
            else
            {
                //2、单抬头：全额开票中的不含税价格

                //开票时，使用实收汇率计算税费 ryan  20211122
                var topPrice = (orderitem.TotalPrice * ConstConfig.TransPremiumInsurance).ToRound(2);
                var total = (topPrice * orderOrigin.CustomsExchangeRate.Value).ToRound(2);
                //关税
                decimal ImportTax = (total * orderitem.ImportTax.Rate).ToRound(2);
                decimal ImportReal = (total * orderitem.ImportTax.ReceiptRate).ToRound(2);
                //消费税
                decimal ExciseTax = ((total + ImportTax) / (1 - orderitem.ExciseTax.ReceiptRate) * orderitem.ExciseTax.ReceiptRate).ToRound(2);
                var exciseTaxRate = orderitem.ExciseTax.ReceiptRate;
                decimal AddedValueTax = (((total + ImportTax) + (total + ImportTax) / (1 - exciseTaxRate) * exciseTaxRate) * orderitem.AddedValueTax.ReceiptRate).ToRound(2);

                //价税合计
                var TaxedDutiablePrice = (orderitem.TotalPrice * orderOrigin.RealExchangeRate.Value + ImportReal + ExciseTax + AddedValueTax + aveAgencyFee + InspectionFee + aveMiscFee).ToRound(4);

                //价
                detail.NoTaxAmount = (TaxedDutiablePrice / (1 + agreement.InvoiceTaxRate)).ToRound(4);
                //服务费
                detail.ServicePrice = ((aveAgencyFee + InspectionFee + aveMiscFee) / (1 + agreement.InvoiceTaxRate)).ToRound(2);
            }

            //开票类型
            detail.InvoiceType = invoiceType.GetDescription();
            detail.InvoiceTaxRate = agreement.InvoiceTaxRate;

        }

    }
}
