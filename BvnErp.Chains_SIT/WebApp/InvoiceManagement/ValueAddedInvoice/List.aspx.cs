using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.InvoiceManagement.ValueAddedInvoice
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string DeductionMonth = Request.QueryString["DeductionMonth"];           
            string InvoiceNo = Request.QueryString["InvoiceNo"];           
            string InvoiceDateFrom = Request.QueryString["InvoiceDateFrom"];
            string InvoiceDateTo = Request.QueryString["InvoiceDateTo"];
            string SellsName = Request.QueryString["SellsName"];

            var predicate = PredicateBuilder.Create<Needs.Ccs.Services.Views.TaxManageView.TaxManageViewModels>();
            predicate = predicate.And(item => item.BusinessType == Needs.Ccs.Services.Enums.BusinessType.Customer&&item.InvoiceType==Needs.Ccs.Services.Enums.InvoiceType.Full);

            if (!string.IsNullOrEmpty(DeductionMonth))
            {
                DeductionMonth = DeductionMonth.Trim() + "-01";
                DateTime dtDeductionMonth = Convert.ToDateTime(DeductionMonth);
                predicate = predicate.And(item => item.AuthenticationMonth == dtDeductionMonth);
            }
            if (!string.IsNullOrEmpty(InvoiceNo))
            {
                InvoiceNo = InvoiceNo.Trim();
                predicate = predicate.And(item => item.InvoiceNo == InvoiceNo);
            }
            if (!string.IsNullOrEmpty(SellsName))
            {
                SellsName = SellsName.Trim();
                predicate = predicate.And(item => item.SellsName.Contains(SellsName));
            }

            if (!string.IsNullOrEmpty(InvoiceDateFrom))
            {
                InvoiceDateFrom = InvoiceDateFrom.Trim();
                DateTime dtPayDateFrom = Convert.ToDateTime(InvoiceDateFrom);
                predicate = predicate.And(item => item.InvoiceDate >= dtPayDateFrom);
            }


            if (!string.IsNullOrEmpty(InvoiceDateTo))
            {
                InvoiceDateTo = InvoiceDateTo.Trim();
                DateTime dtPayDateTo = Convert.ToDateTime(InvoiceDateTo);
                predicate = predicate.And(item => item.InvoiceDate <= dtPayDateTo);
            }



            Needs.Ccs.Services.Views.TaxManageView view = new Needs.Ccs.Services.Views.TaxManageView();
            view.AllowPaging = true;
            view.PageIndex = page;
            view.PageSize = rows;
            view.Predicate = predicate;

            int recordCount = view.RecordCount;
            var decTaxs = view.ToList();

            Func<Needs.Ccs.Services.Views.TaxManageView.TaxManageViewModels, object> convert = decTax => new
            {
                InvoiceCode = decTax.InvoiceCode,
                InvoiceNo = decTax.InvoiceNo,
                InvoiceDate = decTax.InvoiceDate==null?"":decTax.InvoiceDate.Value.ToString("yyyy-MM-dd"),
                SellsName = decTax.SellsName,
                Amount = decTax.Amount,
                VaildAmount = decTax.VaildAmount,
                ConfrimDate = decTax.ConfrimDate == null ? "" : decTax.ConfrimDate.Value.ToString("yyyy-MM"),
                IsVaild = decTax.InvoiceStatus,
                InvoiceDetailID = decTax.InvoiceDetailID,
                AuthenticationMonth = decTax.AuthenticationMonth == null ? "" : decTax.AuthenticationMonth.Value.ToString("yyyy-MM"),
                //IsValidDesc = decTax.InvoiceStatus.GetDescription()
            };

            Response.Write(new
            {
                rows = decTaxs.Select(convert).ToArray(),
                total = recordCount,
            }.Json());
        }

        /// <summary>
        /// 导入抵扣日期等信息
        /// </summary>
        protected void ImportDikouDate()
        {
            try
            {
                HttpPostedFile file = Request.Files["ImportDikouDate"];
                string ext = Path.GetExtension(file.FileName);
                if (ext != ".xls" && ext != ".xlsx")
                {
                    Response.Write((new { success = false, message = "文件格式错误，请上传.xls或.xlsx文件！" }).Json());
                    return;
                }

                //文件保存
                string fileName = file.FileName.ReName();
                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Import);
                fileDic.CreateDataDirectory();
                file.SaveAs(fileDic.FilePath);
                DataTable dt = Ccs.Utils.NPOIHelper.ExcelToDataTable(fileDic.FilePath, false);

                List<TaxManageUpload> UnDeductionDecTaxList = new List<TaxManageUpload>();
                List<string> errStrs = new List<string>();


                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    string DeductionMonthStr = dt.Rows[i][10].ToString();  // 认证月份
                    DateTime? DeductionMonthDt = null;
                    var resultCheckDeductionMonth = CheckDeductionMonth(DeductionMonthStr);
                    if (resultCheckDeductionMonth.Item1 == false)
                    {
                        errStrs.Add("所属月份格式不对，未导入数据");
                    }
                    else
                    {
                        DeductionMonthDt = resultCheckDeductionMonth.Item2; // 所属月份格式 正确
                    }

                    string InvoiceCode = dt.Rows[i][0].ToString(); // 发票代码
                    string InvoiceNo = dt.Rows[i][1].ToString(); // 发票号码
                    string InvoiceDate = dt.Rows[i][2].ToString(); // 发票日期
                    string SellsName = dt.Rows[i][3].ToString(); // 销方名称
                    string AmountStr = dt.Rows[i][4].ToString(); // 金额
                    string VaildAmountStr = dt.Rows[i][5].ToString(); // 有效税额
                    string ConfirmDateStr = dt.Rows[i][7].ToString(); // 认证日期


                    decimal AmountDec,VaildAmountDec;
                    DateTime InvoiceDateDt, ConfirmDateDt;

                    if (string.IsNullOrEmpty(InvoiceCode)||string.IsNullOrEmpty(InvoiceNo))
                    {
                        continue;
                    }
                    InvoiceCode = InvoiceCode.Trim();
                    InvoiceNo = InvoiceNo.Trim();
                    if (string.IsNullOrEmpty(InvoiceCode) || string.IsNullOrEmpty(InvoiceNo))
                    {
                        continue;
                    }

                    if (!decimal.TryParse(AmountStr, out AmountDec))
                    {
                        errStrs.Add("有效税额 " + AmountStr + " 不是有效的数字(" + InvoiceNo + ")");
                    }
                    else if (!decimal.TryParse(VaildAmountStr, out VaildAmountDec))
                    {
                        errStrs.Add("有效税额 " + VaildAmountStr + " 不是有效的数字(" + InvoiceNo + ")");
                    }
                    else if (!DateTime.TryParse(InvoiceDate, out InvoiceDateDt))
                    {
                        errStrs.Add("开票日期 " + InvoiceDate + " 不是有效的日期格式(" + InvoiceNo + ")");
                    }
                    else if (!DateTime.TryParse(ConfirmDateStr, out ConfirmDateDt))
                    {
                        errStrs.Add("确认/认证日期 " + ConfirmDateStr + " 不是有效的日期格式(" + InvoiceNo + ")");
                    }
                    else
                    {
                        if (DeductionMonthDt != null)
                        {
                            TaxManageUpload taxManageUpload = new TaxManageUpload();
                            taxManageUpload.ID = Guid.NewGuid().ToString("N").ToUpper();
                            taxManageUpload.InvoiceCode = InvoiceCode;
                            taxManageUpload.InvoiceNo = InvoiceNo;
                            taxManageUpload.InvoiceDate = InvoiceDateDt;
                            taxManageUpload.SellsName = SellsName;
                            taxManageUpload.Amount = AmountDec;
                            taxManageUpload.VaildAmount = VaildAmountDec;
                            taxManageUpload.ConfrimDate = ConfirmDateDt;
                            taxManageUpload.AuthenticationMonth = DeductionMonthDt;
                            taxManageUpload.InvoiceType = Needs.Ccs.Services.Enums.InvoiceType.Full;
                            taxManageUpload.BusinessType = Needs.Ccs.Services.Enums.BusinessType.Customer;
                            taxManageUpload.IsVaild = Needs.Ccs.Services.Enums.InvoiceVaildStatus.UnChecked;

                            UnDeductionDecTaxList.Add(taxManageUpload);
                        }
                        else
                        {
                            errStrs.Add("认证月份 " + ConfirmDateStr + " 不是有效的日期格式(" + InvoiceNo + ")");
                        }
                    }
                }


                if (errStrs.Count == 0)
                {
                    //检查通过，更新数据
                    foreach (var UnDeductionDecTaxItem in UnDeductionDecTaxList)
                    {
                        UnDeductionDecTaxItem.Upload();
                    }
                    Response.Write((new { success = true, message = "导入成功" }).Json());
                }
                else
                {
                    Response.Write((new { success = false, message = "部分导入失败", errs = errStrs.ToArray() }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导入失败" + ex.Message }).Json());
            }
        }

        private System.Tuple<bool, DateTime?> CheckDeductionMonth(string deductionMonthStr)
        {
            if (string.IsNullOrEmpty(deductionMonthStr))
            {
                return new Tuple<bool, DateTime?>(false, null);
            }
            deductionMonthStr = deductionMonthStr.Trim();
            if (string.IsNullOrEmpty(deductionMonthStr))
            {
                return new Tuple<bool, DateTime?>(false, null);
            }
            if (deductionMonthStr.Length != 6)
            {
                return new Tuple<bool, DateTime?>(false, null);
            }
            string yearStr = deductionMonthStr.Substring(0, 4);
            string monthStr = deductionMonthStr.Substring(4, 2);
            int yearInt, monthInt;
            if (!int.TryParse(yearStr, out yearInt) || !int.TryParse(monthStr, out monthInt))
            {
                return new Tuple<bool, DateTime?>(false, null);
            }

            return new Tuple<bool, DateTime?>(true, new DateTime(yearInt, monthInt, 1));
        }
    }
}