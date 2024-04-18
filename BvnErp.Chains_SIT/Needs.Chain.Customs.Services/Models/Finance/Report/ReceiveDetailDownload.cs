using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class ReceiveDetailDownload
    {
        public List<OrderReceiveDetail> Items { get; set; }

        public DataTable ToDataTable()
        {
            #region 定义DT
            DataTable dt = new DataTable();
            dt.Columns.Add("ReceiveDate");
            dt.Columns.Add("ClientName");
            dt.Columns.Add("InvoiceTypeName");
            dt.Columns.Add("ReceiveAmount");
            dt.Columns.Add("ClearAmount");
            dt.Columns.Add("OrderID");
            dt.Columns.Add("ContrNo");
            dt.Columns.Add("AddedValueTax");
            dt.Columns.Add("ExciseTax");
            dt.Columns.Add("Tariff");
            dt.Columns.Add("ShowAgencyFee");
            dt.Columns.Add("GoodsAmount");
            dt.Columns.Add("PaymentExchangeRate");
            dt.Columns.Add("FCAmount");
            dt.Columns.Add("RealExchangeRate");
            dt.Columns.Add("DueGoods");
            dt.Columns.Add("Gains");
            dt.Columns.Add("FinanceReceiptID");
            #endregion

            #region 第一行
            DataRow drTitle = dt.NewRow();
            drTitle["ReceiveDate"] = "收款日期";
            drTitle["ClientName"] = "客户名称";
            drTitle["InvoiceTypeName"] = "客户类型";
            drTitle["ReceiveAmount"] = "收款金额";
            drTitle["ClearAmount"] = "已确认金额";
            drTitle["OrderID"] = "订单编号";
            drTitle["ContrNo"] = "合同号";
            drTitle["AddedValueTax"] = "增值税";
            drTitle["ExciseTax"] = "消费税";
            drTitle["Tariff"] = "关税";
            drTitle["ShowAgencyFee"] = "代理费";
            drTitle["GoodsAmount"] = "货款";
            drTitle["PaymentExchangeRate"] = "付款汇率";
            drTitle["FCAmount"] = "外币金额";
            drTitle["RealExchangeRate"] = "实时汇率";
            drTitle["DueGoods"] = "应收账款-货款";
            drTitle["Gains"] = "损益";
            drTitle["FinanceReceiptID"] = "收款ID";

            dt.Rows.Add(drTitle);
            #endregion

            Items = this.Items.OrderBy(t => t.FinanceReceiptID).OrderBy(t => t.OrderID).ToList();

            decimal? totalAddedValueTax=0M, totalExciseTax = 0M, totalTariff = 0M, totalAgencyFee = 0M, totalGoodsAmount = 0M, totalDueGoods = 0M, totalGains = 0M;

            string FinanceReceiptID = "";
            foreach (var item in this.Items)
            {
                if(FinanceReceiptID == "")
                {
                    FinanceReceiptID = item.FinanceReceiptID;
                }

                if (FinanceReceiptID==item.FinanceReceiptID)
                {
                    DataRow dr = dt.NewRow();
                    dr["ReceiveDate"] = item.ReceiveDate.ToString("yyyy-MM-dd");
                    dr["ClientName"] = item.Client.Company.Name;
                    dr["InvoiceTypeName"] = item.InvoiceTypeName;
                    dr["ReceiveAmount"] = item.ReceiveAmount;
                    dr["ClearAmount"] = item.ClearAmount;
                    dr["OrderID"] = item.OrderID;
                    dr["ContrNo"] = item.ContrNo;
                    dr["AddedValueTax"] = item.AddedValueTax;
                    dr["ExciseTax"] = item.ExciseTax;
                    dr["Tariff"] = item.Tariff;
                    dr["ShowAgencyFee"] = item.ShowAgencyFee;
                    dr["GoodsAmount"] = item.GoodsAmount;
                    dr["PaymentExchangeRate"] = item.PaymentExchangeRate;
                    dr["FCAmount"] = item.FCAmount;
                    dr["RealExchangeRate"] = item.RealExchangeRate;
                    dr["DueGoods"] = item.DueGoods;
                    dr["Gains"] = item.Gains;
                    dr["FinanceReceiptID"] = item.FinanceReceiptID;
                    dt.Rows.Add(dr);

                    totalAddedValueTax += item.AddedValueTax;
                    totalExciseTax += item.ExciseTax;
                    totalTariff += item.Tariff;
                    totalAgencyFee += item.ShowAgencyFee;
                    totalGoodsAmount += item.GoodsAmount;
                    totalDueGoods += item.DueGoods;
                    totalGains += item.Gains;
                }
                else
                {
                    DataRow drSum = dt.NewRow();
                    drSum["ReceiveDate"] = "合计";
                    drSum["ClientName"] = "";
                    drSum["InvoiceTypeName"] = "";
                    drSum["ReceiveAmount"] = "";
                    drSum["ClearAmount"] = "";
                    drSum["OrderID"] = "";
                    drSum["ContrNo"] = "";
                    drSum["AddedValueTax"] = totalAddedValueTax;
                    drSum["ExciseTax"] = totalExciseTax;
                    drSum["Tariff"] = totalTariff;
                    drSum["ShowAgencyFee"] = totalAgencyFee;
                    drSum["GoodsAmount"] = totalGoodsAmount;
                    drSum["PaymentExchangeRate"] = "";
                    drSum["FCAmount"] = "";
                    drSum["RealExchangeRate"] = "";
                    drSum["DueGoods"] = totalDueGoods;
                    drSum["Gains"] = totalGains;
                    drSum["FinanceReceiptID"] = "";
                    dt.Rows.Add(drSum);

                    FinanceReceiptID = item.FinanceReceiptID;
                    totalAddedValueTax = 0;
                    totalExciseTax = 0;
                    totalTariff = 0;
                    totalAgencyFee = 0;
                    totalGoodsAmount = 0;
                    totalDueGoods = 0;
                    totalGains = 0;

                    DataRow dr = dt.NewRow();
                    dr["ReceiveDate"] = item.ReceiveDate.ToString("yyyy-MM-dd");
                    dr["ClientName"] = item.Client.Company.Name;
                    dr["InvoiceTypeName"] = item.InvoiceTypeName;
                    dr["ReceiveAmount"] = item.ReceiveAmount;
                    dr["ClearAmount"] = item.ClearAmount;
                    dr["OrderID"] = item.OrderID;
                    dr["ContrNo"] = item.ContrNo;
                    dr["AddedValueTax"] = item.AddedValueTax;
                    dr["ExciseTax"] = item.ExciseTax;
                    dr["Tariff"] = item.Tariff;
                    dr["ShowAgencyFee"] = item.ShowAgencyFee;
                    dr["GoodsAmount"] = item.GoodsAmount;
                    dr["PaymentExchangeRate"] = item.PaymentExchangeRate;
                    dr["FCAmount"] = item.FCAmount;
                    dr["RealExchangeRate"] = item.RealExchangeRate;
                    dr["DueGoods"] = item.DueGoods;
                    dr["Gains"] = item.Gains;
                    dr["FinanceReceiptID"] = item.FinanceReceiptID;
                    dt.Rows.Add(dr);

                    totalAddedValueTax += item.AddedValueTax;
                    totalExciseTax += item.ExciseTax;
                    totalTariff += item.Tariff;
                    totalAgencyFee += item.ShowAgencyFee;
                    totalGoodsAmount += item.GoodsAmount;
                    totalDueGoods += item.DueGoods;
                    totalGains += item.Gains;
                }               
            }

            DataRow drSum2 = dt.NewRow();
            drSum2["ReceiveDate"] = "合计";
            drSum2["ClientName"] = "";
            drSum2["InvoiceTypeName"] = "";
            drSum2["ReceiveAmount"] = "";
            drSum2["ClearAmount"] = "";
            drSum2["OrderID"] = "";
            drSum2["ContrNo"] = "";
            drSum2["AddedValueTax"] = totalAddedValueTax;
            drSum2["ExciseTax"] = totalExciseTax;
            drSum2["Tariff"] = totalTariff;
            drSum2["ShowAgencyFee"] = totalAgencyFee;
            drSum2["GoodsAmount"] = totalGoodsAmount;
            drSum2["PaymentExchangeRate"] = "";
            drSum2["FCAmount"] = "";
            drSum2["RealExchangeRate"] = "";
            drSum2["DueGoods"] = totalDueGoods;
            drSum2["Gains"] = totalGains;
            drSum2["FinanceReceiptID"] = "";
            dt.Rows.Add(drSum2);

            return dt;
        }

        public void toExcel(string savePath, DataTable dt)
        {
            //建立空白工作簿
            XSSFWorkbook wb = new XSSFWorkbook();
            ISheet sheet = wb.CreateSheet("Sheet1");
            IRow row;
            NPOI.SS.UserModel.ICell cell;

            #region 设置单元格边框
            ICellStyle borderstyle = wb.CreateCellStyle();
            borderstyle.BorderBottom = BorderStyle.Thin;
            borderstyle.BorderLeft = BorderStyle.Thin;
            borderstyle.BorderRight = BorderStyle.Thin;
            borderstyle.BorderTop = BorderStyle.Thin;
            #endregion

            int imerge = -2;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                row = sheet.CreateRow(i);
                cell = row.CreateCell(0);
                cell.SetCellValue(dt.Rows[i]["ReceiveDate"].ToString());
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(1);
                cell.SetCellValue(dt.Rows[i]["ClientName"].ToString());
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(2);
                cell.SetCellValue(dt.Rows[i]["InvoiceTypeName"].ToString());
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(3);
                cell.SetCellValue(dt.Rows[i]["ReceiveAmount"].ToString());
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(4);
                cell.SetCellValue(dt.Rows[i]["ClearAmount"].ToString());
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(5);
                cell.SetCellValue(dt.Rows[i]["OrderID"].ToString() == "" ? "-" : dt.Rows[i]["OrderID"].ToString());
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(6);
                cell.SetCellValue(dt.Rows[i]["ContrNo"].ToString() == "" ? "-" : dt.Rows[i]["ContrNo"].ToString());
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(7);
                cell.SetCellValue(dt.Rows[i]["AddedValueTax"].ToString() == "" ? "-" : dt.Rows[i]["AddedValueTax"].ToString());
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(8);
                cell.SetCellValue(dt.Rows[i]["ExciseTax"].ToString() == "" ? "-" : dt.Rows[i]["ExciseTax"].ToString());
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(9);
                cell.SetCellValue(dt.Rows[i]["Tariff"].ToString() == "" ? "-" : dt.Rows[i]["Tariff"].ToString());
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(10);
                cell.SetCellValue(dt.Rows[i]["ShowAgencyFee"].ToString() == "" ? "-" : dt.Rows[i]["ShowAgencyFee"].ToString());
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(11);
                cell.SetCellValue(dt.Rows[i]["GoodsAmount"].ToString() == "" ? "-" : dt.Rows[i]["GoodsAmount"].ToString());
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(12);
                cell.SetCellValue(dt.Rows[i]["PaymentExchangeRate"].ToString() == "" ? "-" : dt.Rows[i]["PaymentExchangeRate"].ToString());
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(13);
                cell.SetCellValue(dt.Rows[i]["FCAmount"].ToString() == "" ? "-" : dt.Rows[i]["FCAmount"].ToString());
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(14);
                cell.SetCellValue(dt.Rows[i]["RealExchangeRate"].ToString() == "" ? "-" : dt.Rows[i]["RealExchangeRate"].ToString());
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(15);
                cell.SetCellValue(dt.Rows[i]["DueGoods"].ToString() == "" ? "-" : dt.Rows[i]["DueGoods"].ToString());
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(16);
                cell.SetCellValue(dt.Rows[i]["Gains"].ToString() == "" ? "-" : dt.Rows[i]["Gains"].ToString());
                cell.CellStyle = borderstyle;

                cell = row.CreateCell(17);
                cell.SetCellValue(dt.Rows[i]["FinanceReceiptID"].ToString() == "" ? "-" : dt.Rows[i]["FinanceReceiptID"].ToString());
                cell.CellStyle = borderstyle;

                imerge++;

                if (dt.Rows[i]["ReceiveDate"].ToString() == "合计")
                {
                    sheet.AddMergedRegion(new CellRangeAddress(i, i, 0, 5));
                    int firstrow = i - imerge;                    
                    int lastrow = i - 1;
                    sheet.AddMergedRegion(new CellRangeAddress(firstrow, lastrow, 0, 0));
                    sheet.AddMergedRegion(new CellRangeAddress(firstrow, lastrow, 1, 1));
                    sheet.AddMergedRegion(new CellRangeAddress(firstrow, lastrow, 2, 2));
                    sheet.AddMergedRegion(new CellRangeAddress(firstrow, lastrow, 3, 3));
                    imerge = -1;
                }                               
            }

            FileStream file = new FileStream(savePath, FileMode.Create);
            wb.Write(file);
            file.Close();
        }
    }
}
