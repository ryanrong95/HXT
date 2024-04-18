using Needs.Ccs.Services;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.Finance.Receipt.Record
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var orderFeeType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.OrderFeeType>().Select(item => new { item.Key, item.Value });
            this.Model.OrderFeeType = orderFeeType.Json();
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string CreateDateStartDate = Request.QueryString["CreateDateStartDate"];
            string CreateDateEndDate = Request.QueryString["CreateDateEndDate"];
            string OrderID = Request.QueryString["OrderID"];
            string ReceiptDateStartDate = Request.QueryString["ReceiptDateStartDate"];
            string ReceiptDateEndDate = Request.QueryString["ReceiptDateEndDate"];
            string FeeType = Request.QueryString["FeeType"];
            string SeqNo = Request.QueryString["SeqNo"];

            using (var query = new Needs.Ccs.Services.Views.ReceiveRecordListView())
            {
                var view = query;

                if (!string.IsNullOrEmpty(CreateDateStartDate))
                {
                    DateTime begin = DateTime.Parse(CreateDateStartDate);
                    view = view.SearchByCreateDateBegin(begin);
                }
                if (!string.IsNullOrEmpty(CreateDateEndDate))
                {
                    DateTime end = DateTime.Parse(CreateDateEndDate);
                    end = end.AddDays(1);
                    view = view.SearchByCreateDateEnd(end);
                }
                if (!string.IsNullOrEmpty(OrderID))
                {
                    OrderID = OrderID.Trim();
                    view = view.SearchByOrderID(OrderID);
                }
                if (!string.IsNullOrEmpty(ReceiptDateStartDate))
                {
                    DateTime begin = DateTime.Parse(ReceiptDateStartDate);
                    view = view.SearchByReceiptDateBegin(begin);
                }
                if (!string.IsNullOrEmpty(ReceiptDateEndDate))
                {
                    DateTime end = DateTime.Parse(ReceiptDateEndDate);
                    end = end.AddDays(1);
                    view = view.SearchByReceiptDateEnd(end);
                }
                if (!string.IsNullOrEmpty(FeeType))
                {
                    Needs.Ccs.Services.Enums.OrderFeeType orderFeeType = (Needs.Ccs.Services.Enums.OrderFeeType)(int.Parse(FeeType));
                    view = view.SearchByFeeType(orderFeeType);
                }
                if (!string.IsNullOrEmpty(SeqNo))
                {
                    SeqNo = SeqNo.Trim();
                    view = view.SearchBySeqNo(SeqNo);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }


        protected void Export()
        {
            string CreateDateStartDate = Request.Form["CreateDateStartDate"];
            string CreateDateEndDate = Request.Form["CreateDateEndDate"];
            string OrderID = Request.Form["OrderID"];
            string ReceiptDateStartDate = Request.Form["ReceiptDateStartDate"];
            string ReceiptDateEndDate = Request.Form["ReceiptDateEndDate"];
            string FeeType = Request.Form["FeeType"];
            string SeqNo = Request.Form["SeqNo"];

            var view = new Needs.Ccs.Services.Views.ReceiveRecordListView();

            if (!string.IsNullOrEmpty(CreateDateStartDate))
            {
                DateTime begin = DateTime.Parse(CreateDateStartDate);
                view = view.SearchByCreateDateBegin(begin);
            }
            if (!string.IsNullOrEmpty(CreateDateEndDate))
            {
                DateTime end = DateTime.Parse(CreateDateEndDate);
                end = end.AddDays(1);
                view = view.SearchByCreateDateEnd(end);
            }
            if (!string.IsNullOrEmpty(OrderID))
            {
                OrderID = OrderID.Trim();
                view = view.SearchByOrderID(OrderID);
            }
            if (!string.IsNullOrEmpty(ReceiptDateStartDate))
            {
                DateTime begin = DateTime.Parse(ReceiptDateStartDate);
                view = view.SearchByReceiptDateBegin(begin);
            }
            if (!string.IsNullOrEmpty(ReceiptDateEndDate))
            {
                DateTime end = DateTime.Parse(ReceiptDateEndDate);
                end = end.AddDays(1);
                view = view.SearchByReceiptDateEnd(end);
            }
            if (!string.IsNullOrEmpty(FeeType))
            {
                Needs.Ccs.Services.Enums.OrderFeeType orderFeeType = (Needs.Ccs.Services.Enums.OrderFeeType)(int.Parse(FeeType));
                view = view.SearchByFeeType(orderFeeType);
            }
            if (!string.IsNullOrEmpty(SeqNo))
            {
                SeqNo = SeqNo.Trim();
                view = view.SearchBySeqNo(SeqNo);
            }

            var result = (Needs.Ccs.Services.Views.ReceiveRecordListViewModel[])view.ToMyPage();

            Func<Needs.Ccs.Services.Views.ReceiveRecordListViewModel, object> convert = item => new
            {
                OrderReceiptID = item.OrderReceiptID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                AdminName = item.AdminName,
                item.OrderID,
                FeeTypeInt = (int)item.FeeType,
                FeeTypeName = item.FeeTypeShowName,
                ReceiptTypeName = (0 - item.Amount) > 0 ? "收款" : "冲正",
                Amount = (0 - item.Amount).ToString("#0.00"),
                SeqNo = item.SeqNo,
                ReceiptDate = item.ReceiptDate?.ToString("yyyy-MM-dd"),
                // 2020-09-27 by yeshuangshuang
                ReceiptAmount = item.ReceiptAmount,
                Payer = item.Payer,
                FinanceReceiptID = item.FinanceReceiptID
            };
            if (result.Count() == 0)
            {
                Response.Write((new
                {
                    success = false,
                    message = "暂无数据导出"
                }).Json());
                return;
            }

            //写入数据
            DataTable dt = NPOIHelper.JsonToDataTable(result.Select(convert).ToArray().Json());
            string fileName = DateTime.Now.Ticks + ".xlsx";
            //创建文件目录
            FileDirectory fileDic = new FileDirectory(fileName);
            fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
            fileDic.CreateDataDirectory();

            #region 设置导出格式
            var excelconfig = new ExcelConfig();
            excelconfig.FilePath = fileDic.FilePath;
            excelconfig.Title = "收款记录";
            excelconfig.TitleFont = "微软雅黑";
            excelconfig.TitlePoint = 16;
            excelconfig.IsAllSizeColumn = true;
            //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
            List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
            excelconfig.ColumnEntity = listColumnEntity;
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "CreateDate", ExcelColumn = "收款时间", Alignment = "center" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "AdminName", ExcelColumn = "收款人", Alignment = "center" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OrderID", ExcelColumn = "订单编号", Alignment = "center" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "FeeTypeName", ExcelColumn = "费用类型", Alignment = "center" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ReceiptTypeName", ExcelColumn = "收款类型", Alignment = "center" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Amount", ExcelColumn = "实收", Alignment = "center" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SeqNo", ExcelColumn = "银行流水号", Alignment = "center" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "FinanceReceiptID", ExcelColumn = "收款编号", Alignment = "center" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ReceiptAmount", ExcelColumn = "收款金额", Alignment = "center" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Payer", ExcelColumn = "客户名称", Alignment = "center" });
            excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ReceiptDate", ExcelColumn = "银行流水收款时间", Alignment = "center" });
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

        /// <summary>
        /// 取消收款
        /// </summary>
        protected void UnmackReceipt()
        {
            try
            {
                string OrderID = Request.Form["OrderID"];
                string FeeTypeIntString = Request.Form["FeeTypeInt"];

                var targetUnmacks = (from orderReceipt in Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderReceiptsAllStatus
                                     where orderReceipt.OrderID == OrderID
                                          && orderReceipt.Status == Needs.Ccs.Services.Enums.Status.Normal
                                          && orderReceipt.Type == Needs.Ccs.Services.Enums.OrderReceiptType.Received
                                          && orderReceipt.FeeType == (Needs.Ccs.Services.Enums.OrderFeeType)int.Parse(FeeTypeIntString)
                                     select orderReceipt).ToList();

                if (targetUnmacks == null || !targetUnmacks.Any())
                {
                    Response.Write((new { success = "false", message = "提交失败,不存在有效的收款项,OrderID = " + OrderID + ", FeeTypeInt = " + FeeTypeIntString, }).Json());
                    return;
                }

                var Admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                foreach (var targetUnmack in targetUnmacks)
                {

                    var unmarkReceivedChongZhen = new Needs.Ccs.Services.Models.UnmarkOrderReceipt();
                    unmarkReceivedChongZhen.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderReceipt);//主键ID（OrderReceipt +8位年月日+6位流水号）
                    unmarkReceivedChongZhen.ReceiptNoticeID = targetUnmack.FinanceReceiptID;
                    unmarkReceivedChongZhen.ClientID = targetUnmack.ClientID;
                    unmarkReceivedChongZhen.OrderID = targetUnmack.OrderID;
                    unmarkReceivedChongZhen.FeeSourceID = targetUnmack.FeeSourceID;
                    unmarkReceivedChongZhen.FeeType = targetUnmack.FeeType;
                    unmarkReceivedChongZhen.Amount = targetUnmack.Amount;
                    unmarkReceivedChongZhen.Admin = Admin;
                    unmarkReceivedChongZhen.Enter();

                    var unmarkReceivedOrigin = new Needs.Ccs.Services.Models.OrderReceived(targetUnmack, Needs.Ccs.Services.Enums.Status.Delete);
                    unmarkReceivedOrigin.Enter();

                    //取消垫资记录
                    var advanceRecords = new Needs.Ccs.Services.Views.AdvanceMoneyRecordView().Where(t => t.ClientID == targetUnmack.ClientID
                        && t.OrderID == targetUnmack.OrderID && t.PayExchangeID == targetUnmack.FeeSourceID).ToList();
                    if (advanceRecords != null && advanceRecords.Any())
                    {
                        var orderReceiptSum = ((from orderReceipt in Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderReceiptsAllStatus
                                                where orderReceipt.ID == unmarkReceivedChongZhen.ID || orderReceipt.ID == targetUnmack.ID
                                                     && orderReceipt.Status == Needs.Ccs.Services.Enums.Status.Delete
                                                     && orderReceipt.Type == Needs.Ccs.Services.Enums.OrderReceiptType.Received
                                                select orderReceipt).ToList()).Sum(item => item.Amount);
                        if (orderReceiptSum == 0)
                        {
                            var advanceRecordModel = new Needs.Ccs.Services.Models.AdvanceRecordModel();
                            advanceRecordModel.ClientID = targetUnmack.ClientID;
                            advanceRecordModel.OrderID = targetUnmack.OrderID;
                            advanceRecordModel.PayExchangeID = targetUnmack.FeeSourceID;
                            advanceRecordModel.AmountUsed = targetUnmack.Amount;
                            advanceRecordModel.AdvanceRecordUpdate();
                        }
                    }

                }

                #region 取消收款时, 处理 OrderReceipt 和 SwapNotice 的关联关系

                Needs.Ccs.Services.Models.UnmackSwapNoticeUseHandler unmackSwapNoticeUseHandler = new Needs.Ccs.Services.Models.UnmackSwapNoticeUseHandler(targetUnmacks.ToArray());
                unmackSwapNoticeUseHandler.Execute();

                #endregion

                #region 取消单个收款调用 Yahv

                var admin2 = new Needs.Ccs.Services.Views.AdminsTopView2().Where(t => t.OriginID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID).FirstOrDefault();
                Needs.Ccs.Services.Models.UnmackOneReceiptToYahv unmackOneReceiptToYahv = new Needs.Ccs.Services.Models.UnmackOneReceiptToYahv(admin2, OrderID, int.Parse(FeeTypeIntString));
                unmackOneReceiptToYahv.Execute();

                #endregion

                Response.Write((new { success = "true", message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = "false", message = "提交失败" + ex.Message }).Json());
            }
        }

    }
}