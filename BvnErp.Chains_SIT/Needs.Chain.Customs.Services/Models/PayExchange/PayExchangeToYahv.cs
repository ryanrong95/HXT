using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 付汇应收传 Yahv
    /// </summary>
    public class PayExchangeToYahvReceivable
    {
        public Admin Admin { get; set; }

        private string PayExchangeApplyID { get; set; } = string.Empty;

        public PayExchangeToYahvReceivable(string payExchangeApplyID, Admin admin)
        {
            this.PayExchangeApplyID = payExchangeApplyID;
            this.Admin = admin;
        }

        public void Execute()
        {
            try
            {
                var datas = new Views.PayExchangeToYahvReceivableView().GetData(this.PayExchangeApplyID);

                if (datas != null && datas.Any())
                {
                    for (int i = 0; i < datas.Count; i++)
                    {
                        //Yahv.Payments.PaymentManager.Erp(this.Admin.ErmAdminID)[datas[i].PayerName, PurchaserContext.Current.CompanyName][Yahv.Payments.ConductConsts.代报关]
                        //        .Receivable[Yahv.Payments.CatalogConsts.货款, Yahv.Payments.SubjectConsts.代付货款].Record_XinDaTong(
                        //            Yahv.Underly.Currency.CNY,
                        //            (datas[i].Amount * datas[i].ExchangeRate).ToRound(2),
                        //            orderID: datas[i].MainOrderId, 
                        //            tinyID: datas[i].TinyOrderID, 
                        //            itemID: null, 
                        //            applicationID: datas[i].PayExchangeApplyID);


                        List<Yahv.Payments.Models.XdtFee> listXdtFee = new List<Yahv.Payments.Models.XdtFee>();
                        listXdtFee.Add(new Yahv.Payments.Models.XdtFee(
                                    Yahv.Payments.CatalogConsts.货款, null, Yahv.Underly.Currency.CNY, 
                                    (datas[i].Amount * datas[i].ExchangeRate).ToRound(2)));

                        Yahv.Payments.PaymentManager.Erp(this.Admin.ID)[datas[i].PayerName, PurchaserContext.Current.CompanyName][Yahv.Payments.ConductConsts.代报关]
                            .Receivable.XdtRecord(
                                    vastOrderID: datas[i].MainOrderId,
                                    tinyOrderID: datas[i].TinyOrderID,
                                    rate: 1, //datas[i].ExchangeRate,
                                    itemID: null,
                                    applicationID: datas[i].PayExchangeApplyID,
                                    array: listXdtFee.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                ex.CcsLog("付汇应收传Yahv(PayExchangeToYahvReceivable)|" + this.PayExchangeApplyID);
            }
        }
    }
}
