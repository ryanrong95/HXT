using Needs.Ccs.Services.ApiSettings;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class FinancePimport
    {
        public List<string> FinPIDs { get; set; }
        public string finPIDs { get; set; }
        public FinancePImportModel DataFinP { get; set; }

        public FinancePimport(List<FinPReportItem> list)
        {
            string fpID = Needs.Overall.PKeySigner.Pick(PKeyType.XDTPayTax);
            this.DataFinP = new FinancePImportModel();
            DataFinP.归属模板编号 = MakeAccountSetting.FinancePImport_归属模板编号;
            DataFinP.归属方案编号 = MakeAccountSetting.FinancePImport_归属方案编号;
            DataFinP.归属账套 = MakeAccountSetting.归属账套;
            DataFinP.源文件原始名称 = fpID;
            DataFinP.源文件内容 = new List<Voucher_SFD_XDT_JNGSZZS>();

            this.FinPIDs = new List<string>();
            var banks = BankMapService.Current.BankMap.banks;

            foreach (var t in list)
            {
                this.FinPIDs.Add(t.ID);
                finPIDs += t.ID + ",";
                string bankname = banks[t.FinanceAccount];
                DataFinP.源文件内容.Add(new Voucher_SFD_XDT_JNGSZZS
                {
                   天 = t.PayDay,
                   金额 = t.Amount,
                   银行名称 = bankname,
                   标识 = t.ID
                });
            }
        }

        public bool Make()
        {
            try
            {

                var flag = true;

                var mk = new MakeAccountHandler();

                string requestid = DataFinP.源文件原始名称;
                //报关进口-全额开票
                if (DataFinP.源文件内容.Count() > 0)
                {
                    var jResult = mk.PostToK3(this.DataFinP);
                    using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        //记录日志
                        responsitory.Insert(new Layer.Data.Sqls.ScCustoms.Logs
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            Name = "生成凭证",
                            MainID = "缴纳报关进口关税增值税",
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
                                        RequestID = requestid
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
                ex.CcsLog("推送缴纳报关进口关税增值税：" + finPIDs);
                return false;
            }
        }
    }

    public class FinancePImportModel
    {

        public string 归属模板编号 { get; set; }
        public string 归属方案编号 { get; set; }
        public string 归属账套 { get; set; }
        public string 源文件原始名称 { get; set; }
        public List<Voucher_SFD_XDT_JNGSZZS> 源文件内容 { get; set; }

    }

    public class Voucher_SFD_XDT_JNGSZZS
    {
        public int 天 { get; set; }
        public decimal 金额 { get; set; }
        public string 银行名称 { get; set; }
        public string 标识 { get; set; }
    }

    public class FinPReportItem
    {
        public string ID { get; set; }
        public decimal Amount { get; set; }
        public string BankName { get; set; }
        public string FinanceAccount { get; set; }
        public DateTime PayDate { get; set; }
        public int PayDay
        {
            get
            {
                return this.PayDate.Day;
            }
        }
    }
}
