using Needs.Ccs.Services.ApiSettings;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class AccImport
    {
        public List<string> AccIDs { get; set; }
        public string IDs { get; set; }
        public AccImportModel DataAcc { get; set; }

        public AccImport(List<AccReportItem> list)
        {
            string fullID = Needs.Overall.PKeySigner.Pick(PKeyType.XDTAccept);
            this.DataAcc = new AccImportModel();
            DataAcc.归属模板编号 = MakeAccountSetting.AccImport_归属模板编号;
            DataAcc.归属方案编号 = MakeAccountSetting.AccImport_归属方案编号;
            DataAcc.归属账套 = MakeAccountSetting.归属账套;
            DataAcc.源文件原始名称 = fullID;
            DataAcc.源文件内容 = new List<Voucher_SFD_XDT_CDHPSCD>();

            this.AccIDs = new List<string>();
            
            foreach (var t in list)
            {
                this.AccIDs.Add(t.ID);
                IDs += t.ID + ",";
                DataAcc.源文件内容.Add(new Voucher_SFD_XDT_CDHPSCD
                {
                    金额 = t.Price,
                    客户名称 = t.Endorser,
                    摘要 = t.Summary,
                    标识 = t.Code,
                    日期 = t.CreateDate
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
                if (DataAcc.源文件内容.Count() > 0)
                {
                    string accid = DataAcc.源文件原始名称;
                    var jResult = mk.PostToK3(this.DataAcc);
                    using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        //记录日志
                        responsitory.Insert(new Layer.Data.Sqls.ScCustoms.Logs
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            Name = "生成凭证",
                            MainID = "承兑汇票 收承兑",
                            AdminID = "XDTAdmin",
                            Summary = jResult.Json() + " " + IDs ,                          
                            Json = DataAcc.Json(),
                            CreateDate = DateTime.Now,
                        });

                        ////成功，更新apiNotice
                        if (jResult.success && jResult.data)
                        {                           
                            responsitory.Update<Layer.Data.Sqls.ScCustoms.MoneyOrders>(
                                    new
                                    {
                                        AccCreSta = true,
                                        RequestID = accid
                                    }, item => AccIDs.Contains(item.ID));

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
                ex.CcsLog("推送承兑汇票收承兑：" + IDs);
                return false;
            }
        }

    }

    public class AccImportModel
    {

        public string 归属模板编号 { get; set; }
        public string 归属方案编号 { get; set; }
        public string 归属账套 { get; set; }
        public string 源文件原始名称 { get; set; }
        public List<Voucher_SFD_XDT_CDHPSCD> 源文件内容 { get; set; }

    }

    public class Voucher_SFD_XDT_CDHPSCD
    {
        public decimal 金额 { get; set; }
        public string 客户名称 { get; set; }
        public string 摘要 { get; set; }
        public string 标识 { get; set; }
        public string 日期  { get; set; }
    }

    public class AccReportItem
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public decimal Price { get; set; }
        public string Endorser { get; set; }
        public string Summary
        {
            get
            {
                return "收承兑汇票(" + this.Endorser + ")" + this.Code;
            }
        }
        public string CreateDate { get; set; }
    }
}
