using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;

namespace WebApp.GeneralManage.Receipt.ServiceManager
{
    /// <summary>
    /// 业务经理的已收款查询界面
    /// </summary>
    public partial class ReceivedList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var orderStatus = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.OrderStatus>()
        .Where(item => item.Key == Needs.Ccs.Services.Enums.OrderStatus.Declared.GetHashCode().ToString()
        || item.Key == Needs.Ccs.Services.Enums.OrderStatus.Completed.GetHashCode().ToString()
        || item.Key == Needs.Ccs.Services.Enums.OrderStatus.WarehouseExited.GetHashCode().ToString()).Select(item => new { item.Key, item.Value });
            this.Model.OrderStatus = orderStatus.Json();
        }

        protected void data()
        {
            string ClientCode = Request.QueryString["ClientCode"];
            string OrderId = Request.QueryString["OrderId"];
            string OrderStatus = Request.QueryString["OrderStatus"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            var salesmanID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;

            var commissionProportions = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.CommissionProportions.AsQueryable()
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal).ToArray();

            var orderReceipts = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderReceived.AsQueryable();

            //只能看当前业务员的信息
            orderReceipts = orderReceipts.Where(o => o.Client.ServiceManager.ID == salesmanID);
           

            if (!string.IsNullOrEmpty(ClientCode))
            {
                orderReceipts = orderReceipts.Where(o => o.Client.ClientCode == ClientCode.Trim());
            }
            if (!string.IsNullOrEmpty(OrderId))
            {
                orderReceipts = orderReceipts.Where(o => o.ID == OrderId.Trim());

            }
            if (!string.IsNullOrEmpty(OrderStatus))
            {
                int status = Int32.Parse(OrderStatus);
                orderReceipts = orderReceipts.Where(t => t.OrderStatus == (Needs.Ccs.Services.Enums.OrderStatus)status);
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

            orderReceipts = orderReceipts.OrderByDescending(item => item.DDate);

            Func<Needs.Ccs.Services.Models.OrderReceiptStats, object> convert = t => new
            {
                ClientCode = t.Client.ClientCode,
                CompanyName = t.Client.Company.Name,
                OrderId = t.ID,
                OrderStatus = t.OrderStatus.GetDescription(),
                DeclareDate = t.DDate?.ToShortDateString(),
                DeclarePrice = t.RMBDeclarePrice.ToRound(2).ToString("0.00"),
                Received = t.Received,
                Profit=t.Profit.ToString("0.00"),
                CommissionValue = GetCommissionDisplay(t.Profit, t.Client.CreateDate, commissionProportions).Item2,
                Commission = GetCommissionDisplay(t.Profit, t.Client.CreateDate, commissionProportions).Item1,
            };

            this.Paging(orderReceipts, convert);
        }

        /// <summary>
        /// 获取提成显示
        /// </summary>
        /// <param name="clientCreateDate"></param>
        /// <param name="commissionProportions"></param>
        /// <returns></returns>
        private Tuple<string, decimal> GetCommissionDisplay(decimal profit, DateTime clientCreateDate, Needs.Ccs.Services.Models.CommissionProportion[] commissionProportions)
        {
            decimal proportion = GetCommissionProportion(clientCreateDate, commissionProportions);
            decimal commission = profit * proportion;
            string commissionDisplay = commission.ToString("0.00") + "  (" + proportion.ToString("0.00") + ")";  // + "<br>" + clientCreateDate.ToString("yyyy-MM-dd");

            Tuple<string, decimal> tup = new Tuple<string, decimal>(commissionDisplay, commission);
            return tup;
        }

        /// <summary>
        /// 获取比例
        /// </summary>
        /// <param name="clientCreateDate"></param>
        /// <param name="commissionProportions"></param>
        /// <returns></returns>
        private decimal GetCommissionProportion(DateTime clientCreateDate, Needs.Ccs.Services.Models.CommissionProportion[] commissionProportions)
        {
            if (commissionProportions == null || !commissionProportions.Any())
            {
                return 0;
            }

            commissionProportions = commissionProportions.OrderBy(t => t.RegeisterMonth).ToArray();

            int month = (DateTime.Now.Year - clientCreateDate.Year) * 12 + (DateTime.Now.Month - clientCreateDate.Month);

            int rightIndex = commissionProportions.Length - 1;

            for (int i = 0; i < commissionProportions.Length; i++)
            {
                if (month <= commissionProportions[i].RegeisterMonth)
                {
                    rightIndex = i;
                    break;
                }
            }

            return commissionProportions[rightIndex].Proportion;
        }

    }
}