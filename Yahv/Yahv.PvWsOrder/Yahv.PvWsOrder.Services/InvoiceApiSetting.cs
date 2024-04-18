using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvWsOrder.Services
{
    public class InvoiceApiSetting
    {
        public static string ApiName = "InvoiceApiUrl";
        /// <summary>
        /// 调用dyj生成xml
        /// </summary>
        public static string GenerateXmlUrl = "services/core.ashx";

        public static string ServiceName = "InvoiceFlowHandle";

        public static string XmlRequestItem = "申请开票";

        public static string XmlRequestRecord = "申请记录";

        public static string XmlRequestDetail = "申请明细";

    }
}

#region 生成做账

public class MakeAccountSetting
{

    public static string ApiName = "InvoiceApiUrl";

    public static string MakeAccountHandle = "services/core.ashx";

    public static string request_service = "SmartVoucherHandle";

    public static string request_item = "导入凭证原始数据";

    public static string request_wordNo = "凭证生成结果";

    public static string token = "0A381764-73FD-4B53-99FB-DCEBE8ABE0DE";

    public static string 归属账套 = "深圳市芯达通供应链管理有限公司";

    //一、报关进口-全额发票类型
    //二、报关进口-服务费发票类型
    public static string DeclareImport_归属模板编号 = "103";
    public static string DeclareImport_归属方案编号 = "14";

    //三、发货-服务费发票类型
    public static string DeclareOutService_归属模板编号 = "231";
    public static string DeclareOutService_归属方案编号 = "22";

    // 承兑汇票 收承兑
    public static string AccImport_归属模板编号 = "240";
    public static string AccImport_归属方案编号 = "46";

    // 承兑汇票 收承兑
    public static string BuyImport_归属模板编号 = "241";
    public static string BuyImport_归属方案编号 = "47";

    // 全额发票
    public static string FullInvoImport_归属模板编号 = "102";
    public static string FullInvoImport_归属方案编号 = "19";

    // 服务费发票
    public static string ServiceInvoImport_归属模板编号 = "235";
    public static string ServiceInvoImport_归属方案编号 = "20";

    // 缴纳报关进口关税增值税
    public static string FinancePImport_归属模板编号 = "232";
    public static string FinancePImport_归属方案编号 = "15";

    // 扫码提现
    public static string QRImport_归属模板编号 = "239";
    public static string QRImport_归属方案编号 = "45";

    // 银行往来
    public static string FundTransImport_归属模板编号 = "236";
    public static string FundTransImport_归属方案编号 = "42";

    //收款-全额发票
    public static string ReFullImport_归属模板编号 = "243";
    public static string ReFullImport_归属方案编号 = "48";

    //收款-服务费发票
    public static string ReSerImport_归属模板编号 = "244";
    public static string ReSerImport_归属方案编号 = "49";

    //手续费
    public static string PoundageImport_归属模板编号 = "237";
    public static string PoundageImport_归属方案编号 = "43";

    //货款
    public static string GoodsImport_归属模板编号 = "238";
    public static string GoodsImport_归属方案编号 = "44";

    //换汇
    public static string SwapImport_归属模板编号 = "245";
    public static string SwapImport_归属方案编号 = "51";
}

#endregion
