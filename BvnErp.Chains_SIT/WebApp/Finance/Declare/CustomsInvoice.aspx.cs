using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Npoi;
using Needs.Utils.Serializers;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Declare
{
    public partial class CustomsInvoice : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            //开票类型
            this.Model.InvoiceTypeData = EnumUtils.ToDictionary<InvoiceType>()
                .Select(item => new { item.Key, item.Value }).Json();
            //缴税状态
            this.Model.DecTaxStatusData = EnumUtils.ToDictionary<DecTaxStatus>()
                .Select(item => new { item.Key, item.Value }).Json();
            //缴税类型
            this.Model.DecTaxTypeData = EnumUtils.ToDictionary<DecTaxType>()
                .Select(item => new { item.Key, item.Value }).Json();
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ContrNo = Request.QueryString["ContrNo"];
            string OrderID = Request.QueryString["OrderID"];
            string EntryID = Request.QueryString["EntryID"];
            string InvoiceType = Request.QueryString["InvoiceType"];
            string DecTaxStatus = Request.QueryString["DecTaxStatus"];
            string DateType = Request.QueryString["DateType"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string OwnerName = Request.QueryString["OwnerName"];

            using (var query = new Needs.Ccs.Services.Views.DecTaxListViewRJ())
            {

                var view = query;

                if (!string.IsNullOrWhiteSpace(ContrNo))
                {
                    ContrNo = ContrNo.Trim();
                    view = view.SearchByContrNo(ContrNo);
                }

                if (!string.IsNullOrWhiteSpace(OrderID))
                {
                    OrderID = OrderID.Trim();
                    view = view.SearchByOrderID(OrderID);
                }

                if (!string.IsNullOrWhiteSpace(EntryID))
                {
                    EntryID = EntryID.Trim();
                    view = view.SearchByEntryID(EntryID);
                }

                if (!string.IsNullOrWhiteSpace(InvoiceType))
                {
                    var type = Enum.Parse(typeof(Needs.Ccs.Services.Enums.InvoiceType), InvoiceType);
                    view = view.SearchByInvoiceType((Needs.Ccs.Services.Enums.InvoiceType)type);
                }

                if (!string.IsNullOrWhiteSpace(OwnerName))
                {
                    OwnerName = OwnerName.Trim();
                    view = view.SearchByOwnerName(OwnerName);
                }

                if (!string.IsNullOrEmpty(StartDate))
                {
                    StartDate = StartDate.Trim();
                    var from = DateTime.Parse(StartDate);
                    view = view.SearchByFrom(from);
                }

                if (!string.IsNullOrEmpty(EndDate))
                {
                    EndDate = EndDate.Trim();
                    var to = DateTime.Parse(EndDate).AddDays(1);
                    view = view.SearchByTo(to);
                }

                if (!string.IsNullOrEmpty(DecTaxStatus))
                {
                    DecTaxStatus status = (DecTaxStatus)int.Parse(DecTaxStatus);
                    view = view.SearchByDecTaxStatus(status);
                  
                }

                Response.Write(view.ToMyPage(page, rows).Json());

            }




        }

        /// <summary>
        /// 上传报关单文件
        /// </summary>
        protected void UploadFile()
        {
            try
            {
                HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                if (files.Count > 0)
                {
                    if (files.Count == 1 && files[0].ContentLength == 0)
                    {
                        Response.Write((new { success = false, message = "上传失败，文件为空" }).Json());
                        return;
                    }

                    for (int i = 0; i < files.Count; i++)
                    {
                        //处理附件
                        HttpPostedFile file = files[i];
                        if (file.ContentLength != 0)
                        {
                            var FileName = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                            //海关发票文件名需要去掉后面3位才是EntryId
                            if (FileName.Contains('A') || FileName.Contains('L') || FileName.Contains('Y'))
                            {
                                FileName = FileName.Substring(0, FileName.Length - 4);
                            }
                            var decheads = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead.Where(item => item.EntryId == FileName);
                            if (decheads.Count() == 0)
                            {
                                throw new Exception("文件：" + FileName + "找不到对应的报关单");
                            }
                            //文件保存
                            //string fileName = file.FileName.ReName();
                            string fileName = file.FileName + System.IO.Path.GetExtension(file.FileName);

                            //创建文件目录
                            FileDirectory fileDic = new FileDirectory(fileName);
                            fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.DecHead);
                            fileDic.CreateDataDirectory();
                            file.SaveAs(fileDic.FilePath);

                            var dechead = decheads.FirstOrDefault();
                            DecHeadFile decHeadFile = new DecHeadFile();
                            decHeadFile.DecHeadID = dechead.ID;
                            decHeadFile.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                            decHeadFile.Name = file.FileName;
                            decHeadFile.FileFormat = file.ContentType;
                            decHeadFile.Url = fileDic.VirtualPath;
                            decHeadFile.DecHead = dechead;
                            if (file.FileName.Contains('A'))
                            {
                                decHeadFile.FileType = Needs.Ccs.Services.Enums.FileType.DecHeadTariffFile;
                            }
                            else if (file.FileName.Contains('L'))
                            {
                                decHeadFile.FileType = Needs.Ccs.Services.Enums.FileType.DecHeadVatFile;
                            }
                            else if (file.FileName.Contains('Y'))
                            {
                                decHeadFile.FileType = Needs.Ccs.Services.Enums.FileType.DecHeadExciseTaxFile;
                            }
                            else
                            {
                                decHeadFile.FileType = Needs.Ccs.Services.Enums.FileType.DecHeadFile;
                            }
                            //持久化
                            decHeadFile.Enter();
                        }
                    }
                    Response.Write((new { success = true, message = "上传成功" }).Json());
                }
                else
                {
                    Response.Write((new { success = false, message = "上传失败，文件为空" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "上传失败" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 导入缴税流水
        /// </summary>
        protected void UploadExcel()
        {
            try
            {
                HttpPostedFile file = Request.Files["uploadExcel"];
                string ext = Path.GetExtension(file.FileName);
                if (ext != ".xls" && ext != ".xlsx")
                {
                    Response.Write((new { success = false, message = "文件格式错误，请上传.xls或.xlsx文件！" }).Json());
                    return;
                }
                //查询未缴税
                var DecTaxes = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DecTax.AsQueryable().Where(t => t.DecTaxStatus == DecTaxStatus.Unpaid);
                StringBuilder str = new StringBuilder();
                //文件保存
                string fileName = file.FileName.ReName();
                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Import);
                fileDic.CreateDataDirectory();
                file.SaveAs(fileDic.FilePath);
                DataTable dt = Ccs.Utils.NPOIHelper.ExcelToDataTable(fileDic.FilePath, false);

                //for (int i = 1; i < dt.Rows.Count; i++)
                //{
                //    string EntryId = dt.Rows[i][1].ToString();
                //    var DecTax = DecTaxes.Where(t => t.EntryId == EntryId).FirstOrDefault();
                //    //判断报关已经上传流水，才更新缴税日期；
                //    if (DecTax != null&& DecTax.UploadStatus==UploadStatus.Uploaded)
                //    {
                //        DecTaxFlow flow = new DecTaxFlow();
                //        flow.DecheadID = DecTax.ID;
                //        flow.BankName = dt.Rows[i][4].ToString();
                //        flow.TaxNumber = dt.Rows[i][3].ToString();
                //        //关税类型
                //        if (flow.TaxNumber.Contains("A"))
                //        {
                //            flow.TaxType = DecTaxType.Tariff;
                //        }
                //        else if (flow.TaxNumber.Contains("L"))
                //        {
                //            flow.TaxType = DecTaxType.AddedValueTax;
                //        }
                //        else
                //        {
                //            continue;
                //        }
                //        //付款日期
                //        string dateStr = dt.Rows[i][9].ToString();
                //        DateTime date;
                //        bool flag = DateTime.TryParse(dateStr, out date);
                //        if (flag)
                //        {
                //            flow.PayDate = date;
                //        }
                //        else
                //        {
                //            flow.PayDate = DateTime.FromOADate(double.Parse(dateStr));
                //        }
                //        flow.Amount = decimal.Parse(dt.Rows[i][10].ToString());
                //        flow.Status = DecTaxStatus.Paid;
                //        flow.IsUpload = UploadStatus.Uploaded;
                //        flow.Enter();
                //    }
                //    else
                //    {
                //        str.Append(EntryId + ";");
                //    }
                //}

                var updateData = new Dictionary<string, List<DecTaxFlow>>();
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    string EntryId = dt.Rows[i][1].ToString();
                    var DecTax = DecTaxes.Where(t => t.EntryId == EntryId).FirstOrDefault();
                    if (DecTax != null && DecTax.UploadStatus == UploadStatus.Uploaded)
                    {
                        DecTaxFlow flow = new DecTaxFlow();
                        flow.DecheadID = DecTax.ID;
                        flow.BankName = dt.Rows[i][4].ToString();
                        flow.TaxNumber = dt.Rows[i][3].ToString();
                        flow.Amount = decimal.Parse(dt.Rows[i][10].ToString());
                        flow.OrderID = DecTax.OrderID;
                        flow.EntryID = EntryId;
                        //关税类型
                        if (flow.TaxNumber.Contains("A"))
                        {
                            flow.TaxType = DecTaxType.Tariff;
                        }
                        else if (flow.TaxNumber.Contains("L"))
                        {
                            flow.TaxType = DecTaxType.AddedValueTax;
                        }
                        else if (flow.TaxNumber.Contains("Y"))
                        {
                            flow.TaxType = DecTaxType.ExciseTax;
                        }
                        else
                        {
                            continue;
                        }
                        //付款日期
                        string dateStr = dt.Rows[i][9].ToString();
                        DateTime date;
                        bool flag = DateTime.TryParse(dateStr, out date);
                        if (flag)
                        {
                            flow.PayDate = date;
                        }
                        else
                        {
                            flow.PayDate = DateTime.FromOADate(double.Parse(dateStr));
                        }
                        flow.Status = DecTaxStatus.Paid;
                        if (!updateData.ContainsKey(EntryId))
                        {
                            updateData.Add(EntryId, new List<DecTaxFlow>());
                        }
                        updateData[EntryId].Add(flow);
                    }
                }

                foreach (var item in updateData)
                {
                    var tarifData = item.Value.FirstOrDefault(f => f.TaxType == DecTaxType.Tariff);
                    if (tarifData != null)
                    {
                        tarifData.Enter();
                    }
                    var addedData = item.Value.FirstOrDefault(f => f.TaxType == DecTaxType.AddedValueTax);
                    if (addedData != null)
                    {
                        addedData.Enter();
                    }
                    var exciseTaxData = item.Value.FirstOrDefault(f => f.TaxType == DecTaxType.ExciseTax);
                    if (exciseTaxData != null)
                    {
                        exciseTaxData.Enter();
                    }
                }

                if (str.ToString().Length > 0)
                {
                    Response.Write((new { success = false, message = "上传失败的报关单: " + str.ToString() }).Json());
                }
                else
                {
                    Response.Write((new { success = true, message = "上传成功" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "上传失败" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 导出所有已缴税未抵扣的增值税
        /// </summary>
        protected void Export()
        {
            try
            {
                //已缴税未抵扣的进口增值税
                var DecTaxes = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DecTax
                    .Where(item => item.DecTaxStatus == DecTaxStatus.Paid && item.InvoiceType == InvoiceType.Full).ToList();
                //创建数据
                var linq = DecTaxes.Select((t, i) => new
                {
                    序号 = i + 1,
                    报关单号 = t.EntryId,
                    税费单序号 = t.VatfFlow.TaxNumber.Substring(t.VatfFlow.TaxNumber.Length - 2, 2),
                    税费单号 = t.VatfFlow.TaxNumber,
                    支付银行 = t.VatfFlow.BankName,
                    税费种类 = t.VatfFlow.TaxType.GetDescription(),
                    合同号 = t.ContrNo,
                    申报关区 = t.CustomMaster,
                    缴款期限 = "",
                    银行扣税时间 = t.VatfFlow.PayDate?.ToShortDateString(),
                    支付金额 = t.VatfFlow.Amount.ToRound(2),
                    汇总征税标志 = "汇总纳税",
                    支付状态 = "支付成功",
                    客户名称 = t.OwnerName,
                    抵扣时间 = t.VatfFlow.DeductionTime?.ToShortDateString(),
                    填发日期 = t.VatfFlow.FillinDate.HasValue ? t.VatfFlow.FillinDate.Value.ToString("yyyy-MM-dd") : "",
                    消费使用单位 = t.ConsumeName,
                });
                IWorkbook workbook = ExcelFactory.Create();
                NPOIHelper npoi = new NPOIHelper(workbook);
                int[] columnsWidth = { 5, 20, 13, 23, 18, 13, 18, 13, 13, 13, 13, 13, 13, 25, 13 };
                npoi.EnumrableToExcel(linq, 0, columnsWidth);
                //创建文件夹
                var fileName = DateTime.Now.Ticks + ".xlsx";
                FileDirectory file = new FileDirectory(fileName);
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                file.CreateDataDirectory();
                //保存文件
                npoi.SaveAs(file.FilePath);

                Response.Write((new { success = true, message = "导出成功", url = file.FileUrl }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 导出缴税流水
        /// </summary>
        protected void ExportAll()
        {
            try
            {
                #region 查询筛选
                string ContrNo = Request.Form["ContrNo"];
                string OrderID = Request.Form["OrderID"];
                string EntryID = Request.Form["EntryID"];
                string InvoiceType = Request.Form["InvoiceType"];
                string DecTaxStatus = Request.Form["DecTaxStatus"];
                string DateType = Request.Form["DateType"];
                string StartDate = Request.Form["StartDate"];
                string EndDate = Request.Form["EndDate"];
                string OwnerName = Request.Form["OwnerName"];
                var DecTaxes = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DecTax.AsQueryable();
                if (!string.IsNullOrEmpty(ContrNo))
                {
                    ContrNo = ContrNo.Trim();
                    DecTaxes = DecTaxes.Where(t => t.ContrNo.Contains(ContrNo));
                }
                if (!string.IsNullOrEmpty(OrderID))
                {
                    OrderID = OrderID.Trim();
                    DecTaxes = DecTaxes.Where(t => t.OrderID.Contains(OrderID));
                }
                if (!string.IsNullOrEmpty(EntryID))
                {
                    EntryID = EntryID.Trim();
                    DecTaxes = DecTaxes.Where(t => t.EntryId.Contains(EntryID));
                }
                if (!string.IsNullOrEmpty(OwnerName))
                {
                    OwnerName = OwnerName.Trim();
                    DecTaxes = DecTaxes.Where(t => t.OwnerName.Contains(OwnerName));
                }
                if (!string.IsNullOrEmpty(InvoiceType))
                {
                    InvoiceType type = (InvoiceType)int.Parse(InvoiceType);
                    DecTaxes = DecTaxes.Where(t => t.InvoiceType == type);
                }
                if (!string.IsNullOrEmpty(DecTaxStatus))
                {
                    DecTaxStatus status = (DecTaxStatus)int.Parse(DecTaxStatus);
                    DecTaxes = DecTaxes.Where(t => t.DecTaxStatus == status);
                }
                if (!string.IsNullOrEmpty(DateType))
                {
                    if (DateType == "报关日期")
                    {
                        if (!string.IsNullOrEmpty(StartDate))
                        {
                            var from = DateTime.Parse(StartDate);
                            DecTaxes = DecTaxes.Where(t => t.DDate >= from);
                        }
                        if (!string.IsNullOrEmpty(EndDate))
                        {
                            var to = DateTime.Parse(EndDate).AddDays(1);
                            DecTaxes = DecTaxes.Where(t => t.DDate < to);
                        }
                    }
                    if (DateType == "缴税日期")
                    {

                    }
                    if (DateType == "抵扣日期")
                    {

                    }
                }
                #endregion

                var tariffFlows = DecTaxes.Where(t => t.flows.Where(x => x.TaxType == DecTaxType.Tariff).Count() > 0).ToList()
                    .Select(t => new
                    {
                        报关单号 = t.EntryId,
                        税费单序号 = t.TariffFlow.TaxNumber.Substring(t.TariffFlow.TaxNumber.Length - 2, 2),
                        税费单号 = t.TariffFlow.TaxNumber,
                        支付银行 = t.TariffFlow.BankName,
                        税费种类 = t.TariffFlow.TaxType.GetDescription(),
                        合同号 = t.ContrNo,
                        申报关区 = t.CustomMaster,
                        缴款期限 = "",
                        银行扣税时间 = t.TariffFlow.PayDate?.ToShortDateString(),
                        支付金额 = t.TariffFlow.Amount.ToRound(2),
                        汇总征税标志 = "汇总纳税",
                        支付状态 = "支付成功",
                        客户名称 = t.OwnerName,
                        抵扣时间 = t.TariffFlow.DeductionTime?.ToShortDateString(),
                        抵扣状态 = "关税不抵扣",
                        消费使用单位 = t.ConsumeName,
                        填发日期 = ""
                    });
                var exciseTaxFlows = DecTaxes.Where(t => t.flows.Where(x => x.TaxType == DecTaxType.ExciseTax).Count() > 0).ToList()
                    .Select(t => new
                    {
                        报关单号 = t.EntryId,
                        税费单序号 = t.ExciseTaxFlow.TaxNumber.Substring(t.ExciseTaxFlow.TaxNumber.Length - 2, 2),
                        税费单号 = t.ExciseTaxFlow.TaxNumber,
                        支付银行 = t.ExciseTaxFlow.BankName,
                        税费种类 = t.ExciseTaxFlow.TaxType.GetDescription(),
                        合同号 = t.ContrNo,
                        申报关区 = t.CustomMaster,
                        缴款期限 = "",
                        银行扣税时间 = t.ExciseTaxFlow.PayDate?.ToShortDateString(),
                        支付金额 = t.ExciseTaxFlow.Amount.ToRound(2),
                        汇总征税标志 = "汇总纳税",
                        支付状态 = "支付成功",
                        客户名称 = t.OwnerName,
                        抵扣时间 = t.ExciseTaxFlow.DeductionTime?.ToShortDateString(),
                        抵扣状态 = t.ExciseTaxFlow.Status == Needs.Ccs.Services.Enums.DecTaxStatus.Deducted ? "已抵扣" : "未抵扣",
                        消费使用单位 = t.ConsumeName,
                        填发日期 = ""
                    });
                var varFlows = DecTaxes.Where(t => t.flows.Where(x => x.TaxType == DecTaxType.AddedValueTax).Count() > 0).ToList()
                    .Select(t => new
                    {
                        报关单号 = t.EntryId,
                        税费单序号 = t.VatfFlow.TaxNumber.Substring(t.VatfFlow.TaxNumber.Length - 2, 2),
                        税费单号 = t.VatfFlow.TaxNumber,
                        支付银行 = t.VatfFlow.BankName,
                        税费种类 = t.VatfFlow.TaxType.GetDescription(),
                        合同号 = t.ContrNo,
                        申报关区 = t.CustomMaster,
                        缴款期限 = "",
                        银行扣税时间 = t.VatfFlow.PayDate?.ToShortDateString(),
                        支付金额 = t.VatfFlow.Amount.ToRound(2),
                        汇总征税标志 = "汇总纳税",
                        支付状态 = "支付成功",
                        客户名称 = t.OwnerName,
                        抵扣时间 = t.VatfFlow.DeductionTime?.ToShortDateString(),
                        抵扣状态 = t.VatfFlow.Status == Needs.Ccs.Services.Enums.DecTaxStatus.Deducted ? "已抵扣" : "未抵扣",
                        消费使用单位 = t.ConsumeName,
                        填发日期 = t.VatfFlow.FillinDate.HasValue? t.VatfFlow.FillinDate.Value.ToString("yyyy-MM-dd") : ""
                    });
                var Union = tariffFlows.Union(exciseTaxFlows).Union(varFlows);

                var linq = Union.Select((t, i) => new
                {
                    序号 = i + 1,
                    报关单号 = t.报关单号,
                    税费单序号 = t.税费单序号,
                    税费单号 = t.税费单号,
                    支付银行 = t.支付银行,
                    税费种类 = t.税费种类,
                    合同号 = t.合同号,
                    申报关区 = t.申报关区,
                    缴款期限 = t.缴款期限,
                    银行扣税时间 = t.银行扣税时间,
                    支付金额 = t.支付金额,
                    汇总征税标志 = t.汇总征税标志,
                    支付状态 = t.支付状态,
                    客户名称 = t.客户名称,
                    抵扣时间 = t.抵扣时间,
                    抵扣状态 = t.抵扣状态,
                    消费使用单位 = t.消费使用单位,
                    填发日期 = t.填发日期
                });

                IWorkbook workbook = ExcelFactory.Create();
                NPOIHelper npoi = new NPOIHelper(workbook);
                int[] columnsWidth = { 5, 20, 13, 23, 18, 13, 18, 13, 13, 13, 13, 13, 13, 25, 13, 15, 25, 15 };
                npoi.EnumrableToExcel(linq, 0, columnsWidth);
                //创建文件夹
                var fileName = DateTime.Now.Ticks + ".xlsx";
                FileDirectory file = new FileDirectory(fileName);
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                file.CreateDataDirectory();
                //保存文件
                npoi.SaveAs(file.FilePath);

                Response.Write((new { success = true, message = "导出成功", url = file.FileUrl }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 导出报关数据
        /// </summary>
        protected void ExportDecData()
        {
            try
            {
                string ContrNo = Request.Form["ContrNo"];
                string OrderID = Request.Form["OrderID"];
                string EntryID = Request.Form["EntryID"];
                string InvoiceType = Request.Form["InvoiceType"];
                string DecTaxStatus = Request.Form["DecTaxStatus"];
                string DateType = Request.Form["DateType"];
                string StartDate = Request.Form["StartDate"];
                string EndDate = Request.Form["EndDate"];
                string OwnerName = Request.Form["OwnerName"];

                var DecTax = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DecTax.OrderByDescending(t => t.DDate).AsQueryable();
                if (!string.IsNullOrEmpty(ContrNo))
                {
                    ContrNo = ContrNo.Trim();
                    DecTax = DecTax.Where(t => t.ContrNo.Contains(ContrNo));
                }
                if (!string.IsNullOrEmpty(OrderID))
                {
                    OrderID = OrderID.Trim();
                    DecTax = DecTax.Where(t => t.OrderID.Contains(OrderID));
                }
                if (!string.IsNullOrEmpty(EntryID))
                {
                    EntryID = EntryID.Trim();
                    DecTax = DecTax.Where(t => t.EntryId.Contains(EntryID));
                }
                if (!string.IsNullOrEmpty(InvoiceType))
                {
                    InvoiceType type = (InvoiceType)int.Parse(InvoiceType);
                    DecTax = DecTax.Where(t => t.InvoiceType == type);
                }
                if (!string.IsNullOrEmpty(DecTaxStatus))
                {
                    DecTaxStatus status = (DecTaxStatus)int.Parse(DecTaxStatus);
                    DecTax = DecTax.Where(t => t.DecTaxStatus == status);
                }

                if (!string.IsNullOrEmpty(OwnerName))
                {
                    OwnerName = OwnerName.Trim();
                    DecTax = DecTax.Where(t => t.OwnerName.Contains(OwnerName));
                }

                if (!string.IsNullOrEmpty(DateType))
                {
                    if (DateType == "报关日期")
                    {
                        if (!string.IsNullOrEmpty(StartDate))
                        {
                            var from = DateTime.Parse(StartDate);
                            DecTax = DecTax.Where(t => t.DDate >= from);
                        }
                        if (!string.IsNullOrEmpty(EndDate))
                        {
                            var to = DateTime.Parse(EndDate).AddDays(1);
                            DecTax = DecTax.Where(t => t.DDate < to);
                        }
                    }
                    if (DateType == "缴税日期")
                    {

                    }
                    if (DateType == "抵扣日期")
                    {

                    }
                }

                //创建数据
                var linq = DecTax.ToArray().Select((t, i) => new
                {
                    客户名称 = t.OwnerName,
                    合同号 = t.ContrNo,
                    订单号 = t.OrderID,
                    海关编号 = t.EntryId,
                    币种 = t.Currency,
                    报关金额 = t.DecAmount.Value.ToRound(2),
                    委托金额 = t.OrderAgentAmount.Value.ToRound(2),
                    报关日期 = t.DDate?.ToString("yyyy-MM-dd"),
                    缴税日期 = t.VatfFlow?.PayDate?.ToShortDateString(),
                    抵扣日期 = t.VatfFlow?.DeductionTime?.ToShortDateString(),
                    开票类型 = t.InvoiceType.GetDescription(),
                    缴税状态 = t.DecTaxStatus.GetDescription(),
                    境外发货人 = t.ConsignorCode
                });
                IWorkbook workbook = ExcelFactory.Create();
                NPOIHelper npoi = new NPOIHelper(workbook);
                int[] columnsWidth = { 40, 20, 20, 25, 13, 13, 13, 13, 13, 13, 13, 13, 40 };
                npoi.EnumrableToExcel(linq, 0, columnsWidth);
                //创建文件夹
                var fileName = DateTime.Now.Ticks + ".xlsx";
                FileDirectory file = new FileDirectory(fileName);
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                file.CreateDataDirectory();
                //保存文件
                npoi.SaveAs(file.FilePath);

                Response.Write((new { success = true, message = "导出成功", url = file.FileUrl }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败" + ex.Message }).Json());
            }
        }

        protected void ExportSingleTypeFiles()
        {
            try
            {
                string InstanceType = Request.Form["InstanceType"];
                string FileType = Request.Form["FileType"];
                string FileTypeName = Request.Form["FileTypeName"];

                #region 查询
                string ContrNo = Request.Form["ContrNo"];
                string OrderID = Request.Form["OrderID"];
                string EntryID = Request.Form["EntryID"];
                string InvoiceType = Request.Form["InvoiceType"];
                string DecTaxStatus = Request.Form["DecTaxStatus"];
                string DateType = Request.Form["DateType"];
                string StartDate = Request.Form["StartDate"];
                string EndDate = Request.Form["EndDate"];
                string OwnerName = Request.Form["OwnerName"];

                var DecTax = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DecTax.AsQueryable();
                if (!string.IsNullOrEmpty(ContrNo))
                {
                    ContrNo = ContrNo.Trim();
                    DecTax = DecTax.Where(t => t.ContrNo.Contains(ContrNo));
                }
                if (!string.IsNullOrEmpty(OrderID))
                {
                    OrderID = OrderID.Trim();
                    DecTax = DecTax.Where(t => t.OrderID.Contains(OrderID));
                }
                if (!string.IsNullOrEmpty(EntryID))
                {
                    EntryID = EntryID.Trim();
                    DecTax = DecTax.Where(t => t.EntryId.Contains(EntryID));
                }
                if (!string.IsNullOrEmpty(InvoiceType))
                {
                    InvoiceType type = (InvoiceType)int.Parse(InvoiceType);
                    DecTax = DecTax.Where(t => t.InvoiceType == type);
                }
                if (!string.IsNullOrEmpty(DecTaxStatus))
                {
                    DecTaxStatus status = (DecTaxStatus)int.Parse(DecTaxStatus);
                    DecTax = DecTax.Where(t => t.DecTaxStatus == status);
                }

                if (!string.IsNullOrEmpty(OwnerName))
                {
                    OwnerName = OwnerName.Trim();
                    DecTax = DecTax.Where(t => t.OwnerName.Contains(OwnerName));
                }

                if (!string.IsNullOrEmpty(DateType))
                {
                    if (DateType == "报关日期")
                    {
                        if (!string.IsNullOrEmpty(StartDate))
                        {
                            var from = DateTime.Parse(StartDate);
                            DecTax = DecTax.Where(t => t.DDate >= from);
                        }
                        if (!string.IsNullOrEmpty(EndDate))
                        {
                            var to = DateTime.Parse(EndDate).AddDays(1);
                            DecTax = DecTax.Where(t => t.DDate < to);
                        }
                    }
                    if (DateType == "缴税日期")
                    {

                    }
                    if (DateType == "抵扣日期")
                    {

                    }
                }
                #endregion

                //1.创建文件夹(文件压缩后存放的地址)
                FileDirectory file = new FileDirectory();
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.ZipFiles);
                file.CreateDataDirectory();
                string filePrefix = file.RootDirectory;

                List<string> files = new List<string>();
                switch (InstanceType)
                {
                    //合同，发票
                    case "1":
                        var fileTypeRelated = new Needs.Ccs.Services.Views.BaseEdocCodesView().Where(e => e.Code == FileType).FirstOrDefault();
                        foreach (var item in DecTax)
                        {
                            var DecHead = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead[item.ID];
                            var fileRelated = DecHead.EdocRealations.Where(t => t.Edoc.Name == fileTypeRelated.Name).FirstOrDefault();
                            if (fileRelated != null)
                            {
                                files.Add((filePrefix + @"\" + fileRelated.FileUrl).ToUrl());
                            }
                        }
                        break;

                    //报关单，增值税，关税
                    case "2":
                        foreach (var item in DecTax)
                        {
                            var fileType = (Needs.Ccs.Services.Enums.FileType)Convert.ToInt16(FileType);
                            if (fileType == Needs.Ccs.Services.Enums.FileType.DecHeadTariffFile)
                            {
                                var fileTariff = item.TariffFile;
                                if (fileTariff != null)
                                {
                                    files.Add((filePrefix + @"\" + fileTariff.Url).ToUrl());
                                }
                            }
                            else if (fileType == Needs.Ccs.Services.Enums.FileType.DecHeadVatFile)
                            {
                                var fileVat = item.VatFile;
                                if (fileVat != null)
                                {
                                    files.Add((filePrefix + @"\" + fileVat.Url).ToUrl());
                                }
                            }
                            else if (fileType == Needs.Ccs.Services.Enums.FileType.DecHeadFile)
                            {
                                var fileDec = item.DecFile;
                                if (fileDec != null)
                                {
                                    files.Add((filePrefix + @"\" + fileDec.Url).ToUrl());
                                }
                            }
                        }
                        break;

                    default:
                        break;
                }

                string zipFileName = FileTypeName + DateTime.Now.ToString("yyyyMMddHHmmss") + Needs.Ccs.Services.SysConfig.Postfix;
                ZipFile zip = new ZipFile(zipFileName);
                zip.SetFilePath(file.FilePath);
                zip.Files = files;
                zip.ZipFiles();
                Response.Write((new { success = true, message = "导出成功", url = file.FileUrl + zipFileName }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }

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

                List<UnDeductionDecTax2> UnDeductionDecTaxList = new List<UnDeductionDecTax2>();
                List<string> errStrs = new List<string>();

                string DeductionMonthStr = dt.Rows[1][5].ToString();  // 所属月份
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

                for (int i = 3; i < dt.Rows.Count; i++)
                {
                    string TaxNumberStr = dt.Rows[i][1].ToString(); // 海关缴款书号码
                    string VaildAmountStr = dt.Rows[i][4].ToString(); // 有效税额
                    string DeductionTimeStr = dt.Rows[i][6].ToString(); // 抵扣日期

                    decimal VaildAmountDec;
                    DateTime DeductionTimeDt;

                    if (string.IsNullOrEmpty(TaxNumberStr))
                    {
                        continue;
                    }
                    TaxNumberStr = TaxNumberStr.Trim();
                    if (string.IsNullOrEmpty(TaxNumberStr))
                    {
                        continue;
                    }

                    if (!decimal.TryParse(VaildAmountStr, out VaildAmountDec))
                    {
                        errStrs.Add("有效税额 " + VaildAmountStr + " 不是有效的数字(" + TaxNumberStr + ")");
                    }
                    else if (!DateTime.TryParse(DeductionTimeStr, out DeductionTimeDt))
                    {
                        errStrs.Add("抵扣日期 " + DeductionTimeStr + " 不是有效的日期格式(" + TaxNumberStr + ")");
                    }
                    else
                    {
                        if (DeductionMonthDt != null)
                        {
                            UnDeductionDecTaxList.Add(new UnDeductionDecTax2(TaxNumberStr, VaildAmountDec, DeductionTimeDt, (DateTime)DeductionMonthDt));
                        }
                    }
                }

                if (DeductionMonthDt == null)
                {
                    Response.Write((new { success = false, message = "导入失败", errs = errStrs.ToArray() }).Json());
                    return;
                }

                //检查通过，更新数据
                foreach (var UnDeductionDecTaxItem in UnDeductionDecTaxList)
                {
                    var resultDeduction = UnDeductionDecTaxItem.Deduction();
                    if (resultDeduction.Item1 == false)
                    {
                        errStrs.Add(resultDeduction.Item2);
                    }
                }

                if (errStrs.Count == 0)
                {
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