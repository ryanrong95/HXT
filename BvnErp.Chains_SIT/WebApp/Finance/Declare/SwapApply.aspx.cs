using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.Finance.Declare
{
    public partial class SwapApply : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                load();
            }
        }

        private void load()
        {
            this.Model.Status = Needs.Ccs.Services.MultiEnumUtils.ToDictionary<Needs.Ccs.Services.Enums.CusDecStatus>()
                .Select(item => new { Value = item.Key, Text = item.Value }).Json();

            //币种
            this.Model.CurrData = Needs.Wl.Admin.Plat.AdminPlat.Currencies.Select(item => new { Value = item.Code, Text = item.Code + " " + item.Name }).Json();

            //境外发货人
            this.Model.ConsignorCodeData = Needs.Ccs.Services.MultiEnumUtils.ToDictionary<Needs.Ccs.Services.Enums.ConsignorCode>()
                .Select(item => new { Value = item.Key, Text = item.Value }).Json();
        }

        protected void data1()
        {
            string ContrNo = Request.QueryString["ContrNo"];
            string OrderID = Request.QueryString["OrderID"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string Currency = Request.QueryString["Currency"];
            string ConsignorCode = Request.QueryString["ConsignorCode"];

            var SwapDecHead = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapDecHead.AsQueryable()
                .Where(item => item.SwapStatus == SwapStatus.UnAuditing)
                .Where(item => (item.files.Where(t => t.FileType == FileType.DecHeadFile)).Count() != 0);

            if (!string.IsNullOrEmpty(ContrNo))
            {
                ContrNo = ContrNo.Trim();
                SwapDecHead = SwapDecHead.Where(t => t.ContrNo.Contains(ContrNo));
            }
            if (!string.IsNullOrEmpty(OrderID))
            {
                OrderID = OrderID.Trim();
                SwapDecHead = SwapDecHead.Where(t => t.OrderID.Contains(OrderID));
            }
            if (!string.IsNullOrEmpty(Currency))
            {
                SwapDecHead = SwapDecHead.Where(t => t.Currency == Currency);
            }
            if (!string.IsNullOrEmpty(ConsignorCode))
            {
                SwapDecHead = SwapDecHead.Where(t => t.ConsignorCode == ConsignorCode);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                DateTime start = Convert.ToDateTime(StartDate);
                SwapDecHead = SwapDecHead.Where(t => t.DDate >= start);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                SwapDecHead = SwapDecHead.Where(t => t.DDate < end);
            }

            SwapDecHead = SwapDecHead.OrderByDescending(t => t.CreateTime);

            Func<SwapDecHead, object> convert = head => new
            {
                ID = head.ID,
                ContrNo = head.ContrNo,
                OrderID = head.OrderID,
                EntryID = head.EntryId,
                Currency = head.Currency,
                SwapAmount = head.SwapAmount,
                DDate = head.DDate?.ToString("yyyy-MM-dd"),
                SwapStatus = head.SwapStatus.GetDescription(),
                URL = Needs.Utils.FileDirectory.Current.FileServerUrl + @"/" + head.decheadFile?.Url.ToUrl(),
                ConsignorCode = head.ConsignorCode,
            };
            this.Paging(SwapDecHead, convert);
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ContrNo = Request.QueryString["ContrNo"];
            string OrderID = Request.QueryString["OrderID"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string Currency = Request.QueryString["Currency"];
            string ConsignorCode = Request.QueryString["ConsignorCode"];

            //string strIsOnlyShowInSomeTime = Request.QueryString["IsOnlyShowInSomeTime"];
            //bool isOnlyShowInSomeTime = true;
            string strIsOnlyShowOverDate = Request.QueryString["IsOnlyShowOverDate"];
            bool isOnlyShowOverDate = false;
            string strIsOnlyShowPrePayExchange = Request.QueryString["IsOnlyShowPrePayExchange"];
            bool isOnlyShowPrePayExchange = false;
            string strIsOnlyShowHasLimitArea = Request.QueryString["IsOnlyShowHasLimitArea"];
            bool isOnlyShowHasLimitArea = false;
            var type = Request.QueryString["ClientType"];
            string OwnerName = Request.QueryString["OwnerName"];

            string strIsExpssive = Request.QueryString["IsExpssive"];
            string strIsInExpssive = Request.QueryString["IsInExpensive"];


            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            if (!string.IsNullOrEmpty(ContrNo))
            {
                ContrNo = ContrNo.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.ContrNo.Contains(ContrNo)));
            }
            if (!string.IsNullOrEmpty(OrderID))
            {
                OrderID = OrderID.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.OrderID.Contains(OrderID)));
            }
            if (!string.IsNullOrEmpty(Currency))
            {
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.Currency == Currency));
            }
            if (!string.IsNullOrEmpty(ConsignorCode))
            {
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.ConsignorCode == ConsignorCode));
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                DateTime start = Convert.ToDateTime(StartDate);
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.DDate >= start));
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.DDate < end));
            }

            //if (!string.IsNullOrEmpty(strIsOnlyShowInSomeTime))
            //{
            //    bool.TryParse(strIsOnlyShowInSomeTime, out isOnlyShowInSomeTime);
            //}
            //if (isOnlyShowInSomeTime)
            //{
            //    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.DDate != null && (DateTime.Now - t.DDate.Value).Days <= 90));
            //}

            if (!string.IsNullOrEmpty(strIsOnlyShowOverDate))
            {
                bool.TryParse(strIsOnlyShowOverDate, out isOnlyShowOverDate);
            }
            if (isOnlyShowOverDate)
            {
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.IsOverDate == true));
            }

            if (!string.IsNullOrEmpty(strIsOnlyShowPrePayExchange))
            {
                bool.TryParse(strIsOnlyShowPrePayExchange, out isOnlyShowPrePayExchange);
            }
            if (isOnlyShowPrePayExchange)
            {
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.IsPrePayExchange == true));
            }

            if (!string.IsNullOrEmpty(strIsOnlyShowHasLimitArea))
            {
                bool.TryParse(strIsOnlyShowHasLimitArea, out isOnlyShowHasLimitArea);
            }
            if (isOnlyShowHasLimitArea)
            {
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.IsHasLimitArea == true));
            }

            if (!string.IsNullOrEmpty(type))
            {
                int itype = Int32.Parse(type);
                Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>> lambda = item => item.ClientType == (ClientType)itype;
                lamdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(OwnerName))
            {
                OwnerName = OwnerName.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.OwnerName.Contains(OwnerName)));
            }


            if ((strIsInExpssive == "true" && strIsExpssive == "true") || ((strIsInExpssive == "false" && strIsExpssive == "false")))
            {
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.IsExcessive == true || t.IsExcessive == false));

            }
            else
            {
                bool IsExpssive;
                if ((!string.IsNullOrEmpty(strIsExpssive)))
                {
                    bool.TryParse(strIsExpssive, out IsExpssive);
                    if (IsExpssive)
                        lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.IsExcessive == true));
                }

                bool IsInExpssive;
                if (!string.IsNullOrEmpty(strIsInExpssive))
                {
                    bool.TryParse(strIsInExpssive, out IsInExpssive);
                    if (IsInExpssive)
                        lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.IsExcessive == false));

                }
            }

            int totalCount = 0;

            var unSwapDecHeadList = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.UnSwapDecHeadListView.GetResult(out totalCount, page, rows, lamdas.ToArray());

            Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, object> convert = unSwapDecHead => new
            {
                ID = unSwapDecHead.DecHeadID,
                ContrNo = unSwapDecHead.ContrNo,
                OrderID = unSwapDecHead.OrderID,
                OwnerName = unSwapDecHead.OwnerName,
                EntryID = unSwapDecHead.EntryId,
                Currency = unSwapDecHead.Currency,
                SwapAmount = unSwapDecHead.SwapAmount,
                DDate = unSwapDecHead.DDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                SwapStatus = unSwapDecHead.SwapStatus.GetDescription(),
                URL = Needs.Utils.FileDirectory.Current.FileServerUrl + @"/" + unSwapDecHead?.Url.ToUrl(),
                DecHeadStatus = MultiEnumUtils.ToText<Needs.Ccs.Services.Enums.CusDecStatus>(
                    (Needs.Ccs.Services.Enums.CusDecStatus)Enum.Parse(typeof(Needs.Ccs.Services.Enums.CusDecStatus), unSwapDecHead.CusDecStatus)),
                SwapedAmount = unSwapDecHead.SwapedAmount,
                UnSwapedAmount = unSwapDecHead.SwapAmount - unSwapDecHead.SwapedAmount,
                UserCurrentPayApply = unSwapDecHead.UserCurrentPayApply.ToRound(2),
                SwapSpecialInfo = GetSwapSpecialInfo(unSwapDecHead),
                ConsignorCode = unSwapDecHead.ConsignorCode,
            };

            Response.Write(new
            {
                rows = unSwapDecHeadList.Select(convert).ToArray(),
                total = totalCount,
            }.Json());
        }

        /// <summary>
        /// 获取特殊报关单信息
        /// </summary>
        /// <param name="unSwapDecHead"></param>
        /// <returns></returns>
        private string GetSwapSpecialInfo(Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel unSwapDecHead)
        {
            List<string> listSpecialInfo = new List<string>();

            //if (unSwapDecHead.DDate != null && (DateTime.Now - unSwapDecHead.DDate.Value).Days > 90)
            if (unSwapDecHead.IsOverDate)
            {
                listSpecialInfo.Add("超过90天");
            }

            if (unSwapDecHead.IsPrePayExchange)
            {
                listSpecialInfo.Add("预付汇");
            }

            if (unSwapDecHead.IsHasLimitArea)
            {
                listSpecialInfo.Add("有受限地区（" + string.Join("、", unSwapDecHead.LimitBankNames) + "）");
            }

            if (listSpecialInfo == null || !listSpecialInfo.Any())
            {
                return string.Empty;
            }

            return string.Join("、", listSpecialInfo.ToArray());
        }

        /// <summary>
        /// 导出 Excel
        /// </summary>
        protected void ExportExcel()
        {
            try
            {
                /*
                string ContrNo = Request.Form["ContrNo"];
                string OrderID = Request.Form["OrderID"];
                string StartDate = Request.Form["StartDate"];
                string EndDate = Request.Form["EndDate"];
                string Currency = Request.Form["Currency"];
                string strIsOnlyShowInSomeTime = Request.Form["IsOnlyShowInSomeTime"];
                bool isOnlyShowInSomeTime = true;
                var type = Request.Form["ClientType"];
                
                var predicate = PredicateBuilder.Create<Needs.Ccs.Services.Views.ExportUnSwapDecHeadListModel>();

                if (!string.IsNullOrEmpty(ContrNo))
                {
                    ContrNo = ContrNo.Trim();
                    predicate = predicate.And(t => t.ContrNo.Contains(ContrNo));
                }
                if (!string.IsNullOrEmpty(OrderID))
                {
                    OrderID = OrderID.Trim();
                    predicate = predicate.And(t => t.OrderID.Contains(OrderID));
                }
                if (!string.IsNullOrEmpty(Currency))
                {
                    predicate = predicate.And(t => t.Currency == Currency);
                }
                if (!string.IsNullOrEmpty(StartDate))
                {
                    DateTime start = Convert.ToDateTime(StartDate);
                    predicate = predicate.And(t => t.DDate >= start);
                }
                if (!string.IsNullOrEmpty(EndDate))
                {
                    DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                    predicate = predicate.And(t => t.DDate < end);
                }

                if (!string.IsNullOrEmpty(strIsOnlyShowInSomeTime))
                {
                    bool.TryParse(strIsOnlyShowInSomeTime, out isOnlyShowInSomeTime);
                }
                if (isOnlyShowInSomeTime)
                {
                    predicate = predicate.And(t => t.DDate != null && (DateTime.Now - t.DDate.Value).Days <= 90);
                }
                if (!string.IsNullOrEmpty(type))
                {
                    int itype = Int32.Parse(type);
                    predicate = predicate.And(t => t.ClientType == (ClientType)itype);
                }

                Needs.Ccs.Services.Views.ExportUnSwapDecHeadListView view = new Needs.Ccs.Services.Views.ExportUnSwapDecHeadListView();
                view.AllowPaging = false;
                //view.PageIndex = page;
                //view.PageSize = rows;
                view.Predicate = predicate;

                var dataList = view.ToList();
                */

                int page, rows;
                int.TryParse(Request.QueryString["page"], out page);
                int.TryParse(Request.QueryString["rows"], out rows);

                string ContrNo = Request.Form["ContrNo"];
                string OrderID = Request.Form["OrderID"];
                string StartDate = Request.Form["StartDate"];
                string EndDate = Request.Form["EndDate"];
                string Currency = Request.Form["Currency"];
                string ConsignorCode = Request.Form["ConsignorCode"];
                //string strIsOnlyShowInSomeTime = Request.QueryString["IsOnlyShowInSomeTime"];
                //bool isOnlyShowInSomeTime = true;
                string strIsOnlyShowOverDate = Request.Form["IsOnlyShowOverDate"];
                bool isOnlyShowOverDate = false;
                string strIsOnlyShowPrePayExchange = Request.Form["IsOnlyShowPrePayExchange"];
                bool isOnlyShowPrePayExchange = false;
                string strIsOnlyShowHasLimitArea = Request.Form["IsOnlyShowHasLimitArea"];
                bool isOnlyShowHasLimitArea = false;
                var type = Request.Form["ClientType"];
                string OwnerName = Request.Form["OwnerName"];

                string strIsExpssive = Request.Form["IsExpssive"];
                string strIsInExpssive = Request.Form["IsInExpensive"];

                List<LambdaExpression> lamdas = new List<LambdaExpression>();

                if (!string.IsNullOrEmpty(ContrNo))
                {
                    ContrNo = ContrNo.Trim();
                    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.ExportUnSwapDecHeadListViewNewModel, bool>>)(t => t.ContrNo.Contains(ContrNo)));
                }
                if (!string.IsNullOrEmpty(OrderID))
                {
                    OrderID = OrderID.Trim();
                    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.ExportUnSwapDecHeadListViewNewModel, bool>>)(t => t.OrderID.Contains(OrderID)));
                }
                if (!string.IsNullOrEmpty(Currency))
                {
                    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.ExportUnSwapDecHeadListViewNewModel, bool>>)(t => t.Currency == Currency));
                }
                if (!string.IsNullOrEmpty(ConsignorCode))
                {
                    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.ExportUnSwapDecHeadListViewNewModel, bool>>)(t => t.ConsignorCode == ConsignorCode));
                }
                if (!string.IsNullOrEmpty(StartDate))
                {
                    DateTime start = Convert.ToDateTime(StartDate);
                    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.ExportUnSwapDecHeadListViewNewModel, bool>>)(t => t.DDate >= start));
                }
                if (!string.IsNullOrEmpty(EndDate))
                {
                    DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.ExportUnSwapDecHeadListViewNewModel, bool>>)(t => t.DDate < end));
                }

                if (!string.IsNullOrEmpty(strIsOnlyShowOverDate))
                {
                    bool.TryParse(strIsOnlyShowOverDate, out isOnlyShowOverDate);
                }
                if (isOnlyShowOverDate)
                {
                    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.ExportUnSwapDecHeadListViewNewModel, bool>>)(t => t.IsOverDate == true));
                }

                if (!string.IsNullOrEmpty(strIsOnlyShowPrePayExchange))
                {
                    bool.TryParse(strIsOnlyShowPrePayExchange, out isOnlyShowPrePayExchange);
                }
                if (isOnlyShowPrePayExchange)
                {
                    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.ExportUnSwapDecHeadListViewNewModel, bool>>)(t => t.IsPrePayExchange == true));
                }

                if (!string.IsNullOrEmpty(strIsOnlyShowHasLimitArea))
                {
                    bool.TryParse(strIsOnlyShowHasLimitArea, out isOnlyShowHasLimitArea);
                }
                if (isOnlyShowHasLimitArea)
                {
                    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.ExportUnSwapDecHeadListViewNewModel, bool>>)(t => t.IsHasLimitArea == true));
                }

                if (!string.IsNullOrEmpty(type))
                {
                    int itype = Int32.Parse(type);
                    Expression<Func<Needs.Ccs.Services.Views.ExportUnSwapDecHeadListViewNewModel, bool>> lambda = item => item.ClientType == (ClientType)itype;
                    lamdas.Add(lambda);
                }
                if (!string.IsNullOrEmpty(OwnerName))
                {
                    OwnerName = OwnerName.Trim();
                    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.ExportUnSwapDecHeadListViewNewModel, bool>>)(t => t.OwnerName.Contains(OwnerName)));
                }

                //if ((strIsInExpssive == "true" && strIsExpssive == "true") || ((strIsInExpssive == "false" && strIsExpssive == "false")))
                //{
                //    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.IsExcessive == true || t.IsExcessive == false));

                //}
                //else
                //{
                //    bool IsExpssive;
                //    if ((!string.IsNullOrEmpty(strIsExpssive)))
                //    {
                //        bool.TryParse(strIsExpssive, out IsExpssive);
                //        if (IsExpssive)
                //            lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.IsExcessive == true));
                //    }

                //    bool IsInExpssive;
                //    if (!string.IsNullOrEmpty(strIsInExpssive))
                //    {
                //        bool.TryParse(strIsInExpssive, out IsInExpssive);
                //        if (IsInExpssive)
                //            lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.UnSwapDecHeadListView.UnSwapDecHeadListModel, bool>>)(t => t.IsExcessive == false));

                //    }
                //}
                int totalCount = 0;

                var dataList = new Needs.Ccs.Services.Views.ExportUnSwapDecHeadListViewNew().GetResults(lamdas.ToArray()).ToList();

                Func<Needs.Ccs.Services.Views.ExportUnSwapDecHeadListViewNewModel, object> convert = unSwapDecHead => new
                {
                    DecHeadID = unSwapDecHead.DecHeadID,
                    DDate = unSwapDecHead.DDate?.ToString("yyyy-MM-dd"), //报关日期
                    OwnerName = unSwapDecHead.OwnerName, //客户名称
                    ConsignorCode = unSwapDecHead.ConsignorCode, //境外发货人
                    ContrNo = unSwapDecHead.ContrNo, //合同号
                    OrderID = unSwapDecHead.OrderID, //订单号
                    EntryId = unSwapDecHead.EntryId, //海关编号
                    Currency = unSwapDecHead.Currency, //币种
                    DeclTotalSum = unSwapDecHead.DeclTotalSum?.ToRound(2), //报关金额
                    DeclarePrice = unSwapDecHead.DeclarePrice?.ToRound(2), //委托金额
                    UpSwapedAmount = unSwapDecHead.UpSwapedAmount.ToRound(2), //未换汇金额
                    SwapedAmount = unSwapDecHead.SwapedAmount?.ToRound(2), //已换汇金额
                };

                //写入数据
                DataTable dt = NPOIHelper.JsonToDataTable(dataList.Select(convert).ToArray().Json());

                string fileName = DateTime.Now.Ticks + ".xls";

                //创建文件目录
                FileDirectory fileDic = new FileDirectory(fileName);
                fileDic.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                fileDic.CreateDataDirectory();

                #region 设置导出格式

                var excelconfig = new ExcelConfig();
                excelconfig.FilePath = fileDic.FilePath;
                excelconfig.Title = "未换汇";
                excelconfig.TitleFont = "微软雅黑";
                excelconfig.TitlePoint = 16;
                excelconfig.IsAllSizeColumn = true;
                //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
                List<ColumnEntity> listColumnEntity = new List<ColumnEntity>();
                excelconfig.ColumnEntity = listColumnEntity;
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DDate", ExcelColumn = "报关日期", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OwnerName", ExcelColumn = "客户名称", Alignment = "left" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ConsignorCode", ExcelColumn = "境外发货人", Alignment = "left" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "ContrNo", ExcelColumn = "合同号", Alignment = "left" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "OrderID", ExcelColumn = "订单号", Alignment = "left" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "EntryId", ExcelColumn = "海关编号", Alignment = "left" });

                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "Currency", ExcelColumn = "币种", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DeclTotalSum", ExcelColumn = "报关金额", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "DeclarePrice", ExcelColumn = "委托金额", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "UpSwapedAmount", ExcelColumn = "未换汇金额", Alignment = "center" });
                excelconfig.ColumnEntity.Add(new ColumnEntity() { Column = "SwapedAmount", ExcelColumn = "已换汇金额", Alignment = "center" });

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
