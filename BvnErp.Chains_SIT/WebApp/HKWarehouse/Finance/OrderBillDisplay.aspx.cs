using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.HKWarehouse.Finance
{
    /// <summary>
    /// 已封箱入库通知-详情
    /// 香港库房
    /// </summary>
    public partial class OrderBillDisplay : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 数据加载
        /// </summary>
        protected void data()
        {
            string OrderID = Request.QueryString["OrderID"];
            var OrderWaybillView = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderWaybill;
            var data = OrderWaybillView.Where(item => item.OrderID == OrderID);
            Func<Needs.Ccs.Services.Models.OrderWaybill, object> convert = item => new
            {
                ID = item.ID,
                CompanyName = item.Carrier.Name,
                WaybillCode = item.WaybillCode,
                ArrivalTime = item.ArrivalDate.ToString("yyyy-MM-dd"),
            };
            Response.Write(new
            {
                rows = data.Select(convert).ToArray()
            }.Json());
        }

        /// <summary>
        /// PI文件
        /// </summary>
        protected void filedata()
        {
            string OrderID = Request.QueryString["OrderID"];
            var fileView = new Needs.Ccs.Services.Views.OrderFilesView();
            var data = fileView.Where(Item => Item.OrderID == OrderID && Item.FileType == FileType.OriginalInvoice);
            Func<OrderFile, object> convert = item => new
            {
                ID = item.ID,
                FileName = item.Name,
                FileFormat = item.FileFormat,
                URL = item.Url,
                WebUrl = FileDirectory.Current.FileServerUrl + "/" + item.Url.ToUrl(),//查看路径
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
            };
            Response.Write(new
            {
                rows = data.Select(convert).ToArray()
            }.Json());
        }

        /// <summary>
        /// 报关单DecHeads数据
        /// </summary>
        protected void decdata()
        {
            string OrderID = Request.QueryString["OrderID"];
            var decHeads = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead.Where(item => item.OrderID == OrderID);
            var TransModes = Needs.Wl.Admin.Plat.AdminPlat.BaseTransMode;
            Func<DecHead, object> convert = item => new
            {
                ID = item.ID,
                OrderID = item.OrderID,
                ContrNo = item.ContrNo,
                ConsigneeName = item.ConsigneeName,
                TransMode = TransModes.Where(t=>t.Code == item.TransMode).FirstOrDefault()?.Name,
                PackNo = item.PackNo,
                CreateDate = item.CreateTime.ToString("yyyy-MM-dd"),
            };
            Response.Write(new
            {
                rows = decHeads.Select(convert).ToArray()
            }.Json());
        }

        /// <summary>
        /// 导出销货PI文件
        /// </summary>
        protected void ExportPIFile()
        {
            try
            {
                //报关单ID
                string ID = Request.Form["ID"];
                var decHead = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead.Where(item => item.ID == ID).FirstOrDefault();
                //报关单随附单据（发票）
                EdocRealation realation = decHead.EdocRealations.Where(item => item.EdocCode == "00000001").FirstOrDefault();
                string url = FileDirectory.Current.FileServerUrl + "/" + realation.FileUrl.ToUrl();//查看路径 
                Response.Write((new { success = true, message = "导出成功", url = url }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败：" + ex.Message }).Json());
            }
        }
    }
}