using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Npoi;
using Needs.Utils.Serializers;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.HKWarehouse.Exit
{
    public partial class ExitPrint : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            string subAction = Request.QueryString["subAction"];

            //出库列表(包含各种出库状态)
            if ("exitList" == subAction)
            {
                var exitNotices = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKExitNotice.AsQueryable();

                //过滤掉外单的客户
                exitNotices = from exitNotice in exitNotices
                              where (new OrderType[] { OrderType.Inside, OrderType.Icgoo }).Contains(exitNotice.Order.Type)
                                    && exitNotice.Order.Client.ClientType == ClientType.Internal
                              select exitNotice;

                string ClientName = Request.QueryString["ClientName"];
                string VoyNo = Request.QueryString["VoyNo"];
                string StartDDate = Request.QueryString["StartDDate"];
                string EndDDate = Request.QueryString["EndDDate"];

                if (!string.IsNullOrEmpty(ClientName))
                {
                    exitNotices = exitNotices.Where(t => t.Order.Client.Company.Name.Contains(ClientName));
                }
                if (!string.IsNullOrEmpty(VoyNo))
                {
                    exitNotices = exitNotices.Where(t => t.DecHead.VoyNo.Contains(VoyNo));
                }
                if (!string.IsNullOrEmpty(StartDDate))
                {
                    DateTime dt;
                    if (DateTime.TryParse(StartDDate, out dt))
                    {
                        exitNotices = exitNotices.Where(t => t.DecHead.DDate >= dt);
                    }
                }
                if (!string.IsNullOrEmpty(EndDDate))
                {
                    DateTime dt;
                    if (DateTime.TryParse(EndDDate, out dt))
                    {
                        dt = dt.AddDays(1);
                        exitNotices = exitNotices.Where(t => t.DecHead.DDate < dt);
                    }
                }

                var resultExitNotices = from exitNotice in (from exitNotice in exitNotices
                                                            where exitNotice.DecHead.DDate != null
                                                            select new
                                                            {
                                                                ClientID = exitNotice.Order.Client.ID,
                                                                ClientName = exitNotice.Order.Client.Company.Name,
                                                                VoyNo = exitNotice.DecHead.VoyNo,
                                                                StrDateDDate = exitNotice.DecHead.DDate.ToString().Substring(0, 11),
                                                                DDate = exitNotice.DecHead.DDate,
                                                                PackNo = exitNotice.DecHead.PackNo,
                                                            })
                                        group exitNotice by new { exitNotice.ClientID, exitNotice.VoyNo, exitNotice.StrDateDDate } into g
                                        select new
                                        {
                                            ClientID = g.Key.ClientID,
                                            ClientName = g.FirstOrDefault().ClientName,
                                            VoyNo = g.Key.VoyNo,
                                            StrDateDDate = g.Key.StrDateDDate.ToString(),
                                            DDate = g.FirstOrDefault().DDate,
                                            TotalPackNo = g.Sum(item => item.PackNo),
                                        };

                resultExitNotices = resultExitNotices.OrderByDescending(x => x.DDate);

                this.Paging(resultExitNotices);
            }


        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        protected void Export()
        {
            try
            {
                string clientID = Request.QueryString["ClientID"];
                string voyNo = Request.QueryString["VoyNo"];
                string dDate = Request.QueryString["DDate"];

                DateTime startDDate = DateTime.Parse(dDate);
                DateTime endDDate = startDDate.AddDays(1);

                var exitNotices = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKExitNotice.AsQueryable();

                //过滤掉外单的客户
                exitNotices = from exitNotice in exitNotices
                              where (new OrderType[] { OrderType.Inside, OrderType.Icgoo }).Contains(exitNotice.Order.Type)
                                    && exitNotice.Order.Client.ClientType == ClientType.Internal
                              select exitNotice;

                exitNotices = exitNotices.Where(t => t.Order.Client.ID == clientID && t.DecHead.VoyNo == voyNo
                                                    && t.DecHead.DDate >= startDDate && t.DecHead.DDate < endDDate);

                var resultExitNotices = from exitNotice in (from exitNotice in exitNotices
                                                            where exitNotice.DecHead.DDate != null
                                                            select new
                                                            {
                                                                ClientID = exitNotice.Order.Client.ID,
                                                                ClientName = exitNotice.Order.Client.Company.Name,
                                                                VoyNo = exitNotice.DecHead.VoyNo,
                                                                StrDateDDate = exitNotice.DecHead.DDate.ToString().Substring(0, 11),
                                                                DDate = exitNotice.DecHead.DDate,
                                                                PackNo = exitNotice.DecHead.PackNo,
                                                                Voyage = new Voyage()
                                                                {
                                                                    HKLicense = exitNotice.OutputWayBill.Voyage.HKLicense,
                                                                    DriverCode = exitNotice.OutputWayBill.Voyage.DriverCode,
                                                                    DriverName = exitNotice.OutputWayBill.Voyage.DriverName,
                                                                },
                                                            })
                                        group exitNotice by new { exitNotice.ClientID, exitNotice.VoyNo, exitNotice.StrDateDDate } into g
                                        select new
                                        {
                                            ClientID = g.Key.ClientID,
                                            ClientName = g.FirstOrDefault().ClientName,
                                            VoyNo = g.Key.VoyNo,
                                            StrDateDDate = g.Key.StrDateDDate.ToString(),
                                            DDate = g.FirstOrDefault().DDate,
                                            TotalPackNo = g.Sum(item => item.PackNo),

                                            Voyage = g.FirstOrDefault().Voyage,
                                        };

                var resultExitNotice = resultExitNotices.FirstOrDefault();

                //string voyNo = exitNotice.DecHead.VoyNo;
                string clientName = resultExitNotice.ClientName;
                //string dDate = resultExitNotice.DDate?.ToString("yyyy-MM-dd");
                string hKLicense = resultExitNotice.Voyage.HKLicense ?? string.Empty;
                string driverCode = resultExitNotice.Voyage.DriverCode ?? string.Empty;
                string driverName = resultExitNotice.Voyage.DriverName ?? string.Empty;
                int totalPackNo = resultExitNotice.TotalPackNo;

                var resultExitPrintItem = from exitPrintItems in Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExitPrintItems
                                          where exitPrintItems.HKExitNotice.Order.Client.ID == clientID && exitPrintItems.HKExitNotice.DecHead.VoyNo == voyNo
                                            && exitPrintItems.HKExitNotice.DecHead.DDate >= startDDate && exitPrintItems.HKExitNotice.DecHead.DDate < endDDate
                                          group exitPrintItems by new { exitPrintItems.DecList.CaseNo } into g
                                          select new
                                          {
                                              StockCode = string.Empty,
                                              CaseNo = g.Key.CaseNo,
                                              GoodsModel = string.Empty,
                                              GQtySum = g.Sum(t => t.DecList.GQty),
                                          };
                                          //select new
                                          //{
                                          //    StockCode = exitPrintItems.StoreStorage.StockCode,
                                          //    CaseNo = exitPrintItems.DecList.CaseNo,
                                          //    GoodsModel = exitPrintItems.DecList.GoodsModel,
                                          //    GQty = exitPrintItems.DecList.GQty,
                                          //};

                string fileName = Guid.NewGuid().ToString("N") + ".xlsx";
                FileDirectory file = new FileDirectory(fileName);
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                file.CreateDataDirectory();

                ToExcel(resultExitPrintItem, clientID, clientName, dDate, voyNo, hKLicense, driverCode, driverName, totalPackNo, file.FilePath);

                Response.Write((new { success = true, message = "导出成功（点击确定后开始下载）", url = file.FileUrl }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 转为 Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="clientName"></param>
        /// <param name="dDate"></param>
        /// <param name="voyNo"></param>
        /// <param name="hKLicense"></param>
        /// <param name="driverCode"></param>
        /// <param name="driverName"></param>
        /// <param name="totalPackNo"></param>
        /// <param name="filePath"></param>
        public void ToExcel<T>(IEnumerable<T> query, 
            string clientID,
            string clientName, string dDate, string voyNo, string hKLicense, string driverCode, string driverName, int totalPackNo,
            string filePath)
        {
            IWorkbook WorkBook = ExcelFactory.Create(ExcelFactory.ExcelVersion.Excel07);
            ISheet Sheet = WorkBook.NumberOfSheets == 0 ? WorkBook.CreateSheet("Sheet1") : WorkBook.GetSheetAt(0);

            var vendor = new VendorContext(VendorContextInitParam.ClientID, clientID).Current1;

            //大标题
            SetInfoInExcel(Sheet, 0, 0, 9, $"{vendor.ShortName}库房出库单", 25, CreateHeadStyle(WorkBook));
            //客户名称
            SetInfoInExcel(Sheet, 1, 0, 9, "客户名称：" + clientName, 17, CreateSecInfoStyle(WorkBook));
            //报关日期
            SetInfoInExcel(Sheet, 2, 0, 9, "报关日期：" + dDate, 17, CreateSecInfoStyle(WorkBook));

            ICellStyle thirdInfoStyle = CreateThirdInfoStyle(WorkBook);

            int rownum = 4;
            IRow rowa = Sheet.CreateRow(rownum);
            //运输批次号
            SetInfoInExcel(Sheet, rownum, 0, 1, "运输批次号：", 15, thirdInfoStyle, rowa);
            SetInfoInExcel(Sheet, rownum, 2, 4, voyNo, 15, thirdInfoStyle, rowa);
            //总件数
            SetInfoInExcel(Sheet, rownum, 5, 6, "总件数：", 15, thirdInfoStyle, rowa);
            SetInfoInExcel(Sheet, rownum, 7, 9, Convert.ToString(totalPackNo), 15, thirdInfoStyle, rowa);

            rownum = 5;
            IRow rowb = Sheet.CreateRow(rownum);
            //车牌
            SetInfoInExcel(Sheet, rownum, 0, 1, "车牌：", 15, thirdInfoStyle, rowb);
            SetInfoInExcel(Sheet, rownum, 2, 4, hKLicense, 15, thirdInfoStyle, rowb);
            //总数量
            SetInfoInExcel(Sheet, rownum, 5, 6, "总数量：", 15, thirdInfoStyle, rowb);
            SetInfoInExcel(Sheet, rownum, 7, 9, "", 15, thirdInfoStyle, rowb);

            rownum = 6;
            IRow rowc = Sheet.CreateRow(rownum);
            //司机姓名
            SetInfoInExcel(Sheet, rownum, 0, 1, "司机姓名：", 15, thirdInfoStyle, rowc);
            SetInfoInExcel(Sheet, rownum, 2, 4, driverName, 15, thirdInfoStyle, rowc);

            rownum = 8;
            ICellStyle tableTitleStyle = CreateTableTitleStyle(WorkBook);
            IRow rowTableTitle = Sheet.CreateRow(rownum);
            SetInfoInExcel(Sheet, rownum, 0, 1, "序号", 15, tableTitleStyle, rowTableTitle);
            SetInfoInExcel(Sheet, rownum, 2, 5, "箱号", 15, tableTitleStyle, rowTableTitle);
            SetInfoInExcel(Sheet, rownum, 6, 9, "数量", 15, tableTitleStyle, rowTableTitle);

            var properties = typeof(T).GetProperties();

            int index = 1;
            decimal totalGQty = 0;
            foreach (var q in query)
            {
                ICellStyle tableBodyStyle = CreateTableBodyStyle(WorkBook);
                IRow rowTableBody = Sheet.CreateRow(rownum + index);
                SetInfoInExcel(Sheet, rownum + index, 0, 1, Convert.ToString(index), 15, tableBodyStyle, rowTableBody);
                SetInfoInExcel(Sheet, rownum + index, 2, 5, GetValue(q, properties, 1), 15, tableBodyStyle, rowTableBody);
                SetInfoInExcel(Sheet, rownum + index, 6, 9, decimal.Parse(GetValue(q, properties, 3)).ToString("#.#####"), 15, tableBodyStyle, rowTableBody);

                index++;
                totalGQty += decimal.Parse(GetValue(q, properties, 3));
            }

            //修改总数量的值
            Sheet.GetRow(5).GetCell(7).SetCellValue(totalGQty.ToString("#.#####"));

            //设置列宽
            int[] columnWidth = { 6, 10, 10, 10, 10, 6, 10, 6, 6, 10, };
            for (int i = 0; i < columnWidth.Length; i++)
            {
                Sheet.SetColumnWidth(i, 256 * columnWidth[i]);
            }

            //保存文件
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {

                if (WorkBook != null)
                {
                    WorkBook.Write(fs);
                    fs.Close();
                    WorkBook.Close();
                }
                //根据模板生成且模板路径有误
                else
                {
                    throw new Exception("保存失败");

                }
            }
        }

        private string GetValue<T>(T q, PropertyInfo[] properties, int j)
        {
            var value = properties[j].GetValue(q);
            if (value == null)
            {
                return string.Empty;
            }
            else
            {
                return value.ToString();
            }
        }

        private void SetInfoInExcel(ISheet Sheet, int rownum, int firstCol, int lastCol, string value, float heightInPoints, ICellStyle cellStyle)
        {
            IRow row = Sheet.CreateRow(rownum);
            row.HeightInPoints = heightInPoints;
            row.CreateCell(firstCol).SetCellValue(value);
            row.GetCell(firstCol).CellStyle = cellStyle;
            Sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum, firstCol, lastCol));
        }

        private void SetInfoInExcel(ISheet Sheet, int rownum, int firstCol, int lastCol, string value, float heightInPoints, ICellStyle cellStyle, IRow row)
        {
            row.HeightInPoints = heightInPoints;
            row.CreateCell(firstCol).SetCellValue(value);
            row.GetCell(firstCol).CellStyle = cellStyle;
            for (int i = firstCol + 1; i <= lastCol; i++)
            {
                row.CreateCell(i).SetCellValue("");
                row.GetCell(i).CellStyle = cellStyle;
            }
            Sheet.AddMergedRegion(new CellRangeAddress(rownum, rownum, firstCol, lastCol));
        }

        private void SetInfoInExcel(int firstCol, string value, float heightInPoints, ICellStyle cellStyle, IRow row)
        {
            row.HeightInPoints = heightInPoints;
            row.CreateCell(firstCol).SetCellValue(value);
            row.GetCell(firstCol).CellStyle = cellStyle;
        }

        /// <summary>
        /// 表格体样式
        /// </summary>
        /// <param name="WorkBook"></param>
        /// <returns></returns>
        private ICellStyle CreateTableBodyStyle(IWorkbook WorkBook)
        {
            //字体
            IFont font = WorkBook.CreateFont();
            font.FontName = "宋体";
            font.FontHeight = 11;

            //单元格样式
            ICellStyle style = WorkBook.CreateCellStyle();
            style.VerticalAlignment = VerticalAlignment.Center;
            style.SetFont(font);
            style.WrapText = true;

            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

            return style;
        }

        /// <summary>
        /// 表格头样式
        /// </summary>
        /// <param name="WorkBook"></param>
        /// <returns></returns>
        private ICellStyle CreateTableTitleStyle(IWorkbook WorkBook)
        {
            //字体
            IFont font = WorkBook.CreateFont();
            font.FontName = "宋体";
            font.FontHeight = 11;
            font.IsBold = true;

            //单元格样式
            ICellStyle style = WorkBook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.SetFont(font);

            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

            return style;
        }

        /// <summary>
        /// 第三种信息样式
        /// </summary>
        /// <param name="WorkBook"></param>
        /// <returns></returns>
        private ICellStyle CreateThirdInfoStyle(IWorkbook WorkBook)
        {
            //字体
            IFont font = WorkBook.CreateFont();
            font.FontName = "宋体";
            font.FontHeight = 11;

            //单元格样式
            ICellStyle style = WorkBook.CreateCellStyle();
            style.VerticalAlignment = VerticalAlignment.Center;
            style.SetFont(font);

            return style;
        }

        /// <summary>
        /// 第二种信息样式
        /// </summary>
        /// <param name="WorkBook"></param>
        /// <returns></returns>
        private ICellStyle CreateSecInfoStyle(IWorkbook WorkBook)
        {
            //字体
            IFont font = WorkBook.CreateFont();
            font.FontName = "宋体";
            font.FontHeight = 12;

            //单元格样式
            ICellStyle style = WorkBook.CreateCellStyle();
            style.VerticalAlignment = VerticalAlignment.Center;
            style.SetFont(font);

            return style;
        }

        /// <summary>
        /// 大标题样式
        /// </summary>
        /// <param name="WorkBook"></param>
        /// <returns></returns>
        private ICellStyle CreateHeadStyle(IWorkbook WorkBook)
        {
            //字体
            IFont font = WorkBook.CreateFont();
            font.FontName = "宋体";
            font.FontHeight = 18;
            font.IsBold = true;

            //单元格样式
            ICellStyle style = WorkBook.CreateCellStyle();
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.SetFont(font);

            return style;
        }
    }
}