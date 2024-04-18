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

namespace WebApp.Finance.Payment.Paied
{
    public partial class PaiedList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //费用类型
            var feeTypeAll = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.FinanceFeeType>().Select(item => new { item.Key, item.Value }).ToList();

            foreach (Needs.Ccs.Services.Enums.FeeTypeEnum feeTypeEnum in Enum.GetValues(typeof(Needs.Ccs.Services.Enums.FeeTypeEnum)))
            {
                feeTypeAll.Add(new
                {
                    Key = feeTypeEnum.GetHashCode().ToString(),
                    Value = feeTypeEnum.ToString(),
                });
            }

            this.Model.FeeType = feeTypeAll.Json();

            //付款类型
            this.Model.PayType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.PaymentType>().Select(item => new { item.Key, item.Value }).Json();

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

            string FeeType = Request.QueryString["FeeType"];
            string PayType = Request.QueryString["PayType"];
            string PayDateStartDate = Request.QueryString["PayDateStartDate"];
            string PayDateEndDate = Request.QueryString["PayDateEndDate"];
            string FinanceVaultID = Request.QueryString["FinanceVaultID"];
            string FinanceAccountID = Request.QueryString["FinanceAccountID"];

            using (var query = new Needs.Ccs.Services.Views.PaiedListView())
            {
                var view = query;

                if (!string.IsNullOrEmpty(FeeType))
                {
                    view = view.SearchByFeeType(int.Parse(FeeType));
                }
                if (!string.IsNullOrEmpty(PayType))
                {
                    Needs.Ccs.Services.Enums.PaymentType payTypeEnum = (Needs.Ccs.Services.Enums.PaymentType)(int.Parse(PayType));
                    view = view.SearchByPayType(payTypeEnum);
                }
                if (!string.IsNullOrEmpty(PayDateStartDate))
                {
                    DateTime begin = DateTime.Parse(PayDateStartDate);
                    view = view.SearchByPayDateStartDate(begin);
                }
                if (!string.IsNullOrEmpty(PayDateEndDate))
                {
                    DateTime end = DateTime.Parse(PayDateEndDate);
                    end = end.AddDays(1);
                    view = view.SearchByPayDateEndDate(end);
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
                string FeeType = Request.Form["FeeType"];
                string PayType = Request.Form["PayType"];
                string PayDateStartDate = Request.Form["PayDateStartDate"];
                string PayDateEndDate = Request.Form["PayDateEndDate"];

                using (var query = new Needs.Ccs.Services.Views.PaiedListView())
                {
                    var view = query;

                    if (!string.IsNullOrEmpty(FeeType))
                    {
                        view = view.SearchByFeeType(int.Parse(FeeType));
                    }
                    if (!string.IsNullOrEmpty(PayType))
                    {
                        Needs.Ccs.Services.Enums.PaymentType payTypeEnum = (Needs.Ccs.Services.Enums.PaymentType)(int.Parse(PayType));
                        view = view.SearchByPayType(payTypeEnum);
                    }
                    if (!string.IsNullOrEmpty(PayDateStartDate))
                    {
                        DateTime begin = DateTime.Parse(PayDateStartDate);
                        view = view.SearchByPayDateStartDate(begin);
                    }
                    if (!string.IsNullOrEmpty(PayDateEndDate))
                    {
                        DateTime end = DateTime.Parse(PayDateEndDate);
                        end = end.AddDays(1);
                        view = view.SearchByPayDateEndDate(end);
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
                    excelconfig.Title = "付款";
                    excelconfig.TitleFont = "微软雅黑";
                    excelconfig.TitlePoint = 16;
                    excelconfig.IsAllSizeColumn = true;
                    //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                    List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                    excelconfig.ColumnEntity = listColumnEntity;

                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SeqNo", ExcelColumn = "流水号", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "PayeeName", ExcelColumn = "收款方", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "FinanceVaultName", ExcelColumn = "付款金库", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "FinanceAccountName", ExcelColumn = "付款账户", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "FeeTypeName", ExcelColumn = "费用类型", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Amount", ExcelColumn = "金额", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Currency", ExcelColumn = "币种", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "PayType", ExcelColumn = "付款类型", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "PayerName", ExcelColumn = "付款人", Alignment = "left" });
                    excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "PayDate", ExcelColumn = "付款日期", Alignment = "left" });

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

        /// <summary>
        /// 设置为已上传
        /// </summary>
        protected void SetUploaded()
        {
            try
            {
                string FinancePaymentID = Request.Form["FinancePaymentID"];

                Needs.Ccs.Services.Models.FinancePaymentInvoiceUpload financePaymentInvoiceUpload = new Needs.Ccs.Services.Models.FinancePaymentInvoiceUpload(FinancePaymentID);
                financePaymentInvoiceUpload.SetUploaded();

                Response.Write((new { success = true, message = "设置成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "设置失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 设置为未上传
        /// </summary>
        protected void SetUnUpload()
        {
            try
            {
                string FinancePaymentID = Request.Form["FinancePaymentID"];

                Needs.Ccs.Services.Models.FinancePaymentInvoiceUpload financePaymentInvoiceUpload = new Needs.Ccs.Services.Models.FinancePaymentInvoiceUpload(FinancePaymentID);
                financePaymentInvoiceUpload.SetUnUpload();

                Response.Write((new { success = true, message = "设置成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "设置失败：" + ex.Message }).Json());
            }
        }

    }
}