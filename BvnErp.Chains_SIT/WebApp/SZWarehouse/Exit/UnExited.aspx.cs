using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SZWareHouse.Exit
{
    /// <summary>
    /// 出库通知-出库界面
    /// 深圳库房
    /// </summary>
    public partial class UnExited : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            //出库类型
            this.Model.ExitType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.ExitType>()
                .Select(item => new { item.Key, item.Value }).Json();
        }

        protected void data()
        {
            //新写法
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            //查询条件
            string ExitNoticeID = Request.QueryString["ExitNoticeID"];
            string OrderID = Request.QueryString["OrderID"];
            string CustomerCode = Request.QueryString["EntryNumber"];
            string ExitType = Request.QueryString["ExitType"];
            string ClientName = Request.QueryString["ClientName"];

            var predicate = PredicateBuilder.Create<Needs.Wl.Warehouse.Services.PageModels.SZUnExitedListViewNewModels>();

            if (!string.IsNullOrEmpty(ExitNoticeID))
            {
                ExitNoticeID = ExitNoticeID.Trim();
                predicate = predicate.And(item => item.ExitNoticeID.Contains(ExitNoticeID));
            }
            if (!string.IsNullOrEmpty(OrderID))
            {
                OrderID = OrderID.Trim();
                predicate = predicate.And(item => item.OrderID.Contains(OrderID));
            }
            if (!string.IsNullOrEmpty(ExitType))
            {
                predicate = predicate.And(item => item.ExitType == (Needs.Wl.Models.Enums.ExitType)int.Parse(ExitType));
            }
            if (!string.IsNullOrEmpty(CustomerCode))
            {
                CustomerCode = CustomerCode.Trim();
                predicate = predicate.And(item => item.ClientCode.Contains(CustomerCode));
            }
            if (!string.IsNullOrEmpty(ClientName))
            {
                ClientName = ClientName.Trim();
                predicate = predicate.And(item => item.ClientName.Contains(ClientName));
            }

            Needs.Wl.Warehouse.Services.Views.SZUnExitedListViewNew view = new Needs.Wl.Warehouse.Services.Views.SZUnExitedListViewNew();
            view.AllowPaging = true;
            view.PageIndex = page;
            view.PageSize = rows;
            //view.IsOnReadShips = true;
            view.Predicate = predicate;

            int recordCount = view.RecordCount;
            var exitNotices = view.ToList();
            

            Func<Needs.Wl.Warehouse.Services.PageModels.SZUnExitedListViewNewModels, object> convert = item => new
            {
                ID = item.ExitNoticeID,
                OrderID = item.OrderID,//订单编号
                ClientCode = item.ClientCode,
                ClientName = item.ClientName,//客户

                PackNo = item.PackNo,
                AdminName = item.AdminName,//制单人
                ExitType = item.ExitType.GetDescription(),
                NoticeStatus = item.ExitNoticeStatus.GetDescription(),
                IsPrint = item.IsPrint != null ? ((Needs.Ccs.Services.Enums.IsPrint)item.IsPrint).GetDescription() : string.Empty,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
            };

            Response.Write(new
            {
                rows = exitNotices.Select(convert).ToArray(),
                total = recordCount,
            }.Json());




            //int page, rows;
            //int.TryParse(Request.QueryString["page"], out page);
            //int.TryParse(Request.QueryString["rows"], out rows);

            ////查询条件
            //string ExitNoticeID = Request.QueryString["ExitNoticeID"];
            //string OrderID = Request.QueryString["OrderID"];
            //string CustomerCode = Request.QueryString["EntryNumber"];
            //string ExitType = Request.QueryString["ExitType"];
            //string ClientName = Request.QueryString["ClientName"];

            //List<LambdaExpression> lamdas = new List<LambdaExpression>();

            //if (!string.IsNullOrEmpty(ExitNoticeID))
            //{
            //    ExitNoticeID = ExitNoticeID.Trim();
            //    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.SZUnExitedListView.SZUnExitedListModel, bool>>)(t => t.ExitNoticeID.Contains(ExitNoticeID)));
            //}
            //if (!string.IsNullOrEmpty(OrderID))
            //{
            //    OrderID = OrderID.Trim();
            //    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.SZUnExitedListView.SZUnExitedListModel, bool>>)(t => t.OrderID.Contains(OrderID)));
            //}
            //if (!string.IsNullOrEmpty(ExitType))
            //{
            //    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.SZUnExitedListView.SZUnExitedListModel, bool>>)(t => t.ExitType == (ExitType)int.Parse(ExitType)));
            //}
            //if (!string.IsNullOrEmpty(CustomerCode))
            //{
            //    CustomerCode = CustomerCode.Trim();
            //    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.SZUnExitedListView.SZUnExitedListModel, bool>>)(t => t.ClientCode.Contains(CustomerCode)));
            //}
            //if (!string.IsNullOrEmpty(ClientName))
            //{
            //    ClientName = ClientName.Trim();
            //    lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.SZUnExitedListView.SZUnExitedListModel, bool>>)(t => t.ClientName.Contains(ClientName)));
            //}

            //int totalCount = 0;
            //var exitNotices = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZUnExitedListView.GetResult(out totalCount, page, rows, lamdas.ToArray());

            //Func<Needs.Ccs.Services.Views.SZUnExitedListView.SZUnExitedListModel, object> convert = item => new
            //{
            //    ID = item.ExitNoticeID,
            //    OrderID = item.OrderID,//订单编号
            //    ClientCode = item.ClientCode,
            //    ClientName = item.ClientName,//客户

            //    PackNo = item.PackNo,
            //    AdminName = item.AdminName,//制单人
            //    ExitType = item.ExitType.GetDescription(),
            //    NoticeStatus = item.ExitNoticeStatus.GetDescription(),
            //    IsPrint = item.IsPrint != null ? ((Needs.Ccs.Services.Enums.IsPrint)item.IsPrint).GetDescription() : string.Empty,
            //    CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
            //};

            //Response.Write(new
            //{
            //    rows = exitNotices.Select(convert).ToArray(),
            //    total = totalCount,
            //}.Json());
        }

    }
}