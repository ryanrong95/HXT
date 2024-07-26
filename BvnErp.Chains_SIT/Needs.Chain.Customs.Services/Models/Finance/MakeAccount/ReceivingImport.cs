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
    public class ReceivingImport
    {
        public List<string> ReFullIDs { get; set; }
        public ReFullImportModel ReFull { get; set; }

        public List<string> ReSerIDs { get; set; }
        public ReSerImportModel ReSer { get; set; }

        public ReceivingImport(string ClientName, string QuerenStatus, string StartDate, string EndDate, string ReceiptStartDate, string ReceiptEndDate,List<string> selectedIDs)
        {
            string fullID =  Needs.Overall.PKeySigner.Pick(PKeyType.XDTRecFund);
            string serID = Needs.Overall.PKeySigner.Pick(PKeyType.XDTRecFund);
            this.ReFull = new ReFullImportModel();
            ReFull.归属模板编号 = MakeAccountSetting.ReFullImport_归属模板编号;
            ReFull.归属方案编号 = MakeAccountSetting.ReFullImport_归属方案编号;
            ReFull.归属账套 = MakeAccountSetting.归属账套;
            ReFull.源文件原始名称 = fullID;
            ReFull.源文件内容 = new List<Voucher_SFD_XDT_CYS_QE>();

            this.ReSer = new ReSerImportModel();
            ReSer.归属模板编号 = MakeAccountSetting.ReSerImport_归属模板编号;
            ReSer.归属方案编号 = MakeAccountSetting.ReSerImport_归属方案编号;
            ReSer.归属账套 = MakeAccountSetting.归属账套;
            ReSer.源文件原始名称 = serID;
            ReSer.源文件内容 = new List<Voucher_SFD_XDT_CYS_FWF>();

            this.ReFullIDs = new List<string>();
            this.ReSerIDs = new List<string>();

            #region 所有数据
            //货款明细表视图
            var view = new Views.OrderReceiptReportView();

            var viewDetail = view.GetProductReceiptDetailForCredential(StartDate, EndDate, ReceiptStartDate, ReceiptEndDate);

            //统计表视图
            var viewMain = view.GetFinanceReceiptMainForCredential(StartDate, EndDate, ReceiptStartDate, ReceiptEndDate);

            //查询条件过滤
            if (!string.IsNullOrEmpty(QuerenStatus) && QuerenStatus != "0")
            {
                //0 - 全部, 1 - 未确认, 2 - 已确认
                if (QuerenStatus == "1")
                {
                    viewMain = viewMain.Where(x => x.ClearAmount < x.Amount).ToList();
                }
                else if (QuerenStatus == "2")
                {
                    viewMain = viewMain.Where(x => x.ClearAmount == x.Amount).ToList();
                }
            }
            if (string.IsNullOrEmpty(ClientName) == false)
            {
                viewMain = viewMain.Where(item => item.ClientName == ClientName).ToList();
            }

            viewMain = viewMain.OrderBy(t => t.ReceiptDate).ToList();


            //主表执行查询
            var mainList = viewMain;
            //单独几条生成凭证
            if (selectedIDs.Count != 0) 
            {
                mainList = viewMain.Where(t => selectedIDs.Contains(t.ID)).ToList();
            }

            //之前没有完全过滤收款时间，所以需要按照统计表有的进行汇差统计
            //财务收款ID
            var receiptIDs = mainList.Select(t => t.ID).Distinct().ToArray();
          
            //货款明细执行查询
            var productdetail = viewDetail.Where(t => receiptIDs.Contains(t.FinanceReceiptID)).ToArray();
            //货款明细补全汇率
            var detailList = view.GetProductReceipt(productdetail);

            foreach (var m in mainList)
            {
                //1、统计总汇差
                m.TotalExchangeSpread = detailList.Where(t => t.FinanceReceiptID == m.ID).Sum(t => t.exDiff);

                //2、K3账户名词修改
                switch (m.AccountName)
                {
                    case ("华芯通-星展银行账户"):
                        m.AccountName = "星展银行（中国）有限公司深圳分行";
                        break;
                    case ("华芯通-中国银行深圳罗岗支行"):
                        m.AccountName = "中国银行深圳罗岗支行";
                        break;
                    case ("华芯通-农业银行人民币账户"):
                        m.AccountName = "中国农业银行股份有限公司深圳免税大厦支行";
                        break;
                    case ("华芯通-宁波银行人民币户"):
                        m.AccountName = "宁波银行深圳分行";
                        break;
                    case ("华芯通-兴业银行"):
                        m.AccountName = "兴业银行股份有限公司深圳罗湖支行";
                        break;
                    default:
                        break;
                }
            }

            #endregion

            foreach (var m in mainList)
            {
                if(m.InvoiceType== (int)Enums.InvoiceType.Full)
                {
                    List<string> fullreceiptids = m.OrderRecepitID.Split(',').ToList<string>();
                    this.ReFullIDs.AddRange(fullreceiptids);

               
                    var TotalAddTax = m.TotalAddTax == null ? 0 : m.TotalAddTax.Value;
                    var TotalTariffTax = m.TotalTariffTax == null ? 0 : m.TotalTariffTax.Value;
                    var TotalExciseTax = m.TotalExciseTax == null ? 0 : m.TotalExciseTax.Value;
                    var TotalAgency = m.TotalAgency == null ? 0 : m.TotalAgency.Value;

                    var fullItems = detailList.Where(t => t.FinanceReceiptID == m.ID && t.InvoiceType == (int)Enums.InvoiceType.Full);
                    var TotalProduct = fullItems.Sum(t => t.RMB);
                    var productfee = TotalProduct + TotalAddTax + TotalTariffTax + TotalExciseTax + TotalAgency;

                    ReFull.源文件内容.Add(new Voucher_SFD_XDT_CYS_QE
                    {
                        OrderReceiptID = m.OrderRecepitID,
                        预收账款 = productfee - m.TotalExchangeSpread,
                        汇兑损益 = m.TotalExchangeSpread,
                        货款 = productfee,
                        客户名称 = m.ClientName,
                        标识 = ChainsGuid.NewGuidUp(),
                        //日期 = m.ReceiptDate.ToString("yyyy-MM-dd")
                        日期 = ReceiptEndDate
                    });
                }
                else
                {
                    List<string> serreceiptids = m.OrderRecepitID.Split(',').ToList<string>();
                    this.ReSerIDs.AddRange(serreceiptids);                    
                    List<Voucher_SFD_XDT_CYS_QE_FWF_IIEM> serviceItems = new List<Voucher_SFD_XDT_CYS_QE_FWF_IIEM>();
                    var serItems = detailList.Where(t => t.FinanceReceiptID == m.ID&&t.InvoiceType==(int)Enums.InvoiceType.Service);
                    foreach(var sm in serItems)
                    {
                        serviceItems.Add(new Voucher_SFD_XDT_CYS_QE_FWF_IIEM
                        {
                            收款ID = sm.FinanceReceiptID,
                            流水号 = sm.SeqNo,
                            美金货值 = sm.USD,
                            报关_开票汇率 = sm.DDateRate,
                            货款 = sm.RMB,
                            币别 = sm.DeclareCurrency,
                            //日期 = sm.ReceiptDate.ToString("yyyy-MM-dd")                           
                        });
                    }

                    Voucher_SFD_XDT_CYS_FWF sitem = new Voucher_SFD_XDT_CYS_FWF();
                    sitem.货款明细 = new List<Voucher_SFD_XDT_CYS_QE_FWF_IIEM>();
                    
                    sitem.OrderReceiptID = m.OrderRecepitID;
                    sitem.货款明细 = serviceItems;
                    sitem.汇兑损益 = m.TotalExchangeSpread;
                    sitem.税金 = m.TotalAddTax == null ? 0 : m.TotalAddTax.Value;
                    sitem.关税 = m.TotalTariffTax == null ? 0 : m.TotalTariffTax.Value;
                    sitem.消费税 = m.TotalExciseTax == null ? 0 : m.TotalExciseTax.Value;
                    sitem.代理费 = m.TotalAgency == null ? 0 : m.TotalAgency.Value;
                    sitem.预收账款 = sitem.货款明细.Sum(t => t.货款) + sitem.税金 + sitem.关税 + sitem.消费税 + sitem.代理费 - sitem.汇兑损益;
                    sitem.日期 = ReceiptEndDate;
                    sitem.客户名称 = m.ClientName;
                    sitem.标识 = ChainsGuid.NewGuidUp();


                    ReSer.源文件内容.Add(sitem);
                }
            }
        }

        public bool Make()
        {
            try
            {
                var flag = true;
               
                var mk = new MakeAccountHandler();

                //报关进口-全额开票
                if (ReFull.源文件内容.Count() > 0)
                {
                    var jResult = mk.PostToK3(this.ReFull);
                    using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        string decFullIDs = string.Join(",", ReFullIDs.ToArray());
                        //记录日志
                        responsitory.Insert(new Layer.Data.Sqls.ScCustoms.Logs
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            Name = "生成凭证",
                            MainID = "收款统计-全额发票",
                            AdminID = "XDTAdmin",
                            Summary = decFullIDs + " " + jResult.Json(),                           
                            Json = ReFull.Json(),
                            CreateDate = DateTime.Now,
                        });

                        //成功，更新apiNotice
                        if (jResult.success && jResult.data)
                        {
                            //更新报关单状态：已生成凭证
                            responsitory.Update<Layer.Data.Sqls.ScCustoms.OrderReceipts>(
                                    new
                                    {
                                        ReImport = true
                                    }, item => ReFullIDs.Contains(item.ID));

                            //持久化报关进口全额开票数据
                            string fullRequestID = ReFull.源文件原始名称;
                            foreach (var full in ReFull.源文件内容)
                            {
                                string orderReceiptid = full.OrderReceiptID;
                                if (orderReceiptid.Length >= 50)
                                {
                                    orderReceiptid = full.OrderReceiptID.Substring(0, 30);
                                }
                                responsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderReceiptImport
                                {
                                    ID = full.标识,     
                                    RequestID = fullRequestID,
                                    OrderReceiptID = orderReceiptid,
                                    InvoiceType = (int)InvoiceType.Full,
                                    PreMoney = full.预收账款,
                                    Diff = full.汇兑损益,
                                    GoodsMoney = full.货款,
                                    ClientName = full.客户名称,
                                    Status = (int)Status.Normal,
                                    CreateDate = DateTime.Now,
                                    UpdateDate = DateTime.Now,
                                });

                            }

                            flag &= true;
                        }
                        else
                        {
                            flag &= false;
                        }
                    }
                }

                //报关进口-服务费开票
                if (ReSer.源文件内容.Count() > 0)
                {
                    var jResult = mk.PostToK3(this.ReSer);

                    using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        string decServiceIDs = string.Join(",", ReSerIDs.ToArray());
                        //记录日志
                        responsitory.Insert(new Layer.Data.Sqls.ScCustoms.Logs
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            Name = "生成凭证",
                            MainID = "收款统计-服务费发票",
                            AdminID = "XDTAdmin",
                            Summary = decServiceIDs + " " + jResult.Json(),                            
                            Json = ReSer.Json(),
                            CreateDate = DateTime.Now,
                        });

                        //成功，更新apiNotice
                        if (jResult.success && jResult.data)
                        {
                            //更新报关单状态：已生成凭证
                            responsitory.Update<Layer.Data.Sqls.ScCustoms.OrderReceipts>(
                                    new
                                    {
                                        ReImport = true
                                    }, item => ReSerIDs.Contains(item.ID));

                            //持久化报关进口服务费开票数据
                            string serID = ReSer.源文件原始名称;
                            foreach (var full in ReSer.源文件内容)
                            {
                                string orderReceiptid = full.OrderReceiptID;
                                if (orderReceiptid.Length >= 50) 
                                {
                                    orderReceiptid = full.OrderReceiptID.Substring(0, 30);
                                }
                                responsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderReceiptImport
                                {
                                    ID = full.标识,
                                    RequestID = serID,
                                    OrderReceiptID = orderReceiptid,
                                    InvoiceType = (int)InvoiceType.Service,
                                    PreMoney = full.预收账款,
                                    Diff = full.汇兑损益,                                   
                                    ClientName = full.客户名称,                                  
                                    AddTax = full.税金,
                                    Tariff = full.关税,
                                    ExciseTax = full.消费税,
                                    Agency = full.代理费,
                                    Status = (int)Status.Normal,
                                    CreateDate = DateTime.Now,
                                    UpdateDate = DateTime.Now,
                                });

                                foreach (var item in full.货款明细)
                                {
                                    responsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderReceiptImportItems
                                    {
                                        ID = ChainsGuid.NewGuidUp(),
                                        ImportID = full.标识,
                                        FinanceRepID = item.收款ID,
                                        Seq = item.流水号,
                                        USD = item.美金货值,
                                        RMB = item.货款,
                                        DeclareRate = item.报关_开票汇率,
                                        Currency = item.币别,
                                        Status = (int)Status.Normal,
                                        CreateDate = DateTime.Now,
                                        UpdateDate = DateTime.Now,
                                    });
                                }                               
                            }
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
                string decFullIDs = string.Join(",", ReFullIDs.ToArray());
                string decServiceIDs = string.Join(",", ReSerIDs.ToArray());
                ex.CcsLog("推送收款统计凭证错误：" + decFullIDs + "," + decServiceIDs);
                return false;
            }
        }
    }

    public class ReFullImportModel
    {

        public string 归属模板编号 { get; set; }
        public string 归属方案编号 { get; set; }
        public string 归属账套 { get; set; }
        public string 源文件原始名称 { get; set; }
        public List<Voucher_SFD_XDT_CYS_QE> 源文件内容 { get; set; }

    }

    public class Voucher_SFD_XDT_CYS_QE
    {
        public string OrderReceiptID { get; set; }
        public decimal 预收账款 { get; set; }
        public decimal 汇兑损益 { get; set; }
        public decimal 货款 { get; set; }
        public string 客户名称 { get; set; }
        public string 标识 { get; set; }
        public string 日期 { get; set; }
    }

    public class ReSerImportModel
    {
        public string 归属模板编号 { get; set; }
        public string 归属方案编号 { get; set; }
        public string 归属账套 { get; set; }
        public string 源文件原始名称 { get; set; }
        public List<Voucher_SFD_XDT_CYS_FWF> 源文件内容 { get; set; }

    }

    public class Voucher_SFD_XDT_CYS_FWF
    {
        public string OrderReceiptID { get; set; }
        public decimal 预收账款 { get; set; }
        public List<Voucher_SFD_XDT_CYS_QE_FWF_IIEM> 货款明细 { get; set; }
        public decimal 税金 { get; set; }//(增值税)
        public decimal 关税 { get; set; }
        public decimal 消费税 { get; set; }
        public decimal 代理费 { get; set; }
        public decimal 汇兑损益 { get; set; }
        public string 客户名称 { get; set; }
        public string 标识 { get; set; }
        public string 日期 { get; set; }
    }

     public class Voucher_SFD_XDT_CYS_QE_FWF_IIEM
    {
        public string 收款ID { get; set; }
        public string 流水号 { get; set; }
        public decimal 美金货值 { get; set; } //P
        public decimal 报关_开票汇率 { get; set; }//M
        public decimal 货款 { get; set; } //Q
        public string 币别 { get; set; }       
    }
}
