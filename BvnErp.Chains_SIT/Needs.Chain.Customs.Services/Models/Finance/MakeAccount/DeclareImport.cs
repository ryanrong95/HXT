using Needs.Ccs.Services.ApiSettings;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DeclareImport
    {
        public List<string> DecHeadFullIDs { get; set; }
        public DeclareImportModel DataFull { get; set; }
        public List<string> DecHeadServiceIDs { get; set; }
        public DeclareImportModel DataService { get; set; }
        public DeclareOutServiceModel OutService { get; set; }

        public DeclareImport(List<SubjectReportStatistics> list)
        {
            string fullID = Needs.Overall.PKeySigner.Pick(PKeyType.XDTDecImpFull);
            this.DataFull = new DeclareImportModel();
            DataFull.归属模板编号 = MakeAccountSetting.DeclareImport_归属模板编号;
            DataFull.归属方案编号 = MakeAccountSetting.DeclareImport_归属方案编号;
            DataFull.归属账套 = MakeAccountSetting.归属账套;
            DataFull.源文件原始名称 = fullID;
            DataFull.源文件内容 = new List<DeclareImportItem>();

            string serID = Needs.Overall.PKeySigner.Pick(PKeyType.XDTDecImpSer);
            this.DataService = new DeclareImportModel();
            DataService.归属模板编号 = MakeAccountSetting.DeclareImport_归属模板编号;
            DataService.归属方案编号 = MakeAccountSetting.DeclareImport_归属方案编号;
            DataService.归属账套 = MakeAccountSetting.归属账套;
            DataService.源文件原始名称 = serID;
            DataService.源文件内容 = new List<DeclareImportItem>();

            string outID = Needs.Overall.PKeySigner.Pick(PKeyType.XDTDecImpSer);
            this.OutService = new DeclareOutServiceModel();
            OutService.归属模板编号 = MakeAccountSetting.DeclareOutService_归属模板编号;
            OutService.归属方案编号 = MakeAccountSetting.DeclareOutService_归属方案编号;
            OutService.归属账套 = MakeAccountSetting.归属账套;
            OutService.源文件原始名称 = outID;
            OutService.源文件内容 = new List<DeclareOutServiceItem>();

            this.DecHeadFullIDs = new List<string>();
            this.DecHeadServiceIDs = new List<string>();

            foreach (var t in list)
            {
                if (t.InvoiceTypeName == "全额发票")
                {                    
                    this.DecHeadFullIDs.AddRange(t.DecHeadIDs.Split(',').ToArray());
                    DataFull.源文件内容.Add(new DeclareImportItem
                    {
                        天 = int.Parse(t.Tian),
                        进口 = t.ImportPrice,
                        运保杂费用 = t.YunBaoZa,
                        关税 = t.Tariff,
                        关税实缴 = t.ActualTariff,
                        消费税 = 0M,//消费税不产生分录，默认给0或者空白
                        消费税实缴 = t.ActualExciseTax,
                        税金 = 0M ,
                        待认证进项税额 = t.ActualAddedValueTax,
                        汇兑损益_三方 = t.ExchangeCustomer,
                        三方公司 = t.ClientName,
                        汇兑损益_我方 = t.ExchangeXDT,
                        汇率 = t.RealExchangeRate.ToString(),
                        应付账款_三方 = t.DecAgentTotal,
                        物流方公司 = t.ConsignorCode,
                        应付账款_我方 = t.DecYunBaoZaTotal,
                        标识 = ChainsGuid.NewGuidUp(),
                        币别 = t.Currency,
                        开票类型 = t.InvoiceTypeName,
                        报关日期 = t.DeclareDate
                    });

                }
                else
                {
                    //报关进口-服务费开票                   
                    this.DecHeadServiceIDs.AddRange(t.DecHeadIDs.Split(','));
                    DataService.源文件内容.Add(new DeclareImportItem
                    {
                        天 = int.Parse(t.Tian),
                        进口 = t.ImportPrice,
                        运保杂费用 = t.YunBaoZa,
                        关税 = t.Tariff,
                        关税实缴 = t.ActualTariff,
                        消费税 = 0M,//消费税不产生分录，默认给0或者空白
                        消费税实缴 = t.ActualExciseTax,
                        税金 = t.ActualAddedValueTax,
                        待认证进项税额 = 0M,
                        汇兑损益_三方 = t.ExchangeCustomer,
                        三方公司 = t.ClientName,
                        汇兑损益_我方 = t.ExchangeXDT,
                        汇率 = t.RealExchangeRate.ToString(),
                        应付账款_三方 = t.DecAgentTotal,
                        物流方公司 = t.ConsignorCode,
                        应付账款_我方 = t.DecYunBaoZaTotal,
                        标识 = ChainsGuid.NewGuidUp(),
                        币别 = t.Currency,
                        开票类型 = t.InvoiceTypeName,
                        报关日期 = t.DeclareDate
                    });


                    //服务费发货
                    OutService.源文件内容.Add(new DeclareOutServiceItem {
                        天 = int.Parse(t.Tian),
                        货款 = t.DecAgentTotal1,
                        实缴关税 = t.ActualTariff1,
                        消费税 = 0M,//消费税不产生分录，默认给0或者空白
                        汇兑损益_三方 = t.ExchangeCustomerOpposite,
                        进口 =t.ImportPrice1,
                        应交关税 = t.Tariff1,
                        消费税实缴 = 0M,
                        三方公司 = t.ClientName1,
                        汇率 = t.RealExchangeRate1.ToString(),
                        标识 = ChainsGuid.NewGuidUp(),
                        币别 = t.Currency,
                        报关日期 = t.DeclareDate
                    });
                }
            }
        }

        public bool Make()
        {
            try {

                var flag = true;

                var mk = new MakeAccountHandler();

                //报关进口-全额开票
                if (DataFull.源文件内容.Count() > 0)
                {
                    string rFullID = DataFull.源文件原始名称;
                    var jResult = mk.PostToK3(this.DataFull);
                    using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        string decFullIDs = string.Join(",", DecHeadFullIDs.ToArray());
                        //记录日志
                        responsitory.Insert(new Layer.Data.Sqls.ScCustoms.Logs
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            Name = "生成凭证",
                            MainID = "报关进口-全额开票",
                            AdminID = "XDTAdmin",
                            Summary = decFullIDs + " " + jResult.Json(),
                            Json = DataFull.Json(),
                            CreateDate = DateTime.Now,
                        });

                        //成功，更新apiNotice
                        if (jResult.success && jResult.data)
                        {
                            //更新报关单状态：已生成凭证
                            responsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(
                                    new
                                    {
                                        MKImport = true
                                    }, item => DecHeadFullIDs.Contains(item.ID));

                            //持久化报关进口全额开票数据
                            foreach (var full in DataFull.源文件内容)
                            {
                                responsitory.Insert(new Layer.Data.Sqls.ScCustoms.MKDeclareImport
                                {
                                    ID = full.标识,
                                    RequestID = rFullID,
                                    TemplateCode = DataFull.归属模板编号,
                                    SchemeCode = DataFull.归属方案编号,
                                    Type = (int)DeclareImportType.DeclareFull,
                                    InvoiceType = (int)Enums.InvoiceType.Full,
                                    DeclareDate = full.报关日期,
                                    Tian = full.天,
                                    Jinkou = full.进口,
                                    //Huokuan = full.Huokuan,
                                    Yunbaoza = full.运保杂费用,
                                    Guanshui = full.关税,
                                    GuanshuiShijiao = full.关税实缴,
                                    Xiaofeishui = full.消费税,
                                    XiaofeishuiShijiao = full.消费税实缴,
                                    Shui = full.税金,
                                    Jinxiangshui = full.待认证进项税额,
                                    HuiduiSanfang = full.汇兑损益_三方,
                                    Sanfang = full.三方公司,
                                    HuiduiWofang = full.汇兑损益_我方,
                                    Huilv = decimal.Parse(full.汇率),
                                    YingfuSanfang = full.应付账款_三方,
                                    Wuliufang = full.物流方公司,
                                    YingfuWofang = full.应付账款_我方,
                                    Currency = full.币别,
                                    //PingzhengZi = full.PingzhengZi,
                                    //PingzhengHao = full.PingzhengHao,
                                    Status = (int)Enums.Status.Normal,
                                    CreateDate = DateTime.Now,
                                    UpdateDate = DateTime.Now,
                                    //Summary = full.Summary
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
                if (DataService.源文件内容.Count() > 0)
                {
                    var jResult = mk.PostToK3(this.DataService);

                    //发货-服务费
                    var jResultOut = mk.PostToK3(this.OutService);

                    using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        string rSerID = DataService.源文件原始名称;
                        string rOutID = OutService.源文件原始名称;
                        string decServiceIDs = string.Join(",", DecHeadServiceIDs.ToArray());
                        //记录日志
                        responsitory.Insert(new Layer.Data.Sqls.ScCustoms.Logs
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            Name = "生成凭证",
                            MainID = "报关进口-服务费开票",
                            AdminID = "XDTAdmin",
                            Summary = decServiceIDs + " " + jResult.Json(),
                            Json = DataService.Json(),
                            CreateDate = DateTime.Now,
                        });

                        responsitory.Insert(new Layer.Data.Sqls.ScCustoms.Logs
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            Name = "生成凭证",
                            MainID = "发货-服务费开票",
                            AdminID = "XDTAdmin",
                            Summary = decServiceIDs + " " + jResult.Json(),
                            Json = OutService.Json(),
                            CreateDate = DateTime.Now,
                        });
                        
                        //成功，更新apiNotice
                        if (jResult.success && jResult.data && jResultOut.success && jResultOut.data)
                        {
                            //更新报关单状态：已生成凭证
                            responsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(
                                    new
                                    {
                                        MKImport = true
                                    }, item => DecHeadServiceIDs.Contains(item.ID));

                            //持久化报关进口服务费开票数据
                            foreach (var full in DataService.源文件内容)
                            {
                                responsitory.Insert(new Layer.Data.Sqls.ScCustoms.MKDeclareImport
                                {
                                    ID = full.标识,
                                    RequestID = rSerID,
                                    TemplateCode = DataService.归属模板编号,
                                    SchemeCode = DataService.归属方案编号,
                                    Type = (int)DeclareImportType.DeclareService,
                                    InvoiceType = (int)Enums.InvoiceType.Service,
                                    DeclareDate = full.报关日期,
                                    Tian = full.天,
                                    Jinkou = full.进口,
                                    //Huokuan = full.Huokuan,
                                    Yunbaoza = full.运保杂费用,
                                    Guanshui = full.关税,
                                    GuanshuiShijiao = full.关税实缴,
                                    Xiaofeishui = full.消费税,
                                    XiaofeishuiShijiao = full.消费税实缴,
                                    Shui = full.税金,
                                    Jinxiangshui = full.待认证进项税额,
                                    HuiduiSanfang = full.汇兑损益_三方,
                                    Sanfang = full.三方公司,
                                    HuiduiWofang = full.汇兑损益_我方,
                                    Huilv = decimal.Parse(full.汇率),
                                    YingfuSanfang = full.应付账款_三方,
                                    Wuliufang = full.物流方公司,
                                    YingfuWofang = full.应付账款_我方,
                                    Currency = full.币别,
                                    //PingzhengZi = full.PingzhengZi,
                                    //PingzhengHao = full.PingzhengHao,
                                    Status = (int)Enums.Status.Normal,
                                    CreateDate = DateTime.Now,
                                    UpdateDate = DateTime.Now,
                                    //Summary = full.Summary
                                });
                            }

                            //持久化报关 发货服务费开票数据
                            foreach (var full in OutService.源文件内容)
                            {
                                responsitory.Insert(new Layer.Data.Sqls.ScCustoms.MKDeclareImport
                                {
                                    ID = full.标识,
                                    RequestID = rOutID,
                                    TemplateCode = OutService.归属模板编号,
                                    SchemeCode = OutService.归属方案编号,
                                    Type = (int)DeclareImportType.OutService,
                                    InvoiceType = (int)Enums.InvoiceType.Service,
                                    DeclareDate = full.报关日期,
                                    Tian = full.天,
                                    Jinkou = full.进口,
                                    Huokuan = full.货款,
                                    //Yunbaoza = full.运保杂费用,
                                    //Guanshui = full.关税,
                                    GuanshuiShijiao = full.实缴关税,
                                    Xiaofeishui = full.消费税,
                                    XiaofeishuiShijiao = full.消费税实缴,
                                    //Shui = full.税金,
                                    //Jinxiangshui = full.待认证进项税额,
                                    HuiduiSanfang = full.汇兑损益_三方,
                                    Sanfang = full.三方公司,
                                    //HuiduiWofang = full.汇兑损益_我方,
                                    Huilv = decimal.Parse(full.汇率),
                                    //YingfuSanfang = full.应付账款_三方,
                                    //Wuliufang = full.物流方公司,
                                    //YingfuWofang = full.应付账款_我方,
                                    Currency = full.币别,
                                    //PingzhengZi = full.PingzhengZi,
                                    //PingzhengHao = full.PingzhengHao,
                                    Status = (int)Enums.Status.Normal,
                                    CreateDate = DateTime.Now,
                                    UpdateDate = DateTime.Now,
                                    //Summary = full.Summary
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
                return flag;
            }
            catch (Exception ex) {
                string decFullIDs = string.Join(",", DecHeadFullIDs.ToArray());
                string decServiceIDs = string.Join(",", DecHeadServiceIDs.ToArray());
                ex.CcsLog("推送报关进口凭证错误：" + decFullIDs + ","+ decServiceIDs);
                return false;
            }
        }
    }

    #region 报关进口

    public class DeclareImportModel
    {

        public string 归属模板编号 { get; set; }
        public string 归属方案编号 { get; set; }
        public string 归属账套 { get; set; }
        public string 源文件原始名称 { get; set; }
        public List<DeclareImportItem> 源文件内容 { get; set; }

    }

    public class DeclareImportItem
    {
        public int 天 { get; set; }
        public decimal 进口 { get; set; }
        public decimal 运保杂费用 { get; set; }
        public decimal 关税 { get; set; }
        public decimal 关税实缴 { get; set; }
        public decimal 消费税 { get; set; }
        public decimal 消费税实缴 { get; set; }
        /// <summary>
        /// 服务费开票时赋值，否则写0
        /// </summary>
        public decimal 税金 { get; set; }

        /// <summary>
        /// 全额开票时赋值，否则写0
        /// </summary>
        public decimal 待认证进项税额 { get; set; }
        public decimal 汇兑损益_三方 { get; set; }
        public string 三方公司 { get; set; }
        public decimal 汇兑损益_我方 { get; set; }
        public string 汇率 { get; set; }
        public decimal 应付账款_三方 { get; set; }
        public string 物流方公司 { get; set; }
        public decimal 应付账款_我方 { get; set; }
        public string 标识 { get; set; }
        public string 币别 { get; set; }
        public string 开票类型 { get; set; }
        public string 报关日期 { get; set; }
    }

    #endregion


    #region 发货-服务费发票类型

    public class DeclareOutServiceModel
    {

        public string 归属模板编号 { get; set; }
        public string 归属方案编号 { get; set; }
        public string 归属账套 { get; set; }
        public string 源文件原始名称 { get; set; }
        public List<DeclareOutServiceItem> 源文件内容 { get; set; }

    }

    public class DeclareOutServiceItem
    {
        public int 天 { get; set; }
        public decimal 货款 { get; set; }
        public decimal 实缴关税 { get; set; }
        public decimal 消费税 { get; set; }
        public decimal 汇兑损益_三方 { get; set; }
        public decimal 进口 { get; set; }
        public decimal 应交关税 { get; set; }
        public decimal 消费税实缴 { get; set; }
        public string 三方公司 { get; set; }
        public string 汇率 { get; set; }
        public string 标识 { get; set; }
        public string 币别 { get; set; }
        public string 报关日期 { get; set; }
    }

    #endregion

    public enum DeclareImportType
    {
        /// <summary>
        /// 报关全额开票
        /// </summary>
        [Description("报关-全额开票")]
        DeclareFull = 1,

        /// <summary>
        /// 报关服务费开票
        /// </summary>
        [Description("报关-服务费开票")]
        DeclareService = 2,

        /// <summary>
        /// 发货服务费开票
        /// </summary>
        [Description("发货-服务费开票")]
        OutService = 3
    }

}
