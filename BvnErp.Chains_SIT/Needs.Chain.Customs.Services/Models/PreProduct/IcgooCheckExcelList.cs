using Needs.Ccs.Services.Enums;
using Needs.Linq;
using Needs.Utils.Npoi;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class IcgooCheckExcelList : IUnique
    {
        public string ID { get; set; }
        public List<DecHead> Heads { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string Origin { get; set; }
        public OrderType OrderType { get; set; }
        public IcgooCheckExcelList(List<DecHead> heads,string origin)
        {
            this.Heads = heads;
            this.Origin = origin;
        }

        public void setFilePath(string filePath)
        {
            this.FilePath = filePath;
        }

        public IWorkbook toExcel()
        {
            IWorkbook workBook = ExcelFactory.Create(ExcelFactory.ExcelVersion.Excel07);
            NPOIHelper npoi = new NPOIHelper(workBook);

            var ClassifyProductView = new Needs.Ccs.Services.Views.Alls.ClassifyProductsAll();
            var dataExcel = new List<IcgooCheckExcel>();            
                 
            #region 数据填充
            foreach (var head in this.Heads)
            {              
                var order = new Needs.Ccs.Services.Views.OrdersView()[head.OrderID];              
                if(order.Type== this.OrderType)
                {
                    var ProductFeeExchangeRate = order.ProductFeeExchangeRate;

                    List<DecList> HeadData = head.Lists.ToList();
                    if (!string.IsNullOrEmpty(this.Origin))
                    {
                        HeadData = HeadData.Where(item => item.OriginCountry == Origin).ToList();
                    }

                    //var products = ClassifyProductView.Where(item => HeadData.Select(a => a.OrderItemID).Contains(item.ID)).ToArray();
                    var orderItemIds = HeadData.Select(a => a.OrderItemID).ToList();

                    List<LambdaExpression> lamdasOrderByAscDateTime = new List<LambdaExpression>();
                    lamdasOrderByAscDateTime.Add((Expression<Func<ClassifyProduct, DateTime>>)(t => t.CreateDate));

                    var products = ClassifyProductView.GetTop(
                        orderItemIds.Count(), 
                        item => orderItemIds.Contains(item.ID),
                        lamdasOrderByAscDateTime.ToArray(),
                        null
                        ).ToArray();

                    var singleData = from headdata in HeadData
                                     join product in products on headdata.OrderItemID equals product.ID
                                     select new IcgooCheckExcel
                                     {
                                         CompanyName = head.OwnerName,
                                         ContactNo = head.ContrNo,
                                         customcode = head.EntryId,
                                         GoodsNO = "",
                                         Model = headdata.GoodsModel,
                                         ProductName = headdata.GName,
                                         Brand = headdata.GoodsBrand,
                                         Origin = headdata.OriginCountry,
                                         Qty = headdata.GQty,
                                         UnitPrice = headdata.DeclPrice,
                                         TotalPrice = headdata.DeclTotal,
                                         CustomExchangeRate = order.CustomsExchangeRate,
                                         RealExchangeRate = order.RealExchangeRate,
                                         TotalRMBPrice = headdata.DeclTotal * ProductFeeExchangeRate,
                                         Tariff = product.ImportTax?.Value,
                                         TairffRate = product.ImportTax?.Rate,
                                         ExciseTax = product.ExciseTax?.Value,
                                         ExciseTaxRate = product.ExciseTax?.Rate,
                                         ValueAdded = product.AddedValueTax?.Value,
                                         ValueAddedRate = product.AddedValueTax?.Rate,
                                         HS_Code = product.Category?.HSCode,
                                         CreateTime = head.CreateTime,
                                     };
                   
                    dataExcel.AddRange(singleData);
                }               
            }
          
            #endregion

            var columnNameData = dataExcel.Select(item => new
            {
                公司名称=item.CompanyName,
                合同号=item.ContactNo,
                海关号码=item.customcode,
                交货单号=item.GoodsNO,
                型号=item.Model,
                产品名称=item.ProductName,
                品牌=item.Brand,
                产地=item.Origin,
                数量=item.Qty,
                外币单价=item.UnitPrice,
                货值=item.TotalPrice,
                当月海关汇率=item.CustomExchangeRate,
                货值RMB=item.TotalRMBPrice,
                实际关税=item.Tariff,
                关税率=item.TairffRate,
                实际消费税=item.ExciseTax?.ToString("0.####") ?? "0.0000",
                消费税率=item.ExciseTaxRate?.ToString("0.####") ?? "0.0000",
                实际增值税=item.ValueAdded,
                增值税率=item.ValueAddedRate,
                海关税号=item.HS_Code,
                报关日期=item.CreateTime,
            });

            int[] columnWidth = {10,10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
            npoi.EnumrableToExcel(columnNameData, 0, columnWidth);

            return workBook;
        }
     
        public string[] SaveAs(string fileName)
        {
            this.FileName = fileName;
            var result = new string[3];
            string ExportUrl = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.FilePath + this.FileName);
            try
            {                              
                 IWorkbook doc = this.toExcel();
              
                using (FileStream file = new FileStream(ExportUrl, FileMode.OpenOrCreate))
                {
                    result[0] = this.FileName;
                    result[1] = "";
                    result[2] = file.Length.ToString();

                    doc.Write(file);
                    file.Close();
                }
                //this.OnExceled();
            }
            catch (Exception ex)
            {

            }

            return result;
        }
    }
}
