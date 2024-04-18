using Needs.Ccs.Services.ApiSettings;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class QRImport
    {
        public List<string> QRIDs { get; set; }
        public string qrIDs { get; set; }
        public QRImportModel DataAcc { get; set; }

        public QRImport(List<QRReportItem> list)
        {
            string requestID = Needs.Overall.PKeySigner.Pick(PKeyType.XDTRecPre);
            this.DataAcc = new QRImportModel();
            DataAcc.归属模板编号 = MakeAccountSetting.QRImport_归属模板编号;
            DataAcc.归属方案编号 = MakeAccountSetting.QRImport_归属方案编号;
            DataAcc.归属账套 = MakeAccountSetting.归属账套;
            DataAcc.源文件原始名称 = requestID;
            DataAcc.源文件内容 = new List<Voucher_SFD_XDT_SMTX>();

            this.QRIDs = new List<string>();
            var banks = BankMapService.Current.BankMap.banks;
            foreach (var t in list)
            {
                this.QRIDs.Add(t.ID);
                qrIDs += t.ID + ",";
                string inBankName = banks[t.InAccountID];
                string outBankName = banks[t.OutAccountID];
                DataAcc.源文件内容.Add(new Voucher_SFD_XDT_SMTX
                {
                    金额 = t.OutAmount,
                    银行手续费 = t.QRCodeFee==null?0:t.QRCodeFee.Value,
                    借方银行名称 = inBankName,
                    贷方银行名称 = outBankName,
                    摘要 = t.Summary,
                    标识 = t.ID,
                    日期 = t.CreateDate,
                });
            }
        }

        public bool Make()
        {
            try
            {

                var flag = true;

                var mk = new MakeAccountHandler();

                string requestid = DataAcc.源文件原始名称;
                //报关进口-全额开票
                if (DataAcc.源文件内容.Count() > 0)
                {
                    var jResult = mk.PostToK3(this.DataAcc);
                    using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        //记录日志
                        responsitory.Insert(new Layer.Data.Sqls.ScCustoms.Logs
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            Name = "生成凭证",
                            MainID = "扫码提现",
                            AdminID = "XDTAdmin",
                            Summary = qrIDs + " " + jResult.Json(),
                            Json = DataAcc.Json(),
                            CreateDate = DateTime.Now,
                        });

                        ////成功，更新apiNotice
                        if (jResult.success && jResult.data)
                        {
                            responsitory.Update<Layer.Data.Sqls.ScCustoms.FundTransferApplies>(
                                    new
                                    {
                                        FundTranCreSta = true,
                                        RequestID = requestid
                                    }, item => QRIDs.Contains(item.ID));

                            flag &= true;
                        }
                        else
                        {
                            flag &= false;
                        }
                    }
                }

                return flag;
            }
            catch (Exception ex)
            {
                ex.CcsLog("推送扫描提现：" + qrIDs);
                return false;
            }
        }
    }

    public class QRImportModel
    {

        public string 归属模板编号 { get; set; }
        public string 归属方案编号 { get; set; }
        public string 归属账套 { get; set; }
        public string 源文件原始名称 { get; set; }
        public List<Voucher_SFD_XDT_SMTX> 源文件内容 { get; set; }

    }

    public class Voucher_SFD_XDT_SMTX
    {
        public decimal 金额 { get; set; }
        public decimal 银行手续费 { get; set; }
        public string 借方银行名称 { get; set; }
        public string 贷方银行名称 { get; set; }
        public string 摘要 { get; set; }
        public string 标识 { get; set; }
        public string 日期 { get; set; }
    }

    public class QRReportItem
    {
        public string ID { get; set; }
        public decimal OutAmount { get; set; }
        public decimal? QRCodeFee { get; set; }
        public string OutAccountName { get; set; }
        public string OutAccountID { get; set; }
        public string InAccountName { get; set; }
        public string InAccountID { get; set; }
        public string Summary
        {
            get
            {
                return "扫码支付收单资金汇总清算";
            }
        }
        public string CreateDate { get; set; }
    }
}
