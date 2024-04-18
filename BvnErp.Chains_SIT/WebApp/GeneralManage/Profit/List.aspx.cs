using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Utils.Descriptions;
using WebApp.Ccs.Utils;
using System.Linq.Expressions;

namespace WebApp.GeneralManage.Profit
{
    /// <summary>
    /// 利润提成列表界面
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadDate();
        }

        /// <summary>
        /// 初始化数据加载
        /// </summary>
        protected void LoadDate()
        {
            this.Model.ServiceManager = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles.Where(manager => manager.Role.Name == "业务员" && manager.Admin.RealName != "张庆永").
                Select(item => new { Key = item.Admin.ID, Value = item.Admin.RealName }).ToArray().Json();
            this.Model.DepartmentType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.DepartmentType>()
                                            .Where(t => t.Value.Contains("业务"))
                                            .Select(item => new { item.Key, item.Value }).Json();
            this.Model.Department = new Needs.Ccs.Services.Views.DepartmentView().Where(t => t.FatherID == "A85A9D761ADB49D8B24E29EFBCBEBCD9").Select(t => new { Key = t.ID, Value = t.Name }).Json();
        }

        protected void data()
        {
            string SaleManID = Request.QueryString["SaleManID"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string DepartmentType = Request.QueryString["DepartmentType"];

            ////订单利润提成明细
            //var profitsDetails = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.ProfitsDetails.AsQueryable();
            //if (!string.IsNullOrEmpty(SaleManID))
            //{
            //    profitsDetails = profitsDetails.Where(pd => pd.Client.ServiceManager.ID == SaleManID);
            //}
            //if (!string.IsNullOrEmpty(StartDate))
            //{
            //    var from = DateTime.Parse(StartDate);
            //    profitsDetails = profitsDetails.Where(t => t.DDate >= from);
            //}
            //if (!string.IsNullOrEmpty(EndDate))
            //{
            //    var to = DateTime.Parse(EndDate);
            //    profitsDetails = profitsDetails.Where(item => item.DDate < to.AddDays(1));
            //}

            var ProfitsExport = new Needs.Ccs.Services.Views.ProfitsExportDetailsView();
            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.ProfitDetail, bool>> expression = item => true;

            #region 页面查询条件

            if (!string.IsNullOrEmpty(SaleManID))
            {
                Expression<Func<Needs.Ccs.Services.Models.ProfitDetail, bool>> lambda1 = item => item.Client.ServiceManager.ID == SaleManID;
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


            var lists = ProfitsExport.GetAlls(expression, lamdas.ToArray());

            //业务员的利润提成统计
            var saleMans = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles.Where(manager => manager.Role.Name == "业务员" && manager.Admin.RealName != "张庆永")
                           .Select(manager => new { manager.Admin.ID, manager.Admin.RealName, manager.Admin.DepartmentID }).Distinct().ToList();
            //var salesProfits = from saleMan in saleMans
            //                   join detail in lists on saleMan.ID equals detail.Client.ServiceManager.ID into saleProfits
            //                   where string.IsNullOrEmpty(SaleManID) || saleMan.ID == SaleManID
            //                   orderby saleMan.ID
            //                   select new Needs.Ccs.Services.Models.Profit
            //                   {
            //                       ID = saleMan.ID,
            //                       Name = saleMan.RealName,
            //                       ProfitDetails = saleProfits
            //                   };

            //Func<Needs.Ccs.Services.Models.Profit, object> convert = t => new
            //{
            //    t.ID,//业务员ID
            //    t.Name,
            //    Profits = t.ProfitDetails.Sum(x => x.OrderProfit).ToRound(2),
            //    BusinessCommission = t.ProfitDetails.Sum(c => c.Commission).ToRound(2),

            //};

            var saleManOriginIDs = saleMans.Select(t => t.ID).ToArray();

            //查这些业务员的部门
            //var XDTStaffs = (from staff in new Needs.Ccs.Services.Views.XDTStaffsTopView()
            //                 where saleManOriginIDs.Contains(staff.OriginID)
            //                 select new
            //                 {
            //                     OriginID = staff.OriginID,
            //                     DepartmentCode = staff.DepartmentCode,
            //                 }).ToArray();

            var salesProfitsQuery = from saleMan in saleMans
                                    join detail in lists on saleMan.ID equals detail.Client.ServiceManager.ID into saleProfits

                                    join depart in new Needs.Ccs.Services.Views.DepartmentView() on saleMan.DepartmentID equals depart.ID into depart_temp
                                    from depart in depart_temp.DefaultIfEmpty()

                                        //join staff in XDTStaffs on saleMan.ID equals staff.OriginID into XDTStaffs2
                                        //from staff in XDTStaffs2.DefaultIfEmpty()

                                    where string.IsNullOrEmpty(SaleManID) || saleMan.ID == SaleManID
                                    orderby saleMan.ID
                                    select new Needs.Ccs.Services.Models.Profit
                                    {
                                        ID = saleMan.ID,
                                        Name = saleMan.RealName,
                                        ProfitDetails = saleProfits,
                                        TotalOrderProfit = saleProfits.Sum(x => x.TotalProfit).ToRound(2),
                                        TotalSalesCommission = saleProfits.Sum(x => x.SalesCommission).ToRound(2),
                                        DepartmentCode = depart?.Name
                                        //DepartmentCode = staff?.DepartmentCode,
                                    };

            if (!string.IsNullOrEmpty(DepartmentType))
            {
                salesProfitsQuery = salesProfitsQuery.Where(t => t.DepartmentCode == DepartmentType);
            }

            var salesProfits = salesProfitsQuery.ToList();

            //引荐人的利润提成统计
            var referrers = new Needs.Ccs.Services.Views.Origins.ReferrersOrigin()
                           .Select(manager => new { manager.ID, manager.Name }).Distinct().ToList();

            var referrersProfits = (from referrer in referrers
                                    join detail in lists on referrer.Name equals detail.Client.Referrer into referrerProfits
                                    orderby referrer.ID
                                    select new Needs.Ccs.Services.Models.Profit
                                    {
                                        ID = referrer.ID,
                                        Name = referrer.Name,
                                        ReferrerCommission = referrerProfits.Sum(o => o.ReferrerCommission)
                                    }).ToList();

            for (int i = 0; i < salesProfits.Count; i++)
            {
                var referrersProfit = referrersProfits.Where(o => o.Name == salesProfits[i].Name).FirstOrDefault();
                if (referrersProfit != null)
                {
                    salesProfits[i].TotalSalesCommission = salesProfits[i].TotalSalesCommission + referrersProfit.ReferrerCommission;
                }
            }
            Func<Needs.Ccs.Services.Models.Profit, object> convert = t => new
            {
                t.ID,//业务员ID
                t.Name,
                Profits = t.TotalOrderProfit.ToRound(2),
                BusinessCommission = t.TotalSalesCommission.ToRound(2),
                DepartmentCode = t.DepartmentCode,
                //DepartmentName = !string.IsNullOrEmpty(t.DepartmentCode) ?
                //                    ((Needs.Ccs.Services.Enums.DepartmentType)int.Parse(t.DepartmentCode)).GetDescription() : "",
            };

            Response.Write(new { rows = salesProfits.Select(convert).ToArray() }.Json());
        }

        /// <summary>
        /// 导出
        /// </summary>
        protected void Export()
        {
            string SaleManID = Request.Form["SaleManID"];
            string StartDate = Request.Form["StartDate"];
            string EndDate = Request.Form["EndDate"];
            string DepartmentType = Request.Form["DepartmentType"];

            try
            {

                var ProfitsExport = new Needs.Ccs.Services.Views.ProfitsExportDetailsView();
                List<LambdaExpression> lamdas = new List<LambdaExpression>();
                Expression<Func<Needs.Ccs.Services.Models.ProfitDetail, bool>> expression = item => true;

                #region 页面查询条件

                if (!string.IsNullOrEmpty(SaleManID))
                {
                    Expression<Func<Needs.Ccs.Services.Models.ProfitDetail, bool>> lambda1 = item => item.Client.ServiceManager.ID == SaleManID;
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
                if (!string.IsNullOrEmpty(DepartmentType))
                {
                    Expression<Func<Needs.Ccs.Services.Models.ProfitDetail, bool>> lambda1 = item => item.DepartmentCode == DepartmentType;
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
                    //DepartmentName = !string.IsNullOrEmpty(t.DepartmentCode) ?
                    //                    ((Needs.Ccs.Services.Enums.DepartmentType)int.Parse(t.DepartmentCode)).GetDescription() : "",
                    DepartmentName = t.DepartmentCode,
                    FeeClauseDate = t.FeeClauseDate.ToString("yyyy-MM-dd"),
                    //FeeClauseTypeName = t.FeeClauseType.GetDescription(),

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
                    InvoiceDate = t.ProfitInvoiceInfo == null ? "-" : t.ProfitInvoiceInfo.InvoiceDate.ToString("yyyy-MM-dd HH:mm:ss"),
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



            //try {

            //    var profitsDetails = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.ProfitsDetails.AsQueryable();

            //    if (!string.IsNullOrEmpty(SaleManID))
            //    {
            //        profitsDetails = profitsDetails.Where(pd => pd.Client.ServiceManager.ID == SaleManID);
            //    }
            //    if (!string.IsNullOrEmpty(StartDate))
            //    {
            //        var from = DateTime.Parse(StartDate);
            //        profitsDetails = profitsDetails.Where(t => t.DDate >= from);
            //    }
            //    if (!string.IsNullOrEmpty(EndDate))
            //    {
            //        var to = DateTime.Parse(EndDate);
            //        profitsDetails = profitsDetails.Where(item => item.DDate < to.AddDays(1));
            //    }

            //    Func<Needs.Ccs.Services.Models.ProfitDetail, object> convert = t => new
            //    {
            //        t.DDate,//报关日期
            //        ClientName = t.Client.Company.Name,
            //        OrderID = t.ID,
            //        OrderProfits = t.OrderProfit.ToString("0.0000"),//订单利润
            //        HKReceived = t.HKFeeReceived.ToString("0.0000"), //香港收款纯利润
            //        TotalProfit = t.TotalProfit.ToString("0.0000"), //总利润（报关利润加香港现金利润）

            //        DeclarePrice = t.DeclarePrice.ToString("0.00"),
            //        t.Currency,
            //        RMBDeclarePrice = t.RMBDeclarePrice.ToString("0.00"),
            //        t.RealExchangeRate,
            //        t.CustomsExchangeRate,
            //        #region 税代合计
            //        AgencyRate = t.AgencyRate,
            //        AgencyReceived = t.AgencyReceivedUnTax.ToString("0.00"),
            //        IncidentalReceived = t.IncidentalReceivedUnTax.ToString("0.00"),
            //        AVTReceived = t.AVTReceived.ToString("0.00"),
            //        TariffReceived = t.TariffReceived.ToString("0.00"),
            //        TaxGeneratTotal = t.TaxGeneratTotal.ToString("0.00"),
            //        ReceiveDate = t.ReceiveDate.ToShortDateString(),
            //        #endregion
            //        #region 费用
            //        AVTReceivable = t.AVTReceivable.ToString("0.00"),
            //        TariffReceivable = t.TariffReceivable.ToString("0.00"),//应收关税
            //        IncidentalPaid = t.IncidentalPaid.ToString("0.00"),//实付杂费
            //        FeeTotal = t.FeeTotal.ToString("0.00"),
            //        #endregion
            //        #region 提成核算

            //        UserName = t.Client.ServiceManager.RealName,//业务员
            //        RegisterTime = t.Client.CreateDate.ToString("yyyy-MM-dd"),//注册时间
            //        Proportion = t.proportion,//比例
            //        BusinessCommission = (t.TaxGeneratTotal - t.FeeTotal) * t.proportion,// 业务提成
            //        #endregion
            //        #region  开票
            //        //InvoiceType = t.InvoiceType?.GetDescription(),
            //        //t.InvoiceTaxRate,
            //        //InvoiceDate = t.InvoiceDate?.ToString("yyyy-MM-dd HH:mm:ss")
            //        #endregion

            //    };
            //    //外币金额汇总
            //    Dictionary<string, decimal> SumCurrency = profitsDetails.GroupBy(x => x.Currency)
            //                                            .Select(x => new { Currency = x.Key, DeclarePrice = x.Sum(i => i.DeclarePrice) })
            //                                            .ToDictionary(x => x.Currency, x => x.DeclarePrice);
            //    //写入数据
            //    DataTable dt = NPOIHelper.JsonToDataTable(profitsDetails.Select(convert).ToArray().Json());
            //    ////创建文件夹
            //    string fileName = DateTime.Now.Ticks.ToString() + ".xls";//以时间戳进行导出文件的命名
            //    FileDirectory file = new FileDirectory(fileName);
            //    file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
            //    file.CreateDataDirectory();
            //    NPOIHelper.ProfitsExcel(dt, file.FilePath, SumCurrency);
            //    Response.Write((new
            //    {
            //        success = true,
            //        message = "导出成功",
            //        url = file.FileUrl
            //    }).Json());

            //}
            //catch (Exception ex) {

            //}
        }
    }
}
