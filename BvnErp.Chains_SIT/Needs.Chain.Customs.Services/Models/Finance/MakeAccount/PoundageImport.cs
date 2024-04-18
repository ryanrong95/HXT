using Needs.Ccs.Services.ApiSettings;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PoundageImport
    {
        public List<string> FinPIDs { get; set; }
        public PoundageImportModel DataFinP { get; set; }
        public PoundageImport(List<PoundageItem> list)
        {
            string pID = Needs.Overall.PKeySigner.Pick(PKeyType.XDTPoundage);
            this.DataFinP = new PoundageImportModel();
            DataFinP.归属模板编号 = MakeAccountSetting.PoundageImport_归属模板编号;
            DataFinP.归属方案编号 = MakeAccountSetting.PoundageImport_归属方案编号;
            DataFinP.归属账套 = MakeAccountSetting.归属账套;
            DataFinP.源文件原始名称 = pID;
            DataFinP.源文件内容 = new List<Voucher_SFD_XDT_YHSXF>();

            this.FinPIDs = new List<string>();
            var banks = BankMapService.Current.BankMap.banks;
           
            foreach (var t in list)
            {
                this.FinPIDs.Add(t.ID);
                string bankname = banks[t.FinanceAccount];
                DataFinP.源文件内容.Add(new Voucher_SFD_XDT_YHSXF
                {                   
                    金额 = t.Amount,
                    银行名称 = bankname,
                    标识 = t.ID,
                    摘要 = t.Summary,
                    收款日期 = t.PayDate
                });
            }
        }

        public bool Make()
        {
            try
            {

                var flag = true;

                var mk = new MakeAccountHandler();

                //报关进口-全额开票
                if (DataFinP.源文件内容.Count() > 0)
                {
                    string pid = DataFinP.源文件原始名称;
                    var jResult = mk.PostToK3(this.DataFinP);
                    using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        string finPIDs = string.Join(",", FinPIDs.ToArray());
                        //记录日志
                        responsitory.Insert(new Layer.Data.Sqls.ScCustoms.Logs
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            Name = "生成凭证",
                            MainID = "手续费",
                            AdminID = "XDTAdmin",
                            Summary = finPIDs + " " + jResult.Json(),
                            Json = DataFinP.Json(),
                            CreateDate = DateTime.Now,
                        });

                        ////成功，更新apiNotice
                        if (jResult.success && jResult.data)
                        {
                            responsitory.Update<Layer.Data.Sqls.ScCustoms.FinancePayments>(
                                    new
                                    {
                                        FinPCreSta = true,
                                        RequestID = pid
                                    }, item => FinPIDs.Contains(item.ID));

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
                string finPIDs = string.Join(",", FinPIDs.ToArray());
                ex.CcsLog("推送手续费：" + finPIDs);
                return false;
            }
        }
    }

    public class PoundageImportModel
    {

        public string 归属模板编号 { get; set; }
        public string 归属方案编号 { get; set; }
        public string 归属账套 { get; set; }
        public string 源文件原始名称 { get; set; }
        public List<Voucher_SFD_XDT_YHSXF> 源文件内容 { get; set; }

    }

    public class Voucher_SFD_XDT_YHSXF
    {
        public decimal 金额 { get; set; }
        public string 银行名称 { get; set; }
        public string 摘要 { get; set; }
        public string 标识 { get; set; }
        public string 收款日期 { get; set; }
    }

    public class PoundageItem
    {
        public string ID { get; set; }
        public decimal Amount { get; set; }
        public string BankName { get; set; }
        public string FinanceAccount { get; set; }
        public string PayDate { get; set; }
        public string Summary
        {
            get
            {
                return "银行手续费";
            }
        }
    }
}
