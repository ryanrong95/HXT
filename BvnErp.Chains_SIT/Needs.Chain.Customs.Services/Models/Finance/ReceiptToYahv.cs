using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Utils.Converters.Contents;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 收款到Yahv
    /// </summary>
    public class ReceiptToYahv
    {
        /// <summary>
        /// 小订单ID
        /// </summary>
        private string TinyOrderID { get; set; }

        /// <summary>
        /// Admin
        /// </summary>
        private Admin Admin { get; set; }

        /// <summary>
        /// FinanceReceiptID
        /// </summary>
        private string FinanceReceiptID { get; set; }

        /// <summary>
        /// 本次提交的所有费用
        /// </summary>
        private ReceiptToYahvAmountModel[] Fees { get; set; }

        public ReceiptToYahv(string tinyOrderID, Admin admin, string financeReceiptID, ReceiptToYahvAmountModel[] fees)
        {
            this.TinyOrderID = tinyOrderID;
            this.Admin = admin;
            this.FinanceReceiptID = financeReceiptID;
            this.Fees = fees;
        }

        public ReceiptToYahv(string financeReceiptID, Admin admin)
        {
            this.FinanceReceiptID = financeReceiptID;
            this.Admin = admin;
        }

        public void Execute()
        {
            try
            {
                string payerName = string.Empty;

                //查 payerName
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var orders = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
                    var clients = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
                    var companies = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();

                    var info = (from order in orders
                                join client in clients on order.ClientID equals client.ID
                                join company in companies on client.CompanyID equals company.ID
                                where order.ID == this.TinyOrderID
                                   && order.Status == (int)Enums.Status.Normal
                                   && client.Status == (int)Enums.Status.Normal
                                   && company.Status == (int)Enums.Status.Normal
                                select new { order, company }).FirstOrDefault();

                    if (info != null)
                    {
                        payerName = info.company.Name;
                    }
                }


                List<Yahv.Payments.Models.XdtFee> xdtFees = new List<Yahv.Payments.Models.XdtFee>();

                if (this.Fees != null && this.Fees.Any())
                {
                    foreach (var fee in this.Fees)
                    {
                        string catalog = string.Empty;
                        string subject = string.Empty;

                        var orderFeeType = (Needs.Ccs.Services.Enums.OrderFeeType)fee.Type;

                        switch (orderFeeType)
                        {
                            case Enums.OrderFeeType.Product:
                                catalog = Yahv.Payments.CatalogConsts.货款;
                                subject = null;
                                break;
                            case Enums.OrderFeeType.Tariff:
                                catalog = Yahv.Payments.CatalogConsts.税款;
                                subject = Yahv.Payments.SubjectConsts.关税;
                                break;
                            case Enums.OrderFeeType.AddedValueTax:
                                catalog = Yahv.Payments.CatalogConsts.税款;
                                subject = Yahv.Payments.SubjectConsts.销售增值税;
                                break;
                            case Enums.OrderFeeType.AgencyFee:
                                catalog = Yahv.Payments.CatalogConsts.代理费;
                                subject = Yahv.Payments.SubjectConsts.代理费;
                                break;
                            case Enums.OrderFeeType.Incidental:
                                catalog = Yahv.Payments.CatalogConsts.杂费;
                                subject = GetSubjectByFeeSourceID(fee.FeeSourceID);
                                break;
                            default:
                                break;
                        }

                        xdtFees.Add(new Yahv.Payments.Models.XdtFee(
                            catalog,
                            subject,
                            Yahv.Underly.Currency.CNY,
                            fee.Amount));
                    }
                }

                //查出 流水号, 银行, 银行卡号
                var backData = new Views.ReceiptToYahvView().GetData(this.FinanceReceiptID);

                new Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = Guid.NewGuid().ToString("N"),
                    OrderID = null,
                    TinyOrderID = this.TinyOrderID,
                    RequestContent = xdtFees.Json(),
                    Status = Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "实收调用Yahv的dll",
                }.Enter();

                Yahv.Payments.PaymentManager.Erp(this.Admin.ErmAdminID).Received.For(this.TinyOrderID) //小订单ID
                    .Confirm_XinDaTong(new Yahv.Payments.Models.Rolls.VoucherInput()
                    {
                        Type = Yahv.Underly.VoucherType.Receipt, //收款类型
                        Business = Yahv.Payments.ConductConsts.代报关, //业务
                        CreatorID = this.Admin.ErmAdminID,
                        CreateDate = DateTime.Now,

                        Payer = payerName,
                        Payee = PurchaserContext.Current.CompanyName,
                        FormCode = backData.SeqNo,  //流水号
                        Bank = backData.BankName,  //银行
                        Account = backData.BankAccount,  //银行卡号

                        Currency = Yahv.Underly.Currency.CNY,
                    }, xdtFees.ToArray());

            }
            catch (Exception ex)
            {
                ex.CcsLog("收款到Yahv发生异常(ReceiptToYahv)|" + this.TinyOrderID + "|" + ex.Message);
            }

        }

        private string GetSubjectByFeeSourceID(string feeSourceID)
        {
            //商检费
            if (feeSourceID == null)
            {
                return Yahv.Payments.SubjectConsts.商检费;
            }

            return Yahv.Payments.SubjectConsts.其他;

            ////其他杂费
            //using (var view = new Views.OrderPremiumsView())
            //{
            //    var premium = view.Where(item => item.ID == feeSourceID).FirstOrDefault();

            //    string subject = string.Empty;

            //    switch (premium.Type)
            //    {
            //        case Enums.OrderPremiumType.AgencyFee:
            //            subject = Yahv.Payments.SubjectConsts.代理费;
            //            break;
            //        case Enums.OrderPremiumType.InspectionFee:
            //            subject = Yahv.Payments.SubjectConsts.商检费;
            //            break;
            //        case Enums.OrderPremiumType.DeliveryFee:
            //            subject = Yahv.Payments.SubjectConsts.送货费;
            //            break;
            //        case Enums.OrderPremiumType.ExpressFee:
            //            subject = Yahv.Payments.SubjectConsts.快递费;
            //            break;
            //        case Enums.OrderPremiumType.CustomClearanceFee:
            //            subject = Yahv.Payments.SubjectConsts.清关费;
            //            break;
            //        case Enums.OrderPremiumType.PickUpFee:
            //            subject = Yahv.Payments.SubjectConsts.提货费;
            //            break;
            //        case Enums.OrderPremiumType.ParkingFee:
            //            subject = Yahv.Payments.SubjectConsts.停车费;
            //            break;
            //        case Enums.OrderPremiumType.EntryFee:
            //            subject = Yahv.Payments.SubjectConsts.入仓费;
            //            break;
            //        case Enums.OrderPremiumType.StorageFee:
            //            subject = Yahv.Payments.SubjectConsts.仓储费;
            //            break;
            //        case Enums.OrderPremiumType.UnNormalFee:
            //            subject = Yahv.Payments.SubjectConsts.收货异常费用;
            //            break;
            //        case Enums.OrderPremiumType.OtherFee:
            //            subject = Yahv.Payments.SubjectConsts.其他;
            //            break;
            //        default:
            //            subject = Yahv.Payments.SubjectConsts.其他;
            //            break;
            //    }

            //    return subject;
            //}
        }

        /// <summary>
        /// 取消收款
        /// 用了参数 FinanceReceiptID
        /// 传给 财务 FinanceReceipt 表中的 流水号SeqNo
        /// </summary>
        public void Unmack()
        {
            try
            {
                string seqNo = string.Empty;

                var theFinanceReceipt = new Views.Origins.FinanceReceiptsOrigin()[this.FinanceReceiptID];
                if (theFinanceReceipt != null)
                {
                    seqNo = theFinanceReceipt.SeqNo;
                }

                var ermAdmin = new Needs.Ccs.Services.Views.AdminsTopView2().Where(t => t.OriginID == this.Admin.ID).FirstOrDefault();

                //取消实收
                Yahv.Payments.PaymentManager.Erp(ermAdmin.ID).Received.Abandon(seqNo);
            }
            catch (Exception ex)
            {
                ex.CcsLog("取消收款到Yahv发生异常(ReceiptToYahv.Unmack)");
            }
        }
    }

    public class ReceiptToYahvAmountModel
    {
        public string OrderID { get; set; }

        public decimal Amount { get; set; }

        public int Type { get; set; }

        public string FeeSourceID { get; set; }
    }
}
