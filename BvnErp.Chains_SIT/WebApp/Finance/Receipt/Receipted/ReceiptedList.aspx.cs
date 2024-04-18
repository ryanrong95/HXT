using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.Finance.Receipt.Receipted
{
    public partial class ReceiptedList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.FinanceFeeType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.FinanceFeeType>().Select(item => new { item.Key, item.Value }).Json();

            this.Model.FinanceVaultData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal)
                .Select(item => new { Value = item.ID, Text = item.Name }).Json();

            this.Model.FinanceAccountData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceAccounts
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal)
                .Select(item => new { Value = item.ID, Text = item.AccountName }).Json();
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string FeeTypeInt = Request.QueryString["FeeTypeInt"];
            string Payer = Request.QueryString["Payer"];
            string ReceiptDateStartDate = Request.QueryString["ReceiptDateStartDate"];
            string ReceiptDateEndDate = Request.QueryString["ReceiptDateEndDate"];
            string SeqNo = Request.QueryString["SeqNo"];
            string FinanceVaultID = Request.QueryString["FinanceVaultID"];
            string FinanceAccountID = Request.QueryString["FinanceAccountID"];

            using (var query = new Needs.Ccs.Services.Views.ReceiptedListView())
            {
                var view = query;

                if (!string.IsNullOrEmpty(FeeTypeInt))
                {
                    Needs.Ccs.Services.Enums.FinanceFeeType feeType = (Needs.Ccs.Services.Enums.FinanceFeeType)(int.Parse(FeeTypeInt));
                    view = view.SearchByFeeType(feeType);
                }
                if (!string.IsNullOrEmpty(Payer))
                {
                    Payer = Payer.Trim();
                    view = view.SearchByPayer(Payer);
                }
                if (!string.IsNullOrEmpty(ReceiptDateStartDate))
                {
                    DateTime begin = DateTime.Parse(ReceiptDateStartDate);
                    view = view.SearchByReceiptDateStartDate(begin);
                }
                if (!string.IsNullOrEmpty(ReceiptDateEndDate))
                {
                    DateTime end = DateTime.Parse(ReceiptDateEndDate);
                    end = end.AddDays(1);
                    view = view.SearchByReceiptDateEndDate(end);
                }
                if (!string.IsNullOrEmpty(SeqNo))
                {
                    SeqNo = SeqNo.Trim();
                    view = view.SearchBySeqNo(SeqNo);
                }
                if (!string.IsNullOrEmpty(FinanceVaultID))
                {
                    view = view.SearchByFinanceVaultID(FinanceVaultID);
                }
                if (!string.IsNullOrEmpty(FinanceAccountID))
                {
                    view = view.SearchByFinanceAccountID(FinanceAccountID);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

        /// <summary>
        /// 导出 Excel
        /// </summary>
        protected void ExportExcel()
        {
            try
            {
                string FeeTypeInt = Request.Form["FeeTypeInt"];
                string Payer = Request.Form["Payer"];
                string ReceiptDateStartDate = Request.Form["ReceiptDateStartDate"];
                string ReceiptDateEndDate = Request.Form["ReceiptDateEndDate"];
                string SeqNo = Request.Form["SeqNo"];
                string FinanceVaultID = Request.Form["FinanceVaultID"];
                string FinanceAccountID = Request.Form["FinanceAccountID"];

                using (var query = new Needs.Ccs.Services.Views.ReceiptedListView())
                {
                    var view = query;

                    if (!string.IsNullOrEmpty(FeeTypeInt))
                    {
                        Needs.Ccs.Services.Enums.FinanceFeeType feeType = (Needs.Ccs.Services.Enums.FinanceFeeType)(int.Parse(FeeTypeInt));
                        view = view.SearchByFeeType(feeType);
                    }
                    if (!string.IsNullOrEmpty(Payer))
                    {
                        Payer = Payer.Trim();
                        view = view.SearchByPayer(Payer);
                    }
                    if (!string.IsNullOrEmpty(ReceiptDateStartDate))
                    {
                        DateTime begin = DateTime.Parse(ReceiptDateStartDate);
                        view = view.SearchByReceiptDateStartDate(begin);
                    }
                    if (!string.IsNullOrEmpty(ReceiptDateEndDate))
                    {
                        DateTime end = DateTime.Parse(ReceiptDateEndDate);
                        end = end.AddDays(1);
                        view = view.SearchByReceiptDateEndDate(end);
                    }
                    if (!string.IsNullOrEmpty(SeqNo))
                    {
                        SeqNo = SeqNo.Trim();
                        view = view.SearchBySeqNo(SeqNo);
                    }
                    if (!string.IsNullOrEmpty(FinanceVaultID))
                    {
                        view = view.SearchByFinanceVaultID(FinanceVaultID);
                    }
                    if (!string.IsNullOrEmpty(FinanceAccountID))
                    {
                        view = view.SearchByFinanceAccountID(FinanceAccountID);
                    }

                    var dataListJson = view.ToMyPage().Json();

                    //写入数据
                    DataTable dt = NPOIHelper.JsonToDataTable(dataListJson);

                    string fileName = DateTime.Now.Ticks + ".xls";

                    //创建文件目录
                    FileDirectory fileDic = new FileDirectory(fileName);
                    fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                    fileDic.CreateDataDirectory();

                    #region 设置导出格式

                    var excelconfig = new ExcelConfig();
                    excelconfig.FilePath = fileDic.FilePath;
                    excelconfig.Title = "收款";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 16;
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;

                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SeqNo", ExcelColumn = "银行流水号", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "FinanceVaultName", ExcelColumn = "金库名称", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "FinanceAccountName", ExcelColumn = "账户名称", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Payer", ExcelColumn = "客户名称", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "FeeType", ExcelColumn = "收款类型", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Amount", ExcelColumn = "金额", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Currency", ExcelColumn = "币种", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ReceiptDate", ExcelColumn = "收款日期", Alignment = "center" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ThatDayExchangeRate", ExcelColumn = "当天汇率", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ProductAmount", ExcelColumn = "货款", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SwapExchangeRate", ExcelColumn = "换汇汇率", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "TariffAmount", ExcelColumn = "关税", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "AddedValueTaxAmount", ExcelColumn = "增值税", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ExciseTaxAmount", ExcelColumn = "消费税", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ServiceAmount", ExcelColumn = "服务费", Alignment = "left" });

                    #endregion

                    //调用导出方法
                    NPOIHelper.ExcelDownload(dt, excelconfig);

                    Response.Write((new
                    {
                        success = true,
                        message = "导出成功",
                        url = fileDic.FileUrl
                    }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new
                {
                    success = false,
                    message = "导出失败：" + ex.Message,
                }).Json());
            }
        }

    }
}