using Needs.Ccs.Services.ApiSettings;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class FundTransImport
    {
        public List<string> FundTransIDs { get; set; }
        public string fundTransIDs { get; set; }
        public FundTransImportModel DataAcc { get; set; }
        public FundTransImport(List<FundTransReportItem> list)
        {
            string requestID = Needs.Overall.PKeySigner.Pick(PKeyType.XDTBetBanks);
            this.DataAcc = new FundTransImportModel();
            DataAcc.归属模板编号 = MakeAccountSetting.FundTransImport_归属模板编号;
            DataAcc.归属方案编号 = MakeAccountSetting.FundTransImport_归属方案编号;
            DataAcc.归属账套 = MakeAccountSetting.归属账套;
            DataAcc.源文件原始名称 = requestID;
            DataAcc.源文件内容 = new List<Voucher_SFD_XDT_YHHZ>();

            this.FundTransIDs = new List<string>();
            var banks = BankMapService.Current.BankMap.banks;
            foreach (var t in list)
            {
                this.FundTransIDs.Add(t.ID);
                fundTransIDs += t.ID + ",";
                string inBankName = banks[t.InAccountID];
                string outBankName = banks[t.OutAccountID];
                DataAcc.源文件内容.Add(new Voucher_SFD_XDT_YHHZ
                {
                    金额 = t.OutAmount,                 
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
                //报关进口 - 全额开票
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
                            MainID = "银行往来",
                            AdminID = "XDTAdmin",
                            Summary = fundTransIDs + " " + jResult.Json(),
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
                                    }, item => FundTransIDs.Contains(item.ID));

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
                ex.CcsLog("推送银行往来：" + fundTransIDs);
                return false;
            }
        }
    }

    public class FundTransImportModel
    {

        public string 归属模板编号 { get; set; }
        public string 归属方案编号 { get; set; }
        public string 归属账套 { get; set; }
        public string 源文件原始名称 { get; set; }
        public List<Voucher_SFD_XDT_YHHZ> 源文件内容 { get; set; }
    }

    public class Voucher_SFD_XDT_YHHZ
    {
        public decimal 金额 { get; set; }
        public string 借方银行名称 { get; set; }
        public string 贷方银行名称 { get; set; }
        public string 摘要 { get; set; }
        public string 标识 { get; set; }
        public string 日期 { get; set; }
    }

    public class FundTransReportItem
    {
        public string ID { get; set; }
        public decimal OutAmount { get; set; }           
        public string OutAccountID { get; set; }      
        public string InAccountID { get; set; }
        public string Summary
        {
            get
            {
                return "往来";
            }
        }
        public string CreateDate { get; set; }
    }
}
