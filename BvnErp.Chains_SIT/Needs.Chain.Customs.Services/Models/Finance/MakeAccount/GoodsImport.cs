using Needs.Ccs.Services.ApiSettings;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class GoodsImport
    {
        public List<string> GoodsIDs { get; set; }

        public GoodsImportModel DataAcc { get; set; }

        public GoodsImport(List<GoodsImportItem> list)
        {
            string fullID = Needs.Overall.PKeySigner.Pick(PKeyType.XDTRecPre);
            this.DataAcc = new GoodsImportModel();
            DataAcc.归属模板编号 = MakeAccountSetting.GoodsImport_归属模板编号;
            DataAcc.归属方案编号 = MakeAccountSetting.GoodsImport_归属方案编号;
            DataAcc.归属账套 = MakeAccountSetting.归属账套;
            DataAcc.源文件原始名称 = fullID;
            DataAcc.源文件内容 = new List<Voucher_SFD_XDT_SKHK>();

            this.GoodsIDs = new List<string>();
            var banks = BankMapService.Current.BankMap.banks;

            foreach (var t in list)
            {
                this.GoodsIDs.Add(t.ID);
                string bankname = banks[t.Account];
                DataAcc.源文件内容.Add(new Voucher_SFD_XDT_SKHK
                {
                    金额 = t.Amount,
                    银行名称 = bankname,
                    客户名称 = t.Payer,
                    摘要 = t.Summary,
                    标识 = t.ID,
                    日期 = t.ReceiptDate,
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
                        string goodsIDs = string.Join(",", GoodsIDs.ToArray());
                        //记录日志
                        responsitory.Insert(new Layer.Data.Sqls.ScCustoms.Logs
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            Name = "生成凭证",
                            MainID = "货款",
                            AdminID = "XDTAdmin",
                            Summary = goodsIDs + " " + jResult.Json(),
                            Json = DataAcc.Json(),
                            CreateDate = DateTime.Now,
                        });

                        ////成功，更新apiNotice
                        if (jResult.success && jResult.data)
                        {
                            responsitory.Update<Layer.Data.Sqls.ScCustoms.FinanceReceipts>(
                                    new
                                    {
                                        GoodsCreStatus = true,
                                        RequestID = requestid
                                    }, item => GoodsIDs.Contains(item.ID));

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
                string goodsIDs = string.Join(",", GoodsIDs.ToArray());
                ex.CcsLog("货款：" + goodsIDs);
                return false;
            }
        }

    }

    public class GoodsImportModel
    {

        public string 归属模板编号 { get; set; }
        public string 归属方案编号 { get; set; }
        public string 归属账套 { get; set; }
        public string 源文件原始名称 { get; set; }
        public List<Voucher_SFD_XDT_SKHK> 源文件内容 { get; set; }

    }

    public class Voucher_SFD_XDT_SKHK
    {
        public decimal 金额 { get; set; }
        public string 银行名称 { get; set; }
        public string 客户名称 { get; set; }
        public string 摘要 { get; set; }
        public string 标识 { get; set; }
        public string 日期 { get; set; }
    }

    public class GoodsImportItem
    {
        public string ID { get; set; }    
        public decimal Amount { get; set; }
        public string BankName { get; set; }
        public string Account { get; set; }
        public string Payer { get; set; }
        public string Summary
        {
            get
            {
                return "收货款(" + this.Payer + ")";
            }
        }
        public string ReceiptDate { get; set; }
    }
}
