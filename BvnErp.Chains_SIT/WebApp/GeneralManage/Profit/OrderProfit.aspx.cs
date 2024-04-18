using Needs.Ccs.Services;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.GeneralManage.Profit
{
    public partial class OrderProfit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        protected void LoadData()
        {
            var adminid = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
            var Isleader = new Needs.Ccs.Services.Views.DepartmentView().Any(item => item.LeaderID == adminid);
            IQueryable<dynamic> clients = null;
            if (Isleader)
            {
                clients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.Where(x => x.Department.LeaderID == adminid).Select(x => new { Key = x.ID, Value = x.Company.Name });
            }
            else
            {
                clients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(x => x.ServiceManager.ID == adminid).Select(x => new { Key = x.ID, Value = x.Company.Name });
            }
            this.Model.Clients = clients.Json();

        }

        protected void data()
        {
            string ClientID = Request.QueryString["ClientID"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            var adminid = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
            var Isleader = new Needs.Ccs.Services.Views.DepartmentView().Any(item => item.LeaderID == adminid);

            var Profits = new Needs.Ccs.Services.Views.ProfitsExportDetailsView();
            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.ProfitDetail, bool>> expression = item => true;

            #region 页面查询条件

            if (Isleader)
            {
                Expression<Func<Needs.Ccs.Services.Models.ProfitDetail, bool>> lambda1 = item => item.Client.Department.LeaderID == adminid;
                lamdas.Add(lambda1);
            }
            else
            {

                Expression<Func<Needs.Ccs.Services.Models.ProfitDetail, bool>> lambda1 = item => item.Client.ServiceManager.ID == adminid;
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(ClientID))
            {
                Expression<Func<Needs.Ccs.Services.Models.ProfitDetail, bool>> lambda1 = item => item.Client.ID == ClientID;
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                Expression<Func<Needs.Ccs.Services.Models.ProfitDetail, bool>> lambda1 = item => item.DDate.CompareTo(StartDate) >= 0;
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                var endTime = DateTime.Parse(EndDate).AddDays(1);
                Expression<Func<Needs.Ccs.Services.Models.ProfitDetail, bool>> lambda1 = item => item.DDate.CompareTo(endTime) < 0;
                lamdas.Add(lambda1);
            }

            #endregion
            var lists = Profits.GetAlls(expression, lamdas.ToArray());
            Func<Needs.Ccs.Services.Models.ProfitDetail, object> convert = t => new
            {
                ClientID = t.Client.Company.ID,
                ClientName = t.Client.Company.Name,
                RMBDeclarePrice = t.RMBDeclarePrice.ToString("0.00"),
                OrderDate = t.OrderDate.ToShortDateString(),
                DDate = t.DDate.ToShortDateString(),
                TaxGeneratTotal = t.TaxGeneratTotal.ToString("0.00"),
                FeeTotal = t.FeeTotal.ToString("0.00"),
                HKFeeReceived = t.HKFeeReceived.ToString("0.00"),
                OrderProfit = t.OrderProfit.ToString("0.00"),
                t.proportion,
                Commission = t.Commission.ToString("0.00"),
                t.Client.Referrer,
                ReceiveDate = t.ReceiveDate.ToShortDateString(),
                OrderID = t.ID,

            };
            this.Paging(lists, convert);
        }


        /// <summary>
        /// 导出
        /// </summary>
        protected void Export()
        {
            string ClientID = Request.Form["ClientID"];
            string StartDate = Request.Form["StartDate"];
            string EndDate = Request.Form["EndDate"];

            try
            {

                var ProfitsExport = new Needs.Ccs.Services.Views.ProfitsExportDetailsView();
                List<LambdaExpression> lamdas = new List<LambdaExpression>();
                Expression<Func<Needs.Ccs.Services.Models.ProfitDetail, bool>> expression = item => true;
                var adminid = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                var Isleader = new Needs.Ccs.Services.Views.DepartmentView().Any(item => item.LeaderID == adminid);
                #region 页面查询条件

                if (Isleader)
                {
                    Expression<Func<Needs.Ccs.Services.Models.ProfitDetail, bool>> lambda1 = item => item.Client.Department.LeaderID == adminid;
                    lamdas.Add(lambda1);
                }
                else
                {

                    Expression<Func<Needs.Ccs.Services.Models.ProfitDetail, bool>> lambda1 = item => item.Client.ServiceManager.ID == adminid;
                    lamdas.Add(lambda1);
                }
                if (!string.IsNullOrEmpty(ClientID))
                {
                    Expression<Func<Needs.Ccs.Services.Models.ProfitDetail, bool>> lambda1 = item => item.Client.ID == ClientID;
                    lamdas.Add(lambda1);
                }
                if (!string.IsNullOrEmpty(StartDate))
                {
                    Expression<Func<Needs.Ccs.Services.Models.ProfitDetail, bool>> lambda1 = item => item.DDate.CompareTo(StartDate) >= 0;
                    lamdas.Add(lambda1);
                }
                if (!string.IsNullOrEmpty(EndDate))
                {
                    var endTime = DateTime.Parse(EndDate).AddDays(1);
                    Expression<Func<Needs.Ccs.Services.Models.ProfitDetail, bool>> lambda1 = item => item.DDate.CompareTo(endTime) < 0;
                    lamdas.Add(lambda1);
                }
                #endregion

                #region 页面需要数据

                var lists = ProfitsExport.GetAlls(expression, lamdas.ToArray());

                var s = lists.ToList();

                Func<Needs.Ccs.Services.Models.ProfitDetail, object> convert = t => new
                {
                    DDate = t.DDate.ToString("yyyy-MM-dd"),//报关日期
                    ClientName = t.Client.Company.Name,
                    OrderID = t.ID,

                    InvoiceTypeDesc = t.Client.Agreement.InvoiceType.GetDescription(),//开票类型
                    //千一税赋补充
                    ThousandthTax = t.ThousandthTax.ToString("0.0000"),//单抬头千一税赋
                    ReductionThousandthTax = t.ReductionThousandthTax.ToString("0.0000"),//正常换汇减免
                    OrderProfits = t.OrderProfit.ToString("0.0000"),//订单利润

                    HKReceived = t.HKFeeReceived.ToString("0.0000"), //香港收款纯利润
                    TotalProfit = t.TotalProfit.ToString("0.0000"), //总利润（报关利润加香港现金利润）

                    DeclarePrice = t.DeclarePrice.ToString("0.00"),
                    t.Currency,
                    RMBDeclarePrice = t.RMBDeclarePrice.ToString("0.00"),
                    t.RealExchangeRate,
                    t.CustomsExchangeRate,

                    UnPayExchangeAmount = t.UnPayExchangeAmount.ToString("0.00"),
                    UnPayExchangeAmount4M = t.UnPayExchangeAmount4M.ToString("0.00"),

                    t.DepartmentCode,
                    DepartmentName = t.DepartmentCode,
                    FeeClauseDate = t.FeeClauseDate.ToString("yyyy-MM-dd"),
                    #region 税代合计
                    AgencyRate = t.AgencyRate,
                    AgencyReceived = t.AgencyReceivedUnTax.ToString("0.00"),
                    IncidentalReceived = t.IncidentalReceivedUnTax.ToString("0.00"),
                    AVTReceived = t.AVTReceived.ToString("0.00"),
                    TariffReceived = t.TariffReceived.ToString("0.00"),
                    TaxGeneratTotal = t.TaxGeneratTotal.ToString("0.00"),
                    ReceiveDate = t.ReceiveDate.ToShortDateString(),
                    #endregion
                    #region 费用
                    AVTReceivable = t.AVTReceivable.ToString("0.00"),
                    TariffReceivable = t.TariffReceivable.ToString("0.00"),//应收关税
                    IncidentalPaid = t.IncidentalPaid.ToString("0.00"),//实付杂费
                    FeeTotal = t.FeeTotal.ToString("0.00"),
                    #endregion
                    #region 提成核算

                    UserName = t.Client.ServiceManager.RealName,//业务员
                    RegisterTime = t.Client.CreateDate.ToString("yyyy-MM-dd"),//注册时间
                    Proportion = t.proportion,//比例
                                              //业务员提成：有引荐人，则只能提一部分；没有引荐人，能提该得的部分
                    BusinessCommission = string.IsNullOrEmpty(t.Client.Referrer) ?
                    (t.TotalProfit * t.proportion).ToRound(2).ToString("0.00")
                    : (t.TotalProfit * t.proportion * ConstConfig.ProfitDiscount).ToRound(2).ToString("0.00"),// 业务提成  

                    Referrer = string.IsNullOrEmpty(t.Client.Referrer) ? null : t.Client.Referrer,//引荐人
                    ReferrerCommission = string.IsNullOrEmpty(t.Client.Referrer) ? null : ((t.TotalProfit * t.proportion * (1 - ConstConfig.ProfitDiscount)).ToRound(2)).ToString(),// 引荐人提成

                    Merchandiser = t.Client.Merchandiser.RealName,//跟单员
                    #endregion
                    #region  开票
                    InvoiceType = t.ProfitInvoiceInfo == null ? "-" : t.ProfitInvoiceInfo.InvoiceType.GetDescription(),
                    InvoiceTaxRate = t.ProfitInvoiceInfo == null ? "-" : t.ProfitInvoiceInfo.InvoiceTaxRate.ToString(),
                    InvoiceDate = t.ProfitInvoiceInfo == null ? "-" : t.ProfitInvoiceInfo.InvoiceDate.ToString("yyyy-MM-dd HH:mm:ss")
                    #endregion
                };

                #endregion

                //外币金额汇总
                Dictionary<string, decimal> SumCurrency = lists.GroupBy(x => x.Currency)
                                                        .Select(x => new { Currency = x.Key, DeclarePrice = x.Sum(i => i.DeclarePrice) })
                                                        .ToDictionary(x => x.Currency, x => x.DeclarePrice);
                //写入数据
                DataTable dt = NPOIHelper.JsonToDataTable(lists.Select(convert).ToArray().Json());
                ////创建文件夹
                string fileName = DateTime.Now.Ticks.ToString() + ".xls";//以时间戳进行导出文件的命名
                FileDirectory file = new FileDirectory(fileName);
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                file.CreateDataDirectory();
                NPOIHelper.ProfitsExcel(dt, file.FilePath, SumCurrency);
                Response.Write((new
                {
                    success = true,
                    message = "导出成功",
                    url = file.FileUrl
                }).Json());

            }
            catch (Exception ex)
            {

            }
        }

    }
}
