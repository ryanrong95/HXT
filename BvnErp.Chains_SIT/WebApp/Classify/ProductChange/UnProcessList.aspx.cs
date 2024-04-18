using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services.Enums;

namespace WebApp.Classify.Outside
{
    public partial class BeforeDecChangeList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int page, rows;
            int.TryParse(Request.QueryString["PageNumber"], out page);
            int.TryParse(Request.QueryString["PageSize"], out rows);

            var currentSc = new CurrentSc()
            {
                InitUrl = Request.QueryString["InitUrl"] ?? string.Empty,
                PageNumber = page,
                PageSize = rows,
                OrderID = Convert.ToString(Request.QueryString["OrderID"]) ?? string.Empty,
                ClientCode = Convert.ToString(Request.QueryString["ClientCode"]) ?? string.Empty,
                ProductChangeAddTimeBegin = Convert.ToString(Request.QueryString["ProductChangeAddTimeBegin"]) ?? string.Empty,
                ProductChangeAddTimeEnd = Convert.ToString(Request.QueryString["ProductChangeAddTimeEnd"]) ?? string.Empty,
            };

            this.Model.CurrentSc = currentSc.Json();
        }

        protected void data()
        {
            string ClientCode = Request.QueryString["ClientCode"];
            string OrderId = Request.QueryString["OrderID"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            var productList = new Needs.Ccs.Services.Views.OrderItemChangeNoticesView();
            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.OrderItemChangeNotice, bool>> expression = t => t.ProcessState == ProcessState.UnProcess;

            if (!string.IsNullOrEmpty(ClientCode))
            {
                var orderids = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders.Where(item => item.Client.ClientCode == ClientCode)
                    .Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.OrderItemChangeNotice, bool>> lambda1 = t => orderids.Contains(t.OrderID);
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(OrderId))
            {
                Expression<Func<Needs.Ccs.Services.Models.OrderItemChangeNotice, bool>> lambda1 = t => t.OrderID == OrderId.Trim();
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                var from = DateTime.Parse(StartDate);
                Expression<Func<Needs.Ccs.Services.Models.OrderItemChangeNotice, bool>> lambda1 = t => t.CreateDate >= from;
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                var to = DateTime.Parse(EndDate).AddDays(1);
                Expression<Func<Needs.Ccs.Services.Models.OrderItemChangeNotice, bool>> lambda1 = t => t.CreateDate < to;
                lamdas.Add(lambda1);
            }

            #region 页面需要数据
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var products = productList.GetPageList(page, rows, expression, lamdas.ToArray());

            Response.Write(new
            {
                rows = products.Select(
                    item => new
                    {
                        OrderID = item.OrderID,
                        OrderItemID = item.OrderItemID,
                        ClientCode = item.ClientCode,
                        CompanyName = item.CompanyName,
                        ProductName = item.ProductName,
                        ProductModel = item.ProductModel,
                        Type = item.Type.GetDescription(),
                        Date = item.CreateDate.ToString().Replace("T", ""),
                        ProcessState = item.ProcessState.GetDescription(),
                        LockStatus = item.IsLocked ? "已锁定" : "未锁定",
                        Locker = item.Locker?.RealName ?? "--",
                        LockTime = item.LockDate?.ToString().Replace("T", " ") ?? "--",
                        IsCanClassify = !item.IsLocked || (item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID),
                        IsCanUnlock = item.IsLocked && item.Locker.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID
                    }
                ).ToArray(),
                total = products.Total,
            }.Json());
            #endregion
        }

        //protected void data()
        //{
        //    string ClientCode = Request.QueryString["ClientCode"];
        //    string OrderId = Request.QueryString["OrderID"];
        //    string StartDate = Request.QueryString["StartDate"];
        //    string EndDate = Request.QueryString["EndDate"];

        //    var unProcessedList = new Needs.Ccs.Services.Views.ProductChangeUnProcessListView();
        //    List<Func<Needs.Ccs.Services.Views.ProductChangeUnProcessListView.ProductChangeUnProcessListModel, bool>> funcs 
        //        = new List<Func<Needs.Ccs.Services.Views.ProductChangeUnProcessListView.ProductChangeUnProcessListModel, bool>>();

        //    if (!string.IsNullOrEmpty(ClientCode))
        //    {
        //        var orderids = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders.Where(item => item.Client.ClientCode == ClientCode)
        //            .Select(item => item.ID).ToArray();
        //        Func<Needs.Ccs.Services.Views.ProductChangeUnProcessListView.ProductChangeUnProcessListModel, bool> func1 = t => orderids.Contains(t.OrderID);
        //        funcs.Add(func1);
        //    }
        //    if (!string.IsNullOrEmpty(OrderId))
        //    {
        //        Func<Needs.Ccs.Services.Views.ProductChangeUnProcessListView.ProductChangeUnProcessListModel, bool> func1 = t => t.OrderID == OrderId.Trim();
        //        funcs.Add(func1);
        //    }
        //    if (!string.IsNullOrEmpty(StartDate))
        //    {
        //        var from = DateTime.Parse(StartDate);
        //        Func<Needs.Ccs.Services.Views.ProductChangeUnProcessListView.ProductChangeUnProcessListModel, bool> func1 = t => t.CreateDate >= from;
        //        funcs.Add(func1);
        //    }
        //    if (!string.IsNullOrEmpty(EndDate))
        //    {
        //        var to = DateTime.Parse(EndDate).AddDays(1);
        //        Func<Needs.Ccs.Services.Views.ProductChangeUnProcessListView.ProductChangeUnProcessListModel, bool> func1 = t => t.CreateDate < to;
        //        funcs.Add(func1);
        //    }

        //    #region 页面需要数据

        //    int page, rows;
        //    int.TryParse(Request.QueryString["page"], out page);
        //    int.TryParse(Request.QueryString["rows"], out rows);

        //    int totalCount = 0;

        //    var products = unProcessedList.GetResult(out totalCount, page, rows, funcs.ToArray());

        //    Response.Write(new
        //    {
        //        rows = products.Select(
        //            item => new
        //            {
        //                OrderID = item.OrderID,
        //                OrderItemID = item.OrderItemID,
        //                ClientCode = item.ClientCode,
        //                CompanyName = item.CompanyName,
        //                ProductName = item.ProductName,
        //                ProductModel = item.ProductModel,
        //                Type = item.Types,
        //                Date = item.CreateDate.ToString().Replace("T", ""),
        //                ProcessState = Needs.Ccs.Services.Enums.ProcessState.UnProcess.GetDescription(),
        //                LockStatus = item.IsLocked ? "已锁定" : "未锁定",
        //                Locker = !string.IsNullOrEmpty(item.LockerName) ? item.LockerName : "--",
        //                LockTime = item.LockTime?.ToString().Replace("T", " ") ?? "--",
        //                IsCanClassify = !item.IsLocked || (item.IsLocked && item.LockerID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID),
        //                IsCanUnlock = item.IsLocked && item.LockerID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID
        //            }
        //        ).ToArray(),
        //        total = totalCount,
        //    }.Json());

        //    #endregion
        //}

        /// <summary>
        /// 归类锁定
        /// </summary>
        protected void Lock()
        {
            try
            {
                var id = Request.Form["ID"];
                var classifyProduct = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.ClassifyProductsAll[id];
                if (classifyProduct.IsLocked && classifyProduct.Locker.ID != Needs.Wl.Admin.Plat.AdminPlat.Current.ID)
                {
                    Response.Write((new { success = false, message = "当前产品归类已被锁定，锁定人【" + classifyProduct.Locker.RealName + "】，锁定时间【" + classifyProduct.LockDate + "】" }).Json());
                }
                else
                {
                    if (!classifyProduct.IsLocked)
                    {
                        classifyProduct.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                        var classify = ClassifyFactory.Create(ClassifyStep.ReClassify, classifyProduct);
                        classify.Lock();
                    }
                    Response.Write((new { success = true, message = "" }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = ex.Message }).Json());
            }
        }

        /// <summary>
        /// 解除归类锁定
        /// </summary>
        protected void UnLock()
        {
            try
            {
                var id = Request.Form["ID"];
                var classifyProduct = Needs.Wl.Admin.Plat.AdminPlat.Current.Classify.ClassifyProductsAll[id];
                classifyProduct.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var classify = ClassifyFactory.Create(ClassifyStep.ReClassify, classifyProduct);
                classify.UnLock();

                Response.Write((new { success = true, message = "已解除产品归类锁定" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = ex.Message }).Json());
            }
        }
    }
}