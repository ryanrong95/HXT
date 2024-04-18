using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class WsOrderTopView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public WsOrderTopView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public WsOrderTopView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        private IQueryable<WsOrderTopViewModel> GetAll(LambdaExpression[] expressions)
        {
            var wsOrderTopView = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.WsOrderTopView>();

            var results = from wsOrder in wsOrderTopView
                          orderby wsOrder.CreateDate descending
                          select new WsOrderTopViewModel
                          {
                              ID = wsOrder.ID,
                              Type = wsOrder.Type,
                              ClientID = wsOrder.ClientID,
                              InvoiceID = wsOrder.InvoiceID,
                              PayeeID = wsOrder.PayeeID,
                              BeneficiaryID = wsOrder.BeneficiaryID,
                              CreateDate = wsOrder.CreateDate,
                              ModifyDate = wsOrder.ModifyDate,
                              Summary = wsOrder.Summary,
                              CreatorID = wsOrder.CreatorID,
                              SupplierID = wsOrder.SupplierID,
                              SettlementCurrency = wsOrder.SettlementCurrency,
                              inBeneficiaryID = wsOrder.inBeneficiaryID,
                              IsPayCharge = wsOrder.IsPayCharge,
                              InWayBillID = wsOrder.InWayBillID,
                              inCurrency = wsOrder.inCurrency,
                              InConditions = wsOrder.InConditions,
                              outBeneficiaryID = wsOrder.outBeneficiaryID,
                              IsReciveCharge = wsOrder.IsReciveCharge,
                              OutWayBillID = wsOrder.OutWayBillID,
                              OutConditions = wsOrder.OutConditions,
                              outCurrency = wsOrder.outCurrency,
                              MainStatus = wsOrder.MainStatus,
                              PaymentStatus = wsOrder.PaymentStatus,
                              InvoiceStatus = wsOrder.InvoiceStatus,
                              RemittanceStatus = wsOrder.RemittanceStatus,
                              ClientName = wsOrder.ClientName,
                              ChineseName = wsOrder.ChineseName,
                              EnglishName = wsOrder.EnglishName,
                          };

            foreach (var expression in expressions)
            {
                results = results.Where(expression as Expression<Func<WsOrderTopViewModel, bool>>);
            }

            return results;
        }

        public List<WsOrderTopViewModel> GetList(out int totalCount, int pageIndex, int pageSize, LambdaExpression[] expressions)
        {
            var alls = GetAll(expressions);

            totalCount = alls.Count();

            var pagedLists = alls.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();

            return pagedLists.ToList();
        }

        public IQueryable<WsOrderTopViewModel> GetAll()
        {
            var wsOrderTopView = this.Reponsitory.GetTable<Layer.Data.Sqls.ScCustoms.WsOrderTopView>();

            var results = from wsOrder in wsOrderTopView
                          select new WsOrderTopViewModel
                          {
                              ID = wsOrder.ID,
                              Type = wsOrder.Type,
                              ClientID = wsOrder.ClientID,
                              InvoiceID = wsOrder.InvoiceID,
                              PayeeID = wsOrder.PayeeID,
                              BeneficiaryID = wsOrder.BeneficiaryID,
                              CreateDate = wsOrder.CreateDate,
                              ModifyDate = wsOrder.ModifyDate,
                              Summary = wsOrder.Summary,
                              CreatorID = wsOrder.CreatorID,
                              SupplierID = wsOrder.SupplierID,
                              SettlementCurrency = wsOrder.SettlementCurrency,
                              inBeneficiaryID = wsOrder.inBeneficiaryID,
                              IsPayCharge = wsOrder.IsPayCharge,
                              InWayBillID = wsOrder.InWayBillID,
                              inCurrency = wsOrder.inCurrency,
                              InConditions = wsOrder.InConditions,
                              outBeneficiaryID = wsOrder.outBeneficiaryID,
                              IsReciveCharge = wsOrder.IsReciveCharge,
                              OutWayBillID = wsOrder.OutWayBillID,
                              OutConditions = wsOrder.OutConditions,
                              outCurrency = wsOrder.outCurrency,
                              MainStatus = wsOrder.MainStatus,
                              PaymentStatus = wsOrder.PaymentStatus,
                              InvoiceStatus = wsOrder.InvoiceStatus,
                              RemittanceStatus = wsOrder.RemittanceStatus,
                              ClientName = wsOrder.ClientName,
                              ChineseName = wsOrder.ChineseName,
                              EnglishName = wsOrder.EnglishName,
                          };

            return results;
        }
    }

    public class WsOrderTopViewModel
    {
        public string ID { get; set; } = string.Empty;
        public int Type { get; set; }
        public string ClientID { get; set; } = string.Empty;
        public string InvoiceID { get; set; } = string.Empty;
        public string PayeeID { get; set; } = string.Empty;
        public string BeneficiaryID { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string CreatorID { get; set; } = string.Empty;
        public string SupplierID { get; set; } = string.Empty;
        public int? SettlementCurrency { get; set; }
        public string inBeneficiaryID { get; set; } = string.Empty;
        public bool? IsPayCharge { get; set; }
        public string InWayBillID { get; set; } = string.Empty;
        public int? inCurrency { get; set; }
        public string InConditions { get; set; } = string.Empty;
        public string outBeneficiaryID { get; set; } = string.Empty;
        public bool? IsReciveCharge { get; set; }
        public string OutWayBillID { get; set; } = string.Empty;
        public string OutConditions { get; set; } = string.Empty;
        public int? outCurrency { get; set; }
        public int MainStatus { get; set; }
        public int PaymentStatus { get; set; }
        public int InvoiceStatus { get; set; }
        public int RemittanceStatus { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ChineseName { get; set; } = string.Empty;

        public string EnglishName { get; set; } = string.Empty;
    }

}
