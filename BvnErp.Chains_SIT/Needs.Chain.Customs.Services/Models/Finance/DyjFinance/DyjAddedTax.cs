using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class DyjAddedTax
    {
    }

    public class DyjAddedTaxResponse
	{ 
    
		public int Code { get; set; }

		public bool Success { get; set; }


		/// <summary>
		/// 发票号
		/// </summary>
		public string ContractNo { get; set; }

		/// <summary>
		/// 开票日期
		/// </summary>
		public string InvoiceDate { get; set; }

		/// <summary>
		/// 总数量
		/// </summary>
		public decimal TotalQuantity { get; set; }

		/// <summary>
		/// 总金额
		/// </summary>
		public decimal TotalAmount { get; set; }

		/// <summary>
		/// 税额
		/// </summary>
		public decimal TaxAmount { get; set; }

		/// <summary>
		/// 发票内容
		/// </summary>
		public List<DyjAddedTaxItem> InvoiceItems { get; set; }

		/// <summary>
		/// 海关增值税票 PDF
		/// "FileUrl":"http://erp8.for-ic.net/A/B/C/XDT202103-22-043.pdf"
		/// </summary>
		public string FileUrl { get; set; }

	}

	public class DyjAddedTaxItem
	{
		/// <summary>
		/// 大赢家型号ID
		/// </summary>
		public string ID { get; set; }

		/// <summary>
		/// 型号
		/// </summary>
		public string Model { get; set; }

		/// <summary>
		/// 品名
		/// </summary>
		public string ProductName { get; set; }

		/// <summary>
		/// 税务名称
		/// </summary>
		public string TaxName { get; set; }

		/// <summary>
		/// 税务编码
		/// </summary>
		public string TaxCode { get; set; }

		/// <summary>
		/// 单位
		/// </summary>
		public string Unit { get; set; }

		/// <summary>
		/// 数量
		/// </summary>
		public decimal Quantity { get; set; }

		/// <summary>
		/// 单价
		/// </summary>
		public decimal UnitPrice { get; set; }

		/// <summary>
		/// 金额
		/// </summary>
		public decimal Amount { get; set; }

		/// <summary>
		/// 税率
		/// </summary>
		public decimal TaxRate { get; set; }

		/// <summary>
		/// 应缴增值税
		/// </summary>
		public decimal AddedTaxValue { get; set; }

		/// <summary>
		/// 实缴增值税
		/// </summary>
		public decimal RealAddedTaxValue { get; set; }
	}
}
