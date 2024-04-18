using Needs.Ccs.Services;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services.Enums;
using System.Linq.Expressions;

namespace WebApp.Order.Packing
{
    /// <summary>
    /// 分批到货订单查询界面
    /// 显示客户已确认的订单
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
        }
        protected void LoadComboBoxData()
        {
            this.Model.Clients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.Where(c => c.ClientType == ClientType.Internal).Select(c => new { c.ID, c.Company.Name }).Json();
            var orderStatus = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.OrderStatus>().Select(item => new { item.Key, item.Value });
            this.Model.OrderStatus = orderStatus.Json();
        }

        ///// <summary>
        ///// 初始化订单数据
        ///// </summary>
        //protected void data()
        //{
        //    string orderID = Request.QueryString["OrderID"];
        //    string clientCode = Request.QueryString["ClientCode"];
        //    string startDate = Request.QueryString["StartDate"];
        //    string endDate = Request.QueryString["EndDate"];
        //    string orderStatus = Request.QueryString["OrderStatus"];
        //    var clientID = Request.QueryString["ClientID"];
        //    var type = Request.QueryString["ClientType"];

        //    var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrderPackings1;

        //    List<LambdaExpression> lambdas = new List<LambdaExpression>();
        //    Expression<Func<Needs.Ccs.Services.Models.OrderPacking, bool>> expression = item => true;

        //    #region 查询条件
        //    if (!string.IsNullOrEmpty(orderID))
        //    {
        //        Expression<Func<Needs.Ccs.Services.Models.OrderPacking, bool>> lambda = item => item.ID == orderID.Trim();
        //        lambdas.Add(lambda);
        //    }
        //    if (!string.IsNullOrEmpty(clientCode))
        //    {
        //        var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(item => item.ClientCode == clientCode.Trim()).Select(item => item.ID).ToArray();
        //        Expression<Func<Needs.Ccs.Services.Models.OrderPacking, bool>> lambda = item => clientIds.Contains(item.ClientID);
        //        lambdas.Add(lambda);
        //    }
        //    if (!string.IsNullOrEmpty(startDate))
        //    {
        //        var from = DateTime.Parse(startDate);
        //        Expression<Func<Needs.Ccs.Services.Models.OrderPacking, bool>> lambda = item => item.CreateDate >= from;
        //        lambdas.Add(lambda);
        //    }
        //    if (!string.IsNullOrEmpty(endDate))
        //    {
        //        var to = DateTime.Parse(endDate);
        //        Expression<Func<Needs.Ccs.Services.Models.OrderPacking, bool>> lambda = item => item.CreateDate < to.AddDays(1);
        //        lambdas.Add(lambda);
        //    }
        //    if (!string.IsNullOrEmpty(type))
        //    {
        //        int itype = Int32.Parse(type);
        //        var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(item => item.ClientType == (ClientType)itype).Select(item => item.ID).ToArray();
        //        Expression<Func<Needs.Ccs.Services.Models.OrderPacking, bool>> lambda = item => clientIds.Contains(item.ClientID);
        //        lambdas.Add(lambda);
        //    }
        //    if (!string.IsNullOrEmpty(clientID))
        //    {
        //        Expression<Func<Needs.Ccs.Services.Models.OrderPacking, bool>> lambda = item => item.ClientID == clientID.Trim();
        //        lambdas.Add(lambda);
        //    }
        //    #endregion

        //    #region 页面需要数据
        //    int page, rows;
        //    int.TryParse(Request.QueryString["page"], out page);
        //    int.TryParse(Request.QueryString["rows"], out rows);
        //    var orderlist = orders.GetPageList(page, rows, expression, lambdas.ToArray());

        //    Response.Write(new
        //    {
        //        rows = orderlist.Select(
        //                order => new
        //                {
        //                    order.ID,
        //                    ClientCode = order.Client.ClientCode,
        //                    ClientName = order.Client.Company.Name,
        //                    DeclarePrice = order.DeclarePrice.ToRound(2).ToString("0.00"),
        //                    SupplierName = order.OrderConsignee.ClientSupplier.ChineseName,
        //                    HKWay = order.OrderConsignee.Type.GetDescription(),
        //                    Merchandiser = order.Client.Merchandiser.RealName,
        //                    CreateDate = order.CreateDate.ToShortDateString(),
        //                    PackingStatus = order.HasPacking ? order.PackingStatus.GetDescription() : "待装箱"
        //                }
        //             ).ToArray(),
        //        total = orderlist.Total,
        //    }.Json());
        //    #endregion
        //}

        /// <summary>
        /// 初始化订单数据
        /// </summary>
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string orderID = Request.QueryString["OrderID"];
            string clientCode = Request.QueryString["ClientCode"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];
            string orderStatus = Request.QueryString["OrderStatus"];
            var clientID = Request.QueryString["ClientID"];
            var type = Request.QueryString["ClientType"];

            List<LambdaExpression> lambdas = new List<LambdaExpression>();

            #region 查询条件
            if (!string.IsNullOrEmpty(orderID))
            {
                Expression<Func<Needs.Ccs.Services.Views.OrderPackingListModel, bool>> lambda = item => item.OrderID == orderID.Trim();
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(clientCode))
            {
                Expression<Func<Needs.Ccs.Services.Views.OrderPackingListModel, bool>> lambda = item => item.ClientCode == clientCode;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                var from = DateTime.Parse(startDate);
                Expression<Func<Needs.Ccs.Services.Views.OrderPackingListModel, bool>> lambda = item => item.OrderCreateDate >= from;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                var to = DateTime.Parse(endDate);
                Expression<Func<Needs.Ccs.Services.Views.OrderPackingListModel, bool>> lambda = item => item.OrderCreateDate < to.AddDays(1);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(type))
            {
                int itype = Int32.Parse(type);
                Expression<Func<Needs.Ccs.Services.Views.OrderPackingListModel, bool>> lambda = item => item.ClientType == (Needs.Ccs.Services.Enums.ClientType)itype;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(clientID))
            {
                Expression<Func<Needs.Ccs.Services.Views.OrderPackingListModel, bool>> lambda = item => item.ClientID == clientID.Trim();
                lambdas.Add(lambda);
            }
            #endregion



            int nCount = 0;
            var view = new Needs.Ccs.Services.Views.OrderPackingViewNew();
            bool isSa = Needs.Wl.Admin.Plat.AdminPlat.Current.IsSa;
            string adminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
            List<Needs.Ccs.Services.Views.OrderPackingListModel> orders = view.GetPagedPackingList(isSa, adminID, lambdas.ToArray(), page, rows, out nCount);

            string[] orderIDs = orders.Select(t => t.OrderID).ToArray();
            var packingStatuses = view.GetPackingStatus(orderIDs);

            orders = (from order in orders
                      join packingStatus in packingStatuses on order.OrderID equals packingStatus.OrderID into packingStatuses2
                      from packingStatus in packingStatuses2.DefaultIfEmpty()
                      select new Needs.Ccs.Services.Views.OrderPackingListModel
                      {
                          OrderID = order.OrderID,
                          ClientType = order.ClientType,
                          ClientID = order.ClientID,
                          ClientCode = order.ClientCode,
                          CompanyName = order.CompanyName,
                          DeclarePrice = order.DeclarePrice,
                          Currency = order.Currency,
                          OrderCreateDate = order.OrderCreateDate,
                          ClientSupplierChineseName = order.ClientSupplierChineseName,
                          OrderConsigneeType = order.OrderConsigneeType,

                          HasPacking = packingStatus.HasPacking,
                          PackingStatus = packingStatus.PackingStatus,
                      }).ToList();

            Response.Write(new
            {
                rows = orders.Select(
                        order => new
                        {
                            ID = order.OrderID,
                            ClientCode = order.ClientCode,
                            ClientName = order.CompanyName,
                            DeclarePrice = order.DeclarePrice.ToRound(2).ToString("0.00"),
                            Currency = order.Currency,
                            SupplierName = order.ClientSupplierChineseName,
                            HKWay = order.OrderConsigneeType.GetDescription(),
                            CreateDate = order.OrderCreateDate.ToShortDateString(),
                            PackingStatus = order.HasPacking ? order.PackingStatus.GetDescription() : "待装箱",
                        }
                     ).ToArray(),
                total = nCount,
            }.Json());

        }

    }
}