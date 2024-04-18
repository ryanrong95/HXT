using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class UnmackOneReceiptToYahv
    {
        private Admin Admin { get; set; }

        private string OrderID { get; set; }

        private int FeeTypeInt { get; set; }

        public UnmackOneReceiptToYahv(Admin admin, string orderID, int feeTypeInt)
        {
            this.Admin = admin;
            this.OrderID = orderID;
            this.FeeTypeInt = feeTypeInt;
        }

        public void Execute()
        {
            try
            {
                string catalog = string.Empty;
                string subject = string.Empty;

                var orderFeeType = (Needs.Ccs.Services.Enums.OrderFeeType)this.FeeTypeInt;

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
                        subject = null;
                        break;
                    default:
                        break;
                }


                new Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = Guid.NewGuid().ToString("N"),
                    OrderID = null,
                    TinyOrderID = this.OrderID,
                    RequestContent = "catalog = " + catalog + ", subject = " + subject + ", currency = " + Yahv.Underly.Currency.CNY,
                    Status = Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "取消单个收款调用Yahv的dll",
                }.Enter();

                Yahv.Payments.PaymentManager.Erp(this.Admin.ErmAdminID).Received.Abandon(this.OrderID, catalog, subject, Yahv.Underly.Currency.CNY);


            }
            catch (Exception ex)
            {
                ex.CcsLog("取消单个收款到Yahv发生异常(UnmackOneReceiptToYahv)|" + this.OrderID + "|" + this.FeeTypeInt + "|" + ex.Message);
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


    }
}
