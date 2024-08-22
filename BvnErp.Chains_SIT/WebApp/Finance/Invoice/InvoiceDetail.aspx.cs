using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.Finance
{
    public partial class InvoiceDetail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }



        /// <summary>
        /// 加载开票通知
        /// </summary>
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string orderID = Request.QueryString["OrderID"];
            string comanyName = Request.QueryString["ComanyName"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];

            DateTime dtFlag = DateTime.Parse("2022-06-16");
            if (!string.IsNullOrEmpty(startDate))
            {
                var stDate = DateTime.Parse(startDate);
                if (stDate > dtFlag)
                {
                    using (var query = new Needs.Ccs.Services.Views.InvoiceXmlDetaiViewNew())
                    {
                        var view = query;

                        if (!string.IsNullOrEmpty(orderID))
                        {
                            orderID = orderID.Trim();
                            view = view.SearchByOrderID(orderID);
                        }
                        if (!string.IsNullOrEmpty(comanyName))
                        {
                            comanyName = comanyName.Trim();
                            //noticeitem = noticeitem.Where(t => t.Client.Company.Name.Contains(comanyName));
                            view = view.SearchByCompanyName(comanyName);
                        }
                        if (!string.IsNullOrEmpty(startDate))
                        {
                            var from = DateTime.Parse(startDate);
                            //noticeitem = noticeitem.Where(t => t.InvoiceTime >= from);
                            view = view.SearchByInvoiceTimeStartDate(from);
                        }
                        if (!string.IsNullOrEmpty(endDate))
                        {
                            var to = DateTime.Parse(endDate);
                            //noticeitem = noticeitem.Where(t => t.InvoiceTime < to.AddDays(1));
                            view = view.SearchByInvoiceTimeEndDate(to);
                        }

                        Response.Write(view.ToMyPage(page, rows).Json());
                    }
                }
                else
                {
                    using (var query = new Needs.Ccs.Services.Views.InvoiceDetaiViewNew())
                    {
                        var view = query;

                        if (!string.IsNullOrEmpty(orderID))
                        {
                            orderID = orderID.Trim();
                            view = view.SearchByOrderID(orderID);
                        }
                        if (!string.IsNullOrEmpty(comanyName))
                        {
                            comanyName = comanyName.Trim();
                            //noticeitem = noticeitem.Where(t => t.Client.Company.Name.Contains(comanyName));
                            view = view.SearchByCompanyName(comanyName);
                        }
                        if (!string.IsNullOrEmpty(startDate))
                        {
                            var from = DateTime.Parse(startDate);
                            //noticeitem = noticeitem.Where(t => t.InvoiceTime >= from);
                            view = view.SearchByInvoiceTimeStartDate(from);
                        }
                        if (!string.IsNullOrEmpty(endDate))
                        {
                            var to = DateTime.Parse(endDate);
                            //noticeitem = noticeitem.Where(t => t.InvoiceTime < to.AddDays(1));
                            view = view.SearchByInvoiceTimeEndDate(to);
                        }

                        Response.Write(view.ToMyPage(page, rows).Json());
                    }
                }
            }
            else
            {
                using (var query = new Needs.Ccs.Services.Views.InvoiceXmlDetaiViewNew())
                {
                    var view = query;

                    if (!string.IsNullOrEmpty(orderID))
                    {
                        orderID = orderID.Trim();
                        view = view.SearchByOrderID(orderID);
                    }
                    if (!string.IsNullOrEmpty(comanyName))
                    {
                        comanyName = comanyName.Trim();
                        //noticeitem = noticeitem.Where(t => t.Client.Company.Name.Contains(comanyName));
                        view = view.SearchByCompanyName(comanyName);
                    }
                    if (!string.IsNullOrEmpty(startDate))
                    {
                        var from = DateTime.Parse(startDate);
                        //noticeitem = noticeitem.Where(t => t.InvoiceTime >= from);
                        view = view.SearchByInvoiceTimeStartDate(from);
                    }
                    if (!string.IsNullOrEmpty(endDate))
                    {
                        var to = DateTime.Parse(endDate);
                        //noticeitem = noticeitem.Where(t => t.InvoiceTime < to.AddDays(1));
                        view = view.SearchByInvoiceTimeEndDate(to);
                    }

                    Response.Write(view.ToMyPage(page, rows).Json());
                }
            }

            
        }

        #region 导出Excel文件

        protected void Export()
        {
            string startDate = Request.Form["StartDate"];
            if (!string.IsNullOrEmpty(startDate))
            {
                var stDate = DateTime.Parse(startDate);
                DateTime dtFlag = DateTime.Parse("2022-06-16");
                if (stDate > dtFlag)
                {
                    ExportByXml();
                }
                else
                {
                    ExportByNoticeItem();
                }
            }
            else
            {
                ExportByXml();
            }
        }

        /// <summary>
        /// 导出发票信息
        /// </summary>
        protected void ExportByNoticeItem()
        {

            string orderID = Request.Form["OrderID"];
            string comanyName = Request.Form["ComanyName"];
            string startDate = Request.Form["StartDate"];
            string endDate = Request.Form["EndDate"];

            var noticeitem = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceDetail.AsQueryable();

            if (!string.IsNullOrEmpty(orderID))
            {
                orderID = orderID.Trim();
                noticeitem = noticeitem.Where(t => t.OrderID.Contains(orderID));
            }
            if (!string.IsNullOrEmpty(comanyName))
            {
                comanyName = comanyName.Trim();
                noticeitem = noticeitem.Where(t => t.Client.Company.Name.Contains(comanyName));
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                var from = DateTime.Parse(startDate);
                noticeitem = noticeitem.Where(t => t.InvoiceTime >= from);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                var to = DateTime.Parse(endDate);
                noticeitem = noticeitem.Where(t => t.InvoiceTime < to.AddDays(1));
            }


            //前台显示
            Func<InvoiceNoticeItem, object> convert = item => new
            {
                TimeNow = DateTime.Now.ToString("yyyy/MM/dd"),
                //ID = item.ID,
                item.OrderID,
                ProductName = item.OrderItem == null ? "*经纪代理服务*经纪代理" : item.OrderItem.Category.TaxName,  //产品名称
                ProductModel = item.OrderItem?.Model == null ? "" : item.OrderItem?.Model,//型号
                //Unit = item.OrderItem?.Unit == null ? "" : item.OrderItem?.Unit,//计量单位
                Unit = item.UnitName,//单位
                SalesUnitPrice =item.DetailSalesUnitPrice, //不含税价格
                Quantity = item.OrderItem == null ? 1 : item.OrderItem.Quantity,//数量
                SalesTotalPrice=item.DetailSalesTotalPrice, //不含税金额
                UnitPrice=item.DetailUnitPrice, //含税单价
                Amount = (item.Amount + item.Difference).ToRound(2),//含税总额
                item.Client.Company.Name,
                InvoiceNo = item.InvoiceNo == null ? "" : item.InvoiceNo,
                UpdateDate = item.InvoiceTime?.ToString("yyyy/MM/dd"),
                TaxAmount = (item.DetailSalesTotalPrice * item.InvoiceTaxRate).ToRound(2),
            };

            //写入数据
            DataTable dt = NPOIHelper.JsonToDataTable(noticeitem.Select(convert).ToArray().Json());

            var fileName = DateTime.Now.Ticks + ".xlsx";

            //创建文件夹
            FileDirectory file = new FileDirectory(fileName);
            file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
            file.CreateDataDirectory();

            NPOIHelper.NPOIExcel(dt, file.FilePath);

            Response.Write((new
            {
                success = true,
                message = "导出成功",
                url = file.FileUrl
            }).Json());

        }

    
        protected void ExportByXml()
        {
            string orderID = Request.Form["OrderID"];
            string comanyName = Request.Form["ComanyName"];
            string startDate = Request.Form["StartDate"];
            string endDate = Request.Form["EndDate"];

            var noticeitem = new Needs.Ccs.Services.Views.InvoiceXmlDetaiView().AsQueryable();

            if (!string.IsNullOrEmpty(orderID))
            {
                orderID = orderID.Trim();
                noticeitem = noticeitem.Where(t => t.OrderID.Contains(orderID));
            }
            if (!string.IsNullOrEmpty(comanyName))
            {
                comanyName = comanyName.Trim();
                noticeitem = noticeitem.Where(t => t.Client.Company.Name.Contains(comanyName));
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                var from = DateTime.Parse(startDate);
                noticeitem = noticeitem.Where(t => t.InvoiceTime >= from);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                var to = DateTime.Parse(endDate);
                noticeitem = noticeitem.Where(t => t.InvoiceTime < to.AddDays(1));
            }


            //前台显示
            Func<InvoiceNoticeItem, object> convert = item => new
            {
                TimeNow = DateTime.Now.ToString("yyyy/MM/dd"),
                //ID = item.ID,
                item.OrderID,
                ProductName = item.OrderItem == null ? "*经纪代理服务*经纪代理" : item.OrderItem.Category.TaxName,  //产品名称
                ProductModel = item.OrderItem?.Model == null ? "" : item.OrderItem?.Model,//型号
                //Unit = item.OrderItem?.Unit == null ? "" : item.OrderItem?.Unit,//计量单位
                Unit = item.UnitName,//单位
                SalesUnitPrice = item.UnitPrice, //不含税价格
                //Quantity = item.OrderItem == null ? 1 : item.OrderItem.Quantity,//数量
                Quantity = item.InvoiceQty,
                SalesTotalPrice = item.TaxFreeAmout, //不含税金额
                UnitPrice = ((item.TaxFreeAmout + item.Tax) /item.Amount).ToRound(4), //含税单价
                Amount = item.TaxFreeAmout+item.Tax,//含税总额
                item.Client.Company.Name,
                InvoiceNo = item.InvoiceNo == null ? "" : item.InvoiceNo,
                UpdateDate = item.InvoiceTime?.ToString("yyyy/MM/dd"),
                TaxAmount = item.Tax
            };

            //写入数据
            DataTable dt = NPOIHelper.JsonToDataTable(noticeitem.Select(convert).ToArray().Json());

            var fileName = DateTime.Now.Ticks + ".xlsx";

            //创建文件夹
            FileDirectory file = new FileDirectory(fileName);
            file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
            file.CreateDataDirectory();

            NPOIHelper.NPOIExcel(dt, file.FilePath);

            Response.Write((new
            {
                success = true,
                message = "导出成功",
                url = file.FileUrl
            }).Json());
        }
        #endregion
    }
}