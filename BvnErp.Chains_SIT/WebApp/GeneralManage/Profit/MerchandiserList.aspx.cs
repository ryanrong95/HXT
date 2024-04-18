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
    public partial class MerchandiserList : Uc.PageBase
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
            this.Model.Merchandiser = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles.Where(manager => manager.Role.Name == "跟单员").
                Select(item => new { Key = item.Admin.ID, Value = item.Admin.RealName }).ToArray().Json();
        }

        protected void data()
        {
            string MerchandiserId = Request.QueryString["MerchandiserId"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            var ProfitsExport = new Needs.Ccs.Services.Views.ProfitsExportDetailsView();
            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.ProfitDetail, bool>> expression = item => true;

            #region 页面查询条件

            if (!string.IsNullOrEmpty(MerchandiserId))
            {
                Expression<Func<Needs.Ccs.Services.Models.ProfitDetail, bool>> lambda1 = item => item.Client.Merchandiser.ID == MerchandiserId;
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

            //当月海关汇率 = list<订单>.where(currency = USD).first().CustomsExchangeRate
            var list = lists.Where(item => item.Currency == "USD").ToList();
            decimal NewCustomsExchangeRate = 0;
            if (list.Count != 0)
            {
                NewCustomsExchangeRate = list.First().CustomsExchangeRate;
            }
            //跟单员提成统计
            var merchandiser = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles.Where(manager => manager.Role.Name == "跟单员").Select(manager => new { manager.Admin.ID, manager.Admin.RealName }).Distinct().ToList();

            var salesProfits = (from merchandiserId in merchandiser
                                join detail in lists on merchandiserId.ID equals detail.Client.Merchandiser.ID into saleProfits
                                where string.IsNullOrEmpty(MerchandiserId) || merchandiserId.ID == MerchandiserId
                                orderby merchandiserId.ID
                                select new Needs.Ccs.Services.Models.Profit
                                {
                                    ID = merchandiserId.ID,
                                    Name = merchandiserId.RealName,
                                    ProfitDetails = saleProfits,
                                    TotalOrderProfit = saleProfits.Sum(x => x.RMBDeclarePrice).ToRound(2),
                                    TotalSalesCommission = saleProfits.Sum(x => (x.DeclarePrice * x.RealExchangeRate - 500000 * NewCustomsExchangeRate) * Convert.ToDecimal(0.0001)).ToRound(2)
                                }).ToList();
            Func<Needs.Ccs.Services.Models.Profit, object> convert = t => new
            {
                t.ID,
                t.Name,
                Profits = t.TotalOrderProfit.ToRound(2),
                BusinessCommission = t.TotalSalesCommission.ToRound(2)//跟单提成
            };

            Response.Write(new { rows = salesProfits.Select(convert).ToArray() }.Json());
        }

        /// <summary>
        /// 导出
        /// </summary>
        protected void Export()
        {
            string MerchandiserId = Request.Form["MerchandiserId"];
            string StartDate = Request.Form["StartDate"];
            string EndDate = Request.Form["EndDate"];


            try
            {

                var ProfitsExport = new Needs.Ccs.Services.Views.ProfitsExportDetailsView();
                List<LambdaExpression> lamdas = new List<LambdaExpression>();
                Expression<Func<Needs.Ccs.Services.Models.ProfitDetail, bool>> expression = item => true;

                #region 页面查询条件

                if (!string.IsNullOrEmpty(MerchandiserId))
                {
                    Expression<Func<Needs.Ccs.Services.Models.ProfitDetail, bool>> lambda1 = item => item.Client.Merchandiser.ID == MerchandiserId;
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

                //当月海关汇率 = list<订单>.where(currency = USD).first().CustomsExchangeRate
                var list = lists.Where(item => item.Currency == "USD").ToList();
                decimal NewCustomsExchangeRate = 0;
                if (list.Count != 0)
                {
                    NewCustomsExchangeRate = list.First().CustomsExchangeRate;
                }

                Func<Needs.Ccs.Services.Models.ProfitDetail, object> convert = t => new
                {
                    t.DDate,//报关日期
                    ClientName = t.Client.Company.Name,
                    OrderID = t.ID,
                    //  OrderProfits = t.OrderProfit.ToString("0.0000"),//订单利润
                    HKReceived = t.HKFeeReceived.ToString("0.0000"), //香港收款纯利润
                                                                     // TotalProfit = t.TotalProfit.ToString("0.0000"), //总利润（报关利润加香港现金利润）

                    DeclarePrice = t.DeclarePrice.ToString("0.00"),
                    t.Currency,
                    RMBDeclarePrice = t.RMBDeclarePrice.ToString("0.00"),
                    t.RealExchangeRate,
                    t.CustomsExchangeRate,

                    #region 提成核算

                    UserName = t.Client.ServiceManager.RealName,//业务员
                    RegisterTime = t.Client.CreateDate.ToString("yyyy-MM-dd"),//注册时间

                    Merchandiser = t.Client.Merchandiser.RealName,//跟单员
                    MerchandiserCommission = (t.DeclarePrice * t.RealExchangeRate - 500000 * NewCustomsExchangeRate) * Convert.ToDecimal(0.0001)//t.MerchandiserCommission.ToString("0.00"),//提成
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
                NPOIHelper.MerchandiserProfitsExcel(dt, file.FilePath, SumCurrency);
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