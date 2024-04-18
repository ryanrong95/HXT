using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Enums;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class InvoiceImport
    {
        public List<string> FullIDs { get; set; }
        public string fullIDs { get; set; }

        public List<string> ServiceIDs { get; set; }
        public string serviceIDs { get; set; }

        public ServiceImportModel DataService { get; set; }
        public FullImportModel DataFull { get; set; }

        public InvoiceImport(List<InvoiceReportItem> list)
        {
            string serID = Needs.Overall.PKeySigner.Pick(PKeyType.XDTReceipt);
            string fullID = Needs.Overall.PKeySigner.Pick(PKeyType.XDTReceipt);
            this.DataService = new ServiceImportModel();
            DataService.归属模板编号 = MakeAccountSetting.ServiceInvoImport_归属模板编号;
            DataService.归属方案编号 = MakeAccountSetting.ServiceInvoImport_归属方案编号;
            DataService.归属账套 = MakeAccountSetting.归属账套;
            DataService.源文件原始名称 = serID;
            DataService.源文件内容 = new List<Voucher_SFD_XDT_FWF>();

            this.DataFull = new FullImportModel();
            DataFull.归属模板编号 = MakeAccountSetting.FullInvoImport_归属模板编号;
            DataFull.归属方案编号 = MakeAccountSetting.FullInvoImport_归属方案编号;
            DataFull.归属账套 = MakeAccountSetting.归属账套;
            DataFull.源文件原始名称 = fullID;
            DataFull.源文件内容 = new List<Voucher_SFD_XDT_XH>();

            this.ServiceIDs = new List<string>();
            this.FullIDs = new List<string>();

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                foreach (var t in list)
                {
                    var invoiceItems =  reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmlItems>().
                                            Where(item => item.InvoiceNoticeXmlID == t.ID).Select(item =>
                                              new {
                                                  item.Je,
                                                  item.Se
                                              });
                                       

                    if (t.InvoiceType == (int)InvoiceType.Full)
                    {
                        this.FullIDs.Add(t.ID);
                        fullIDs += t.ID + ",";
                        DataFull.源文件内容.Add(new Voucher_SFD_XDT_XH
                        {
                            天 = t.InvoiceDay,
                            金额 = invoiceItems.Sum(i => i.Je),
                            应交增值税 = invoiceItems.Sum(i=>i.Se),
                            购方企业名称 = t.CompanyName,
                            标识 = t.ID
                        });
                    }
                    else
                    {
                        this.ServiceIDs.Add(t.ID);
                        serviceIDs += t.ID + ",";
                        DataService.源文件内容.Add(new Voucher_SFD_XDT_FWF
                        {
                            天 = t.InvoiceDay,
                            金额 = invoiceItems.Sum(i => i.Je),
                            应交增值税 = invoiceItems.Sum(i => i.Se),
                            供应商 = t.CompanyName,
                            标识 = t.ID
                        });
                    }
                }
            }            
        }

        public bool Make()
        {
            try
            {

                var flag = true;

                var mk = new MakeAccountHandler();

                string fRequestID = DataFull.源文件原始名称;
                string sRequestID = DataService.源文件原始名称;
                //全额开票
                if (DataService.源文件内容.Count() > 0)
                {
                    var jResult = mk.PostToK3(this.DataService);
                    using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        //记录日志
                        responsitory.Insert(new Layer.Data.Sqls.ScCustoms.Logs
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            Name = "生成凭证",
                            MainID = "服务费发票",
                            AdminID = "XDTAdmin",
                            Summary = serviceIDs + " " + jResult.Json(),
                            Json = DataService.Json(),
                            CreateDate = DateTime.Now,
                        });

                        ////成功，更新apiNotice
                        if (jResult.success && jResult.data)
                        {
                            responsitory.Update<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmls>(
                                    new
                                    {
                                        InCreSta = true,
                                        RequestID = sRequestID
                                    }, item => ServiceIDs.Contains(item.ID));

                            flag &= true;
                        }
                        else
                        {
                            flag &= false;
                        }
                    }
                }


                //全额开票
                if (DataFull.源文件内容.Count() > 0)
                {
                    var jResult = mk.PostToK3(this.DataFull);
                    using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        //记录日志
                        responsitory.Insert(new Layer.Data.Sqls.ScCustoms.Logs
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            Name = "生成凭证",
                            MainID = "全额发票",
                            AdminID = "XDTAdmin",
                            Summary = fullIDs + " " + jResult.Json(),
                            Json = DataFull.Json(),
                            CreateDate = DateTime.Now,
                        });

                        ////成功，更新apiNotice
                        if (jResult.success && jResult.data)
                        {
                            responsitory.Update<Layer.Data.Sqls.ScCustoms.InvoiceNoticeXmls>(
                                    new
                                    {
                                        InCreSta = true,
                                        RequestID = fRequestID
                                    }, item => FullIDs.Contains(item.ID));

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
                ex.CcsLog("推送发票：" + serviceIDs + " " + fullIDs);
                return false;
            }
        }
    }

    public class ServiceImportModel
    {

        public string 归属模板编号 { get; set; }
        public string 归属方案编号 { get; set; }
        public string 归属账套 { get; set; }
        public string 源文件原始名称 { get; set; }
        public List<Voucher_SFD_XDT_FWF> 源文件内容 { get; set; }

    }

    public class Voucher_SFD_XDT_FWF
    {
        public int 天 { get; set; }
        public decimal 金额 { get; set; }
        public decimal 应交增值税 { get; set; }
        public string 供应商 { get; set; }
        public string 标识 { get; set; }
    }


    public class FullImportModel
    {

        public string 归属模板编号 { get; set; }
        public string 归属方案编号 { get; set; }
        public string 归属账套 { get; set; }
        public string 源文件原始名称 { get; set; }
        public List<Voucher_SFD_XDT_XH> 源文件内容 { get; set; }

    }

    public class Voucher_SFD_XDT_XH
    {
        public int 天 { get; set; }
        public decimal 金额 { get; set; }
        public decimal 应交增值税 { get; set; }
        public string 购方企业名称 { get; set; }
        public string 标识 { get; set; }
    }

    public class InvoiceReportItem
    {
        public string ID { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string CompanyName { get; set; }
        public int InvoiceDay
        {
            get
            {
                return this.InvoiceDate.Day;
            }
        }

        public int InvoiceType { get; set; }
    }

}
