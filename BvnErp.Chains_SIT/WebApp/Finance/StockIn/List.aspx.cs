using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace WebApp.Finance.StockIn
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.loaddata();
            }
        }


        protected void loaddata()
        {

            //this.Model.DecTaxStatus = Needs.Ccs.Services.MultiEnumUtils.ToDictionary<Needs.Ccs.Services.Enums.DecTaxStatus>()
            //   .Select(item => new { Value = item.Key, Text = item.Value }).Json();


            //Dictionary<string, string> dic = new Dictionary<string, string>() { { "-100", "全部" } };
            //this.Model.DecTaxStatus = dic.Concat(ExtendsEnum.ToDictionary<Needs.Ccs.Services.Enums.DecTaxStatus>()).Select(item => new
            //{
            //    Value = int.Parse(item.Key),
            //    Text = item.Value.ToString()


            //});


            this.Model.DecTaxStatusData = EnumUtils.ToDictionary<DecTaxStatus>().Where(item => item.Key != "1")
               .Select(item => new { item.Key, item.Value }).Json();

        }

        protected void data()
        {
            string name = Request.QueryString["ClientName"];
            string orderID = Request.QueryString["OrderID"];
            string contrNo = Request.QueryString["ContrNo"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];
            string entryId = Request.QueryString["EntryId"];
            string decTaxStatus = Request.QueryString["DecTaxStatus"];
            var list = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceStockIn.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                list = list.Where(t => t.ClientName.Contains(name.Trim()));
            }
            if (!string.IsNullOrEmpty(orderID))
            {
                list = list.Where(t => t.OrderID.Contains(orderID.Trim()));
            }
            if (!string.IsNullOrEmpty(contrNo))
            {
                list = list.Where(t => t.ContrNo.Contains(contrNo.Trim()));
            }
            if (!string.IsNullOrEmpty(entryId))
            {
                list = list.Where(t => t.EntryId.Contains(entryId.Trim()));
            }

            DecTaxStatus status;
            if (Enum.TryParse(decTaxStatus, out status))
            {

                list = list.Where(t => t.DecTaxStatus == status);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                DateTime start = Convert.ToDateTime(startDate);
                list = list.Where(item => item.DDate >= start);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                DateTime end = Convert.ToDateTime(endDate).AddDays(1);
                list = list.Where(item => item.DDate < end);

            }
            Func<Needs.Ccs.Services.Models.FinanceStockInModel, object> convert = item => new
            {
                ID = item.DecHeadID,
                ClientName = item.ClientName,
                item.OrderID,
                ContrNo = item.ContrNo,
                item.EntryId,
                DDate = item.DDate?.ToString("yyyy-MM-dd"),
                DecTaxStatus = item.DecTaxStatus.GetDescription(),
            };
            this.Paging(list, convert);
        }
        /// <summary>
        /// 批量勾选推送
        /// </summary>
        protected void BatchCheckPush()
        {

            try
            {
                string[] ids = Request.Form["IDs"].Split(',');
                var items = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceStockIn.Where(item => ids.Contains(item.DecHeadID)).ToArray();

                foreach (var item in items)
                {
                    item.BatchPush();
                }
                Response.Write((new { success = true, message = "批量推送成功" }).Json());
            }
            catch (Exception ex)
            {

                Response.Write((new { success = false, message = "批量推送失败：" + ex.Message }).Json());
            }
        }


        /// <summary>
        /// 批量查询推送
        /// </summary>
        protected void BatchQueryPush()
        {

            try
            {

                string name = Request.QueryString["ClientName"];
                string orderID = Request.QueryString["OrderID"];
                string contrNo = Request.QueryString["ContrNo"];
                string startDate = Request.QueryString["StartDate"];
                string endDate = Request.QueryString["EndDate"];
                string entryId = Request.QueryString["EntryId"];
                string decTaxStatus = Request.QueryString["DecTaxStatus"];
                var list = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceStockIn.AsQueryable();

                if (!string.IsNullOrEmpty(name))
                {
                    list = list.Where(t => t.ClientName.Contains(name.Trim()));
                }
                if (!string.IsNullOrEmpty(orderID))
                {
                    list = list.Where(t => t.OrderID.Contains(orderID.Trim()));
                }
                if (!string.IsNullOrEmpty(contrNo))
                {
                    list = list.Where(t => t.ContrNo.Contains(contrNo.Trim()));
                }
                if (!string.IsNullOrEmpty(entryId))
                {
                    list = list.Where(t => t.EntryId.Contains(entryId.Trim()));
                }

                DecTaxStatus status;
                if (Enum.TryParse(decTaxStatus, out status))
                {

                    list = list.Where(t => t.DecTaxStatus == status);
                }
                if (!string.IsNullOrEmpty(startDate))
                {
                    DateTime start = Convert.ToDateTime(startDate);
                    list = list.Where(item => item.DDate >= start);
                }
                if (!string.IsNullOrEmpty(endDate))
                {
                    DateTime end = Convert.ToDateTime(endDate).AddDays(1);
                    list = list.Where(item => item.DDate < end);

                }

                var arrList = list.ToArray();

                if (arrList.Count() > 0)
                {
                    foreach (var item in arrList)
                    {

                        item.BatchPush();
                    }

                }
                Response.Write((new { success = true, message = "批量推送成功" }).Json());


            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "批量推送失败：" + ex.Message }).Json());
            }

        }

        /// <summary>
        /// 统计报关金额
        /// </summary>
        protected void CheckAmount()
        {
            try
            {
                string[] ids = Request.Form["IDs"].Split(',');
                var items = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceStockInStatistic.Where(item => ids.Contains(item.ID)).ToArray();
                if (items.Count() < 1)
                {
                    Response.Write((new { success = false, message = "未查询到报关单" }).Json());
                }

                var totalContent = "";
                foreach (var curr in items.Select(t => t.Currency).Distinct())
                {
                    totalContent += items.Where(t => t.Currency == curr).Sum(t => t.DecTotalAmount) + "  " + curr + ";";
                }
                Response.Write((new { success = true, count = items.Count(), TotalAmount = totalContent }).Json());
            }
            catch (Exception ex)
            {

                Response.Write((new { success = false, message = ex.Message }).Json());
            }

        }


    }

}
