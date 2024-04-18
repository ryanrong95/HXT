using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Layer.Data.Sqls.BvScsm;
using Needs.Ccs.Services;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Npoi;
using Needs.Utils.Serializers;
using Needs.Wl.Views;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using NPOI.XSSF.UserModel;

namespace WebApp.GeneralManage.Receipt.Admin
{
    /// <summary>
    /// 管理员的待收款查询界面
    /// </summary>
    public partial class UnReceiveList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            //Roles（Name=“业务员”）-> AdminRoles-> RealName
            this.Model.Salesman = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles.Where(s => s.Role.Name == "业务员").Select(t => new
            {
                Value = t.Admin.ID,
                Text = t.Admin.RealName
            }).Json();

            this.Model.PeriodType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.PeriodType>().Select(item => new { Value = item.Key, Text = item.Value }).Json();

            //页面上所有的欠款
            //var orderReceipts = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderUnReceiveStats.AsQueryable();
            //this.Model.Overdraft = orderReceipts.Sum(t => (decimal?)t.Overdraft).GetValueOrDefault().Json();
        }
        protected void data()
        {
            string ClientCode = Request.QueryString["ClientCode"];
            string OrderId = Request.QueryString["OrderId"];
            string SettlementDate = Request.QueryString["SettlementDate"];
            var Salesman = Request.QueryString["Salesman"];
            var PeriodType = Request.QueryString["PeriodType"];
            var StartDate = Request.QueryString["StartDate"];
            var EndDate = Request.QueryString["EndDate"];

            var orderReceipts = new Needs.Ccs.Services.Views.OrderUnReceiveByAgreementView().AsQueryable();

            if (!string.IsNullOrEmpty(ClientCode))
            {
                orderReceipts = orderReceipts.Where(o => o.Client.ClientCode == ClientCode.Trim());
            }
            if (!string.IsNullOrEmpty(Salesman))
            {
                orderReceipts = orderReceipts.Where(o => o.Client.ServiceManager.ID == Salesman.Trim());
            }
            if (!string.IsNullOrEmpty(OrderId))
            {
                orderReceipts = orderReceipts.Where(o => o.ID == OrderId.Trim());
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                var from = DateTime.Parse(StartDate);
                orderReceipts = orderReceipts.Where(t => t.DDate >= from);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                var to = DateTime.Parse(EndDate);
                orderReceipts = orderReceipts.Where(item => item.DDate < to.AddDays(1));
            }
            //增加筛选条件
            if (!string.IsNullOrEmpty(PeriodType))
            {
                orderReceipts = orderReceipts.Where(o => o.PeriodType == PeriodType);
            }
            if (!string.IsNullOrEmpty(SettlementDate) && !string.IsNullOrEmpty(PeriodType))
            {
                if (int.Parse(PeriodType) == (int)Needs.Ccs.Services.Enums.PeriodType.Monthly)
                {
                    //月结日期
                    orderReceipts = orderReceipts.Where(o => o.MonthlyDay == SettlementDate);
                }
                else 
                {
                    //约定日期
                    orderReceipts = orderReceipts.Where(o => o.DaysLimit == SettlementDate);
                }
            }

            orderReceipts = orderReceipts.OrderByDescending(item => item.DDate);

            Func<Needs.Ccs.Services.Models.OrderReceiptStats, Needs.Ccs.Services.Models.OrderReceiptStatViewModel> convert = t => new Needs.Ccs.Services.Models.OrderReceiptStatViewModel
            {
                ClientCode = t.Client.ClientCode,
                CompanyName = t.Client.Company.Name,
                OrderId = t.ID,
                OrderStatus = t.OrderStatus.GetDescription(),
                DeclareDate = t.DDate?.ToShortDateString(),
                DeclarePrice = (t.DeclarePrice * t.RealExchangeRate).ToRound(2).ToString("0.00"),
                Receivable = t.Receivable,
                Received = t.Received,
                Overdraft = t.Overdraft,
                Salesman = t.Client.ServiceManager.RealName,
                Merchandiser = t.Client.Merchandiser.RealName,
                PeriodType = t.PeriodType,
                PeriodTypeDesc = ((Needs.Ccs.Services.Enums.PeriodType)int.Parse(t.PeriodType)).GetDescription(),
                SettlementDate = int.Parse(t.PeriodType) == Needs.Ccs.Services.Enums.PeriodType.PrePaid.GetHashCode() ? "-"
                : (int.Parse(t.PeriodType) == Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod.GetHashCode() ? t.DaysLimit.ToString() : t.MonthlyDay.ToString())
            };

            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            int total = orderReceipts.Count();
            var sumDraft = total == 0 ? 0M : orderReceipts?.Sum(t => t.Overdraft);
            var query = orderReceipts?.Skip(rows * (page - 1)).Take(rows);

            var queryExeced = query?.Select(convert).ToArray();
            foreach (var item in queryExeced)
            {
                item.ExceedDays = CountExceed(item.DeclareDate, item.PeriodType, item.SettlementDate);
            }

            Response.Write(new
            {
                rows = queryExeced,
                total = total,
                sumOverdraft = sumDraft
            }.Json());
        }

        /// <summary>
        /// 导出欠款Excel
        /// </summary>
        protected void ExportExcel()
        {
            try {

                string ClientCode = Request.Form["ClientCode"];
                string OrderId = Request.Form["OrderId"];
                var Salesman = Request.Form["Salesman"];
                var StartDate = Request.Form["StartDate"];
                var EndDate = Request.Form["EndDate"];
                var PeriodType = Request.Form["PeriodType"];
                string SettlementDate = Request.Form["SettlementDate"];

                //var orderReceipts = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderUnReceiveStats.AsQueryable();
                var orderReceipts = new Needs.Ccs.Services.Views.OrderUnReceiveByAgreementView().AsQueryable();

                if (!string.IsNullOrEmpty(ClientCode))
                {
                    orderReceipts = orderReceipts.Where(o => o.Client.ClientCode == ClientCode.Trim());
                }
                if (!string.IsNullOrEmpty(Salesman))
                {
                    orderReceipts = orderReceipts.Where(o => o.Client.ServiceManager.ID == Salesman.Trim());
                }
                if (!string.IsNullOrEmpty(OrderId))
                {
                    orderReceipts = orderReceipts.Where(o => o.ID == OrderId.Trim());
                }
                if (!string.IsNullOrEmpty(StartDate))
                {
                    var from = DateTime.Parse(StartDate);
                    orderReceipts = orderReceipts.Where(t => t.DDate >= from);
                }
                if (!string.IsNullOrEmpty(EndDate))
                {
                    var to = DateTime.Parse(EndDate);
                    orderReceipts = orderReceipts.Where(item => item.DDate < to.AddDays(1));
                }
                //增加筛选条件
                if (!string.IsNullOrEmpty(PeriodType))
                {
                    orderReceipts = orderReceipts.Where(o => o.PeriodType == PeriodType);
                }
                if (!string.IsNullOrEmpty(SettlementDate) && !string.IsNullOrEmpty(PeriodType))
                {
                    if (int.Parse(PeriodType) == (int)Needs.Ccs.Services.Enums.PeriodType.Monthly)
                    {
                        //月结日期
                        orderReceipts = orderReceipts.Where(o => o.MonthlyDay == SettlementDate);
                    }
                    else
                    {
                        //约定日期
                        orderReceipts = orderReceipts.Where(o => o.DaysLimit == SettlementDate);
                    }
                }

                orderReceipts = orderReceipts.OrderByDescending(item => item.DDate);

                #region 未收款
                var detailData = orderReceipts.ToList().Select(d => new Needs.Ccs.Services.Models.OrderReceiptStatViewModel
                {
                    ClientCode = d.Client.ClientCode,
                    CompanyName = d.Client.Company.Name,
                    OrderId = d.ID,
                    OrderStatus = d.OrderStatus.GetDescription(),
                    Salesman = d.Client.ServiceManager.RealName,
                    DeclareDate = d.DDate?.ToShortDateString(),
                    DeclarePrice = (d.DeclarePrice * d.RealExchangeRate).ToRound(2).ToString("0.00"),
                    Receivable = d.Receivable,
                    Received = d.Received,
                    Overdraft = d.Overdraft,
                    Merchandiser = d.Client.Merchandiser.RealName,
                    //    结算方式 = d.Client.Agreement.AgencyFeeClause.PeriodType.GetDescription(),
                    //    结算日 = d.Client.Agreement.AgencyFeeClause.PeriodType == Needs.Ccs.Services.Enums.PeriodType.PrePaid ? "-"
                    //: (d.Client.Agreement.AgencyFeeClause.PeriodType == Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod ? d.Client.Agreement.AgencyFeeClause.DaysLimit.ToString() : d.Client.Agreement.AgencyFeeClause.MonthlyDay.ToString())
                    PeriodType = d.PeriodType,
                    PeriodTypeDesc = ((Needs.Ccs.Services.Enums.PeriodType)int.Parse(d.PeriodType)).GetDescription(),
                    SettlementDate = int.Parse(d.PeriodType) == Needs.Ccs.Services.Enums.PeriodType.PrePaid.GetHashCode() ? "-"
                : (int.Parse(d.PeriodType) == Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod.GetHashCode() ? d.DaysLimit.ToString() : d.MonthlyDay.ToString())
                });

                foreach (var item in detailData)
                {
                    item.ExceedDays = CountExceed(item.DeclareDate, item.PeriodType, item.SettlementDate);
                }

                var detailDataResult = detailData.Select(d => new {
                    客户编号 = d.ClientCode,
                    客户名称 = d.CompanyName,
                    订单编号 = d.OrderId,
                    订单状态 = d.OrderStatus,
                    业务员 = d.Salesman,
                    报关日期 = d.DeclareDate,
                    报关货值 = d.DeclarePrice,
                    应收款 = d.Receivable,
                    已收款 = d.Received,
                    欠款 = d.Overdraft,
                    客服 = d.Merchandiser,
                    结算方式 =d.PeriodTypeDesc,
                    结算日 = d.SettlementDate,
                    逾期天数 = d.ExceedDays
                });

                #endregion

                //文件
                string filename = "欠款统计表" + DateTime.Now.Ticks + ".xlsx";
                FileDirectory fileDic5 = new FileDirectory(filename);
                fileDic5.SetChildFolder(Needs.Ccs.Services.SysConfig.Export);
                fileDic5.CreateDataDirectory();

                var templatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Needs.Ccs.Services.SysConfig.ExportUnReceipts);
                using (FileStream file = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                {
                    XSSFWorkbook workbook = new XSSFWorkbook(file);
                    NPOIHelper npoi = new NPOIHelper(workbook);

                    npoi.SetSheet("欠款统计表");
                    npoi.GenerateExcelByTemplate(detailDataResult, 1);
                    npoi.SaveAs(fileDic5.FilePath);
                }

                Response.Write((new { success = true, message = "导出成功", url = fileDic5.FileUrl }).Json());

            }
            catch (Exception ex) {

            }
        }


        protected void OwnMoney()
        {
            string ClientCode = Request.Form["ClientCode"];
            string OrderId = Request.Form["OrderId"];
            var Salesman = Request.Form["Salesman"];
            var StartDate = Request.Form["StartDate"];
            var EndDate = Request.Form["EndDate"];

            //页面上所有的欠款
            var orderReceipts = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderUnReceiveStats.AsQueryable();
            if (!string.IsNullOrEmpty(ClientCode))
            {
                orderReceipts = orderReceipts.Where(o => o.Client.ClientCode == ClientCode.Trim());
            }
            if (!string.IsNullOrEmpty(Salesman))
            {
                orderReceipts = orderReceipts.Where(o => o.Client.ServiceManager.ID == Salesman.Trim());
            }
            if (!string.IsNullOrEmpty(OrderId))
            {
                orderReceipts = orderReceipts.Where(o => o.ID == OrderId.Trim());
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                var from = DateTime.Parse(StartDate);
                orderReceipts = orderReceipts.Where(t => t.DDate >= from);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                var to = DateTime.Parse(EndDate);
                orderReceipts = orderReceipts.Where(item => item.DDate < to.AddDays(1));
            }

            Response.Write(new
            {
                success = true,
                Amount = orderReceipts.Sum(t => (decimal?)t.Overdraft).GetValueOrDefault()
            }.Json());
        }

        /// <summary>
        /// 计算逾期天数
        /// </summary>
        /// <returns></returns>
        public int CountExceed(string DDate, string type, string Day)
        {
            //算出应收款日期
            var periodType = (Needs.Ccs.Services.Enums.PeriodType)int.Parse(type);
            var ddate = DateTime.Parse(DDate);

            DateTime shouldDate;
            switch (periodType)
            {
                case Needs.Ccs.Services.Enums.PeriodType.PrePaid:
                    shouldDate = ddate;
                    break;
                case Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod:
                    shouldDate = ddate.AddDays(int.Parse(Day));
                    break;
                case Needs.Ccs.Services.Enums.PeriodType.Monthly:

                    shouldDate = new DateTime(ddate.Year, ddate.AddMonths(1).Month, int.Parse(Day));
                    break;
                default:
                    shouldDate = ddate;
                    break;
            }

            return DateTime.Now.Subtract(shouldDate).Days;
        }

    }
}