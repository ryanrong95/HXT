using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.InvoiceService.Services.InvVeri
{
    public class ResultModelApi
    {
        /// <summary>
        /// 提示信息，resultCode为1000返回：查验结果成功，resultCode为2001返回对应invoicefalseCode的错误信息
        /// </summary>
        public string resultMsg { get; set; }

        /// <summary>
        /// 发票名称
        /// </summary>
        public string invoiceName { get; set; }

        /// <summary>
        /// 查询是否免费，Y：是，N：否
        /// </summary>
        public string isFree { get; set; }

        /// <summary>
        /// 00：成功，99：失败
        /// </summary>
        public string RtnCode { get; set; }

        /// <summary>
        /// 查询发票状态码，1000：查询到票的信息，2001：没有查询到票的信息
        /// </summary>
        public string resultCode { get; set; }

        /// <summary>
        /// 数据查询结果，详情请查看返回业务参数
        /// </summary>
        public string invoiceResult { get; set; }

        /// <summary>
        /// 失败状态码，如果resultCode为1000，该字段不返回，如果resultCode为2001，会返回不同类型错误码
        /// </summary>
        public string invoicefalseCode { get; set; }
    }

    public class InvoiceResultApi
    {
        /// <summary>
        /// 发票类型名称
        /// </summary>
        public string invoiceTypeName { get; set; }

        /// <summary>
        /// 发票类型，01：增值税专票，03：机动车销售统一发票，04：增值税普通发票，10：电子发票，11：卷式普通发票，14:电子普通[通行费]发票，15：二手车统一发票
        /// </summary>
        public string invoiceTypeCode { get; set; }

        /// <summary>
        /// 查询日期
        /// </summary>
        public string checkDate { get; set; }

        /// <summary>
        /// 查验次数
        /// </summary>
        public string checkNum { get; set; }

        /// <summary>
        /// 发票代码
        /// </summary>
        public string invoiceDataCode { get; set; }

        /// <summary>
        /// 发票号码
        /// </summary>
        public string invoiceNumber { get; set; }

        /// <summary>
        /// 机打号码
        /// </summary>
        public string machineNumber { get; set; }

        /// <summary>
        /// 开票时间
        /// </summary>
        public string billingTime { get; set; }

        /// <summary>
        /// 购方名称
        /// </summary>
        public string purchaserName { get; set; }

        /// <summary>
        /// 购方纳税人识别号
        /// </summary>
        public string taxpayerNumber { get; set; }

        /// <summary>
        /// 购方地址，电话
        /// </summary>
        public string taxpayerAddressOrId { get; set; }

        /// <summary>
        /// 购方银行账号
        /// </summary>
        public string taxpayerBankAccount { get; set; }

        /// <summary>
        /// 机器码
        /// </summary>
        public string taxDiskCode { get; set; }

        /// <summary>
        /// 销方名称
        /// </summary>
        public string salesName { get; set; }

        /// <summary>
        /// 销方纳税人识别号
        /// </summary>
        public string salesTaxpayerNum { get; set; }

        /// <summary>
        /// 销方地址，电话
        /// </summary>
        public string salesTaxpayerAddress { get; set; }

        /// <summary>
        /// 销方银行，账号
        /// </summary>
        public string salesTaxpayerBankAccount { get; set; }

        /// <summary>
        /// 不含税价（金额）
        /// </summary>
        public string totalAmount { get; set; }

        /// <summary>
        /// 税额
        /// </summary>
        public string totalTaxNum { get; set; }

        /// <summary>
        /// 价税合计
        /// </summary>
        public string totalTaxSum { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string invoiceRemarks { get; set; }

        /// <summary>
        /// 收货员（卷式发票新增字段，其他票可以不用）
        /// </summary>
        public string goodsClerk { get; set; }

        /// <summary>
        /// 校验码
        /// </summary>
        public string checkCode { get; set; }

        /// <summary>
        /// 作废标志，0：正常，1：作废
        /// </summary>
        public string voidMark { get; set; }

        /// <summary>
        /// 是否为清单票，Y：是，N：否
        /// </summary>
        public string isBillMark { get; set; }

        /// <summary>
        /// 发票详情
        /// </summary>
        public List<InvoiceDetailDataItemApi> invoiceDetailData { get; set; }

        /// <summary>
        /// 支持票种：普通发票，专用发票，卷式发票，普通增值税（通行费），普通增值税（折叠费）
        /// </summary>
        public string tollSign { get; set; }

        /// <summary>
        /// 收费标志名称（没有为空）
        /// </summary>
        public string tollSignName { get; set; }
    }

    public class InvoiceDetailDataItemApi
    {
        /// <summary>
        /// 行号
        /// </summary>
        public string lineNum { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string goodserviceName { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string model { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string unit { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public string number { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public string price { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public string sum { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        public string taxRate { get; set; }

        /// <summary>
        /// 税额
        /// </summary>
        public string tax { get; set; }

        /// <summary>
        /// 是否为清单行 Y：是，N：否（是的时候，货物名称为“请详见货物清单”，“折扣额合计”，“原价合计”，可以不用做账）
        /// </summary>
        public string isBillLine { get; set; }

        /// <summary>
        /// 零税率标志字段（空:非零税率， 1:税率栏位显示“免税”， 2:税率栏位显示“不征收”， 3:零税率）
        /// </summary>
        public string zeroTaxRateSign { get; set; }

        /// <summary>
        /// 零税率标志名称
        /// </summary>
        public string zeroTaxRateSignName { get; set; }
    }
}
