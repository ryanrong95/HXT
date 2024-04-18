using Needs.Ccs.Services.Views;
using Needs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class CenterFee
    {
        /// <summary>
        /// 收款方账号
        /// </summary>
        public string ReceiveAccountNo { get; set; }

        /// <summary>
        /// 付款账户
        /// </summary>
        public string AccountNo { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string SeqNo { get; set; }
        /// <summary>
        /// 老流水号
        /// </summary>
        public string OldSeqNo { get; set; }
        /// <summary>
        /// 总额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// 币制
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// 付款日期
        /// </summary>
        public DateTime PaymentDate { get; set; }
        /// <summary>
        /// 付款方式
        /// </summary>
        public int PaymentType { get; set; }
        /// <summary>
        /// 1-费用申请，2-银行自动扣除
        /// </summary>
        public int MoneyType { get; set; }
        /// <summary>
        /// 费用项目
        /// </summary>
        public List<CenterFeeItem> FeeItems { get; set; }
        /// <summary>
        /// 费用附件
        /// </summary>
        public List<CenterFeeFile> Files { get; set; }

        public CenterFee() { }

        public CenterFee(PaymentNotice paymentNotice,string costapplyID)
        {
            if (paymentNotice != null)
            {
                this.ReceiveAccountNo = paymentNotice.BankAccount;
                this.AccountNo = paymentNotice.FinanceAccount.BankAccount;
                //this.CreatorID = paymentNotice.Admin.ErmAdminID;
                this.SeqNo = paymentNotice.SeqNo;
                this.Amount = paymentNotice.Amount;
                this.Currency = paymentNotice.Currency;
                this.Rate = paymentNotice.ExchangeRate==null?1:paymentNotice.ExchangeRate.Value;
                this.PaymentDate = paymentNotice.PayDate;

                int paymentType = PaymentTypeTransfer.Current.L2CTransfer(paymentNotice.PayType);
                this.PaymentType = paymentType;

                var ErmAdminID = new AdminsTopView2().FirstOrDefault(t => t.OriginID == paymentNotice.Admin.ID)?.ErmAdminID;
                this.CreatorID = ErmAdminID;

                var costApplyDetail = new Needs.Ccs.Services.Views.CostApplyDetailView().GetResult(costapplyID);
                this.MoneyType = (int)costApplyDetail.MoneyType;

                var view = new Needs.Ccs.Services.Views.CostApplyItemsView().Where(t => t.CostApplyID == costapplyID).ToArray();

                this.FeeItems = new List<CenterFeeItem>();

                foreach (var t in view)
                {
                    string ceterFeetype = FeeTypeTransfer.Current.L2COutTransfer(t.FeeType);

                    CenterFeeItem feeItem = new CenterFeeItem();
                    feeItem.FeeType = ceterFeetype;
                    feeItem.Amount = t.Amount;
                    feeItem.FeeDesc = t.FeeDesc;

                    this.FeeItems.Add(feeItem);
                }

                this.Files = new List<CenterFeeFile>();
                var files = new Needs.Ccs.Services.Views.CostApplyFilesView().GetResults(costapplyID);
                foreach(var t in files)
                {
                    CenterFeeFile centerFile = new CenterFeeFile();
                    centerFile.FileName = t.FileName;
                    centerFile.FileFormat = t.FileFormat;
                    centerFile.FileType = (int)CenterFeeFileType.FeeFile;
                    centerFile.Url = FileDirectory.Current.FileServerUrl + "/" + t.Url.Replace(@"\",@"/");
                    this.Files.Add(centerFile);
                }

                var paymentFiles = new Needs.Ccs.Services.Views.PaymentNoticeFileView().Where(t => t.PaymentNoticeID == paymentNotice.ID).ToArray();
                foreach (var t in paymentFiles)
                {
                    CenterFeeFile centerFile = new CenterFeeFile();
                    centerFile.FileName = t.FileName;
                    centerFile.FileFormat = t.FileFormat;
                    centerFile.FileType = (int)CenterFeeFileType.PayFile;
                    centerFile.Url = FileDirectory.Current.FileServerUrl + "/" + t.Url.Replace(@"\", @"/");
                    this.Files.Add(centerFile);
                }
            }
        }
    }
}
