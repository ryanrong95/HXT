using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services
{
    /// <summary>
    /// 配置 or 初始化常量信息
    /// </summary>
    public class ConstConfig
    {
        /// <summary>
        /// 一步申报 报关总价取整
        /// </summary>
        public const int CustomsValueOne = 0;

        /// <summary>
        /// 两步申报 报关总价取两位小数
        /// </summary>
        public const int CustomsValueTwo = 2;
        /// <summary>
        /// 海关运保杂费
        /// </summary>
        public const decimal TransPremiumInsurance = 1.002M;

        /// <summary>
        /// 单抬头税赋比例
        /// </summary>
        public const decimal ThousandthTaxRate = 0.001M;

        /// <summary>
        /// 增值税率
        /// </summary>
        public const decimal ValueAddedTaxRate = 0.13M;

        /// <summary>
        /// 单个型号最小装箱毛重
        /// </summary>
        public const decimal MinPackingGrossWeight = 0.01M;

        /// <summary>
        /// 单个型号最小装箱净重
        /// </summary>
        public const decimal MinPackingNetWeight = 0.01M;

        /// <summary>
        /// 报关单最小毛重
        /// </summary>
        public const decimal MinDecHeadGrossWeight = 2M;

        /// <summary>
        /// 报关单最小净重
        /// </summary>
        public const decimal MinDecHeadNetWeight = 1M;

        /// <summary>
        /// 电子随附单据问价类型
        /// </summary>
        public const string EdocFomatType = "US";

        /// <summary>
        /// 电子单据-发票
        /// </summary>
        public const string PaymentInstruction = "00000001";

        /// <summary>
        /// 电子单据-装箱单
        /// </summary>
        public const string PackingList = "00000002";

        /// <summary>
        /// 电子单据-合同
        /// </summary>
        public const string Contract = "00000004";

        /// <summary>
        /// 换汇管控国家
        /// </summary>
        public static readonly string[] SwapLimitCountry = { "AFG", "BDI", "BLR", "IRQ", "LAO", "LBN", "ROU", "SVN", "SOM", "UKR", "UGA", "VEN", "YEM", "ZWE", "SSD", "ERI", "GNB", "RUS", "COG", "COD", "CAF", "ALB", "BGR", "BIH", "GRC", "MKD", "ROU", "TUR" };

        /// <summary>
        /// 香港库房现金账户
        /// </summary>
        public static readonly Models.FinanceAccount account = new Models.FinanceAccount
        {
            ID = "FinAccount20190327000002",
        };

        /// <summary>
        /// 客户如果有引荐人则业务员的提前打折（业务员提成部分）
        /// 1 - ProfitDiscount = 引荐人提成部分
        /// </summary>
        public const decimal ProfitDiscount = 0.5M;
    }

    public class SysConfig
    {
        /// <summary>
        /// 系统文件目录 （即将作废）
        /// </summary>
        [Obsolete]
        public const string FileDirectory = "Files";

        /// <summary>
        /// 报关单电子单据文件目录
        /// </summary>
        public const string DeclareDirectory = "DeclareEdoc";

        /// <summary>
        /// 报关单电子单据文件目录
        /// </summary>
        public const string DecMessageDirectory = "DecMessage";

        /// <summary>
        /// 报关单据压缩文件目录
        /// </summary>
        public const string ZipFiles = "ZipFiles";

        /// <summary>
        /// 下载的文件目录
        /// </summary>
        public const string Dowload = "Dowload";

        /// <summary>
        /// 导入模板的文件目录
        /// </summary>
        public const string Import = "Import";

        /// <summary>
        /// 订单文件目录
        /// </summary>
        public const string Order = "Order";

        ///// <summary>
        ///// 报关单报文根目录
        ///// </summary>
        //public const string DeclareMessageRootPath = @"D:\ImpPath\Deccus001\";

        ///// <summary>
        ///// 舱单报文根目录
        ///// </summary>
        //public const string ManifestMessageRootPath = @":\ImpPath\Rmft\";

        /// <summary>
        /// 报文发送文件目录
        /// </summary>
        public const string MeaasgeFolder = "OutBox";

        /// <summary>
        /// 报关单文件目录
        /// </summary>
        public const string DecHead = "DecHead";

        /// <summary>
        /// 报关单3C文件
        /// </summary>
        public const string DecHeadCCC = "DecHeadCCC";

        /// <summary>
        /// 客户文件目录
        /// </summary>
        public const string Client = "Client";

        /// <summary>
        /// 库房文件目录
        /// </summary>
        public const string Warehouse = "Warehouse";

        /// <summary>
        /// 付汇文件目录
        /// </summary>
        public const string PayExchange = "PayExchange";

        /// <summary>
        /// 换汇文件目录
        /// </summary>
        public const string SwapFile = "SwapFile";
        /// <summary>
        /// 换汇文件的压缩目录
        /// </summary>
        public const string ZipSwapFile = "ZipSwapFile";

        /// <summary>
        /// 财务收付款附件目录
        /// </summary>
        public const string Finance = "Finance";

        /// <summary>
        /// 运输批次附件目录
        /// </summary>
        public const string Voyage = "Voyage";

        /// <summary>
        /// 导出模板路径
        /// </summary>

        public const string ExportUrl = "Content\\templates\\进口付汇报关单清单模板(农业).xlsx";

        public const string ExportXingZhanUrl = "Content\\templates\\进口付汇报关单清单模板(星展).xlsx";
        /// <summary>
        /// 导出文件路径
        /// </summary>

        public const string Export = "Export";

        public const string ExcelDeclareFileName = "Content\\templates\\报关单模板单一窗口精简.xlsx";
        /// <summary>
        /// 导出每日报关数据
        /// </summary>

        // public const string ExcelDailyDeclare = "Content\\templates\\每日报关导出模板.xlsx";

        /// <summary>
        /// 导出归类统计明细
        /// </summary>
        public const string ExportClassifiedStatistics = "Content\\templates\\归类任务统计明细表模板.xlsx";

        /// <summary>
        /// 财务做账-导出收款统计信息
        /// </summary>
        public const string ExportReceiptStatistics = "Content\\templates\\财务做账-收款统计表模板.xlsx";

        /// <summary>
        /// 财务做账-导出换汇统计信息
        /// </summary>
        public const string ExportSwapStatistics = "Content\\templates\\财务做账-换汇统计表模板.xlsx";

        /// <summary>
        /// 风控-导出未收款订单模板
        /// </summary>
        public const string ExportUnReceipts = "Content\\templates\\风控-未收款订单模板.xlsx";

        /// <summary>
        /// 综合-导出报关量统计模板
        /// </summary>
        public const string ExportClientDeclareTotal = "Content\\templates\\综合-导出报关量统计模板.xlsx";

        /// <summary>
        /// 综合-导出业务量统计模板
        /// </summary>
        public const string ExportSalesDeclareTotal = "Content\\templates\\综合-导出业务量统计模板.xlsx";

        /// <summary>
        /// 删除报文
        /// </summary>
        public const string MessageDelete = "03";

        public const string Postfix = ".zip";

        /// <summary>
        /// 费用申请上传的附件
        /// </summary>
        public const string Cost = "Cost";

        /// <summary>
        /// 垫资申请上传的附件
        /// </summary>
        public const string AdvanceMoney = "AdvanceMoney";

        /// <summary>
        /// 公告板上传图片
        /// </summary>
        public const string NoticeBoard = "NoticeBoard";
        /// <summary>
        /// 顺丰
        /// </summary>
        public const string SF = "SF";
    }

    public class Icgoo
    {
        /// <summary>
        /// 默认创建人
        /// 正式环境 Admin0000000282
        /// 开发环境 Admin0000000077
        /// 测试环境 Admin0000000229
        /// </summary>
        public const string DefaultCreator = "XDTAdmin";

        /// <summary>
        /// Icgoo接口认证
        /// </summary>
        public const string IFAuthorization = "basic YW5kYWVsZWM6YmlnYmFuZzIwMTg=";

        /// <summary>
        /// 认证名
        /// </summary>
        public const string IFAuthorizationName = "Authorization";

        /// <summary>
        /// 请求超时事件
        /// </summary>
        public const int RequestOvertime = 90000;

        /// <summary>
        /// Post正式地址 "http://package.dzji.com/api_recept_json_data/";
        /// Post测试地址 "http://baoguan.k0v.cn/api_recept_json_data/";
        /// </summary>
        //public const string PostUrl = "http://package.dzji.com/api_recept_json_data/";

        /// <summary>
        /// 发短信地址
        /// </summary>
        public const string MessageAddressUrl = "http://cf.51welink.com/submitdata/Service.asmx/g_Submit?sname=dlydcx00&spwd=rYfl76qL&scorpid=&sprdid=1012818&sdst={0}&smsg={1}";

        /// <summary>
        /// Icgoo库位号
        /// </summary>
        public const string ShelveNumber = "IcgooLocation";

        /// <summary>
        /// 内单库位号
        /// </summary>
        public const string InsideShelveNumber = "InsideLocation";


        /// <summary>
        /// Icgoo包装类型
        /// </summary>
        public const string WrapType = "22";

        /// <summary>
        /// 不能识别的国家(长度要小于3)
        /// </summary>
        public const string UnknownCountry = "NG";

        /// <summary>
        /// 预处理一
        /// </summary>
        public const string First = "first";

        /// <summary>
        /// 预处理二
        /// </summary>
        public const string Second = "second";

        /// <summary>
        /// Icgoo下单的重量是g
        /// </summary>
        public const decimal UnitConvert = 1000;

        public const decimal BaseGWPara = 1.1M;
        /// <summary>
        /// 成交单位固定为个
        /// </summary>
        public const string Gunit = "007";

        /// <summary>
        /// 队列名
        /// </summary>
        public const string IcgooTariffDiffMQName = "IcgooTariffDiff";
    }

    public class InsideOrder
    {/// <summary>
     /// 内单接口地址
     /// </summary>
        public const string GetUrl = "http://210.51.190.36:810/ApplyCustom/GetApplyCustomInfoByID?id=idpara&key=YgycwGvB8Sv/UrpMpMXQfdPeAnbLFmevccNVL3OTww=";

        /// <summary>
        /// 增值税
        /// </summary>
        public const decimal ValueAddedTaxRate = 0.13M;

        /// <summary>
        /// 消费税
        /// </summary>
        public const decimal ExciseTax = 0M;

        /// <summary>
        /// 内单单号
        /// </summary>
        public const string GetInsideOrderNoUrl = "http://210.51.190.36:810/ApplyCustom/GetApplyCustomIDS?stime=timepara&key=YgycwGvB8Sv/UrpMpMXQfdPeAnbLFmevccNVL3OTww=";
    }

    public class SmsConfig
    {
        /// <summary>
        /// 短信发送地址
        /// </summary>
        public const string MessageAddress = "http://cf.51welink.com/submitdata/Service.asmx/g_Submit?sname=dlydcx00&spwd=rYfl76qL&scorpid=&sprdid=1012818&sdst={0}&smsg={1}";
    }

    public class InvoiceXmlConfig
    {
        //版本号
        public const string Version = "2.0";
        //复核人
        public const string Fhr = "鲁亚慧";
        //收款人
        public const string Skr = "郝红梅";
        //开票人
        public const string Kpr = "殷菲";
        //商品编码版本号(20 字节)（商品版本号是你开票机的版本号）
        public const string Spbmbbh = "33.0";
        //含税标志 0：不含税税率，1：含税税率，2：差额税;中外合作油气田（原海洋石油）5%税率、1.5%税率为 1，差额税为 2，其他为 0；
        public const string Hsbz = "1";

        /// <summary>
        /// 每张发票的限额
        /// </summary>
        public static decimal XianEPerFp = decimal.Parse(PurchaserContext.Current.MaxAmountPerInvoice);
    }

    public class ClassifyConfig
    {
        public const string juniorDeclarant = "芯达通_初级报关员";
    }

    /// <summary>
    /// 库房ID，与代仓储通讯使用
    /// </summary>
    public class WarehouseConfig
    {
        //香港万路通库房
        public const string HK_WLT = "HK01";
        //香港畅运库房
        public const string HK_CY = "HK01";
        //深圳芯达通库房
        public const string SZ_XDT = "SZ01";
        //深圳创新恒远库房
        public const string SZ_CXHY = "SZ02";
        //深圳科睿鑫汇库房
        public const string SZ_KRXH = "SZ03";
    }

    /// <summary>
    /// 收付款对接大赢家配置
    /// </summary>
    public class DyjCwConfig
    {
        public const string Key = "8c2b75ad115b467a8e976123033319f2";

        //芯达通出纳 郝红梅
        public const string uid = "632";

        #region  收款接口地址

        //获取账户信息列表
        public const string GetZhangHuList = "/api/ShouKuan/GetZhangHuList";
        //根据账户id获取账户信息
        public const string GetZhangHuInfo = "/api/ShouKuan/GetZhangHuInfo";
        //修改账户信息
        public const string UpZhangHuInfo = "/api/ShouKuan/UpZhangHuInfo";
        //收款类型
        public const string GetShouKuanTypeList = "/api/ShouKuan/GetShouKuanTypeList";
        //收款凭证列表
        public const string GetShouKuanList = "/api/ShouKuan/GetShouKuanList";
        //收款凭证明细信息
        public const string GetShouKuanDetailList = "/api/ShouKuan/GetShouKuanDetailList";
        //新增收款
        public const string SetShouKuan = "/api/ShouKuan/SetShouKuan";

        #endregion


        #region 付款接口地址

        //基础付款数据信息
        public const string GetFeeApplyABaseInfo = "/api/XDTSFK/GetFeeApplyABaseInfo";

        //供应商信息
        public const string GetFeeApplyProviderInfo = "/api/XDTSFK/GetFeeApplyProviderInfo";

        //付款申请列表
        public const string GetFeeApplyAList = "/api/XDTSFK/GetFeeApplyAList";

        //付款申请明细信息
        public const string GetFeeApplyAllByID = "/api/XDTSFK/GetFeeApplyAllByID";

        //新增费用单据 Post
        public const string SetFeeApplyA = "/api/XDTSFK/SetFeeApplyA";

        //审核 Post
        public const string CheckFeeApplyA = "/api/XDTSFK/CheckFeeApplyA";

        //删除 Post
        public const string DeleteFeeApplyA = "/api/XDTSFK/DeleteFeeApplyA";

        //不能付款 Post
        public const string PayFeeApplyANoPass = "/api/XDTSFK/PayFeeApplyANoPass";

        //付款 Post
        public const string PayFeeApplyAPass = "/api/XDTSFK/PayFeeApplyAPass";


        //汇兑信息(付汇申请新增)
        public const string SetFeeApplyAHD = "/api/XDTSFK/SetFeeApplyAHD";

        //汇兑收款公司信息
        public const string GetFeeApplyHDClientInfo = "/api/XDTSFK/GetFeeApplyHDClientInfo";

        #endregion


    }
}
